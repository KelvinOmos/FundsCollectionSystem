using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CollectionSystem.Application.Wrappers
{
    public class Response<T>
    {
        public Response()
        {
        }
        public Response(T data, string message = null, string responseCode = "", bool succeeded = false, long code = 0)
        {
            Succeeded = succeeded;
            Message = message;
            Description = message;
            Data = data;            
            ResponseCode = responseCode;
            Code = code;
        }
        public Response(string message)
        {
            Succeeded = false;
            Message = message;
        }
        public bool Succeeded { get; set; }
        public string ResponseCode { get; set; }
        public long Code { get; set; }
        public string Message { get; set; }
        public string Description { get; set; }
        public List<string> Errors { get; set; }
        public T Data { get; set; }        
    }

    public enum ApiResponseCodes
    {
        [Description("Server error occured, please try again.")]
        EXCEPTION = -5,
        [Description("Unauthorized Access")]
        UNAUTHORIZED = -4,
        NOT_FOUND = -3,
        INVALID_REQUEST = -2,
        [Description("ERROR")]
        ERROR = -1,
        [Description("FAIL")]
        FAIL = 2,
        [Description("SUCCESS")]
        OK = 1,
        INACTIVE_ACCOUNT = -10,
        EMAIL_NOT_CONFIRMED = -11,
    }
}
