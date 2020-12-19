using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariCapitalCRM.Models.ViewModels.Charts
{
    public class LineValue
    {
        public string name { get; set; }
        public string type { get; set; } = "line";
        public string stack { get; set; } = "总量";
        public bool smooth { get; set; } = true;
        public List<int> data { get; set; }
    }

    public class LineStyleValue : LineValue
    {
        public ItemStyle itemStyle { get; set; }
    }

    public class ItemStyle
    {
        public LineNormal normal { get; set; }
    }

    public class LineNormal
    {
        public AreaStyle areaStyle { get; set; }
    }

    public class AreaStyle
    {
        public string type { get; set; } = "default";
    }


    public class EchartsData<T>
    {
        public int code { get; set; }
        public string name { get; set; }
        public string[] xAxisData { get; set; }
        public List<T> series { get; set; }
    }

    public class ChartModels
    {
        public string ShowDate { get; set; }

        public int UserCount { get; set; }
    }

    public class HomeChartModels
    {
        public string ShowDate { get; set; }

        public int VideoCount { get; set; }

        public int PictureCount { get; set; }
        public int DocumentCount { get; set; }
    }

    public class KeyValue
    {
        public string name { get; set; }
        public int value { get; set; }
    }

    public class ChartResultModel
    {
        public ChartResultModel()
        {
            MediaFileLineData = new EchartsData<LineStyleValue>();
            MaterialPieData = new EchartsData<KeyValue>();
            UserLineData = new EchartsData<LineValue>();
        }
        public EchartsData<LineStyleValue> MediaFileLineData { get; set; }
        public EchartsData<KeyValue> MaterialPieData { get; set; }
        public EchartsData<LineValue> UserLineData { get; set; }
    }

}
