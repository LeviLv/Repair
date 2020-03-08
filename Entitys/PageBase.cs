using System.Collections.Generic;

namespace Repair.Entitys
{
    public class PageBase
    {
        public PageBase()
        {
            PageIndex = 0;
            PageSize = 10;
        }

        public List<int> Role { get; set; }
        
        public int PageIndex { get; set; }
        
        public int PageSize { get; set; }
    }
}