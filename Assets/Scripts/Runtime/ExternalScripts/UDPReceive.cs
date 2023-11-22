using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using P307.Runtime.Hands;
using static P307.Shared.Const307;

[DisallowMultipleComponent]
public sealed class UDPReceive : MonoBehaviour
{
    [SerializeField] int port = 30707;
    [SerializeField] bool startReceiving = true;
    [SerializeField] bool printToConsole;
    [SerializeField] byte[] clientData;

    [SerializeField] bool resetHandOnIdle = true;
    [SerializeField, Range(ZERO, ONE_HUNDRED)] float timeBeforeIdleStart = 5f;
    [SerializeField, Range(ZERO, TEN)] int idleTimeBeforeReset = 5;
    [SerializeField] float currentTimeBeforeIdleStart;
    [SerializeField] float currentIdleTimeBeforeReset;
    [SerializeField] bool handIsIdle;

    bool resetHand;
    byte[] previousClientData = { };
    Thread receiveThread;
    UdpClient client;

    public string DataStream => clientData is not null && clientData.Length is not ZERO
        ? Encoding.UTF8.GetString(clientData)
        : string.Empty;

    public void StopReceiving()
    {
        startReceiving = false;
        clientData = Array.Empty<byte>();
    }

    public void StartReceiving() => startReceiving = true;
    public bool IsReceiving() => startReceiving && clientData.Length is not ZERO;
    
    public void Start()
    {
        resetHandOnIdle = idleTimeBeforeReset is not ZERO;
        client = new UdpClient(port);
        receiveThread = new Thread(ReceiveData)
        {
            IsBackground = true
        };
        
        receiveThread.Start();
    }

    void ReceiveData()
    {
        while (startReceiving)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                clientData = client.Receive(ref anyIP);

                if (printToConsole)
                    print(DataStream);
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    // TODO: Flyt alt herunder til et mere relevant script.
    void Update()
    {
        //EvaluateHandActivity();
    }

    bool CountUpToIdleStart() => CountUp(ref currentTimeBeforeIdleStart, timeBeforeIdleStart);

    bool CountUpToHandReset() => CountUp(ref currentIdleTimeBeforeReset, idleTimeBeforeReset);
    
    void EvaluateHandActivity()
    {
        if (resetHandOnIdle is false)
            return;
        if (CheckIfClientDataIsHasUpdated() is true)
            return;
        if (CountUpToIdleStart() is false)
            return;
        
        handIsIdle = true;
        if (CountUpToHandReset() is false)
            return;
        
        ResetHand();
    }

    void ResetHand()
    {
        if (resetHand)
            return;
        resetHand = true;
        HandTracking.resetHand?.Invoke(resetHand);
    }
    
    bool CheckIfClientDataIsHasUpdated()
    {
        if (clientData == previousClientData)
            return false;
            
        handIsIdle = false;
        currentIdleTimeBeforeReset = ZERO;
        currentTimeBeforeIdleStart = ZERO;
        previousClientData = clientData;
        
        if (resetHand is false)
            return true;
            
        resetHand = false;
        HandTracking.resetHand?.Invoke(resetHand);
        return true;
    }
    
    static bool CountUp(ref float currentTime, float stopTime)
    {
        if (Mathf.Approximately(currentTime, stopTime))
            return true;
        currentTime += Time.deltaTime;
        currentTime = Mathf.Min(currentTime, stopTime);
        return false;
    }
}
