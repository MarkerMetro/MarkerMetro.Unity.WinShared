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
#if !(QA || DEBUG)
            // get a Raygun API key for client and uncomment next line
            MarkerMetro.Unity.WinIntegration.ExceptionLogger.Initialize("J5M66WHC/fIcZWudEXXGOw==");
#endif
        }
    }
}
