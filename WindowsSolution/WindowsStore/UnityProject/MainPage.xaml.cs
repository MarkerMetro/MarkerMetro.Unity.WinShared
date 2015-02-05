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
using Windows.UI.Popups;
using MarkerMetro.Unity.WinShared;
using MarkerMetro.Unity.WinIntegration;
using MarkerMetro.Unity.WinIntegration.Logging;
using UnityProject.Config;

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
            settingsPane.CommandsRequested += SettingsPaneCommandsRequested;

            UnityPlayer.AppCallbacks.Instance.RenderingStarted += () =>
                {
                    isUnityLoaded = true;

                    InitializeExceptionLogger();

                    IntegrationManager.Init();
                    IntegrationManager.CrashApp += Crash;
                };

            // create extended splash timer
            extendedSplashTimer = new DispatcherTimer();
            extendedSplashTimer.Interval = TimeSpan.FromMilliseconds(100);
            extendedSplashTimer.Tick += ExtendedSplashTimerTick;
            extendedSplashTimer.Start();
        }

        /// <summary>
        /// Control the extended splash experience
        /// </summary>
        async void ExtendedSplashTimerTick(object sender, object e)
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

        void SettingsPaneCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            var loader = ResourceLoader.GetForViewIndependentUse();

            if (AppConfig.Instance.NoticationsControlEnabled || AppConfig.Instance.MusicFXControlEnabled)
            {
                args.Request.ApplicationCommands.Add(new SettingsCommand(Guid.NewGuid(), 
                    loader.GetString("SettingsCharm_Settings"), h =>
                {
                    var sf = new GameSettingsFlyout();
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
            args.Request.ApplicationCommands.Add(
                new SettingsCommand(Guid.NewGuid(),
                    "Crash",
                    h => Crash()));
#endif
        }

        static async void Crash()
        {
            var dialog = new MessageDialog("Do you want to cause the crash to test error reporting?", "Crash?");

            dialog.Commands.Add(new UICommand("Yes"));
            dialog.Commands.Add(new UICommand("No"));

            var result = await dialog.ShowAsync();

            if (result.Label == "Yes")
            {
                ExceptionLogger.IsEnabled = true;
                throw new InvalidOperationException("A test crash from Windows Store solution!");
            }
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
            if (!AppCallbacks.Instance.IsInitialized())
            {
                return;
            }

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
            try
            {
                //***********************************************
                //Adjust these parameters with appropriate values
                string frontMediumImagePath = "/Assets/Logo.png";
                string frontWideImagePath = "/Assets/WideLogo.png";
                string backWideImagePath = "/Assets/Logo.png";

                string mediumText1 = "Level"; // **Don't forget to localise**
                string mediumText2 = "Score";
                string mediumText3 = "Time";
                string mediumText4 = "Coins";

                string mediumData1 = "99"; // **This data should come from a game class**
                string mediumData2 = "1234";
                string mediumData3 = "11:22";
                string mediumData4 = "7";

                string wideText1 = mediumText1; // **These can be made different if more detail is wanted for wide tile**
                string wideText2 = mediumText2;
                string wideText3 = mediumText3;
                string wideText4 = mediumText4;

                string wideData1 = mediumData1;
                string wideData2 = mediumData2;
                string wideData3 = mediumData3;
                string wideData4 = mediumData4;
                //
                //***********************************************

                // Retrieve the XML that defines the appearance of the tiles
                XmlDocument frontMediumTemplate = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Image);
                XmlDocument backMediumTemplate = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text01);
                XmlDocument frontWideTemplate = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150Image);
                XmlDocument backWideTemplate = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150SmallImageAndText02);

                // Tiles should display game name on the back, but not on the front, as it will be in the image
                // retrieve the 'binding' element from the xml
                XmlElement frontMediumBinding = (XmlElement)frontMediumTemplate.GetElementsByTagName("binding").Single();
                XmlElement backMediumBinding = (XmlElement)backMediumTemplate.GetElementsByTagName("binding").Single();
                XmlElement frontWideBinding = (XmlElement)frontWideTemplate.GetElementsByTagName("binding").Single();
                XmlElement backWideBinding = (XmlElement)backWideTemplate.GetElementsByTagName("binding").Single();

                // Set 'branding' attribute on 'binding' element
                // fronts need 'none' backs need 'name'
                frontMediumBinding.SetAttribute("branding", "none");
                backMediumBinding.SetAttribute("branding", "name");
                frontWideBinding.SetAttribute("branding", "none");
                backWideBinding.SetAttribute("branding", "name");

                // Set 'src' attribute in 'image' elements
                // Both front tiles need images
                // only wide tile has image on back
                // Access elements
                XmlElement frontMediumImage = (XmlElement)frontMediumTemplate.GetElementsByTagName("image").Single();
                XmlElement frontWideImage = (XmlElement)frontWideTemplate.GetElementsByTagName("image").Single();
                XmlElement backWideImage = (XmlElement)backWideTemplate.GetElementsByTagName("image").Single();
                // Set attributes
                frontMediumImage.SetAttribute("src", frontMediumImagePath);
                frontWideImage.SetAttribute("src", frontWideImagePath);
                backWideImage.SetAttribute("src", backWideImagePath);

                // Set lines of text to appear on back of tiles.
                // Retrieve lists of text elements
                XmlNodeList mediumTextList = backMediumTemplate.GetElementsByTagName("text");
                XmlNodeList wideTextList = backWideTemplate.GetElementsByTagName("text");
                // Set text for each element
                mediumTextList[0].InnerText = string.Format("{0}: {1}", mediumText1, mediumData1);
                mediumTextList[1].InnerText = string.Format("{0}: {1}", mediumText2, mediumData2);
                mediumTextList[2].InnerText = string.Format("{0}: {1}", mediumText3, mediumData3);
                mediumTextList[3].InnerText = string.Format("{0}: {1}", mediumText4, mediumData4);
                wideTextList[0].InnerText = string.Format("{0}: {1}", wideText1, wideData1);
                wideTextList[1].InnerText = string.Format("{0}: {1}", wideText2, wideData2);
                wideTextList[2].InnerText = string.Format("{0}: {1}", wideText3, wideData3);
                wideTextList[3].InnerText = string.Format("{0}: {1}", wideText4, wideData4);

                // Each notification requires the data for the wide and medium tiles
                // One notification for the front, and one for the back
                // Join the templates.
                // Access 'visual' element of medium which contains medium 'binding' element
                IXmlNode frontVisual = frontMediumTemplate.GetElementsByTagName("visual").Single();
                IXmlNode backVisual = backMediumTemplate.GetElementsByTagName("visual").Single();
                //add 'wide' binding element to it
                frontVisual.AppendChild(frontMediumTemplate.ImportNode(frontWideBinding, true)); 
                backVisual.AppendChild(backMediumTemplate.ImportNode(backWideBinding, true));

                TileNotification frontNotification = new TileNotification(frontMediumTemplate); // Both contains medium and wide after join.
                TileNotification BackNotification = new TileNotification(backMediumTemplate); 

                TileUpdater tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();  // Access tile updater
                tileUpdater.EnableNotificationQueue(true);                                      // Enable queing to cycle through front and back
                tileUpdater.Clear();                                                            // Make sure queue is empty
                tileUpdater.Update(BackNotification);                                           // Switch to back tile
                tileUpdater.Update(frontNotification);                                          // Queue up front tile

                // Reading the actual Xml help to understand what is happening here.
                Debug.WriteLine(frontMediumTemplate.GetXml());
                Debug.WriteLine(backMediumTemplate.GetXml());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                if (ExceptionLogger.IsEnabled)
                {
                    ExceptionLogger.Send(ex);
                }
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
                {
                    return;
                }
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
        }

		protected override Windows.UI.Xaml.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
		{
			return new UnityPlayer.XamlPageAutomationPeer(this);
		}
    }
}
