using Repair.AutoMapper;
using Repair.EntityFramework;
using Repair.EntityFramework.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Repair.Entitys;
using Repair.Models;
using PageBase = Microsoft.AspNetCore.Mvc.RazorPages.PageBase;

namespace Repair.Services
{
    public class CommunityService : BaseService<Community>
    {
        private readonly IRepository<User, int> _userRepository;
        public CommunityService(IRepository<Community, int> repository
        ,IRepository<User, int> userRepository)
            : base(repository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<Community>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list;
        }


        public async Task<QueryResult<CommunityDTO>> GetList(QueryCommunityModel model)
        {
            Expression<Func<Community, bool>> func = p=>true;
            if (model.AdminId.HasValue)
            {
                func = func.And(p => p.AdminId == model.AdminId);
            }

            var user = await _userRepository.GetAllAsync();

            var communityList = await _repository.PageListAsync<CommunityDTO>(func.Compile(), p => p.Id.Value, model);
            communityList.List.ForEach(p =>
            {
                p.AdminName = user.FirstOrDefault(x => x.Id == p.AdminId)?.Name;
                p.RepairManName = user.FirstOrDefault(x => x.Id == p.RepairManId)?.Name;
            });
            
            
            return communityList;
        }

        public async Task SetCommuityRepairMan(int commuityId, int userId)
        {
            var sql = $"update Community set RepairManId = {userId} where Id = {commuityId} ";
            await DapperService.Execute(sql);
        }
        
        public async Task SetCommuityAdmin(int commuityId, int userId)
        {
            var sql = $"update Community set AdminId = {userId} where Id = {commuityId} ";
            await DapperService.Execute(sql);
            var sql2 = $"update Users set IsAdmin = 1 where Id = {userId}";
            await DapperService.Execute(sql2);
        }

        public async Task<string> GetCommunityRepairMan(int id)
        {
            var community = await _repository.FirstOrDefultAsync(p=>p.Id == id);
            var user = await _userRepository.FirstOrDefultAsync(p => p.Id == community.RepairManId);
            return user?.Name;
        }
        
        public async Task<string> GetCommunityAdminMan(int id)
        {
            var community = await _repository.FirstOrDefultAsync(p=>p.Id == id);
            var user = await _userRepository.FirstOrDefultAsync(p => p.Id == community.AdminId);
            return user?.Name;
        }
    }
}