Nautilus Coding Challenge

# Design

This logging library follows the Logger + LogReader pattern. The shared state is represented by the LogStore class.  
LogStore uses a thread-safe ConcurrentDictionary and ConcurrentStack. Both writes and reads are fast. O(1) read access on the dictionary according to priority followed by O(1) access on the stack for the most recent log in that priority.  
If the common use case ends up being most logs are written as lowest priority, the LogReader will make 2 unnecessary accesses on the dictionary. If this is the case, we can change LogReader to check the size of each object in the dictionary before attempting to pop off the stack.  
However, if we do that and the common use case ends up being most LogReader.Get() calls will return an item in the highest priority stack, then checking the size becomes a wasteful step. In practice we would observe typical usage patterns and adjust the implementation accordingly.

## Memory Constraints

This library will throw OutOfMemoryException if memory constraints are violated. A few more sophisticated options are available.  
Fundamentally, this will involve shuffling in-memory data that we don't anticipate being retrieved soon to the file system or other storage, which will be more abundant, but also slower to access, and adds complexity of maintaining sync with the in-memory structures. Think of it like a Level 1 vs Level 2 cache.  
If we're willing to take on a lightweight dependency like sqlite, then we have a robust, familiar and easy to integrate option that will manage storing data to disk when it does not fit in its available memory space.  
We may even want to consider using sqlite for the entire LogStorage implementation rather than write our own code to manage the two "cache levels" as sqlite may be fast enough on its own, and would reduce our own code's complexity (and therefore potential for bugs, as well as time to market).  
If sqlite is not a dependency we'd be willing to take on, we'd likely start with a partioning strategy as the LogStorage size grows, taking fixed-sized chunks out from the least-prioritized and least-recent parts of the data structures, serializing to disk, and periodically bringing those chunks back into the in-memory stucture whenever memory pressure subsides.

# Unit Tests

This project has 90%+ unit test coverage. A few considerations were made in this project to better accomodate unit testing requirements given the time constraints.  

The LogStore class is stubbed out and injected into the Logger and LogReader instances in the tests. This implementation uses constructor injection for simplicity.  
In a production implementation we would want to ensure that the LogStore is shared state across all instances of Logger and LogReader.  
Consumers of this library may see the constructors that allow passing in your own LogStore object and jeopardize the shared state design assumption.  
This can be solved with a proper dependency injection framework that can be configured to inject a singleton of LogStore into Logger and LogReader and allow us to eliminate the potentially troublesome constructors.  
Furthermore, unit testing for thread-safety is often challenging and unreliable. I have not attempted to unit test for thread-safety, and am relying on the thread-safe guarantees of the ConcurrentDictionary and ConcurrentStack data types used by LogStore.