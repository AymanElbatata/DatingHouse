﻿using AYMDatingCore.BLL.Interfaces;
using AYMDatingCore.DAL.BaseEntity;
using AYMDatingCore.DAL.IRepositories;
using Microsoft.AspNetCore.Identity;

namespace AYMDatingCore.BLL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public IGenderTBLRepository GenderTBLRepository { get; }
        public ICountryTBLRepository CountryTBLRepository { get; }
        public IEmailTBLRepository EmailTBLRepository { get; }
        public IAppErrorTBLRepository AppErrorTBLRepository { get; }
        public SignInManager<AppUser> SignInManager { get; }
        public RoleManager<AppRole> RoleManager { get; }
        public UserManager<AppUser> UserManager { get; }
        public IMySPECIALGUID MySPECIALGUID { get; }

        public IEducationRepository EducationRepository { get; }
        public IFinancialModeRepository FinancialModeRepository { get; }
        public ILanguageRepository LanguageRepository { get; }
        public IProfessionRepository ProfessionRepository { get; }
        public IPurposeRepository PurposeRepository { get; }
        public IUserBlockRepository UserBlockRepository { get; }
        public IUserFavoriteRepository UserFavoriteRepository { get; }
        public IUserHistoryRepository UserHistoryRepository { get; }
        public IUserImageRepository UserImageRepository { get; }
        public IUserLikeRepository UserLikeRepository { get; }
        public IUserMessageGroupRepository UserMessageGroupRepository { get; }
        public IUserMessageRepository UserMessageRepository { get; }
        public IUserReportRepository UserReportRepository { get; }
        public IUserViewRepository UserViewRepository { get; }


        public UnitOfWork( IGenderTBLRepository GenderTBLRepository
            ,ICountryTBLRepository CountryTBLRepository,
            IEmailTBLRepository EmailTBLRepository,
            IAppErrorTBLRepository AppErrorTBLRepository,
            SignInManager<AppUser> SignInManager,
            RoleManager<AppRole> RoleManager, UserManager<AppUser> UserManager,
            IMySPECIALGUID MySPECIALGUID, IEducationRepository EducationRepository,
            IFinancialModeRepository FinancialModeRepository, ILanguageRepository LanguageRepository,
            IProfessionRepository ProfessionRepository, IPurposeRepository PurposeRepository,
            IUserFavoriteRepository UserFavoriteRepository, IUserBlockRepository UserBlockRepository,
            IUserHistoryRepository UserHistoryRepository, IUserImageRepository UserImageRepository,
            IUserLikeRepository UserLikeRepository, IUserMessageGroupRepository UserMessageGroupRepository,
            IUserMessageRepository UserMessageRepository, IUserReportRepository UserReportRepository,
            IUserViewRepository UserViewRepository

            )
        {
            this.CountryTBLRepository = CountryTBLRepository;
            this.GenderTBLRepository = GenderTBLRepository;
            this.EmailTBLRepository = EmailTBLRepository;
            this.AppErrorTBLRepository = AppErrorTBLRepository;
            this.SignInManager = SignInManager;
            this.RoleManager = RoleManager;
            this.UserManager = UserManager;
            this.MySPECIALGUID = MySPECIALGUID;

            this.UserBlockRepository = UserBlockRepository;
            this.UserLikeRepository = UserLikeRepository;
            this.UserMessageRepository = UserMessageRepository;
            this.UserReportRepository = UserReportRepository;
            this.UserViewRepository = UserViewRepository;
            this.LanguageRepository = LanguageRepository;
            this.ProfessionRepository = ProfessionRepository;
            this.UserImageRepository = UserImageRepository;
            this.UserMessageGroupRepository = UserMessageGroupRepository;
            this.FinancialModeRepository = FinancialModeRepository;
            this.EducationRepository = EducationRepository;
            this.PurposeRepository = PurposeRepository;
            this.UserHistoryRepository = UserHistoryRepository;
            this.UserFavoriteRepository = UserFavoriteRepository;
        }
    }
}


