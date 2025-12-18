

namespace SharedObject
{
    public class Pagination<T> where T : class
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public List<T> Data { get; set; } = new List<T>();

        public Pagination(int pageNumber, int pageSize, int totalRecords, List<T> data)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            Data = data;
            TotalPages = (int)Math.Ceiling((double)TotalRecords / PageSize);
        }
    }
}
