using System;
using System.Diagnostics;
using System.Reflection;
using EPiServer.Logging.Compatibility;
using EPiServer.PlugIn;
using EPiServer.Scheduler;

namespace Addon.Episerver.EnvironmentSynchronizer.Jobs
{
	[ScheduledPlugIn(DisplayName = "Environment synchronization", SortIndex = 100)]
	public class SynchronizationJob : ScheduledJobBase
	{
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private readonly EnvironmentSynchronizationManager _environmentSynchronizationManager;

		public SynchronizationJob(EnvironmentSynchronizationManager environmentSynchronizationManager)
		{
			IsStoppable = false;
			_environmentSynchronizationManager = environmentSynchronizationManager;
		}

		private long Duration { get; set; }

		public override string Execute()
		{
			var tmr = Stopwatch.StartNew();

			try
			{
				_environmentSynchronizationManager.Synchronize();
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
