using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Specifications
{
    public class PagingParams
    {
        private const int MaxPageSize = 50;
        public int PageIndex { get; set; } = 1; //which page user want to see

        private int _pageSize = 6; //number of elements per page
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
