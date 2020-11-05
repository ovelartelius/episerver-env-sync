using EPiServer.Logging;
using EPiServer.ServiceLocation;
using System.Collections.Generic;

namespace Addon.Episerver.EnvironmentSynchronizer
{
    public class EnvironmentSynchronizationManager
    {
        private static readonly ILogger _logger = LogManager.GetLogger();
        private readonly IEnumerable<IEnvironmentSynchronizer> _environmentSynchronizers;

        public EnvironmentSynchronizationManager(
            IEnumerable<IEnvironmentSynchronizer> environmentSynchronizers)
        {
            _environmentSynchronizers = environmentSynchronizers;
        }

        public void Synchronize()
        {
            IEnvironmentNameSource environmentNameSource;

            ServiceLocator.Current.TryGetExistingInstance<IEnvironmentNameSource>(out environmentNameSource);
            string environmentName = environmentNameSource != null ? environmentNameSource.GetCurrentEnvironementName() : string.Empty;

            Synchronize(environmentName);
        }

        public void Synchronize(string environmentName)
        {
            _logger.Information($"Starting environment synchronization for enviroment named: {environmentName}");

            foreach (var environmentSynchronizer in _environmentSynchronizers)
            {
                environmentSynchronizer.Synchronize(environmentName);
            }

            _logger.Information($"Starting environment synchronization for enviroment named: {environmentName}");
        }
    }
}
