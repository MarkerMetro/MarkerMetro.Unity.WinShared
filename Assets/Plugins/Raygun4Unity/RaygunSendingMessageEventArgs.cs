using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Mindscape.Raygun4Unity.Messages;

namespace Mindscape.Raygun4Unity
{
  /// <summary>
  /// Can be used to modify the message before sending, or to cancel the send operation.
  /// </summary>
  public class RaygunSendingMessageEventArgs : CancelEventArgs
  {
    public RaygunSendingMessageEventArgs(RaygunMessage message)
    {
      Message = message;
    }

    public RaygunMessage Message { get; private set; }
  }
}
