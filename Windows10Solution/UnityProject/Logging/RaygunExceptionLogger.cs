﻿//using System;
//using System.Diagnostics;

//using Mindscape.Raygun4Net;
//using MarkerMetro.Unity.WinIntegration.Logging;
//using MarkerMetro.Unity.WinIntegration;

//namespace UnityProject.Logging
//{

//    /// <summary>
//    /// Implementation of IExceptionLogger for Raygun Exception Logger.
//    /// </summary>
//    public sealed class RaygunExceptionLogger : IExceptionLogger
//    {
//        Lazy<RaygunClient> _raygun;

//        public bool IsEnabled { get; set; }

//        public RaygunExceptionLogger(string apiKey)
//        {
//            _raygun = new Lazy<RaygunClient>(() => BuildRaygunClient(apiKey));
//        }

//        RaygunClient BuildRaygunClient(string apiKey)
//        {
//            string version = null, user = null;

//            version = Helper.Instance.GetAppVersion();
//            try
//            {
//                user = Helper.Instance.GetUserDeviceId();
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine("Failed to get UserDeviceId: {0}", ex);
//            }

//            return new RaygunClient(apiKey)
//            {
//                ApplicationVersion = version,
//                User = user,
//            };
//        }

//        public void Send(Exception ex)
//        {
//            if (_raygun != null)
//            {
//                _raygun.Value.Send(ex);
//            }
//        }

//        public void Send(string message, string stackTrace)
//        {
//            if (_raygun != null)
//            {
//                _raygun.Value.Send(new WrappedException(message, stackTrace));
//            }
//        }
//    }
//}