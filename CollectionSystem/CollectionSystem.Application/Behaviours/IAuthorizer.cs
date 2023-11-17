using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionSystem.Application.Behaviours
{
    public interface IAuthorizer<T>
    {
        Task<AuthorizationResult> AuthorizeAsync(T instance, CancellationToken cancellation = default);
    }
}
