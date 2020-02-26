using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.AdminService.Web.Configuration;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;

namespace SFA.DAS.AdminService.Web.Services
{
    public class GatewayCompositionService : IGatewayCompositionService
    {
        private readonly List<GatewayPageConfiguration> _gatewayPages;

        public GatewayCompositionService(IOptions<List<GatewayPageConfiguration>> gatewayPages)
        {
            _gatewayPages = gatewayPages.Value;
        }

        public RoatpGatewayPageViewModel GetViewModelForPage(Guid applicationId, string pageId)
        {
            var model = new RoatpGatewayPageViewModel();
            model.ApplicationId = applicationId;
            model.PageId = pageId;
            model.NextPageId = "shutter"; //shutter page id
            model.TextListing = new TabularData();
            model.SummaryList = new TabularData();
            model.OptionPass = new Option {Label = "Pass", Value = "Pass", Heading = "Add comments (optional)"};
            model.OptionFail = new Option {Label = "Fail", Value = "Fail", Heading = "Add comments"};
            model.OptionInProgress = new Option
                {Label = "In progress", Value = "In Progress", Heading = "Add comments (optional)"};


            if (pageId == "110")
            {

                var gatewayPageConfiguration = GetConfigurationForPageId(pageId);
                model.NextPageId = gatewayPageConfiguration.NextPageId;
                model.Caption = gatewayPageConfiguration.Caption;
                model.Heading = gatewayPageConfiguration.Heading;

                var textListing = new TabularData();
                //{
                //    DataRows = new List<TabularDataRow>
                //    {
                //        new TabularDataRow
                //        {
                //            Columns = new List<string> {"UKPRN: ", "12345678"}

                //        },
                //        new TabularDataRow
                //        {
                //            Columns = new List<string> {"Application submitted on: ", "17 Oct 2019"}

                //        },
                //        new TabularDataRow
                //        {
                //            Columns = new List<string> {"Sources checked on: ", "27 Nov 2019"}

                //        }
                //    }
                //};

                var textListings = gatewayPageConfiguration.TextListings;

                var dataRowsTextListing = new List<TabularDataRow>();

                foreach (var listing in textListings)
                {
                    var columns = new List<string>();
                    foreach (var element in listing.Elements)
                    {
                        if (element.Source == "Text")
                        {
                            columns.Add(element.Value);
                        }
                        else
                        {
                            // needs work
                            columns.Add("Source: " + element.Source + ": " + element.Key);
                        }
                    }

                    dataRowsTextListing.Add(new TabularDataRow {Columns = columns});

                }

                textListing.DataRows = dataRowsTextListing;


                var tableListing = new TabularData {HeadingTitles = new List<string> {"Source", "Legal Name"}};
                var dataRows = new List<TabularDataRow>
                {
                    new TabularDataRow
                    {
                        Id = pageId + "_1",
                        Columns = new List<string> {"Submitted application data", "ABC TRAINING LIMITED"}
                    },
                    new TabularDataRow
                    {
                        Id = pageId + "_2",
                        Columns = new List<string> {"UKRLP data", "ABC TRAINING LIMITED"}
                    },
                    new TabularDataRow
                    {
                        Id = pageId + "_3",
                        Columns = new List<string> {"Companies House data", "ABC TRAINING LIMITED"}
                    }
                };
                tableListing.DataRows = dataRows;
                model.TextListing = textListing;
                model.Tables = new List<TabularData> {tableListing};
            }

            ;

            if (pageId == "120")
            {
                model.NextPageId =
                    "120"; //== "tasklist"  // once dev starts, this will be next page or tasklist page....

                model.Caption = "Organisation checks";
                model.Heading = "Organisation status check";
                var textListing = new TabularData
                {
                    DataRows = new List<TabularDataRow>
                    {
                        new TabularDataRow
                        {
                            Columns = new List<string> {"UKPRN: ", "12345678"}

                        },
                        new TabularDataRow
                        {
                            Columns = new List<string> {"Application submitted on: ", "17 Oct 2019"}

                        },
                        new TabularDataRow
                        {
                            Columns = new List<string> {"Sources checked on: ", "27 Nov 2019"}

                        }
                    }
                };



                var tableListing = new TabularData {HeadingTitles = new List<string> {"Source", "Status"}};
                var dataRows = new List<TabularDataRow>
                {
                    new TabularDataRow
                    {
                        Id = pageId + "_1",
                        Columns = new List<string> {"UKRLP data", "Active"}
                    },
                    new TabularDataRow
                    {
                        Id = pageId + "_2",
                        Columns = new List<string> {"Companies House data", "Dissolved"}
                    }
                };
                tableListing.DataRows = dataRows;
                model.TextListing = textListing;
                model.Tables = new List<TabularData> {tableListing};
            }

            ;

            return model;
        }


        private GatewayPageConfiguration GetConfigurationForPageId(string pageId)
        {
            return _gatewayPages.FirstOrDefault(x => x.PageId == pageId);
        }
    }
}
