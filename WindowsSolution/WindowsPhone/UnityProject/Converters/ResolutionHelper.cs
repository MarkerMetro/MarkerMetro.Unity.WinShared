﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MarkerMetro.Common.Framework
{
    public enum Resolutions { WVGA, WXGA, HD };

    public static class ResolutionHelper
    {
        private static bool IsWvga
        {
            get
            {
                return Application.Current.Host.Content.ScaleFactor == 100;
            }
        }

        private static bool IsWxga
        {
            get
            {
                return Application.Current.Host.Content.ScaleFactor == 160;
            }
        }

        private static bool IsHD
        {
            get
            {
                return Application.Current.Host.Content.ScaleFactor == 150;
            }
        }

        public static Resolutions CurrentResolution
        {
            get
            {
                if (IsWvga) return Resolutions.WVGA;
                else if (IsWxga) return Resolutions.WXGA;
                else if (IsHD) return Resolutions.HD;
                //default?
                return Resolutions.WVGA;
            }
        }
    }
}