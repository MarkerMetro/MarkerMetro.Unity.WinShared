using MarkerMetro.Unity.WinIntegration.Facebook;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Info;
using Windows.Foundation;
using Windows.Devices.Geolocation;

using UnityApp = UnityPlayer.UnityApp;
using UnityBridge = WinRTBridge.WinRTBridge;
using System.Windows.Threading;
using Microsoft.Phone.Tasks;
using MarkerMetro.Unity.WinIntegration.Store;
using System.Threading.Tasks;
using UnityProject.WinPhone.Resources;
using System.Diagnostics;
using MarkerMetro.Common.Converters;

namespace UnityProject.WinPhone
{
	public partial class MainPage : PhoneApplicationPage
	{
		bool _unityStartedLoading;
		bool _useLocation;
        DispatcherTimer _extendedSplashTimer;
        public static bool IsUnityLoaded { get; set; }

		// Constructor
		public MainPage()
		{
			var bridge = new UnityBridge();
			UnityApp.SetBridge(bridge);
			InitializeComponent();
			FB.SetPlatformInterface(web);
            Initialize();

			bridge.Control = DrawingSurfaceBackground;

            _extendedSplashTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100),
            };
            _extendedSplashTimer.Tick += ExtendedSplashTimer_Tick;
            _extendedSplashTimer.Start();

           
            // control memory debugging flag here
            #if DEBUG || QA
                 //DisplayMemoryInfo = true;
            #endif
        }

        /// <summary>
        /// Allows user to rate app
        /// </summary>
        void ShowRateUI()
        {
            Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    var marketplaceReviewTask = new MarketplaceReviewTask();


                    marketplaceReviewTask.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to show MarketplaceReviewTask because of: " + ex.Message);
                }
            });
        }

		void DrawingSurfaceBackground_Loaded(object sender, RoutedEventArgs e)
		{
			if (!_unityStartedLoading)
			{
				_unityStartedLoading = true;

				UnityApp.SetLoadedCallback(() => { Dispatcher.BeginInvoke(Unity_Loaded); });
				
				int physicalWidth, physicalHeight;
				object physicalResolution;

				var content = Application.Current.Host.Content;
				var nativeWidth = (int)Math.Floor(content.ActualWidth * content.ScaleFactor / 100.0 + 0.5);
				var nativeHeight = (int)Math.Floor(content.ActualHeight * content.ScaleFactor / 100.0 + 0.5);

				if (DeviceExtendedProperties.TryGetValue("PhysicalScreenResolution", out physicalResolution))
				{
					var resolution = (System.Windows.Size) physicalResolution;

					physicalWidth = (int)resolution.Width;
					physicalHeight = (int)resolution.Height;
				}
				else
				{
					physicalWidth = nativeWidth;
					physicalHeight = nativeHeight;
				}

				UnityApp.SetNativeResolution(nativeWidth, nativeHeight);
				UnityApp.SetRenderResolution(physicalWidth, physicalHeight);
				UnityPlayer.UnityApp.SetOrientation((int)Orientation);

				DrawingSurfaceBackground.SetBackgroundContentProvider(UnityApp.GetBackgroundContentProvider());
				DrawingSurfaceBackground.SetBackgroundManipulationHandler(UnityApp.GetManipulationHandler());

                // initialise plugins
                MarkerMetro.Unity.WinLegacy.Dispatcher.InvokeOnAppThread = InvokeOnAppThread;
                MarkerMetro.Unity.WinLegacy.Dispatcher.InvokeOnUIThread = InvokeOnUIThread;
                MarkerMetro.Unity.WinIntegration.Dispatcher.InvokeOnAppThread = InvokeOnAppThread;
                MarkerMetro.Unity.WinIntegration.Dispatcher.InvokeOnUIThread = InvokeOnUIThread;
			}
		}

		void Unity_Loaded()
		{
            IsUnityLoaded = true;

			SetupGeolocator();

            //Initialise Store system
#if QA || DEBUG
            StoreManager.Instance.Initialise(true);
#else
            StoreManager.Instance.Initialise(false);
#endif
            CheckForOFT();
		}

        public void InvokeOnAppThread(System.Action callback)
        {
            UnityApp.BeginInvoke(() => callback());
        }
        public void InvokeOnUIThread(System.Action callback)
        {
            Dispatcher.BeginInvoke(() => callback());
        }

        async void ExtendedSplashTimer_Tick(object sender, EventArgs e)
        {
            var increment = _extendedSplashTimer.Interval.TotalMilliseconds * 10;
            if (!IsUnityLoaded && SplashProgress.Value <= (SplashProgress.Maximum - increment))
                SplashProgress.Value += increment;
            else
            {
                SplashProgress.Value = SplashProgress.Maximum;
                await Task.Delay(250);
                RemoveExtendedSplash();
                _extendedSplashTimer.Stop();
            }
        }

        void RemoveExtendedSplash()
        {
            if (_extendedSplashTimer != null)
                _extendedSplashTimer.Stop();

            if (DrawingSurfaceBackground.Children.Count > 0)
                DrawingSurfaceBackground.Children.Remove(ExtendedSplashGrid);
        }

		void PhoneApplicationPage_BackKeyPress(object sender, CancelEventArgs e)
		{
			if (web.Visibility != Visibility.Collapsed)
            {
                web.Finish();

                e.Cancel = true;
            }
            else
            {
                e.Cancel = UnityApp.BackButtonPressed();
            }
		}

		void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
		{
			UnityApp.SetOrientation((int)e.Orientation);
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!UnityApp.IsLocationEnabled())
                return;
            if (IsolatedStorageSettings.ApplicationSettings.Contains("LocationConsent"))
                _useLocation = (bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"];
            else
            {
                MessageBoxResult result = MessageBox.Show("Can this application use your location?",
                    "Location Services", MessageBoxButton.OKCancel);
                _useLocation = result == MessageBoxResult.OK;
                IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = _useLocation;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

		void SetupGeolocator()
        {
            if (!_useLocation)
                return;

            try
            {
				UnityApp.EnableLocationService(true);
                Geolocator geolocator = new Geolocator();
				geolocator.ReportInterval = 5000;
                IAsyncOperation<Geoposition> op = geolocator.GetGeopositionAsync();
                op.Completed += (asyncInfo, asyncStatus) =>
                    {
                        if (asyncStatus == AsyncStatus.Completed)
                        {
                            Geoposition geoposition = asyncInfo.GetResults();
                            UnityApp.SetupGeolocator(geolocator, geoposition);
                        }
                        else
                            UnityApp.SetupGeolocator(null, null);
                    };
            }
            catch (Exception)
            {
                UnityApp.SetupGeolocator(null, null);
            }
        }
	}
}