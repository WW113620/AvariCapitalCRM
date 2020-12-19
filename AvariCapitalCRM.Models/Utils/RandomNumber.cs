using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariCapitalCRM.Models.Utils
{
    public class RandomNumber
    {
        public static object _lock = new object();

        public static string CreateOrderSn(string prefix = "T")
        {
            lock (_lock)
            {
                return prefix + DateTime.Now.Ticks;
            }
        }


    }
}
