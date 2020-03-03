using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;

namespace SFA.DAS.AdminService.Web.Handlers.Gateway
{
    public class GetLegalNameHandler : IRequestHandler<GetLegalNameRequest, RoatpGatewayPageViewModel>
    {
        public async Task<RoatpGatewayPageViewModel> Handle(GetLegalNameRequest request, CancellationToken cancellationToken)

        {


            // go get the record for this application/pageId and dump this in the viewmodel and return it straight away if it exists

            const string Caption = "Organisation checks";
            const string Heading = "Legal name check";
            const string PageId = "1-10";

            var model = new RoatpGatewayPageViewModel();
            model.ApplicationId = request.ApplicationId;
            model.PageId = PageId;

            model.TextListing = new TabularData();
            model.Tables = new List<TabularData>();
            model.SummaryList = new TabularData();

            // These can go when Greg has finished with the page
            //model.OptionPass = new Option { Label = "Pass", Value = "Pass", Heading = "Add comments (optional)" };
            //model.OptionFail = new Option { Label = "Fail", Value = "Fail", Heading = "Add comments (mandatory)" };
            //model.OptionInProgress = new Option
            //{ Label = "In progress", Value = "In Progress", Heading = "Add comments (optional)" };
            /////////////////////////////////////////////////

            model.NextPageId = PageId; /// needs to be actual next page - will probably need the logic for trading name in this handler to jump over it if not rewuired
            model.Caption = Caption;
            model.Heading = Heading;
            var ukprnValue = "ApplyQuestionTag: UKPRN";
            var applicationSubmittedOn = "ApplySpecial: SubmittedOnDate";
            var applicationSourcesCheckedOn = "ApplySpecial: CheckedOnDate";
            var submittedApplicationData = "ApplyQuestionTag: UKRLPLegalName";
            var ukrlpData = "UKRLP: UKRLPLegalName";

           


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


            // the following 2 will be dynamic, depending on whether its a company or charity
            // these two depend on company etc
            var companiesHouseData = "CompaniesHouse: LegalName";
            table.DataRows.Add(new TabularDataRow { Columns = new List<string> { "Companies House data", companiesHouseData } });

            var charityCommissionData = "CharityCommission: LegalName";
            table.DataRows.Add(new TabularDataRow { Columns = new List<string> { "Charity Commission data", charityCommissionData } });

            model.Tables.Add(table);

            // write this model straight to the database, and then return it


            return model;
        }
    }
}
