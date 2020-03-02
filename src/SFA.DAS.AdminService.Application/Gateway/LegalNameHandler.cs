using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AdminService.Application.Models;
using SFA.DAS.AdminService.Application.ViewModels;

namespace SFA.DAS.AdminService.Application.Gateway
{
    public class LegalNameHandler : IRequestHandler<LegalNameRequest, RoatpGatewayPageViewModel>
    {
        public async Task<RoatpGatewayPageViewModel> Handle(LegalNameRequest request, CancellationToken cancellationToken)

        {
            const string Caption = "Organisation checks";
            const string Heading = "Legal name check";
            const string PageId = "legal-name";
            var model = new RoatpGatewayPageViewModel();
            model.ApplicationId = request.ApplicationId;
            model.PageId = PageId;
            // model.NextPageId = "shutter"; //shutter page id
            model.TextListing = new TabularData();
            model.Tables = new List<TabularData>();
            model.SummaryList = new TabularData();

            model.OptionPass = new Option { Label = "Pass", Value = "Pass", Heading = "Add comments (optional)" };
            model.OptionFail = new Option { Label = "Fail", Value = "Fail", Heading = "Add comments (mandatory)" };
            model.OptionInProgress = new Option
            { Label = "In progress", Value = "In Progress", Heading = "Add comments (optional)" };

            model.NextPageId = PageId; /// needs to be actual next page
            model.Caption = Caption;
            model.Heading = Heading;
            var ukprnValue = "ApplyQuestionTag: UKPRN";
            var applicationSubmittedOn = "ApplySpecial: SubmittedOnDate";
            var applicationSourcesCheckedOn = "ApplySpecial: CheckedOnDate";
            var submittedApplicationData = "ApplyQuestionTag: UKRLPLegalName";
            var ukrlpData = "UKRLP: UKRLPLegalName";

            // these two depend on company etc
            var companiesHouseData = "CompaniesHouse: LegalName";
            var charityCommissionData = "CharityCommission: LegalName";


            var textListing = new TabularData { DataRows = new List<TabularDataRow>() };

            // building the textListing
            textListing.DataRows.Add(new TabularDataRow { Columns = new List<string> { $"UKPRN: {ukprnValue}" } });
            textListing.DataRows.Add(new TabularDataRow { Columns = new List<string> { $"Application submitted on: {applicationSubmittedOn}" } });
            textListing.DataRows.Add(new TabularDataRow { Columns = new List<string> { $"Sources checked on: {applicationSourcesCheckedOn}" } });
            model.TextListing = textListing;

            // building the tables
            var table = new TabularData { DataRows = new List<TabularDataRow>(), HeadingTitles = new List<string> { "Source", "Legal name" } };

            table.DataRows.Add(new TabularDataRow { Columns = new List<string> { "Submitted application data", submittedApplicationData } });
            table.DataRows.Add(new TabularDataRow { Columns = new List<string> { "UKRLP data", ukrlpData } });
            table.DataRows.Add(new TabularDataRow { Columns = new List<string> { "Companies House data", companiesHouseData } });
            table.DataRows.Add(new TabularDataRow { Columns = new List<string> { "Charity Commission data", charityCommissionData } });

            model.Tables.Add(table);

            return model;
        }
    }
}
