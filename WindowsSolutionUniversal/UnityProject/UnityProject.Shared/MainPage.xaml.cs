﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.Windows;
using UnityPlayer;
using Windows.Data.Xml.Dom;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using MarkerMetro.Unity.WinIntegration;
using MarkerMetro.Unity.WinIntegration.Logging;
using UnityProject.Logging;
using UnityProject.Config;
#if UNITY_METRO_8_1
using Windows.UI.ApplicationSettings;
using MarkerMetro.Unity.WinIntegration.Facebook;
#endif
#if UNITY_WP_8_1
using System.Threading;
#endif

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UnityProject
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

#if UNITY_METRO_8_1
        static SettingsPane settingsPane;
#endif
#if UNITY_WP_8_1
        private Timer timer;
#endif
        private WinRTBridge.WinRTBridge _bridge;

        SplashScreen splash;
        Rect splashImageRect;
        WindowSizeChangedEventHandler onResizeHandler;
        DispatcherTimer extendedSplashTimer;
        bool isUnityLoaded;

        public MainPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;

            AppCallbacks appCallbacks = AppCallbacks.Instance;
            // Setup scripting bridge
            _bridge = new WinRTBridge.WinRTBridge();
            appCallbacks.SetBridge(_bridge);

#if !UNITY_WP_8_1
            appCallbacks.SetKeyboardTriggerControl(this);
#endif
            appCallbacks.SetSwapChainPanel(GetSwapChainPanel());
            appCallbacks.SetCoreWindowEvents(Window.Current.CoreWindow);
            appCallbacks.InitializeD3DXAML();

            splash = ((App)App.Current).SplashScreen;
            GetSplashBackgroundColor();
            OnResize();

            onResizeHandler = new WindowSizeChangedEventHandler((o, e) => OnResize());
            Window.Current.SizeChanged += onResizeHandler;
            Window.Current.VisibilityChanged += OnWindowVisibilityChanged;

#if UNITY_WP_8_1
            SetupLocationService();
#endif

#if UNITY_METRO_8_1
            // Configure settings charm
            settingsPane = SettingsPane.GetForCurrentView();
            settingsPane.CommandsRequested += SettingsPaneCommandsRequested;
#endif
            // provide the game configuration
            MarkerMetro.Unity.WinShared.GameController.Instance.Init(AppConfig.Instance);

            AppCallbacks.Instance.RenderingStarted += () =>
                {
                    isUnityLoaded = true;
                    AppCallbacks.Instance.InvokeOnAppThread(() =>
                    {
                        MarkerMetro.Unity.WinShared.ExceptionManager.Instance.Init(Crash);
                    }, false);
                };

            // create extended splash timer
            extendedSplashTimer = new DispatcherTimer();
            extendedSplashTimer.Interval = TimeSpan.FromMilliseconds(100);
            extendedSplashTimer.Tick += ExtendedSplashTimer_Tick;
            extendedSplashTimer.Start();
            
#if UNITY_METRO_8_1
            // set the fb web interface (only for Win8.1).
            FB.SetPlatformInterface(web);
#endif
        }

        /// <summary>
        /// Control the extended splash experience
        /// </summary>
        async void ExtendedSplashTimer_Tick(object sender, object e)
        {
            var increment = extendedSplashTimer.Interval.TotalMilliseconds;
            if (!isUnityLoaded && SplashProgress.Value <= (SplashProgress.Maximum - increment))
            {
                SplashProgress.Value += increment;
            }
            else
            {
                SplashProgress.Value = SplashProgress.Maximum;
                extendedSplashTimer.Stop();
                await Task.Delay(250); // force a little delay so that user can see progress bar maxing out very briefly
                RemoveSplashScreen();
            }
        }

