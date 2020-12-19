using System;
using System.Web;

namespace AvariCapitalCRM.Models.Utils
{
    public class CookieHelper
    {
        public static void SetCookie(string _name, string _value)
        {
            SetCookie(_name, _value, DateTime.Now.AddMonths(1));
        }

        public static void SetCookie(string _name, string _value, DateTime _expireTime)
        {
            SetCookie(_name, _value, _expireTime, string.Empty, false, string.Empty, false);
        }

        public static void SetCookie(string _name, string _value, DateTime _expireTime, string _domain, bool _httpOnly, string _path, bool _secure)
        {
            _value = System.Web.HttpUtility.UrlEncode(_value);

            HttpCookie _cookie = new HttpCookie(_name);
            _cookie.Value = _value;
            _cookie.Expires = _expireTime;
            _cookie.HttpOnly = _httpOnly;
            _cookie.Secure = _secure;

            if (!string.IsNullOrEmpty(_domain))
                _cookie.Domain = _domain;
            if (!string.IsNullOrEmpty(_path))
                _cookie.Path = _path;
            HttpContext.Current.Response.Cookies.Add(_cookie);
        }

        public static string GetCookie(string _name)
        {
            var cookie = HttpContext.Current.Request.Cookies[_name];
            return cookie == null ? string.Empty : cookie.Value;
        }

        public static void DeleteCookie(string _name)
        {
            HttpCookie cookie = new HttpCookie(_name);
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddYears(-3);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

    }

}
