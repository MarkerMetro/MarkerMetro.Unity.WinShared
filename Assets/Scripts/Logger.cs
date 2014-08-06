using UnityEngine;
using System.Collections;
using Mindscape.Raygun4Unity;

public class Logger : MonoBehaviour 
{
    public static void Initialize()
    {
        if(MarkerMetro.Unity.WinIntegration.SharedLogger.Instance==null)
            MarkerMetro.Unity.WinIntegration.SharedLogger.Instance = new RaygunSharedLogger();
    }

	// Use this for initialization
	void Start () 
    {
        Initialize();

        Application.RegisterLogCallback(HandleException);
	}

    class RaygunSharedLogger : MarkerMetro.Unity.WinIntegration.SharedLogger
    {
        readonly RaygunClient client = new RaygunClient("J5M66WHC/fIcZWudEXXGOw==")   // MarkerMetro API key
        {
            ApplicationVersion = MarkerMetro.Unity.WinIntegration.Helper.Instance.GetAppVersion(),
            User = MarkerMetro.Unity.WinIntegration.Helper.Instance.GetUserDeviceId(),
        };


        override public void Send(System.Exception exception)
        {
            try
            {
                client.Send(exception);
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }

        override public void Send(string message, string stackTrace)
        {
            try
            {
                client.Send(message, stackTrace);
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }
    }

    public static void HandleException(string message, string stackTrace, LogType type)
    {
        if (type == LogType.Exception || type == LogType.Error)
            MarkerMetro.Unity.WinIntegration.SharedLogger.Instance.Send(message, stackTrace);
    }
}
