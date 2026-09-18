using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.AdminService.Web.Extensions;
using SFA.DAS.AdminService.Web.ViewModels.DigitalAccess;
using SFA.DAS.AdminService.Application.Commands.CheckUserActionByCode;
using SFA.DAS.AdminService.Application.Queries.GetUserAllActivityByCode;
using SFA.DAS.AdminService.Application.Queries.GetUserActionByCode;
using SFA.DAS.AdminService.Common.Models;
using System;
using SFA.DAS.AdminService.Application.Commands.UnlockUser;

namespace SFA.DAS.AdminService.Web.Orchestrators
{
    public class DigitalAccessOrchestrator : IDigitalAccessOrchestrator
    {
        private readonly IMediator _mediator;

        public DigitalAccessOrchestrator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<DigitalAccessReferenceSearchViewModel> GetDigitalAccessReferenceViewModel(string reference, string username)
        {
            var result = await _mediator.Send(new CheckUserActionByCodeCommand { Code = reference, Username = username });

            if (result == null)
                return null;

            var vm = new DigitalAccessReferenceSearchViewModel
            {
                ReferenceNumber = reference,
                ActionType = result.ActionType
            };

            return vm;
        }

        public async Task<UserNotFoundViewModel> GetUserNotFoundViewModel(string reference)
        {
            var result = await _mediator.Send(new GetUserActionByCodeQuery { Code = reference });

            if (result == null)
                return null;

            return new UserNotFoundViewModel
            {
                ReferenceNumber = reference,
                FirstName = result.GivenNames,
                LastName = result.FamilyName
            };
        }

        public async Task<UserNotMatchedViewModel> GetUserNotMatchedViewModel(string reference)
        {
            GetUserAllActivityByCodeQueryResult response = await _mediator.Send(new GetUserAllActivityByCodeQuery { Code = reference });

            if (response == null)
                return null;

            var history = new List<UserAccessHistoryItem>();

            var filteredActions = response.UserActions?.Where(u => u.ActionType == ActionType.NotMatched).ToList();

            if (filteredActions != null && filteredActions.Count > 0)
            {
                foreach (var ua in filteredActions.OrderByDescending(u => u.ActionTime))
                {
                    var item = new UserAccessHistoryItem
                    {
                        FormattedActionTime = ua.ActionTime.ToUkDateTimeString(),
                        ActionType = ua.ActionType,
                        ReferenceNumber = ua.ActionCode
                    };

                    if (ua.UserMatches != null && ua.UserMatches.Count > 0)
                    {
                        foreach (var um in ua.UserMatches.OrderBy(u => u.EventTime))
                        {
                            item.Attempts.Add(new UserAttempt
                            {
                                FormattedEventTime = um.EventTime.ToUkDateTimeString(),
                                Uln = um.Uln?.ToString() ?? Constants.DigitalAccessConstants.Unknown,
                                CourseName = string.IsNullOrWhiteSpace(um.CourseName) ? Constants.DigitalAccessConstants.Unknown : um.CourseName,
                                DateAwarded = um.DateAwarded?.ToString() ?? Constants.DigitalAccessConstants.Unknown,
                                ProviderName = string.IsNullOrWhiteSpace(um.ProviderName) ? Constants.DigitalAccessConstants.Unknown : um.ProviderName
                            });
                        }
                    }

                    if (ua.AdminActions != null && ua.AdminActions.Count > 0)
                    {
                        var unlocked = ua.AdminActions.Find(a =>
                        {
                            if (!Enum.TryParse<AdminActionType>(a.Action, true, out var adminActionType)) return false;
                            return adminActionType == AdminActionType.Unlocked && a.ActionTime > ua.ActionTime;
                        });

                        if (unlocked != null)
                        {
                            item.IsUnlocked = true;
                            item.UnlockedBy = unlocked.Username;
                            item.FormattedUnlockedTime = unlocked.ActionTime.ToUkDateTimeString();
                        }
                    }

                    item.TagClass = item.IsUnlocked ? Constants.DigitalAccessConstants.TagClassUnlocked : Constants.DigitalAccessConstants.TagClassLocked;
                    item.TagText = item.IsUnlocked ? Constants.DigitalAccessConstants.TagTextUnlocked : Constants.DigitalAccessConstants.TagTextLocked;

                    history.Add(item);
                }
            }

            var firstAction = filteredActions?.FirstOrDefault();

            var vm = new UserNotMatchedViewModel
            {
                ReferenceNumber = reference,
                FirstName = firstAction?.GivenNames,
                LastName = firstAction?.FamilyName,
                History = history,
                IsUserLocked = response.IsLocked
            };

            return vm;
        }

        public async Task<RestoreAccessViewModel> GetRestoreAccessViewModel(string reference)
        {
            var response = await _mediator.Send(new GetUserAllActivityByCodeQuery { Code = reference });

            if (response == null) return null;

            if (response.UserActions == null || response.UserActions.Count == 0) return null;

            var ua = response.UserActions
                .Where(u => u.ActionType == ActionType.NotMatched)
                .OrderByDescending(u => u.ActionTime)
                .FirstOrDefault();
            if (ua == null) return null;

            return new RestoreAccessViewModel
            {
                ReferenceNumber = reference,
                UserId = response.UserId,
                UserActionId = ua.Id
            };
        }

        public async Task UnlockUser(Guid userId, string username, long userActionId)
        {
            await _mediator.Send(new UnlockUserCommand
            {
                UserId = userId,
                Username = username,
                UserActionId = userActionId
            });
        }

        public async Task<CertificateChangeRequestViewModel> GetCertificateChangeRequestViewModel(string reference)
        {
            var ua = await _mediator.Send(new GetUserActionByCodeQuery { Code = reference });

            if (ua == null) return null;

            return new CertificateChangeRequestViewModel
            {
                ReferenceNumber = reference,
                CourseName = ua.CourseName ?? string.Empty,
                CertificateId = ua.CertificateId ?? throw new InvalidOperationException($"Missing CertificateId for reference {reference}"),
                CertificateType = ua.CertificateType ?? throw new InvalidOperationException($"Missing CertificateType for reference {reference}"),
                StandardCode = ua.StandardCode,
                FirstName = ua.GivenNames,
                LastName = ua.FamilyName,
                Uln = ua.Uln
            };
        }

        public async Task<NonSpecificContactRequestViewModel> GetNonSpecificContactRequestViewModel(string reference)
        {
            var ua = await _mediator.Send(new GetUserActionByCodeQuery { Code = reference });

            if (ua == null) return null;

            return new NonSpecificContactRequestViewModel
            {
                ReferenceNumber = reference,
                RequestType = "Incorrect details",
                FirstName = ua.GivenNames,
                LastName = ua.FamilyName,
                Uln = ua.Uln
            };
        }

        public async Task<CertificatePrintRequestViewModel> GetCertificatePrintRequestViewModel(string reference)
        {
            var ua = await _mediator.Send(new GetUserActionByCodeQuery { Code = reference });

            if (ua == null) return null;

            return new CertificatePrintRequestViewModel
            {
                ReferenceNumber = reference,
                RequestType = "Reprint request",
                CourseName = ua.CourseName ?? string.Empty,
                CertificateId = ua.CertificateId ?? throw new InvalidOperationException($"Missing CertificateId for reference {reference}"),
                CertificateType = ua.CertificateType ?? throw new InvalidOperationException($"Missing CertificateType for reference {reference}"),
                StandardCode = ua.StandardCode,
                FirstName = ua.GivenNames,
                LastName = ua.FamilyName,
                Uln = ua.Uln
            };
        }
    }
}
