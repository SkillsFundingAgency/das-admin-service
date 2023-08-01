using System.Linq;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Validators.Roatp.Applications;

namespace SFA.DAS.AdminService.Web.Tests.Validators.Roatp.Applications
{
    [TestFixture]
    public class RoatpSearchTermValidatorTests
    {
        private const int MinimumLength = 3;
        private readonly RoatpSearchTermValidator _validator = new RoatpSearchTermValidator();

        [Test]
        public void When_searchTerm_is_not_provided_then_an_error_is_returned()
        {
            var response = _validator.Validate(null);

            Assert.That(response.IsValid, Is.False);
            Assert.That(response.Errors.First().ErrorMessage, Is.EqualTo($"Enter an organisation name or UKPRN"));
            Assert.That(response.Errors.First().Field, Is.EqualTo("SearchTerm"));
        }

        [Test]
        public void When_searchTerm_is_empty_string_then_an_error_is_returned()
        {
            var searchTerm = string.Empty;
            var response = _validator.Validate(searchTerm);

            Assert.That(response.IsValid, Is.False);
            Assert.That(response.Errors.First().ErrorMessage, Is.EqualTo($"Enter an organisation name or UKPRN"));
            Assert.That(response.Errors.First().Field, Is.EqualTo("SearchTerm"));
        }

        [Test]
        public void When_searchTerm_is_whitespace_only_then_an_error_is_returned()
        {
            var searchTerm = string.Concat(Enumerable.Repeat(" ", MinimumLength)); ;
            var response = _validator.Validate(searchTerm);

            Assert.That(response.IsValid, Is.False);
            Assert.That(response.Errors.First().ErrorMessage, Is.EqualTo($"Enter an organisation name or UKPRN"));
            Assert.That(response.Errors.First().Field, Is.EqualTo("SearchTerm"));
        }

        [Test]
        public void When_searchTerm_is_less_than_minimum_length_then_an_error_is_returned()
        {
            var searchTerm = string.Concat(Enumerable.Repeat("a", MinimumLength - 1));
            var response = _validator.Validate(searchTerm);

            Assert.That(response.IsValid, Is.False);
            Assert.That(response.Errors.First().ErrorMessage, Is.EqualTo($"Enter a UKPRN or an organisation name using {MinimumLength} or more characters"));
            Assert.That(response.Errors.First().Field, Is.EqualTo("SearchTerm"));
        }

        [Test]
        public void When_searchTerm_is_minimum_length_then_validation_passes()
        {
            var searchTerm = string.Concat(Enumerable.Repeat("a", MinimumLength));
            var response = _validator.Validate(searchTerm);

            Assert.IsTrue(response.IsValid);
        }

        [Test]
        public void When_searchTerm_is_greater_than_minimum_length_then_validation_passes()
        {
            var searchTerm = string.Concat(Enumerable.Repeat("a", MinimumLength + 1));
            var response = _validator.Validate(searchTerm);

            Assert.IsTrue(response.IsValid);
        }
    }
}
