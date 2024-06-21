﻿using CafeteriaServer.Commands.Admin;
using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands
{
    public class GetFeedbacksCommand : ICommand
    {
        private readonly IChefService _chefService;
        private readonly ILogger<GetFeedbacksCommand> _logger;

        public GetFeedbacksCommand(IChefService chefService, ILogger<GetFeedbacksCommand> logger)
        {
            _chefService = chefService;
            _logger = logger;
        }

        public async Task<string> Execute(string requestData)
        {
            _logger.LogInformation("Get feedbacks endpoint invoked");

            var response = new ViewFeedbackResponse();

            try
            {
                var feedbacks = await _chefService.GetAllFeedbacks();
                response.IsSuccess = true;
                response.Feedbacks = feedbacks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred");
                response.IsSuccess = false;
                response.ErrorMessage = ex.Message;
            }

            return JsonConvert.SerializeObject(response);
        }
    }
}
