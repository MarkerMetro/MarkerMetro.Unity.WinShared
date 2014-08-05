using UnityEngine;
using System.Collections;
using Mindscape.Raygun4Unity;

public class Logger : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
    {
        Application.RegisterLogCallback(HandleException);

        MarkerMetro.Unity.WinIntegration.SharedLogger.Instance = new RaygunSharedLogger();
	}

    class RaygunSharedLogger : MarkerMetro.Unity.WinIntegration.SharedLogger
    {
        readonly RaygunClient client = new RaygunClient("J5M66WHC/fIcZWudEXXGOw==")   // MarkerMetro API key
        {
            ApplicationVersion = MarkerMetro.Unity.WinIntegration.Helper.Instance.GetAppVersion(),
            User = MarkerMetro.Unity.WinIntegration.Helper.Instance.GetUserDeviceId(),
        };


        override public void Send(System.Exception ex)
        {
            client.Send(ex);
        }

        override public void Send(string message, string stackTrace)
        {
            client.Send(message, stackTrace);
        }
    }

    void HandleException(string message, string stackTrace, LogType type)
    {
        if (type == LogType.Exception || type == LogType.Error)
            MarkerMetro.Unity.WinIntegration.SharedLogger.Instance.Send(message, stackTrace);
    }
}
