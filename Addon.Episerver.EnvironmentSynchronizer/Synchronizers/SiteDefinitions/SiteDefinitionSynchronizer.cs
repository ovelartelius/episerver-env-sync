using Addon.Episerver.EnvironmentSynchronizer.Configuration;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Addon.Episerver.EnvironmentSynchronizer.Synchronizers.SiteDefinitions
{
    [ServiceConfiguration(typeof(IEnvironmentSynchronizer))]
    public class SiteDefinitionSynchronizer : IEnvironmentSynchronizer
    {
		private static readonly ILogger _logger = LogManager.GetLogger();
		private readonly ISiteDefinitionRepository _siteDefinitionRepository;
		private readonly ConfigurationReader _configurationReader;

		public SiteDefinitionSynchronizer(
			ISiteDefinitionRepository siteDefinitionRepository,
			ConfigurationReader configurationReader)
        {
			_siteDefinitionRepository = siteDefinitionRepository;
			_configurationReader = configurationReader;
		}

		public void Synchronize(string environmentName)
        {
			var stopwatch = Stopwatch.StartNew();

			var syncConfiguration = _configurationReader.ReadConfiguration();

			try
			{
				if (syncConfiguration.SiteDefinitions != null && syncConfiguration.SiteDefinitions.Any())
				{
					var updatedSites = MergeSiteDefinitions(syncConfiguration.SiteDefinitions);

					_logger.Information($"Updated total of {updatedSites} sites.");
				}
				else
				{
					_logger.Information("No site definitions found to synchronize.");
				}
			}
			catch (Exception ex)
			{
				_logger.Error("An excetion occured when trying to synchronize site definitions", ex);
			}

			stopwatch.Stop();
			_logger.Information($"Synchronize site definitions took {stopwatch.ElapsedMilliseconds}ms.");
		}

		private int MergeSiteDefinitions(IEnumerable<SiteDefinition> mergeSiteDefinitions)
		{
			var updatedSites = 0;
			var existingSites = _siteDefinitionRepository.List();

			foreach (var definition in mergeSiteDefinitions)
			{
				//Update the site definition if it doesn't have the same value for SiteUrl 
				var site = existingSites.FirstOrDefault(s => s.Name == definition.Name && s.SiteUrl != definition.SiteUrl);
				if (site != null)
				{
					site = site.CreateWritableClone();
					site.SiteUrl = definition.SiteUrl;
					site.Hosts = definition.Hosts;

					_siteDefinitionRepository.Save(site);
					updatedSites++;
					_logger.Information($"Updated {definition.Name} to site URL {definition.SiteUrl} and {definition.Hosts.Count} hostnames.");
				}
				else
				{
					_logger.Warning($"Could not find site {definition.Name} or site already has site URL {definition.SiteUrl}.");
				}
			}

			return updatedSites;
		}
	}
}
