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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using UnityPlayer;
using MarkerMetro.Unity.WinIntegration.Store;
using System.Diagnostics;
// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace UnityProject.Win
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	sealed partial class App : Application
	{
		private WinRTBridge.WinRTBridge _bridge;
		private AppCallbacks appCallbacks;
		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			this.InitializeComponent();

            MarkerMetro.Unity.WinIntegration.SharedLogger.Instance = new RaygunSharedLogger();

			appCallbacks = new AppCallbacks(false);
            appCallbacks.RenderingStarted += AppCallBacks_Initialized;
            UnhandledException += App_UnhandledException;
		}

        void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                MarkerMetro.Unity.WinIntegration.SharedLogger.Instance.Send(e.Exception);
            }
            catch (Exception ex)
            {
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
			appCallbacks.SetAppArguments(args);
			Frame rootFrame = Window.Current.Content as Frame;

			// Do not repeat app initialization when the Window already has content,
			// just ensure that the window is active
			if (rootFrame == null && !appCallbacks.IsInitialized())
			{

                // Initialise Store system
#if QA || DEBUG
                StoreManager.Instance.Initialise(true);
#else
                StoreManager.Instance.Initialise(false);
#endif

				var mainPage = new MainPage(splashScreen);
				Window.Current.Content = mainPage;
				Window.Current.Activate();

				// Setup scripting bridge
				_bridge = new WinRTBridge.WinRTBridge();
				appCallbacks.SetBridge(_bridge);

				appCallbacks.SetKeyboardTriggerControl(mainPage);
                appCallbacks.SetSwapChainPanel(mainPage.GetSwapChainPanel());


				appCallbacks.SetCoreWindowEvents(Window.Current.CoreWindow);

				appCallbacks.InitializeD3DXAML();
			}

			Window.Current.Activate();
		}


        void AppCallBacks_Initialized()
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
