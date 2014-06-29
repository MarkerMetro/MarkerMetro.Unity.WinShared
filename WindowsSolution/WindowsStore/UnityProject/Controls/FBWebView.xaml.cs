using MarkerMetro;
using MarkerMetro.WinIntegration.Facebook;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UnityProject.Win.Controls
{
    public sealed partial class FBWebView : UserControl, IWebInterface
    {
        private NavigationEventCallback _onFinished;
        private NavigationErrorCallback _onError;
        private NavigationEventCallback _onStart;
        private object _state;

        public FBWebView()
        {
            this.InitializeComponent();
            web.NavigationStarting += HandleNavigationStarting;
            web.NavigationCompleted += HandleNavigationCompleted;
            web.NavigationFailed += HandleNavigationFailed;
        }

        private void HandleNavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            var uri = e.Uri;
            var error = e.WebErrorStatus;
            UnityPlayer.AppCallbacks.Instance.InvokeOnAppThread(() =>
            {
                if (_onError != null)
                    _onError(uri, (int)error, _state);
            }, false);
        }

        private void HandleNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            var uri = args.Uri;
            var error = args.WebErrorStatus;
            if (args.IsSuccess)
            {
                UnityPlayer.AppCallbacks.Instance.InvokeOnAppThread(() =>
                {
                    if (_onFinished != null)
                        _onFinished(uri, _state);
                }, false);
            }
            else
            {
                UnityPlayer.AppCallbacks.Instance.InvokeOnAppThread(() =>
                {
                    if (_onError != null)
                        _onError(uri, (int)error, _state);
                }, false);
            }
        }

        private void HandleNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            var uri = args.Uri;
            UnityPlayer.AppCallbacks.Instance.InvokeOnAppThread(() =>
            {
                if (_onStart != null)
                    _onStart(uri, _state);
            }, false);
        }

        public void Finish()
        {
            UnityPlayer.AppCallbacks.Instance.InvokeOnUIThread(() => CloseFB(), true);
        }

        void CloseFB()
        {
            _onError = null;
            _onStart = null;
            _onFinished = null;
            web.Stop();
            this.Visibility = Visibility.Collapsed;
            UnityPlayer.AppCallbacks.Instance.UnitySetInput(true);
            IsActive = false;
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

            UnityPlayer.AppCallbacks.Instance.InvokeOnUIThread(() =>
            {
                this.Visibility = Visibility.Visible;
                UnityPlayer.AppCallbacks.Instance.UnitySetInput(false);
                IsActive = true;
                web.Navigate(uri);
            }, false);
        }

        void backButton_Click(object sender, RoutedEventArgs e)
        {
            CloseFB();
        }
    }
}
