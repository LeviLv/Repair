using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repair.EntityFramework.Domain
{
    public interface IHasCreatedTime
    {
        DateTime CreateTime { get; set; }
    }
}
