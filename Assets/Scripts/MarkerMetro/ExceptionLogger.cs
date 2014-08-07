using UnityEngine;
using System.Collections;

public class ExceptionLogger : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
    {
        Application.RegisterLogCallback(HandleException);
	}

    public static void HandleException(string message, string stackTrace, LogType type)
    {
        if (type == LogType.Exception || type == LogType.Error)
        {
            try
            {
                MarkerMetro.Unity.WinIntegration.ExceptionLogger.Instance.Send(message, stackTrace);
            }
            catch 
            { 
                // not sure there's much useful we can do here 
            }
        }
    }
}
