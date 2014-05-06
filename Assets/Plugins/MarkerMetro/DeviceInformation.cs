using System;

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

    public enum Environment
    {
        Dev,
        QA,
        Production
    }
}
