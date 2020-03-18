using Repair.AutoMapper;
using Repair.EntityFramework;
using Repair.EntityFramework.Domain;
using Repair.Entitys;
using Repair.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace Repair.Services
{
    public class UserService : BaseService
    {
        private readonly IRepository<User, int> _repository;
        private readonly IRepository<RepairMan, int> _repairManRepository;
        private readonly CommunityService _communityService;

        public UserService(IRepository<User, int> repository
            , IRepository<RepairMan, int> repairManRepository
            , CommunityService communityService)
        {
            _repository = repository;
            _repairManRepository = repairManRepository;
            _communityService = communityService;
        }

        public async Task<UserDTO> GetUserAsync(int id)
        {
            var user = await _repository.FirstOrDefultAsync(p => p.Id == id);
            return user.MapTo<UserDTO>();
        }

        public async Task<User> GetUserByMobile(string mobile)
        {
            return await _repository.FirstOrDefultAsync(p => p.Mobile == mobile);
        }

        public async Task<User> Register(UserRegisterDto dto)
        {
            var u = new User
            {
                Mobile = dto.mobile,
                CreateTime = DateTime.Now,
                ModifyTime = DateTime.Now
            };
            var user = await _repository.InsertAsync(u);
            return user;
        }

        public async Task<UserDTO> Update(User user)
        {
            var u = await _repository.UpdateAndGetModelAsync(user);
            return u.MapTo<UserDTO>();
        }

        public async Task<UserDTO> Insert(User user)
        {
            var u = await _repository.InsertAsync(user);
            return u.MapTo<UserDTO>();
        }

        public async Task<UserDTO> UpdateOrInsert(User user)
        {
            var hasUser = await _repository.CountAsync(p => p.Id == user.Id);
            if (hasUser > 0)
            {
                await _repository.UpdateAndGetModelAsync(user);
            }
            else
            {
                await _repository.InsertAsync(user);
            }

            return user.MapTo<UserDTO>();
        }


        public async Task<QueryResult<User>> GetUserList(QueryUserModel pageBase)
        {
            Expression<Func<User, bool>> func = p => !p.IsAdmin;

            if (!string.IsNullOrWhiteSpace(pageBase.mobile))
            {
                func = func.And(p => p.Mobile == pageBase.mobile);
            }

            if (pageBase.IsRepair.HasValue)
            {
                func = func.And(p => p.IsRepairMan == pageBase.IsRepair);
            }

            var list = await _repository.PageListAsync(func.Compile(), p => p.Id, pageBase);
            return list;
        }

        public async Task<QueryResult<User>> GetUserList2(QueryUserModel pageBase)
        {
            var comm = await _communityService.FirstOrDefult(p => p.AdminId == pageBase.AdminId); 
            var sql = $" SELECT u.* from RepairMan r JOIN Users u ON u.Id = r.UserId where r.CommunityId = {comm.Id} ";

            var list = await DapperService.PageList<User>(sql, pageBase);
            return list;
        }
        
        public async Task<QueryResult<RepairManDTO>> GetRepairManList(QueryUserModel pageBase)
        {
            var community = await _communityService.GetAllAsync();
            Expression<Func<User, bool>> func = p => p.IsRepairMan;
            if (!string.IsNullOrWhiteSpace(pageBase.mobile))
            {
                func = func.And(p => p.Mobile == pageBase.mobile);
            }

            var list = await _repository.PageListAsync(func.Compile(), p => p.Id, pageBase);
            var dtoList = list.List.MapTo<List<RepairManDTO>>();
            dtoList.ForEach(p =>
            {
                var nameList = community.Where(x => x.RepairManId == p.Id).Select(x => x.Name).ToList();
                p.CommunityName = string.Join(",", nameList);
            });

            var result = new QueryResult<RepairManDTO>();
            result.List = dtoList;
            result.Total = list.Total;
            return result;
        }

        public async Task<QueryResult<RepairManDTO>> GetAllRepairManList(QueryRepairManModel pageBase)
        {
            var sql = $" SELECT * from Users where IsRepairMan = 1 ";
            if (!string.IsNullOrWhiteSpace(pageBase.mobile))
            {
                sql += $" and Mobile = '{pageBase.mobile}' ";
            }

            var list = await DapperService.PageList<RepairManDTO>(sql, pageBase);

            var commList = await _repairManRepository.GetAllAsync();
            list.List.ForEach(p =>
            {
                p.CommunityName = string.Join(',', commList.Where(x => x.UserId == p.Id).Select(x => x.CommunityName).ToList());
            });
            return list;
        }

        public async Task<QueryResult<RepairManDTO>> GetRepairManList(QueryRepairManModel pageBase)
        {
            var sql = "select u.* from Community c right join Users u on c.RepairManId = u.Id where u.IsAdmin = 0 and u.IsRepairMan = 1 ";
            if (!string.IsNullOrWhiteSpace(pageBase.mobile))
            {
                sql += $" and u.mobile = '{pageBase.mobile}' ";
            }

            if (pageBase.communnityId.HasValue)
            {
                sql += $" and c.id = {pageBase.communnityId.Value} ";
            }

            var list = await DapperService.PageList<RepairManDTO>(sql, pageBase, "group by u.id");

            var commList = await _communityService.GetAllAsync();
            list.List.ForEach(p =>
            {
                var nameList = commList.Where(x => x.RepairManId == p.Id).Select(x => x.Name).ToList();
                p.CommunityName = string.Join("，", nameList);
                p.AdminName = commList.FirstOrDefault(x => x.AdminId == p.Id)?.Name;
            });
            return list;
        }

        public async Task MakeRepairMan(int userId, string communityIds)
        {
            var sql = $"update Users set IsRepairMan = 1 where id = {userId}";
            await DapperService.Execute(sql);

            //var hasMan = await _repairManRepository.CountAsync(p => p.UserId == userId);
            //if (hasMan > 0)
            //{
            //    var sql2 = $"update RepairMan set CommunityIds = '{communityIds}' ";
            //    await DapperService.Execute(sql2);
            //}
            //else
            //{
            //    var repairMan = new RepairMan()
            //    {
            //        UserId = userId,
            //        CommunityIds = communityIds
            //    };
            //    await _repairManRepository.InsertAsync(repairMan);
            //}
        }

        public async Task CancleRepairMan(int userId)
        {
            var sql = $"update Users set IsRepairMan = 0 where id = {userId}";
            await DapperService.Execute(sql);

            var sql2 = $"delete from RepairMan where UserId = {userId}";
            await DapperService.Execute(sql2);
        }

        public async Task<bool> AdminLogin(string mobile, string pwd)
        {
            var count = await _repository.CountAsync(p => p.Mobile == mobile
                                                          && p.PassWord == pwd
                                                          && p.IsAdmin);
            return count > 0 ? true : false;
        }
    }
}