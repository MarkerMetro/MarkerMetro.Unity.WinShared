using System;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using Windows.System;
using MarkerMetro.Unity.WinIntegration;
using UnityPlayer;

#if WINDOWS_PHONE
using Microsoft.Phone.Info;
#elif NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
#endif

#if NETFX_CORE
namespace Template
#else
namespace XXXXXXXXXXXXXXXXXXXX   //  <--- Your Windows Phone MainPage.xaml.cs namespace here!
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
         * 
         */
        private bool DisplayMemoryInfo = false;


        /**
         * Call this on MainPage.xaml.cs.
         */
        private void Initialize()
        {
            // wire up the configuration file handler:
            DeviceInformation.DoGetEnvironment = GetEnvironment;

            if (DisplayMemoryInfo)
                BeginRecording();
#if NETFX_CORE
            // Calls WinIntegration's visibility change delegate:
            Window.Current.VisibilityChanged += (s, e) =>
            {
                if (!e.Visible)
                    FireTilesUpdate();
                AppCallbacks.Instance.InvokeOnAppThread(() =>
                {
                    if (Helper.Instance.OnVisibilityChanged != null)
                        Helper.Instance.OnVisibilityChanged(e.Visible);
                }, false);
            };
#endif
        }

        private DeviceInformation.Environment GetEnvironment()
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
            TextBoxMemoryStats.Visibility = System.Windows.Visibility.Visible;

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
                    TextBoxMemoryStats.Text = report;
                    //Debug.WriteLine(report);
                });

            },
                null,
                TimeSpan.FromSeconds(3),
                TimeSpan.FromSeconds(3));
#endif
        }

#if NETFX_CORE
        private void FireTilesUpdate()
        {
            // For examples of all possible tile template types go to http://msdn.microsoft.com/library/windows/apps/windows.ui.notifications.tiletemplatetype
            var squareTile = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text03); // This template requires a SquareTile image in solution Assets folder!
            var wideTile = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150Text01); // This template requires a Wide310x150Logo image in solution Assets folder!

            AppCallbacks.Instance.InvokeOnAppThread(() =>
            {
                try
                {
                    var wideTexts = wideTile.GetElementsByTagName("text");
                    var squareTexts = squareTile.GetElementsByTagName("text");
                    UpdateLiveTiles(wideTexts, squareTexts);

                    AppCallbacks.Instance.InvokeOnUIThread(() =>
                    {
                        var updater = TileUpdateManager.CreateTileUpdaterForApplication();
                        var squareTileNotification = new TileNotification(squareTile);
                        var wideTileNotification = new TileNotification(wideTile);

                        updater.Update(squareTileNotification);
                        updater.Update(wideTileNotification);
                    }, false);

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            }, false);
        }

        /**
         * Updates the Live Tiles.
         * This method offers access to the texts to the wide and medium tiles, but you can tailor
         * it to your project's specific needs.
         * 
         * To use it, just write the game code bits here to update the texts.
         * This method already runs in the game thread, no need to call InvokeOnAppThread.
         */
        private void UpdateLiveTiles(XmlNodeList wideTexts, XmlNodeList squareTexts) {
            /* implement this method! */
        } 
#endif
    }
}
