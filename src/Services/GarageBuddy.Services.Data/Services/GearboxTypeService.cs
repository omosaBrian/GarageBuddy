﻿namespace GarageBuddy.Services.Data.Services
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    using GarageBuddy.Data.Common.Repositories;
    using GarageBuddy.Data.Models.Vehicle;

    using Microsoft.EntityFrameworkCore;

    using Models.Vehicle.GearboxType;

    public class GearboxTypeService : BaseService<GearboxType, int>, IGearboxTypeService
    {
        private readonly IDeletableEntityRepository<GearboxType, int> gearboxTypeRepository;

        public GearboxTypeService(
            IDeletableEntityRepository<GearboxType, int> entityRepository,
            IMapper mapper)
            : base(entityRepository, mapper)
        {
            this.gearboxTypeRepository = entityRepository;
        }

        public async Task<ICollection<GearboxTypeServiceModel>> GetAllAsync()
        {
            var query = this.gearboxTypeRepository
                .All(ReadOnlyOption.ReadOnly, DeletedFilter.Deleted)
                .ProjectTo<GearboxTypeServiceModel>(this.Mapper.ConfigurationProvider)
                .OrderBy(d => d.IsDeleted)
                .ThenBy(b => b.Id);

            return await query.ToListAsync();
        }

        public async Task<ICollection<GearboxTypeSelectServiceModel>> GetAllSelectAsync()
        {
            return await gearboxTypeRepository.All(ReadOnlyOption.ReadOnly, DeletedFilter.Deleted)
                .OrderBy(b => b.IsDeleted)
                .ThenBy(b => b.GearboxTypeName)
                .Select(b => new GearboxTypeSelectServiceModel
                {
                    Id = b.Id,
                    GearboxTypeName = b.GearboxTypeName,
                }).ToListAsync();
        }

        public async Task<IResult<int>> CreateAsync(GearboxTypeServiceModel model)
        {
            var isValid = ValidateModel(model);
            if (!isValid)
            {
                return await Result<int>.FailAsync(string.Format(Errors.EntityNotFound, "Gearbox type"));
            }

            var gearboxType = this.Mapper.Map<GearboxType>(model);

            var entity = await gearboxTypeRepository.AddAsync(gearboxType);
            await gearboxTypeRepository.SaveChangesAsync();
            var id = entity?.Entity.Id ?? UnknownId;

            if (entity?.Entity.Id > 0)
            {
                return await Result<int>.SuccessAsync(id);
            }

            return await Result<int>.FailAsync(string.Format(Errors.EntityNotCreated, "Gearbox type"));
        }
    }
}
