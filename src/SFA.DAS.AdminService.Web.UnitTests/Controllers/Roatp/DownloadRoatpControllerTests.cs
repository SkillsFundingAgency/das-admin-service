using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types;
using SFA.DAS.AdminService.Web.AutoMapperProfiles;
using SFA.DAS.AdminService.Web.Controllers.Roatp;
using SFA.DAS.AdminService.Web.Helpers;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients;
using SFA.DAS.AdminService.Web.Models.Roatp;
using SFA.DAS.AdminService.Web.Services;
using SFA.DAS.AdminService.Web.ViewModels.Roatp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Tests.Controllers.Roatp
{
    [TestFixture]
    public class DownloadRoatpControllerTests
    {
        private DownloadRoatpController _controller;
        private Mock<IRoatpApplicationApiClient> _applicationApplyApiClient;
        private Mock<ICsvExportService> _csvExportService;
        private IMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            _applicationApplyApiClient = new Mock<IRoatpApplicationApiClient>();
            _csvExportService = new Mock<ICsvExportService>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile<AutoMapperMappings>();
            });
            IMapper mapper = mappingConfig.CreateMapper();
            _mapper = mapper;

            _controller = new DownloadRoatpController(Mock.Of<IRoatpApiClient>(), 
                Mock.Of<IDataTableHelper>(), 
                Mock.Of<ILogger<DownloadRoatpController>>(),
                _csvExportService.Object, 
                _applicationApplyApiClient.Object,
                _mapper
            );
        }

        [Test]
        public async Task ApplicationDownloadCsv_downloads_file()
        {
            var apiResponse = new List<RoatpApplicationOversightDownloadItem>();

            _applicationApplyApiClient.Setup(
                x => x.GetApplicationOversightDetailsForDownload(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(apiResponse);

            var expectedFileContents = Encoding.ASCII.GetBytes("THIS IS A TEST");

            _csvExportService.Setup(x =>
                    x.WriteCsvToByteArray<RoatpOversightOutcomeExportViewModel, RoatpOversightOutcomeExportCsvMap>(new List<RoatpOversightOutcomeExportViewModel>()))
                .Returns(expectedFileContents);

            var result = await _controller.ApplicationDownloadCsv(new ApplicationDownloadViewModel
            {
                FromDate = DateTime.UtcNow,
                ToDate = DateTime.UtcNow
            }) as FileContentResult;

            Assert.That(result.FileContents, Is.EqualTo(expectedFileContents));
        }
    }
}
