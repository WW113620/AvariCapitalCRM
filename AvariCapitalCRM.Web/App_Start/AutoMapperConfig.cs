using AutoMapper;
using AvariCapitalCRM.Models.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AvariCapitalCRM.Web
{
    public class AutoMapperConfig
    {
        public static void Register()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<MapperProfile>();
            });
        }

    }
}