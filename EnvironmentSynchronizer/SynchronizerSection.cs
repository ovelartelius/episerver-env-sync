using System.Configuration;

namespace EnvironmentSynchronizer
{
	public class SynchronizerSection : ConfigurationSection
	{
		[ConfigurationProperty("runAsInitializationModule", IsRequired = false)]
		public bool RunAsInitializationModule
		{
			get => (bool)this["runAsInitializationModule"];
			set => this["runAsInitializationModule"] = value;
		}

		[ConfigurationProperty("sitedefinitions")]
		[ConfigurationCollection(typeof(SiteDefinitionCollection), AddItemName = "sitedefinition")]
		public SiteDefinitionCollection SiteDefinitions
		{
			get
			{
				if (base["sitedefinitions"] != null && ((ConfigurationElement)base["sitedefinitions"]).ElementInformation.IsPresent)
				{
					return (SiteDefinitionCollection)base["sitedefinitions"];
				}
				else
				{
					var defaultCollection = new SiteDefinitionCollection();
					defaultCollection.AddElement(new SiteDefinitionElement { Name = string.Empty });
					return defaultCollection;
				}
			}
		}

		[ConfigurationProperty("schedulejobs")]
		[ConfigurationCollection(typeof(ScheduledJobCollection), AddItemName = "schedulejob")]
		public ScheduledJobCollection ScheduleJobs
		{
			get
			{
				if (base["schedulejobs"] != null && ((ConfigurationElement)base["schedulejobs"]).ElementInformation.IsPresent)
				{
					return (ScheduledJobCollection)base["schedulejobs"];
				}
				else
				{
					var defaultCollection = new ScheduledJobCollection();
					defaultCollection.AddElement(new ScheduledJobElement() { Name = string.Empty });
					return defaultCollection;
				}
			}
		}
	}
}
