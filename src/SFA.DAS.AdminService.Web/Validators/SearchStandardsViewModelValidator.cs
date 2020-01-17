using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AdminService.Web.Validators
{
    public class SearchStandardsViewModelValidator : AbstractValidator<SearchStandardsViewModel>
    {
        private readonly IOrganisationsApiClient _apiClient;

        public SearchStandardsViewModelValidator(ApiClientFactory<OrganisationsApiClient> apiClient)
        {
            _apiClient = apiClient.GetApiClient(ApplicationType.EPAO);

            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var searchstring = vm.StandardSearchString?.Trim().ToLower();
                searchstring = string.IsNullOrEmpty(searchstring) ? "" : searchstring;
                var rx = new System.Text.RegularExpressions.Regex("<[^>]*>/");
                searchstring = rx.Replace(searchstring, "");
                searchstring = searchstring.Replace("/", "");
                searchstring = searchstring.Replace("+", "");
                var searchTerm = Uri.EscapeUriString(searchstring);
                var validationResult = _apiClient.ValidateSearchStandards(searchTerm).Result;
                if (validationResult.IsValid) return;
                foreach (var error in validationResult.Errors)
                {
                    context.AddFailure(error.Field, error.ErrorMessage);
                }
            });
        }
    }
}
