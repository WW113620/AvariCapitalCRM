using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariCapitalCRM.Models.Utils
{
    public class EmailTempleteHelper
    {
        public static string ConverToHtml<T>(string path, T t) where T : new()
        {
            if (path == string.Empty)
            {
                return string.Empty;
            }
            StreamReader htmlSReader = new StreamReader(path);
            string strHtml = htmlSReader.ReadToEnd();
            if (!string.IsNullOrWhiteSpace(strHtml))
            {
                var propertys = t.GetType().GetProperties();
                if (propertys != null && propertys.Length > 0)
                {
                    foreach (var p in propertys)
                    {
                        strHtml = strHtml.Replace($"${p.Name}$", p.GetValue(t).ToString());
                    }
                    return strHtml;
                }
            }
            return string.Empty;
        }
    }

}
