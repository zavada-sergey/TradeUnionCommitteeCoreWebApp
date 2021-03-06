﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradeUnionCommittee.Api.Attributes;
using TradeUnionCommittee.Api.Configurations;
using TradeUnionCommittee.Api.Models;
using TradeUnionCommittee.ViewModels.ViewModels;

namespace TradeUnionCommittee.Api.Controllers.Account
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IJwtBearerConfiguration _jwtBearer;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IJwtBearerConfiguration jwtBearer, ILogger<AccountController> logger)
        {
            _jwtBearer = jwtBearer;
            _logger = logger;
        }

        /// <summary>
        /// Client Type available values:
        /// 'Web-Application',
        /// 'Desktop-Application' or
        /// 'Mobile-Application'
        /// </summary>
        [HttpPost]
        [Route("Token", Name = "Token")]
        [ModelValidation]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Token([FromBody] TokenViewModel viewModel)
        {
            var account = await _jwtBearer.SignInByPassword(viewModel);
            if (account.IsValid)
            {
                return WriteToken(account.Result);
            }
            return BadRequest(account.ErrorsList);
        }

        /// <summary>
        /// Client Type available values:
        /// 'Web-Application',
        /// 'Desktop-Application' or
        /// 'Mobile-Application'
        /// </summary>
        [HttpPost]
        [Route("RefreshToken", Name = "RefreshToken")]
        [ModelValidation]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenViewModel viewModel)
        {
            var account = await _jwtBearer.SignInByRefreshToken(viewModel);
            if (account.IsValid)
            {
                return WriteToken(account.Result);
            }
            return BadRequest(account.ErrorsList);
        }

        //-----------------------------------------------------------------------------------------------

        private OkObjectResult WriteToken(TokenModel model)
        {
            return Ok(new
            {
                access_token = model.AccessToken,
                refresh_token = model.RefreshToken,
                token_type = model.TokenType
            });
        }
    }
}