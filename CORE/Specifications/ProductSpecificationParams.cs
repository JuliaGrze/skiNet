using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Specifications
{
    //Represents the input data from the query string, i.e. what the user provided in the URL.
    public class ProductSpecificationParams
    {
        private List<String> _brands = [];
        private List<String> _types = [];

        private const int MaxPageSize = 50;
        public int PageIndex { get; set; } = 1; //which page user want to see

        private int _pageSize = 6; //number of elements per page
        public int PageSize 
        { 
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value; 
        }

        public List<string> Brands 
        { 
            get => _brands; //types = boards,gloves
            set 
            {
                //"Nike,Adidas" → ["Nike", "Adidas"]
                _brands = value.SelectMany(x => x.Split(',', 
                    StringSplitOptions.RemoveEmptyEntries)).ToList(); 
            }
        }

        
        public List<string> Types
        {
            get => _types; //types = boards,gloves
            set
            {
                _types = value.SelectMany(x => x.Split(',', 
                    StringSplitOptions.RemoveEmptyEntries)).ToList();
            }
        }

        public string? Sort { get; set; }

        private string? _search;
        public string Search 
        { 
            get => _search ?? "";
            set => _search = value.ToLower();
        }
    }
}
