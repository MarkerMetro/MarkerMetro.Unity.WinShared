using UnityEngine;
using System.Collections;
using MarkerMetro.Unity.WinIntegration;
using WinIntegrationLogging = MarkerMetro.Unity.WinIntegration.Logging;

namespace Assets.Scripts.MarkerMetro
{

    public class ExceptionLogger : MonoBehaviour
    {

        // Use this for initialization
        void Awake()
        {
            Application.RegisterLogCallback(HandleException);
        }

        public static void HandleException(string message, string stackTrace, LogType type)
        {
            if (type == LogType.Exception || type == LogType.Error)
            {
                try
                {
                    if (WinIntegrationLogging.ExceptionLogger.IsInitialized)
                    {
                        WinIntegrationLogging.ExceptionLogger.Send(message, stackTrace);
                        Helper.Instance.ShowDialog(message, "Exception Thrown", null, "OK");
                    }
                    else
                    {
                        Helper.Instance.ShowDialog("You have not initialized an exception logger.", "Exception Thrown", null, "OK");
                    }
                }
                catch (System.Exception ex)
                {
                    // not sure there's much useful we can do here 
                    Debug.LogWarning(string.Format("Failed to handle exception: {0} - because of: {1}", message, ex.Message));
                }
            }
        }
    }

}
