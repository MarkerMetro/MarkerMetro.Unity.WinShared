using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace UnityProject.WinPhone
{
    public class ScaledLocalizedImageLocator
    {
        private static readonly ScaledLocalizedImageLocator _current = new ScaledLocalizedImageLocator();

        public static ScaledLocalizedImageLocator Current
        {
            get
            {
                return _current;
            }
        }

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
                    case Resolutions.HD720p:
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

                var locale = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;

                var uri = string.Format("{0}.Screen-{1}.{2}{3}", imagePath.Substring(0, imagePath.Length - 4), res, locale, imagePath.Substring(imagePath.Length - 4));

                if (File.Exists(Package.Current.InstalledLocation.Path + @"\" + uri.Substring(1)))
                {
                    return new Uri(uri, UriKind.Relative);
                }
                else
                {
                    return new Uri(String.Format("{0}.Screen-{1}{2}", imagePath.Substring(0, imagePath.Length - 4), res, imagePath.Substring(imagePath.Length - 4)), UriKind.Relative);
                }
            }
        }
    }
}
