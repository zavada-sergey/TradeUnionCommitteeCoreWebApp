﻿using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using TradeUnionCommittee.Api.Attributes;
using TradeUnionCommittee.Api.Extensions;
using TradeUnionCommittee.BLL.Contracts.Directory;
using TradeUnionCommittee.BLL.Contracts.SystemAudit;
using TradeUnionCommittee.BLL.DTO;
using TradeUnionCommittee.BLL.Enums;
using TradeUnionCommittee.ViewModels.ViewModels;

namespace TradeUnionCommittee.Api.Controllers.Directory
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class DormitoryController : ControllerBase
    {
        private readonly IDormitoryService _services;
        private readonly ISystemAuditService _systemAuditService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _accessor;
        private readonly ILogger<DormitoryController> _logger;

        public DormitoryController(IDormitoryService services, ISystemAuditService systemAuditService, IMapper mapper, IHttpContextAccessor accessor, ILogger<DormitoryController> logger)
        {
            _services = services;
            _systemAuditService = systemAuditService;
            _mapper = mapper;
            _accessor = accessor;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetAll")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(IEnumerable<DormitoryDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin,Accountant", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _services.GetAllAsync();
            if (result.IsValid)
            {
                return Ok(result.Result);
            }
            return BadRequest(result.ErrorsList);
        }

        [HttpGet]
        [Route("Get/{id}", Name = "GetDormitory")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(DormitoryDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin,Accountant", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Get([Required] string id)
        {
            var result = await _services.GetAsync(id);
            if (result.IsValid)
            {
                return Ok(result.Result);
            }
            return NotFound(result.ErrorsList);
        }

        [HttpPost]
        [Route("Create")]
        [ModelValidation]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(CreateDormitoryViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin,Accountant", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Create([FromBody] CreateDormitoryViewModel vm)
        {
            var result = await _services.CreateAsync(_mapper.Map<DormitoryDTO>(vm));
            if (result.IsValid)
            {
                await _systemAuditService.AuditAsync(User.GetEmail(), _accessor.GetIp(), Operations.Insert, Tables.AddressPublicHouse);
                return CreatedAtRoute("GetDormitory", new { version = "1.0", controller = "Dormitory", id = result.Result }, vm);
            }
            return UnprocessableEntity(result.ErrorsList);
        }

        [HttpPut]
        [Route("Update")]
        [ModelValidation]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin,Accountant", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Update([FromBody] UpdateDormitoryViewModel vm)
        {
            var result = await _services.UpdateAsync(_mapper.Map<DormitoryDTO>(vm));
            if (result.IsValid)
            {
                await _systemAuditService.AuditAsync(User.GetEmail(), _accessor.GetIp(), Operations.Update, Tables.AddressPublicHouse);
                return NoContent();
            }
            return BadRequest(result.ErrorsList);
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Delete([Required] string id)
        {
            var result = await _services.DeleteAsync(id);
            if (result.IsValid)
            {
                await _systemAuditService.AuditAsync(User.GetEmail(), _accessor.GetIp(), Operations.Delete, Tables.AddressPublicHouse);
                return NoContent();
            }
            return NotFound(result.ErrorsList);
        }
    }
}