using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MarkerMetro.Unity.WinIntegration;
using Mindscape.Raygun4Net;

namespace UnityProject.Win
{
    public class RaygunSharedLogger : MarkerMetro.Unity.WinIntegration.SharedLogger
    {
        public override void Send(Exception ex)
        {
            new RaygunClient("J5M66WHC/fIcZWudEXXGOw==").Send(ex);
        }

        public override void Send(string message, string stackTrace)
        {
            new RaygunClient("J5M66WHC/fIcZWudEXXGOw==").Send(new Exception(message));
        }
    }
}
