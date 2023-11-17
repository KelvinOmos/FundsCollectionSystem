using System;
using System.Collections.Generic;
using System.Text;

namespace CollectionSystem.Application.Interfaces
{
    public interface IDateTimeService
    {
        DateTime NowUtc { get; }
    }
}
