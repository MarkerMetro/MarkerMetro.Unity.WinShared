using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityProject.WinPhone
{
    [Serializable]
    public class WrappedException : Exception
    {
        readonly string _stackTrace;
        public WrappedException() { }
        public WrappedException(string message) : base(message) { }
        public WrappedException(string message, string stackTrace)
            : base(message)
        {
            _stackTrace = stackTrace;
        }

        public override string StackTrace
        {
            get
            {
                if (_stackTrace == null)
                    return base.StackTrace;
                else
                    return _stackTrace;
            }
        }
    }
}
