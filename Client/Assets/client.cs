using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Linq;
using System.Threading;
using Message;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class client : MonoBehaviour
{
    private string staInfo = "NULL"; //状态信息
    private string inputIp = "127.0.0.1:15000"; //输入ip地址
    private string recMes = "NULL"; //接收到的消息
    private List<ulong> roleList = new List<ulong>();
    private ulong playerId => roleList.FirstOrDefault();
    private string loginKey;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        NetClient.Instance.ReceiveMsg();
    }

    //建立链接
    private void ClickConnect()
    {
        try
        {
            int _port = Convert.ToInt32(inputIp.Split(":")[1]); //获取端口号
            string _ip = inputIp.Split(":")[0]; //获取ip地址

            //断开老链接
            NetClient.Instance.Close();
            NetClient.Instance.Init(_ip, _port);
            NetClient.Instance.onRecMsg = OnRecMsg;
            Debug.Log("连接成功 , " + " ip = " + _ip + " port = " + _port);
            staInfo = _ip + ":" + _port + "  连接成功";
        }
        catch (Exception e)
        {
            Debug.Log($"IP或者端口号错误...... {e}");
            staInfo = "IP或者端口号错误......";
        }
    }

    private void DisConnect()
    {
        NetClient.Instance.Close();
        Debug.Log("断开成功");
        staInfo = "断开成功";
    }

    /// <summary>
    /// 向服务器发送消息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void Send(IRequest msg)
    {
        //第一层序列化

        //第二层序列化

        //写入2长度 大端

        //写入内容

        //发送
    }

    void dealHttpResult(string url, string result)
    {
        var data = Convert.FromBase64String(result);
        var msg = SerializeHelper.FromBinary<ApiResult>(data);
        if (msg.Code != Code.Ok)
        {
            Debug.Log($"code:{msg.Code}, please check");
            return;
        }

        switch (url)
        {
            case "/api/rolelist":
            {
                var res = SerializeHelper.FromBinary<A2CGetRoleList>(msg.Content);
                roleList.Clear();
                foreach (var simpleRole in res.Rols)
                {
                    roleList.Add(simpleRole.Uid);
                }

                recMes = string.Join(",", roleList.ToArray());
                break;
            }
            case "/api/login":
            {
                var res = SerializeHelper.FromBinary<A2CRoleLogin>(msg.Content);
                loginKey = res.Key;
                inputIp = res.Addr;
                recMes = loginKey;
                break;
            }
        }
    }

    IEnumerator GetRequest(string url, IHttpRequest req)
    {
        var data = Convert.ToBase64String(req.ToBinary());
        //UnityWebRequest 会对post的data进入url编码
        UnityWebRequest uwr = UnityWebRequest.Put($"http://127.0.0.1:20001{url}", data);
        yield return uwr.SendWebRequest();
        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
            dealHttpResult(url, uwr.downloadHandler.text);
        }
    }

    IEnumerator GetRoleList()
    {
        yield return GetRequest("/api/rolelist", new C2AGetRoleList {Token = "xxx"});
    }

    IEnumerator LoginApi()
    {
        yield return GetRequest("/api/login", new C2ARoleLogin {Token = "xxx", Uid = playerId});
    }

    void LoginHome()
    {
        Send(new C2SLogin
        {
            PlayerId = playerId,
            Key = loginKey
        });
    }

    void AddGlod1()
    {
    }

    void CosetGold1()
    {
    }


    private void OnDisable()
    {
        Debug.Log("begin OnDisable()");
        NetClient.Instance.Close();
        Debug.Log("end OnDisable()");
    }

    //用户界面
    void OnGUI()
    {
        GUI.color = Color.black;

        GUI.Label(new Rect(65, 10, 60, 20), "状态信息");

        GUI.Label(new Rect(135, 10, 80, 60), staInfo);

        GUI.Label(new Rect(65, 70, 50, 20), "服务器ip地址");

        inputIp = GUI.TextField(new Rect(125, 70, 100, 20), inputIp, 20);

        GUI.Label(new Rect(65, 110, 80, 20), "接收到消息：");

        GUI.Label(new Rect(155, 110, 300, 60), recMes);

        if (GUI.Button(new Rect(65, 190, 100, 20), "开始连接"))
        {
            ClickConnect();
        }

        if (GUI.Button(new Rect(185, 190, 100, 20), "断开连接"))
        {
            DisConnect();
        }

        if (GUI.Button(new Rect(65, 230, 100, 20), "获取角色列表"))
        {
            StartCoroutine(GetRoleList());
        }

        if (GUI.Button(new Rect(185, 230, 100, 20), "选择或创建角色"))
        {
            StartCoroutine(LoginApi());
        }

        if (GUI.Button(new Rect(305, 230, 100, 20), "登录游戏服"))
        {
            LoginHome();
        }

        if (GUI.Button(new Rect(65, 270, 100, 20), "增加1金币"))
        {
            AddGlod1();
        }

        if (GUI.Button(new Rect(185, 270, 100, 20), "减少1金币"))
        {
            CosetGold1();
        }
    }

    void OnRecMsg(byte[] msg)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            ms.Write(msg, 0, msg.Length);
            ms.Position = 0;

            //第1层反序列化

            //第2层反序列化

            //显示
        }
    }
}