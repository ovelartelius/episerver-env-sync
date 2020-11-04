using EPiServer.Logging;
using System.Collections.Generic;

namespace Addon.Episerver.EnvironmentSynchronizer
{
    public class EnvironmentSynchronizationManager
	{
		private static readonly ILogger _logger = LogManager.GetLogger();
		private readonly IEnumerable<IEnvironmentSynchronizer> _environmentSynchronizers;

		public EnvironmentSynchronizationManager(IEnumerable<IEnvironmentSynchronizer> environmentSynchronizers)
		{
			_environmentSynchronizers = environmentSynchronizers;
		}

		public void Synchronize()
		{
			_logger.Information("Environment synchronizer starts its duty.");

			//TODO: Fetch real name from a service that can be implemented for each solution.
			string environmentName = "ABC";
			
			foreach(var environmentSynchronizer in _environmentSynchronizers)
            {
				environmentSynchronizer.Synchronize(environmentName);
            }

			_logger.Information("Environment synchronizer has completed its duty.");
		}
	}
}
