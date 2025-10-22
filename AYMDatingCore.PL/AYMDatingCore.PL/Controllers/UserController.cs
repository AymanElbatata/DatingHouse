using AutoMapper;
using AYMDatingCore.BLL.Interfaces;
using AYMDatingCore.BLL.Repositories;
using AYMDatingCore.DAL.BaseEntity;
using AYMDatingCore.DAL.Entities;
using AYMDatingCore.Helpers;
using AYMDatingCore.PL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;
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
        private readonly IHubContext<NotificationHub> _hubContext;


        public UserController(ILogger<UserController> logger, IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, IHubContext<NotificationHub> hubContext)
        {
            this.unitOfWork = unitOfWork;
            this.configuration = configuration;
            Mapper = mapper;
            _hubContext = hubContext;
        }

        public IActionResult Index(string? UserName)
        {
            return RedirectToAction("UserProfile", "Home", new { UserName =  UserName});
        }

        #region Edit Profile and Upload Images
        [HttpGet]
        public async Task<IActionResult> EditProfile(string? UserName)
        {
            var CurrentUser = await GetUserByUserName(User.Identity.Name);
            if (CurrentUser != null)
            {
                var data = GetUserHistoryByUserId(CurrentUser.Id);
                data.UserImageTBL_VM = Mapper.Map<List<UserImageTBL_VM>>(unitOfWork.UserImageRepository.GetAllCustomized(
                            filter: a => a.IsDeleted == false && a.AppUserId == CurrentUser.Id).OrderBy(a => a.CreationDate));

                data.EducationOptions = unitOfWork.EducationRepository.GetAllCustomized(
                    filter: a => a.IsDeleted == false).Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.Name }).ToList();
                data.ProfessionOptions = unitOfWork.ProfessionRepository.GetAllCustomized(
                     filter: a => a.IsDeleted == false).Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.Name }).ToList();
                data.MaritalStatusOptions = unitOfWork.MaritalStatusRepository.GetAllCustomized(
                     filter: a => a.IsDeleted == false).Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.Name }).ToList();
                data.LanguageOptions = unitOfWork.LanguageRepository.GetAllCustomized(
                     filter: a => a.IsDeleted == false).Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.Name }).ToList();
                data.PurposeOptions = unitOfWork.PurposeRepository.GetAllCustomized(
                     filter: a => a.IsDeleted == false).Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.Name }).ToList();
                data.FinancialModeOptions = unitOfWork.FinancialModeRepository.GetAllCustomized(
                     filter: a => a.IsDeleted == false).Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.Name }).ToList();

                return View(data);
            }

            return View(new UserHistoryTBL_VM());           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(UserHistoryTBL_VM model)
        {
            try
            {
                var CurrentUser = await GetUserByUserName(User.Identity.Name);

                if (ModelState.IsValid)
                {
                    var currentUserProfile = unitOfWork.UserHistoryRepository.GetAllCustomized(filter: a => a.IsDeleted == false && a.IsMain == true && a.AppUserId == CurrentUser.Id).FirstOrDefault();
                    currentUserProfile.City = model.City.Length > 50 ? model.AboutYou.Substring(0, 50) : model.City;
                    currentUserProfile.SearchAgeFrom = model.SearchAgeFrom < 18 || model.SearchAgeFrom > 99 ? 18 : model.SearchAgeFrom;
                    currentUserProfile.SearchAgeTo = model.SearchAgeTo > 99 || model.SearchAgeTo < 18 ? 99 : model.SearchAgeTo;
                    currentUserProfile.ProfileHeading = model.ProfileHeading.Length > 50 ? model.ProfileHeading.Substring(0, 50) : model.ProfileHeading;
                    currentUserProfile.AboutYou = model.AboutYou.Length > 1000 ? model.AboutYou.Substring(0, 1000) : model.AboutYou;
                    currentUserProfile.AboutPartner = model.AboutPartner.Length > 1000 ? model.AboutPartner.Substring(0, 1000) : model.AboutPartner;
                    currentUserProfile.EducationId = model.EducationId;
                    currentUserProfile.ProfessionId = model.ProfessionId;
                    currentUserProfile.MaritalStatusId = model.MaritalStatusId;
                    currentUserProfile.LanguageId = model.LanguageId;
                    currentUserProfile.PurposeId = model.PurposeId;
                    currentUserProfile.FinancialModeId = model.FinancialModeId;
                    unitOfWork.UserHistoryRepository.Update(currentUserProfile);
                    return RedirectToAction("UserProfile", "Home", new { UserName = CurrentUser.UserName });
                }
                model = GetUserHistoryByUserId(CurrentUser.Id);
                model.UserImageTBL_VM = Mapper.Map<List<UserImageTBL_VM>>(unitOfWork.UserImageRepository.GetAllCustomized(
                            filter: a => a.IsDeleted == false && a.AppUserId == CurrentUser.Id).OrderBy(a => a.CreationDate));

                model.EducationOptions = unitOfWork.EducationRepository.GetAllCustomized(
                 filter: a => a.IsDeleted == false).Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.Name }).ToList();
                model.ProfessionOptions = unitOfWork.ProfessionRepository.GetAllCustomized(
                     filter: a => a.IsDeleted == false).Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.Name }).ToList();
                model.MaritalStatusOptions = unitOfWork.MaritalStatusRepository.GetAllCustomized(
                     filter: a => a.IsDeleted == false).Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.Name }).ToList();
                model.LanguageOptions = unitOfWork.LanguageRepository.GetAllCustomized(
                     filter: a => a.IsDeleted == false).Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.Name }).ToList();
                model.PurposeOptions = unitOfWork.PurposeRepository.GetAllCustomized(
                     filter: a => a.IsDeleted == false).Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.Name }).ToList();
                model.FinancialModeOptions = unitOfWork.FinancialModeRepository.GetAllCustomized(
                     filter: a => a.IsDeleted == false).Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.Name }).ToList();
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        // Add them again (if you want to display them in summary)
                        ModelState.AddModelError(string.Empty, error.ErrorMessage);
                    }
                }
                return View(model);
            }
            catch (Exception e)
            {
                return View(new UserHistoryTBL_VM());
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file, int number, string oldImage)
        {
            if (file == null || file.Length == 0 || number == 0)
                return Json(new { success = false, message = "No file uploaded." });

            if (file.Length > 1 * 1024 * 1024)
                return Json(new { success = false, message = "Image size must be less than 1 MB." });

            var CurrentUser = await GetUserByUserName(User.Identity.Name);

            await DeleteUserImage(oldImage, number);

            var ImageName = await AddUserImage(file, CurrentUser.UserName);

            if (number == 1)
            {
                var CurrentProfile = unitOfWork.UserHistoryRepository.GetAllCustomized(filter: a => a.IsDeleted == false &&a.IsMain == true && a.AppUserId == CurrentUser.Id).FirstOrDefault();
                CurrentProfile.MainImageUrl = ImageName;
                unitOfWork.UserHistoryRepository.Update(CurrentProfile);
            }
            else if (number > 1 && number <= 6)
            {
                unitOfWork.UserImageRepository.Add(new UserImageTBL() { AppUserId = CurrentUser.Id, ImageUrl = ImageName });
            }

            return Json(new { success = true, imageUrl = ImageName });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(string imageUrl, int number)
        {
            if (string.IsNullOrEmpty(imageUrl) || number == 0)
                return Json(new { success = false, message = "No file uploaded." });

            await DeleteUserImage(imageUrl, number);

            return Json(new { success = true });
        }
        #endregion

        #region New Messages, Likes, Views, Favorites, Block
        public async Task<IActionResult> UserMessages()
        {
            var Messages_VM = new Messages_VM();

            var currentUser = await GetUserByUserName(User.Identity.Name);
            var allNewMessages = unitOfWork.UserMessageRepository.GetAllCustomized(filter: a => a.IsDeleted == false && a.IsDeletedFromReceiver == false && a.ReceiverAppUserId == currentUser.Id).OrderByDescending(a=>a.CreationDate).ToList();
            foreach (var item in allNewMessages)
            {
                if (!Messages_VM.UserHistoryTBL_VM.Where(a => a.AppUserId == item.SenderAppUserId).Any())
                {
                    Messages_VM.UserHistoryTBL_VM.Add(GetUserHistoryByUserId(item.SenderAppUserId));
                    Messages_VM.MessageCounterDTO.Add(new DTO.MessageCounterDTO { AppUserId = item.SenderAppUserId, Counter = allNewMessages.Where(a=>a.IsSeen == false && a.SenderAppUserId == item.SenderAppUserId).Count(), LatestMessageDate = item.CreationDate });
                }

                //if (!Messages_VM.MessageCounterDTO.Where(a=>a.AppUserId == item.SenderAppUserId).Any())
                //{
                //    Messages_VM.MessageCounterDTO.Add(new DTO.MessageCounterDTO { AppUserId = item.SenderAppUserId, Counter =1 });
                //    Messages_VM.MessageCounterDTO.Where(a => a.AppUserId == item.SenderAppUserId).FirstOrDefault().LatestMessageDate = item.CreationDate;
                //}
                //else
                //{
                //    Messages_VM.MessageCounterDTO.Where(a => a.AppUserId == item.SenderAppUserId).FirstOrDefault().Counter++;
                //    Messages_VM.MessageCounterDTO.Where(a => a.AppUserId == item.SenderAppUserId).FirstOrDefault().LatestMessageDate = item.CreationDate;
                //}
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

        [HttpPost]
        public async Task<IActionResult> AddRemoveNewLikeFavoriteBlock(string receiverUserName, int operation)
        {
            var SenderUser = await GetUserByUserName(User.Identity.Name);
            var RecieverUser = await GetUserByUserName(receiverUserName);

            switch (operation)
            {
                case 1:
                    var ExistingRecord1 = unitOfWork.UserLikeRepository.GetAllCustomized(filter: a => a.IsDeleted == false &&
                    a.SenderAppUserId == SenderUser.Id && a.ReceiverAppUserId == RecieverUser.Id).FirstOrDefault();
                    if (ExistingRecord1 != null)
                    {
                        ExistingRecord1.IsDeleted = true;
                        unitOfWork.UserLikeRepository.Update(ExistingRecord1);
                    }
                    else
                    {
                        unitOfWork.UserLikeRepository.Add(new DAL.Entities.UserLikeTBL
                        {
                            SenderAppUserId = SenderUser.Id,
                            ReceiverAppUserId = RecieverUser.Id
                        });
                    }
                    await _hubContext.Clients.User(RecieverUser.Id).SendAsync("ReceiveLikeNotification", unitOfWork.UserLikeRepository.GetAllCustomized(filter: a => a.IsDeleted == false && a.IsSeen == false && a.ReceiverAppUserId == RecieverUser.Id).Count());
                    break;

                case 2:
                    var ExistingRecord2 = unitOfWork.UserFavoriteRepository.GetAllCustomized(filter: a => a.IsDeleted == false &&
                    a.SenderAppUserId == SenderUser.Id && a.ReceiverAppUserId == RecieverUser.Id).FirstOrDefault();
                    if (ExistingRecord2 != null)
                    {
                        ExistingRecord2.IsDeleted = true;
                        unitOfWork.UserFavoriteRepository.Update(ExistingRecord2);
                    }
                    else
                    {
                        unitOfWork.UserFavoriteRepository.Add(new DAL.Entities.UserFavoriteTBL
                        {
                            SenderAppUserId = SenderUser.Id,
                            ReceiverAppUserId = RecieverUser.Id
                        });
                    }
                    await _hubContext.Clients.User(RecieverUser.Id).SendAsync("ReceiveFavoriteNotification", unitOfWork.UserFavoriteRepository.GetAllCustomized(filter: a => a.IsDeleted == false && a.IsSeen == false && a.ReceiverAppUserId == RecieverUser.Id).Count());
                    break;

                case 3:
                    var ExistingRecord3 = unitOfWork.UserBlockRepository.GetAllCustomized(filter: a => a.IsDeleted == false &&
                    a.SenderAppUserId == SenderUser.Id && a.ReceiverAppUserId == RecieverUser.Id).FirstOrDefault();
                    if (ExistingRecord3 != null)
                    {
                        ExistingRecord3.IsDeleted = true;
                        unitOfWork.UserBlockRepository.Update(ExistingRecord3);
                    }
                    else
                    {
                        unitOfWork.UserBlockRepository.Add(new DAL.Entities.UserBlockTBL
                        {
                            SenderAppUserId = SenderUser.Id,
                            ReceiverAppUserId = RecieverUser.Id
                        });
                    }
                    await _hubContext.Clients.User(RecieverUser.Id).SendAsync("ReceiveBlockNotification", unitOfWork.UserBlockRepository.GetAllCustomized(filter: a => a.IsDeleted == false && a.IsSeen == false && a.ReceiverAppUserId == RecieverUser.Id).Count());
                    break;

                default:
                    // Optional: handle unexpected operation values
                    break;
            }

            return Json(new { success = true });
        }
        #endregion

        #region Chat
        public async Task<IActionResult> Chat(string? RecieverUserName)
        {
            if (RecieverUserName == User.Identity.Name)
                return RedirectToAction("UserProfile", "Home", new { UserName = RecieverUserName });

            var SenderUser = await GetUserByUserName(User.Identity.Name);
            var ReceiverUser = await GetUserByUserName(RecieverUserName);

            var UserMessageGroupExisting = unitOfWork.UserMessageGroupRepository.GetAllCustomized(filter: a => a.IsDeleted == false && (a.SenderAppUserId == SenderUser.Id && a.ReceiverAppUserId == ReceiverUser.Id) || (a.SenderAppUserId == ReceiverUser.Id && a.ReceiverAppUserId == SenderUser.Id)).Any();

            if (!UserMessageGroupExisting)
                unitOfWork.UserMessageGroupRepository.Add(new UserMessageGroupTBL { SenderAppUserId = SenderUser.Id, ReceiverAppUserId = ReceiverUser.Id, NameGuid = Guid.NewGuid().ToString() });
            var UserMessageGroup = unitOfWork.UserMessageGroupRepository.GetAllCustomized(filter: a => (a.IsDeleted == false && a.SenderAppUserId == SenderUser.Id && a.ReceiverAppUserId == ReceiverUser.Id) || (a.IsDeleted == false && a.SenderAppUserId == ReceiverUser.Id && a.ReceiverAppUserId == SenderUser.Id)).FirstOrDefault();

            var data = new Chat_VM();
            data.Users_VM.Add(new UserChat_VM() { Id = SenderUser.Id, UserName = SenderUser.UserName});
            data.Users_VM.Add(new UserChat_VM() { Id = ReceiverUser.Id, UserName = ReceiverUser.UserName });
            data.SenderAppUser = SenderUser;
            data.ReceiverAppUser = ReceiverUser;
            data.GroupName = UserMessageGroup.NameGuid;
            data.UserMessage_VM = Mapper.Map<List<UserMessage_VM>>(unitOfWork.UserMessageRepository.GetAllCustomized(filter: a => a.IsDeleted == false && a.IsDeletedFromSender == false && (a.SenderAppUserId == SenderUser.Id && a.ReceiverAppUserId == ReceiverUser.Id) || (a.SenderAppUserId == ReceiverUser.Id && a.ReceiverAppUserId == SenderUser.Id)).OrderBy(a => a.CreationDate).ToList());

            var RecieverUserProfile = GetUserHistoryByUserId(ReceiverUser.Id);

            data.UserHistoryTBL_VM = Mapper.Map<UserHistoryTBL_VM>(RecieverUserProfile);
            data.IsThereBlocking = unitOfWork.UserBlockRepository.GetAllCustomized(filter: a => (a.IsDeleted == false && a.SenderAppUserId == SenderUser.Id && a.ReceiverAppUserId == ReceiverUser.Id) || (a.IsDeleted == false && a.SenderAppUserId == ReceiverUser.Id && a.ReceiverAppUserId == SenderUser.Id)).Any();

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
        #endregion

        #region Helper Methods
        [HttpPost]
        public async Task<IActionResult> SaveMessageinChat(string receiverUserName, string message)
        {
            var SenderUser = await GetUserByUserName(User.Identity.Name);
            var RecieverUser = await GetUserByUserName(receiverUserName);
            bool IsThereBlocking = unitOfWork.UserBlockRepository.GetAllCustomized(filter: a => (a.IsDeleted == false && a.SenderAppUserId == SenderUser.Id && a.ReceiverAppUserId == RecieverUser.Id) || (a.IsDeleted == false && a.SenderAppUserId == RecieverUser.Id && a.ReceiverAppUserId == SenderUser.Id)).Any();
            if (!IsThereBlocking)
            {
                unitOfWork.UserMessageRepository.Add(new DAL.Entities.UserMessageTBL() { SenderAppUserId = SenderUser.Id, ReceiverAppUserId = RecieverUser.Id, Message = message });
                await _hubContext.Clients.User(RecieverUser.Id).SendAsync("ReceiveMessageNotification", unitOfWork.UserMessageRepository.GetAllCustomized(filter: a => a.IsDeleted == false && a.IsSeen == false && a.ReceiverAppUserId == RecieverUser.Id).Count());
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> SaveAudioMessageInChat(string receiverUserName, string base64Audio)
        {
            if (base64Audio == null || base64Audio.Length == 0)
                return Json(new { success = false, message = "No file uploaded." });

            var SenderUser = await GetUserByUserName(User.Identity.Name);
            var RecieverUser = await GetUserByUserName(receiverUserName);
            bool IsThereBlocking = unitOfWork.UserBlockRepository.GetAllCustomized(filter: a => (a.IsDeleted == false && a.SenderAppUserId == SenderUser.Id && a.ReceiverAppUserId == RecieverUser.Id) || (a.IsDeleted == false && a.SenderAppUserId == RecieverUser.Id && a.ReceiverAppUserId == SenderUser.Id)).Any();
            if (!IsThereBlocking)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "AudioUsers");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                // 🎵 Generate unique filename
                string fileName = SenderUser.UserName+"_"+ RecieverUser.UserName+"-"+ $"{Guid.NewGuid()}.webm";
                string filePath = Path.Combine(folderPath, fileName);

                // 🎧 Convert base64 string to bytes and save file
                byte[] audioBytes = Convert.FromBase64String(base64Audio);
                System.IO.File.WriteAllBytes(filePath, audioBytes);
                unitOfWork.UserMessageRepository.Add(new DAL.Entities.UserMessageTBL() { SenderAppUserId = SenderUser.Id, ReceiverAppUserId = RecieverUser.Id, AudioDataUrl = fileName });
                await _hubContext.Clients.User(RecieverUser.Id).SendAsync("ReceiveMessageNotification", unitOfWork.UserMessageRepository.GetAllCustomized(filter: a => a.IsDeleted == false && a.IsSeen == false && a.ReceiverAppUserId == RecieverUser.Id).Count());
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> SaveFileMessageInChat(string receiverUserName, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Json(new { success = false, message = "No file uploaded." });

            var SenderUser = await GetUserByUserName(User.Identity.Name);
            var RecieverUser = await GetUserByUserName(receiverUserName);
            bool IsThereBlocking = unitOfWork.UserBlockRepository.GetAllCustomized(filter: a => (a.IsDeleted == false && a.SenderAppUserId == SenderUser.Id && a.ReceiverAppUserId == RecieverUser.Id) || (a.IsDeleted == false && a.SenderAppUserId == RecieverUser.Id && a.ReceiverAppUserId == SenderUser.Id)).Any();
            if (!IsThereBlocking)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "FileUsers");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                // 🎵 Generate unique filename
                string fileName = SenderUser.UserName + "_" + RecieverUser.UserName + "-" + $"{Guid.NewGuid()}"+ Path.GetExtension(file.FileName);
                string filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                unitOfWork.UserMessageRepository.Add(new DAL.Entities.UserMessageTBL() { SenderAppUserId = SenderUser.Id, ReceiverAppUserId = RecieverUser.Id, FileDataUrl = fileName });
                await _hubContext.Clients.User(RecieverUser.Id).SendAsync("ReceiveMessageNotification", unitOfWork.UserMessageRepository.GetAllCustomized(filter: a => a.IsDeleted == false && a.IsSeen == false && a.ReceiverAppUserId == RecieverUser.Id).Count());
                //return Json(new { success = true, fileUrl = $"/FileUsers/{file.FileName}" });
                return Json(new { success = true, fileUrl = fileName });
            }

            return Json(new { success = false });
        }

        private async Task DeleteUserImage(string oldImage, int number)
        {
            if (oldImage == "blankprofile973460.png")
                return;

            var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImageUsers", oldImage);

            if (System.IO.File.Exists(oldImagePath))
            {
                try
                {
                    //System.IO.File.Delete(oldImagePath);
                    var CurrentUser = await GetUserByUserName(User.Identity.Name);

                    if (number > 1)
                    {
                        var OldImageRow = unitOfWork.UserImageRepository.GetAllCustomized(filter: a => a.ImageUrl == oldImage && a.IsDeleted == false && a.AppUserId == CurrentUser.Id).FirstOrDefault();
                        if (OldImageRow != null)
                        {
                            OldImageRow.IsDeleted = true;
                            unitOfWork.UserImageRepository.Update(OldImageRow);
                        }
                    }
                    //else if (number == 1) {
                    //    var currentUserProfile = unitOfWork.UserHistoryRepository.GetAllCustomized(filter: a => a.IsDeleted == false && a.IsMain == true && a.AppUserId == CurrentUser.Id).FirstOrDefault();
                    //    currentUserProfile.MainImageUrl = null;
                    //    unitOfWork.UserHistoryRepository.Update(currentUserProfile);
                    //}
                }
                catch (Exception ex)
                {
                    Console.WriteLine("⚠️ Error deleting old image: " + ex.Message);
                }
            }
        }

        private async Task<string> AddUserImage(IFormFile file, string UserName)
        {
            try
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImageUsers");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = UserName + "_" + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                return fileName;
            }
            catch (Exception e)
            {
                return string.Empty;
            }

        }

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
                                         p => p.Profession,
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
        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
