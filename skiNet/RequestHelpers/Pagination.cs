namespace API.RequestHelpers
{
    public class Pagination<T>(int pageIndex, int pageSize, int count, IReadOnlyList<T> data) 
    {
        //current page number
        public int PageIndex { get; set; } = pageIndex;
        //Number of items per page
        public int PageSize { get; set; } = pageSize;
        //Total number of items (i.e. without pagination)
        public int Count { get; set; } = count;
        //List of elements of the current page
        public IReadOnlyList<T> Data { get; set; } = data;
    }
}
