using AutoMapper;
using AvariCapitalCRM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariCapitalCRM.Models.AutoMapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            //CreateMap<TestUser, UserViewModel>();
            //CreateMap<UserViewModel, TestUser>();
            

            //Mapper.CreateMap<UserViewModel, TestUser>(). ForMember(d => d.UserName, opt => opt.MapFrom(s => s.UserName));
        }
    }
}
