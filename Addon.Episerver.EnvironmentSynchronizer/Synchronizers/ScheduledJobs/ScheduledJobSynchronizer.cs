﻿using Addon.Episerver.EnvironmentSynchronizer.Configuration;
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
		private static readonly ILogger Logger = LogManager.GetLogger();
		private readonly IScheduledJobRepository _scheduledJobRepository;
		private readonly ConfigurationReader _configurationReader;

		public ScheduledJobSynchronizer(
			IScheduledJobRepository scheduledJobRepository,
			ConfigurationReader configurationReader)
        {
            Logger.Information("ScheduledJobSynchronizer initialized.");
			_scheduledJobRepository = scheduledJobRepository;
			_configurationReader = configurationReader;
		}

		public void Synchronize(string environmentName)
        {
	        Logger.Information("ScheduledJobSynchronizer start synchronization.");
            var syncConfiguration = _configurationReader.ReadConfiguration();

            if(syncConfiguration.ScheduledJobs == null)
            {
                return;
            }

			UpdateScheduledJobs(syncConfiguration.ScheduledJobs);
		}

		private void UpdateScheduledJobs(List<ScheduledJobDefinition> scheduledJobConfiguration)
		{
			var existingScheduledJobs = _scheduledJobRepository.List().ToList();

			foreach (var job in scheduledJobConfiguration)
			{
				if (!string.IsNullOrEmpty(job.Id) && job.Id == "*")
                {
                    UpdateAllScheduleJobSettings(existingScheduledJobs, job);
                }
                else
                {
                    UpdateScheduleJobSettings(existingScheduledJobs, job);
                }
            }
		}

        private void UpdateAllScheduleJobSettings(List<ScheduledJob> existingScheduledJobs, ScheduledJobDefinition job)
        {
            foreach (var existingScheduledJob in existingScheduledJobs)
            {
                if (existingScheduledJob.IsEnabled == job.IsEnabled)
                {
                    continue;
                }

                existingScheduledJob.IsEnabled = job.IsEnabled;
                _scheduledJobRepository.Save(existingScheduledJob);
            }
        }

        private void UpdateScheduleJobSettings(List<ScheduledJob> existingScheduledJobs, ScheduledJobDefinition job)
        {
            ScheduledJob existingJob = null;
            var extraInfoMessage = string.Empty;

            try
            {
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
            }
            catch (Exception ex)
            {
	            extraInfoMessage = $"id=\"{job.Id}\" name=\"{job.Name}\"";
                Logger.Error($"Error when try to loaf schedulejob id=\"{job.Id}\" name=\"{job.Name}\".", ex);
            }

            if (existingJob != null)
            {
                existingJob.IsEnabled = job.IsEnabled;
                _scheduledJobRepository.Save(existingJob);
            }
            else
            {
                Logger.Warning($"Could not find scheduled job with {extraInfoMessage}");
            }
        }
    }
}
