using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AdminService.Web.Validators.Roatp.Applications;
using SFA.DAS.AdminService.Web.ViewModels.Apply.Financial;
using SFA.DAS.AssessorService.ApplyTypes.Roatp.Apply;
using System;
using System.Linq;
using SFA.DAS.AdminService.Web.ViewModels.Roatp.Financial;

namespace SFA.DAS.AdminService.Web.Tests.Validators.Roatp.Applications
{
//MFCMFC
    //public class RoatpFinancialClarificationViewModelValidatorTests
    //{
    //    private RoatpFinancialClarificationViewModelValidator _validator = new RoatpFinancialClarificationViewModelValidator();
    //    private const int _maxWordCount = 500;

    //    [Test]
    //    public void Validator_rejects_missing_SelectedGrade()
    //    {
    //        var _viewModel = new RoatpFinancialClarificationViewModel
    //        {
    //            FinancialReviewDetails = new FinancialReviewDetails
    //            {
    //                SelectedGrade = null
    //            }
    //        };

    //        var validationResponse = _validator.Validate(_viewModel);

    //        var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "FinancialReviewDetails.SelectedGrade");
    //        error.Should().NotBeNull();
    //    }

    //    [Test]
    //    public void Validator_rejects_missing_InadequateComments_when_graded_Inadequate()
    //    {
    //        var _viewModel = new RoatpFinancialClarificationViewModel
    //        {
    //            InadequateComments = null,
    //            FinancialReviewDetails = new FinancialReviewDetails
    //            {
    //                SelectedGrade = FinancialApplicationSelectedGrade.Inadequate,
    //            }
    //        };

    //        var validationResponse = _validator.Validate(_viewModel);
            
    //        var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "InadequateComments");
    //        error.Should().NotBeNull();
    //    }

    //    [Test]
    //    public void Validator_rejects_InadequateComments_above_maxwordcount_when_graded_Inadequate()
    //    {
    //        var _viewModel = new RoatpFinancialClarificationViewModel
    //        {
    //            InadequateComments = string.Join(" ", Enumerable.Repeat("a", _maxWordCount + 1)),
    //            FinancialReviewDetails = new FinancialReviewDetails
    //            {
    //                SelectedGrade = FinancialApplicationSelectedGrade.Inadequate,
    //            }
    //        };

    //        var validationResponse = _validator.Validate(_viewModel);

    //        var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "InadequateComments");
    //        error.Should().NotBeNull();
    //    }

    //    [Test]
    //    public void Validator_rejects_missing_FinancialDueDate_when_graded_Outstanding()
    //    {
    //        var _viewModel = new RoatpFinancialClarificationViewModel
    //        {
    //            OutstandingFinancialDueDate = new FinancialDueDate { },
    //            FinancialReviewDetails = new FinancialReviewDetails
    //            {
    //                SelectedGrade = FinancialApplicationSelectedGrade.Outstanding,
    //            }
    //        };

    //        var validationResponse = _validator.Validate(_viewModel);

    //        var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "OutstandingFinancialDueDate");
    //        error.Should().NotBeNull();
    //    }

    //    [Test]
    //    public void Validator_rejects_unparseable_FinancialDueDate_when_graded_Outstanding()
    //    {
    //        var _viewModel = new RoatpFinancialClarificationViewModel
    //        {
    //            OutstandingFinancialDueDate = new FinancialDueDate
    //            {
    //                Day = "test",
    //                Month = "test2",
    //                Year = "test3"
    //            },
    //            FinancialReviewDetails = new FinancialReviewDetails
    //            {
    //                SelectedGrade = FinancialApplicationSelectedGrade.Outstanding,
    //            }
    //        };

    //        var validationResponse = _validator.Validate(_viewModel);

    //        var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "OutstandingFinancialDueDate");
    //        error.Should().NotBeNull();
    //    }

