using System.Configuration;

namespace Addon.Episerver.EnvironmentSynchronizer
{
	public interface ISynchronizerConfiguration
	{
		SynchronizerSection Settings { get; }
	}

	public class SynchronizerConfiguration : ISynchronizerConfiguration
	{
		public SynchronizerSection Settings => GetSection<SynchronizerSection>("env.synchronizer");

		protected static T GetSection<T>(string path) => (T)ConfigurationManager.GetSection(path);
	}
}
