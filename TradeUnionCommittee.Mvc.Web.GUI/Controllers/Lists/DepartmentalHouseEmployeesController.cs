﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using TradeUnionCommittee.BLL.DTO;
using TradeUnionCommittee.BLL.Enums;
using TradeUnionCommittee.BLL.Interfaces.Lists;
using TradeUnionCommittee.BLL.Interfaces.SystemAudit;
using TradeUnionCommittee.Mvc.Web.GUI.Controllers.Directory;
using TradeUnionCommittee.ViewModels.ViewModels;

namespace TradeUnionCommittee.Mvc.Web.GUI.Controllers.Lists
{
    public class DepartmentalHouseEmployeesController : Controller
    {
        private readonly IPublicHouseEmployeesService _services;
        private readonly IDirectories _directories;
        private readonly IMapper _mapper;
        private readonly ISystemAuditService _systemAuditService;
        private readonly IHttpContextAccessor _accessor;

        public DepartmentalHouseEmployeesController(IPublicHouseEmployeesService service, IDirectories directories, IMapper mapper, ISystemAuditService systemAuditService, IHttpContextAccessor accessor)
        {
            _services = service;
            _mapper = mapper;
            _systemAuditService = systemAuditService;
            _accessor = accessor;
            _directories = directories;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        [HttpGet]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> Index([Required] string id)
        {
            var result = await _services.GetAllAsync(id, PublicHouse.Departmental);
            if (result.IsValid)
            {
                ViewData["HashIdEmployee"] = id;
                return View(result.Result);
            }
            TempData["ErrorsList"] = result.ErrorsList;
            return View();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        [HttpGet]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> Create([Required] string id)
        {
            ViewBag.Departmental = await _directories.GetDepartmental();
            return View(new CreatePublicHouseEmployeesViewModel { HashIdEmployee = id });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Accountant")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePublicHouseEmployeesViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var result = await _services.CreateAsync(_mapper.Map<PublicHouseEmployeesDTO>(vm));
                if (result.IsValid)
                {
                    await _systemAuditService.AuditAsync(User.Identity.Name, _accessor.HttpContext.Connection.RemoteIpAddress.ToString(), Operations.Insert, Tables.PublicHouseEmployees);
                    return RedirectToAction("Index", new { id = vm.HashIdEmployee });
                }
                TempData["ErrorsList"] = result.ErrorsList;
            }
            return View(vm);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        [HttpGet]
        [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> Update([Required] string id)
        {
            var result = await _services.GetAsync(id);
            if (result.IsValid)
            {
                ViewBag.Departmental = await _directories.GetDepartmental(result.Result.HashIdAddressPublicHouse);
                return View(_mapper.Map<UpdatePublicHouseEmployeesViewModel>(result.Result));
            }
            TempData["ErrorsList"] = result.ErrorsList;
            return View();
        }

        [HttpPost, ActionName("Update")]
        [Authorize(Roles = "Admin,Accountant")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateConfirmed(UpdatePublicHouseEmployeesViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var result = await _services.UpdateAsync(_mapper.Map<PublicHouseEmployeesDTO>(vm));
                if (result.IsValid)
                {
                    await _systemAuditService.AuditAsync(User.Identity.Name, _accessor.HttpContext.Connection.RemoteIpAddress.ToString(), Operations.Update, Tables.PublicHouseEmployees);
                    return RedirectToAction("Index", new { id = vm.HashIdEmployee });
                }
                TempData["ErrorsListConfirmed"] = result.ErrorsList;
            }
            return View(vm);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([Required] string id)
        {
            var result = await _services.GetAsync(id);
            if (result.IsValid)
            {
                return View(result.Result);
            }
            TempData["ErrorsList"] = result.ErrorsList;
            return View();
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([Required] string hashId, [Required] string hashIdEmployee)
        {
            var result = await _services.DeleteAsync(hashId);
            if (result.IsValid)
            {
                await _systemAuditService.AuditAsync(User.Identity.Name, _accessor.HttpContext.Connection.RemoteIpAddress.ToString(), Operations.Delete, Tables.PublicHouseEmployees);
                return RedirectToAction("Index", new { id = hashIdEmployee });
            }
            TempData["ErrorsList"] = result.ErrorsList;
            return View();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        protected override void Dispose(bool disposing)
        {
            _services.Dispose();
            base.Dispose(disposing);
        }
    }
}