﻿namespace GarageBuddy.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using GarageBuddy.Data.Common.Repositories;
    using GarageBuddy.Data.Models;
    using GarageBuddy.Services.Data.Contracts;
    using GarageBuddy.Web.ViewModels.Settings;

    using Microsoft.AspNetCore.Mvc;

    public class SettingsController : BaseController
    {
        private readonly ISettingsService settingsService;

        private readonly IDeletableEntityRepository<Setting, Guid> repository;

        public SettingsController(ISettingsService settingsService,
            IDeletableEntityRepository<Setting, Guid> repository)
        {
            this.settingsService = settingsService;
            this.repository = repository;
        }

        public IActionResult Index()
        {
            var model = new SettingsListViewModel
            {
                Settings = settingsService.GetAll<SettingViewModel>(),
            };

            return this.View(model);
        }

        public async Task<IActionResult> InsertSetting()
        {
            var random = new Random();
            var setting = new Setting { Name = $"Name_{random.Next()}", Value = $"Value_{random.Next()}" };

            await this.repository.AddAsync(setting);
            await this.repository.SaveChangesAsync();

            return this.RedirectToAction(nameof(this.Index));
        }
    }
}
