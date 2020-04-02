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
        private readonly IRepository<RepairList, int> _repairListRepository;
        private readonly IRepository<User, int> _userRepository;

        public RepairListInfoService(IRepository<RepairListInfo, int> repository
            , IRepository<RepairList, int> repairListRepository
            , IRepository<User, int> userRepository)
        {
            _repository = repository;
            _repairListRepository = repairListRepository;
            _userRepository = userRepository;
        }

        public async Task<List<RepairListInfoDTO>> GetRepairInfo(int repairId)
        {
            var list = await _repository.GetListAsync(p => p.ListId == repairId);
            var repair = await _repairListRepository.FirstOrDefultAsync(p => p.Id == repairId);
            var u = await _userRepository.FirstOrDefultAsync(p => p.Id == repair.UserId);
            var dtoList = list.MapTo<List<RepairListInfoDTO>>();
            dtoList.ForEach(p => 
            {
                p.user = u;
                p.StatusName = EumHelper.GetDisplayName((RepairStatusEnum)p.Status);
            });
            return dtoList;
        }

        public async Task CommRepairRemake(ComRepairDTO dto)
        {
            var sql = $" UPDATE RepairList SET `Status` = {(int)RepairStatusEnum.Success} WHERE Id = {dto.repairId} ;";
            await DapperService.Execute(sql);

            var info = new RepairListInfo()
            {
                ListId = dto.repairId,
                Remake = dto.str,
                Status = (int)RepairStatusEnum.Success
            };
            await _repository.InsertAsync(info);
        }

    }
}
