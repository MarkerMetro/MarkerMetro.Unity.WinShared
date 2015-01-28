﻿using MarkerMetro.Common.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkerMetro.Common.Converters
{
    public class ScaledImageLocator
    {
        public Uri this[string imagePath]
        {
            get
            {
                // This presumes a 3 character file extension. Change if this assumption proves incorrect.

                if (string.IsNullOrEmpty(imagePath) || imagePath.Length < 4)
                {
                    return new Uri(imagePath, UriKind.Relative);
                }

                string res;
                switch (ResolutionHelper.CurrentResolution)
                {
                    case Resolutions.HD:
                        res = "720p";
                        break;
                    case Resolutions.WXGA:
                        res = "WXGA";
                        break;
                    case Resolutions.WVGA:
                        res = "WVGA";
                        break;
                    default:
                        throw new InvalidOperationException("Unknown resolution type");
                }

                var uri = new Uri(String.Format("{0}.Screen-{1}{2}", imagePath.Substring(0, imagePath.Length - 4), res, imagePath.Substring(imagePath.Length - 4)), UriKind.Relative);

                return uri;
            }
        }
    }
}