﻿using System.Linq;
using System.Threading.Tasks;
using TradeUnionCommittee.BLL.Configurations;
using TradeUnionCommittee.BLL.DTO;
using TradeUnionCommittee.BLL.Enums;
using TradeUnionCommittee.BLL.Interfaces.General;
using TradeUnionCommittee.Common.ActualResults;
using TradeUnionCommittee.DAL.Entities;
using TradeUnionCommittee.DAL.Interfaces;

namespace TradeUnionCommittee.BLL.Services.General
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _database;
        private readonly IAutoMapperConfiguration _mapperService;
        private readonly IHashIdConfiguration _hashIdUtilities;

        public EmployeeService(IUnitOfWork database, IAutoMapperConfiguration mapperService, IHashIdConfiguration hashIdUtilities)
        {
            _database = database;
            _mapperService = mapperService;
            _hashIdUtilities = hashIdUtilities;
        }

        public async Task<ActualResult> AddEmployeeAsync(CreateEmployeeDTO dto)
        {
            var employee = _mapperService.Mapper.Map<DAL.Entities.Employee>(dto);
            await _database.EmployeeRepository.Create(employee);
            var createEmployee = await _database.SaveAsync();

            if (createEmployee.IsValid)
            {
                dto.IdEmployee = employee.Id;

                await _database.PositionEmployeesRepository.Create(_mapperService.Mapper.Map<PositionEmployees>(dto));

                if (dto.TypeAccommodation == AccommodationType.PrivateHouse || dto.TypeAccommodation == AccommodationType.FromUniversity)
                {
                    await _database.PrivateHouseEmployeesRepository.Create(_mapperService.Mapper.Map<PrivateHouseEmployees>(dto));
                }

                if (dto.TypeAccommodation == AccommodationType.Dormitory || dto.TypeAccommodation == AccommodationType.Departmental)
                {
                    await _database.PublicHouseEmployeesRepository.Create(_mapperService.Mapper.Map<PublicHouseEmployees>(dto));
                }

                if (dto.SocialActivity)
                {
                    await _database.SocialActivityEmployeesRepository.Create(_mapperService.Mapper.Map<SocialActivityEmployees>(dto));
                }

                if (dto.Privileges)
                {
                    await _database.PrivilegeEmployeesRepository.Create(_mapperService.Mapper.Map<PrivilegeEmployees>(dto));
                }
            }

            var result = await _database.SaveAsync();
            if (result.IsValid)
            {
                return new ActualResult();
            }
            await DeleteAsync(dto.IdEmployee);
            return new ActualResult(result.ErrorsList);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        public async Task<ActualResult<GeneralInfoEmployeeDTO>> GetMainInfoEmployeeAsync(string hashId)
        {
            var resultSearchByHashId = await _database
                .EmployeeRepository
                .GetById(_hashIdUtilities.DecryptLong(hashId, Enums.Services.Employee));
            return _mapperService.Mapper.Map<ActualResult<GeneralInfoEmployeeDTO>>(resultSearchByHashId);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        public async Task<ActualResult> UpdateMainInfoEmployeeAsync(GeneralInfoEmployeeDTO dto)
        {
            await _database.EmployeeRepository.Update(_mapperService.Mapper.Map<DAL.Entities.Employee>(dto));
            return _mapperService.Mapper.Map<ActualResult>(await _database.SaveAsync());
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        public async Task<ActualResult> DeleteAsync(string hashId)
        {
            return _mapperService.Mapper.Map<ActualResult>(await DeleteAsync(_hashIdUtilities.DecryptLong(hashId, Enums.Services.Employee)));
        }

        private async Task<ActualResult> DeleteAsync(long id)
        {
            await _database.EmployeeRepository.Delete(id);
            return _mapperService.Mapper.Map<ActualResult>(await _database.SaveAsync());
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        public async Task<bool> CheckIdentificationСode(string identificationСode)
        {
            var result = await _database.EmployeeRepository.Find(p => p.IdentificationСode == identificationСode);
            return result.Result.Any();
        }

        public async Task<bool> CheckMechnikovCard(string mechnikovCard)
        {
            var result = await _database.EmployeeRepository.Find(p => p.MechnikovCard == mechnikovCard);
            return result.Result.Any();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        public void Dispose()
        {
            _database.Dispose();
        }
    }
}