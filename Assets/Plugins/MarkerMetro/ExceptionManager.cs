using System;
using UnityEngine;
using WinIntegration = MarkerMetro.Unity.WinIntegration;
using System.Linq;

namespace MarkerMetro.Unity.WinShared
{

    /// <summary>
    /// Manages exceptions from Unity
    /// </summary>
    public class ExceptionManager
    {

        private static ExceptionManager _instance;
        private static readonly object _sync = new object();

        /// <summary>
        /// Handled by the application to perform crash test
        /// </summary>
        private Action _appCrashTest;

        private ExceptionManager() { }

        public static ExceptionManager Instance
        {
            get
            {
                lock (_sync)
                {
                    if (_instance == null)
                    {
                        _instance = new ExceptionManager();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Initializes exception management within the Unity side
        /// </summary>
        public void Init(Action appCrashTest = null)
        {
            _appCrashTest = appCrashTest;

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

#if UNITY_5
            Application.logMessageReceived += handleException;
#else
            Application.RegisterLogCallback(handleException);
#endif

        }

        /// <summary>
        /// Allows Unity game to crash test the application
        /// </summary>
        public void AppCrashTest()
        {
            if (_appCrashTest != null)
            {
                _appCrashTest();
            }
        }

    }
}