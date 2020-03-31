using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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


            model.CompanyDirectors = new TabularData();
            model.PeopleWithSignificantControl = new TabularData();
            model.Trustees = new TabularData();
            model.WhosInControl = new TabularData();

            //var companyData = await _applyApiClient.GetCompaniesHouseDetails(request.ApplicationId);

            //var directorsFromApplyStorage = new List<string>();

            //if (companyData?.Directors != null && companyData.Directors.Any())
            //{
            //    foreach (var director in companyData.Directors.OrderBy(x => x.Name))
            //    {
            //        var directorRecord = director?.Name.ToUpper();
            //        if (!string.IsNullOrEmpty(director.DateOfBirth) &&
            //            DateTime.TryParse(director.DateOfBirth, out var dateOfBirth))
            //        {
            //            directorRecord = $"{directorRecord} ({dateOfBirth:MMM yyyy})";
            //        }
            //        if (!string.IsNullOrEmpty(directorRecord))
            //            directorsFromApplyStorage.Add(directorRecord);
            //    }
            //}

            var directorsDataFromApply = await _organisationSummaryApiClient.GetDirectorsFromApplyData(request.ApplicationId);
            var directorsFromApplyStorage = new List<string>();

            if (directorsDataFromApply != null && directorsDataFromApply.Any())
            {
                foreach (var director in directorsDataFromApply.OrderBy(x => x.Name))
                {
                    var directorRecord = director.Name.ToUpper();

                    if (!string.IsNullOrEmpty(director.DateOfBirth))
                    {
                        directorRecord = $"{directorRecord} ({director.DateOfBirth})";
                    }
                    if (!string.IsNullOrEmpty(directorRecord))
                        directorsFromApplyStorage.Add(directorRecord);
                }
            }


            var directorsData = await _organisationSummaryApiClient.GetDirectors(request.ApplicationId);
            var directorsFromQnaStorage = new List<string>();

            if (directorsData != null && directorsData.Any())
            {
                foreach (var director in directorsData.OrderBy(x => x.Name))
                {
                    var directorRecord = director.Name.ToUpper();

                    if (!string.IsNullOrEmpty(director.DateOfBirth))
                    {
                        directorRecord = $"{directorRecord} ({director.DateOfBirth})";
                    }
                    if (!string.IsNullOrEmpty(directorRecord))
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
                if (directorFromQnaStorage!=string.Empty || directorFromApplyStorage != string.Empty)
                    directorData.DataRows.Add(row);
            }

            model.CompanyDirectors=  directorData;


            model.CompanyDirectorsData = new PeopleInControlData
            {
                Caption = "Company directors",
                SubmittedApplicationHeading = "Submitted application data",
                ExternalSourceHeading = "Companies House data",
                FromExternalSource = directorsDataFromApply,
                FromSubmittedApplication = directorsData
            };

            // PeopleWithSignificantControl
            // Only show if company and has PSC's
            // tag CompaniesHousePSCs
            //var pscsData = await _tabularDataRepository.GetTabularDataAnswer(applicationId, RoatpWorkflowQuestionTags.CompaniesHousePscs);
            //if (pscsData != null && pscsData.DataRows != null && pscsData.DataRows.Count > 0)
            //{
            //    // transform this
            //    model.PeopleWithSignificantControl = pscsData;
            //}
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
                    if(!string.IsNullOrEmpty(pscRecord))
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
                if (pscsFromQnaStorage.Count > i)
                    fromQnaStorage = pscsFromQnaStorage[i];
                var pcsFromApplyStorage = string.Empty;
                if (pscDataFromApplyStorage.Count > i)
                    pcsFromApplyStorage = pscDataFromApplyStorage[i];

                if (fromQnaStorage != string.Empty || pcsFromApplyStorage != string.Empty)
                {
                    pscTableData.DataRows.Add(new TabularDataRow
                        {
                            Columns = new List<string> { fromQnaStorage, pcsFromApplyStorage }
                        });
                }
            }


            model.PeopleWithSignificantControl =   pscTableData; 
            model.Trustees = new TabularData(); //trustees


          

     
            // Trustees
            // Only show if charity
            // tag CharityTrustees
            //var trusteesData = await _tabularDataRepository.GetTabularDataAnswer(applicationId, RoatpWorkflowQuestionTags.CharityCommissionTrustees);
            //if (trusteesData != null && trusteesData.DataRows != null && trusteesData.DataRows.Count > 0)
            //{
            //    model.Trustees = trusteesData;
            //}

            var trusteesFromQna = await _organisationSummaryApiClient.GetTrustees(request.ApplicationId);
            var charityData = await _applyApiClient.GetCharityCommissionDetails(request.ApplicationId);




            var charityDataFromApplyStorage = new List<string>();

            if (charityData?.Trustees != null && charityData.Trustees.Any())
            {
                foreach (var trustee in charityData.Trustees.OrderBy(x => x.Name))
                {
                    var trusteeName = trustee?.Name.ToUpper();
                   if (!string.IsNullOrEmpty(trusteeName))

                       charityDataFromApplyStorage.Add(trusteeName);
                }
            }


            var trusteesFromQnaStorage = new List<string>();

            if (trusteesFromQna?.DataRows != null && trusteesFromQna.DataRows.Any())
            {
                foreach (var trustee in trusteesFromQna.DataRows.Where(x => x.Columns.Any()).OrderBy(x => x.Columns[0]))
                {
                    var trusteeRecord = trustee.Columns[0].ToUpper();

                    if (trustee.Columns.Any() && trustee.Columns.Count >= 2)
                    {
                        trusteeRecord = $"{trusteeRecord} ({trustee.Columns[1]})";
                    }
                    trusteesFromQnaStorage.Add(trusteeRecord);
                }
            }

            var upperTrusteeCount = charityDataFromApplyStorage.Count;

            if (trusteesFromQnaStorage.Count > charityDataFromApplyStorage.Count)
                upperTrusteeCount = pscsFromQnaStorage.Count;


            var trusteeTableData = new TabularData
            {
                Caption = "Trustees",
                HeadingTitles = new List<string> { "Submitted application data", "Charity commission data" },
                DataRows = new List<TabularDataRow>()
            };

            for (var i = 0; i < upperTrusteeCount; i++)
            {
                var fromQnaStorage = string.Empty;
                if (trusteesFromQnaStorage.Count > i)
                    fromQnaStorage = trusteesFromQnaStorage[i];
                var fromApplyStorage = string.Empty;
                if (charityDataFromApplyStorage.Count > i)
                    fromApplyStorage = charityDataFromApplyStorage[i];

                if (fromQnaStorage != string.Empty || fromApplyStorage != string.Empty)
                {
                    trusteeTableData.DataRows.Add(new TabularDataRow
                    {
                        Columns = new List<string> { fromQnaStorage, fromApplyStorage }
                    });
                }
            }


            model.Trustees = trusteeTableData;
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
            // this might use GetPeopleInControl, or alternatives for sole trader....

             var whosInControl = new TabularData
            {
                Caption = "Who's in control",
                HeadingTitles = new List<string> {"Submitted application data"},
                DataRows = new List<TabularDataRow>()
            };

             var peopleInControl = await _organisationSummaryApiClient.GetPeopleInControl(request.ApplicationId);
             if (peopleInControl == null)
             {
                 //MFCMFC need to debug/check this
                 peopleInControl = await _organisationSummaryApiClient.GetPartners(request.ApplicationId);
             }

             if (peopleInControl != null)
             {
                 if (peopleInControl?.DataRows != null && peopleInControl.DataRows.Any())
                 {
                     foreach (var row in peopleInControl.DataRows)
                     {
                         var person = string.Empty;
                         if (row.Columns.Count == 1)
                             person = row.Columns[0];

                         if (row.Columns.Count > 1)
                             person = $"{row.Columns[0]} ({row.Columns[1]})";

                         if (person != string.Empty)
                         {
                             whosInControl.DataRows.Add(new TabularDataRow
                                 {Columns = new List<string> {person.ToUpper()}});
                         }
                     }
                 }
            }
             else
             {
                 var soleTraderDob= await _organisationSummaryApiClient.GetSoleTraderDob(request.ApplicationId);

                  if (!string.IsNullOrEmpty(soleTraderDob))
                 {
                     var person = commonDetails.LegalName.ToUpper();

                    person = $"{person} ({soleTraderDob})";
                     whosInControl.DataRows.Add(new TabularDataRow
                         { Columns = new List<string> { person } });

                }
             }

            model.WhosInControl = whosInControl;


            return model;
        }
    }
}
