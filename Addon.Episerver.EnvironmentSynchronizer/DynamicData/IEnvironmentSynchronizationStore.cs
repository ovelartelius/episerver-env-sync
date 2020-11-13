namespace Addon.Episerver.EnvironmentSynchronizer.DynamicData
{
	public interface IEnvironmentSynchronizationStore
	{
		EnvironmentSynchronizationFlag GetFlag();

		void SetFlag(EnvironmentSynchronizationFlag flag);
	}
}