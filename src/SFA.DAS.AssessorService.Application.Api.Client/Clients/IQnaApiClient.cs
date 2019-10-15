﻿using Microsoft.AspNetCore.Http;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Answer = SFA.DAS.QnA.Api.Types.Page.Answer;
using Page = SFA.DAS.QnA.Api.Types.Page.Page;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IQnaApiClient
    {
        Task<StartApplicationResponse> StartApplications(StartApplicationRequest startAppRequest);
        Task<Dictionary<string, object>> GetApplicationData(Guid applicationId);
        Task UpdateApplicationData(Guid applicationId, ApplicationData applicationData);
        Task<List<Sequence>> GetAllApplicationSequences(Guid applicationId);
        Task<Sequence> GetSequence(Guid applicationId, Guid sequenceId);
        Task<List<Section>> GetSections(Guid applicationId, Guid sequenceId);
        Task<Section> GetSection(Guid applicationId, Guid sectionId);
        Task<Page> GetPage(Guid applicationId, Guid sectionId, string pageId);
        Task<SetPageAnswersResponse> AddPageAnswer(Guid applicationId, Guid sectionId, string pageId, List<Answer> answer);
        Task<AddPageAnswerResponse> AddPageAnswers(Guid applicationId, Guid sectionId, string pageId, List<Answer> answer);
        Task<SetPageAnswersResponse> Upload(Guid applicationId, Guid sectionId, string pageId, IFormFileCollection files);
        Task<HttpResponseMessage> DownloadFile(Guid applicationId, Guid sectionId, string pageId, string questionId, string fileName);
        Task DeleteFile(Guid applicationId, Guid sectionId, string pageId, string questionId, string fileName);
        Task<Page> RemovePageAnswer(Guid applicationId, Guid sectionId, string pageId, Guid answerId);
        Task<Page> UpdateFeedback(Guid applicationId, Guid sectionId, string pageId, QnA.Api.Types.Page.Feedback feedback);
    }
}