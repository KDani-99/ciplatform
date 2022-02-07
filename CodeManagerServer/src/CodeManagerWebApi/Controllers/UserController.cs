﻿using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using CodeManager.Data.Commands;
using CodeManager.Data.Configuration;
using CodeManager.Data.Entities;
using CodeManagerWebApi.DataTransfer;
using CodeManagerWebApi.Services;
using CodeManagerWebApi.WebSocket;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CodeManagerWebApi.Controllers
{
    [ApiController]
    [Route("/api/user")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IWebApiClient _webApiClient;
        private readonly IRequestClient<QueueRunCommand> _requestClient;

        public UserController(IUserService userService, ILogger<UserController> logger, IRequestClient<QueueRunCommand> requestClient)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _requestClient = requestClient;
        }

        [HttpPost, Route("test")]
        public async Task<IActionResult> QueueRun()
        {
            string content = await new StreamReader(Request.Body).ReadToEndAsync();
          /*  var deserializer = new DeserializerBuilder()
                .WithNamingConvention(LowerCaseNamingConvention.Instance)
                .Build();
            
            var configuration = deserializer.Deserialize<RunConfiguration>(content);*/
            
          // TODO: send data with the request
            var response = await _requestClient.GetResponse<SuccessfulQueueRunCommandResponse, FailedQueueRunCommandResponse>(new QueueRunCommand {RunConfigurationString = content, Repository = "https://github.com/KDani-99/doc-assistant"});

            //var response = await _webApiClient.HubConnection.InvokeAsync<long?>(CommonManagerMethods.QueueRun);
           
            return Ok(response);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet, Route("{id:long}")]
        public async Task<IActionResult> GetUserById([FromRoute] long id )
        {
            var user = HttpContext.Items["user"] as User;

            var userDto = await _userService.GetUserAsync(id, user);

            return Ok(userDto);
        }
        
        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var user = HttpContext.Items["user"] as User;
            var userDto = await _userService.GetUserAsync(user.Id, user);

            return Ok(userDto);
        }
        
        [Authorize(Roles = "User,Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateUser()
        {
            throw new NotImplementedException();
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet, Route("/api/users")]
        public async Task<IActionResult> GetUsers()
        {
            var user = HttpContext.Items["user"] as User;
            var usersDto = await _userService.GetUsersAsync(user);

            return Ok(usersDto);
        }

        [HttpPost, Route("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDto createUserDto)
        {
            await _userService.CreateUser(createUserDto);
            _logger.LogInformation($"User `{createUserDto.Username}` has been created @ {DateTime.Now}");

            return StatusCode((int)HttpStatusCode.Created);
        }
        
    }
}