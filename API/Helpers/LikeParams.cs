using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class LikeParams : PaginationParams
    {
        public int UserId { get; set; }
        public required string Predicate { get; set; } = "liked";
    }
}