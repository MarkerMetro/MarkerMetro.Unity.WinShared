using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MarkerMetro;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MarkerMetro.WinIntegration.Facebook;

namespace UnityProject.WinPhone.Controls
{
    public partial class FBWebView : UserControl, IWebInterface
    {
        private NavigationEventCallback _onFinished;
        private NavigationErrorCallback _onError;
        private NavigationEventCallback _onStart;
        private object _state;

        public FBWebView()
        {
            InitializeComponent();
            web.Navigated += NavigationFinished;
            web.Navigating += NavigationStarted;
            web.NavigationFailed += NavigationFailed;
            web.NavigateToString("");
        }

        private void NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (_onError != null)
                _onError(e.Uri, 1, _state);
        }

        private void NavigationStarted(object sender, NavigatingEventArgs e)
        {
            if (_onStart != null)
                _onStart(e.Uri, _state);
        }

        private void NavigationFinished(object sender, NavigationEventArgs e)
        {
            if (_onFinished != null)
                _onFinished(e.Uri, _state);
        }

        public void Finish()
        {
            _onError = null;
            _onStart = null;
            _onFinished = null;
            web.NavigateToString("");
            this.Visibility = Visibility.Collapsed;
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

            Dispatcher.BeginInvoke(() =>
            {
                this.Visibility = Visibility.Visible;
                IsActive = true;
                web.Navigate(uri);
            });
        }
    }
}
