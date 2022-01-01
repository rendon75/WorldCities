﻿using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using WorldCities.Controllers;
using WorldCities.Data;
using WorldCities.Data.Models;
using Xunit;

namespace WorldCities.Tests
{
    public class SeeedController_Tests
    {
        /// <summary>
        /// Test the CreateDefaultUsers() method.
        /// </summary>
        [Fact]
        public async void CreateDefaultUsers()
        {
            #region Arrange
            // Create the option instances required by the
            // ApplicationDbContext
            var options = new
                DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(databaseName: "WorldCities")
                    .Options;
            var storeOptions = Options.Create(new OperationalStoreOptions());

            // create a IWebHost environment mock instance
            var mockEnv = new Mock<IWebHostEnvironment>().Object;

            // define the variables for the users we want to test
            ApplicationUser user_Admin = null;
            ApplicationUser user_User = null;
            ApplicationUser user_NotExisting = null;
            #endregion

            #region Act

            // create a ApplicationDbContext instance using the
            // in-memory DB
            using (var context = new ApplicationDbContext(options, storeOptions))
            {
                // create a RoleManager instance
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = IdentityHelper.GetRoleManager(
                    new RoleStore<IdentityRole>(context));

                // create a UserManager instance
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = IdentityHelper.GetUserManager(
                    new UserStore<ApplicationUser>(context));

                // create a SeedController instance
                var controller = new SeedController(
                    context,
                    roleManager,
                    userManager,
                    mockEnv);

                // execute the SeedController's CreateDefaultUsers()
                // method to create the default users (and roles)
                await controller.CreateDefaultUsers();

                // retrieve the users
                user_Admin = await userManager.FindByEmailAsync("admin@email.com");
                user_User = await userManager.FindByEmailAsync("user@email.com");
                user_NotExisting = await userManager.FindByEmailAsync("notexisting@email.com");
            }
            #endregion

            #region Assert
            Assert.NotNull(user_Admin);
            Assert.NotNull(user_User);
            Assert.Null(user_NotExisting);
            #endregion
        }
    }
}
