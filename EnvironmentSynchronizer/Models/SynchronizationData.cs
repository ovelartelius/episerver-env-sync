using System.Collections.Generic;
using EPiServer.Web;

namespace EnvironmentSynchronizer.Models
{
	public interface ISynchronizationData
	{
		bool RunAsInitializationModule { get; set; }
		List<SiteDefinition> SiteDefinitions { get; set; }
		List<Job> ScheduledJobs { get; set; }
	}

	public class SynchronizationData : ISynchronizationData
	{
		public bool RunAsInitializationModule { get; set; }
		public List<SiteDefinition> SiteDefinitions { get; set; }
		public List<Job> ScheduledJobs { get; set; }
	}
}
