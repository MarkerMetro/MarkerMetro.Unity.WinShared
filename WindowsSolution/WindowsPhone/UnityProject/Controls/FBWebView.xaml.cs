using Windows.UI.Core;
using MarkerMetro.Unity.WinIntegration.Facebook;
using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Shell;

namespace UnityProject.WinPhone.Controls
{
    public partial class FBWebView : UserControl, IWebInterface
    {
        private NavigationEventCallback _onFinished;
        private NavigationErrorCallback _onError;
        private NavigationEventCallback _onStart;
        private object _state;
        private ProgressIndicator _prog;

        public FBWebView()
        {
            InitializeComponent();
            web.Navigated += NavigationFinished;
            web.Navigating += NavigationStarted;
            web.NavigationFailed += NavigationFailed;
            web.NavigateToString("");
            web.IsScriptEnabled = true;
            _prog = new ProgressIndicator()
            {
                IsIndeterminate = true,
                IsVisible = false,
                Text = "Loading..."
            };
        }

        public void Cancel()
        {
            if (IsActive)
            {
                StopProgress();
                if (_onError != null)
                    _onError(null, -1, _state);
                Finish();
            }
        }

        private void StartProgress()
        {
            SystemTray.IsVisible = true;
            SystemTray.SetProgressIndicator((App.Current.RootVisual as PhoneApplicationFrame).Content as PhoneApplicationPage, _prog);
            _prog.IsVisible = true;
        }

        private void StopProgress()
        {
            _prog.IsVisible = false;
            SystemTray.IsVisible = false;
        }

        private void NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            StopProgress();
            if (_onError != null)
                _onError(e.Uri, 1, _state);
        }

        private void NavigationStarted(object sender, NavigatingEventArgs e)
        {
            StartProgress();
            if (_onStart != null)
                _onStart(e.Uri, _state);
        }

        private void NavigationFinished(object sender, NavigationEventArgs e)
        {
            StopProgress();
            if (_onFinished != null)
                _onFinished(e.Uri, _state);
        }

        public void Finish()
        {
            Dispatcher.BeginInvoke(() =>
            {
                StopProgress();
                _onError = null;
                _onStart = null;
                _onFinished = null;
                if (web.Visibility == Visibility.Visible)
                {
                    web.NavigateToString("");
                    this.Visibility = Visibility.Collapsed;
                }
                IsActive = false;
            });
        }

        public bool IsActive
        {
            get;
            private set;
        }

        public void Navigate(
            Uri uri,
            NavigationEventCallback finishedCallback,
            NavigationErrorCallback onError,
            object state = null,
            NavigationEventCallback startedCallback = null)
        {
            _onFinished = finishedCallback;
            _onStart = startedCallback;
            _onError = onError;
            _state = state;

            Dispatcher.BeginInvoke(() =>
            {
                this.Visibility = Visibility.Visible;
                IsActive = true;
                web.Navigate(uri);
            });
        }
    }
}
