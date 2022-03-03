# Z
## 简述
    基于akka.net的actor模型的分布式服务器框架.
    采用c#，前后端公用代码，前端可参与业务逻辑开发
## 特点
    1.支持逻辑更新(service,handler)和配置表热更新。
    2.actor同一时刻只会被1个线程持有,保证高性能和线程安全(非actor的异步线程方法通过ETTask回到actor线程)。
    3.支持水平扩容，无单点故障(主要指home,home容纳在线玩家的实体)
    4.高性能,性能指标参考下图

## 设计
    1.home运行PlayerActor的实体玩法逻辑(空闲的PlayerActor会在6分钟后保存数据并销毁)
    2.world等按需拆分多人玩法逻辑(如拆成legion arena world等)
    3.login负责http登录 验证 触发actor上线
    4.基于组件模型，组件模型分2层，3类
        4.1 GlobalComponent是最底层，全局的业务处理，如etcd，console，repl等
        4.2 GameComponent和PlayerComponent是第二层，如WorldActor，PlayerActor。
    5.因为要实现逻辑热更新，部分代码不优雅，持续迭代完善中

## 启动流程
    1.配置mongo(目前在AddDbComponent时候设置)
    2.配置etcd(目前在AddEtcdompoent设置)
    3.生成Boot Login.Hotfix World.Hotfix Home.Hotfix
        运行 ./Boot -t=Home -i=2
        运行 ./Boot -t=World -i=1
        运行 ./Boot -t=Login -i=0
    4.unity运行Client
        点击 获取所有角色列表
        点击 创建选择角色
        点击 链接tcp服务器
        点击 登录到home
        点击 获取邮件

## 简单使用教程
### 通信协议编辑和代码生成
    1.编辑inner.proto, inner.csv 内部通信协议
    2.编辑outer.proto, outer.csv 外部通信协议
    3.运行Proto2CS
### 实现handler(用player举例)
    1.HomeInnerHandlerDispatcherGen里有返回值的加在DispatcherWithResult 无返回值的加在DispatcherNoResult
    2.后续这里使用代码生成器生成
### 实现server
    1.参考PlayerService，所有的service使用静态扩展方法实现
### db数据
    1.参考BagState
    2.数据加载在Component的load里完成。通过BaseState的dirty定期保存到mongo
    3.因为有了actor做数据缓存，所以不在需要redis(redis后续可能会在一些如http获取好友列表的显示数据)
### 热更新
    1.控制台直接输入reaload即可更新逻辑
    2.控制台输入table即可更新表格
### todo
    1.目前已有EtcdComponent,未来的表格,hotfix.dll会保存在etcd上,自动热更新


## 登录流程介绍
    1.client请求LoginServer获得角色列表
    2.client请求LoginServer登录或者创建角色
        2.1 LoginServer请求Home分配Actor实体并返回tcp地址以及token
    3.client链接上一步获得的地址，发送登录home请求
    4.✅
