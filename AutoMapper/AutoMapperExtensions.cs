using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repair.AutoMapper
{
    public static class AutoMapperExtensions
    {
        public static IMapper Mapper { get; set; }

        public static T MapTo<T>(this object source)
        {
            if (source == null)
            {
                return default;
            }

            return Mapper.Map<T>(source);
        }
    }
}
