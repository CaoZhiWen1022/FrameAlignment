using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using Google.Protobuf;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Proto;

public class GameSocketMgr : MonoBehaviour
{
    private static GameSocketMgr _instance;
    public static GameSocketMgr Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("GameSocketMgr");
                _instance = go.AddComponent<GameSocketMgr>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    [SerializeField] private string _wsServerUrl = "ws://localhost:3000";

    // 客户端WebSocket实例
    private ClientWebSocket _wsClient;
    // 取消令牌（用于终止异步任务）
    private CancellationTokenSource _cts;



    public async Task InitSocket()
    {
        await ConnectToWsServerAsync();
    }

    /// <summary>
    /// 异步连接WS服务器
    /// </summary>
    private async Task ConnectToWsServerAsync()
    {
        // 初始化
        _wsClient = new ClientWebSocket();
        _cts = new CancellationTokenSource();

        // 1. 发起异步连接
        Debug.Log("正在连接WS服务器...");
        await _wsClient.ConnectAsync(new Uri(_wsServerUrl), _cts.Token);
        Debug.Log("WS服务器连接成功！");

        // 2. 连接成功后，启动接收消息循环（异步）
        ReceiveWsMessageLoopAsync();
    }

    /// <summary>
    /// 异步发送文本消息到WS服务器（保持兼容）
    /// </summary>
    /// <param name="message">要发送的字符串消息</param>
    public async void SendWsMessageAsync(string message)
    {
        if (_wsClient == null || _wsClient.State != WebSocketState.Open)
        {
            Debug.LogError("WS连接未建立，无法发送消息！");
            return;
        }

        if (string.IsNullOrEmpty(message))
        {
            Debug.LogWarning("发送的消息不能为空！");
            return;
        }

        try
        {
            // 转换字符串为字节数组（UTF-8编码）
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            // 异步发送消息
            await _wsClient.SendAsync(new ArraySegment<byte>(messageBytes),
                WebSocketMessageType.Text, true, _cts.Token);
            Debug.Log($"已发送消息到WS服务器：{message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"发送消息失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 使用 Protobuf IMessage 发送二进制消息到 WS 服务器
    /// </summary>
    public async void Send(IMessage message)
    {
        if (_wsClient == null || _wsClient.State != WebSocketState.Open)
        {
            Debug.LogError("WS连接未建立，无法发送Proto消息！");
            return;
        }

        if (message == null)
        {
            Debug.LogWarning("发送的Proto消息不能为空！");
            return;
        }

        byte[] messageBytes = message.ToByteArray();
        await _wsClient.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Binary, true, _cts.Token);
    }

    /// <summary>
    /// 异步接收消息循环（持续监听服务器消息）
    /// </summary>
    private async void ReceiveWsMessageLoopAsync()
    {
        // 使用流式接收以支持分片消息
        byte[] buffer = new byte[4096];

        while (_wsClient.State == WebSocketState.Open && !_cts.Token.IsCancellationRequested)
        {
            try
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await _wsClient.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await _wsClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "服务器关闭连接", CancellationToken.None);
                            return;
                        }

                        ms.Write(buffer, 0, result.Count);
                    } while (!result.EndOfMessage);

                    var messageBytes = ms.ToArray();

                    HandleBinaryMessage(messageBytes);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"接收消息失败：{ex.Message}");
                break;
            }
        }
    }

    private void HandleBinaryMessage(byte[] data)
    {
        // 尝试以已知的 Proto 类型解析，顺序按照最可能的类型
        // RegisterResponse
        var reg = Proto.CommonResponse.Parser.ParseFrom(data);
        if (reg != null && reg.Base != null)
        {
            //Debug.Log("收到 Response 消息:" + reg.Base.Id);
            //判断状态码
            if (reg.Base.Code != Proto.StatusCode.StatusSuccess)
            {
                Debug.LogWarning(reg.Base.Id + "：状态异常：" + reg.Base.Code + "异常日志：" + reg.Base.Message);
            }
            else
            {
                Action<byte[]> action = SocketHandler.Instance.getHandler(reg.Base.Id);
                if (action != null)
                {
                    action.Invoke(data);
                }
                else
                {
                    Debug.LogWarning(reg.Base.Id + "：回调未注册");
                }
            }
        }
    }


    /// <summary>
    /// 主动断开WS连接
    /// </summary>
    public void DisconnectWsServer()
    {
        if (_cts != null && !_cts.IsCancellationRequested)
        {
            _cts.Cancel(); // 取消异步任务
            Debug.Log("主动断开WS连接");
        }
    }

    // 场景销毁/对象销毁时，断开连接
    private void OnDestroy()
    {
        DisconnectWsServer();
    }

}
