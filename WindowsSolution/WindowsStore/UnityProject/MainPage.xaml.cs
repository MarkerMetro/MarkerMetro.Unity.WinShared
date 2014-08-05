using System;
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
using Windows.UI.ApplicationSettings;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UnityProject.Win
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        SplashScreen splash;
        Rect splashImageRect;
        WindowSizeChangedEventHandler onResizeHandler;
        static SettingsPane settingsPane;
        DispatcherTimer extendedSplashTimer;
        bool isUnityLoaded;

        public MainPage(SplashScreen splashScreen)
        {
            this.InitializeComponent();

            Initialize();

            splash = splashScreen;
            GetSplashBackgroundColor();
            OnResize();
            Window.Current.SizeChanged += onResizeHandler = new WindowSizeChangedEventHandler((o, e) => OnResize());

            Window.Current.VisibilityChanged += OnWindowVisibilityChanged;

            // Configure settings charm
            settingsPane = SettingsPane.GetForCurrentView();
            settingsPane.CommandsRequested += settingsPane_CommandsRequested;

            UnityPlayer.AppCallbacks.Instance.RenderingStarted += () => isUnityLoaded = true;

            // create extended splash timer
            extendedSplashTimer = new DispatcherTimer();
            extendedSplashTimer.Interval = TimeSpan.FromMilliseconds(100);
            extendedSplashTimer.Tick += ExtendedSplashTimer_Tick;
            extendedSplashTimer.Start();
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

        void settingsPane_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            var loader = ResourceLoader.GetForViewIndependentUse();

            args.Request.ApplicationCommands.Add(new SettingsCommand(Guid.NewGuid(), loader.GetString("SettingsCharm_Settings"), h =>
            {
                var sf = new GameSettingsFlyout();
                sf.Show();
            }));
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
        }

        static void OnViewUrl(string url)
        {
            AppCallbacks.Instance.InvokeOnUIThread(async () =>
            {
                await Launcher.LaunchUriAsync(new Uri(url, UriKind.Absolute));
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
                AppCallbacks.Instance.UnityPause(1);

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    UpdateLiveTiles();
                });

                // make sure Unity Player Prefs are saved!
                AppCallbacks.Instance.InvokeOnAppThread(() =>
                {
                    UnityEngine.PlayerPrefs.Save();
                }, false);
            }
        }

        void UpdateLiveTiles()
        {
            var content = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text01);
            var contentWide = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150SmallImageAndText02);
            var resources = ResourceLoader.GetForViewIndependentUse();

            Debug.WriteLine(content.GetXml());
            Debug.WriteLine(contentWide.GetXml());

            try
            {
                var wideBinding = (XmlElement)contentWide.GetElementsByTagName("binding").Single();
                wideBinding.SetAttribute("branding", "name");

                var xTexts = content.GetElementsByTagName("text");
                var xWideTexts = wideBinding.GetElementsByTagName("text");
                
                xWideTexts[0].InnerText = xTexts[0].InnerText = "UnityProject"; // update with localized game value
                xWideTexts[1].InnerText = xTexts[1].InnerText = "UnityProject"; // update with localized game value
                xWideTexts[2].InnerText = xTexts[2].InnerText = "UnityProject"; // update with localized game value

                xTexts[3].InnerText = "UnityProject"; // update with localized game value
                xWideTexts[3].InnerText = "UnityProject"; // update with localized game value

                var xImage = wideBinding.GetElementsByTagName("image");
                ((XmlElement)xImage[0]).SetAttribute("src", "/Assets/WideLogo.png");
                //((XmlElement)xImage[1]).SetAttribute("src", "/Assets/Logo.png");

                var xVisual = (XmlElement)content.GetElementsByTagName("visual").Single();
                xVisual.AppendChild(content.ImportNode(wideBinding, true));
                Debug.WriteLine(content.GetXml());

                var notificiation = new TileNotification(content)
                {
                    Tag = content.GetHashCode().ToString(),
                };

                TileUpdateManager.CreateTileUpdaterForApplication().Update(notificiation);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                MarkerMetro.Unity.WinIntegration.SharedLogger.Instance.Send(ex);
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

        void PositionImage()
        {
            ExtendedSplashImage.SetValue(Canvas.LeftProperty, splashImageRect.X);
            ExtendedSplashImage.SetValue(Canvas.TopProperty, splashImageRect.Y);
            ExtendedSplashImage.Height = splashImageRect.Height;
            ExtendedSplashImage.Width = splashImageRect.Width;
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

            CheckForOFT();
        }

		protected override Windows.UI.Xaml.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
		{
			return new UnityPlayer.XamlPageAutomationPeer(this);
		}
    }
}
