﻿namespace GarageBuddy.Services.Data.Services
{
    using AutoMapper;

    using GarageBuddy.Data.Common.Repositories;
    using GarageBuddy.Data.Models.Vehicle;

    using Microsoft.EntityFrameworkCore;

    using Models.Vehicle.DriveType;

    public class DriveTypeService : BaseService<DriveType, int>, IDriveTypeService
    {
        private readonly IDeletableEntityRepository<DriveType, int> driveTypeRepository;

        public DriveTypeService(
            IDeletableEntityRepository<DriveType, int> entityRepository,
            IMapper mapper)
            : base(entityRepository, mapper)
        {
            this.driveTypeRepository = entityRepository;
        }

        public async Task<ICollection<DriveTypeSelectServiceModel>> GetAllSelectAsync()
        {
            return await driveTypeRepository.All(ReadOnlyOption.ReadOnly, DeletedFilter.Deleted)
                .OrderBy(d => d.IsDeleted)
                .ThenBy(b => b.DriveTypeName)
                .Select(b => new DriveTypeSelectServiceModel
                {
                    Id = b.Id,
                    DriveTypeName = b.DriveTypeName,
                }).ToListAsync();
        }
    }
}
