using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;

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

            const string Caption = "Organisation checks";
            const string Heading = "Legal name check";


            var model = new LegalNamePageViewModel();
            model.ApplicationId = request.ApplicationId;
            

            // go get UKPRN
            // use cache?
            var ukprn = await  _qnaApiClient.GetQuestionTag(model.ApplicationId, "UKPRN");
            model.Ukrpn = ukprn;


            // go get submitted on
            var applicationDetails = await _applyApiClient.GetApplication(model.ApplicationId);

            DateTime? applicationSubmittedOn = null;
            if (applicationDetails?.ApplyData?.ApplyDetails?.ApplicationSubmittedOn != null)
                applicationSubmittedOn = applicationDetails.ApplyData.ApplyDetails.ApplicationSubmittedOn;
            model.ApplicationSubmittedOn = applicationSubmittedOn;


            //go get Legal name stored in qna
            // use cache?
            var legalName = await _qnaApiClient.GetQuestionTag(model.ApplicationId, "UKRLPLegalName");
            model.ApplyLegalName = legalName;




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

            
           
        


            // the following 2 will be dynamic, depending on whether its a company or charity
            // these two depend on company etc
            var companiesHouseData = "CompaniesHouse: LegalName";

            var charityCommissionData = "CharityCommission: LegalName";
            model.CompaniesHouseLegalName = companiesHouseData;
            model.CharityCommissionLegalName = charityCommissionData;

            // write this model straight to the database, and then return it


            return model;
        }
    }
}
