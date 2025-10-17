﻿using AutoMapper;
using AYMDatingCore.BLL.Interfaces;
using AYMDatingCore.BLL.Repositories;
using AYMDatingCore.DAL.BaseEntity;
using AYMDatingCore.DAL.Entities;
using AYMDatingCore.Helpers;
using AYMDatingCore.PL.DTO;
using AYMDatingCore.PL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AYMDatingCore.PL.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper Mapper;
        private readonly IHubContext<NotificationHub> _hubContext;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, IMapper Mapper, IHubContext<NotificationHub> hubContext)
        {
            _logger = logger;
            this.unitOfWork = unitOfWork;
            this.Mapper = Mapper;
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            return View(GetAllUsersOrFiltered(new UserMainSearchDTO()));
        }

        [HttpPost]
        public IActionResult Index(UserMainSearchDTO model)
        {
            return View(GetAllUsersOrFiltered(model));
        }

        public async Task<IActionResult> UserProfile(string? UserName)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                return RedirectToAction("NotFound", "Home");
            }

            var user = await GetUserByUserName(UserName);

            if (user == null)
            {
                return RedirectToAction("NotFound", "Home");
            }


            if (!string.IsNullOrEmpty(User.Identity.Name) && User.Identity.Name != UserName) {
                var SenderUser = await GetUserByUserName(User.Identity.Name);
                if (!unitOfWork.UserViewRepository.GetAllCustomized(filter: a => a.IsDeleted == false && a.SenderAppUserId == SenderUser.Id && a.ReceiverAppUserId == user.Id).Any())
                {
                    unitOfWork.UserViewRepository.Add(new UserViewTBL() { SenderAppUserId = SenderUser.Id, ReceiverAppUserId = user.Id });
                    await _hubContext.Clients.User(user.Id).SendAsync("ReceiveViewNotification", unitOfWork.UserViewRepository.GetAllCustomized(filter: a => a.IsDeleted == false && a.IsSeen == false && a.ReceiverAppUserId == user.Id).Count());

                }
            }

            var currentUser = unitOfWork.UserHistoryRepository.GetAllCustomized(filter: a => a.IsDeleted == false && a.IsMain == true && a.AppUserId == user.Id, includes: new Expression<Func<UserHistoryTBL, object>>[]
{
                                         p => p.AppUser,
                                         p => p.Country,
                                         p => p.Language,
                                         p => p.Gender,
                                         p => p.MaritalStatus,
                                         p => p.Job,
                                         p => p.Purpose,
                                         p => p.FinancialMode,
                                         p => p.Education,

                        }).Where(u => unitOfWork.UserManager.IsInRoleAsync(u.AppUser, "User").Result)
                        .FirstOrDefault();
            if (currentUser != null) 
            {
                var data = Mapper.Map<UserHistoryTBL_VM>(currentUser);
                data.UserImageTBL_VM = Mapper.Map<List<UserImageTBL_VM>>(unitOfWork.UserImageRepository.GetAllCustomized(
                            filter: a => a.IsDeleted == false && a.AppUserId == user.Id).OrderByDescending(a => a.CreationDate));

                if (!string.IsNullOrEmpty(User.Identity.Name) && User.Identity.Name != UserName)
                {
                    var SenderUser = await GetUserByUserName(User.Identity.Name);
                    data.IsLiked = unitOfWork.UserLikeRepository.GetAllCustomized(filter: a => a.IsDeleted == false && a.SenderAppUserId == SenderUser.Id && a.ReceiverAppUserId == user.Id).Any();
                    data.Isblocked = unitOfWork.UserBlockRepository.GetAllCustomized(filter: a => a.IsDeleted == false && a.SenderAppUserId == SenderUser.Id && a.ReceiverAppUserId == user.Id).Any();
                    data.IsFavorite = unitOfWork.UserFavoriteRepository.GetAllCustomized(filter: a => a.IsDeleted == false && a.SenderAppUserId == SenderUser.Id && a.ReceiverAppUserId == user.Id).Any();
                }
                return View(data);
            }
            return View(new UserHistoryTBL_VM());
        }


        public IActionResult PrivacyNotice()
        {

            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult ConditionsofUse()
        {
            return View();
        }

        public IActionResult HelpPage()
        {
            return View();
        }

        public new IActionResult NotFound()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private List<UserHistoryTBL_VM> GetAllUsersOrFiltered(UserMainSearchDTO model)
        {
            var users = unitOfWork.UserHistoryRepository.GetAllCustomized(filter: a => a.IsDeleted == false && a.AppUser.EmailConfirmed == true && a.IsMain == true, includes: new Expression<Func<UserHistoryTBL, object>>[]
            {
                                         p => p.AppUser,
                                         p => p.Country,
                                         p => p.Language,
                                         p => p.Gender,
                                         p => p.MaritalStatus,
                                         p => p.Job,
                                         p => p.Purpose,
                                         p => p.FinancialMode,
                                         p => p.Education,

            })
        .Where(u => !model.CountrySearch.HasValue || u.CountryId == model.CountrySearch.Value)
        .Where(u => string.IsNullOrWhiteSpace(model.City) || u.City.Contains(model.City))
        .Where(u => !model.IwantGender.HasValue || u.GenderId == model.IwantGender.Value)
        .Where(u => !model.AgeFrom.HasValue || CalculateAge(u.AppUser.DateOfBirth) >= model.AgeFrom.Value)
        .Where(u => !model.AgeTo.HasValue || CalculateAge(u.AppUser.DateOfBirth) <= model.AgeTo.Value)
        .Where(u => !model.UserHasImage || (u.MainImageUrl != null && u.MainImageUrl != ""))
        .Where(u => unitOfWork.UserManager.IsInRoleAsync(u.AppUser, "User").Result)
        .ToList();

            var data = Mapper.Map<List<UserHistoryTBL_VM>>(users.OrderByDescending(a => a.AppUser.DateOfJoin));
            if (users.Count() > 0)
            {
                foreach (var user in data)
                {
                    user.UserImageTBL_VM = Mapper.Map<List<UserImageTBL_VM>>(unitOfWork.UserImageRepository.GetAllCustomized(
                            filter: a => a.IsDeleted == false).OrderByDescending(a => a.CreationDate));
                }
                return data;
            }
            return new List<UserHistoryTBL_VM>();
        }

        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            int age = today.Year - birthDate.Year;

            // لو عيد ميلاده لسه مجاش السنة دي
            if (birthDate.Date > today.AddYears(-age))
                age--;

            return age;
        }

        private async Task<AppUser> GetUserByUserName(string UserName)
        {
            return await unitOfWork.UserManager.FindByNameAsync(UserName);
        }
    }
}
