using System;
using System.Threading;

namespace NautilusChallenge
{
  /// <summary>
  /// Writes logs to the <see cref="LogStore"/>
  /// </summary>
  public class Logger : IContinueableLogger
  {
    private static Log continuingLog;
    private readonly LogStore logStore;

    public Logger(LogStore logStore = null)
    {
      // for unit testing purposes only, we allow a LogStore to be injected into this class
      this.logStore = logStore ?? LogStore.Instance;
    }

    /// <summary>
    /// Writes logs to the <see cref="LogStore"/>
    /// </summary>
    public void Log(string message)
    {
      Log(LogPriority.DefaultPriority, message);
    }

    /// <summary>
    /// Writes logs to the <see cref="LogStore"/>
    /// </summary>
    public void Log(int priority, string message)
    {
      // NOTE: the convention in .NET is to use default parameter values, where the optional parameters go last in the parameter list
      // In this case, the API surface feels more appropriate to list priority first, in which case we cannot take
      // advantage of default parameter value syntax, so we will have to define two constructors.

      var log = new Log(priority, message, DateTime.UtcNow, Thread.CurrentThread.Name);
      logStore.Logs[priority].Push(log);
    }

    /// <summary>
    /// Continue composing the previously created log entry. The message will be appended to the previous log message.
    /// The log will be written to the <see cref="LogStore"/> once the <see cref="Complete"/> method is called.
    /// </summary>
    public IContinueableLogger LogContinued(string message)
    {
      var priority = continuingLog != null ? continuingLog.Priority : LogPriority.DefaultPriority;
      message = (continuingLog != null ? continuingLog.Message + message : message);
      return LogContinued(priority, message);
    }

    /// <summary>
    /// Begin composing a log entry.
    /// The log will be written to the <see cref="LogStore"/> once the <see cref="Complete"/> method is called.
    /// </summary>
    public IContinueableLogger LogContinued(int priority, string message)
    {
      continuingLog = new Log(priority, message, DateTime.UtcNow, Thread.CurrentThread.Name);
      return this;
    }

    /// <summary>
    /// Write the previously created log entry to the <see cref="LogStore"/>
    /// </summary>
    public void Complete()
    {
      if (continuingLog == null)
      {
        return;
      }

      logStore.Logs[continuingLog.Priority].Push(continuingLog);
      continuingLog = null;
    }
  }

  /// <summary>
  /// Provides a fluent interface for <see cref="Logger.LogContinued"/>
  /// </summary>
  public interface IContinueableLogger
  {
    IContinueableLogger LogContinued(string message);
    void Complete();
  }
}
