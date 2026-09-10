using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Application.Commands.CheckUserActionByCode;
using SFA.DAS.AdminService.Application.Queries.GetUserAllActivityByCode;
using SFA.DAS.AdminService.Application.Queries.GetUserActionByCode;
using SFA.DAS.AdminService.Application.Models;
using SFA.DAS.AdminService.Web.Orchestrators;
using SFA.DAS.AdminService.Application.Commands.UnlockUser;
using SFA.DAS.AdminService.Common.Models;

namespace SFA.DAS.AdminService.Web.UnitTests.Orchestrators
{
    [TestFixture]
    public class DigitalAccessOrchestratorTests
    {
        private Mock<IMediator> _mediatorMock;
        private DigitalAccessOrchestrator _sut;

        [SetUp]
        public void SetUp()
        {
            _mediatorMock = new Mock<IMediator>();
            _sut = new DigitalAccessOrchestrator(_mediatorMock.Object);
        }

        [Test]
        public async Task FindUserActionByReference_ReturnsNull_WhenMediatorReturnsNull()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<CheckUserActionByCodeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CheckUserActionByCodeCommandResult)null);

            var vm = await _sut.GetDigitalAccessReferenceViewModel("ref1", "user1");

            vm.Should().BeNull();
            _mediatorMock.Verify(m => m.Send(It.IsAny<CheckUserActionByCodeCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task FindUserActionByReference_ReturnsMappedViewModel_WhenMediatorReturnsResult()
        {
            var response = new CheckUserActionByCodeCommandResult
            {
                Id = 1,
                UserId = Guid.NewGuid(),
                ActionType = ActionType.Reprint,
                ActionTime = DateTime.UtcNow,
                ActionStatus = UserActionStatus.Viewed,
                Uln = 123,
                FamilyName = "Smith",
                GivenNames = "John",
                CertificateId = Guid.NewGuid(),
                CertificateType = CertificateType.Standard,
                CourseName = "Course",
                AdminActions = new List<AdminAction>
                {
                    new AdminAction { Username = "admin", ActionTime = DateTime.UtcNow, Action = "Viewed" }
                }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CheckUserActionByCodeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var vm = await _sut.GetDigitalAccessReferenceViewModel("ref2", "user2");

            vm.Should().NotBeNull();
            vm.ReferenceNumber.Should().Be("ref2");
            vm.ActionType.Should().Be(response.ActionType);
            _mediatorMock.Verify(m => m.Send(It.IsAny<CheckUserActionByCodeCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetUserNotFoundViewModel_ReturnsNull_WhenMediatorReturnsNull()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserActionByCodeQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetUserActionByCodeQueryResult)null);

            var vm = await _sut.GetUserNotFoundViewModel("ref3");

            vm.Should().BeNull();
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetUserActionByCodeQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetNonSpecificContactRequestViewModel_ReturnsNull_WhenMediatorReturnsNull()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserActionByCodeQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetUserActionByCodeQueryResult)null);

            var vm = await _sut.GetNonSpecificContactRequestViewModel("REFN");

            vm.Should().BeNull();
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetUserActionByCodeQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetNonSpecificContactRequestViewModel_ReturnsMappedViewModel_WhenMediatorReturnsResult()
        {
            var response = new GetUserActionByCodeQueryResult
            {
                Id = 3,
                UserId = Guid.NewGuid(),
                ActionType = ActionType.Contact,
                ActionTime = DateTime.UtcNow,
                GivenNames = "Diane",
                FamilyName = "Lockhart",
                Uln = 123456
            };


            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserActionByCodeQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var vm = await _sut.GetNonSpecificContactRequestViewModel("REFN");

            vm.Should().NotBeNull();
            vm.ReferenceNumber.Should().Be("REFN");
            vm.FirstName.Should().Be("Diane");
            vm.LastName.Should().Be("Lockhart");
            vm.Uln.Should().Be(123456);
            vm.RequestType.Should().Be("Incorrect details");

            _mediatorMock.Verify(m => m.Send(It.IsAny<GetUserActionByCodeQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetCertificateChangeRequestViewModel_Throws_WhenCertificateIdMissing()
        {
            var response = new GetUserActionByCodeQueryResult
            {
                Id = 10,
                UserId = Guid.NewGuid(),
                ActionType = ActionType.Contact,
                ActionTime = DateTime.UtcNow,
                CourseName = "Course",
                CertificateId = null,
                CertificateType = CertificateType.Standard,
                GivenNames = "A",
                FamilyName = "B",
                Uln = 1
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserActionByCodeQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            Func<Task> act = async () => await _sut.GetCertificateChangeRequestViewModel("REFX");

            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Missing CertificateId*");
        }

        [Test]
        public async Task GetCertificateChangeRequestViewModel_Throws_WhenCertificateTypeMissing()
        {
            var response = new GetUserActionByCodeQueryResult
            {
                Id = 11,
                UserId = Guid.NewGuid(),
                ActionType = ActionType.Contact,
                ActionTime = DateTime.UtcNow,
                CourseName = "Course",
                CertificateId = Guid.NewGuid(),
                CertificateType = null,
                GivenNames = "A",
                FamilyName = "B",
                Uln = 2
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserActionByCodeQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            Func<Task> act = async () => await _sut.GetCertificateChangeRequestViewModel("REFY");

            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Missing CertificateType*");
        }

        [Test]
        public async Task GetCertificatePrintRequestViewModel_Throws_WhenCertificateIdMissing()
        {
            var response = new GetUserActionByCodeQueryResult
            {
                Id = 12,
                UserId = Guid.NewGuid(),
                ActionType = ActionType.Reprint,
                ActionTime = DateTime.UtcNow,
                CourseName = "Course",
                CertificateId = null,
                CertificateType = CertificateType.Standard,
                GivenNames = "C",
                FamilyName = "D",
                Uln = 3
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserActionByCodeQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            Func<Task> act = async () => await _sut.GetCertificatePrintRequestViewModel("REFP");

            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Missing CertificateId*");
        }

        [Test]
        public async Task GetCertificatePrintRequestViewModel_Throws_WhenCertificateTypeMissing()
        {
            var response = new GetUserActionByCodeQueryResult
            {
                Id = 13,
                UserId = Guid.NewGuid(),
                ActionType = ActionType.Reprint,
                ActionTime = DateTime.UtcNow,
                CourseName = "Course",
                CertificateId = Guid.NewGuid(),
                CertificateType = null,
                GivenNames = "E",
                FamilyName = "F",
                Uln = 4
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserActionByCodeQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            Func<Task> act = async () => await _sut.GetCertificatePrintRequestViewModel("REFQ");

            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Missing CertificateType*");
        }

        [Test]
        public async Task GetCertificatePrintRequestViewModel_ReturnsNull_WhenMediatorReturnsNull()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserActionByCodeQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetUserActionByCodeQueryResult)null);

            var vm = await _sut.GetCertificatePrintRequestViewModel("REFP");

            vm.Should().BeNull();
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetUserActionByCodeQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetCertificatePrintRequestViewModel_ReturnsMappedViewModel_WhenMediatorReturnsResult()
        {
            var certId = Guid.NewGuid();

            var response = new GetUserActionByCodeQueryResult
            {
                Id = 2,
                UserId = Guid.NewGuid(),
                ActionType = ActionType.Reprint,
                ActionTime = DateTime.UtcNow,
                CourseName = "Course Y",
                CertificateId = certId,
                CertificateType = CertificateType.Standard,
                StandardCode = 456,
                GivenNames = "Eve",
                FamilyName = "Evans",
                Uln = 999
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserActionByCodeQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var vm = await _sut.GetCertificatePrintRequestViewModel("REFP");

            vm.Should().NotBeNull();
            vm.ReferenceNumber.Should().Be("REFP");
            vm.CourseName.Should().Be("Course Y");
            vm.CertificateId.Should().Be(certId);
            vm.CertificateType.Should().Be(response.CertificateType);
            vm.FirstName.Should().Be("Eve");
            vm.LastName.Should().Be("Evans");
            vm.Uln.Should().Be(999);
            vm.StandardCode.Should().Be(456);
            vm.RequestType.Should().Be("Reprint request");

            _mediatorMock.Verify(m => m.Send(It.IsAny<GetUserActionByCodeQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetUserNotFoundViewModel_ReturnsNames_WhenMediatorReturnsResult()
        {
            var response = new GetUserAllActivityByCodeQueryResult
            {
                GovUKIdentifier = "gov",
                EmailAddress = "diane@example.com",
                PhoneNumber = "01234",
                UserActions = new List<UserAction>
                {
                    new UserAction
                    {
                        GivenNames = "Wrong",
                        FamilyName = "Person",
                        ActionCode = "OTHER",
                        ActionType = ActionType.NotFound,
                        ActionTime = DateTime.UtcNow,
                        ActionStatus = UserActionStatus.Viewed
                    },
                    new UserAction
                    {
                        GivenNames = "Diane",
                        FamilyName = "Lockhart",
                        ActionCode = "REF4",
                        ActionType = ActionType.NotFound,
                        ActionTime = DateTime.UtcNow,
                        ActionStatus = UserActionStatus.Viewed
                    }
                }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserActionByCodeQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetUserActionByCodeQueryResult { GivenNames = "Diane", FamilyName = "Lockhart" });

            var vm = await _sut.GetUserNotFoundViewModel("ref4");

            vm.Should().NotBeNull();
            vm.ReferenceNumber.Should().Be("ref4");
            vm.FirstName.Should().Be("Diane");
            vm.LastName.Should().Be("Lockhart");
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetUserActionByCodeQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetUserNotMatchedViewModel_ReturnsNull_WhenMediatorReturnsNull()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserAllActivityByCodeQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetUserAllActivityByCodeQueryResult)null);

            var vm = await _sut.GetUserNotMatchedViewModel("ref-nm");

            vm.Should().BeNull();
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetUserAllActivityByCodeQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetUserNotMatchedViewModel_ReturnsMappedViewModel_WhenMediatorReturnsResult()
        {
            var response = new GetUserAllActivityByCodeQueryResult
            {
                IsLocked = true,
                GovUKIdentifier = "GOV123",
                EmailAddress = "alice@example.com",
                PhoneNumber = "0123456789",
                UserActions = new List<UserAction>
                {
                    new UserAction
                    {
                        Id = 1,
                        ActionCode = "REF1",
                        ActionType = ActionType.NotMatched,
                        ActionTime = DateTime.UtcNow,
                        ActionStatus = UserActionStatus.Viewed,
                        GivenNames = "Alice",
                        FamilyName = "Jones",
                        CertificateType = CertificateType.Standard,
                        UserMatches = new List<UserMatch>
                        {
                            new UserMatch { EventTime = DateTime.UtcNow.AddMinutes(-5), Uln = 111, CourseName = "Course A", DateAwarded = 2020, ProviderName = "Provider A", FamilyName = "Jones", CertificateType = CertificateType.Standard },
                            new UserMatch { EventTime = DateTime.UtcNow, Uln = 222, CourseName = "Course B", DateAwarded = 2021, ProviderName = "Provider B", FamilyName = "Jones", CertificateType = CertificateType.Standard }
                        },
                        AdminActions = new List<AdminAction>
                        {
                            new AdminAction { Username = "admin", Action = "Unlocked", ActionTime = DateTime.UtcNow.AddMinutes(1) }
                        }
                    }
                }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserAllActivityByCodeQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var vm = await _sut.GetUserNotMatchedViewModel("ref-nm");

            vm.Should().NotBeNull();
            vm.ReferenceNumber.Should().Be("ref-nm");
            vm.FirstName.Should().Be("Alice");
            vm.LastName.Should().Be("Jones");
            vm.IsUserLocked.Should().BeTrue();
            vm.History.Should().HaveCount(1);
            var item = vm.History[0];
            item.TagText.Should().Be(Web.Constants.DigitalAccessConstants.TagTextUnlocked );
            item.Attempts.Should().HaveCount(2);
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetUserAllActivityByCodeQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetUserNotMatchedViewModel_ExcludesNonNotMatchedActions_FromHistory()
        {
            var response = new GetUserAllActivityByCodeQueryResult
            {
                IsLocked = false,
                GovUKIdentifier = "GOV1",
                EmailAddress = "a@b.com",
                PhoneNumber = "01234",
                UserActions = new List<UserAction>
                {
                    new UserAction
                    {
                        Id = 1,
                        ActionCode = "REF-NM",
                        ActionType = ActionType.NotMatched,
                        ActionTime = DateTime.UtcNow.AddMinutes(-10),
                        GivenNames = "NM",
                        FamilyName = "User",
                        UserMatches = new List<UserMatch>
                        {
                            new UserMatch { EventTime = DateTime.UtcNow.AddMinutes(-9), Uln = 111, FamilyName = "User", CertificateType = CertificateType.Standard }
                        }
                    },
                    new UserAction
                    {
                        Id = 2,
                        ActionCode = "REF-RP",
                        ActionType = ActionType.Reprint,
                        ActionTime = DateTime.UtcNow,
                        GivenNames = "RP",
                        FamilyName = "Requester"
                    }
                }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserAllActivityByCodeQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var vm = await _sut.GetUserNotMatchedViewModel("ref-nm");

            vm.Should().NotBeNull();
            vm.History.Should().HaveCount(1);
            vm.FirstName.Should().Be("NM");
            vm.LastName.Should().Be("User");
        }

        [Test]
        public async Task GetUserNotMatchedViewModel_IgnoresOnlyNonNotMatchedActions()
        {
            var response = new GetUserAllActivityByCodeQueryResult
            {
                IsLocked = false,
                GovUKIdentifier = "GOV2",
                EmailAddress = "b@c.com",
                PhoneNumber = "09876",
                UserActions = new List<UserAction>
                {
                    new UserAction
                    {
                        Id = 2,
                        ActionCode = "REF-RP",
                        ActionType = ActionType.Reprint,
                        ActionTime = DateTime.UtcNow,
                        GivenNames = "RP",
                        FamilyName = "Requester"
                    }
                }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserAllActivityByCodeQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var vm = await _sut.GetUserNotMatchedViewModel("ref-rp");

            vm.Should().NotBeNull();
            vm.History.Should().BeEmpty();
            vm.FirstName.Should().BeNull();
            vm.LastName.Should().BeNull();
        }

        [Test]
        public async Task GetRestoreAccessViewModel_ReturnsNull_WhenMediatorReturnsNull()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserAllActivityByCodeQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetUserAllActivityByCodeQueryResult)null);

            var vm = await _sut.GetRestoreAccessViewModel("ref-x");

            vm.Should().BeNull();
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetUserAllActivityByCodeQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetRestoreAccessViewModel_ReturnsNull_WhenUserActionNotFound()
        {
            var response = new GetUserAllActivityByCodeQueryResult
            {
                UserId = Guid.NewGuid(),
                GovUKIdentifier = "GOV1",
                EmailAddress = "a@b.com",
                PhoneNumber = "0123",
                UserActions = null
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserAllActivityByCodeQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var vm = await _sut.GetRestoreAccessViewModel("ref-x");

            vm.Should().BeNull();
        }

        [Test]
        public async Task GetRestoreAccessViewModel_ReturnsViewModel_WhenUserActionFound()
        {
            var userId = Guid.NewGuid();
            var response = new GetUserAllActivityByCodeQueryResult
            {
                UserId = userId,
                GovUKIdentifier = "GOV2",
                EmailAddress = "c@d.com",
                PhoneNumber = "0987",
                UserActions = new List<UserAction>
                {
                    new UserAction { Id = 5, ActionCode = "REF5", GivenNames = "X", FamilyName = "Y", ActionType = ActionType.NotMatched, ActionTime = DateTime.UtcNow }
                }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserAllActivityByCodeQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var vm = await _sut.GetRestoreAccessViewModel("ref-x");

            vm.Should().NotBeNull();
            vm.UserId.Should().Be(userId);
            vm.UserActionId.Should().Be(5);
            vm.ReferenceNumber.Should().Be("ref-x");
        }

        [Test]
        public async Task UnlockUser_SendsUnlockUserCommandToMediator()
        {
            var userId = Guid.NewGuid();
            await _sut.UnlockUser(userId, "admin", 77);

            _mediatorMock.Verify(m => m.Send(It.IsAny<UnlockUserCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetCertificateChangeRequestViewModel_ReturnsNull_WhenMediatorReturnsNull()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserActionByCodeQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetUserActionByCodeQueryResult)null);

            var vm = await _sut.GetCertificateChangeRequestViewModel("REFX");

            vm.Should().BeNull();
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetUserActionByCodeQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetCertificateChangeRequestViewModel_ReturnsMappedViewModel_WhenMediatorReturnsResult()
        {
            var certId = Guid.NewGuid();

            var response = new GetUserActionByCodeQueryResult
            {
                Id = 1,
                UserId = Guid.NewGuid(),
                ActionType = ActionType.Contact,
                ActionTime = DateTime.UtcNow,
                CourseName = "Course X",
                CertificateId = certId,
                CertificateType = CertificateType.Framework,
                StandardCode = 123,
                GivenNames = "Bob",
                FamilyName = "Brown",
                Uln = 555
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserActionByCodeQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var vm = await _sut.GetCertificateChangeRequestViewModel("REFC");

            vm.Should().NotBeNull();
            vm.ReferenceNumber.Should().Be("REFC");
            vm.CourseName.Should().Be("Course X");
            vm.CertificateId.Should().Be(certId);
            vm.FirstName.Should().Be("Bob");
            vm.LastName.Should().Be("Brown");
            vm.Uln.Should().Be(555);
            vm.StandardCode.Should().Be(123);
            vm.LastName.Should().Be("Brown");
            vm.Uln.Should().Be(555);
            vm.StandardCode.Should().Be(123);
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetUserActionByCodeQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
