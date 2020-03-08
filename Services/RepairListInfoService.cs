using Repair.AutoMapper;
using Repair.EntityFramework;
using Repair.EntityFramework.Domain;
using Repair.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repair.Services
{
    public class RepairListInfoService : BaseService
    {
        private readonly IRepository<RepairListInfo, int> _repository;

        public RepairListInfoService(IRepository<RepairListInfo, int> repository) => _repository = repository;

        public async Task<List<RepairListInfoDTO>> GetRepairInfo(int repairId)
        {
            var list = await _repository.GetListAsync(p => p.ListId == repairId);
            var dtoList = list.MapTo<List<RepairListInfoDTO>>();
            dtoList.ForEach(p => 
            {
                p.StatusName = EumHelper.GetDisplayName((RepairStatusEnum)p.Status);
            });
            return dtoList;
        }
    }
}
