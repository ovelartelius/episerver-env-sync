using System.Linq;
using EPiServer.Data.Dynamic;
using EPiServer.Logging;
using EPiServer.ServiceLocation;

namespace Addon.Episerver.EnvironmentSynchronizer.DynamicData
{
	public interface IEnvironmentSynchronizationStore
	{
		EnvironmentSynchronizationFlag GetFlag();

		void SetFlag(EnvironmentSynchronizationFlag flag);
	}

	[ServiceConfiguration(ServiceType = typeof(IEnvironmentSynchronizationStore))]
	public class EnvironmentSynchronizationStore: IEnvironmentSynchronizationStore
	{
		private static readonly ILogger Logger = LogManager.GetLogger();
		private readonly DynamicDataStore _store;

		public EnvironmentSynchronizationStore()
		{
			_store = DynamicDataStoreFactory.Instance.GetStore(typeof(EnvironmentSynchronizationFlag));
			if (_store == null)
			{
				_store = DynamicDataStoreFactory.Instance.CreateStore(typeof(EnvironmentSynchronizationFlag));
				Logger.Information("Create data store for 'EnvironmentSynchronizationFlag'.");
			}
		}

		public EnvironmentSynchronizationFlag GetFlag()
		{
			var flag = _store.Items<EnvironmentSynchronizationFlag>().FirstOrDefault();
			return flag;
		}

		public void SetFlag(EnvironmentSynchronizationFlag flag)
		{
			var existingFlag = _store.Items<EnvironmentSynchronizationFlag>().FirstOrDefault();
			if (existingFlag != null)
			{
				existingFlag.TimeStamp = flag.TimeStamp;
				existingFlag.Environment = flag.Environment;
				_store.Save(existingFlag);
			}
			else
			{
				_store.Save(flag);
			}
			Logger.Information("Saved environment synchronization flag to data store.");
		}
	}
}
