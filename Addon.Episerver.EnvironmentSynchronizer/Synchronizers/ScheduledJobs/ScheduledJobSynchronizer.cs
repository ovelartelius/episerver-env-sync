using Addon.Episerver.EnvironmentSynchronizer.Configuration;
using EPiServer.DataAbstraction;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Addon.Episerver.EnvironmentSynchronizer.Synchronizers.ScheduledJobs
{
	[ServiceConfiguration(typeof(IEnvironmentSynchronizer))]
	public class ScheduledJobSynchronizer : IEnvironmentSynchronizer
    {
		private static readonly ILogger _logger = LogManager.GetLogger();
		private readonly IScheduledJobRepository _scheduledJobRepository;
		private readonly ConfigurationReader _configurationReader;

		public ScheduledJobSynchronizer(
			IScheduledJobRepository scheduledJobRepository,
			ConfigurationReader configurationReader)
        {
			_scheduledJobRepository = scheduledJobRepository;
			_configurationReader = configurationReader;
		}

		public void Synchronize(string environmentName)
        {
			var syncConfiguration = _configurationReader.ReadConfiguration();
			UpdateScheduledJobs(syncConfiguration.ScheduledJobs);
		}

		private int UpdateScheduledJobs(List<ScheduledJobDefinition> jobs)
		{
			var updatedJobs = 0;

			var existingScheduledJobs = _scheduledJobRepository.List().ToList();

			foreach (var job in jobs)
			{
				if (!string.IsNullOrEmpty(job.Id) && job.Id == "*")
				{
					foreach (var existingScheduledJob in existingScheduledJobs)
					{
                        if (existingScheduledJob.IsEnabled == job.IsEnabled)
                        {
							continue;
                        }

						existingScheduledJob.IsEnabled = job.IsEnabled;
						_scheduledJobRepository.Save(existingScheduledJob);
						updatedJobs++;
					}
				}
				else
				{
					ScheduledJob existingJob = null;
					var extraInfoMessage = string.Empty;
					if (!string.IsNullOrEmpty(job.Id))
					{
						existingJob = existingScheduledJobs.FirstOrDefault(x => x.ID == Guid.Parse(job.Id));
						extraInfoMessage = $"Id = {job.Id}";
					}
					else if (!string.IsNullOrEmpty(job.Name))
					{
						existingJob = existingScheduledJobs.FirstOrDefault(x => x.Name == job.Name || x.AssemblyName == job.Name);
						extraInfoMessage = $"Name/AssemblyName = {job.Name}";
					}

					if (existingJob != null)
					{
						existingJob.IsEnabled = job.IsEnabled;
						_scheduledJobRepository.Save(existingJob);
						updatedJobs++;
					}
					else
					{
						_logger.Warning($"Could not find scheduled job with {extraInfoMessage}");
					}
				}
			}

			return updatedJobs;
		}
	}
}
