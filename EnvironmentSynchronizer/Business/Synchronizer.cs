using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using EnvironmentSynchronizer.Models;
using EPiServer.DataAbstraction;
using EPiServer.Logging.Compatibility;
using EPiServer.ServiceLocation;
using EPiServer.Web;

namespace EnvironmentSynchronizer.Business
{
	public interface ISynchronizer
	{
		void Synchronize(SynchronizationData syncData);
	}

	public class Synchronizer : ISynchronizer
	{
		private readonly ILog _logger;
		private readonly IScheduledJobRepository _scheduledJobRepository;

		public Synchronizer()
		{
			_logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
			_scheduledJobRepository = ServiceLocator.Current.GetInstance<IScheduledJobRepository>();
		}

		public Synchronizer(ILog logger)
		{
			_logger = logger;
			_scheduledJobRepository = ServiceLocator.Current.GetInstance<IScheduledJobRepository>();
		}

		public Synchronizer(ILog logger, IScheduledJobRepository scheduledJobRepository)
		{
			_logger = logger;
			_scheduledJobRepository = scheduledJobRepository;
		}

		public void Synchronize(SynchronizationData syncData)
		{
			_logger.Info("Environment synchronizer starts its duty.");
			SynchronizeSites(syncData);
			SynchronizeScheduleJobs(syncData);
			_logger.Info("Environment synchronizer has completed its duty.");
		}

		private void SynchronizeSites(SynchronizationData syncData)
		{
			var stopwatch = Stopwatch.StartNew();

			try
			{
				if (syncData.SiteDefinitions != null && syncData.SiteDefinitions.Any())
				{
					var updatedSites = MergeSiteDefinitions(syncData.SiteDefinitions);

					_logger.Info($"Updated total of {updatedSites} sites.");
				}
				else
				{
					_logger.Warn("No site definitions found to synchronize.");
				}
			}
			catch (Exception ex)
			{
				_logger.Error(ex);
			}

			stopwatch.Stop();
			_logger.Info($"Synchronize site definitions took {stopwatch.ElapsedMilliseconds}ms.");
		}

		private void SynchronizeScheduleJobs(SynchronizationData syncData)
		{
			var stopwatch = Stopwatch.StartNew();

			try
			{
				if (syncData.ScheduledJobs != null && syncData.ScheduledJobs.Any())
				{
					var updatedJobs = UpdateScheduledJobs(syncData.ScheduledJobs);
					_logger.Info($"Updated total of {updatedJobs} scheduled jobs.");
				}
				else
				{
					_logger.Warn("No scheduled jobs found to synchronize.");
				}
			}
			catch (Exception ex)
			{
				_logger.Error(ex);
			}

			stopwatch.Stop();
			_logger.Info($"Synchronize scheduled jobs took {stopwatch.ElapsedMilliseconds}ms.");
		}

		private int MergeSiteDefinitions(IEnumerable<SiteDefinition> mergeSiteDefinitions)
		{
			var updatedSites = 0;
			var siteDefinitionRepository = ServiceLocator.Current.GetInstance<ISiteDefinitionRepository>();
			var existingSites = siteDefinitionRepository.List().ToList();
			foreach (var definition in mergeSiteDefinitions)
			{
				//Update the site definition if it doesn't have the same value for SiteUrl 
				var site = existingSites.FirstOrDefault(s => s.Name == definition.Name && s.SiteUrl != definition.SiteUrl);
				if (site != null)
				{
					site = site.CreateWritableClone();
					site.SiteUrl = definition.SiteUrl;
					site.Hosts = definition.Hosts;
					siteDefinitionRepository.Save(site);
					updatedSites++;
					_logger.Info($"Updated {definition.Name} to site URL {definition.SiteUrl} and {definition.Hosts.Count} hostnames.");
				}
				else
				{
					_logger.Warn($"Could not find site {definition.Name} or site already has site URL {definition.SiteUrl}.");
				}
			}

			return updatedSites;
		}

		private int UpdateScheduledJobs(List<Job> jobs)
		{
			var updatedJobs = 0;

			var existingScheduledJobs = _scheduledJobRepository.List().ToList();

			foreach (var job in jobs)
			{
				if (!string.IsNullOrEmpty(job.Id) && job.Id == "*")
				{
					foreach (var existingScheduledJob in existingScheduledJobs)
					{
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
						_logger.Warn($"Could not find scheduled job with {extraInfoMessage}");
					}
				}

				updatedJobs++;
			}

			return updatedJobs;
		}
	}
}
