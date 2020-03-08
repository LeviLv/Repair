using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Repair.Services;

namespace Repair.Controllers
{
    public class OtherController : BaseController
    {
        private CommunityService _communityService;

        public OtherController(CommunityService communityService)
        {
            _communityService = communityService;
        }

        public async Task<JsonResult> GetCommunityList()
        {
            var list = await _communityService.GetAllAsync();
            return Success(list);
        }
    }
}