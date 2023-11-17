using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CollectionSystem.WebApp.Wrappers
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
}
