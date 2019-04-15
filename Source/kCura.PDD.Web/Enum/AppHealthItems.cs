using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kCura.PDD.Web.Enum
{
    public enum AppHealthItems
    {
        LRQCounts = 1,
        ErrorCounts = 2,
        AvgLatency = 3,
        UserCounts = 4
    }

    public enum RamHealthItems
    {
        Page = 5,
        PageFault = 6        
    }

    public enum ProcesserHealthItems
    {
        ProcesserTime = 10
    }
    public enum HardDiskHealthItems
    {
        DiskRead = 7,
        DiskWrite = 8
    }
    public enum SqlServerHealthItems
    {
        PageLifeExpectancy = 11
    }    
}