using System.Configuration;

namespace EnvironmentSynchronizer
{
	public class ScheduledJobCollection : ConfigurationElementCollection, System.Collections.ICollection
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ScheduledJobCollection"/> class.
		/// </summary>
		public ScheduledJobCollection()
		{
		}

		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new ScheduledJobElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((ScheduledJobElement)element).Name;
		}

		public void AddElement(ScheduledJobElement newElement)
		{
			BaseAdd(newElement);
		}

		public new ScheduledJobElement this[string index]
		{
			get { return (ScheduledJobElement)BaseGet(index); }
		}
	}
}