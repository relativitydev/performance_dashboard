namespace kCura.PDB.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class AttributeHelper
	{
		public static T Get<T>(this MemberInfo type)
			where T : Attribute
		{
			return (T)Attribute.GetCustomAttribute(type, typeof(T), true);
		}

		public static T Get<T>(Enum type)
			where T : Attribute
		{
			var enumType = type.GetType();
			var name = Enum.GetName(enumType, type);
			var enumMemberAttribute = ((T[])GetList<T>(enumType.GetField(name))).SingleOrDefault();
			return enumMemberAttribute;
		}

		public static IEnumerable<T> GetList<T>(MemberInfo type)
			where T : Attribute
		{
			return (IEnumerable<T>)Attribute.GetCustomAttributes(type, typeof(T), true);
		}

		public static bool IsDefined<T>(this MemberInfo element)
		{
			return Attribute.IsDefined(element, typeof(T), true);
		}
	}
}
