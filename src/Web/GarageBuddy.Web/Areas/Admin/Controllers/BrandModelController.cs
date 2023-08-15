﻿namespace GarageBuddy.Web.Areas.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using DataTables.AspNet.AspNetCore;
    using DataTables.AspNet.Core;
    using GarageBuddy.Services.Data.Common;

    using GarageBuddy.Services.Data.Models.Vehicle.Brand;

    using GarageBuddy.Web.ViewModels.Admin.Brand;

    using Microsoft.AspNetCore.Mvc;

    using Services.Data.Contracts;
    using Services.Data.Models.Vehicle.BrandModel;
    using ViewModels.Admin.BrandModel;

    public class BrandModelController : AdminController
    {
        private readonly IBrandModelService brandModelService;
        private readonly IBrandService brandService;
        private readonly IMapper mapper;

        public BrandModelController(
            IBrandModelService brandModelService,
            IBrandService brandService,
            IMapper mapper)
        {
            this.brandModelService = brandModelService;
            this.mapper = mapper;
            this.brandService = brandService;
        }


        public IActionResult Index(Guid? brandId)
        {
            return View();
        }

        public async Task<IActionResult> BrandModelList(IDataTablesRequest request)
        {
            if (request.Length <= 0)
            {
                return new DataTablesJsonResult(DataTablesResponse.Create(request, 0, 0, null), true);
            }

            request.AdditionalParameters.TryGetValue(IncludeDeletedFilterName, out var searchValue);

            var brandsResult = await this.brandModelService.GetAllAsync(
                new QueryOptions<BrandModelListServiceModel>()
                {
                    IncludeDeleted = (bool)(searchValue ?? false),
                    Skip = request.Start,
                    Take = request.Length,
                });

            var data = mapper.Map<ICollection<BrandModelListViewModel>>(brandsResult.Data);


            var response = DataTablesResponse.Create(request, brandsResult.TotalCount, brandsResult.TotalCount, data);

            return new DataTablesJsonResult(response, true);
        }
    }
}