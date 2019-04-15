namespace kCura.PDB.Core.Enumerations
{
	using System;

	[Flags]
	public enum ManagementField
	{
		PagesPerSec = 1,
		PageFaultsPerSec = 2,
		Name = 4,
		DiskReadsPersec = 8,
		DiskWritesPersec = 16,
		NumberOfCores = 32,
		NumberOfLogicalProcessors = 64,
		PercentProcessorTime = 128,
		AvailableKBytes = 256,
		TotalVisibleMemorySize = 512,
		FreeMegabytes = 1024,
		AvgDiskSecPerRead = 2048,
		AvgDiskSecPerRead_Base = 2049,
		Frequency_PerfTime = 2050,
		AvgDiskSecPerWrite = 2051,
		AvgDiskSecPerWrite_Base = 2052,
		All = 99,

		ServicePackMajorVersion = 4104,
		Version = 8208,
		Caption = 16416,
	}
}
