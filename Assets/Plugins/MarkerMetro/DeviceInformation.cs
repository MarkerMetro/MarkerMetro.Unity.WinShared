using System;

namespace MarkerMetro.Unity.WinShared.Tools
{
    public static class DeviceInformation
    {
        public static Func<Environment> DoGetEnvironment;

        public static Environment GetEnvironment()
        {
            if (DoGetEnvironment != null)
                return DoGetEnvironment();
            else
                return Environment.Dev;
        }
    }

}
