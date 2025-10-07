using AYMDatingCore.DAL.Contexts;
using AYMDatingCore.DAL.BaseEntity;
using AYMDatingCore.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AYMDatingCore.DAL.Contexts
{
    public class AymanDatingCoreDbContextSeed
    {
        public static async Task SeedAsync(AymanDatingCoreDbContext context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager,  ILoggerFactory loggerFactory)
        {
            try
            {
                if (!context.CountryTBLs.Any())
                {
                    var Countries = File.ReadAllText("../AYMDatingCore.DAL/Contexts/SeedData/Country.json");
                    var CountryCollection = JsonSerializer.Deserialize<List<CountryTBL>>(Countries);
                    for (int i = 0; i < CountryCollection?.Count; i++)
                    {
                        context.CountryTBLs.Add(CountryCollection[i]);
                    }
                    await context.SaveChangesAsync();
                }
                if (!context.GenderTBLs.Any())
                {
                    var Genders = File.ReadAllText("../AYMDatingCore.DAL/Contexts/SeedData/Gender.json");
                    var GenderCollection = JsonSerializer.Deserialize<List<GenderTBL>>(Genders);
                    for (int i = 0; i < GenderCollection?.Count; i++)
                    {
                        context.GenderTBLs.Add(GenderCollection[i]);
                    }
                    await context.SaveChangesAsync();
                }

                if (!context.EducationTBLs.Any())
                {
                    var EducationData = File.ReadAllText("../AYMDatingCore.DAL/Contexts/SeedData/Education.json");
                    var Education = JsonSerializer.Deserialize<List<EducationTBL>>(EducationData);

                    for (int i = 0; i < Education?.Count; i++)
                    {
                        context.EducationTBLs.Add(Education[i]);
                    }
                    await context.SaveChangesAsync();
                }

                if (!context.FinancialModeTBLs.Any())
                {
                    var FinancialMode = File.ReadAllText("../AYMDatingCore.DAL/Contexts/SeedData/FinancialMode.json");
                    var FMode = JsonSerializer.Deserialize<List<FinancialModeTBL>>(FinancialMode);

                    for (int i = 0; i < FMode?.Count; i++)
                    {
                        context.FinancialModeTBLs.Add(FMode[i]);
                    }
                    await context.SaveChangesAsync();
                }
                if (!context.ProfessionTBLs.Any())
                {
                    var Jobs = File.ReadAllText("../AYMDatingCore.DAL/Contexts/SeedData/Profession.json");
                    var Job = Jobs.Split(',').ToList();
                    //var Job = JsonSerializer.Deserialize<List<Job>>(jobsArr);

                    for (int i = 0; i < Job?.Count; i++)
                    {
                        ProfessionTBL Newjob = new ProfessionTBL { Name = Job[i].Split("\"")[1] };
                        context.ProfessionTBLs.Add(Newjob);
                    }
                    await context.SaveChangesAsync();
                }
                if (!context.MaritalStatusTBLs.Any())
                {
                    var MaritalStatus = File.ReadAllText("../AYMDatingCore.DAL/Contexts/SeedData/MaritalStatus.json");
                    var MStatus = JsonSerializer.Deserialize<List<MaritalStatusTBL>>(MaritalStatus);

                    for (int i = 0; i < MStatus?.Count; i++)
                    {
                        context.MaritalStatusTBLs.Add(MStatus[i]);
                    }
                    await context.SaveChangesAsync();
                }
                if (!context.PurposeTBLs.Any())
                {
                    var Purposes = File.ReadAllText("../AYMDatingCore.DAL/Contexts/SeedData/Purpose.json");
                    var Purpose = JsonSerializer.Deserialize<List<PurposeTBL>>(Purposes);

                    for (int i = 0; i < Purpose?.Count; i++)
                    {
                        context.PurposeTBLs.Add(Purpose[i]);
                    }
                    await context.SaveChangesAsync();
                }
                if (!context.LanguageTBLs.Any())
                {
                    var Languages = File.ReadAllText("../AYMDatingCore.DAL/Contexts/SeedData/Language.json");
                    var Language = JsonSerializer.Deserialize<List<LanguageTBL>>(Languages);

                    for (int i = 0; i < Language?.Count; i++)
                    {
                        context.LanguageTBLs.Add(Language[i]);
                    }
                    await context.SaveChangesAsync();
                }

                if (!roleManager.Roles.Any())
                {
                    var role1 = new AppRole
                    {
                        Name = "Admin"
                    };
                    var role2 = new AppRole
                    {
                        Name = "User"
                    };


                    await roleManager.CreateAsync(role1);
                    await roleManager.CreateAsync(role2);
                }

                if (!userManager.Users.Any())
                {
                    var user1 = new AppUser
                    {
                        NormalizedUserName = "AymanElbatata".ToUpper(),
                        Email = "Ayman.Fathy.Elbatata@gmail.com",
                        UserName = "Ayman.Elbatata",
                        FirstName = "Ayman",
                        LastName = "Elbatata",
                        IsActivated = true,
                        EmailConfirmed = true,
                        LockoutEnabled = false,
                        PhoneNumber = "201284878483",
                        DateOfBirth = new DateTime(1982, 02, 01),
                        CountryTBLId = 50
                    };
                    var user2 = new AppUser
                    {
                        NormalizedUserName = "AymanFathy".ToUpper(),
                        Email = "Ayman_Elbatata@outlook.com",
                        UserName = "Ayman.Fathy",
                        FirstName = "Ayman",
                        LastName = "Fathy",
                        IsActivated = true,
                        DateOfBirth = new DateTime(1979, 07, 01),
                        EmailConfirmed = true,
                        LockoutEnabled = false,
                        PhoneNumber = "201202025251",
                        CountryTBLId = 50
                    };

                    var user3 = new AppUser
                    {
                        NormalizedUserName = "NoraAli".ToUpper(),
                        Email = "NoraAli@yahoo.com",
                        UserName = "Nora.Ali",
                        FirstName = "Nora",
                        LastName = "Ali",
                        IsActivated = true,
                        DateOfBirth = new DateTime(1983, 03, 01),
                        EmailConfirmed = true,
                        LockoutEnabled = false,
                        PhoneNumber = "201284878421",
                        CountryTBLId = 50
                    };
                    var user4 = new AppUser
                    {
                        NormalizedUserName = "SamiaAdel".ToUpper(),
                        Email = "SamiaAdel@yahoo.com",
                        UserName = "Samia.Adel",
                        FirstName = "Samia",
                        LastName = "Adel",
                        IsActivated = true,
                        DateOfBirth = new DateTime(1985, 05, 01),
                        EmailConfirmed = true,
                        LockoutEnabled = false,
                        PhoneNumber = "201284878422",
                        CountryTBLId = 51
                    };

                    var user5 = new AppUser
                    {
                        NormalizedUserName = "SamahAnwar".ToUpper(),
                        Email = "SamahAnwar@yahoo.com",
                        UserName = "Samah.Anwar",
                        FirstName = "Samah",
                        LastName = "Anwar",
                        IsActivated = true,
                        DateOfBirth = new DateTime(1988, 06, 01),
                        EmailConfirmed = true,
                        LockoutEnabled = false,
                        PhoneNumber = "201284878423",
                        CountryTBLId = 52
                    };

                    await userManager.CreateAsync(user1, "Aym@8513");
                    await userManager.CreateAsync(user2, "Aym@8513");
                    await userManager.CreateAsync(user3, "Aym@8513");
                    await userManager.CreateAsync(user4, "Aym@8513");
                    await userManager.CreateAsync(user5, "Aym@8513");

                    await userManager.AddToRoleAsync(user1, "Admin");
                    await userManager.AddToRoleAsync(user2, "User");
                    await userManager.AddToRoleAsync(user3, "User");
                    await userManager.AddToRoleAsync(user4, "User");
                    await userManager.AddToRoleAsync(user5, "User");
                    await context.SaveChangesAsync();

                    var userhistory1 = new UserHistoryTBL
                    {
                        AppUserId = user1.Id,
                        SearchAgeFrom = 18,
                        SearchAgeTo = 40,
                        CountryId = 1,
                        EducationId = 1,
                        FinancialModeId = 1,
                        GenderId = 1,
                        JobId = 1,
                        ProfileHeading = "ProfileHeading0123456789",
                        AboutPartner = "AboutPartnerTest0123456789",
                        AboutYou = "AboutYouTest0123456789",
                        City = "CityTest0123456789",
                        IsMain = true,
                        LanguageId = 1,
                        MaritalStatusId = 1,
                        PurposeId = 1,
                        IsSwitchedOff = false,
                        MainImageUrl = "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fcdn.openart.ai%2Fpublished%2FEb8kKVRSVNmrpwsB1rRO%2FfatzFxYM_ad86_raw.jpg&f=1&nofb=1&ipt=4b6ed6e2d1741cda6d11b4ebcc307b480221e9cd529f6b7fcf2ffd29c2c8da01"
                    };

                    var userhistory2 = new UserHistoryTBL
                    {
                        AppUserId = user2.Id,
                        SearchAgeFrom = 20,
                        SearchAgeTo = 55,
                        CountryId = 1,
                        EducationId = 1,
                        FinancialModeId = 1,
                        GenderId = 1,
                        JobId = 1,
                        ProfileHeading = "ProfileHeading0123456789",
                        AboutPartner = "AboutPartnerTest0123456789",
                        AboutYou = "AboutYouTest0123456789",
                        City = "CityTest0123456789",
                        IsMain = true,
                        LanguageId = 1,
                        MaritalStatusId = 1,
                        PurposeId = 1,
                        IsSwitchedOff = false,
                        MainImageUrl = "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fupload.wikimedia.org%2Fwikipedia%2Fcommons%2Fthumb%2F4%2F48%2FOutdoors-man-portrait_%2528cropped%2529.jpg%2F320px-Outdoors-man-portrait_%2528cropped%2529.jpg&f=1&nofb=1&ipt=10bded7f3dc0c3a8573b0782609be0da41eae1b5f92c8c4c5d6bab88a124a171"
                    };
                    var userhistory3 = new UserHistoryTBL
                    {
                        AppUserId = user3.Id,
                        SearchAgeFrom = 21,
                        SearchAgeTo = 70,
                        CountryId = 2,
                        EducationId = 2,
                        FinancialModeId = 2,
                        GenderId = 2,
                        JobId = 2,
                        ProfileHeading = "ProfileHeading0123456789",
                        AboutPartner = "AboutPartnerTest0123456789",
                        AboutYou = "AboutYouTest0123456789",
                        City = "CityTest0123456789",
                        IsMain = true,
                        LanguageId = 2,
                        MaritalStatusId = 2,
                        PurposeId = 2,
                        IsSwitchedOff = false,
                        MainImageUrl = "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fwww.1zoom.me%2Fbig2%2F17%2F179945-stranger.jpg&f=1&nofb=1&ipt=a918f10e958cc819a292f2aa2abd17975eb284e1eeae30e2859a653a8a0686be"
                    };

                    var userhistory4 = new UserHistoryTBL
                    {
                        AppUserId = user4.Id,
                        SearchAgeFrom = 23,
                        SearchAgeTo = 56,
                        CountryId = 3,
                        EducationId = 3,
                        FinancialModeId = 3,
                        GenderId = 2,
                        JobId = 3,
                        ProfileHeading = "ProfileHeading0123456789",
                        AboutPartner = "AboutPartnerTest0123456789",
                        AboutYou = "AboutYouTest0123456789",
                        City = "CityTest0123456789",
                        IsMain = true,
                        LanguageId = 3,
                        MaritalStatusId = 3,
                        PurposeId = 3,
                        IsSwitchedOff = false,
                        MainImageUrl = "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fwww.looktothestars.org%2Fphoto%2F10073-katherine-mcnamara%2Fstory_wide.jpg&f=1&nofb=1&ipt=3d527c59d28bcbca6ab5e270cfe0e807cd1e280761190c115fb437eef562d7d1"
                    };
                    var userhistory5 = new UserHistoryTBL
                    {
                        AppUserId = user5.Id,
                        SearchAgeFrom = 19,
                        SearchAgeTo = 66,
                        CountryId = 4,
                        EducationId = 4,
                        FinancialModeId = 4,
                        GenderId = 2,
                        JobId = 4,
                        ProfileHeading = "ProfileHeading0123456789",
                        AboutPartner = "AboutPartnerTest0123456789",
                        AboutYou = "AboutYouTest0123456789",
                        City = "CityTest0123456789",
                        IsMain = true,
                        LanguageId = 4,
                        MaritalStatusId = 4,
                        PurposeId = 1,
                        IsSwitchedOff = false,
                        MainImageUrl = "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fthumbs.dreamstime.com%2Fb%2Fportrait-young-girl-22962757.jpg&f=1&nofb=1&ipt=eb7723b5b12679e6134cf10bad4d21bdd6949af6e204b7b1747d62c5522d412c"
                    };
                    context.UserHistoryTBLs.Add(userhistory1);
                    context.UserHistoryTBLs.Add(userhistory2);
                    context.UserHistoryTBLs.Add(userhistory3);
                    context.UserHistoryTBLs.Add(userhistory4);
                    context.UserHistoryTBLs.Add(userhistory5);
                    await context.SaveChangesAsync();

                    context.UserImageTBLs.Add(new UserImageTBL() { AppUserId = user2.Id, ImageUrl = "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fimages.pexels.com%2Fphotos%2F614810%2Fpexels-photo-614810.jpeg%3Fcs%3Dsrgb%26dl%3Dpexels-simon-robben-55958-614810.jpg%26fm%3Djpg&f=1&nofb=1&ipt=1b1531702376832372569e03fb9372ac06071e12ddf446404ba0b7390c1b5188" });
                    context.UserImageTBLs.Add(new UserImageTBL() { AppUserId = user2.Id, ImageUrl = "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fthumbs.dreamstime.com%2Fb%2Fman-posing-image-english-retro-gangster-near-brick-wall-s-dressed-peaky-blinders-style-old-175309337.jpg%3Fw%3D768&f=1&nofb=1&ipt=6e9f17696391968f7f94ea202cc98acba373bb751fed5a735bba060ba85f38c0" });
                    context.UserImageTBLs.Add(new UserImageTBL() { AppUserId = user2.Id, ImageUrl = "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fc8.alamy.com%2Fcomp%2FE51NMR%2Fyoung-man-in-a-suit-showing-something-E51NMR.jpg&f=1&nofb=1&ipt=8aae35c5c97e1fb13e27e4e987381a5d2fc72d5fc362da55c774af70eb87f1c4" });
                    context.UserImageTBLs.Add(new UserImageTBL() { AppUserId = user2.Id, ImageUrl = "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fimages.pexels.com%2Fphotos%2F1222271%2Fpexels-photo-1222271.jpeg%3Fcs%3Dsrgb%26dl%3Dpexels-justin-shaifer-501272-1222271.jpg%26fm%3Djpg&f=1&nofb=1&ipt=0b9d9b5f4abeda8c71b152f2e8a5042e0366f6566ce2c5e5686b0c036a18488d" });
                    context.UserImageTBLs.Add(new UserImageTBL() { AppUserId = user2.Id, ImageUrl = "https://external-content.duckduckgo.com/iu/?u=https%3A%2F%2Fimages.pexels.com%2Fphotos%2F2379005%2Fpexels-photo-2379005.jpeg%3Fcs%3Dsrgb%26dl%3Dpexels-italo-melo-2379005.jpg%26fm%3Djpg&f=1&nofb=1&ipt=60791089232b007af701e7b373730da18e87c36c235083b816afe61144cd9a94" });
                    await context.SaveChangesAsync();
                }



            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<AymanDatingCoreDbContext>();
                logger.LogError(ex.Message);
            }
        }



    }
}
