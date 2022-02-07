using System.Data.Paging;

namespace WindowsFocusViewer
{
    public class PagingResult : IPagingResult
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int PagingDataCount { get; set; }
    }
}