namespace kCura.PDB.Service.Services
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using kCura.PDB.Core.Attributes;
	using kCura.PDB.Core.Helpers;
	using kCura.PDB.Core.Interfaces.Services;

	/// <summary>
	/// Attribute service factory is a key based factory that given a key fetches a service based off the attributes on the service implementations
	/// </summary>
	/// <typeparam name="TService">The service type</typeparam>
	/// <typeparam name="TKey">The key type</typeparam>
	public class AttributeServiceFactory<TService, TKey> : IServiceFactory<TService, TKey>
		where TService : class
	{
		private readonly IList<TService> services;
		private IDictionary<TKey, TService> typesToServices;

		public AttributeServiceFactory(IEnumerable<TService> services)
		{
			this.services = services.ToList();
		}

		/// <inheritdoc />
		public TService GetService(TKey type)
		{
			ThrowOn.IsNull(type, $"key");

			if (this.typesToServices == null)
			{
				this.typesToServices = this.services.ToDictionary(GetServiceType, ml => ml);
			}

			if (this.typesToServices.ContainsKey(type) == false)
			{
				return null;
			}

			return this.typesToServices[type];
		}

		/// <summary>
		/// Gets the name of the service logic class by looking at the mapping type attribute to determine how the service logic maps a given key
		/// </summary>
		/// <param name="service">The service logic class that we want to map to a key</param>
		/// <returns>The result key that the service logic corresponds to</returns>
		private static TKey GetServiceType(TService service)
		{
			var serviceType = service.GetType();

			if (serviceType.IsDefined<ServiceMappingAttribute>())
			{
				var desc = serviceType.Get<ServiceMappingAttribute>();
				return (TKey)desc.Type;
			}
			else
			{
				throw new Exception($"Cannot determine service type for logic: {serviceType.Name}");
			}
		}
	}
}
