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
            model.Tables = new List<TabularData>();
            model.SummaryList = new TabularData();
            
            model.OptionPass = new Option {Label = "Pass", Value = "Pass", Heading = "Add comments (optional)"};
            model.OptionFail = new Option {Label = "Fail", Value = "Fail", Heading = "Add comments (mandatory)"};
            model.OptionInProgress = new Option
                {Label = "In progress", Value = "In Progress", Heading = "Add comments (optional)"};

          
            model = BuildPageView(pageId, model);
            
            return model;
        }

        private RoatpGatewayPageViewModel BuildPageView(string pageId, RoatpGatewayPageViewModel model)
        {
            var gatewayPageConfiguration = GetConfigurationForPageId(pageId);
            if (gatewayPageConfiguration== null) return new RoatpGatewayPageViewModel();

            model.NextPageId = gatewayPageConfiguration.NextPageId;
            model.Caption = gatewayPageConfiguration.Caption;
            model.Heading = gatewayPageConfiguration.Heading;
            var textListingConfiguration = gatewayPageConfiguration.TextListing;
            var textListing = new TabularData {DataRows = RollupDataRows(textListingConfiguration)};
            var summaryListConfiguration = gatewayPageConfiguration.SummaryList;
            var summaryList = new TabularData { DataRows = RollupDataRows(summaryListConfiguration) };

            var tableListings = new List<TabularData>();
            var tablesListingConfiguration = gatewayPageConfiguration.Tables;

            if (tablesListingConfiguration != null)
            {
                foreach (var table in tablesListingConfiguration)
                {
                    var tableToAdd = new TabularData
                    {
                        HeadingTitles = table.HeadingTitles, DataRows = RollupDataRows(table.DataRows)
                    };
                    tableListings.Add(tableToAdd);
                }
            }

            model.TextListing = textListing;
            model.Tables = tableListings;
            model.SummaryList = summaryList;

            return model;
        }

        private List<TabularDataRow> RollupDataRows(List<DataRow> listing)
        {
            var dataRows = new List<TabularDataRow>();
            if (listing == null) return dataRows;

            foreach (var row in listing)
            {
                var columns = new List<string>();
                foreach (var element in row.Elements)
                {
                    if (element.Source == "Text")
                    {
                        columns.Add(element.Value);
                    }
                    else
                    {
                        // needs work to use MappingServices
                        columns.Add("Source: " + element.Source + ": " + element.Key);
                    }
                }
                
                // add in mapping
                dataRows.Add(new TabularDataRow {Columns = columns, DetailsValue = "Source: Apply : " + row.DetailsKey, DetailsLabel = row.DetailsLabel});
            }

            return dataRows;
        }


        private GatewayPageConfiguration GetConfigurationForPageId(string pageId)
        {
            return _gatewayPages.FirstOrDefault(x => x.PageId == pageId);
        }
    }
}
