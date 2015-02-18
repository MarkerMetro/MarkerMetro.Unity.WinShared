using System;
using UnityEngine;
using WinIntegration = MarkerMetro.Unity.WinIntegration;
using System.Linq;

namespace MarkerMetro.Unity.WinShared
{
    internal static class ExceptionLogger
    {
        /// <summary>
        /// Allows for handling of all unity exceptions
        /// </summary>
        internal static void Init()
        {
            Application.LogCallback handleException = (message, stackTrace, type) =>
            {
                if (type == LogType.Exception || type == LogType.Error)
                {
                    try
                    {
                        if (WinIntegration.Logging.ExceptionLogger.IsEnabled)
                        {
                            WinIntegration.Logging.ExceptionLogger.Send(message, stackTrace);
                            WinIntegration.Logging.ExceptionLogger.IsEnabled = GameController.Instance.GameConfig.ExceptionLoggingAllowed;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        // not sure there's much useful we can do here 
                        Debug.LogWarning(string.Format("Failed to handle exception: {0} - because of: {1}",
                            message, ex.Message));
                    }
                }
            };

            Application.RegisterLogCallback(handleException);
        }
    }
}