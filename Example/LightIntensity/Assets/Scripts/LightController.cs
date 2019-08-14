using System;
using SerialHandlerLib;
using UnityEngine;

public class LightController : MonoBehaviour
{
    private SerialHandler serialHandler;
    private Light lightComponent;
    private float intensity;

    void Start()
    {
        lightComponent = GetComponent<Light>();
        serialHandler = new SerialHandler();
        serialHandler.OnDataReceived += OnDataReceived;
        serialHandler.SendLog += WriteLog;
        serialHandler.Init("Write serial port name", 115200);
        serialHandler.StartRead();
    }

    void Update()
    {
        lightComponent.intensity = Map(intensity, 250, 500, 2, 0);
    }

    void OnApplicationQuit()
    {
        serialHandler.Dispose();
    }

    void OnDataReceived(string message)
    {
        try
        {
            Debug.Log(message);
            intensity = float.Parse(message);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }

    void WriteLog(string message)
    {
        try
        {
            Debug.Log(message);
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }

    float Map(float value, float start1, float end1, float start2, float end2)
    {
        return start2 + (end2 - start2) * ((value - start1) / (end1 - start1));
    }
}
