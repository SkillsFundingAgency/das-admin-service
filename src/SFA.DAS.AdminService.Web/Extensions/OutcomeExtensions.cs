using SFA.DAS.AdminService.Web.ViewModels.RoatpAssessor.Gateway;
using SFA.DAS.RoatpAssessor.Configuration;
using SFA.DAS.RoatpAssessor.Domain.DTOs;

namespace SFA.DAS.AdminService.Web.Extensions
{
    public static class OutcomeExtensions
    {
        public static Outcome ToOutcome(this OutcomeViewModel model, QuestionConfig question)
        {
            var outcome = new Outcome
            {
                SectionId = question.SectionId,
                PageId = question.PageId,
                QuestionId = question.QuestionId,
                Result = model.Outcome
            };

            switch(model.Outcome)
            {
                case OutcomeResult.Pass:
                    outcome.Message = model.PassComment;
                    break;
                case OutcomeResult.Reject:
                    outcome.Message = model.RejectComment;
                    break;
                case OutcomeResult.InProgress:
                    outcome.Message = model.InProgressComment;
                    break;
            }
            
            return outcome;
        }

        public static OutcomeViewModel ToOutcomeViewModel(this Outcome outcome)
        {
            var vm = new OutcomeViewModel();

            if (outcome == null)
                return vm;

            vm.Outcome = outcome.Result;

            switch(outcome.Result)
            {
                case OutcomeResult.Pass:
                    vm.PassComment = outcome.Message;
                    break;
                case OutcomeResult.Reject:
                    vm.RejectComment = outcome.Message;
                    break;
                case OutcomeResult.InProgress:
                    vm.InProgressComment = outcome.Message;
                    break;
            }

            return vm;
        }
    }
}
