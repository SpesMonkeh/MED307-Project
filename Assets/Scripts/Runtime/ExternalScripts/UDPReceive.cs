using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

[DisallowMultipleComponent]
public sealed class UDPReceive : MonoBehaviour
{
    [SerializeField] int port = 30707;
    [SerializeField] bool startReceiving = true;
    [SerializeField] bool printToConsole;
    [SerializeField] string dataStream;
    
    Thread receiveThread;
    UdpClient client;

    public string DataStream => dataStream;

    public void Start()
    {
        receiveThread = new Thread(ReceiveData)
        {
            IsBackground = true
        };
        receiveThread.Start();
    }

    void ReceiveData()
    {
        client = new UdpClient(port);
        while (startReceiving)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] dataByte = client.Receive(ref anyIP);
                dataStream = Encoding.UTF8.GetString(dataByte);

                if (printToConsole)
                {
                    print(dataStream);
                }
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

}
