using MediatR;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Handlers.Gateway
{
    public class GetLegalNameHandler : IRequestHandler<GetLegalNameRequest, LegalNamePageViewModel>
    {

        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly IRoatpApiClient _roatpApiClient;

        public GetLegalNameHandler(IRoatpApplicationApiClient applyApiClient, IQnaApiClient qnaApiClient, IRoatpApiClient roatpApiClient)
        {
            _applyApiClient = applyApiClient;
            _qnaApiClient = qnaApiClient;
            _roatpApiClient = roatpApiClient;
        }


        public async Task<LegalNamePageViewModel> Handle(GetLegalNameRequest request, CancellationToken cancellationToken)

        {
            // go get the record for this application/pageId and dump this in the viewmodel and return it straight away if it exists
            // else get gathering

            var model = new LegalNamePageViewModel {ApplicationId = request.ApplicationId};


            // go get UKPRN
            // use cache?
            var ukprn = await  _qnaApiClient.GetQuestionTag(model.ApplicationId, "UKPRN");
            model.Ukrpn = ukprn;


            // go get submitted on
            var applicationDetails = await _applyApiClient.GetApplication(model.ApplicationId);
            
            if (applicationDetails?.ApplyData?.ApplyDetails?.ApplicationSubmittedOn != null)
                model.ApplicationSubmittedOn = applicationDetails.ApplyData.ApplyDetails.ApplicationSubmittedOn;


            //go get Legal name stored in qna
            // use cache?
            model.ApplyLegalName = await _qnaApiClient.GetQuestionTag(model.ApplicationId, "UKRLPLegalName");
            

            // go get Legal name from ukrlp
            // use cache?
            var ukrlpLegalName = "";

            var ukrlpData = await _roatpApiClient.GetUkrlpProviderDetails(ukprn);
            if (ukrlpData.Any())
            {
                var ukrlpDetail = ukrlpData.First();
                ukrlpLegalName = ukrlpDetail.ProviderName;
            }
            model.UkrlpLegalName = ukrlpLegalName;



            model.SourcesCheckedOn = DateTime.Now;


            // get company information
            // companies house

            var companyNumber = "";
            var charityNumber = "";

            var qnaApplyData = await _qnaApiClient.GetApplicationData(model.ApplicationId);
          
            foreach (var variable in qnaApplyData)
            {
                if (variable.Value == null) continue;

                // put into cache???
                if (variable.Key == "UKRLPVerificationCompanyNumber" )
                {
                    companyNumber = variable.Value.ToString();
                }

                // pout into cache????
                if (variable.Key == "UKRLPVerificationCharityRegNumber")
                {
                    charityNumber = variable.Value.ToString();
                }
            }

    
            if (!string.IsNullOrEmpty(companyNumber))
            {
                var companyDetails = await _applyApiClient.GetCompanyDetails(companyNumber);

                if (companyDetails != null && !string.IsNullOrEmpty(companyDetails.CompanyName))
                    model.CompaniesHouseLegalName  = companyDetails.CompanyName;
            }

            if (!string.IsNullOrEmpty(charityNumber))
            {
                var charityDetails = await _applyApiClient.GetCharityDetails(charityNumber);

                if (charityDetails != null && !string.IsNullOrEmpty(charityDetails.Name))
                    model.CharityCommissionLegalName = charityDetails.Name;
            } 
         
            // write this model straight to the database, and then return it


            return model;
        }
    }
}
