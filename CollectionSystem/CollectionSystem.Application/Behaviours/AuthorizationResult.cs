using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionSystem.Application.Behaviours
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message)
            : base(message)
        {
        }
    }
    public class AuthorizationResult
    {
        public AuthorizationResult() { }
        private AuthorizationResult(bool isAuthorized, string failureMessage)
        {
            IsAuthorized = isAuthorized;
            FailureMessage = failureMessage;
        }
        public bool IsAuthorized { get; }
        public string FailureMessage { get; set; }

        public static AuthorizationResult Fail()
        {
            return new AuthorizationResult(false, null);
        }

        public static AuthorizationResult Fail(string failureMessage)
        {
            return new AuthorizationResult(false, failureMessage);
        }

        public static AuthorizationResult Succeed()
        {
            return new AuthorizationResult(true, null);
        }
    }



    public class RequestAuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {

        private readonly IEnumerable<IAuthorizer<TRequest>> _authorizers;

        public RequestAuthorizationBehavior(IEnumerable<IAuthorizer<TRequest>> authorizers)
        {
            _authorizers = authorizers;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            foreach (var authorizer in _authorizers)
            {
                var result = await authorizer.AuthorizeAsync(request, cancellationToken);
                if (!result.IsAuthorized)
                    throw new UnauthorizedException(result.FailureMessage);
            }

            return await next();
        }
    }
}
