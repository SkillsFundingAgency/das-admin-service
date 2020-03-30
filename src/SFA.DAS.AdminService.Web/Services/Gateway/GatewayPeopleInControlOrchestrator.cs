using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Validators;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Infrastructure;
using SFA.DAS.AdminService.Web.Infrastructure.Apply;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Gateway;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;

namespace SFA.DAS.AdminService.Web.Services.Gateway
{
    public class GatewayPeopleInControlOrchestrator: IGatewayPeopleInControlOrchestrator
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly IRoatpOrganisationSummaryApiClient _organisationSummaryApiClient;
        private readonly ILogger<GatewayPeopleInControlOrchestrator> _logger;

        public GatewayPeopleInControlOrchestrator(IRoatpApplicationApiClient applyApiClient, IRoatpOrganisationSummaryApiClient organisationSummaryApiClient, ILogger<GatewayPeopleInControlOrchestrator> logger)
        {
            _applyApiClient = applyApiClient;
            _logger = logger;
            _organisationSummaryApiClient = organisationSummaryApiClient;
        }

        public async Task<PeopleInControlPageViewModel> GetPeopleInControlViewModel(GetPeopleInControlRequest request)
        {
             _logger.LogInformation($"Retrieving legal name details for application {request.ApplicationId}");

            var pageId = GatewayPageIds.LegalName;

            var commonDetails =
                await _applyApiClient.GetPageCommonDetails(request.ApplicationId, pageId, request.UserName);

            var model = new PeopleInControlPageViewModel
            {
                ApplicationId = request.ApplicationId,
                PageId = pageId,
                ApplyLegalName = commonDetails.LegalName,
                Ukprn = commonDetails.Ukprn,
                Status = commonDetails.Status,
                OptionPassText = commonDetails.OptionPassText,
                OptionFailText = commonDetails.OptionFailText,
                OptionInProgressText = commonDetails.OptionInProgressText,
                SourcesCheckedOn = commonDetails.CheckedOn,
                ApplicationSubmittedOn = commonDetails.ApplicationSubmittedOn,
                GatewayReviewStatus = commonDetails.GatewayReviewStatus
            };


            model.TypeOfOrganisation = await _organisationSummaryApiClient.GetTypeOfOrganisation(request.ApplicationId);
            //  Company
            //  Company and charity
            //  Charity
            //  Sole trader or partnership
            //  Statutory institute?


            // Stubbing while I work it out


            //model.CompanyDirectors = new TabularData();
            //model.PeopleWithSignificantControl = new TabularData();
            //model.Trustees = new TabularData();
            //model.WhosInControl = new TabularData();

            model.Tables = new List<TabularData>();
            var companyData = await _applyApiClient.GetCompaniesHouseDetails(request.ApplicationId);

            var directorsFromApplyStorage = new List<string>();

            if (companyData?.Directors != null && companyData.Directors.Any())
            {
                foreach (var director in companyData.Directors.OrderBy(x => x.Name))
                {
                    var directorRecord = director?.Name.ToUpper();
                    if (!string.IsNullOrEmpty(director.DateOfBirth) &&
                        DateTime.TryParse(director.DateOfBirth, out var dateOfBirth))
                    {
                        directorRecord = $"{directorRecord} ({dateOfBirth:MMM yyyy})";
                    }
                    directorsFromApplyStorage.Add(directorRecord);
                }
            }
            
            var directorsData = await _organisationSummaryApiClient.GetDirectors(request.ApplicationId);
            var directorsFromQnaStorage = new List<string>();

            if (directorsData?.DataRows != null && directorsData.DataRows.Any())
            {
                foreach (var director in directorsData.DataRows.Where(x=>x.Columns.Any()).OrderBy(x => x.Columns[0]))
                {
                    var directorRecord = director.Columns[0].ToUpper();

                    if (director.Columns.Any() && director.Columns.Count>=2)
                    {
                        directorRecord = $"{directorRecord} ({director.Columns[1]})";
                    }
                    directorsFromQnaStorage.Add(directorRecord);
                }
            }

            var upperDirectorCount = directorsFromApplyStorage.Count;

            if (directorsFromQnaStorage.Count > directorsFromApplyStorage.Count)
                upperDirectorCount = directorsFromQnaStorage.Count;


            var directorData = new TabularData
            {
                Caption = "Company directors",
                HeadingTitles = new List<string> {"Submitted application data", "Companies House data"},
                DataRows = new List<TabularDataRow>()
            };

            for (var i = 0; i < upperDirectorCount; i++)
            {
                var directorFromQnaStorage = string.Empty;
                if (directorsFromQnaStorage.Count >= i)
                    directorFromQnaStorage = directorsFromQnaStorage[i];
                var directorFromApplyStorage = string.Empty;
                if (directorsFromApplyStorage.Count >= i)
                    directorFromApplyStorage = directorsFromApplyStorage[i];

                var row = new TabularDataRow
                {
                    Columns = new List<string> {directorFromQnaStorage, directorFromApplyStorage}
                };

                directorData.DataRows.Add(row);
            }

            model.Tables.Add(directorData);


            var pscData = await _applyApiClient.GetCompaniesHouseDetails(request.ApplicationId);

            var pscDataFromApplyStorage = new List<string>();

            if (pscData?.PersonsWithSignificantControl != null && pscData.PersonsWithSignificantControl.Any())
            {
                foreach (var psc in pscData.PersonsWithSignificantControl.OrderBy(x => x.Name))
                {
                    var pscRecord = psc?.Name.ToUpper();
                    if (!string.IsNullOrEmpty(psc.DateOfBirth) &&
                        DateTime.TryParse(psc.DateOfBirth, out var dateOfBirth))
                    {
                        pscRecord = $"{pscRecord} ({dateOfBirth:MMM yyyy})";
                    }
                    pscDataFromApplyStorage.Add(pscRecord);
                }
            }

            var pscDataFromQna = await _organisationSummaryApiClient.GetPersonsWithSignificantControl(request.ApplicationId);
            var pscsFromQnaStorage = new List<string>();

            if (pscDataFromQna?.DataRows != null && pscDataFromQna.DataRows.Any())
            {
                foreach (var psc in pscDataFromQna.DataRows.Where(x => x.Columns.Any()).OrderBy(x => x.Columns[0]))
                {
                    var pscRecord = psc.Columns[0].ToUpper();

                    if (psc.Columns.Any() && psc.Columns.Count >= 2)
                    {
                        pscRecord = $"{pscRecord} ({psc.Columns[1]})";
                    }
                    pscsFromQnaStorage.Add(pscRecord);
                }
            }

            var upperPscCount = pscDataFromApplyStorage.Count;

            if (pscsFromQnaStorage.Count > pscDataFromApplyStorage.Count)
                upperPscCount = pscsFromQnaStorage.Count;


            var pscTableData = new TabularData
            {
                Caption = "People with significant control (PSC’s)",
                HeadingTitles = new List<string> { "Submitted application data", "Companies House data" },
                DataRows = new List<TabularDataRow>()
            };

            for (var i = 0; i < upperPscCount; i++)
            {
                var fromQnaStorage = string.Empty;
                if (pscsFromQnaStorage.Count >= i)
                    fromQnaStorage = pscsFromQnaStorage[i];
                var pcsFromApplyStorage = string.Empty;
                if (pscDataFromApplyStorage.Count >= i)
                    pcsFromApplyStorage = pscDataFromApplyStorage[i];

                var row = new TabularDataRow
                {
                    Columns = new List<string> { fromQnaStorage, pcsFromApplyStorage }
                };

                pscTableData.DataRows.Add(row);
            }


            model.Tables.Add(pscTableData); //pscs
            model.Tables.Add(new TabularData()); //trustees


            // PeopleWithSignificantControl
            // Only show if company and has PSC's
            // tag CompaniesHousePSCs
            //var pscsData = await _tabularDataRepository.GetTabularDataAnswer(applicationId, RoatpWorkflowQuestionTags.CompaniesHousePscs);
            //if (pscsData != null && pscsData.DataRows != null && pscsData.DataRows.Count > 0)
            //{
            //    // transform this
            //    model.PeopleWithSignificantControl = pscsData;
            //}

            var pscs = await _organisationSummaryApiClient.GetPersonsWithSignificantControl(request.ApplicationId);

            // Trustees
            // Only show if charity
            // tag CharityTrustees
            //var trusteesData = await _tabularDataRepository.GetTabularDataAnswer(applicationId, RoatpWorkflowQuestionTags.CharityCommissionTrustees);
            //if (trusteesData != null && trusteesData.DataRows != null && trusteesData.DataRows.Count > 0)
            //{
            //    model.Trustees = trusteesData;
            //}

            var trustees = await _organisationSummaryApiClient.GetTrustees(request.ApplicationId);
            // whosInControl
            //Only show is sole trader, partnership, statutory institute or any other manual entry
            // partnership, tag is AddPartners
            // sole trader  ?? nothing?? there is a tag for AddSoleTradeDOB  1,1980
            // when sole trader (SoleTradeOrPartnership": "Sole trader") use UKRLPLegalName, AddsoleTradDob....
            // when partnership ("SoleTradeOrPartnership": "Partnership") and individual ("PartnershipType": "Individual") use AddPartners
            // when partnership and organisation - name of org

            // gov statute tag was AddPeopleInControl

            // urn school was AddPeopleInControl


            //var whosInControl = new TabularData
            //{
            //    Caption = "Company directors",   // this could be anything....
            //    HeadingTitles = new List<string> {"Name", "Date of birth"},
            //    DataRows = new List<TabularDataRow>
            //    {
            //        new TabularDataRow {Columns = new List<string> {"BARTON, Catherine-Jane", "Jan 1969"}}
            //    }
            //};

            var peopleInControl = await _organisationSummaryApiClient.GetPeopleInControl(request.ApplicationId);
            model.Tables.Add(peopleInControl);


            //// Note as per comment by Cherky on 15:55 26/03/20, make all names in tables uppercase

            //model.WhosInControl = whosInControl;


            //var personData = await _tabularDataRepository.GetTabularDataAnswer(applicationId, RoatpWorkflowQuestionTags.AddPeopleInControl);
            //if (personData != null && personData.DataRows != null && personData.DataRows.Count > 0)
            //{
            //    result.Add(personData);
            //}



            return model;
        }
    }
}
