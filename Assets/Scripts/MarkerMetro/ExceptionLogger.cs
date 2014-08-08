using UnityEngine;
using System.Collections;
using MarkerMetro.Unity.WinIntegration;

public class ExceptionLogger : MonoBehaviour 
{

	// Use this for initialization
	void Awake () 
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
            catch (System.Exception ex)
            { 
                // not sure there's much useful we can do here 
                Debug.LogWarning(string.Format("Failed to handle exception: {0} - because of: {1}", message, ex.Message));
            }
        }
    }
}
