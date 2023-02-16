using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;
using SFA.DAS.AssessorService.ApplyTypes.CharityCommission;
using SFA.DAS.AssessorService.ApplyTypes.CompaniesHouse;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.ApplyTypes
{
    public class Application : ApplyTypeBase
    {
        public Domain.Entities.Organisation ApplyingOrganisation { get; set; }
        public Guid ApplyingOrganisationId { get; set; }
        public DateTime WithdrawnAt { get; set; }
        public string WithdrawnBy { get; set; }
        public string ApplicationStatus { get; set; }
        public ApplicationData ApplicationData { get; set; }
    }

    public class ApplicationData
    {
        public string OrganisationReferenceId { get; set; }
        public string OrganisationName { get; set; }
        public string ReferenceNumber { get; set; }
        public string StandardName { get; set; }
        public int? StandardCode { get; set; }
        public string TradingName { get; set; }
        public bool UseTradingName { get; set; }
        public string ContactGivenName { get; set; }

        public CompaniesHouseSummary CompanySummary { get; set; }
        public CharityCommissionSummary CharitySummary { get; set; }
    }

    public class Submission
    {
        public DateTime SubmittedAt { get; set; }
        public Guid SubmittedBy { get; set; }
        public string SubmittedByEmail { get; set; }
    }


    public class DisplayAnswerPage
    {
        public List<IDisplayAnswer> DisplayAnswers { get; set; }
    }

    public class DisplayAnswer : IDisplayAnswer
    {
        public string Label { get; set; }
        string IDisplayAnswer.Answer()
        {
            return Answer;
        }

        public string QuestionId { get; set; }

        public string Answer { private get; set; }
    }

    public class FileUploadDisplayAnswer : IDisplayAnswer
    {
        public string Label { get; set; }
        public Guid ApplicationId { get; set; }
        public int SequenceId { get; set; }
        public int SectionId { get; set; }
        public string PageId { get; set; }
        public string QuestionId { get; set; }
        public string FileName { get; set; }
        public string Answer()
        {
            return "";
        }
    }
    
    public interface IDisplayAnswer
    {
        string Label { get; set; }
        string Answer();
        string QuestionId { get; set; }
    }


    public class Question
    {
        public string QuestionId { get; set; }
        public string Label { get; set; }
        public string Hint { get; set; }
        public Input Input { get; set; }
        public int? Order { get; set; }
        public string PageId { get; set; }
        public string QuestionBodyText { get; set; }
    }
    
    public class Feedback
    {
        public Guid Id { get; set; }
        public string From { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsNew { get; set; }
    }
    
    public class PageOfAnswers
    {
        public Guid Id { get; set; }
        public List<Answer> Answers { get; set; }
    }
    
    public class Next
    {
        public string Action { get; set; }
        public string ReturnId { get; set; }
        public Condition Condition { get; set; }
        public bool ConditionMet { get; set; }
    }
    
    public class Input
    {
        public string Type { get; set; }
        public List<InputOptions> Options { get; set; }
        public List<ValidationDefinition> Validations { get; set; }
    }

    public class InputOptions
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public List<Question> FurtherQuestions { get; set; }
    }

    public class Answer
    {
        public string QuestionId { get; set; }

        [JsonIgnore]
        public string Value
        {
            get { return JsonValue as string; }
            set { JsonValue = value; }
        }

        [JsonPropertyName("Value")]
        public dynamic JsonValue { get; set; }

        public override string ToString()
        {
            if (JsonValue == null)
                return null;

            if (JsonValue is string stringValue)
            {
                return stringValue;
            }

            var jsonValue = new JObject(JsonValue);
            return string.Join(", ", jsonValue.Properties().
                Where(p => !string.IsNullOrEmpty(p.Value.ToString())).
                Select(p => p.Value.ToString()));
        }
    }
    
    public class Condition
    {
        public string QuestionId { get; set; }
        public string MustEqual { get; set; }
    }
    
    public class ValidationDefinition
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ApplyData
    {
        public List<ApplySequence> Sequences { get; set; }
        public Apply Apply { get; set; }
    }

    public class ApplySequence
    {
        public Guid SequenceId { get; set; }
        public List<ApplySection> Sections { get; set; }
        public string Status { get; set; }
        public int SequenceNo { get; set; }
        public bool IsActive { get; set; }
        public bool NotRequired { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string ApprovedBy { get; set; }

    }

    public class ApplySection
    {
        public Guid SectionId { get; set; }
        public int SectionNo { get; set; }
        public string Status { get; set; }
        public DateTime? ReviewStartDate { get; set; }
        public string ReviewedBy { get; set; }
        public DateTime? EvaluatedDate { get; set; }
        public string EvaluatedBy { get; set; }
        public bool NotRequired { get; set; }
        public bool? RequestedFeedbackAnswered { get; set; }
    }

    public class Apply
    {
        public string ReferenceNumber { get; set; }

        public int? StandardCode { get; set; }
        public string StandardReference { get; set; }
        public string StandardName { get; set; }
        public List<string> Versions { get; set; }

        public List<Submission> InitSubmissions { get; private set; } = new List<Submission>();

        public void AddInitSubmission(Submission submission)
        {
            if (InitSubmissions == null)
                ResetInitSubmissions();

            InitSubmissions.Add(submission);
        }

        public void ResetInitSubmissions()
        {
            InitSubmissions = new List<Submission>();
        }

        [JsonIgnore]
        public Submission LatestInitSubmission => InitSubmissions?.OrderByDescending(o => o.SubmittedAt).FirstOrDefault();

        public int InitSubmissionsCount => InitSubmissions?.Count ?? 0;
        public DateTime? LatestInitSubmissionDate => LatestInitSubmission?.SubmittedAt;
        public DateTime? InitSubmissionFeedbackAddedDate { get; set; }
        public DateTime? InitSubmissionClosedDate { get; set; }

        public List<Submission> StandardSubmissions { get; private set; } = new List<Submission>();

        public void AddStandardSubmission(Submission submission)
        {
            if (StandardSubmissions == null)
                ResetStandardSubmissions();

            StandardSubmissions.Add(submission);
        }

        public void ResetStandardSubmissions()
        {
            StandardSubmissions = new List<Submission>();
        }

        [JsonIgnore]
        public Submission LatestStandardSubmission => StandardSubmissions?.OrderByDescending(o => o.SubmittedAt).FirstOrDefault();

        public int StandardSubmissionsCount => StandardSubmissions?.Count ?? 0;
        public DateTime? LatestStandardSubmissionDate => LatestStandardSubmission?.SubmittedAt;
        public DateTime? StandardSubmissionFeedbackAddedDate { get; set; }
        public DateTime? StandardSubmissionClosedDate { get; set; }

        public List<Submission> OrganisationWithdrawalSubmissions { get; private set; } = new List<Submission>();

        public void AddOrganisationWithdrawalSubmission(Submission submission)
        {
            if (OrganisationWithdrawalSubmissions == null)
                ResetOrganisationWithdrawalSubmissions();

            OrganisationWithdrawalSubmissions.Add(submission);
        }

        public void ResetOrganisationWithdrawalSubmissions()
        {
            OrganisationWithdrawalSubmissions = new List<Submission>();
        }

        [JsonIgnore]
        public Submission LatestOrganisationWithdrawalSubmission => OrganisationWithdrawalSubmissions?.OrderByDescending(o => o.SubmittedAt).FirstOrDefault();

        public int OrganisationWithdrawalSubmissionsCount => OrganisationWithdrawalSubmissions?.Count ?? 0;
        public DateTime? LatestOrganisationWithdrawalSubmissionDate => LatestOrganisationWithdrawalSubmission?.SubmittedAt;
        public DateTime? OrganisationWithdrawalSubmissionFeedbackAddedDate { get; set; }
        public DateTime? OrganisationWithdrawalSubmissionClosedDate { get; set; }

        public List<Submission> StandardWithdrawalSubmissions { get; private set; } = new List<Submission>();

        public void AddStandardWithdrawalSubmission(Submission submission)
        {
            if (StandardWithdrawalSubmissions == null)
                ResetStandardWithdrawalSubmissions();

            StandardWithdrawalSubmissions.Add(submission);
        }

        public void ResetStandardWithdrawalSubmissions()
        {
            StandardWithdrawalSubmissions = new List<Submission>();
        }

        [JsonIgnore]
        public Submission LatestStandardWithdrawalSubmission => StandardWithdrawalSubmissions?.OrderByDescending(o => o.SubmittedAt).FirstOrDefault();

        public int StandardWithdrawalSubmissionsCount => StandardWithdrawalSubmissions?.Count ?? 0;
        public DateTime? LatestStandardWithdrawalSubmissionDate => LatestStandardWithdrawalSubmission?.SubmittedAt;
        public DateTime? StandardWithdrawalSubmissionFeedbackAddedDate { get; set; }
        public DateTime? StandardWithdrawalSubmissionClosedDate { get; set; }

        [JsonIgnore]
        public string StandardWithReference => $"{StandardName} ({StandardReference})";
    }

    public class InitSubmission
    {
        public DateTime SubmittedAt { get; set; }
        public Guid SubmittedBy { get; set; }
        public string SubmittedByEmail { get; set; }
    }

    public class StandardSubmission
    {
        public DateTime SubmittedAt { get; set; }
        public Guid SubmittedBy { get; set; }
        public string SubmittedByEmail { get; set; }
    }
}


// 