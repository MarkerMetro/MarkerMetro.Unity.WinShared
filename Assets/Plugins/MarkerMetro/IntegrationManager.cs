using System;
using UnityEngine;
using WinIntegration = MarkerMetro.Unity.WinIntegration;
using System.Linq;

namespace MarkerMetro.Unity.WinShared
{
    public static class IntegrationManager
    {
        public static event Action CrashApp;

        public static void DoCrashApp ()
        {
            if (CrashApp != null)
            {
                CrashApp();
            }
        }

        /// <summary>
        /// Initializes all features on the Unity side.
        /// This method is supposed to be called from the Windows code at
        /// app startup.
        /// </summary>
        public static void Init()
        {
            InitExceptionLogger();
        }

        /// <summary>
        /// Allows for handling of all unity exceptions
        /// </summary>
        static void InitExceptionLogger()
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
                            WinIntegration.Logging.ExceptionLogger.IsEnabled = GameConfig.Instance.ExceptionLoggingAllowed;
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