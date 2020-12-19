using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariCapitalCRM.Models.ViewModels
{
    public class StatisticsModel
    {
        public int Vidoes { get; set; } = 0;
        public int Images { get; set; } = 0;
        public int Documents { get; set; } = 0;
        public int Download { get; set; } = 0;
    }

    public class DownloadStatisticsModel
    {
        public long _row_number_ { get; set; }
        public string MediaFileName { get; set; }
        public string UserName { get; set; }
        public int DownloadCount { get; set; }
    }
}
