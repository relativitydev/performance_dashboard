namespace kCura.PDB.Core.Extensions
{
	using System;
	using System.Globalization;
	using System.Linq.Expressions;
	using System.Reflection;

	public static class ExpressionExtensions
	{
		public static bool IsProperty(this LambdaExpression expression)
		{
			var memberExpression = expression.Body as MemberExpression;
			return memberExpression != null && memberExpression.Member is PropertyInfo;
		}

		public static PropertyInfo ToPropertyInfo(this LambdaExpression expression)
		{
			var prop = expression.Body as MemberExpression;
			if (prop != null)
			{
				var info = prop.Member as PropertyInfo;
				if (info != null)
				{
					if (info.DeclaringType != prop.Expression.Type && info.CanRead)
					{
						var propertyInLeft = prop.Expression.Type.GetProperty(info.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
						if (propertyInLeft != null && propertyInLeft.GetMethod.GetBaseDefinition() == info.GetMethod)
						{
							info = propertyInLeft;
						}
					}

					return info;
				}
			}

			throw new ArgumentException("Cannot get property info");
		}
	}
}
