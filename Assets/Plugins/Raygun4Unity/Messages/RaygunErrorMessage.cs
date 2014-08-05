using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Mindscape.Raygun4Unity.Messages
{
  public class RaygunErrorMessage
  {
    public RaygunErrorMessage()
    {
    }

    public RaygunErrorMessage(string message, string stackTrace)
    {
      if ("WP8Player".Equals(Application.platform.ToString()))
      {
        ParseWindowsPhoneMessage(message);
        if (!String.IsNullOrEmpty(stackTrace))
        {
          StackTrace = BuildStackTrace(stackTrace);
        }
      }
      else
      {
        Message = message;
        StackTrace = BuildStackTrace(stackTrace);
      }
    }

    public RaygunErrorMessage(Exception exception)
    {
      Type exceptionType = exception.GetType();

      Message = string.Format("{0}: {1}", exceptionType.Name, exception.Message);
      ClassName = exceptionType.FullName;

      StackTrace = BuildStackTrace(exception);
      Data = exception.Data;

      if (exception.InnerException != null)
      {
        InnerError = new RaygunErrorMessage(exception.InnerException);
      }
    }

    private void ParseWindowsPhoneMessage(string message)
    {
      RawMessage = message;

      if (!String.IsNullOrEmpty(message))
      {
        string exception = null;
        string type = null;

        int exceptionIndex = message.IndexOf("Exception: ");
        if (exceptionIndex >= 0)
        {
          int endExceptionIndex = message.IndexOf('\n', exceptionIndex);
          if (endExceptionIndex >= 0)
          {
            exception = message.Substring(exceptionIndex + 11, endExceptionIndex - exceptionIndex - 11);
          }
        }

        int typeIndex = message.IndexOf("Type: ");
        if (typeIndex >= 0)
        {
          int endTypeIndex = message.IndexOf('\n', typeIndex);
          if (endTypeIndex >= 0)
          {
            type = message.Substring(typeIndex + 6, endTypeIndex - typeIndex - 6);
          }
        }

        ClassName = type;
        int index = type.LastIndexOf(".");
        if (index >= 0)
        {
          string exceptionType = type.Substring(index + 1);
          Message = exceptionType + ": " + exception;
        }

        if (String.IsNullOrEmpty(Message))
        {
          Message = message;
        }

        int stackIndex = message.IndexOf("   at ");
        if (stackIndex >= 0)
        {
          string stackTrace = message.Substring(stackIndex);
          StackTrace = BuildStackTrace(stackTrace);
        }
      }
    }

    // Unity stack trace parsing
    private RaygunErrorStackTraceLineMessage[] BuildStackTrace(string stackTrace)
    {
      List<RaygunErrorStackTraceLineMessage> lines = new List<RaygunErrorStackTraceLineMessage>();

      if (stackTrace == null)
      {
        RaygunErrorStackTraceLineMessage line = new RaygunErrorStackTraceLineMessage();
        line.FileName = "none";
        line.LineNumber = 0;

        lines.Add(line);
        return lines.ToArray();
      }

      try
      {
        string[] stackTraceLines = stackTrace.Split('\r', '\n');
        foreach (string stackTraceLine in stackTraceLines)
        {
          if (!String.IsNullOrEmpty(stackTraceLine))
          {
            int lineNumber = 0;
            string fileName = null;
            string methodName = null;
            string className = null;
            string stackTraceLn = stackTraceLine;
            // Line number
            int index = stackTraceLine.LastIndexOf(":");
            if (index > 0)
            {
              string lineNumberString = stackTraceLn.Substring(index + 1).Replace(")", "");
              bool success = int.TryParse(lineNumberString, out lineNumber);
              if (success)
              {
                stackTraceLn = stackTraceLn.Substring(0, index);
              }
            }
            // File name
            index = stackTraceLn.LastIndexOf(" (at ");
            if (index > 0)
            {
              fileName = stackTraceLn.Substring(index + 5);
              stackTraceLn = stackTraceLn.Substring(0, index);
            }
            // Method name
            index = stackTraceLn.LastIndexOf("(");
            if (index > 0)
            {
              index = stackTraceLn.LastIndexOf(".", index);
              if (index > 0)
              {
                methodName = stackTraceLn.Substring(index + 1).Trim();
                methodName = methodName.Replace(" (", "(");
                stackTraceLn = stackTraceLn.Substring(0, index);
              }
              // Class name
              className = stackTraceLn.Trim();
              if (className.StartsWith("at "))
              {
                className = className.Substring(3);
              }
            }
            else
            {
              fileName = stackTraceLn;
            }
            RaygunErrorStackTraceLineMessage line = new RaygunErrorStackTraceLineMessage();
            line.FileName = fileName;
            line.LineNumber = lineNumber;
            line.MethodName = methodName;
            line.ClassName = className;

            lines.Add(line);
          }
        }
        if (lines.Count > 0)
        {
          return lines.ToArray();
        }
      }
      catch (Exception ex)
      {
        RaygunClient.Log(string.Format("Error parsing Unity stack trace: {0}", ex.Message));
      }

      return lines.ToArray();
    }

    private RaygunErrorStackTraceLineMessage[] BuildStackTrace(Exception exception)
    {
      List<RaygunErrorStackTraceLineMessage> lines = new List<RaygunErrorStackTraceLineMessage>();

      string stackTraceStr = exception.StackTrace;
      if (stackTraceStr == null)
      {
        RaygunErrorStackTraceLineMessage line = new RaygunErrorStackTraceLineMessage();
        line.FileName = "none";
        line.LineNumber = 0;

        lines.Add(line);
        return lines.ToArray();
      }
      try
      {
        string[] stackTraceLines = stackTraceStr.Split('\n');
        foreach (string stackTraceLine in stackTraceLines)
        {
          int lineNumber = 0;
          string fileName = null;
          string methodName = null;
          string className = null;
          string stackTraceLn = stackTraceLine;
          // Line number
          int index = stackTraceLine.LastIndexOf(":");
          if (index > 0)
          {
            bool success = int.TryParse(stackTraceLn.Substring(index + 1), out lineNumber);
            if (success)
            {
              stackTraceLn = stackTraceLn.Substring(0, index);
              // File name
              index = stackTraceLn.LastIndexOf("] in ");
              if (index > 0)
              {
                fileName = stackTraceLn.Substring(index + 5);
                if ("<filename unknown>".Equals(fileName))
                {
                  fileName = null;
                }
                stackTraceLn = stackTraceLn.Substring(0, index);
                // Method name
                index = stackTraceLn.LastIndexOf("(");
                if (index > 0)
                {
                  index = stackTraceLn.LastIndexOf(".", index);
                  if (index > 0)
                  {
                    int endIndex = stackTraceLn.IndexOf("[0x");
                    if (endIndex < 0)
                    {
                      endIndex = stackTraceLn.Length;
                    }
                    methodName = stackTraceLn.Substring(index + 1, endIndex - index - 1).Trim();
                    methodName = methodName.Replace(" (", "(");
                    stackTraceLn = stackTraceLn.Substring(0, index);
                  }
                }
                // Class name
                index = stackTraceLn.IndexOf("at ");
                if (index >= 0)
                {
                  className = stackTraceLn.Substring(index + 3);
                }
              }
              else
              {
                fileName = stackTraceLn;
              }
            }
            else
            {
              index = stackTraceLn.IndexOf("at ");
              if (index >= 0)
              {
                index += 3;
              }
              else
              {
                index = 0;
              }
              fileName = stackTraceLn.Substring(index);
            }
          }
          else
          {
            fileName = stackTraceLn;
          }
          var line = new RaygunErrorStackTraceLineMessage
          {
            FileName = fileName,
            LineNumber = lineNumber,
            MethodName = methodName,
            ClassName = className
          };

          lines.Add(line);
        }
        if (lines.Count > 0)
        {
          return lines.ToArray();
        }
      }
      catch (Exception ex)
      {
        RaygunClient.Log(string.Format("Error parsing .Net stack trace: {0}", ex.Message));
      }

      return lines.ToArray();
    }

    public RaygunErrorMessage InnerError { get; set; }

    public IDictionary Data { get; set; }

    public string ClassName { get; set; }

    public string Message { get; set; }

    public string RawMessage { get; set; }

    public RaygunErrorStackTraceLineMessage[] StackTrace { get; set; }
  }
}
