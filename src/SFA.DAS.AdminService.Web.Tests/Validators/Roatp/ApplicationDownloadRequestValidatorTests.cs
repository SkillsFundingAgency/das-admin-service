using System;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Validators.Roatp;
using SFA.DAS.AdminService.Web.ViewModels.Roatp;

namespace SFA.DAS.AdminService.Web.Tests.Validators.Roatp
{
    [TestFixture]
    public class ApplicationDownloadRequestValidatorTests
    {
        private ApplicationDownloadRequestValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new ApplicationDownloadRequestValidator();
        }

        [Test]
        public void Validator_accepts_valid_data()
        {
            var model = new ApplicationDownloadViewModel
            {
                FromDate = DateTime.UtcNow.AddMonths(-3),
                ToDate = DateTime.UtcNow
            };

            var result = _validator.Validate(model);
            Assert.IsTrue(result.IsValid);
        }

        [Test]
        public void Validator_rejects_missing_FromDate()
        {
            var model = new ApplicationDownloadViewModel
            {
                FromDate = null,
                ToDate = DateTime.UtcNow
            };

            var result = _validator.Validate(model);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.First().PropertyName == nameof(model.FromDate));
        }

        [Test]
        public void Validator_rejects_missing_ToDate()
        {
            var model = new ApplicationDownloadViewModel
            {
                FromDate = DateTime.UtcNow,
                ToDate = null
            };

            var result = _validator.Validate(model);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.First().PropertyName == nameof(model.ToDate));
        }

        [Test]
        public void Validator_rejects_missing_FromDate_and_ToDate()
        {
            var model = new ApplicationDownloadViewModel
            {
                FromDate = null,
                ToDate = null
            };

            var result = _validator.Validate(model);

            Assert.IsFalse(result.IsValid);
            Assert.IsNotNull(result.Errors.FirstOrDefault(x => x.PropertyName == nameof(model.FromDate)));
            Assert.IsNotNull(result.Errors.FirstOrDefault(x => x.PropertyName == nameof(model.ToDate)));
        }

        [Test]
        public void Validator_rejects_ToDate_before_FromDate()
        {
            var model = new ApplicationDownloadViewModel
            {
                FromDate = DateTime.UtcNow,
                ToDate = DateTime.UtcNow.AddDays(-1)
            };

            var result = _validator.Validate(model);

            Assert.IsFalse(result.IsValid);
            Assert.IsNotNull(result.Errors.FirstOrDefault(x => x.PropertyName == nameof(model.ToDate)));
        }
    }
}
