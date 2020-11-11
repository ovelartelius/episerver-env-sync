using Addon.Episerver.EnvironmentSynchronizer.Configuration;
using Addon.Episerver.EnvironmentSynchronizer.DynamicData;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Logging;
using EPiServer.ServiceLocation;

namespace Addon.Episerver.EnvironmentSynchronizer.InitializationModule
{

	[InitializableModule]
	[ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
	public class SynchronizationInitializationModule : IInitializableModule
	{
		private static readonly ILogger Logger = LogManager.GetLogger();

		public void Initialize(InitializationEngine context)
		{
			var configReader = new ConfigurationReader();
			var syncData = configReader.ReadConfiguration();
			

			if (syncData.RunAsInitializationModule)
			{
				Logger.Information($"Environment Synchronizer found RunAsInitializationModule=true");
				var runInitialization = true;
				var environmentSynchronizationManager = ServiceLocator.Current.GetInstance<EnvironmentSynchronizationManager>();

				if (!syncData.RunInitializationModuleEveryStartup)
				{
					Logger.Information($"Environment Synchronizer found RunInitializationModuleEveryStartup=false");
					var store = ServiceLocator.Current.GetInstance<EnvironmentSynchronizationStore>();
					var flag = store.GetFlag();
					if (flag != null && flag.Environment == environmentSynchronizationManager.GetEnvironmentName())
					{
						runInitialization = false;
						Logger.Information($"Environment Synchronizer will not run. Flag match the current environment {flag.Environment}");
					}
				}

				if (!runInitialization) return;

				environmentSynchronizationManager.Synchronize();
			}
		}

		public void Preload(string[] parameters) { }

		public void Uninitialize(InitializationEngine context) { }
	}
}
