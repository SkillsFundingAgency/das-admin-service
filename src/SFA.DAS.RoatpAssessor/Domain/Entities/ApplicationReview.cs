using System;
using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Domain.Entities
{
    public class ApplicationReview
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public ApplicationReviewStatus GatewayReviewStatus { get; set; }
        public ApplicationReviewStatus PmoReviewStatus { get; set; }
        public ApplicationReviewStatus AssessorReview1Status { get; set; }
        public ApplicationReviewStatus AssessorReview2Status { get; set; }
        public ApplicationReviewStatus AssessorModerationStatus { get; set; }
        public PageComments AssessorReview1Comments { get; set; }
        public PageComments AssessorReview2Comments { get; set; }


        //todo these will need to be populated by joining on the Application table
        public string ApplicationRef => Id.ToString(); //todo we need to create this at time of submit.
        public DateTime ApplicationSubmittedAt => DateTime.Parse("2018-01-01"); //todo we need to create this at time of submit.


        public bool GatewayReviewCanBeStarted => GatewayReviewStatus == ApplicationReviewStatus.Pending;

        public bool GatewayReviewIsInProgress => GatewayReviewStatus == ApplicationReviewStatus.InProgress;

        public bool GatewayReviewIsCompleted => GatewayReviewStatus == ApplicationReviewStatus.Passed || GatewayReviewStatus == ApplicationReviewStatus.Failed;

        public bool CanStartPmoReview => GatewayReviewStatus == ApplicationReviewStatus.Passed && PmoReviewStatus == ApplicationReviewStatus.Pending;
        
        public bool CanUpdatePmoReview => GatewayReviewStatus == ApplicationReviewStatus.Passed && PmoReviewStatus == ApplicationReviewStatus.InProgress;

        public bool CanStartAssessor1Review => GatewayReviewStatus == ApplicationReviewStatus.Passed && AssessorReview1Status == ApplicationReviewStatus.Pending;

        public bool CanUpdateAssessor1Review => GatewayReviewStatus == ApplicationReviewStatus.Passed && AssessorReview1Status == ApplicationReviewStatus.InProgress;

        public bool CanStartAssessor2Review => GatewayReviewStatus == ApplicationReviewStatus.Passed && AssessorReview2Status == ApplicationReviewStatus.Pending;

        public bool CanUpdateAssessor2Review => GatewayReviewStatus == ApplicationReviewStatus.Passed && AssessorReview2Status == ApplicationReviewStatus.InProgress;

        public bool CanStartAssessorModeration => GatewayReviewStatus == ApplicationReviewStatus.Passed &&
            (AssessorReview1Status == ApplicationReviewStatus.Passed || AssessorReview1Status == ApplicationReviewStatus.Failed) &&
            (AssessorReview2Status == ApplicationReviewStatus.Passed || AssessorReview2Status == ApplicationReviewStatus.Failed) &&
            AssessorModerationStatus == ApplicationReviewStatus.Pending;

        public bool CanUpdateAssessorModeration => GatewayReviewStatus == ApplicationReviewStatus.Passed &&
            (AssessorReview1Status == ApplicationReviewStatus.Passed || AssessorReview1Status == ApplicationReviewStatus.Failed) &&
            (AssessorReview2Status == ApplicationReviewStatus.Passed || AssessorReview2Status == ApplicationReviewStatus.Failed) &&
            AssessorModerationStatus == ApplicationReviewStatus.InProgress;
    }

    public enum ApplicationReviewStatus
    {
        Pending,
        InProgress,
        Feedback,
        Passed,
        Failed
    }

    public class PageComment
    {
        public Guid SectionId { get; set; }
        public string PageId { get; set; }
        public string Comment { get; set; }
    }

    public class PageComments
    {
        public List<PageComment> Comments { get; set; }
    }

    public enum AssessorReviewNo
    {
        Review1 = 1,
        Review2 = 2
    }
}
