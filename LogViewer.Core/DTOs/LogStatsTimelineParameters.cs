using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogViewer.Core.DTOs
{
    public class LogStatsTimelineParameters: LogStatsParameters
    {
        public string GroupBy { get; set; } = "day";
    }
}
