using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreApiUndockerized.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace CoreApiUndockerized.Data
{
    public class ProductIdentityInitializer
    {
        private RoleManager<IdentityRole> _roleMgr;
        private UserManager<User> _userMgr;

        public ProductIdentityInitializer(UserManager<User> userMgr, RoleManager<IdentityRole> roleMgr)
        {
            _userMgr = userMgr;
            _roleMgr = roleMgr;
        }

        public async Task Seed()
        {
            var user = await _userMgr.FindByNameAsync("ensarinthehouse");

            // Add User
            if (user == null)
            {
                if (!await _roleMgr.RoleExistsAsync("Admin"))
                {
                    var role = new IdentityRole("Admin");
                    //role.Claims.Add(new IdentityRoleClaim<string>() { ClaimType = "IsAdmin", ClaimValue = "True" }); //TODO: Role.Claims does not work at 2.0. Need to figure out.
                    await _roleMgr.CreateAsync(role);
                }
            }

            user = new User()
            {
                UserName = "ensarinthehouse",
                FirstName = "Ensar",
                LastName = "Tobbas",
                Email = "wholetthedogsout@mail.com"
            };

            var userResult = await _userMgr.CreateAsync(user, "P@ssw0rd!");
            var roleResult = await _userMgr.AddToRoleAsync(user, "Admin");
            var claimResult = await _userMgr.AddClaimAsync(user, new Claim("SuperUser", "True"));


            if (!userResult.Succeeded || !roleResult.Succeeded || !claimResult.Succeeded)
            {
                throw new InvalidOperationException("Failed to build user and roles");
            }
        }
    }
}
