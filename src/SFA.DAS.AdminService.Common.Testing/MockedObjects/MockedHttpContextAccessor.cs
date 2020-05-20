using Microsoft.AspNetCore.Http;
using Moq;

namespace SFA.DAS.AdminService.Common.Testing.MockedObjects
{
    public class MockedHttpContextAccessor
    {
        public static Mock<IHttpContextAccessor> Setup()
        {
            var user = MockedUser.Setup();

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext { User = user };

            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
            return mockHttpContextAccessor;
        }
    }
}
