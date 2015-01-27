using UnityEngine;
using System.Collections;
using WinIntegration = MarkerMetro.Unity.WinIntegration;

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
                    WinIntegration.ExceptionLogger.Instance.Send(message, stackTrace);
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
