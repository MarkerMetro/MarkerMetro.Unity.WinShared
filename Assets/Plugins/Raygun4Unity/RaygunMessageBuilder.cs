using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Mindscape.Raygun4Unity.Messages;

namespace Mindscape.Raygun4Unity
{
  public class RaygunMessageBuilder : IRaygunMessageBuilder
  {
    public static RaygunMessageBuilder New
    {
      get
      {
        return new RaygunMessageBuilder();
      }
    }

    private readonly RaygunMessage _raygunMessage;

    private RaygunMessageBuilder()
    {
      _raygunMessage = new RaygunMessage();
    }

    public RaygunMessage Build()
    {
      return _raygunMessage;
    }

    public IRaygunMessageBuilder SetMachineName(string machineName)
    {
      _raygunMessage.Details.MachineName = machineName;
      return this;
    }

    public IRaygunMessageBuilder SetEnvironmentDetails()
    {
      _raygunMessage.Details.Environment = new RaygunEnvironmentMessage();
      return this;
    }

    public IRaygunMessageBuilder SetExceptionDetails(string message, string stackTrace)
    {
      _raygunMessage.Details.Error = new RaygunErrorMessage(message, stackTrace);
      return this;
    }

    public IRaygunMessageBuilder SetExceptionDetails(Exception exception)
    {
      _raygunMessage.Details.Error = new RaygunErrorMessage(exception);
      return this;
    }

    public IRaygunMessageBuilder SetClientDetails()
    {
      _raygunMessage.Details.Client = new RaygunClientMessage();
      return this;
    }

    public IRaygunMessageBuilder SetUserCustomData(IDictionary userCustomData)
    {
      _raygunMessage.Details.UserCustomData = userCustomData;
      return this;
    }

    public IRaygunMessageBuilder SetTags(IList<string> tags)
    {
      _raygunMessage.Details.Tags = tags;
      return this;
    }

    public IRaygunMessageBuilder SetUser(RaygunIdentifierMessage user)
    {
      _raygunMessage.Details.User = user;
      return this;
    }

    public IRaygunMessageBuilder SetVersion(string version)
    {
      if (!String.IsNullOrEmpty(version))
      {
        _raygunMessage.Details.Version = version;
      }
      else
      {
        _raygunMessage.Details.Version = "Not supplied";
      }
      return this;
    }
  }
}
