using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Register;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using System.Collections.Generic;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Register
{
    public class RegisterAddStandardBase : RegisterBase
    {
        protected RegisterAddOrganisationStandardViewModel AddOrganisationStandardViewModel;

        [SetUp]
        public void BaseArrange()
        {
            ControllerSession = new Mock<IControllerSession>();
        }

        protected void SetupCreateEpaOrganisationStandard(string orgStandardId)
        {
            ApiClient.Setup(c => c.CreateEpaOrganisationStandard(It.IsAny<CreateEpaOrganisationStandardRequest>()))
                .ReturnsAsync(orgStandardId);
        }

        protected void SetUpAddStandardViewModelWithStandard()
        {
            AddOrganisationStandardViewModel = GetAddStandardViewModel();

            ControllerSession.Setup(p => p.AddOrganisationStandardViewModel).Returns(AddOrganisationStandardViewModel);
        }

        protected void SetUpEmptyAddStandardViewModel()
        {
            ControllerSession.Setup(p => p.AddOrganisationStandardViewModel).Returns((RegisterAddOrganisationStandardViewModel)null);
        }

        private RegisterAddOrganisationStandardViewModel GetAddStandardViewModel()
        {
            var ifateReferenceNumber = $"ST{Fixture.Create<int>()}";

            StandardVersion1 = Fixture.Build<OrganisationStandardVersion>()
                .With(v => v.IFateReferenceNumber, ifateReferenceNumber)
                .With(v => v.Version, "1.0")
                .Without(v => v.EffectiveFrom)
                .Without(v => v.EffectiveTo)
                .Without(v => v.DateVersionApproved)
                .Create();

            StandardVersion2 = Fixture.Build<OrganisationStandardVersion>()
                .With(v => v.IFateReferenceNumber, ifateReferenceNumber)
                .With(v => v.Version, "2.0")
                .Without(v => v.EffectiveFrom)
                .Without(v => v.EffectiveTo)
                .Without(v => v.DateVersionApproved)
                .Create();

            return Fixture.Build<RegisterAddOrganisationStandardViewModel>()
                .With(vm => vm.IFateReferenceNumber, ifateReferenceNumber)
                .With(vm => vm.Versions, new List<OrganisationStandardVersion>
                {
                    StandardVersion1,
                    StandardVersion2
                }).Create();
        }
    }
}
