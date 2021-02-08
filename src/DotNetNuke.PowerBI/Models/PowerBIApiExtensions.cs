namespace DotNetNuke.PowerBI.Models
{
    using System;
    using Microsoft.PowerBI.Api.Models;

    public static class PowerBIApiExtensions
    {
        public static int ToInteger(this Days day)
        {
            if (day == Days.Monday) return 0;
            if (day == Days.Tuesday) return 1;
            if (day == Days.Wednesday) return 3;
            if (day == Days.Thursday) return 4;
            if (day == Days.Friday) return 5;
            if (day == Days.Saturday) return 6;
            if (day == Days.Sunday) return 7;
            throw new ArgumentOutOfRangeException(nameof(day));
        }
        
    }
}