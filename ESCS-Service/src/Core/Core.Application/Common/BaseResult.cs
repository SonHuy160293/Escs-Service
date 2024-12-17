namespace Core.Application.Common
{
    public class BaseResult
    {
        public bool Succeeded { get; private set; }
        public string ErrorCode { get; private set; }
        public string ErrorMessage { get; private set; }

        protected BaseResult(bool succeeded = true, string errorCode = "0000", string errorMessage = "")
        {
            Succeeded = succeeded;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public static BaseResult Success()
        {
            return new BaseResult();
        }

        public static BaseResult Failure(string errorCode, string errorMessage)
        {
            return new BaseResult(false, errorCode, errorMessage);
        }

    }

    public class BaseResult<T> : BaseResult
    {
        public T Data { get; private set; }

        protected BaseResult(T data, bool succeeded = true, string errorCode = "0000", string errorMessage = "") : base(succeeded, errorCode, errorMessage)
        {
            Data = data;
        }

        public static BaseResult<T> Success(T data)
        {
            return new BaseResult<T>(data);
        }


        public static BaseResult<T> Failure(string errorCode, string errorMessage, T data = default(T))
        {
            return new BaseResult<T>(data, false, errorCode, errorMessage);
        }
    }

    public class BasePagingResult<T> : BaseResult
    {
        public List<T> Items { get; private set; }


        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        protected BasePagingResult(List<T> data, int count, int pageIndex, int pageSize, bool succeeded = true, string errorCode = "0000", string errorMessage = "")
            : base(succeeded, errorCode, errorMessage)
        {
            Items = data;
            TotalCount = count;
            PageSize = pageSize;
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        public static BasePagingResult<T> Create(List<T> items, int pageIndex, int pageSize)
        {
            var count = items.Count; // Count items directly from the list
            items = items.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();


            return new BasePagingResult<T>(items, count, pageIndex, pageSize);
        }






    }
}
