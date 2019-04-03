namespace kCura.PDB.Core
{
	using System;
	using System.Runtime.InteropServices;

	/// <summary>
	/// Note: This class is intended to just make sure that every pdb assembly has "new" code in it 
	/// so that Relativity will mark the dll as changed when it uploads it. File is modified during
	/// build by ForceUpdateDLLS.ps1 
	/// </summary>
	[Guid("a92723b3-bd67-4e05-958c-a2f0dc1e007e")]
	public class zClass_2018_1_8_11_55_6
	{
		public Guid IncrementGuid = new Guid("a92723b3-bd67-4e05-958c-a2f0dc1e007e");

		private int value;

		public int _method_2018_1_8_11_55_6(int _param_2018_1_8_11_55_6)
		{
			value = 1140404816; // Update this value
			value += _param_2018_1_8_11_55_6;

			return value;
		}

		public int _method2_2018_1_8_11_55_6 => 1140404816; // Update this value
	}


	public class zClass2_2018_1_8_11_55_6
	{
		public string Id { get; set; }
	}
}
