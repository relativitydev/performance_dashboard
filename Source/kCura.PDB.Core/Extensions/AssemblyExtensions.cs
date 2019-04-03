namespace kCura.PDB.Core.Extensions
{
	using System.Collections.Generic;
	using System.Reflection;
	using kCura.PDB.Core.Helpers;

	public static class AssemblyExtensions
	{
		public static byte[] GetResourceBytes(this Assembly assembly, string resourceName)
		{
			ThrowOn.IsNull(assembly, "assembly");
			ThrowOn.IsNull(resourceName, "resourceName");
			var result = new List<byte>();
			using (var stream = assembly.GetManifestResourceStream(resourceName))
			{
				ThrowOn.IsNull(resourceName, $"Couldn't find resource {resourceName} in assembly {assembly.FullName}. stream");

				var buffer = new byte[1028];
				while (stream.Read(buffer, 0, buffer.Length) > 0)
				{
					result.AddRange(buffer);
				}

				return result.ToArray();
			}
		}
	}
}