    //    [Test]
    //    public void Validator_rejects_invalid_FinancialDueDate_when_graded_Outstanding()
    //    {
    //        var _viewModel = new RoatpFinancialClarificationViewModel
    //        {
    //            OutstandingFinancialDueDate = new FinancialDueDate
    //            {
    //                Day = "999",
    //                Month = DateTime.Today.Month.ToString(),
    //                Year = DateTime.Today.Year.ToString()
    //            },
    //            FinancialReviewDetails = new FinancialReviewDetails
    //            {
    //                SelectedGrade = FinancialApplicationSelectedGrade.Outstanding,
    //            }
    //        };

    //        var validationResponse = _validator.Validate(_viewModel);

    //        var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "OutstandingFinancialDueDate");
    //        error.Should().NotBeNull();
    //    }

    //    [Test]
    //    public void Validator_rejects_FinancialDueDate_before_today_when_graded_Outstanding()
    //    {
    //        var _viewModel = new RoatpFinancialClarificationViewModel
    //        {
    //            OutstandingFinancialDueDate = new FinancialDueDate
    //            {
    //                Day = DateTime.Today.AddDays(-1).Day.ToString(),
    //                Month = DateTime.Today.Month.ToString(),
    //                Year = DateTime.Today.Year.ToString()
    //            },
    //            FinancialReviewDetails = new FinancialReviewDetails
    //            {
    //                SelectedGrade = FinancialApplicationSelectedGrade.Outstanding,
    //            }
    //        };

    //        var validationResponse = _validator.Validate(_viewModel);

    //        var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "OutstandingFinancialDueDate");
    //        error.Should().NotBeNull();
    //    }

    //    [Test]
    //    public void Validator_rejects_missing_FinancialDueDate_when_graded_Good()
    //    {
    //        var _viewModel = new RoatpFinancialClarificationViewModel
    //        {
    //            GoodFinancialDueDate = new FinancialDueDate { },
    //            FinancialReviewDetails = new FinancialReviewDetails
    //            {
    //                SelectedGrade = FinancialApplicationSelectedGrade.Good,
    //            }
    //        };

    //        var validationResponse = _validator.Validate(_viewModel);

    //        var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "GoodFinancialDueDate");
    //        error.Should().NotBeNull();
    //    }

    //    [Test]
    //    public void Validator_rejects_unparseable_FinancialDueDate_when_graded_Good()
    //    {
    //        var _viewModel = new RoatpFinancialClarificationViewModel
    //        {
    //            GoodFinancialDueDate = new FinancialDueDate
    //            {
    //                Day = "test",
    //                Month = "test",
    //                Year = "test"
    //            },
    //            FinancialReviewDetails = new FinancialReviewDetails
    //            {
    //                SelectedGrade = FinancialApplicationSelectedGrade.Good,
    //            }
    //        };

    //        var validationResponse = _validator.Validate(_viewModel);

    //        var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "GoodFinancialDueDate");
    //        error.Should().NotBeNull();
    //    }

    //    [Test]
    //    public void Validator_rejects_invalid_FinancialDueDate_when_graded_Good()
    //    {
    //        var _viewModel = new RoatpFinancialClarificationViewModel
    //        {
    //            GoodFinancialDueDate = new FinancialDueDate
    //            {
    //                Day = "999",
    //                Month = DateTime.Today.Month.ToString(),
    //                Year = DateTime.Today.Year.ToString()
    //            },
    //            FinancialReviewDetails = new FinancialReviewDetails
    //            {
    //                SelectedGrade = FinancialApplicationSelectedGrade.Good,
    //            }
    //        };

    //        var validationResponse = _validator.Validate(_viewModel);

    //        var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "GoodFinancialDueDate");
    //        error.Should().NotBeNull();
    //    }

