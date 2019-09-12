using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.ApplyTypes
{
    public class Application : ApplyTypeBase
    {
        public Organisation ApplyingOrganisation { get; set; }
        public Guid ApplyingOrganisationId { get; set; }
        public DateTime WithdrawnAt { get; set; }
        public string WithdrawnBy { get; set; }
        public string ApplicationStatus { get; set; }
        public ApplicationData ApplicationData { get; set; }
    }

    public class ApplicationData
    {
        public string ReferenceNumber { get; set; }
        public int StandardCode { get; set; }
        public string StandardReference { get; set; }
        public string StandardName { get; set; }
        public List<Submission> InitSubmissions { get; set; }
        public int InitSubmissionsCount { get; set; }
        public DateTime? LatestInitSubmissionDate { get; set; }
        public DateTime? InitSubmissionFeedbackAddedDate { get; set; }
        public DateTime? InitSubmissionClosedDate { get; set; }
        public List<Submission> StandardSubmissions { get; set; }
        public int StandardSubmissionsCount { get; set; }
        public DateTime? LatestStandardSubmissionDate { get; set; }
        public DateTime? StandardSubmissionFeedbackAddedDate { get; set; }
        public DateTime? StandardSubmissionClosedDate { get; set; }
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

        [JsonProperty(PropertyName = "Value")]
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

    //New from assessor

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
        public bool? RequestedFeedbackAnswered { get; set; }
    }

    public class Apply
    {
        public string ReferenceNumber { get; set; }
        public int StandardCode { get; set; }
        public string StandardReference { get; set; }
        public string StandardName { get; set; }
        public List<InitSubmission> InitSubmissions { get; set; }
        public List<StandardSubmission> StandardSubmissions { get; set; }
        public int InitSubmissionCount { get; set; }
        public DateTime? LatestInitSubmissionDate { get; set; }
        public DateTime? InitSubmissionFeedbackAddedDate { get; set; }
        public DateTime? InitSubmissionClosedDate { get; set; }
        public int StandardSubmissionsCount { get; set; }
        public DateTime? LatestStandardSubmissionDate { get; set; }
        public DateTime? StandardSubmissionFeedbackAddedDate { get; set; }
        public DateTime? StandardSubmissionClosedDate { get; set; }
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