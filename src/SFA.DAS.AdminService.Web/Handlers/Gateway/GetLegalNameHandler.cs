using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore.Internal;
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
            const string PageId = "1-10";

            var model = new LegalNamePageViewModel();
            model.ApplicationId = request.ApplicationId;
            model.PageId = PageId;

            model.Tables = new List<TabularData>();
            model.SummaryList = new TabularData();

            model.Caption = Caption;
            model.Heading = Heading;

            //MFCMFC DO THE SHUTTER PAGE IF THE LINK IS DOWN
            // go get UKPRN
            // use cache
            var ukprn = await  _qnaApiClient.GetQuestionTag(model.ApplicationId, "UKPRN");

            var applicationDetails = await _applyApiClient.GetApplication(model.ApplicationId);

            DateTime? applicationSubmittedOn = null;

            if (applicationDetails?.ApplyData?.ApplyDetails?.ApplicationSubmittedOn != null)
                applicationSubmittedOn = applicationDetails.ApplyData.ApplyDetails.ApplicationSubmittedOn;

            var legalName = await _qnaApiClient.GetQuestionTag(model.ApplicationId, "UKRLPLegalName");


            var ukrlpLegalName = "";

            var ukrlpData = await _roatpApiClient.GetUkrlpProviderDetails(ukprn);
            if (ukrlpData.Any())
            {
                var ukrlpDetail = ukrlpData.First();
                ukrlpLegalName = ukrlpDetail.ProviderName;
            }



            //var x= _applyApiClient.Get
            model.Ukrpn = ukprn;
            model.ApplicationSubmittedOn = applicationSubmittedOn;
            model.SourcesCheckedOn = DateTime.Now;

            // building the tables
            //var table = new TabularData { DataRows = new List<TabularDataRow>(), HeadingTitles = new List<string> { "Source", "Legal name" } };

            //table.DataRows.Add(new TabularDataRow { Columns = new List<string> { "Submitted application data", legalName } });
            //table.DataRows.Add(new TabularDataRow { Columns = new List<string> { "UKRLP data", ukrlpData } });
            var companiesHouseData = "CompaniesHouse: LegalName";

            var charityCommissionData = "CharityCommission: LegalName";
            model.ApplyLegalName = legalName;
            model.UkrlpLegalName = ukrlpLegalName;
            model.CompaniesHouseLegalName = companiesHouseData;
            model.CharityCommissionLegalName = charityCommissionData;


            // the following 2 will be dynamic, depending on whether its a company or charity
            // these two depend on company etc
          
           

            // write this model straight to the database, and then return it


            return model;
        }
    }
}
