using SFA.DAS.AssessorService.ApplyTypes;
using System;
using System.Linq;

namespace SFA.DAS.AdminService.Web.ViewModels.Apply.Applications
{
    public class ApplicationSequenceViewModel
    {
        public string ApplicationReference { get; }
        public string Standard { get; }
        public string LegalName { get; }
        public string TradingName { get; }
        public string ProviderName { get; }
        public int? Ukprn { get; }
        public string CompanyNumber { get; }
        public DateTime? FinancialDueDate { get; }

        public ApplicationSequence Sequence { get; }

        public Guid ApplicationId { get; }

        public int SequenceId { get; }


        public ApplicationSequenceViewModel(Guid applicationId, int sequenceId, ApplicationSequence sequence, AssessorService.ApplyTypes.Application application)
        {
            if (sequence != null)
            {
                Sequence = sequence;
                ApplicationId = sequence.ApplicationId;
                SequenceId = sequence.SequenceId;
            }
            else
            {
                ApplicationId = applicationId;
                SequenceId = sequenceId;
            }

            if (application != null)
            {
                if (application.ApplicationData != null)
                {
                    ApplicationReference = application.ApplicationData.ReferenceNumber;
                    Standard = $"{application.ApplicationData.StandardName} ({application.ApplicationData.StandardCode})";
                }

                if (application.ApplyingOrganisation?.OrganisationDataFromJson != null)
                {
                    Ukprn = application.ApplyingOrganisation.EndPointAssessorUkprn;
                    LegalName = application.ApplyingOrganisation.OrganisationDataFromJson.LegalName;
                    TradingName = application.ApplyingOrganisation.OrganisationDataFromJson.TradingName;
                    ProviderName = application.ApplyingOrganisation.OrganisationDataFromJson.ProviderName;
                    CompanyNumber = application.ApplyingOrganisation.OrganisationDataFromJson.CompanyNumber;
                

                    if (!sequence.Sections.All(s => s.SectionId != 3))
                        {
                            FinancialDueDate = application.ApplyingOrganisation.OrganisationDataFromJson.FhaDetails?.FinancialDueDate;
                        }
                    }
            }
        }
    }
}
 