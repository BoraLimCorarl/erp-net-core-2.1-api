using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using CorarlERP.Authorization.Users.Dto;
using CorarlERP.Authorization.Users.Profile.Dto;
using CorarlERP.Dto;

namespace CorarlERP.Authorization.Users.Profile
{
    public interface IProfileAppService : IApplicationService
    {
        Task<CurrentUserProfileEditDto> GetCurrentUserProfileForEdit();

        Task UpdateCurrentUserProfile(CurrentUserProfileEditDto input);
        
        Task ChangePassword(ChangePasswordInput input);

        Task UpdateProfilePicture(UpdateProfilePictureInput input);

        Task<GetPasswordComplexitySettingOutput> GetPasswordComplexitySetting();

        Task<GetProfilePictureOutput> GetProfilePicture();

        Task<GetProfilePictureOutput> GetProfilePictureById(Guid profilePictureId);

        Task<GetProfilePictureOutput> GetFriendProfilePictureById(GetFriendProfilePictureByIdInput input);

        Task ChangeLanguage(ChangeUserLanguageDto input);

        Task<UpdateGoogleAuthenticatorKeyOutput> UpdateGoogleAuthenticatorKey();

        Task SendVerificationSms();

        Task VerifySmsCode(VerifySmsCodeInputDto input);

        Task PrepareCollectedData();
        Task<UpdateUserProfileImageInput> GetUserProfileImageId();
        Task UpdateProfileImage(UpdateUserProfileImageInput input);
    }
}
