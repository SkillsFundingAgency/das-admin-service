using System;

namespace SFA.DAS.RoatpAssessor.Application.Services
{
    public class TimeProvider : ITimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
