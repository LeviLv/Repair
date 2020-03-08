using Repair.AutoMapper;
using Repair.EntityFramework;
using Repair.EntityFramework.Domain;
using Repair.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Repair.Services
{
    public class RepairListService
    {
        private readonly IRepository<RepairList, int> _repository;

        private readonly IRepository<User, int> _userRepository;


        public RepairListService(IRepository<RepairList, int> repository, IRepository<User, int> userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        public async Task<RepairList> Insert(RepairList repair)
        {
            return await _repository.InsertAsync(repair);
        }

        public async Task<List<RepairListDTO>> GetRepairListByStatus(int userId, int? status)
        {
            Expression<Func<RepairList, bool>> func;
            if (status != null)
            {
                func = p => (p.Status == status.Value) && (p.UserId == userId);
            }
            else
            {
                func = p => p.UserId == userId;
            }

            var list = (await _repository.GetListAsync(func)).MapTo<List<RepairListDTO>>();
            list.ForEach(p => { p.statusName = ((RepairStatusEnum) p.Status).GetDisplayName(); });
            return list;
        }

        public async Task<List<RepairListDTO>> PageRepairList(PageBase pageBase)
        {
            Func<RepairList, bool> func = p => pageBase.Role.Contains(p.CommunityId);
            var pageList = (await _repository.PageListAsync(func, p => p.Id, pageBase))
                .MapTo<List<RepairListDTO>>();
            pageList.ForEach(async p =>
            {
                p.User = await _userRepository.FirstOrDefultAsync(x => x.Id == p.UserId);
                p.RepairMan = await _userRepository.FirstOrDefultAsync(x => x.Id == p.RepairManId);
                p.statusName = ((RepairStatusEnum) p.Status).GetDisplayName();
            });
            return pageList;
        }

        public async Task AppointRepairMan(int repairId, int repairManId)
        {
            var sql = $"update RepairList set RepairManId = {repairManId} where Id = {repairId}";
            await DapperService.Execute(sql);
        }
    }
}