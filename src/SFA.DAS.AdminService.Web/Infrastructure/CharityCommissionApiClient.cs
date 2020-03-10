﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.ApplyTypes.CharityCommission;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public class CharityCommissionApiClient:ICharityCommissionApiClient
    {
        private ILogger<CharityCommissionApiClient> _logger;
        private readonly IRoatpApplyTokenService _tokenService;
        private static  HttpClient _httpClient = new HttpClient();

        public CharityCommissionApiClient(string baseUri,
            ILogger<CharityCommissionApiClient> logger, IRoatpApplyTokenService tokenService)
        {
            _tokenService = tokenService;
            _logger = logger;
 
            _httpClient = new HttpClient { BaseAddress = new Uri(baseUri) };
            
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
        }

        public async Task<Charity> GetCharityDetails(int charityNumber)
        {
            var requestMessage =
                await _httpClient.GetAsync($"/charity-commission-lookup?charityNumber={charityNumber}");

            return await requestMessage.Content.ReadAsAsync<Charity>();
            //return await Get<Charity>($"/charity-commission-lookup?charityNumber={charityNumber}");
        }
    }



    //public class ApplyServicePassthroughApiClient : IApplyServicePassthroughApiClient
    //{
    //    private readonly HttpClient _client;
    //    private readonly IRoatpApplyTokenService _tokenService;

    //    public ApplyServicePassthroughApiClient(string baseUri, IRoatpApplyTokenService tokenService)
    //    {
    //        _client = new HttpClient { BaseAddress = new Uri(baseUri) };
    //        _tokenService = tokenService;
    //    }

    //    public async Task<Charity> GetCharityDetails(string charityNumber)
    //    {
    //        return await Get<Charity>($"/charity-commission-lookup?charityNumber={charityNumber}");
    //    }


    //    private async Task<T> Get<T>(string uri)
    //    {
    //        _client.DefaultRequestHeaders.Authorization =
    //            new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

    //        using (var response = await _client.GetAsync(new Uri(uri, UriKind.Relative)))
    //        {
    //            return await response.Content.ReadAsAsync<T>();
    //        }
    //    }
    //}
}