#if UNITY_METRO_8_1
        void SettingsPaneCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            var loader = ResourceLoader.GetForViewIndependentUse();

            if (AppConfig.Instance.NotificationsControlEnabled || AppConfig.Instance.MusicFXControlEnabled)
            {
                args.Request.ApplicationCommands.Add(new SettingsCommand(Guid.NewGuid(),
                    loader.GetString("SettingsCharm_Settings"), h =>
                {
                    var sf = new UnityProject.Win.GameSettingsFlyout();
                    sf.Show();
                }));
            }
            args.Request.ApplicationCommands.Add(
                new SettingsCommand(Guid.NewGuid(),
                    loader.GetString("SettingsCharm_CustomerSupport"),
                    h => OnViewUrl(loader.GetString("SettingsCharm_CustomerSupport_Url"))));
            args.Request.ApplicationCommands.Add(
                new SettingsCommand(Guid.NewGuid(),
                    loader.GetString("SettingsCharm_TermsOfUse"),
                    h => OnViewUrl(loader.GetString("SettingsCharm_TermsOfUse_Url"))));
            args.Request.ApplicationCommands.Add(
                new SettingsCommand(Guid.NewGuid(),
                    loader.GetString("SettingsCharm_PrivacyPolicy"),
                    h => OnViewUrl(loader.GetString("SettingsCharm_PrivacyPolicy_Url"))));
#if DEBUG || QA
            if (AppConfig.Instance.ExceptionLoggingEnabled)
            {
                args.Request.ApplicationCommands.Add(
                    new SettingsCommand(Guid.NewGuid(),
                        "Crash",
                        h => Crash()));
            }
#endif
        }

        static void OnViewUrl(string url)
        {
            AppCallbacks.Instance.InvokeOnUIThread(async () =>
            {
                await Launcher.LaunchUriAsync(new Uri(url, UriKind.Absolute));
            }, false);
        }
#endif

        static void Crash()
        {
            AppCallbacks.Instance.InvokeOnUIThread(() =>
            {
                ExceptionLogger.IsEnabled = true; // override to allow test crashing
                throw new InvalidOperationException("A test crash from Windows Universal solution!");
            }, false);
        }

        async void OnWindowVisibilityChanged(object sender, VisibilityChangedEventArgs e)
        {
            if (!AppCallbacks.Instance.IsInitialized()) return;

            if (e.Visible)
            {
                AppCallbacks.Instance.UnityPause(0);
                return;
            }
            else
            {
                // Make sure Unity Player Prefs are saved!
                AppCallbacks.Instance.InvokeOnAppThread(() =>
                {
                    UnityEngine.PlayerPrefs.Save();
                }, false);

#if UNITY_METRO_8_1
                // Unity pauses automatically on WP.
                AppCallbacks.Instance.UnityPause(1);
#endif
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            splash = (SplashScreen)e.Parameter;
            OnResize();
        }

        void OnResize()
        {
            if (splash != null)
            {
                splashImageRect = splash.ImageLocation;
                PositionImage();
            }
        }

        private void PositionImage()
        {
            var inverseScaleX = 1.0f;
            var inverseScaleY = 1.0f;
#if UNITY_WP_8_1
            inverseScaleX = inverseScaleX / DXSwapChainPanel.CompositionScaleX;
            inverseScaleY = inverseScaleY / DXSwapChainPanel.CompositionScaleY;
#endif

            ExtendedSplashImage.SetValue(Canvas.LeftProperty, splashImageRect.X * inverseScaleX);
            ExtendedSplashImage.SetValue(Canvas.TopProperty, splashImageRect.Y * inverseScaleY);
            ExtendedSplashImage.Height = splashImageRect.Height * inverseScaleY;
            ExtendedSplashImage.Width = splashImageRect.Width * inverseScaleX;
        }

        async void GetSplashBackgroundColor()
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///AppxManifest.xml"));
                string manifest = await FileIO.ReadTextAsync(file);
                int idx = manifest.IndexOf("SplashScreen");
                manifest = manifest.Substring(idx);
                idx = manifest.IndexOf("BackgroundColor");
                if (idx < 0)  // background is optional
                    return;
                manifest = manifest.Substring(idx);
                idx = manifest.IndexOf("\"");
                manifest = manifest.Substring(idx + 2); // also remove quote and # char after it
                idx = manifest.IndexOf("\"");
                manifest = manifest.Substring(0, idx);
                int value = Convert.ToInt32(manifest, 16) & 0x00FFFFFF;
                byte r = (byte)(value >> 16);
                byte g = (byte)((value & 0x0000FF00) >> 8);
                byte b = (byte)(value & 0x000000FF);

                await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.High, delegate()
                    {
                        ExtendedSplashGrid.Background = new SolidColorBrush(Color.FromArgb(0xFF, r, g, b));
                    });
            }
            catch (Exception)
            { }
        }

        public SwapChainPanel GetSwapChainPanel()
        {
            return DXSwapChainPanel;
        }

        public void RemoveSplashScreen()
        {
            DXSwapChainPanel.Children.Remove(ExtendedSplashGrid);
            if (onResizeHandler != null)
            {
                Window.Current.SizeChanged -= onResizeHandler;
                onResizeHandler = null;
            }
            if (AppConfig.Instance.IapDisclaimerEnabled)
            {
                CheckForOFT();
            }
            if (AppConfig.Instance.DisplayMemoryUsageAllowed)
            {
                BeginRecording();
            }
        }

