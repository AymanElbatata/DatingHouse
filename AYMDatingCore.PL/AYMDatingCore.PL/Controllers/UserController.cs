using AutoMapper;
using AYMDatingCore.BLL.Interfaces;
using AYMDatingCore.DAL.BaseEntity;
using AYMDatingCore.DAL.Entities;
using AYMDatingCore.PL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AYMDatingCore.PL.Controllers
{
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;

        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper Mapper;
        private readonly IConfiguration configuration;

        public UserController(ILogger<UserController> logger, IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            this.unitOfWork = unitOfWork;
            this.configuration = configuration;
            Mapper = mapper;
        }

        public IActionResult Index(string? UserName)
        {
            return RedirectToAction("UserProfile", "Home", new { UserName =  UserName});
        }

        #region New Messages, Likes, Views, Favorites, Block
        public async Task<IActionResult> UserMessages()
        {
            var Messages_VM = new Messages_VM();

            var currentUser = await GetUserByUserName(User.Identity.Name);
            var allNewMessages = unitOfWork.UserMessageRepository.GetAllCustomized(filter: a => a.IsDeleted == false && a.IsDeletedFromReceiver == false && a.ReceiverAppUserId == currentUser.Id).OrderBy(a=>a.CreationDate).ToList();
            foreach (var item in allNewMessages)
            {
                if (!Messages_VM.UserHistoryTBL_VM.Where(a => a.AppUserId == item.SenderAppUserId).Any())
                {
                    Messages_VM.UserHistoryTBL_VM.Add(GetUserHistoryByUserId(item.SenderAppUserId));
                }

                if (!Messages_VM.MessageCounterDTO.Where(a=>a.AppUserId == item.SenderAppUserId).Any())
                {
                    Messages_VM.MessageCounterDTO.Add(new DTO.MessageCounterDTO { AppUserId = item.SenderAppUserId, Counter =1 });
                    Messages_VM.MessageCounterDTO.Where(a => a.AppUserId == item.SenderAppUserId).FirstOrDefault().LatestMessageDate = item.CreationDate;
                }
                else
                {
                    Messages_VM.MessageCounterDTO.Where(a => a.AppUserId == item.SenderAppUserId).FirstOrDefault().Counter++;
                    Messages_VM.MessageCounterDTO.Where(a => a.AppUserId == item.SenderAppUserId).FirstOrDefault().LatestMessageDate = item.CreationDate;
                }
            }
            Messages_VM.UserHistoryTBL_VM.OrderByDescending(a => a.CreationDate);
            return View(Messages_VM);
        }

        public async Task<IActionResult> UserViews()
        {
            var Views_VM = new Views_Likes_Favorite_Block_VM();

            var currentUser = await GetUserByUserName(User.Identity.Name);
            var allNewViews = unitOfWork.UserViewRepository.GetAllCustomized(filter: a => a.IsDeleted == false && a.ReceiverAppUserId == currentUser.Id).ToList();
            foreach (var item in allNewViews)
            {
                Views_VM.UserHistoryTBL_VM.Add(GetUserHistoryByUserId(item.SenderAppUserId));
                item.IsSeen = true;
                unitOfWork.UserViewRepository.Update(item);
            }
            Views_VM.UserHistoryTBL_VM.OrderByDescending(a => a.CreationDate);
            return View(Views_VM);
        }

        public async Task<IActionResult> UserLikes()
        {
            var Views_VM = new Views_Likes_Favorite_Block_VM();

            var currentUser = await GetUserByUserName(User.Identity.Name);
            var allNewLikes = unitOfWork.UserLikeRepository.GetAllCustomized(filter: a => a.IsDeleted == false && a.ReceiverAppUserId == currentUser.Id).ToList();
            foreach (var item in allNewLikes)
            {
                Views_VM.UserHistoryTBL_VM.Add(GetUserHistoryByUserId(item.SenderAppUserId));
                item.IsSeen = true;
                unitOfWork.UserLikeRepository.Update(item);
            }
            Views_VM.UserHistoryTBL_VM.OrderByDescending(a => a.CreationDate);
            return View(Views_VM);
        }

        public async Task<IActionResult> UserFavorites()
        {
            var Views_VM = new Views_Likes_Favorite_Block_VM();

            var currentUser = await GetUserByUserName(User.Identity.Name);
            var allNewFavorites = unitOfWork.UserFavoriteRepository.GetAllCustomized(filter: a => a.IsDeleted == false && a.ReceiverAppUserId == currentUser.Id).ToList();
            foreach (var item in allNewFavorites)
            {
                Views_VM.UserHistoryTBL_VM.Add(GetUserHistoryByUserId(item.SenderAppUserId));
                item.IsSeen = true;
                unitOfWork.UserFavoriteRepository.Update(item);
            }
            Views_VM.UserHistoryTBL_VM.OrderByDescending(a => a.CreationDate);
            return View(Views_VM);
        }

        public async Task<IActionResult> UserBlocks()
        {
            var Views_VM = new Views_Likes_Favorite_Block_VM();

            var currentUser = await GetUserByUserName(User.Identity.Name);
            var allNewBlocks = unitOfWork.UserBlockRepository.GetAllCustomized(filter: a => a.IsDeleted == false && a.ReceiverAppUserId == currentUser.Id).ToList();
            foreach (var item in allNewBlocks)
            {
                Views_VM.UserHistoryTBL_VM.Add(GetUserHistoryByUserId(item.SenderAppUserId));
                item.IsSeen = true;
                unitOfWork.UserBlockRepository.Update(item);
            }
            Views_VM.UserHistoryTBL_VM.OrderByDescending(a => a.CreationDate);
            return View(Views_VM);
        }

        #endregion

        #region Chat
        public async Task<IActionResult> Chat(string? RecieverUserName)
        {
            var SenderUser = await GetUserByUserName(User.Identity.Name);
            var ReceiverUser = await GetUserByUserName(RecieverUserName);

            var UserMessageGroupExisting = unitOfWork.UserMessageGroupRepository.GetAllCustomized(filter: a => a.IsDeleted == false && (a.SenderAppUserId == SenderUser.Id && a.ReceiverAppUserId == ReceiverUser.Id) || (a.SenderAppUserId == ReceiverUser.Id && a.ReceiverAppUserId == SenderUser.Id)).Any();

            if (!UserMessageGroupExisting)
                unitOfWork.UserMessageGroupRepository.Add(new UserMessageGroupTBL { SenderAppUserId = SenderUser.Id, ReceiverAppUserId = ReceiverUser.Id, NameGuid = Guid.NewGuid().ToString() });
            var UserMessageGroup = unitOfWork.UserMessageGroupRepository.GetAllCustomized(filter: a => a.IsDeleted == false && (a.SenderAppUserId == SenderUser.Id && a.ReceiverAppUserId == ReceiverUser.Id) || (a.SenderAppUserId == ReceiverUser.Id && a.ReceiverAppUserId == SenderUser.Id)).FirstOrDefault();

            var data = new Chat_VM();
            data.SenderAppUser = SenderUser;
            data.ReceiverAppUser = ReceiverUser;
            data.GroupName = UserMessageGroup.NameGuid;
            data.UserMessage_VM = Mapper.Map<List<UserMessage_VM>>(unitOfWork.UserMessageRepository.GetAllCustomized(filter: a => a.IsDeleted == false && a.IsDeletedFromSender == false && (a.SenderAppUserId == SenderUser.Id && a.ReceiverAppUserId == ReceiverUser.Id) || (a.SenderAppUserId == ReceiverUser.Id && a.ReceiverAppUserId == SenderUser.Id)).OrderByDescending(a => a.CreationDate).ToList());

            var RecieverUserProfile = unitOfWork.UserHistoryRepository.GetAllCustomized(filter: a => a.IsDeleted == false && a.IsMain == true && a.AppUserId == ReceiverUser.Id, includes: new Expression<Func<UserHistoryTBL, object>>[]
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
            data.UserHistoryTBL_VM = Mapper.Map<UserHistoryTBL_VM>(RecieverUserProfile);
            data.IsThereBlocking = unitOfWork.UserBlockRepository.GetAllCustomized(filter: a => a.IsDeleted == false && (a.SenderAppUserId == SenderUser.Id && a.ReceiverAppUserId == ReceiverUser.Id) || (a.SenderAppUserId == ReceiverUser.Id || a.ReceiverAppUserId == SenderUser.Id)).Any();

            // Seen Old Messages
            var allNewMessages = unitOfWork.UserMessageRepository.GetAllCustomized(filter: a => a.IsDeleted == false && a.IsDeletedFromSender == false && a.IsSeen == false && a.ReceiverAppUserId == SenderUser.Id && a.SenderAppUserId == ReceiverUser.Id).ToList();
            foreach (var item in allNewMessages)
            {
                item.IsSeen = true;
                unitOfWork.UserMessageRepository.Update(item);
            }
            //
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> SaveMessageinChat(string receiverUserName, string message)
        {
            var SenderUser = await GetUserByUserName(User.Identity.Name);
            var RecieverUser = await GetUserByUserName(receiverUserName);

            unitOfWork.UserMessageRepository.Add(new DAL.Entities.UserMessageTBL() { SenderAppUserId = SenderUser.Id, ReceiverAppUserId = RecieverUser.Id, Message = message });

            return Json(new { success = true });
        }
        #endregion

        private async Task<AppUser> GetUserByUserName(string UserName)
        {
            return await unitOfWork.UserManager.FindByNameAsync(UserName);
        }
        private UserHistoryTBL_VM GetUserHistoryByUserId(string? userId)
        {
            var currentUser = unitOfWork.UserHistoryRepository.GetAllCustomized(filter: a => a.IsDeleted == false && a.IsMain == true && a.AppUserId == userId, includes: new Expression<Func<UserHistoryTBL, object>>[]
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
                return Mapper.Map<UserHistoryTBL_VM>(currentUser);
            }
            return new UserHistoryTBL_VM();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
