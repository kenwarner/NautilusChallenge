using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NautilusChallenge.Tests
{
    public class LogReaderTests
    {
	    [Fact]
	    public void GivenTestCase()
	    {
			var logStore = new LogStore();
		    var logger = new Logger(logStore);
		    logger.Log(1, "abc");
		    logger.Log(2, "def");
		    logger.Log(1, "ghi");

			var sut = new LogReader(logStore);

			Assert.Equal("def", sut.Get());
			Assert.Equal("ghi", sut.Get());
			Assert.Equal("abc", sut.Get());
			Assert.Null(sut.Get());
		}

	    [Fact]
	    public void OneLoggerWithOneLog()
	    {
		    var log1 = "abc";
			var logStore = new LogStore();
			var logger1 = new Logger(logStore);
		    logger1.Log(log1);

			var sut = new LogReader(logStore);

			Assert.Equal(log1, sut.Get());
	    }

		[Fact]
		public void OneLoggerWithTwoLogs()
		{
			var log1 = "abc";
			var log2 = "def";
			var logStore = new LogStore();
			var logger = new Logger(logStore);
			logger.Log(log1);
			logger.Log(log2);

			var sut = new LogReader(logStore);

			Assert.Equal(log2, sut.Get());
			Assert.Equal(log1, sut.Get());
		}

		[Fact]
		public void OneLoggerWithTwoLogsOfDifferentPriority()
		{
			var log1 = "abc";
			var log2 = "def";
			var logStore = new LogStore();
			var logger = new Logger(logStore);
			logger.Log(3, log1);
			logger.Log(1, log2);

			var sut = new LogReader(logStore);

			Assert.Equal(log1, sut.Get());
			Assert.Equal(log2, sut.Get());
		}

		[Fact]
		public void TwoLoggersWithOneLogEach()
		{
			var log1 = "abc";
			var logStore = new LogStore();
			var logger1 = new Logger(logStore);
			logger1.Log(log1);

			var log2 = "def";
			var logger2 = new Logger(logStore);
			logger2.Log(log2);

			var sut = new LogReader(logStore);

			Assert.Equal(log2, sut.Get());
			Assert.Equal(log1, sut.Get());
		}

		[Fact]
		public void TwoLoggersWithOneLogEachOfDifferentPriority()
		{
			var log1 = "abc";
			var log2 = "def";
			var logStore = new LogStore();
			var logger1 = new Logger(logStore);
			logger1.Log(3, log1);

			var logger2 = new Logger(logStore);
			logger2.Log(1, log2);

			var sut = new LogReader(logStore);

			Assert.Equal(log1, sut.Get());
			Assert.Equal(log2, sut.Get());
		}

		[Fact]
		public void TwoLoggersWithOneLogEachOfDifferentPriorityAddedInOppositeOrder()
		{
			var log1 = "abc";
			var log2 = "def";
			var logStore = new LogStore();
			var logger1 = new Logger(logStore);
			logger1.Log(1, log1);

			var logger2 = new Logger(logStore);
			logger2.Log(3, log2);

			var sut = new LogReader(logStore);

			Assert.Equal(log2, sut.Get());
			Assert.Equal(log1, sut.Get());
		}

		[Fact]
		public void TwoLoggersWithOneLogEachWithTwoReaders()
		{
			var log1 = "abc";
			var log2 = "def";
			var logStore = new LogStore();
			var logger1 = new Logger(logStore);
			logger1.Log(3, log1);

			var logger2 = new Logger(logStore);
			logger2.Log(1, log2);

			var sut1 = new LogReader(logStore);
			Assert.Equal(log1, sut1.Get());

			var sut2 = new LogReader(logStore);
			Assert.Equal(log2, sut2.Get());
		}

		[Fact]
		public void LogContinueWithNoLogs()
		{
			var logStore = new LogStore();
			var logger1 = new Logger(logStore);
			logger1.LogContinued(null).Complete();

			var sut = new LogReader(logStore);

			Assert.Equal(null, sut.Get());
		}

		[Fact]
		public void LogContinue()
		{
			var log1 = "abc";
			var log2 = "def";
			var logStore = new LogStore();
			var logger1 = new Logger(logStore);
			logger1.LogContinued(log1).LogContinued(log2).Complete();

			var sut = new LogReader(logStore);

			Assert.Equal(log1 + log2, sut.Get());
		}

		[Fact]
		public void LogContinueButNoCompletion()
		{
			var log1 = "abc";
			var log2 = "def";
			var logStore = new LogStore();
			var logger1 = new Logger(logStore);
			var continuation = logger1.LogContinued(log1).LogContinued(log2);

			var sut = new LogReader(logStore);

			Assert.Null(sut.Get());

			continuation.Complete();
		}

		[Fact]
		public void LogContinueWithIntermediateStep()
		{
			var log1 = "abc";
			var log2 = "def";
			var log3 = "ghi";
			var logStore = new LogStore();
			var logger1 = new Logger(logStore);
			var continuation = logger1.LogContinued(log1);
			
			logger1.Log(log3);
			
			continuation.LogContinued(log2).Complete();

			var sut = new LogReader(logStore);

			Assert.Equal(log1 + log2, sut.Get());
			Assert.Equal(log3, sut.Get());
		}
	}
}
