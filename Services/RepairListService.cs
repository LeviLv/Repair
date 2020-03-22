using Repair.AutoMapper;
using Repair.EntityFramework;
using Repair.EntityFramework.Domain;
using Repair.Entitys;
using Repair.Models;
using Repair.SMS;
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

        private readonly IRepository<Community, int> _communityRepository;

        private readonly IRepository<RepairListInfo, int> _infoRepository;


        public RepairListService(IRepository<RepairList, int> repository, IRepository<User, int> userRepository
            , IRepository<Community, int> communityRepository
            , IRepository<RepairListInfo, int> infoRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
            _communityRepository = communityRepository;
            _infoRepository = infoRepository;
        }

        public async Task<RepairList> Insert(RepairList repair)
        {
            var model = await _repository.InsertAsync(repair);

            var info = new RepairListInfo()
            {
                ListId = model.Id,
                Remake = repair.Remake,
                Status = (int) RepairStatusEnum.Init
            };
            await _infoRepository.InsertAsync(info);
            return model;
        }

        public async Task update(int repairListId,RepairStatusEnum statusEnum)
        {
            var sql = $" update RepairList set Status = {(int)statusEnum} where Id = {repairListId} ";
            await DapperService.Execute(sql);

            var msg = "";
            if (statusEnum == RepairStatusEnum.Doing)
            {
                msg = "修理工已到达";
            }

            if (statusEnum == RepairStatusEnum.Down)
            {
                msg = "已完成，待评价";
            }

            if (statusEnum == RepairStatusEnum.Success)
            {
                msg = "完成";
            }
            var info = new RepairListInfo()
            {
                ListId = repairListId,
                Remake = msg,
                Status = (int) statusEnum
            };
            await _infoRepository.InsertAsync(info);
        }

        public async Task<List<RepairListDTO>> GetRepairListByStatus(int? userId, int? status)
        {
            Expression<Func<RepairList, bool>> func = p => true;
            if (status != null)
            {
                func = func.And(p => p.Status == status.Value);
            }
            if (userId.HasValue)
            {
                func = func.And(p => p.UserId == userId);
            }

            var list = (await _repository.GetListAsync(func)).MapTo<List<RepairListDTO>>();
            var commonity = await _communityRepository.GetAllAsync();
            var user = await _userRepository.GetAllAsync();
            list.ForEach(p =>
            {
                p.statusName = ((RepairStatusEnum) p.Status).GetDisplayName();
                p.CommunityName = commonity.FirstOrDefault(x => x.Id == p.CommunityId)?.Name;
                p.RepairManName = user.FirstOrDefault(x => x.Id == p.RepairManId)?.Name;
                p.User = user.FirstOrDefault(x => x.Id == p.UserId);
                p.RepairManMobile = user.FirstOrDefault(x => x.Id == p.RepairManId)?.Mobile;
                p.Img = AliOssHelper.GetIamgeUri(p.Img);
            });
            return list;
        }

        public async Task<List<RepairListDTO>> GetRepairListByStatusByMan(int? userId, int? status)
        {
            Expression<Func<RepairList, bool>> func = p => true;
            if (status != null)
            {
                func = func.And(p => p.Status == status.Value);
            }
            if (userId.HasValue)
            {
                func = func.And(p => p.RepairManId == userId);
            }

            var list = (await _repository.GetListAsync(func)).MapTo<List<RepairListDTO>>();
            var commonity = await _communityRepository.GetAllAsync();
            var user = await _userRepository.GetAllAsync();
            list.ForEach(p =>
            {
                p.statusName = ((RepairStatusEnum)p.Status).GetDisplayName();
                p.CommunityName = commonity.FirstOrDefault(x => x.Id == p.CommunityId)?.Name;
                p.RepairManName = user.FirstOrDefault(x => x.Id == p.RepairManId)?.Name;
                p.User = user.FirstOrDefault(x => x.Id == p.UserId);
                p.RepairManMobile = user.FirstOrDefault(x => x.Id == p.RepairManId)?.Mobile;
                p.Img = AliOssHelper.GetIamgeUri(p.Img);
            });
            return list;
        }

        public async Task<List<RepairList>> GetRepairListByRepairManId(int id, int? status)
        {
            Expression<Func<RepairList, bool>> func = p => true;
            if (status != null)
            {
                func = func.And(p => (p.Status == status.Value) && (p.RepairManId == id));
            }
            else
            {
                func = func.And(p => p.RepairManId == id);
            }

            return await _repository.GetListAsync(func);
        }

        public async Task<QueryResult<RepairListDTO>> PageRepairList(QueryRepairListModel pageBase)
        {
            var sql = @"select 
                    r.*,
                    c.`Name` as CommunityName,
                    (select name from Users where id = r.RepairManId) as RepairManName
                    from 
                    Community c 
                    join RepairList r on r.CommunityId = c.Id where 1=1 ";
            if (pageBase.AdminId.HasValue)
            {
                sql += $" and c.AdminId = {pageBase.AdminId.Value} ";
            }

            if (pageBase.StatusEnum.HasValue)
            {
                sql += $" and r.status = {pageBase.StatusEnum.Value} ";
            }

            var list = await DapperService.PageList<RepairListDTO>(sql, pageBase);
            var user = await _userRepository.GetAllAsync();
            list.List.ForEach(p =>
            {
                p.statusName = ((RepairStatusEnum) p.Status).GetDisplayName();
                p.User = user.FirstOrDefault(x => x.Id == p.UserId);
            });

            return list;
        }

        public async Task AppointRepairMan(int repairId, int repairManId)
        {
            var sql = $"update RepairList set RepairManId = {repairManId} ,status = 2 where Id = {repairId}";
            await DapperService.Execute(sql);

            var repair = await _userRepository.FirstOrDefultAsync(p => p.Id == repairManId);
            var info = new RepairListInfo()
            {
                ListId = repairId,
                Remake = $"系统派单，修理工：{repair.Name}（{repair.Mobile}）",
                Status = (int) RepairStatusEnum.Sure
            };
            await _infoRepository.InsertAsync(info);

            var repairList = await _repository.FirstOrDefultAsync(p => p.Id == repairId);
            var user = await _userRepository.FirstOrDefultAsync(p => p.Id == repairList.UserId);

            SmsHelper.sendUserMsg(user.Mobile, new { name = repair.Name, tel = repair.Mobile });

            var comm = await _communityRepository.FirstOrDefultAsync(p => p.Id == user.CommunityId);
            SmsHelper.sendRepairMsg(repair.Mobile, new { name = user.Name, tel = user.Mobile, home = $"{comm.Name}-{user.HomeAddress}-{user.HomeNum}" });
        }
    }
}