#if !UNITY_WP_8_1
        protected override Windows.UI.Xaml.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
        {
            return new UnityPlayer.XamlPageAutomationPeer(this);
        }
#else
        // This is the default setup to show location consent message box to the user
        // You can customize it to your needs, but do not remove it completely if your application
        // uses location services, as it is a requirement in Windows Store certification process
        private async void SetupLocationService()
        {
            AppCallbacks appCallbacks = AppCallbacks.Instance;
            if (!appCallbacks.IsLocationCapabilitySet())
            {
                return;
            }

            const string settingName = "LocationContent";
            bool userGaveConsent = false;

            object consent;
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var userWasAskedBefore = settings.Values.TryGetValue(settingName, out consent);

            if (!userWasAskedBefore)
            {
                var messageDialog = new Windows.UI.Popups.MessageDialog("Can this application use your location?", "Location services");

                var acceptCommand = new Windows.UI.Popups.UICommand("Yes");
                var declineCommand = new Windows.UI.Popups.UICommand("No");

                messageDialog.Commands.Add(acceptCommand);
                messageDialog.Commands.Add(declineCommand);

                userGaveConsent = (await messageDialog.ShowAsync()) == acceptCommand;
                settings.Values.Add(settingName, userGaveConsent);
            }
            else
            {
                userGaveConsent = (bool)consent;
            }

            if (userGaveConsent)
            {	// Must be called from UI thread
                appCallbacks.SetupGeolocator();
            }
        }
#endif

        async void CheckForOFT()
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

        /**
         * Add this to your DrawingSurfaceBackgroundGrid block in MaingPage.xaml:
         *  <TextBlock x:Name="TextBoxMemoryStats" Text="0 MB" IsHitTestVisible="False" Visibility="Collapsed"/>
         */
        private void BeginRecording()
        {
            // start a timer to report memory conditions every 3 seconds 
#if UNITY_WP_8_1
            if (AppConfig.Instance.DisplayMemoryUsageEnabled)
            {
                TextBlockMemoryStats.Visibility = Visibility.Visible;

                timer = new Timer(state =>
                {
                    string report = "";

                    report +=
                       "Current: " + (MemoryManager.AppMemoryUsage / 1000000).ToString() + "MB\n" +
                       "Memory Limit: " + (MemoryManager.AppMemoryUsageLimit / 1000000).ToString() + "MB\n\n" +
                       "Memory Usage Level: " + MemoryManager.AppMemoryUsageLevel.ToString();


                    AppCallbacks.Instance.InvokeOnUIThread(delegate
                    {
                        TextBlockMemoryStats.Text = report;
                    }, false);

                },
                    null,
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(3));
            }
#endif
        }

    }
}
