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
        private readonly IRepository<RepairMan, int> _repairManRepository;

        public CommunityService(IRepository<Community, int> repository
            ,IRepository<User, int> userRepository
            , IRepository<RepairMan, int> repairManRepository)
            : base(repository)
        {
            _userRepository = userRepository;
            _repairManRepository = repairManRepository;
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

            var idList = communityList.List.Select(p => p.Id).ToList();
            var manList = await _repairManRepository.GetListAsync(p => idList.Contains(p.CommunityId));
            communityList.List.ForEach(p =>
            {
                
                p.AdminName = user.FirstOrDefault(x => x.Id == p.AdminId)?.Name;
                p.RepairManName = string.Join(',', manList.Where(x => x.CommunityId == p.Id).Select(x => x.RepairManName).ToList());
            });
            
            
            return communityList;
        }

        public async Task SetCommuityRepairMan(int commuityId, int userId)
        {
            var sql = $"update Community set RepairManId = {userId} where Id = {commuityId} and RepairManId > 0 ";
            await DapperService.Execute(sql);

            var u = await _userRepository.FirstOrDefultAsync(p => p.Id == userId);
            var comm = await _repository.FirstOrDefultAsync(p => p.Id == commuityId);
            var man = new RepairMan()
            {
                CommunityId = commuityId,
                UserId = userId,
                RepairManName = u?.Name,
                CommunityName = comm?.Name
            };
            await _repairManRepository.InsertAsync(man);
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