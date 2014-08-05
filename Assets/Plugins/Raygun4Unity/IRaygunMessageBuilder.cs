using System;
using System.Collections;
using System.Collections.Generic;
using Mindscape.Raygun4Unity.Messages;

namespace Mindscape.Raygun4Unity
{
  public interface IRaygunMessageBuilder
  {
    RaygunMessage Build();

    IRaygunMessageBuilder SetMachineName(string machineName);

    IRaygunMessageBuilder SetExceptionDetails(string message, string stackTrace);

    IRaygunMessageBuilder SetExceptionDetails(Exception exception);

    IRaygunMessageBuilder SetClientDetails();

    IRaygunMessageBuilder SetEnvironmentDetails();

    IRaygunMessageBuilder SetVersion(string version);

    IRaygunMessageBuilder SetUserCustomData(IDictionary userCustomData);

    IRaygunMessageBuilder SetTags(IList<string> tags);

    IRaygunMessageBuilder SetUser(RaygunIdentifierMessage user);
  }
}