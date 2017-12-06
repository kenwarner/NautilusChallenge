using System.Collections.Concurrent;

namespace NautilusChallenge
{
  /// <summary>
  /// Stores logs in a prioritized, ordered, thread-safe structure
  /// </summary>
  public class LogStore
  {
    // ConcurrentDictionary and ConcurrentStack are thread-safe by definition
    public ConcurrentDictionary<int, ConcurrentStack<Log>> Logs { get; private set; }

    internal static readonly LogStore Instance = new LogStore();

    public LogStore()
    {
      // initialize the Logs with empty structures
      Logs = new ConcurrentDictionary<int, ConcurrentStack<Log>>();
      for (int i = LogPriority.LowestPriority; i <= LogPriority.HighestPriority; i++)
      {
        Logs[i] = new ConcurrentStack<Log>();
      }
    }
  }
}
