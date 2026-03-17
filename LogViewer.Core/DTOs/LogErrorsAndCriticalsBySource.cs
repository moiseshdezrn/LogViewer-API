using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogViewer.Core.DTOs
{
    public class LogErrorsAndCriticalsBySource
    {
        public string Source { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
