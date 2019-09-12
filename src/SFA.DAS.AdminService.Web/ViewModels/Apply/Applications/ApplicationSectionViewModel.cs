using SFA.DAS.AssessorService.ApplyTypes;
using System;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class ApplicationSectionViewModel
    {
        public string ApplicationReference { get; }
        public string LegalName { get; }
        public string TradingName { get; }
        public string ProviderName { get; }
        public int? Ukprn { get; }
        public string CompanyNumber { get; }

        public ApplicationSection Section { get; }

        public string Title { get; }

        public Guid ApplicationId { get; }

        public int SequenceId { get; }

        public int SectionId { get; }

        public bool? IsSectionComplete { get; set; }

        public ApplicationSectionViewModel(Guid applicationId, int sequenceId,  int sectionId, ApplicationSection section, AssessorService.ApplyTypes.Application application)
        {
            if (section != null)
            {
                Section = section;
                Title = section.Title;
                ApplicationId = section.ApplicationId;
                SequenceId = section.SequenceId;
                SectionId = section.SectionId;

                if (section.Status == ApplicationSectionStatus.Evaluated)
                {
                    IsSectionComplete = true;
                }
            }
            else
            {
                ApplicationId = applicationId;
                SequenceId = sequenceId;
                SectionId = sectionId;
            }

            if (application != null)
            {
                if (application.ApplicationData != null)
                {
                    ApplicationReference = application.ApplicationData.ReferenceNumber;
                }

                if (application.ApplyingOrganisation?.OrganisationData != null)
                {
                    Ukprn = application.ApplyingOrganisation.EndPointAssessorUkprn;
                    LegalName = application.ApplyingOrganisation.OrganisationData.LegalName;
                    TradingName = application.ApplyingOrganisation.OrganisationData.TradingName;
                    ProviderName = application.ApplyingOrganisation.OrganisationData.ProviderName;
                    CompanyNumber = application.ApplyingOrganisation.OrganisationData.CompanyNumber;
                }
            }
        }
    }
}
