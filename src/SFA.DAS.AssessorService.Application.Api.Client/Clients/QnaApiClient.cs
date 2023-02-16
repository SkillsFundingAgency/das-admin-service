using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.QnA.Api.Types;
using SFA.DAS.QnA.Api.Types.Page;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Answer = SFA.DAS.QnA.Api.Types.Page.Answer;
using Page = SFA.DAS.QnA.Api.Types.Page.Page;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class QnaApiClient : ApiClientBase, IQnaApiClient
    {
        private readonly ILogger<QnaApiClient> _logger;


        public QnaApiClient(string baseUri, ITokenService qnaTokenService, ILogger<QnaApiClient> logger) : base(baseUri, qnaTokenService, logger)
        {
            _logger = logger;
        }

        public async Task<StartApplicationResponse> StartApplications(StartApplicationRequest startAppRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/applications/start"))
            {
               return await PostPutRequestWithResponse<StartApplicationRequest,StartApplicationResponse>(request, startAppRequest);
            }
        }

        public async Task<CreateSnapshotResponse> SnapshotApplication(Guid applicationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/applications/{applicationId}/snapshot"))
            {
                return await RequestAndDeserialiseAsync<CreateSnapshotResponse>(request, $"Could not snapshot the requested application");
            }
        }

        public async Task<Dictionary<string, object>> GetApplicationDataDictionary(Guid applicationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/applicationData"))
            {
                return await RequestAndDeserialiseAsync<Dictionary<string, object>>(request,
                    $"Could not find the application");
            }
        }

        public async Task<string> GetQuestionTag(Guid applicationId, string questionTag)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/applicationData/{questionTag}"))
            {
                return await RequestAndDeserialiseAsync<string>(request,
                    $"Could not find the question tag");
            }
        }

        public async Task<List<Sequence>> GetAllApplicationSequences(Guid applicationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/sequences"))
            {
                return await RequestAndDeserialiseAsync<List<Sequence>>(request,
                    $"Could not find all sequences");
            }
        }

        public async Task<List<Section>> GetAllApplicationSections(Guid applicationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/sections"))
            {
                return await RequestAndDeserialiseAsync<List<Section>>(request,
                    $"Could not find all sections");
            }
        }

        public async Task<Sequence> GetSequence(Guid applicationId, Guid sequenceId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/sequences/{sequenceId}"))
            {
                return await RequestAndDeserialiseAsync<Sequence>(request,
                    $"Could not find the sequence");
            }
        }

        public async Task<Sequence> GetSequenceBySequenceNo(Guid applicationId, int sequenceNo)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/sequences/{sequenceNo}"))
            {
                return await RequestAndDeserialiseAsync<Sequence>(request,
                    $"Could not find the sequence");
            }
        }

        public async Task<List<Section>> GetSections(Guid applicationId, Guid sequenceId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/sequences/{sequenceId}/sections"))
            {
                return await RequestAndDeserialiseAsync<List<Section>>(request,
                    $"Could not find the sections");
            }
        }

        public async Task<Section> GetSection(Guid applicationId, Guid sectionId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/sections/{sectionId}"))
            {
                return await RequestAndDeserialiseAsync<Section>(request,
                    $"Could not find the section");
            }
        }

        public async Task<Section> GetSectionBySectionNo(Guid applicationId, int sequenceNo, int sectionNo)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/sequences/{sequenceNo}/sections/{sectionNo}"))
            {
                return await RequestAndDeserialiseAsync<Section>(request,
                    $"Could not find the section");
            }
        }

        public async Task<Page> GetPage(Guid applicationId, Guid sectionId, string pageId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/sections/{sectionId}/pages/{pageId}"))
            {
                return await RequestAndDeserialiseAsync<Page>(request,
                    $"Could not find the page");
            }
        }

        public async Task<Page> GetPageBySectionNo(Guid applicationId, int sequenceNo, int sectionNo, string pageId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/sequences/{sequenceNo}/sections/{sectionNo}/pages/{pageId}"))
            {
                return await RequestAndDeserialiseAsync<Page>(request,
                    $"Could not find the page");
            }
        }

        public async Task<AddPageAnswerResponse> AddPageAnswers(Guid applicationId, Guid sectionId, string pageId, List<Answer> answer)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/applications/{applicationId}/sections/{sectionId}/pages/{pageId}/multiple"))
            {
                JsonSerializerOptions options = new();
                options.Converters.Add(new AddResultConverter());
                return await PostPutRequestWithResponse<List<Answer>, AddPageAnswerResponse> (request, answer, options);
            }
        }

        public async Task<Page> RemovePageAnswer(Guid applicationId, Guid sectionId, string pageId, Guid answerId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"/applications/{applicationId}/sections/{sectionId}/pages/{pageId}/multiple/{answerId}"))
            {
                return await Delete<Page> (request);
            }
        }

        public async Task<SetPageAnswersResponse> AddPageAnswer(Guid applicationId, Guid sectionId, string pageId, List<Answer> answer)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/applications/{applicationId}/sections/{sectionId}/pages/{pageId}"))
            {
                JsonSerializerOptions options = new();
                options.Converters.Add(new SetResultConverter());
                return await PostPutRequestWithResponse<List<Answer>, SetPageAnswersResponse>(request, answer,options);
            }
        }

        public async Task<SetPageAnswersResponse> Upload(Guid applicationId, Guid sectionId, string pageId,  IFormFileCollection files)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/applications/{applicationId}/sections/{sectionId}/pages/{pageId}/upload"))
            {
                var formDataContent = new MultipartFormDataContent();
                foreach (var file in files)
                {
                    var fileContent = new StreamContent(file.OpenReadStream())
                    { Headers = { ContentLength = file.Length, ContentType = new MediaTypeHeaderValue(file.ContentType) } };
                    formDataContent.Add(fileContent, file.Name, file.FileName);
                }
                JsonSerializerOptions options = new();
                options.Converters.Add(new SetResultConverter());
                return await PostRequestWithFileAndResponse<SetPageAnswersResponse>(request, formDataContent, options);
            }
        }

        public async Task<HttpResponseMessage> DownloadFile(Guid applicationId, Guid sectionId, string pageId, string questionId, string fileName)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/applications/{applicationId}/sections/{sectionId}/pages/{pageId}/questions/{questionId}/download/{fileName}"))
            {
                return await RequestToDownloadFile(request,
                    $"Could not download file {fileName}");
            }
        }

        public async Task DeleteFile(Guid applicationId, Guid sectionId, string pageId, string questionId, string fileName)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"/applications/{applicationId}/sections/{sectionId}/pages/{pageId}/questions/{questionId}/download/{fileName}"))
            {
                await Delete(request);
            }
        }

        public async Task<Page> UpdateFeedback(Guid applicationId, Guid sectionId, string pageId, QnA.Api.Types.Page.Feedback feedback)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Put, $"/applications/{applicationId}/sections/{sectionId}/pages/{pageId}/feedback"))
            {
                return await PostPutRequestWithResponse<QnA.Api.Types.Page.Feedback, Page>(request, feedback);
            }
        }

        public async Task<Page> DeleteFeedback(Guid applicationId, Guid sectionId, string pageId, Guid feedbackId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"/applications/{applicationId}/sections/{sectionId}/pages/{pageId}/feedback/{feedbackId}"))
            {
                 return await RequestAndDeserialiseAsync<Page>(request);
            }
        }

        private class SetResultConverter : JsonConverter<SetPageAnswersResponse>
        {
            public override SetPageAnswersResponse Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var jo = JsonNode.Parse(ref reader).AsObject();
                string nextAction = (string)jo["nextAction"];
                string nextActionId = (string)jo["nextActionId"];
                var errors = JsonSerializer.Deserialize<List<KeyValuePair<string, string>>>(jo["validationErrors"]?.ToString());
                
                return errors == null ? new SetPageAnswersResponse(nextAction, nextActionId) : new SetPageAnswersResponse(errors);
            }

            public override void Write(Utf8JsonWriter writer, SetPageAnswersResponse value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }

        private class AddResultConverter : JsonConverter<AddPageAnswerResponse>
        {
            public override AddPageAnswerResponse Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var jo = JsonNode.Parse(ref reader).AsObject();
                Page page = JsonSerializer.Deserialize<Page>(jo["page"].ToString());
                var errors = JsonSerializer.Deserialize<List<KeyValuePair<string, string>>>(jo["validationErrors"]?.ToString());

                return errors == null ? new AddPageAnswerResponse(page) : new AddPageAnswerResponse(errors);
            }

            public override void Write(Utf8JsonWriter writer, AddPageAnswerResponse value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }
    }
}
