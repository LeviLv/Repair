using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Repair.EntityFramework;
using Repair.EntityFramework.Domain;

namespace Repair.Services
{
    public class AdminRoleService : BaseService<AdminRole>
    {
        public AdminRoleService(IRepository<AdminRole, int> repository) 
            : base(repository)
        {
            
        }

        public async Task<String> GetAdminRoleString(int userId)
        {
            var list = await _repository.GetListAsync(p => p.UserId == userId);
            return String.Join(",",list.Select(p=>p.CommunityId).ToList());
        }

        public async Task<List<int>> GetAdminRoleList(int userId)
        {
            var list = await _repository.GetListAsync(p => p.UserId == userId);
            return list.Select(p => p.CommunityId).ToList();
        }
    }
}