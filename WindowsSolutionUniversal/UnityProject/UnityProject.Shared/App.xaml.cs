﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using UnityPlayer;
using System.Diagnostics;
#if UNITY_WP_8_1
using Windows.System;
#endif

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace Template
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	sealed partial class App : Application
	{
		WinRTBridge.WinRTBridge _bridge;
		AppCallbacks appCallbacks;


		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			this.InitializeComponent();

            InitializeExceptionLogger();

            appCallbacks = new AppCallbacks(false);
            UnhandledException += LogUnhandledException;

            // Prevents display to dim while the app is visible:
            var displayRequest = new Windows.System.Display.DisplayRequest();
            displayRequest.RequestActive();

#if DEBUG
            DebugSettings.EnableFrameRateCounter = true;
#endif
        }

        void LogUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    // An unhandled exception has occurred; break into the debugger
                    System.Diagnostics.Debugger.Break();
                }
                else
                {
                    MarkerMetro.Unity.WinIntegration.ExceptionLogger.Instance.Send(e.Exception);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("FAILED to report unhandled exception:");
                Debug.WriteLine(ex.ToString());
            }
        }

		/// <summary>
		/// Invoked when application is launched through protocol.
		/// Read more - http://msdn.microsoft.com/library/windows/apps/br224742
		/// </summary>
		/// <param name="args"></param>
		protected override void OnActivated(IActivatedEventArgs args)
		{
			string appArgs = "";
			Windows.ApplicationModel.Activation.SplashScreen splashScreen = null;
			switch (args.Kind)
			{
				case ActivationKind.Protocol:
					ProtocolActivatedEventArgs eventArgs = args as ProtocolActivatedEventArgs;
					splashScreen = eventArgs.SplashScreen;
					appArgs += string.Format("Uri={0}", eventArgs.Uri.AbsoluteUri);

#if UNITY_WP_8_1
                    MarkerMetro.Unity.WinIntegration.Facebook.FBNative.MapUri(eventArgs.Uri);
#endif
					break;
			}
			InitializeUnity(appArgs, splashScreen);
		}

		/// <summary>
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used when the application is launched to open a specific file, to display
		/// search results, and so forth.
		/// </summary>
		/// <param name="args">Details about the launch request and process.</param>
		protected override void OnLaunched(LaunchActivatedEventArgs args)
		{
			InitializeUnity(args.Arguments, args.SplashScreen);
		}

		void InitializeUnity(string args, Windows.ApplicationModel.Activation.SplashScreen splashScreen)
		{
#if UNITY_WP_8_1
			ApplicationView.GetForCurrentView().SuppressSystemOverlays = true;
#pragma warning disable 4014
			StatusBar.GetForCurrentView().HideAsync();
#pragma warning restore 4014
#endif

			appCallbacks.SetAppArguments(args);
			Frame rootFrame = Window.Current.Content as Frame;

			// Do not repeat app initialization when the Window already has content,
			// just ensure that the window is active
			if (rootFrame == null && !appCallbacks.IsInitialized())
			{
                // Initialise Store system
#if QA || DEBUG
                MarkerMetro.Unity.WinIntegration.Store.StoreManager.Instance.Initialise(true);
#else
                MarkerMetro.Unity.WinIntegration.Store.StoreManager.Instance.Initialise(false);
#endif
                var mainPage = new MainPage(splashScreen);
				Window.Current.Content = mainPage;
				Window.Current.Activate();

				// Setup scripting bridge
				_bridge = new WinRTBridge.WinRTBridge();
				appCallbacks.SetBridge(_bridge);

#if !UNITY_WP_8_1
				appCallbacks.SetKeyboardTriggerControl(mainPage);
#endif

				appCallbacks.SetSwapChainPanel(mainPage.GetSwapChainPanel());
				appCallbacks.SetCoreWindowEvents(Window.Current.CoreWindow);
				appCallbacks.InitializeD3DXAML();

                AppCallBacksInitialized();
                InitializePlatformSpecificMethods();
			}

			Window.Current.Activate();
			
#if UNITY_WP_8_1
			SetupLocationService();
#endif
		}
		
#if UNITY_WP_8_1
		// This is the default setup to show location consent message box to the user
		// You can customize it to your needs, but do not remove it completely if your application
		// uses location services, as it is a requirement in Windows Store certification process
		async void SetupLocationService()
		{
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

        void AppCallBacksInitialized()
        {
            // wire up dispatcher for plugins
            MarkerMetro.Unity.WinLegacy.Dispatcher.InvokeOnAppThread = InvokeOnAppThread;
            MarkerMetro.Unity.WinLegacy.Dispatcher.InvokeOnUIThread = InvokeOnUIThread;
            MarkerMetro.Unity.WinIntegration.Dispatcher.InvokeOnAppThread = InvokeOnAppThread;
            MarkerMetro.Unity.WinIntegration.Dispatcher.InvokeOnUIThread = InvokeOnUIThread;
        }

        public void InvokeOnAppThread(Action callback)
        {
            appCallbacks.InvokeOnAppThread(() => callback(), false);
        }

        public void InvokeOnUIThread(Action callback)
        {
            appCallbacks.InvokeOnUIThread(() => callback(), false);
        }

        void InitializeExceptionLogger()
        {
            // get a Raygun API key for client and uncomment next line
#if !(QA || DEBUG)
            MarkerMetro.Unity.WinIntegration.ExceptionLogger.Initialize("J5M66WHC/fIcZWudEXXGOw==");
#endif
        }

        void InitializePlatformSpecificMethods ()
        {
            MarkerMetro.Unity.WinIntegration.Helper.Instance.CheckIsLowEndDevice = IsLowEndDevice;
        }

        bool IsLowEndDevice ()
        {
#if UNITY_WP_8_1
            long result = 0;
            try
            {
                result = (long)MemoryManager.AppMemoryUsageLimit;
            }
            catch (ArgumentOutOfRangeException)
            {
                // The device does not support querying for this value. This occurs
                // on Windows Phone OS 7.1 and older phones without OS updates.
            }
            return result <= 188743680; // less than or equal to 180MB including failure scenario
#else
            var systemInfo = new SYSTEM_INFO();
            GetNativeSystemInfo(ref systemInfo);

            return systemInfo.wProcessorArchitecture == (uint)ProcessorArchitecture.ARM;
#endif
        }
	}
}