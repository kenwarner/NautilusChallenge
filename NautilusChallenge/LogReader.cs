namespace NautilusChallenge
{
  /// <summary>
  /// Gets logs from the <see cref="LogStore"/>
  /// </summary>
  public class LogReader
  {
    private readonly LogStore logStore;

    public LogReader(LogStore logStore = null)
    {
      // for unit testing purposes only, we allow a LogStore to be injected into this class
      this.logStore = logStore ?? LogStore.Instance;
    }

    /// <summary>
    /// Gets the highest priority, most recently logged message from the <see cref="LogStore"/>
    /// </summary>
    /// <returns>the message</returns>
    public string Get()
    {
      for (int priorityIndex = LogPriority.HighestPriority; priorityIndex >= LogPriority.LowestPriority; priorityIndex--)
      {
        Log log = null;
        var doesLogExist = logStore.Logs[priorityIndex].TryPop(out log);
        if (doesLogExist)
        {
          return log.Message;
        }
      }

      return null;
    }
  }
}
