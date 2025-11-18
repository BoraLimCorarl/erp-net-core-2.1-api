using System;
using System.Collections.Generic;
using CorarlERP.Galleries;

namespace CorarlERP
{
    public class CorarlERPConsts
    {
        public const string LocalizationSourceName = "CorarlERP";

        public const string ConnectionStringName = "Default";

        public const bool MultiTenancyEnabled = true;

        public const int PaymentCacheDurationInMinutes = 30;

        public const string ItemTypeAsset = "Asset";

        public const int MaxNameLength = 512;
        public const int MaxContentTypeLength = 100;
        public const int MaxFilePathLength = 256;
        public const int MaxFileName = 256;

        public const int MaxGalleryFileSize = 10485760; //10MB limit for us
        public const string DocumentFolder = "Documents";
        public static string BuildTenantFolderName(int? tenantId) => tenantId.HasValue ? $"Tenant_{tenantId}" : "Host";

        public const int DelayMinutes = 1;


        /// <summary>
        /// Default page size for paged requests.
        /// </summary>
        public const int DefaultPageSize = 25;

        /// <summary>
        /// Maximum allowed page size for paged requests.
        /// </summary>
        public const int MaxPageSize = 1000;

        /// <summary>
        /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
        /// </summary>
        public const string DefaultPassPhrase = "gsKxGZ012HLL3MI5";

        public const int ResizedMaxProfilPictureBytesUserFriendlyValue = 1024;

        public const int MaxProfilPictureBytesUserFriendlyValue = 5;

        public const string TokenValidityKey = "token_validity_key";

        public static string UserIdentifier = "user_identifier";

    }
}