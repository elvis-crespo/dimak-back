namespace dimax_front.Domain.DTOs
{
    public class PagedResponseDTO<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public bool IsLastPage { get; set; }
        public List<T> Data { get; set; } = new List<T>();
    }
}
