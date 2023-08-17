﻿namespace GarageBuddy.Services.Data.Services
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    using GarageBuddy.Data.Common.Repositories;
    using GarageBuddy.Data.Models.Vehicle;

    using Microsoft.EntityFrameworkCore;

    using Models.Vehicle.FuelType;

    public class FuelTypeService : BaseService<FuelType, int>, IFuelTypeService
    {
        private readonly IDeletableEntityRepository<FuelType, int> fuelTypeRepository;

        public FuelTypeService(
            IDeletableEntityRepository<FuelType, int> entityRepository,
            IMapper mapper)
            : base(entityRepository, mapper)
        {
            this.fuelTypeRepository = entityRepository;
        }

        public async Task<ICollection<FuelTypeServiceModel>> GetAllAsync()
        {
            var query = this.fuelTypeRepository
                .All(ReadOnlyOption.ReadOnly, DeletedFilter.Deleted)
                .ProjectTo<FuelTypeServiceModel>(this.Mapper.ConfigurationProvider)
                .OrderBy(d => d.IsDeleted)
                .ThenBy(b => b.Id);

            return await query.ToListAsync();
        }

        public async Task<ICollection<FuelTypeSelectServiceModel>> GetAllSelectAsync()
        {
            return await fuelTypeRepository.All(ReadOnlyOption.ReadOnly, DeletedFilter.Deleted)
                .OrderBy(b => b.IsDeleted)
                .ThenBy(b => b.FuelName)
                .Select(b => new FuelTypeSelectServiceModel
                {
                    Id = b.Id,
                    FuelName = b.FuelName,
                }).ToListAsync();
        }

        public async Task<IResult<int>> CreateAsync(FuelTypeServiceModel model)
        {
            var isValid = ValidateModel(model);
            if (!isValid)
            {
                return await Result<int>.FailAsync(string.Format(Errors.EntityNotFound, "Fuel type"));
            }

            var fuelType = this.Mapper.Map<FuelType>(model);

            var entity = await fuelTypeRepository.AddAsync(fuelType);
            await fuelTypeRepository.SaveChangesAsync();
            var id = entity?.Entity.Id ?? UnknownId;

            if (entity?.Entity.Id > 0)
            {
                return await Result<int>.SuccessAsync(id);
            }

            return await Result<int>.FailAsync(string.Format(Errors.EntityNotCreated, "Fuel type"));
        }
    }
}
