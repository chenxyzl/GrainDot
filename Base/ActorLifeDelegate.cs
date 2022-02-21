using System.Threading.Tasks;

namespace Base;

//启动 加载db数据 ~只能处理序列化相关不要有业务逻辑
public delegate Task LoadDelegate();

//开始 第一个tick开始前  @param:crossDay 是否跨天
public delegate Task StartDelegate(bool crossDay);

//停止前
public delegate Task PreStopDelegate();

//停止
public delegate Task StopDelegate();
//
// //每一帧 @param:now 参数为服务器当前时间
// public delegate Task OnlineDelegate(bool newLogin, long lastLogoutTime);
//
// //下线
// public delegate Task OfflineDelegate();

//tick
public delegate Task TickDelegate(long dt);