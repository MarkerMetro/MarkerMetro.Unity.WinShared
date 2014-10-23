using System;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using Windows.System;
using System.Diagnostics;

using UnityPlayer;

#if WINDOWS_PHONE
using System.Linq;
using Microsoft.Phone.Info;
using Microsoft.Phone.Shell;
using MarkerMetro.Unity.WinIntegration.Facebook;
using System.IO.IsolatedStorage;
using UnityProject.WinPhone.Resources;

#elif NETFX_CORE
using Windows.Storage;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using MarkerMetro.Unity.WinIntegration.Facebook;
#endif

using DeviceInformation = Assets.Plugins.MarkerMetro.DeviceInformation;

#if NETFX_CORE
namespace UnityProject.Win
#else
namespace UnityProject.WinPhone
#endif
{
    /**
     * This is a partial class containing code that can be shared between all 
     * Unity porting projects in Marker Metro.
     * 
     * 
     * (Dont forget that the namespace must match)
     */
    public sealed partial class MainPage
    {
#if WINDOWS_PHONE
        private Timer timer;
#endif

        /**
         * Exhibits information about memory usage in the game screen. WP8 only.
         */
        internal bool DisplayMemoryInfo = false; 

        /**
         * Call this on MainPage.xaml.cs.
         */
        private void Initialize()
        {

            // initialize exception logger (api key is MM's test app by default, update with client's and uncomment to use)
            //MarkerMetro.Unity.WinIntegration.ExceptionLogger.Instance.Initialize("J5M66WHC/fIcZWudEXXGOw=="); 

            // wire up the configuration file handler:
            DeviceInformation.DoGetEnvironment = GetEnvironment;

            if (DisplayMemoryInfo)
                BeginRecording();

#if NETFX_CORE

            // set the fb web interface (only for Win8.1)
            FB.SetPlatformInterface(web);

#endif

        }

        internal DeviceInformation.Environment GetEnvironment()
        {
#if QA
            return DeviceInformation.Environment.QA;
#elif DEBUG
            return DeviceInformation.Environment.Dev;
#else
            return DeviceInformation.Environment.Production;
#endif
        }

        /**
         * Add this to your DrawingSurfaceBackgroundGrid block in MaingPage.xaml:
         *  <TextBlock x:Name="TextBoxMemoryStats" Text="0 MB" IsHitTestVisible="False" Visibility="Collapsed"/>
         */
        private void BeginRecording()
        {
            // start a timer to report memory conditions every 3 seconds 
#if WINDOWS_PHONE
            TextBlockMemoryStats.Visibility = System.Windows.Visibility.Visible;

            timer = new Timer(state =>
            {
                string report = "";
                
                report +=
                   "Current: " + (DeviceStatus.ApplicationCurrentMemoryUsage / 1000000).ToString() + "MB\n" +
                   "Peak: " + (DeviceStatus.ApplicationPeakMemoryUsage / 1000000).ToString() + "MB\n" +
                   "Memory Limit: " + (DeviceStatus.ApplicationMemoryUsageLimit / 1000000).ToString() + "MB\n\n" +
                   "Device Total Memory: " + (DeviceStatus.DeviceTotalMemory / 1000000).ToString() + "MB\n" +
                   "Working Limit: " + Convert.ToInt32((Convert.ToDouble(DeviceExtendedProperties.GetValue("ApplicationWorkingSetLimit")) / 1000000)).ToString() + "MB";

                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    TextBlockMemoryStats.Text = report;
                    //Debug.WriteLine(report);
                });

            },
                null,
                TimeSpan.FromSeconds(3),
                TimeSpan.FromSeconds(3));
#endif
        }

        /// <summary>
        /// Displayes IAP Disclamer once per installation
        /// </summary>
        /// <remarks>
        /// If your game does not contain IAP, don't call this method
        /// </remarks>
#if WINDOWS_PHONE
        void CheckForOFT()
        {
            try
            {
                var appSettings = IsolatedStorageSettings.ApplicationSettings;

                if (!appSettings.Contains("OFT"))
                {
                    MessageBox.Show(AppResources.OFT_Disclosure);

                    appSettings["OFT"] = true;
                    appSettings.Save();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
#elif NETFX_CORE
        async void CheckForOFT()
        {
            try
            {
                var settings = ApplicationData.Current.LocalSettings;

                if (!settings.Values.ContainsKey("OFT"))
                {
                    var message = new ResourceLoader().GetString("OFT_Disclosure");

                    var md = new MessageDialog(message);
                    md.Commands.Add(new UICommand("OK"));

                    await md.ShowAsync();

                    settings.Values.Add("OFT", true);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
#endif
    }
}
