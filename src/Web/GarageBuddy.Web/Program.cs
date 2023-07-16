﻿using System;
using System.Reflection;

using GarageBuddy.Data;
using GarageBuddy.Data.Common;
using GarageBuddy.Data.Common.Repositories;
using GarageBuddy.Data.Models;
using GarageBuddy.Data.Repositories;
using GarageBuddy.Data.Seeding;
using GarageBuddy.Services.Data;
using GarageBuddy.Services.Mapping;
using GarageBuddy.Services.Messaging;
using GarageBuddy.Web.ViewModels;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (connectionString == null)
{
    throw new InvalidOperationException("Connection string not found");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<CookiePolicyOptions>(
    options =>
    {
        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.None;
    });

builder.Services.AddControllersWithViews()
    .AddMvcOptions(options =>
    {
        // TODO: Add model binder providers
        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    }).AddRazorRuntimeCompilation();

builder.Services.AddRazorPages();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddSingleton(builder.Configuration);

// Data repositories
builder.Services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped<IDbQueryRunner, DbQueryRunner>();

// Application services
builder.Services.AddTransient<IEmailSender, NullMessageSender>();
builder.Services.AddTransient<ISettingsService, SettingsService>();

var app = builder.Build();

// Seed data on application startup
using (var serviceScope = app.Services.CreateScope())
{
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
    new ApplicationDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider).GetAwaiter()
        .GetResult();
}

AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    // TODO: Add custom error pages
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();