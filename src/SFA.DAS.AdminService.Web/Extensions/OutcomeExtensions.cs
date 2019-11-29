using SFA.DAS.AdminService.Web.ViewModels.RoatpAssessor.Gateway;
using SFA.DAS.RoatpAssessor.Configuration;
using SFA.DAS.RoatpAssessor.Domain.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AdminService.Web.Extensions
{
    public static class OutcomeExtensions
    {
        public static Outcome ToOutcome(this OutcomeViewModel model, OutcomeConfig question)
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

        public static string GetCheckValue(this Outcome outcome, string checkName)
        {
            if (outcome?.Checks == null)
                return null;

            return outcome.Checks.FirstOrDefault(c => c.Name == checkName)?.Value;
        }

        public static void SetCheckValue(this Outcome outcome, string checkName, string value)
        {
            if (outcome.Checks == null)
                outcome.Checks = new List<Check>();

            var check = outcome.Checks.SingleOrDefault(c => c.Name == checkName);

            if(check == null)
            {
                check = new Check { Name = checkName };
                outcome.Checks.Add(check);
            }

            check.Value = value;
        }
    }
}
