using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Validators.Roatp.Applications;
using SFA.DAS.AdminService.Web.ViewModels.Apply.Financial;
using SFA.DAS.AssessorService.ApplyTypes.Roatp.Apply;
using System;
using System.Linq;

namespace SFA.DAS.AdminService.Web.Tests.Validators.Roatp.Applications
{
    public class RoatpFinancialApplicationViewModelValidatorTests
    {
        private RoatpFinancialApplicationViewModelValidator _validator = new RoatpFinancialApplicationViewModelValidator();
        private const int _maxWordCount = 500;

        [Test]
        public void Validator_rejects_missing_SelectedGrade()
        {
            var _viewModel = new RoatpFinancialApplicationViewModel
            {
                FinancialReviewDetails = new FinancialReviewDetails
                {
                    SelectedGrade = null
                }
            };

            var validationResponse = _validator.Validate(_viewModel);

            var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "FinancialReviewDetails.SelectedGrade");
            error.Should().NotBeNull();
        }

        [Test]
        public void Validator_rejects_missing_InadequateComments_when_graded_Inadequate()
        {
            var _viewModel = new RoatpFinancialApplicationViewModel
            {
                InadequateComments = null,
                FinancialReviewDetails = new FinancialReviewDetails
                {
                    SelectedGrade = FinancialApplicationSelectedGrade.Inadequate,
                }
            };

            var validationResponse = _validator.Validate(_viewModel);
            
            var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "InadequateComments");
            error.Should().NotBeNull();
        }

        [Test]
        public void Validator_rejects_InadequateComments_above_maxwordcount_when_graded_Inadequate()
        {
            var _viewModel = new RoatpFinancialApplicationViewModel
            {
                InadequateComments = string.Join(" ", Enumerable.Repeat("a", _maxWordCount + 1)),
                FinancialReviewDetails = new FinancialReviewDetails
                {
                    SelectedGrade = FinancialApplicationSelectedGrade.Inadequate,
                }
            };

            var validationResponse = _validator.Validate(_viewModel);

            var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "InadequateComments");
            error.Should().NotBeNull();
        }

        [Test]
        public void Validator_rejects_missing_ClarificationComments_when_graded_Clarification()
        {
            var _viewModel = new RoatpFinancialApplicationViewModel
            {
                ClarificationComments = null,
                FinancialReviewDetails = new FinancialReviewDetails
                {
                    SelectedGrade = FinancialApplicationSelectedGrade.Clarification,
                }
            };

            var validationResponse = _validator.Validate(_viewModel);

            var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "ClarificationComments");
            error.Should().NotBeNull();
        }

        [Test]
        public void Validator_rejects_ClarificationComments_above_maxwordcount_when_graded_Clarification()
        {
            var _viewModel = new RoatpFinancialApplicationViewModel
            {
                ClarificationComments = string.Join(" ", Enumerable.Repeat("a", _maxWordCount + 1)),
                FinancialReviewDetails = new FinancialReviewDetails
                {
                    SelectedGrade = FinancialApplicationSelectedGrade.Clarification,
                }
            };

            var validationResponse = _validator.Validate(_viewModel);

            var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "ClarificationComments");
            error.Should().NotBeNull();
        }

        [Test]
        public void Validator_rejects_missing_FinancialDueDate_when_graded_Outstanding()
        {
            var _viewModel = new RoatpFinancialApplicationViewModel
            {
                OutstandingFinancialDueDate = new FinancialDueDate { },
                FinancialReviewDetails = new FinancialReviewDetails
                {
                    SelectedGrade = FinancialApplicationSelectedGrade.Outstanding,
                }
            };

            var validationResponse = _validator.Validate(_viewModel);

            var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "OutstandingFinancialDueDate");
            error.Should().NotBeNull();
        }

        [Test]
        public void Validator_rejects_unparseable_FinancialDueDate_when_graded_Outstanding()
        {
            var _viewModel = new RoatpFinancialApplicationViewModel
            {
                OutstandingFinancialDueDate = new FinancialDueDate
                {
                    Day = "test",
                    Month = "test",
                    Year = "test"
                },
                FinancialReviewDetails = new FinancialReviewDetails
                {
                    SelectedGrade = FinancialApplicationSelectedGrade.Outstanding,
                }
            };

            var validationResponse = _validator.Validate(_viewModel);

            var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "OutstandingFinancialDueDate");
            error.Should().NotBeNull();
        }

        [Test]
        public void Validator_rejects_invalid_FinancialDueDate_when_graded_Outstanding()
        {
            var _viewModel = new RoatpFinancialApplicationViewModel
            {
                OutstandingFinancialDueDate = new FinancialDueDate
                {
                    Day = "999",
                    Month = DateTime.Today.Month.ToString(),
                    Year = DateTime.Today.Year.ToString()
                },
                FinancialReviewDetails = new FinancialReviewDetails
                {
                    SelectedGrade = FinancialApplicationSelectedGrade.Outstanding,
                }
            };

            var validationResponse = _validator.Validate(_viewModel);

            var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "OutstandingFinancialDueDate");
            error.Should().NotBeNull();
        }

        [Test]
        public void Validator_rejects_FinancialDueDate_before_today_when_graded_Outstanding()
        {
            var _viewModel = new RoatpFinancialApplicationViewModel
            {
                OutstandingFinancialDueDate = new FinancialDueDate
                {
                    Day = DateTime.Today.AddDays(-2).Day.ToString(),
                    Month = DateTime.Today.Month.ToString(),
                    Year = DateTime.Today.Year.ToString()
                },
                FinancialReviewDetails = new FinancialReviewDetails
                {
                    SelectedGrade = FinancialApplicationSelectedGrade.Outstanding,
                }
            };

            var validationResponse = _validator.Validate(_viewModel);

            var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "OutstandingFinancialDueDate");
            error.Should().NotBeNull();
        }

        [Test]
        public void Validator_rejects_missing_FinancialDueDate_when_graded_Good()
        {
            var _viewModel = new RoatpFinancialApplicationViewModel
            {
                GoodFinancialDueDate = new FinancialDueDate { },
                FinancialReviewDetails = new FinancialReviewDetails
                {
                    SelectedGrade = FinancialApplicationSelectedGrade.Good,
                }
            };

            var validationResponse = _validator.Validate(_viewModel);

            var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "GoodFinancialDueDate");
            error.Should().NotBeNull();
        }

        [Test]
        public void Validator_rejects_unparseable_FinancialDueDate_when_graded_Good()
        {
            var _viewModel = new RoatpFinancialApplicationViewModel
            {
                GoodFinancialDueDate = new FinancialDueDate
                {
                    Day = "test",
                    Month = "test",
                    Year = "test"
                },
                FinancialReviewDetails = new FinancialReviewDetails
                {
                    SelectedGrade = FinancialApplicationSelectedGrade.Good,
                }
            };

            var validationResponse = _validator.Validate(_viewModel);

            var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "GoodFinancialDueDate");
            error.Should().NotBeNull();
        }

        [Test]
        public void Validator_rejects_invalid_FinancialDueDate_when_graded_Good()
        {
            var _viewModel = new RoatpFinancialApplicationViewModel
            {
                GoodFinancialDueDate = new FinancialDueDate
                {
                    Day = "999",
                    Month = DateTime.Today.Month.ToString(),
                    Year = DateTime.Today.Year.ToString()
                },
                FinancialReviewDetails = new FinancialReviewDetails
                {
                    SelectedGrade = FinancialApplicationSelectedGrade.Good,
                }
            };

            var validationResponse = _validator.Validate(_viewModel);

            var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "GoodFinancialDueDate");
            error.Should().NotBeNull();
        }

        [Test]
        public void Validator_rejects_FinancialDueDate_before_today_when_graded_Good()
        {
            var _viewModel = new RoatpFinancialApplicationViewModel
            {
                GoodFinancialDueDate = new FinancialDueDate
                {
                    Day = DateTime.Today.AddDays(-2).Day.ToString(),
                    Month = DateTime.Today.Month.ToString(),
                    Year = DateTime.Today.Year.ToString()
                },
                FinancialReviewDetails = new FinancialReviewDetails
                {
                    SelectedGrade = FinancialApplicationSelectedGrade.Good,
                }
            };

            var validationResponse = _validator.Validate(_viewModel);

            var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "GoodFinancialDueDate");
            error.Should().NotBeNull();
        }


        [Test]
        public void Validator_rejects_missing_FinancialDueDate_when_graded_Satisfactory()
        {
            var _viewModel = new RoatpFinancialApplicationViewModel
            {
                SatisfactoryFinancialDueDate = new FinancialDueDate { },
                FinancialReviewDetails = new FinancialReviewDetails
                {
                    SelectedGrade = FinancialApplicationSelectedGrade.Satisfactory,
                }
            };

            var validationResponse = _validator.Validate(_viewModel);

            var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "SatisfactoryFinancialDueDate");
            error.Should().NotBeNull();
        }

        [Test]
        public void Validator_rejects_unparseable_FinancialDueDate_when_graded_Satisfactory()
        {
            var _viewModel = new RoatpFinancialApplicationViewModel
            {
                SatisfactoryFinancialDueDate = new FinancialDueDate
                {
                    Day = "test",
                    Month = "test",
                    Year = "test"
                },
                FinancialReviewDetails = new FinancialReviewDetails
                {
                    SelectedGrade = FinancialApplicationSelectedGrade.Satisfactory,
                }
            };

            var validationResponse = _validator.Validate(_viewModel);

            var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "SatisfactoryFinancialDueDate");
            error.Should().NotBeNull();
        }

        [Test]
        public void Validator_rejects_invalid_FinancialDueDate_when_graded_Satisfactory()
        {
            var _viewModel = new RoatpFinancialApplicationViewModel
            {
                SatisfactoryFinancialDueDate = new FinancialDueDate
                {
                    Day = "999",
                    Month = DateTime.Today.Month.ToString(),
                    Year = DateTime.Today.Year.ToString()
                },
                FinancialReviewDetails = new FinancialReviewDetails
                {
                    SelectedGrade = FinancialApplicationSelectedGrade.Satisfactory,
                }
            };

            var validationResponse = _validator.Validate(_viewModel);

            var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "SatisfactoryFinancialDueDate");
            error.Should().NotBeNull();
        }

        [Test]
        public void Validator_rejects_FinancialDueDate_before_today_when_graded_Satisfactory()
        {
            var _viewModel = new RoatpFinancialApplicationViewModel
            {
                SatisfactoryFinancialDueDate = new FinancialDueDate
                {
                    Day = DateTime.Today.AddDays(-2).Day.ToString(),
                    Month = DateTime.Today.Month.ToString(),
                    Year = DateTime.Today.Year.ToString()
                },
                FinancialReviewDetails = new FinancialReviewDetails
                {
                    SelectedGrade = FinancialApplicationSelectedGrade.Satisfactory,
                }
            };

            var validationResponse = _validator.Validate(_viewModel);

            var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "SatisfactoryFinancialDueDate");
            error.Should().NotBeNull();
        }
    }
}
