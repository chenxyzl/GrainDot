﻿namespace Common;

public enum ServerState
{
    Unknow = 0, //刚启动的状态
    Load = 1, //加载
    Runing = 2, //运行
    Stop = 4 //停止
}