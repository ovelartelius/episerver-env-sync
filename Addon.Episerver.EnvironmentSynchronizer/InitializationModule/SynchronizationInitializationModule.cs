using System.Reflection;
using Addon.Episerver.EnvironmentSynchronizer.Configuration;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Logging.Compatibility;
using EPiServer.ServiceLocation;

namespace Addon.Episerver.EnvironmentSynchronizer.InitializationModule
{

	[InitializableModule]
	[ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
	public class SynchronizationInitializationModule : IInitializableModule
	{
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public void Initialize(InitializationEngine context)
		{
			var configReader = new ConfigurationReader();
			var syncData = configReader.ReadConfiguration();
			if (syncData.RunAsInitializationModule)
			{
				Logger.Info($"Environment synchronizer found RunAsInitializationModule=true");
				Logger.Info($"Will start to synchronize the configured data.");

				var environmentSynchronizationManager = ServiceLocator.Current.GetInstance<EnvironmentSynchronizationManager>();
				environmentSynchronizationManager.Synchronize();
			}
		}

		public void Preload(string[] parameters) { }

		public void Uninitialize(InitializationEngine context) { }
	}
}
