﻿using System.Collections.Generic;
using System.Net;

namespace Base.Helper;

public static class NetHelper
{
    public static string[] GetAddressIPs()
    {
        //获取本地的IP地址
        var addressIPs = new List<string>();
        foreach (var address in Dns.GetHostEntry(Dns.GetHostName()).AddressList) addressIPs.Add(address.ToString());

        return addressIPs.ToArray();
    }
}