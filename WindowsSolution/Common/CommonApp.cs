using MarkerMetro.Unity.WinIntegration.Logging;
using System;
using System.Diagnostics;
using UnityProject.Config;
using UnityProject.Logging;
#if NETFX_CORE
namespace UnityProject.Win
#else
namespace UnityProject
#endif
{
    /**
     * This is a partial class containing code that can be shared between all 
     * Unity porting projects in Marker Metro.
     * 
     * (Dont forget that the namespace must match)
     */
    public sealed partial class App
    {

        void InitializeExceptionLogger()
        {
            if (AppConfig.Instance.ExceptionLoggingEnabled)
            {
                var apiKey = AppConfig.Instance.ExceptionLoggingApiKey;
                if (!string.IsNullOrEmpty(apiKey))
                {
                    try
                    {
                        // swap this out with an IExceptionLogger implementation as required
                        ExceptionLogger.Initialize(new RaygunExceptionLogger(apiKey));
                        ExceptionLogger.IsEnabled = AppConfig.Instance.ExceptionLoggingAllowed;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Failed initializing exception logger.");
                        Debug.WriteLine(ex.Message);
                    }
                }
            }
        }

    }
}
