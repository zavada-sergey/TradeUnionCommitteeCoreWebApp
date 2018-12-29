﻿using System.Threading.Tasks;
using TradeUnionCommittee.BLL.DTO;
using TradeUnionCommittee.BLL.Interfaces.Lists;
using TradeUnionCommittee.BLL.Utilities;
using TradeUnionCommittee.Common.ActualResults;
using TradeUnionCommittee.DAL.Entities;
using TradeUnionCommittee.DAL.Interfaces;

namespace TradeUnionCommittee.BLL.Services.Lists
{
    public class PrivilegeEmployeesService : IPrivilegeEmployeesService
    {
        private readonly IUnitOfWork _database;
        private readonly IAutoMapperUtilities _mapperService;
        private readonly IHashIdUtilities _hashIdUtilities;

        public PrivilegeEmployeesService(IUnitOfWork database, IAutoMapperUtilities mapperService, IHashIdUtilities hashIdUtilities)
        {
            _database = database;
            _mapperService = mapperService;
            _hashIdUtilities = hashIdUtilities;
        }

        public async Task<ActualResult<PrivilegeEmployeesDTO>> GetAsync(string hashIdEmployee)
        {
            var id = _hashIdUtilities.DecryptLong(hashIdEmployee, Enums.Services.Employee);
            var result = await _database.PrivilegeEmployeesRepository.GetWithInclude(x => x.IdEmployee == id, c => c.IdPrivilegesNavigation);
            return _mapperService.Mapper.Map<ActualResult<PrivilegeEmployeesDTO>>(result);
        }

        public async Task<ActualResult> CreateAsync(PrivilegeEmployeesDTO dto)
        {
            await _database.PrivilegeEmployeesRepository.Create(_mapperService.Mapper.Map<PrivilegeEmployees>(dto));
            return _mapperService.Mapper.Map<ActualResult>(await _database.SaveAsync());
        }

        public async Task<ActualResult> UpdateAsync(PrivilegeEmployeesDTO dto)
        {
            await _database.PrivilegeEmployeesRepository.Update(_mapperService.Mapper.Map<PrivilegeEmployees>(dto));
            return _mapperService.Mapper.Map<ActualResult>(await _database.SaveAsync());
        }

        public async Task<ActualResult> DeleteAsync(string hashId)
        {
            await _database.PrivilegeEmployeesRepository.Delete(_hashIdUtilities.DecryptLong(hashId, Enums.Services.PrivilegeEmployees));
            return _mapperService.Mapper.Map<ActualResult>(await _database.SaveAsync());
        }

        public void Dispose()
        {
            _database.Dispose();
        }
    }
}