using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  AvariCapitalCRM.Models.ViewModels.SMSMessage
{
     public class SMSResponse
    {
        public int recipients { get; set; }
        public delivery_stats delivery_Stats { get; set; }
        public int sms { get; set; }
        public int cost { get; set; }
        public DateTime send_at { get; set; }
        public error error { get; set; }
        public int message_id { get; set; }
        public string errormsg { get; set; }
    }

    public class error
    {
        public string code { get; set; }
        public string description { get; set; }
    }

    public class delivery_stats
    {
        public int delivered { get; set; }
        public int bounced { get; set; }
        public int responses { get; set; }
        public int pending { get; set; }
        public int optouts { get; set; }
    }

}
