using System;

namespace SFA.DAS.RoatpAssessor.Application.Services
{
    public interface ITimeProvider
    {
        DateTime UtcNow { get; }
    }
}
