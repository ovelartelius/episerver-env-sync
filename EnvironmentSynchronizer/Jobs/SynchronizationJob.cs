using System;
using System.Diagnostics;
using System.Reflection;
using EnvironmentSynchronizer.Business;
using EPiServer.Logging.Compatibility;
using EPiServer.PlugIn;
using EPiServer.Scheduler;

namespace EnvironmentSynchronizer.Jobs
{
	[ScheduledPlugIn(DisplayName = "Environment synchronization", SortIndex = 100)]
	public class SynchronizationJob : ScheduledJobBase
	{
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public SynchronizationJob()
		{
			IsStoppable = false;
		}

		private long Duration { get; set; }

		public override string Execute()
		{
			var tmr = Stopwatch.StartNew();

			try
			{
				var configReader = new ConfigurationReader();
				var syncData = configReader.ReadConfiguration();
				var synchronizer = new Synchronizer(Logger);
				synchronizer.Synchronize(syncData);
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
			}

			tmr.Stop();
			Duration = tmr.ElapsedMilliseconds;

			return ToString();
		}

		public override string ToString()
		{
			return $"Updated SiteDefinitions. {Duration}ms on {Environment.MachineName}.";
		}
	}
}