    //    [Test]
    //    public void Validator_rejects_FinancialDueDate_before_today_when_graded_Good()
    //    {
    //        var _viewModel = new RoatpFinancialClarificationViewModel
    //        {
    //            GoodFinancialDueDate = new FinancialDueDate
    //            {
    //                Day = DateTime.Today.AddDays(-1).Day.ToString(),
    //                Month = DateTime.Today.Month.ToString(),
    //                Year = DateTime.Today.Year.ToString()
    //            },
    //            FinancialReviewDetails = new FinancialReviewDetails
    //            {
    //                SelectedGrade = FinancialApplicationSelectedGrade.Good,
    //            }
    //        };

    //        var validationResponse = _validator.Validate(_viewModel);

    //        var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "GoodFinancialDueDate");
    //        error.Should().NotBeNull();
    //    }


    //    [Test]
    //    public void Validator_rejects_missing_FinancialDueDate_when_graded_Satisfactory()
    //    {
    //        var _viewModel = new RoatpFinancialClarificationViewModel
    //        {
    //            SatisfactoryFinancialDueDate = new FinancialDueDate { },
    //            FinancialReviewDetails = new FinancialReviewDetails
    //            {
    //                SelectedGrade = FinancialApplicationSelectedGrade.Satisfactory,
    //            }
    //        };

    //        var validationResponse = _validator.Validate(_viewModel);

    //        var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "SatisfactoryFinancialDueDate");
    //        error.Should().NotBeNull();
    //    }

    //    [Test]
    //    public void Validator_rejects_unparseable_FinancialDueDate_when_graded_Satisfactory()
    //    {
    //        var _viewModel = new RoatpFinancialClarificationViewModel
    //        {
    //            SatisfactoryFinancialDueDate = new FinancialDueDate
    //            {
    //                Day = "test",
    //                Month = "test",
    //                Year = "test"
    //            },
    //            FinancialReviewDetails = new FinancialReviewDetails
    //            {
    //                SelectedGrade = FinancialApplicationSelectedGrade.Satisfactory,
    //            }
    //        };

    //        var validationResponse = _validator.Validate(_viewModel);

    //        var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "SatisfactoryFinancialDueDate");
    //        error.Should().NotBeNull();
    //    }

    //    [Test]
    //    public void Validator_rejects_invalid_FinancialDueDate_when_graded_Satisfactory()
    //    {
    //        var _viewModel = new RoatpFinancialClarificationViewModel
    //        {
    //            SatisfactoryFinancialDueDate = new FinancialDueDate
    //            {
    //                Day = "999",
    //                Month = DateTime.Today.Month.ToString(),
    //                Year = DateTime.Today.Year.ToString()
    //            },
    //            FinancialReviewDetails = new FinancialReviewDetails
    //            {
    //                SelectedGrade = FinancialApplicationSelectedGrade.Satisfactory,
    //            }
    //        };

    //        var validationResponse = _validator.Validate(_viewModel);

    //        var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "SatisfactoryFinancialDueDate");
    //        error.Should().NotBeNull();
    //    }

    //    [Test]
    //    public void Validator_rejects_FinancialDueDate_before_today_when_graded_Satisfactory()
    //    {
    //        var _viewModel = new RoatpFinancialClarificationViewModel
    //        {
    //            SatisfactoryFinancialDueDate = new FinancialDueDate
    //            {
    //                Day = DateTime.Today.AddDays(-1).Day.ToString(),
    //                Month = DateTime.Today.Month.ToString(),
    //                Year = DateTime.Today.Year.ToString()
    //            },
    //            FinancialReviewDetails = new FinancialReviewDetails
    //            {
    //                SelectedGrade = FinancialApplicationSelectedGrade.Satisfactory,
    //            }
    //        };

    //        var validationResponse = _validator.Validate(_viewModel);

    //        var error = validationResponse.Errors.FirstOrDefault(x => x.PropertyName == "SatisfactoryFinancialDueDate");
    //        error.Should().NotBeNull();
    //    }
    //}
}
