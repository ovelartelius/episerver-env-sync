using System;
using System.Diagnostics;
using EPiServer.Logging;
using EPiServer.PlugIn;
using EPiServer.Scheduler;

namespace Addon.Episerver.EnvironmentSynchronizer.Jobs
{
	[ScheduledPlugIn(
		DisplayName = "Environment Synchronization", 
		Description = "Ensures that content and settings that are stored in the databases are corrected given the current environment. This is helpful after a content synchronization between different Episerver environments. https://github.com/ovelartelius/episerver-env-sync", 
		SortIndex = 100)
	]
	public class SynchronizationJob : ScheduledJobBase
	{
		private static readonly ILogger Logger = LogManager.GetLogger();
		private readonly IEnvironmentSynchronizationManager _environmentSynchronizationManager;

		public SynchronizationJob(IEnvironmentSynchronizationManager environmentSynchronizationManager)
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
				Logger.Error("Error when run Environment Synchronization job.", ex);
			}

			tmr.Stop();
			Duration = tmr.ElapsedMilliseconds;

			return ToString();
		}

		public override string ToString()
		{
			return $"Ran environment synchronization job. {Duration}ms on {Environment.MachineName}.";
		}
	}
}
