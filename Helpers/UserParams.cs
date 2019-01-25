using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialApp.API.Helpers
{
    public class UserParams
    {
        private const int MaxPagSize = 50;
        public int PageNumber { get; set; } = 1;

        private int pageSize = 10;

        //if they request a page size of a thousand then the value is going to
        //be greater than the max page size.
        //So then we'll return a max page size which is 50.
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > MaxPagSize) ? MaxPagSize : value; }
        }

    }
}
