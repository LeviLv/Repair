using System.Collections.Generic;

namespace Repair.Models
{
    public class QueryResult<T> where T : class 
    {
        public List<T> List;
        public long Total;
    }
}