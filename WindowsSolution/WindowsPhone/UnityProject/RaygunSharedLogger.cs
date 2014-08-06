using MarkerMetro.Unity.WinIntegration;
using Mindscape.Raygun4Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityProject.WinPhone
{
    public class RaygunSharedLogger : MarkerMetro.Unity.WinIntegration.SharedLogger
    {
        Lazy<RaygunClient> Client = new Lazy<RaygunClient>(
            () => new RaygunClient("J5M66WHC/fIcZWudEXXGOw==")
            {
                ApplicationVersion = Helper.Instance.GetAppVersion(),
                // User = Helper.Instance.GetUserDeviceId(),
                // UserInfo = 
            });

        public override void Send(Exception ex)
        {
            Client.Value.Send(ex);
        }

        public override void Send(string message, string stackTrace)
        {
            Client.Value.Send(new WrappedException(message, stackTrace));
        }
    }
}
