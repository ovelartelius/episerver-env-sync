using System;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Addon.Episerver.EnvironmentSynchronizer.DynamicData;

namespace Addon.Episerver.EnvironmentSynchronizer
{
	public interface IEnvironmentSynchronizationManager
	{
		void Synchronize();

		void Synchronize(string environmentName);

		string GetEnvironmentName();
	}

	[ServiceConfiguration(ServiceType = typeof(IEnvironmentSynchronizationManager))]
    public class EnvironmentSynchronizationManager : IEnvironmentSynchronizationManager
    {
        private static readonly ILogger Logger = LogManager.GetLogger();
        private readonly IEnumerable<IEnvironmentSynchronizer> _environmentSynchronizers;
        private readonly IEnvironmentSynchronizationStore _environmentSynchronizationStore;

        public EnvironmentSynchronizationManager(
            IEnumerable<IEnvironmentSynchronizer> environmentSynchronizers, IEnvironmentSynchronizationStore environmentSynchronizationStore)
        {
            _environmentSynchronizers = environmentSynchronizers;
            _environmentSynchronizationStore = environmentSynchronizationStore;
        }

        public void Synchronize()
        {
            string environmentName = GetEnvironmentName();

            Synchronize(environmentName);
        }

        public void Synchronize(string environmentName)
        {
            Logger.Information($"Starting environment synchronization for environment named: {environmentName}");

            if (_environmentSynchronizers is null || !_environmentSynchronizers.Any())
            {
	            Logger.Information($"No synchronizers found.");
            }

            foreach (var environmentSynchronizer in _environmentSynchronizers)
            {
                environmentSynchronizer.Synchronize(environmentName);
            }

            Logger.Information($"Finished environment synchronization for environment named: {environmentName}");

            var environmentSynchronizationStamp = new EnvironmentSynchronizationStamp
            {
                TimeStamp = DateTime.Now,
                Environment = environmentName
            };
            _environmentSynchronizationStore.SetStamp(environmentSynchronizationStamp);
        }

        public string GetEnvironmentName()
        {
	        ServiceLocator.Current.TryGetExistingInstance<IEnvironmentNameSource>(out var environmentNameSource);
	        var environmentName = environmentNameSource != null ? environmentNameSource.GetCurrentEnvironmentName() : string.Empty;

	        if (string.IsNullOrEmpty(environmentName))
	        {
                environmentName = ConfigurationManager.AppSettings["episerver:EnvironmentName"];
            }

	        return environmentName;
        }
    }
}
