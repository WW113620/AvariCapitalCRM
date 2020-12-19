using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  AvariCapitalCRM.Models.ViewModels
{
    public class BaseResult
    {
        public BaseResult() { }

        public BaseResult(int _code, string _msg)
        {
            this.code = _code;
            this.msg = _msg;
        }
        public int code { get; set; }
        public string msg { get; set; }
    }

    public class BaseEnitity
    {
        public int code { get; set; }
        public string msg { get; set; }
    }

    public class ModelResult<T> : BaseEnitity
    {
        public ModelResult() { }
         public ModelResult(int _code, string _msg)
        {
            this.code = _code;
            this.msg = _msg;
        }
        public ModelResult(int _code, string _msg,T _model)
        {
            this.code = _code;
            this.msg = _msg;
            this.model = _model;
        }
        public T model { get; set; }
    }

    public class DataResult<T> : BaseEnitity
    {
        public DataResult()
        {
            data = new List<T>();
        }
        public List<T> data { get; set; }
    }

    public class BaseReqestParams
    {
        public int page { get; set; } = 1;
        public int limit { get; set; } = 15;
        public long count { get; set; } = 0;
        public long pageCount => (long)Math.Ceiling(count / Convert.ToDouble(limit));
    }


    public class PageResult<T> : BaseEnitity
    {
        public PageResult()
        {
            page = new BaseReqestParams();
            data = new List<T>();
        }
        public BaseReqestParams page { get; set; }
        public List<T> data { get; set; }
    }

    public class LayuiPageResult<T> where T : class
    {
        public LayuiPageResult()
        {
            data = new List<T>();
        }
        public int code { get; set; }
        public string msg { get; set; }
        public long count { get; set; }
        public List<T> data { get; set; }
    }


}
