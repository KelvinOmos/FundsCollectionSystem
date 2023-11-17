using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionSystem.Application.Wrappers
{
    public interface IApiResponse<T> 
    {
        ApiResponseCodes Code { get; set; }
        string Description { get; set; }
        T Payload { get; set; } 
        int TotalCount { get; set; }
        List<string> Errors { get; set; }
      //  bool get HasErrors => Errors.Any();
    }

    public class ApiResponse
    {
        public ApiResponseCodes Code { get; set; }
        //public int Result
        //{
        //    get
        //    {
        //        return (int)Code;
        //    }
        //}
        public string Description { get; set; }
    }

    public class ApiResponse<T> : ApiResponse, IApiResponse<T>
    {
        public T Payload { get; set; } = default(T);
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int PageNo { get; set; }
        public int PageCount { get; set; }
        public string ResponseCode { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public bool HasErrors => Errors.Any();

        public static implicit operator ApiResponse<T>(ApiResponse<BaseViewModel> source)
        {
            var destination = new ApiResponse<T>()
            {
             
                TotalCount = source.TotalCount,
                Errors = source.Errors,
                Description = source.Description,
                Code = source.Code,
                PageNo = source.PageNo,
                PageCount=source.PageCount,
                TotalPages=source.TotalPages
            };
            return destination;
        }
    }   
}
