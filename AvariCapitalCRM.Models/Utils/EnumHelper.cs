using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AvariCapitalCRM.Models.Utils
{
    public static class EnumHelper
    {
        public static string GetEnumName<T>(this int status)
        {
            return Enum.GetName(typeof(T), status);
        }

        public static string GetDescription(Enum enumValue)
        {
            string value = enumValue.ToString();
            System.Reflection.FieldInfo field = enumValue.GetType().GetField(value);
            object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (objs.Length == 0)
                return value;
            DescriptionAttribute descriptionAttribute = (DescriptionAttribute)objs[0];
            return descriptionAttribute.Description;
        }


        public static List<SelectOption> EnumToList<T>()
        {
            List<SelectOption> list = new List<SelectOption>();
            SelectOption m = null;
            foreach (var e in Enum.GetValues(typeof(T)))
            {
                m = new SelectOption();
                object[] objArr = e.GetType().GetField(e.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (objArr != null && objArr.Length > 0)
                {
                    DescriptionAttribute da = objArr[0] as DescriptionAttribute;
                    m.Desction = da.Description;
                }
                m.Value = e.ToInt(0);
                m.Text = e.ToString();
                list.Add(m);
            }
            return list;
        }


    }

    public class SelectOption
    {
        public int Value { get; set; }
        public string Text { get; set; }

        public string Desction { set; get; }
    }

}
