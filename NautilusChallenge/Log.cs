using System;

namespace NautilusChallenge
{
  /// <summary>
  /// A log message and associated metadata
  /// </summary>
  public class Log
  {
    public int Priority { get; private set; }
    public string Message { get; private set; }
    public DateTime Timestamp { get; set; }
    public string ThreadName { get; set; }

    public Log(int priority, string message, DateTime timestamp, string threadName)
    {
      if (priority < LogPriority.LowestPriority || priority > LogPriority.HighestPriority)
      {
        throw new ArgumentOutOfRangeException(nameof(priority));
      }

      Priority = priority;
      Message = message;
      Timestamp = timestamp;
      ThreadName = threadName;
    }
  }
}