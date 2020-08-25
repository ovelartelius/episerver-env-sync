using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using EPiServer.Web;

namespace EnvironmentSynchronizer
{
	public class HostCollection : ConfigurationElementCollection
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SiteDefinitionCollection"/> class.
		/// </summary>
		public HostCollection()
		{
		}

		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new HostElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((HostElement)element).Name;
		}

		public void AddElement(HostElement newElement)
		{
			BaseAdd(newElement);
		}

		public new HostElement this[string index]
		{
			get { return (HostElement)BaseGet(index); }
		}

		public List<HostDefinition> ToHostDefinitions()
		{
			var hostDefinitions = new List<HostDefinition>();
			foreach (HostElement element in this)
			{
				var hostDefinition = new HostDefinition
				{
					Name = element.Name,
					Type = element.Type != HostDefinitionType.Undefined ? element.Type : HostDefinitionType.Undefined,
					UseSecureConnection = element.UseSecureConnection,
					Language = string.IsNullOrEmpty(element.Language) ? null : new CultureInfo(element.Language)
				};
				hostDefinitions.Add(hostDefinition);
			}
			return hostDefinitions;
		}
	}
}