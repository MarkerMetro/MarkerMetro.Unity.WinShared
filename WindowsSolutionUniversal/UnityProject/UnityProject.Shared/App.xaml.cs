using System;
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
using MarkerMetro.Unity.WinIntegration.Logging;
using UnityProject.Config;
using UnityProject.Logging;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace UnityProject
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private AppCallbacks appCallbacks;
        internal SplashScreen SplashScreen { get; private set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            appCallbacks = new AppCallbacks();

            UnhandledException += LogUnhandledException;

            // Prevents display to dim while the app is visible:
            var displayRequest = new Windows.System.Display.DisplayRequest();
            displayRequest.RequestActive();

            InitializeExceptionLogger();

#if DEBUG
            DebugSettings.EnableFrameRateCounter = true;
#endif
        }

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
                    if (ExceptionLogger.IsEnabled)
                    {                        
                        ExceptionLogger.Send(e.Exception);
                        ExceptionLogger.IsEnabled = AppConfig.Instance.ExceptionLoggingAllowed;
                    }
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

            switch (args.Kind)
            {
                case ActivationKind.Protocol:
                    ProtocolActivatedEventArgs eventArgs = args as ProtocolActivatedEventArgs;
                    SplashScreen = eventArgs.SplashScreen;
                    appArgs += string.Format("Uri={0}", eventArgs.Uri.AbsoluteUri);

#if UNITY_WP_8_1
                    MarkerMetro.Unity.WinIntegration.Facebook.FBNative.MapUri(eventArgs.Uri);
#endif
                    break;
            }
            InitializeUnity(appArgs);
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            SplashScreen = args.SplashScreen;
            InitializeUnity(args.Arguments);
        }

        private void InitializeUnity(string args)
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

                rootFrame = new Frame();
                Window.Current.Content = rootFrame;
                Window.Current.Activate();

                rootFrame.Navigate(typeof(MainPage));

                AppCallBacksInitialized();
            }

            Window.Current.Activate();
        }

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
    }
}