using MarkerMetro.Unity.WinIntegration.Logging;
using MarkerMetro.Unity.WinShared.Tools;
using UnityProject.Logging;
using Environment = MarkerMetro.Unity.WinShared.Tools.Environment;
using System;
using System.Diagnostics;

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
        /**
         * Call this on MainPage.xaml.cs.
         */
        void InitializeExceptionLogger()
        {
            if (FeaturesManager.Instance.IsExceptionLoggingEnabled)
            {
                if (!string.IsNullOrEmpty(FeaturesManager.Instance.ExceptionLoggingApiKey))
                {
                    try
                    {
                        // Initialize Raygun with API key set in the features setting menu.
                        ExceptionLogger.Initialize(new RaygunExceptionLogger(FeaturesManager.Instance.ExceptionLoggingApiKey));
#if DEBUG
                        ExceptionLogger.IsEnabled = FeaturesManager.Instance.IsExceptionLoggingEnabledForEnvironment(Environment.Dev);
#elif QA
                        ExceptionLogger.IsEnabled = FeaturesManager.Instance.IsExceptionLoggingEnabledForEnvironment(Environment.QA);
#else
                        ExceptionLogger.IsEnabled = FeaturesManager.Instance.IsExceptionLoggingEnabledForEnvironment(Environment.Production);
#endif
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
