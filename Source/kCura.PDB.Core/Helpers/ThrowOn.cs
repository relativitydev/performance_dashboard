namespace kCura.PDB.Core.Helpers
{
	using System;

	public static class ThrowOn
	{
		public static void IsNull(object value, string name)
		{
			if (value == null)
			{
				throw new Exception(name + " can't be null.");
			}
		}

		public static void IsLessThanOne(int value, string name)
		{
			if (value < 1)
			{
				throw new Exception(name + " can't be less than 0.");
			}
		}
	}
}
