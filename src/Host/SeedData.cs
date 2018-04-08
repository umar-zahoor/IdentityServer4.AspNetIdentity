// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Host.Configuration;
using Host.Data;
using Host.Models;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Host
{
    public class SeedData
    {
        public static async Task EnsureSeedData(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                {
                    var context = scope.ServiceProvider.GetService<ConfigurationDbContext>();
                    context.Database.Migrate();
                    EnsureSeedData(context).Wait();
                }

                {
                    var context = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
                    context.Database.Migrate();

                    #region Ensure Users

                    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApiUser>>();
                    var alice = await userMgr.FindByNameAsync("alice");
                    if (alice == null)
                    {
                        alice = new ApiUser
                        {
                            UserName = "alice"
                        };

                        var result = await userMgr.CreateAsync(alice, "Pass123$");
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = await userMgr.AddClaimsAsync(alice, new Claim[]
                        {
                            new Claim(JwtClaimTypes.Name, "Alice Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Alice"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                            new Claim(JwtClaimTypes.Address,
                                @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }",
                                IdentityServerConstants.ClaimValueTypes.Json)
                        });
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        Console.WriteLine("alice created");
                    }
                    else
                    {
                        Console.WriteLine("alice already exists");
                    }

                    var bob = await userMgr.FindByNameAsync("bob");
                    if (bob == null)
                    {
                        bob = new ApiUser
                        {
                            UserName = "bob"
                        };
                        var result = await userMgr.CreateAsync(bob, "Pass123$");
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = await userMgr.AddClaimsAsync(bob, new Claim[]
                        {
                            new Claim(JwtClaimTypes.Name, "Bob Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Bob"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                            new Claim(JwtClaimTypes.Address,
                                @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }",
                                IdentityServerConstants.ClaimValueTypes.Json),
                            new Claim("location", "somewhere")
                        });
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        Console.WriteLine("bob created");
                    }
                    else
                    {
                        Console.WriteLine("bob already exists");
                    }

                    #endregion

                    #region Ensure Roles

                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApiRole>>();

                    var roleExists = await roleManager.RoleExistsAsync("superuser");

                    if (!roleExists)
                    {
                        var role = new ApiRole {Name = "superuser"};
                        var result = await roleManager.CreateAsync(role);

                        if (result.Succeeded)
                        {
                            await userMgr.AddToRoleAsync(alice, role.Name);
                        }
                    }

                    #endregion
                }
            }
        }

        private static async Task EnsureSeedData(ConfigurationDbContext context)
        {
            if (!context.Clients.Any())
            {
                Console.WriteLine("Clients being populated");
                foreach (var client in TestConfig.GetClients().ToList())
                {
                    context.Clients.Add(client.ToEntity());
                }
                await context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("Clients already populated");
            }

            if (!context.IdentityResources.Any())
            {
                Console.WriteLine("IdentityResources being populated");
                foreach (var resource in TestConfig.GetIdentityResources().ToList())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                await context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("IdentityResources already populated");
            }

            if (!context.ApiResources.Any())
            {
                Console.WriteLine("ApiResources being populated");
                foreach (var resource in TestConfig.GetApiResources().ToList())
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
                await context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("ApiResources already populated");
            }
        }
    }
}