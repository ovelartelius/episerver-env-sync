using System;
using System.Collections.Generic;
using System.Reflection;
using EnvironmentSynchronizer.Models;
using EPiServer.Logging.Compatibility;
using EPiServer.Web;

namespace EnvironmentSynchronizer.Business
{
	public interface IConfigurationReader
	{
		SynchronizationData ReadConfiguration();
	}

	public class ConfigurationReader : IConfigurationReader
	{
		private readonly ILog _logger;

		public ConfigurationReader()
		{
			_logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}

		public ConfigurationReader(ILog logger)
		{
			_logger = logger;
		}

		public SynchronizationData ReadConfiguration()
		{
			var config = new SynchronizerConfiguration();

			var syncData = new SynchronizationData();

			syncData.RunAsInitializationModule = config.Settings.RunAsInitializationModule;

			if (config.Settings.SiteDefinitions != null && config.Settings.SiteDefinitions.Count > 0)
			{
				syncData.SiteDefinitions = new List<SiteDefinition>();
				foreach (SiteDefinitionElement element in config.Settings.SiteDefinitions)
				{
					var siteDefinition = new SiteDefinition()
					{
						Id = string.IsNullOrEmpty(element.Id) ? Guid.Empty : new Guid(element.Id),
						Name = string.IsNullOrEmpty(element.Name) ? string.Empty : element.Name,
						SiteUrl = string.IsNullOrEmpty(element.SiteUrl) ? null : new Uri(element.SiteUrl),
						Hosts = element.Hosts.ToHostDefinitions()
					};
					syncData.SiteDefinitions.Add(siteDefinition);
				}
			}
			else
			{
				_logger.Info($"Found no site definitions to handle.");
			}

			if (config.Settings.ScheduleJobs != null && config.Settings.ScheduleJobs.Count > 0)
			{
				syncData.ScheduledJobs = new List<Job>();
				foreach (ScheduledJobElement element in config.Settings.ScheduleJobs)
				{
					var job = new Job
					{
						Id = element.Id,
						Name = element.Name,
						IsEnabled = element.IsEnabled
					};
					syncData.ScheduledJobs.Add(job);
				}
			}
			else
			{
				_logger.Info($"Found no schedule jobs to handle.");
			}

			return syncData;
		}
	}
}
