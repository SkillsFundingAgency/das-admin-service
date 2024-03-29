﻿using SFA.DAS.Http;
using System.Net.Http;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.QnA
{
    public class QnaApiClientFactory : IQnaApiClientFactory
    {
        private readonly QnaApiClientConfiguration _qnaApiClientConfiguration;

        public QnaApiClientFactory(QnaApiClientConfiguration qnaApiClientConfiguration)
        {
            _qnaApiClientConfiguration = qnaApiClientConfiguration;
        }

        public HttpClient CreateHttpClient()
        {
            var httpClient = new ManagedIdentityHttpClientFactory(_qnaApiClientConfiguration).CreateHttpClient();
            return httpClient;
        }
    }
}
