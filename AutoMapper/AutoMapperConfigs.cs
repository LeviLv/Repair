using AutoMapper;
using Repair.EntityFramework.Domain;
using Repair.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repair.AutoMapper
{
    public class AutoMapperConfigs : Profile
    {
        public AutoMapperConfigs()
        {
            //var types = AppDomain.CurrentDomain.GetAssemblies()
            //    .Select(p => p.GetTypes().Where(x => x.GetInterfaces().Contains(typeof(IHasAutoMapper<>))));


            CreateMap<User, UserDTO>();

            CreateMap<User, RepairManDTO>();

            CreateMap<UserDTO, User>();

            CreateMap<RepairListDTO, RepairList>();

            CreateMap<RepairList, RepairListDTO>();

            CreateMap<RepairListInfo, RepairListInfoDTO>();

            CreateMap<RepairListInfoDTO, RepairListInfo>();

            CreateMap<Community, CommunityDTO>();
        }
    }
}
