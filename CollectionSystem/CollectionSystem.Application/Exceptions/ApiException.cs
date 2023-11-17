using CollectionSystem.Application.Wrappers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CollectionSystem.Application.Exceptions
{
    public class ApiException : Exception
    {
        public ApiException() : base() { }

        public ApiException(string message) : base(message) { }

        public ApiException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }

    public class ApplicationException : ExceptionFilterAttribute
    {
        private readonly IHostEnvironment _env;
        private readonly ILogger _logger;

    public ApplicationException(IWebHostEnvironment env, ILogger<ApplicationException> logger)
    {
        _env = env;
        _logger = logger;
    }

    public override void OnException(ExceptionContext context)
    {
        var apiError = new Response<string>();
        apiError.Code = (long)ApiResponseCodes.EXCEPTION;

        if (context.Exception is UnauthorizedAccessException)
        {
            apiError.Code = (long)ApiResponseCodes.UNAUTHORIZED;
            apiError.Message = "Unauthorized";
            context.HttpContext.Response.StatusCode = 401;
            _logger.LogError("Unauthorized Access in Controller Filter.");
        }
        else
        {
            var msg = string.Empty;
            var stack = string.Empty;
            if (_env.IsDevelopment())
            {
                msg = context.Exception.GetBaseException().Message;
                stack = context.Exception.StackTrace;
            }
            else
            {
                stack = msg = "An unhandled error occurred.";
            }

            apiError.Data = msg;
            apiError.Message = stack;
            context.HttpContext.Response.StatusCode = 500;
            _logger.LogError(new EventId(0), context.Exception, msg);
        }

        context.Result = new ObjectResult(apiError);

        base.OnException(context);
    }
}
}
