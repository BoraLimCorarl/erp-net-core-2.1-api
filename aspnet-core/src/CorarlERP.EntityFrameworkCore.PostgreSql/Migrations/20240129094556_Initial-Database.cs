using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CorarlERP.Migrations
{
    public partial class InitialDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AbpAuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    TenantId = table.Column<int>(nullable: true),
                    UserId = table.Column<long>(nullable: true),
                    ServiceName = table.Column<string>(maxLength: 256, nullable: true),
                    MethodName = table.Column<string>(maxLength: 256, nullable: true),
                    Parameters = table.Column<string>(maxLength: 1024, nullable: true),
                    ExecutionTime = table.Column<DateTime>(nullable: false),
                    ExecutionDuration = table.Column<int>(nullable: false),
                    ClientIpAddress = table.Column<string>(maxLength: 64, nullable: true),
                    ClientName = table.Column<string>(maxLength: 128, nullable: true),
                    BrowserInfo = table.Column<string>(maxLength: 512, nullable: true),
                    Exception = table.Column<string>(maxLength: 2000, nullable: true),
                    ImpersonatorUserId = table.Column<long>(nullable: true),
                    ImpersonatorTenantId = table.Column<int>(nullable: true),
                    CustomData = table.Column<string>(maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpAuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpBackgroundJobs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    JobType = table.Column<string>(maxLength: 512, nullable: false),
                    JobArgs = table.Column<string>(maxLength: 1048576, nullable: false),
                    TryCount = table.Column<short>(nullable: false),
                    NextTryTime = table.Column<DateTime>(nullable: false),
                    LastTryTime = table.Column<DateTime>(nullable: true),
                    IsAbandoned = table.Column<bool>(nullable: false),
                    Priority = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpBackgroundJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpEditions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 32, nullable: false),
                    DisplayName = table.Column<string>(maxLength: 64, nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    ExpiringEditionId = table.Column<int>(nullable: true),
                    MonthlyPrice = table.Column<decimal>(nullable: true),
                    AnnualPrice = table.Column<decimal>(nullable: true),
                    TrialDayCount = table.Column<int>(nullable: true),
                    DailyPrice = table.Column<decimal>(nullable: true),
                    IsPaid = table.Column<bool>(nullable: true, defaultValue: false),
                    WaitingDayAfterExpire = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpEditions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpEntityChangeSets",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    BrowserInfo = table.Column<string>(maxLength: 512, nullable: true),
                    ClientIpAddress = table.Column<string>(maxLength: 64, nullable: true),
                    ClientName = table.Column<string>(maxLength: 128, nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    ExtensionData = table.Column<string>(nullable: true),
                    ImpersonatorTenantId = table.Column<int>(nullable: true),
                    ImpersonatorUserId = table.Column<long>(nullable: true),
                    Reason = table.Column<string>(maxLength: 256, nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    UserId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpEntityChangeSets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpLanguages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 10, nullable: false),
                    DisplayName = table.Column<string>(maxLength: 64, nullable: false),
                    Icon = table.Column<string>(maxLength: 128, nullable: true),
                    IsDisabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpLanguages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpLanguageTexts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    LanguageName = table.Column<string>(maxLength: 10, nullable: false),
                    Source = table.Column<string>(maxLength: 128, nullable: false),
                    Key = table.Column<string>(maxLength: 256, nullable: false),
                    Value = table.Column<string>(maxLength: 67108864, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpLanguageTexts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    NotificationName = table.Column<string>(maxLength: 96, nullable: false),
                    Data = table.Column<string>(maxLength: 1048576, nullable: true),
                    DataTypeName = table.Column<string>(maxLength: 512, nullable: true),
                    EntityTypeName = table.Column<string>(maxLength: 250, nullable: true),
                    EntityTypeAssemblyQualifiedName = table.Column<string>(maxLength: 512, nullable: true),
                    EntityId = table.Column<string>(maxLength: 96, nullable: true),
                    Severity = table.Column<byte>(nullable: false),
                    UserIds = table.Column<string>(maxLength: 131072, nullable: true),
                    ExcludedUserIds = table.Column<string>(maxLength: 131072, nullable: true),
                    TenantIds = table.Column<string>(maxLength: 131072, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpNotificationSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    UserId = table.Column<long>(nullable: false),
                    NotificationName = table.Column<string>(maxLength: 96, nullable: true),
                    EntityTypeName = table.Column<string>(maxLength: 250, nullable: true),
                    EntityTypeAssemblyQualifiedName = table.Column<string>(maxLength: 512, nullable: true),
                    EntityId = table.Column<string>(maxLength: 96, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpNotificationSubscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpOrganizationUnits",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ParentId = table.Column<long>(nullable: true),
                    Code = table.Column<string>(maxLength: 95, nullable: false),
                    DisplayName = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpOrganizationUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpOrganizationUnits_AbpOrganizationUnits_ParentId",
                        column: x => x.ParentId,
                        principalTable: "AbpOrganizationUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AbpPersistedGrants",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 200, nullable: false),
                    Type = table.Column<string>(maxLength: 50, nullable: false),
                    SubjectId = table.Column<string>(maxLength: 200, nullable: true),
                    ClientId = table.Column<string>(maxLength: 200, nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Expiration = table.Column<DateTime>(nullable: true),
                    Data = table.Column<string>(maxLength: 50000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpPersistedGrants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpTenantNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    NotificationName = table.Column<string>(maxLength: 96, nullable: false),
                    Data = table.Column<string>(maxLength: 1048576, nullable: true),
                    DataTypeName = table.Column<string>(maxLength: 512, nullable: true),
                    EntityTypeName = table.Column<string>(maxLength: 250, nullable: true),
                    EntityTypeAssemblyQualifiedName = table.Column<string>(maxLength: 512, nullable: true),
                    EntityId = table.Column<string>(maxLength: 96, nullable: true),
                    Severity = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpTenantNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserAccounts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    UserId = table.Column<long>(nullable: false),
                    UserLinkId = table.Column<long>(nullable: true),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    EmailAddress = table.Column<string>(maxLength: 256, nullable: true),
                    LastLoginTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserLoginAttempts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    TenantId = table.Column<int>(nullable: true),
                    TenancyName = table.Column<string>(maxLength: 64, nullable: true),
                    UserId = table.Column<long>(nullable: true),
                    UserNameOrEmailAddress = table.Column<string>(maxLength: 255, nullable: true),
                    ClientIpAddress = table.Column<string>(maxLength: 64, nullable: true),
                    ClientName = table.Column<string>(maxLength: 128, nullable: true),
                    BrowserInfo = table.Column<string>(maxLength: 512, nullable: true),
                    Result = table.Column<byte>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserLoginAttempts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    UserId = table.Column<long>(nullable: false),
                    TenantNotificationId = table.Column<Guid>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserOrganizationUnits",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    UserId = table.Column<long>(nullable: false),
                    OrganizationUnitId = table.Column<long>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserOrganizationUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpUsers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    AuthenticationSource = table.Column<string>(maxLength: 64, nullable: true),
                    UserName = table.Column<string>(maxLength: 256, nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    EmailAddress = table.Column<string>(maxLength: 256, nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    Surname = table.Column<string>(maxLength: 64, nullable: false),
                    Password = table.Column<string>(maxLength: 128, nullable: false),
                    EmailConfirmationCode = table.Column<string>(maxLength: 328, nullable: true),
                    PasswordResetCode = table.Column<string>(maxLength: 328, nullable: true),
                    LockoutEndDateUtc = table.Column<DateTime>(nullable: true),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    IsLockoutEnabled = table.Column<bool>(nullable: false),
                    PhoneNumber = table.Column<string>(maxLength: 32, nullable: true),
                    IsPhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    SecurityStamp = table.Column<string>(maxLength: 128, nullable: true),
                    IsTwoFactorEnabled = table.Column<bool>(nullable: false),
                    IsEmailConfirmed = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    LastLoginTime = table.Column<DateTime>(nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: false),
                    NormalizedEmailAddress = table.Column<string>(maxLength: 256, nullable: false),
                    ConcurrencyStamp = table.Column<string>(maxLength: 128, nullable: true),
                    ProfilePictureId = table.Column<Guid>(nullable: true),
                    ShouldChangePasswordOnNextLogin = table.Column<bool>(nullable: false),
                    SignInTokenExpireTimeUtc = table.Column<DateTime>(nullable: true),
                    SignInToken = table.Column<string>(nullable: true),
                    GoogleAuthenticatorKey = table.Column<string>(nullable: true),
                    Deactivate = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpUsers_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpUsers_AbpUsers_DeleterUserId",
                        column: x => x.DeleterUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpUsers_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppBinaryObjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    Bytes = table.Column<byte[]>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppBinaryObjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppChatMessages",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    UserId = table.Column<long>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    TargetUserId = table.Column<long>(nullable: false),
                    TargetTenantId = table.Column<int>(nullable: true),
                    Message = table.Column<string>(maxLength: 4096, nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Side = table.Column<int>(nullable: false),
                    ReadState = table.Column<int>(nullable: false),
                    ReceiverReadState = table.Column<int>(nullable: false),
                    SharedMessageId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppChatMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppFriendships",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    UserId = table.Column<long>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    FriendUserId = table.Column<long>(nullable: false),
                    FriendTenantId = table.Column<int>(nullable: true),
                    FriendUserName = table.Column<string>(maxLength: 256, nullable: false),
                    FriendTenancyName = table.Column<string>(nullable: true),
                    FriendProfilePictureId = table.Column<Guid>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppFriendships", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppInvoices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    InvoiceNo = table.Column<string>(nullable: true),
                    InvoiceDate = table.Column<DateTime>(nullable: false),
                    TenantLegalName = table.Column<string>(nullable: true),
                    TenantAddress = table.Column<string>(nullable: true),
                    TenantTaxNo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppInvoices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpAccountCycles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    RoundingDigit = table.Column<int>(nullable: false),
                    RoundingDigitUnitCost = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Remark = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpAccountCycles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpAccountTypes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    AccountTypeName = table.Column<string>(maxLength: 512, nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpAccountTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpAutoSequences",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    DocumentType = table.Column<int>(nullable: false),
                    DefaultPrefix = table.Column<string>(nullable: true),
                    SymbolFormat = table.Column<string>(nullable: true),
                    NumberFormat = table.Column<string>(nullable: true),
                    CustomFormat = table.Column<bool>(nullable: false),
                    RequireReference = table.Column<bool>(nullable: false),
                    YearFormat = table.Column<int>(nullable: true),
                    LastAutoSequenceNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpAutoSequences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpBatchNoFormulas",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    StandardPrePos = table.Column<int>(nullable: false),
                    PrePosCode = table.Column<string>(nullable: true),
                    DateFormat = table.Column<string>(nullable: false),
                    FieldName = table.Column<string>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpBatchNoFormulas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpBatchNos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Code = table.Column<string>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false),
                    IsStandard = table.Column<bool>(nullable: false),
                    ReceiptQty = table.Column<decimal>(nullable: false),
                    IssueQty = table.Column<decimal>(nullable: false),
                    BalanceQty = table.Column<decimal>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    IsSerial = table.Column<bool>(nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "Date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpBatchNos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpBillInvoiceSettings",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    SettingType = table.Column<int>(nullable: false),
                    ReferenceSameAsGoodsMovement = table.Column<bool>(nullable: false),
                    TurnOffStockValidationForImportExcel = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpBillInvoiceSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpCache",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    KeyName = table.Column<string>(maxLength: 512, nullable: false),
                    KeyValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpCache", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpClasses",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ClassName = table.Column<string>(maxLength: 512, nullable: false),
                    ClassParent = table.Column<bool>(nullable: false),
                    ParentClassId = table.Column<long>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpClasses_CarlErpClasses_ParentClassId",
                        column: x => x.ParentClassId,
                        principalTable: "CarlErpClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpCurrencies",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Code = table.Column<string>(maxLength: 16, nullable: false),
                    Name = table.Column<string>(maxLength: 512, nullable: false),
                    Symbol = table.Column<string>(maxLength: 16, nullable: false),
                    PluralName = table.Column<string>(maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpCurrencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpCustomerTypes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    CustomerTypeName = table.Column<string>(maxLength: 512, nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpCustomerTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpExchanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    FromDate = table.Column<DateTime>(nullable: false),
                    ToDate = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpExchanges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpFormats",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Key = table.Column<string>(maxLength: 50, nullable: true),
                    Web = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpFormats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpGalleries",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 512, nullable: false),
                    FileSize = table.Column<long>(nullable: false),
                    ContentType = table.Column<string>(maxLength: 512, nullable: false),
                    StorageFilePath = table.Column<string>(maxLength: 256, nullable: false),
                    UploadFrom = table.Column<int>(nullable: false),
                    UploadSource = table.Column<int>(nullable: false),
                    StorageMainFolderName = table.Column<string>(maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpGalleries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpInventoryCalculationSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ScheduleTime = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpInventoryCalculationSchedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpInventoryTransactionItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    TransactionId = table.Column<Guid>(nullable: false),
                    TransferOrProductionId = table.Column<Guid>(nullable: true),
                    TransferOrProductionItemId = table.Column<Guid>(nullable: true),
                    JournalId = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    OrderIndex = table.Column<string>(nullable: true),
                    JournalType = table.Column<int>(nullable: false),
                    JournalRef = table.Column<string>(nullable: true),
                    JournalNo = table.Column<string>(nullable: true),
                    CreationTimeIndex = table.Column<long>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    InventoryAccountId = table.Column<Guid>(nullable: false),
                    LocationId = table.Column<long>(nullable: false),
                    LotId = table.Column<long>(nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    LineCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    QtyOnHand = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    AvgCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    AdjustmentCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    LatestCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    IsItemReceipt = table.Column<bool>(nullable: false),
                    LastSyncTime = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpInventoryTransactionItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemTypes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(maxLength: 512, nullable: false),
                    DisplayInventoryAccount = table.Column<bool>(nullable: false),
                    DisplayPurchase = table.Column<bool>(nullable: false),
                    DisplaySale = table.Column<bool>(nullable: false),
                    DisplayReorderPoint = table.Column<bool>(nullable: false),
                    DisplayTrackSerialNumber = table.Column<bool>(nullable: false),
                    DisplaySubItem = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpLocations",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    LocationName = table.Column<string>(maxLength: 512, nullable: false),
                    LocationParent = table.Column<bool>(nullable: false),
                    Member = table.Column<int>(nullable: false),
                    ParentLocationId = table.Column<long>(nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 512, nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpLocations_CarlErpLocations_ParentLocationId",
                        column: x => x.ParentLocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpLocks",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    IsLock = table.Column<bool>(nullable: false),
                    LockDate = table.Column<DateTime>(nullable: true),
                    LockKey = table.Column<int>(nullable: false),
                    GenerateDate = table.Column<DateTime>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    ExpiredTime = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpLocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPaymentMethodBases",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(maxLength: 512, nullable: false),
                    Icon = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPaymentMethodBases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPermissionLockSchedules",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ScheduleType = table.Column<int>(nullable: false),
                    ScheduleTime = table.Column<DateTime>(nullable: false),
                    ScheduleDate = table.Column<int>(nullable: false),
                    DaysBeforeYesterday = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPermissionLockSchedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpProductionLines",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    ProductionLineName = table.Column<string>(maxLength: 512, nullable: false),
                    Memo = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpProductionLines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpProperties",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(maxLength: 512, nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsUnit = table.Column<bool>(nullable: false),
                    IsItemGroup = table.Column<bool>(nullable: false),
                    IsStandardCostGroup = table.Column<bool>(nullable: false),
                    IsStatic = table.Column<bool>(nullable: false),
                    IsRequired = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpProperties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpReportTemplate",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ReportType = table.Column<int>(nullable: false),
                    ReportCategory = table.Column<int>(nullable: false),
                    TemplateType = table.Column<int>(nullable: false),
                    TemplateName = table.Column<string>(nullable: true),
                    HeaderTitle = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false),
                    Sortby = table.Column<string>(nullable: true),
                    Groupby = table.Column<string>(nullable: true),
                    DefaultTemplateReport = table.Column<string>(nullable: true),
                    DefaultTemplateReport2 = table.Column<string>(nullable: true),
                    DefaultTemplateReport3 = table.Column<string>(nullable: true),
                    PermissionReadWrite = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpReportTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpTaxes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    TaxName = table.Column<string>(maxLength: 32, nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(6,4)", nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpTaxes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpTransactionTypes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    TransactionTypeName = table.Column<string>(maxLength: 512, nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsPOS = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpTransactionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpVendorTypes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    VendorTypeName = table.Column<string>(maxLength: 512, nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpVendorTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpFeatures",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Value = table.Column<string>(maxLength: 2000, nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    EditionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpFeatures_AbpEditions_EditionId",
                        column: x => x.EditionId,
                        principalTable: "AbpEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppSubscriptionPayments",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    Gateway = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    EditionId = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    DayCount = table.Column<int>(nullable: false),
                    PaymentPeriodType = table.Column<int>(nullable: true),
                    PaymentId = table.Column<string>(nullable: true),
                    InvoiceNo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSubscriptionPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppSubscriptionPayments_AbpEditions_EditionId",
                        column: x => x.EditionId,
                        principalTable: "AbpEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpEntityChanges",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ChangeTime = table.Column<DateTime>(nullable: false),
                    ChangeType = table.Column<byte>(nullable: false),
                    EntityChangeSetId = table.Column<long>(nullable: false),
                    EntityId = table.Column<string>(maxLength: 48, nullable: true),
                    EntityTypeFullName = table.Column<string>(maxLength: 192, nullable: true),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpEntityChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpEntityChanges_AbpEntityChangeSets_EntityChangeSetId",
                        column: x => x.EntityChangeSetId,
                        principalTable: "AbpEntityChangeSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpRoles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 32, nullable: false),
                    DisplayName = table.Column<string>(maxLength: 64, nullable: false),
                    IsStatic = table.Column<bool>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false),
                    NormalizedName = table.Column<string>(maxLength: 32, nullable: false),
                    ConcurrencyStamp = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpRoles_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpRoles_AbpUsers_DeleterUserId",
                        column: x => x.DeleterUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpRoles_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AbpSettings",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    UserId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    Value = table.Column<string>(maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpSettings_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserClaims",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    UserId = table.Column<long>(nullable: false),
                    ClaimType = table.Column<string>(maxLength: 256, nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpUserClaims_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserLogins",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    TenantId = table.Column<int>(nullable: true),
                    UserId = table.Column<long>(nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserLogins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpUserLogins_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserRoles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    UserId = table.Column<long>(nullable: false),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpUserRoles_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpUserTokens",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    TenantId = table.Column<int>(nullable: true),
                    UserId = table.Column<long>(nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: true),
                    Value = table.Column<string>(maxLength: 512, nullable: true),
                    ExpireDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpUserTokens_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpVendorCustomerOpenBalaces",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    TransactionId = table.Column<Guid>(nullable: false),
                    Key = table.Column<int>(nullable: false),
                    AccountCycleId = table.Column<long>(nullable: false),
                    Balance = table.Column<decimal>(nullable: false),
                    MuliCurrencyBalance = table.Column<decimal>(nullable: false),
                    LocationId = table.Column<long>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpVendorCustomerOpenBalaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpVendorCustomerOpenBalaces_CarlErpAccountCycles_Accou~",
                        column: x => x.AccountCycleId,
                        principalTable: "CarlErpAccountCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpMultiCurrencies",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    CurrencyId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpMultiCurrencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpMultiCurrencies_CarlErpCurrencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpCustomerTypeMembers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    MemberId = table.Column<long>(nullable: false),
                    CustomerTypeId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpCustomerTypeMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpCustomerTypeMembers_CarlErpCustomerTypes_CustomerTyp~",
                        column: x => x.CustomerTypeId,
                        principalTable: "CarlErpCustomerTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpCustomerTypeMembers_AbpUsers_MemberId",
                        column: x => x.MemberId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpExchangeItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    FromCurrencyId = table.Column<long>(nullable: false),
                    ToCurencyId = table.Column<long>(nullable: false),
                    Bid = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Ask = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    ExchangeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpExchangeItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpExchangeItems_CarlErpExchanges_ExchangeId",
                        column: x => x.ExchangeId,
                        principalTable: "CarlErpExchanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpExchangeItems_CarlErpCurrencies_FromCurrencyId",
                        column: x => x.FromCurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpExchangeItems_CarlErpCurrencies_ToCurencyId",
                        column: x => x.ToCurencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpInoviceTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(maxLength: 512, nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    TemplateOption = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    GalleryId = table.Column<Guid>(nullable: false),
                    TemplateType = table.Column<int>(nullable: false),
                    ShowDetail = table.Column<bool>(nullable: false),
                    ShowSummary = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpInoviceTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpInoviceTemplates_CarlErpGalleries_GalleryId",
                        column: x => x.GalleryId,
                        principalTable: "CarlErpGalleries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpLots",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    LotName = table.Column<string>(maxLength: 512, nullable: false),
                    LocationId = table.Column<long>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpLots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpLots_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPermissionLocks",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    LockTransaction = table.Column<int>(nullable: false),
                    LockAction = table.Column<int>(nullable: false),
                    PermissionDate = table.Column<DateTime>(nullable: true),
                    LocationId = table.Column<long>(nullable: true),
                    TransactionNo = table.Column<string>(nullable: true),
                    PermissionCode = table.Column<string>(nullable: true),
                    PermissionCodeGenerateDate = table.Column<DateTime>(nullable: false),
                    ExpiredDuration = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPermissionLocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPermissionLocks_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPhysicalCounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    PhysicalCountNo = table.Column<string>(maxLength: 128, nullable: false),
                    Reference = table.Column<string>(maxLength: 128, nullable: true),
                    PhysicalCountDate = table.Column<DateTime>(nullable: false),
                    LocationId = table.Column<long>(nullable: false),
                    ClassId = table.Column<long>(nullable: false),
                    Memo = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPhysicalCounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPhysicalCounts_CarlErpClasses_ClassId",
                        column: x => x.ClassId,
                        principalTable: "CarlErpClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPhysicalCounts_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpTransferOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: false),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    TransferNo = table.Column<string>(maxLength: 128, nullable: false),
                    Reference = table.Column<string>(maxLength: 128, nullable: true),
                    TransferDate = table.Column<DateTime>(nullable: false),
                    TransferToLocationId = table.Column<long>(nullable: false),
                    TransferFromLocationId = table.Column<long>(nullable: false),
                    TransferToClassId = table.Column<long>(nullable: false),
                    TransferFromClassId = table.Column<long>(nullable: false),
                    Memo = table.Column<string>(nullable: true),
                    ShipedStatus = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    ConvertToIssueAndReceiptTransfer = table.Column<bool>(nullable: false),
                    ItemReceiptTransferDate = table.Column<DateTime>(nullable: true),
                    ItemIssueTransferDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpTransferOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpTransferOrders_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpTransferOrders_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpTransferOrders_CarlErpClasses_TransferFromClassId",
                        column: x => x.TransferFromClassId,
                        principalTable: "CarlErpClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpTransferOrders_CarlErpLocations_TransferFromLocation~",
                        column: x => x.TransferFromLocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpTransferOrders_CarlErpClasses_TransferToClassId",
                        column: x => x.TransferToClassId,
                        principalTable: "CarlErpClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpTransferOrders_CarlErpLocations_TransferToLocationId",
                        column: x => x.TransferToLocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpUserGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    LocationId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpUserGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpUserGroups_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPermissionLockScheduleItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    TransactionLockSheduleId = table.Column<long>(nullable: false),
                    LockTransaction = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPermissionLockScheduleItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPermissionLockScheduleItems_CarlErpPermissionLockSch~",
                        column: x => x.TransactionLockSheduleId,
                        principalTable: "CarlErpPermissionLockSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpProductionPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    DocumentNo = table.Column<string>(nullable: false),
                    Reference = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    LocationId = table.Column<long>(nullable: false),
                    ProductionLineId = table.Column<long>(nullable: true),
                    TotalIssueQty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalReceiptQty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalIssueNetWeight = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalReceiptNetWeight = table.Column<decimal>(type: "decimal(19,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpProductionPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpProductionPlans_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpProductionPlans_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpProductionPlans_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpProductionPlans_CarlErpProductionLines_ProductionLin~",
                        column: x => x.ProductionLineId,
                        principalTable: "CarlErpProductionLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPropertyValues",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Value = table.Column<string>(maxLength: 512, nullable: false),
                    PropertyId = table.Column<long>(nullable: false),
                    NetWeight = table.Column<decimal>(nullable: false),
                    GrossWeight = table.Column<decimal>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPropertyValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPropertyValues_CarlErpProperties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "CarlErpProperties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpReportColumnTemplate",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ColumnName = table.Column<string>(nullable: true),
                    ColumnTitle = table.Column<string>(nullable: true),
                    SortOrder = table.Column<int>(nullable: false),
                    Visible = table.Column<bool>(nullable: false),
                    IsDisplay = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    AllowGroupby = table.Column<bool>(nullable: false),
                    AllowFilter = table.Column<bool>(nullable: false),
                    ColumnLength = table.Column<decimal>(nullable: false),
                    ColumnType = table.Column<int>(nullable: false),
                    AllowFunction = table.Column<string>(nullable: true),
                    DisableDefault = table.Column<bool>(nullable: false),
                    ReportTemplateId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpReportColumnTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpReportColumnTemplate_CarlErpReportTemplate_ReportTem~",
                        column: x => x.ReportTemplateId,
                        principalTable: "CarlErpReportTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpReportFilterTemplate",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    FilterName = table.Column<string>(nullable: true),
                    FilterValue = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false),
                    Visible = table.Column<bool>(nullable: false),
                    FilterType = table.Column<int>(nullable: false),
                    DefaultValueId = table.Column<string>(nullable: true),
                    ReportTemplateId = table.Column<long>(nullable: false),
                    AllowShowHideFilter = table.Column<bool>(nullable: false),
                    ShowHideFilter = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpReportFilterTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpReportFilterTemplate_CarlErpReportTemplate_ReportTem~",
                        column: x => x.ReportTemplateId,
                        principalTable: "CarlErpReportTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpChartOfAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    AccountCode = table.Column<string>(maxLength: 16, nullable: false),
                    AccountName = table.Column<string>(maxLength: 512, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    TaxId = table.Column<long>(nullable: false),
                    AccountTypeId = table.Column<long>(nullable: false),
                    ParentAccountId = table.Column<Guid>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    SubAccountType = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpChartOfAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpChartOfAccounts_CarlErpAccountTypes_AccountTypeId",
                        column: x => x.AccountTypeId,
                        principalTable: "CarlErpAccountTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpChartOfAccounts_CarlErpChartOfAccounts_ParentAccount~",
                        column: x => x.ParentAccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpChartOfAccounts_CarlErpTaxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "CarlErpTaxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemPrices",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    TransactionTypeSaleId = table.Column<long>(nullable: true),
                    LocationId = table.Column<long>(nullable: true),
                    CustomerTypeId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemPrices_CarlErpCustomerTypes_CustomerTypeId",
                        column: x => x.CustomerTypeId,
                        principalTable: "CarlErpCustomerTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemPrices_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemPrices_CarlErpTransactionTypes_TransactionTypeSa~",
                        column: x => x.TransactionTypeSaleId,
                        principalTable: "CarlErpTransactionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpVendorTypeMembers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    MemberId = table.Column<long>(nullable: false),
                    VendorTypeId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpVendorTypeMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpVendorTypeMembers_AbpUsers_MemberId",
                        column: x => x.MemberId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpVendorTypeMembers_CarlErpVendorTypes_VendorTypeId",
                        column: x => x.VendorTypeId,
                        principalTable: "CarlErpVendorTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AbpEntityPropertyChanges",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    EntityChangeId = table.Column<long>(nullable: false),
                    NewValue = table.Column<string>(maxLength: 512, nullable: true),
                    OriginalValue = table.Column<string>(maxLength: 512, nullable: true),
                    PropertyName = table.Column<string>(maxLength: 96, nullable: true),
                    PropertyTypeFullName = table.Column<string>(maxLength: 192, nullable: true),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpEntityPropertyChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpEntityPropertyChanges_AbpEntityChanges_EntityChangeId",
                        column: x => x.EntityChangeId,
                        principalTable: "AbpEntityChanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpPermissions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    IsGranted = table.Column<bool>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    RoleId = table.Column<int>(nullable: true),
                    UserId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpPermissions_AbpRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AbpRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AbpPermissions_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AbpRoleClaims",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    RoleId = table.Column<int>(nullable: false),
                    ClaimType = table.Column<string>(maxLength: 256, nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpRoleClaims_AbpRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AbpRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpInvoiceTemplateMaps",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    TemplateType = table.Column<int>(nullable: false),
                    SaleTypeId = table.Column<long>(nullable: true),
                    TemplateId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpInvoiceTemplateMaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpInvoiceTemplateMaps_CarlErpTransactionTypes_SaleType~",
                        column: x => x.SaleTypeId,
                        principalTable: "CarlErpTransactionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpInvoiceTemplateMaps_CarlErpInoviceTemplates_Template~",
                        column: x => x.TemplateId,
                        principalTable: "CarlErpInoviceTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpGroupMemberItemTamplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ReportTemplateId = table.Column<long>(nullable: false),
                    UserGroupId = table.Column<Guid>(nullable: true),
                    MemberUserId = table.Column<long>(nullable: true),
                    PermissionReadWrite = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpGroupMemberItemTamplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpGroupMemberItemTamplates_AbpUsers_MemberUserId",
                        column: x => x.MemberUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpGroupMemberItemTamplates_CarlErpReportTemplate_Repor~",
                        column: x => x.ReportTemplateId,
                        principalTable: "CarlErpReportTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpGroupMemberItemTamplates_CarlErpUserGroups_UserGroup~",
                        column: x => x.UserGroupId,
                        principalTable: "CarlErpUserGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpUserGroupMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    MemberId = table.Column<long>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false),
                    UserGroupId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpUserGroupMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpUserGroupMembers_AbpUsers_MemberId",
                        column: x => x.MemberId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarlErpUserGroupMembers_CarlErpUserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "CarlErpUserGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpProductionStandardCosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    ProductionPlanId = table.Column<Guid>(nullable: false),
                    StandardCostGroupId = table.Column<long>(nullable: true),
                    TotalQty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalNetWeight = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    QtyPercentage = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    NetWeightPercentage = table.Column<decimal>(type: "decimal(19,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpProductionStandardCosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpProductionStandardCosts_CarlErpProductionPlans_Produ~",
                        column: x => x.ProductionPlanId,
                        principalTable: "CarlErpProductionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpProductionStandardCosts_CarlErpPropertyValues_Standa~",
                        column: x => x.StandardCostGroupId,
                        principalTable: "CarlErpPropertyValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AbpTenants",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TenancyName = table.Column<string>(maxLength: 64, nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    ConnectionString = table.Column<string>(maxLength: 1024, nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    EditionId = table.Column<int>(nullable: true),
                    SubscriptionEndDateUtc = table.Column<DateTime>(nullable: true),
                    IsInTrialPeriod = table.Column<bool>(nullable: false),
                    CustomCssId = table.Column<Guid>(nullable: true),
                    LogoId = table.Column<Guid>(nullable: true),
                    LogoFileType = table.Column<string>(maxLength: 64, nullable: true),
                    LegalName = table.Column<string>(maxLength: 512, nullable: true),
                    BusinessId = table.Column<string>(maxLength: 256, nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 128, nullable: true),
                    Website = table.Column<string>(maxLength: 512, nullable: true),
                    Email = table.Column<string>(maxLength: 512, nullable: true),
                    CompanyAddress_Street = table.Column<string>(maxLength: 512, nullable: true),
                    CompanyAddress_CityTown = table.Column<string>(maxLength: 256, nullable: true),
                    CompanyAddress_Province = table.Column<string>(maxLength: 256, nullable: true),
                    CompanyAddress_PostalCode = table.Column<string>(maxLength: 64, nullable: true),
                    CompanyAddress_Country = table.Column<string>(maxLength: 3, nullable: true),
                    LegalAddress_Street = table.Column<string>(maxLength: 512, nullable: true),
                    LegalAddress_CityTown = table.Column<string>(maxLength: 256, nullable: true),
                    LegalAddress_Province = table.Column<string>(maxLength: 256, nullable: true),
                    LegalAddress_PostalCode = table.Column<string>(maxLength: 64, nullable: true),
                    LegalAddress_Country = table.Column<string>(maxLength: 3, nullable: true),
                    SameAsCompanyAddress = table.Column<bool>(nullable: false),
                    CurrencyId = table.Column<long>(nullable: true),
                    TransitAccountId = table.Column<Guid>(nullable: true),
                    SaleAllowanceAccountId = table.Column<Guid>(nullable: true),
                    BillPaymentAccountId = table.Column<Guid>(nullable: true),
                    ClassId = table.Column<long>(nullable: true),
                    LocationId = table.Column<long>(nullable: true),
                    AccountCycleId = table.Column<long>(nullable: true),
                    FormatNumberId = table.Column<long>(nullable: true),
                    FormatDateId = table.Column<long>(nullable: true),
                    ItemRecieptCustomerCreditId = table.Column<Guid>(nullable: true),
                    ItemRecieptTransferId = table.Column<Guid>(nullable: true),
                    ItemRecieptAdjustmentId = table.Column<Guid>(nullable: true),
                    ItemRecieptOtherId = table.Column<Guid>(nullable: true),
                    ItemRecieptPhysicalCountId = table.Column<Guid>(nullable: true),
                    ItemIssueVendorCreditId = table.Column<Guid>(nullable: true),
                    ItemIssueTransferId = table.Column<Guid>(nullable: true),
                    ItemIssueAdjustmentId = table.Column<Guid>(nullable: true),
                    ItemIssueOtherId = table.Column<Guid>(nullable: true),
                    ItemIssuePhysicalCountId = table.Column<Guid>(nullable: true),
                    BankTransferAccountId = table.Column<Guid>(nullable: true),
                    RawProductionAccountId = table.Column<Guid>(nullable: true),
                    FinishProductionAccountId = table.Column<Guid>(nullable: true),
                    RoundDigitAccountId = table.Column<Guid>(nullable: true),
                    CurrentPeriod = table.Column<DateTime>(nullable: false),
                    VendorAccountId = table.Column<Guid>(nullable: true),
                    CustomerAccountId = table.Column<Guid>(nullable: true),
                    IsAutoSequence = table.Column<bool>(nullable: false),
                    PropertyId = table.Column<long>(nullable: true),
                    POSCurrencyId = table.Column<long>(nullable: true),
                    ExchangeLossAndGainId = table.Column<Guid>(nullable: true),
                    SplitCashCreditPayment = table.Column<bool>(nullable: false),
                    UseDefaultAccount = table.Column<bool>(nullable: false),
                    InventoryAccountId = table.Column<Guid>(nullable: true),
                    COGSAccountId = table.Column<Guid>(nullable: true),
                    RevenueAccountId = table.Column<Guid>(nullable: true),
                    ExpenseAccountId = table.Column<Guid>(nullable: true),
                    ValidateProductionNetWeight = table.Column<bool>(nullable: false),
                    AutoItemCode = table.Column<bool>(nullable: false),
                    Prifix = table.Column<string>(nullable: true),
                    ItemCode = table.Column<string>(nullable: true),
                    SubscriptionId = table.Column<Guid>(nullable: true),
                    UseBatchNo = table.Column<bool>(nullable: false),
                    TaxId = table.Column<long>(nullable: true),
                    DefaultInventoryReportTemplate = table.Column<bool>(nullable: false),
                    ProductionSummaryQty = table.Column<bool>(nullable: false),
                    ProductionSummaryNetWeight = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpTenants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpAccountCycles_AccountCycleId",
                        column: x => x.AccountCycleId,
                        principalTable: "CarlErpAccountCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpChartOfAccounts_BankTransferAccountId",
                        column: x => x.BankTransferAccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpChartOfAccounts_BillPaymentAccountId",
                        column: x => x.BillPaymentAccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpChartOfAccounts_COGSAccountId",
                        column: x => x.COGSAccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpClasses_ClassId",
                        column: x => x.ClassId,
                        principalTable: "CarlErpClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpCurrencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpChartOfAccounts_CustomerAccountId",
                        column: x => x.CustomerAccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_AbpUsers_DeleterUserId",
                        column: x => x.DeleterUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_AbpEditions_EditionId",
                        column: x => x.EditionId,
                        principalTable: "AbpEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpChartOfAccounts_ExchangeLossAndGainId",
                        column: x => x.ExchangeLossAndGainId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpChartOfAccounts_ExpenseAccountId",
                        column: x => x.ExpenseAccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpChartOfAccounts_FinishProductionAccountId",
                        column: x => x.FinishProductionAccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpFormats_FormatDateId",
                        column: x => x.FormatDateId,
                        principalTable: "CarlErpFormats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpFormats_FormatNumberId",
                        column: x => x.FormatNumberId,
                        principalTable: "CarlErpFormats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpChartOfAccounts_InventoryAccountId",
                        column: x => x.InventoryAccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemIssueAdjustmentId",
                        column: x => x.ItemIssueAdjustmentId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemIssueOtherId",
                        column: x => x.ItemIssueOtherId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemIssuePhysicalCountId",
                        column: x => x.ItemIssuePhysicalCountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemIssueTransferId",
                        column: x => x.ItemIssueTransferId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemIssueVendorCreditId",
                        column: x => x.ItemIssueVendorCreditId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemRecieptAdjustmentId",
                        column: x => x.ItemRecieptAdjustmentId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemRecieptCustomerCredit~",
                        column: x => x.ItemRecieptCustomerCreditId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemRecieptOtherId",
                        column: x => x.ItemRecieptOtherId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemRecieptPhysicalCountId",
                        column: x => x.ItemRecieptPhysicalCountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpChartOfAccounts_ItemRecieptTransferId",
                        column: x => x.ItemRecieptTransferId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpCurrencies_POSCurrencyId",
                        column: x => x.POSCurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpProperties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "CarlErpProperties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpChartOfAccounts_RawProductionAccountId",
                        column: x => x.RawProductionAccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpChartOfAccounts_RevenueAccountId",
                        column: x => x.RevenueAccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpChartOfAccounts_RoundDigitAccountId",
                        column: x => x.RoundDigitAccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpChartOfAccounts_SaleAllowanceAccountId",
                        column: x => x.SaleAllowanceAccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpTaxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "CarlErpTaxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpChartOfAccounts_TransitAccountId",
                        column: x => x.TransitAccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbpTenants_CarlErpChartOfAccounts_VendorAccountId",
                        column: x => x.VendorAccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpAccountTransactionCloses",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    DateOnly = table.Column<DateTime>(type: "Date", nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    Debit = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    AccountCycleId = table.Column<long>(nullable: false),
                    LocationId = table.Column<long>(nullable: true),
                    CurrencyId = table.Column<long>(nullable: true),
                    MultiCurrencyDebit = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyCredit = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyBalance = table.Column<decimal>(type: "decimal(19,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpAccountTransactionCloses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpAccountTransactionCloses_CarlErpAccountCycles_Accoun~",
                        column: x => x.AccountCycleId,
                        principalTable: "CarlErpAccountCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpAccountTransactionCloses_CarlErpChartOfAccounts_Acco~",
                        column: x => x.AccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpAccountTransactionCloses_CarlErpCurrencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpAccountTransactionCloses_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpBankTransfers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: false),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    BankTransferNo = table.Column<string>(maxLength: 128, nullable: false),
                    Reference = table.Column<string>(maxLength: 128, nullable: true),
                    BankTransferDate = table.Column<DateTime>(nullable: false),
                    TransferToClassId = table.Column<long>(nullable: false),
                    TransferFromClassId = table.Column<long>(nullable: false),
                    Memo = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    BankTransferToAccountId = table.Column<Guid>(nullable: false),
                    BankTransferFromAccountId = table.Column<Guid>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    FromLocationId = table.Column<long>(nullable: true),
                    ToLocationId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpBankTransfers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpBankTransfers_CarlErpChartOfAccounts_BankTransferFro~",
                        column: x => x.BankTransferFromAccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpBankTransfers_CarlErpChartOfAccounts_BankTransferToA~",
                        column: x => x.BankTransferToAccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpBankTransfers_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpBankTransfers_CarlErpLocations_FromLocationId",
                        column: x => x.FromLocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpBankTransfers_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpBankTransfers_CarlErpLocations_ToLocationId",
                        column: x => x.ToLocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpBankTransfers_CarlErpClasses_TransferFromClassId",
                        column: x => x.TransferFromClassId,
                        principalTable: "CarlErpClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpBankTransfers_CarlErpClasses_TransferToClassId",
                        column: x => x.TransferToClassId,
                        principalTable: "CarlErpClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpCustomers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    CustomerCode = table.Column<string>(maxLength: 256, nullable: false),
                    CustomerName = table.Column<string>(maxLength: 512, nullable: false),
                    Email = table.Column<string>(maxLength: 512, nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 128, nullable: true),
                    Website = table.Column<string>(maxLength: 512, nullable: true),
                    BillingAddress_Street = table.Column<string>(maxLength: 512, nullable: true),
                    BillingAddress_CityTown = table.Column<string>(maxLength: 256, nullable: true),
                    BillingAddress_Province = table.Column<string>(maxLength: 256, nullable: true),
                    BillingAddress_PostalCode = table.Column<string>(maxLength: 64, nullable: true),
                    BillingAddress_Country = table.Column<string>(maxLength: 3, nullable: true),
                    ShippingAddress_Street = table.Column<string>(maxLength: 512, nullable: true),
                    ShippingAddress_CityTown = table.Column<string>(maxLength: 256, nullable: true),
                    ShippingAddress_Province = table.Column<string>(maxLength: 256, nullable: true),
                    ShippingAddress_PostalCode = table.Column<string>(maxLength: 64, nullable: true),
                    ShippingAddress_Country = table.Column<string>(maxLength: 3, nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    SameAsShippngAddress = table.Column<bool>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    CustomerTypeId = table.Column<long>(nullable: true),
                    Member = table.Column<int>(nullable: false),
                    IsWalkIn = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpCustomers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpCustomers_CarlErpChartOfAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpCustomers_CarlErpCustomerTypes_CustomerTypeId",
                        column: x => x.CustomerTypeId,
                        principalTable: "CarlErpCustomerTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ItemCode = table.Column<string>(maxLength: 256, nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    ItemName = table.Column<string>(maxLength: 512, nullable: false),
                    PurchaseCost = table.Column<decimal>(type: "decimal(19,6)", nullable: true),
                    SalePrice = table.Column<decimal>(type: "decimal(19,6)", nullable: true),
                    ReorderPoint = table.Column<decimal>(type: "decimal(19,6)", nullable: true),
                    Description = table.Column<string>(nullable: true),
                    TrackSerial = table.Column<bool>(nullable: false),
                    Member = table.Column<int>(nullable: false),
                    ItemTypeId = table.Column<long>(nullable: false),
                    SaleCurrenyId = table.Column<long>(nullable: true),
                    PurchaseCurrencyId = table.Column<long>(nullable: true),
                    SaleAccountId = table.Column<Guid>(nullable: true),
                    PurchaseAccountId = table.Column<Guid>(nullable: true),
                    InventoryAccountId = table.Column<Guid>(nullable: true),
                    PurchaseTaxId = table.Column<long>(nullable: true),
                    SaleTaxId = table.Column<long>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    ShowSubItems = table.Column<bool>(nullable: false),
                    ImageId = table.Column<Guid>(nullable: true),
                    Barcode = table.Column<string>(nullable: true),
                    UseBatchNo = table.Column<bool>(nullable: false),
                    AutoBatchNo = table.Column<bool>(nullable: false),
                    BatchNoFormulaId = table.Column<long>(nullable: true),
                    TrackExpiration = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItems_CarlErpBatchNoFormulas_BatchNoFormulaId",
                        column: x => x.BatchNoFormulaId,
                        principalTable: "CarlErpBatchNoFormulas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItems_CarlErpGalleries_ImageId",
                        column: x => x.ImageId,
                        principalTable: "CarlErpGalleries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItems_CarlErpChartOfAccounts_InventoryAccountId",
                        column: x => x.InventoryAccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItems_CarlErpItemTypes_ItemTypeId",
                        column: x => x.ItemTypeId,
                        principalTable: "CarlErpItemTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarlErpItems_CarlErpChartOfAccounts_PurchaseAccountId",
                        column: x => x.PurchaseAccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItems_CarlErpCurrencies_PurchaseCurrencyId",
                        column: x => x.PurchaseCurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItems_CarlErpTaxes_PurchaseTaxId",
                        column: x => x.PurchaseTaxId,
                        principalTable: "CarlErpTaxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItems_CarlErpChartOfAccounts_SaleAccountId",
                        column: x => x.SaleAccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItems_CarlErpCurrencies_SaleCurrenyId",
                        column: x => x.SaleCurrenyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItems_CarlErpTaxes_SaleTaxId",
                        column: x => x.SaleTaxId,
                        principalTable: "CarlErpTaxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPaymentMethods",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    PaymentMethodId = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    Member = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPaymentMethods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPaymentMethods_CarlErpChartOfAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPaymentMethods_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPaymentMethods_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPaymentMethods_CarlErpPaymentMethodBases_PaymentMeth~",
                        column: x => x.PaymentMethodId,
                        principalTable: "CarlErpPaymentMethodBases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpProductionProcess",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    ProcessName = table.Column<string>(maxLength: 512, nullable: false),
                    SortOrder = table.Column<int>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    Memo = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    UseStandard = table.Column<bool>(nullable: false),
                    IsRequiredProductionPlan = table.Column<bool>(nullable: false),
                    ProductionProcessType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpProductionProcess", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpProductionProcess_CarlErpChartOfAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpVendors",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    VendorCode = table.Column<string>(maxLength: 256, nullable: false),
                    VendorName = table.Column<string>(maxLength: 512, nullable: false),
                    Email = table.Column<string>(maxLength: 512, nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 128, nullable: true),
                    Websit = table.Column<string>(maxLength: 512, nullable: true),
                    BillingAddress_Street = table.Column<string>(maxLength: 512, nullable: true),
                    BillingAddress_CityTown = table.Column<string>(maxLength: 256, nullable: true),
                    BillingAddress_Province = table.Column<string>(maxLength: 256, nullable: true),
                    BillingAddress_PostalCode = table.Column<string>(maxLength: 64, nullable: true),
                    BillingAddress_Country = table.Column<string>(maxLength: 3, nullable: true),
                    ShippingAddress_Street = table.Column<string>(maxLength: 512, nullable: true),
                    ShippingAddress_CityTown = table.Column<string>(maxLength: 256, nullable: true),
                    ShippingAddress_Province = table.Column<string>(maxLength: 256, nullable: true),
                    ShippingAddress_PostalCode = table.Column<string>(maxLength: 64, nullable: true),
                    ShippingAddress_Country = table.Column<string>(maxLength: 3, nullable: true),
                    Remark = table.Column<string>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    SameAsShippngAddress = table.Column<bool>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    VendorTypeId = table.Column<long>(nullable: true),
                    Member = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpVendors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpVendors_CarlErpChartOfAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpVendors_CarlErpVendorTypes_VendorTypeId",
                        column: x => x.VendorTypeId,
                        principalTable: "CarlErpVendorTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpSubscriptionPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    SubscriptionDate = table.Column<DateTime>(nullable: false),
                    AffectedDate = table.Column<DateTime>(nullable: false),
                    Duration = table.Column<int>(nullable: false),
                    DurationType = table.Column<int>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    PaymentMethod = table.Column<int>(nullable: false),
                    PackagePrice = table.Column<decimal>(nullable: false),
                    TotalPrice = table.Column<decimal>(nullable: false),
                    EditionId = table.Column<int>(nullable: false),
                    SubscriptionType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpSubscriptionPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpSubscriptionPayments_AbpEditions_EditionId",
                        column: x => x.EditionId,
                        principalTable: "AbpEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpSubscriptionPayments_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    Duration = table.Column<int>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EditionId = table.Column<int>(nullable: true),
                    Endate = table.Column<DateTime>(nullable: true),
                    DurationType = table.Column<int>(nullable: true),
                    Unlimited = table.Column<bool>(nullable: false),
                    IsTrail = table.Column<bool>(nullable: false),
                    ShowWarning = table.Column<bool>(nullable: false),
                    SubScriptionEndDate = table.Column<DateTime>(nullable: true),
                    PackagePrice = table.Column<decimal>(nullable: false),
                    TotalPrice = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpSubscriptions_AbpEditions_EditionId",
                        column: x => x.EditionId,
                        principalTable: "AbpEditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpSubscriptions_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpCustomerContactPersons",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Title = table.Column<string>(maxLength: 64, nullable: true),
                    FirstName = table.Column<string>(maxLength: 256, nullable: true),
                    LastName = table.Column<string>(maxLength: 256, nullable: true),
                    DisplayNameAs = table.Column<string>(maxLength: 512, nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 128, nullable: true),
                    CustomerId = table.Column<Guid>(nullable: false),
                    IsPrimary = table.Column<bool>(nullable: false),
                    Email = table.Column<string>(maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpCustomerContactPersons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpCustomerContactPersons_CarlErpCustomers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CarlErpCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpCustomerGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    CustomerId = table.Column<Guid>(nullable: false),
                    UserGroupId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpCustomerGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpCustomerGroups_CarlErpCustomers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CarlErpCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarlErpCustomerGroups_CarlErpUserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "CarlErpUserGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpSaleOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    CustomerId = table.Column<Guid>(nullable: false),
                    BillingAddress_Street = table.Column<string>(maxLength: 512, nullable: true),
                    BillingAddress_CityTown = table.Column<string>(maxLength: 256, nullable: true),
                    BillingAddress_Province = table.Column<string>(maxLength: 256, nullable: true),
                    BillingAddress_PostalCode = table.Column<string>(maxLength: 64, nullable: true),
                    BillingAddress_Country = table.Column<string>(maxLength: 3, nullable: true),
                    ShippingAddress_Street = table.Column<string>(maxLength: 512, nullable: true),
                    ShippingAddress_CityTown = table.Column<string>(maxLength: 256, nullable: true),
                    ShippingAddress_Province = table.Column<string>(maxLength: 256, nullable: true),
                    ShippingAddress_PostalCode = table.Column<string>(maxLength: 64, nullable: true),
                    ShippingAddress_Country = table.Column<string>(maxLength: 3, nullable: true),
                    OrderNumber = table.Column<string>(maxLength: 128, nullable: false),
                    OrderDate = table.Column<DateTime>(nullable: false),
                    ETD = table.Column<DateTime>(nullable: false),
                    Reference = table.Column<string>(maxLength: 128, nullable: true),
                    CurrencyId = table.Column<long>(nullable: false),
                    MultiCurrencyId = table.Column<long>(nullable: true),
                    LocationId = table.Column<long>(nullable: true),
                    SubTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencySubTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTax = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    SameAsShippingAddress = table.Column<bool>(nullable: false),
                    Memo = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    ReceiveStatus = table.Column<int>(nullable: false),
                    ApprovalStatus = table.Column<int>(nullable: false, defaultValue: 1),
                    SaleTransactionTypeId = table.Column<long>(nullable: true),
                    IssueCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpSaleOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpSaleOrders_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpSaleOrders_CarlErpCurrencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpSaleOrders_CarlErpCustomers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CarlErpCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpSaleOrders_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpSaleOrders_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpSaleOrders_CarlErpCurrencies_MultiCurrencyId",
                        column: x => x.MultiCurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpSaleOrders_CarlErpTransactionTypes_SaleTransactionTy~",
                        column: x => x.SaleTransactionTypeId,
                        principalTable: "CarlErpTransactionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpInventoryCalculationItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpInventoryCalculationItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpInventoryCalculationItems_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpInventoryCalculationItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpInventoryCalculationItems_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpInventoryCostCloses",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    LocationId = table.Column<long>(nullable: false),
                    AccountCycleId = table.Column<long>(nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    QtyOnhand = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    CloseDate = table.Column<DateTime>(nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpInventoryCostCloses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpInventoryCostCloses_CarlErpAccountCycles_AccountCycl~",
                        column: x => x.AccountCycleId,
                        principalTable: "CarlErpAccountCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpInventoryCostCloses_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpInventoryCostCloses_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemLots",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    LotId = table.Column<long>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemLots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemLots_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemLots_CarlErpLots_LotId",
                        column: x => x.LotId,
                        principalTable: "CarlErpLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemPriceItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    CurrencyId = table.Column<long>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    Price = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    ItemPriceId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemPriceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemPriceItems_CarlErpCurrencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemPriceItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemPriceItems_CarlErpItemPrices_ItemPriceId",
                        column: x => x.ItemPriceId,
                        principalTable: "CarlErpItemPrices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemProperties",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    PropertyId = table.Column<long>(nullable: false),
                    PropertyValueId = table.Column<long>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemProperties_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarlErpItemProperties_CarlErpProperties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "CarlErpProperties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemProperties_CarlErpPropertyValues_PropertyValueId",
                        column: x => x.PropertyValueId,
                        principalTable: "CarlErpPropertyValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemUserGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false),
                    UserGroupId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemUserGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemUserGroups_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarlErpItemUserGroups_CarlErpUserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "CarlErpUserGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPhysicalCountItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    PhysicalCountId = table.Column<Guid>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false),
                    QtyChange = table.Column<decimal>(nullable: false),
                    QtyOnHand = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPhysicalCountItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPhysicalCountItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPhysicalCountItems_CarlErpPhysicalCounts_PhysicalCou~",
                        column: x => x.PhysicalCountId,
                        principalTable: "CarlErpPhysicalCounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpSubItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    ItemId = table.Column<Guid>(nullable: false),
                    ParentSubItemId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpSubItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpSubItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpSubItems_CarlErpItems_ParentSubItemId",
                        column: x => x.ParentSubItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpTransferOrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    TransferOrderId = table.Column<Guid>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(nullable: false),
                    FromLotId = table.Column<long>(nullable: false),
                    ToLotId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpTransferOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpTransferOrderItems_CarlErpLots_FromLotId",
                        column: x => x.FromLotId,
                        principalTable: "CarlErpLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpTransferOrderItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpTransferOrderItems_CarlErpLots_ToLotId",
                        column: x => x.ToLotId,
                        principalTable: "CarlErpLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpTransferOrderItems_CarlErpTransferOrders_TransferOrd~",
                        column: x => x.TransferOrderId,
                        principalTable: "CarlErpTransferOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPayBills",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    FiFo = table.Column<bool>(nullable: false),
                    TotalOpenBalance = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalOpenBalanceVendorCredit = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Change = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyChange = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalPayment = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalPaymentVendorCredit = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalPaymentBill = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalPaymentDue = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalPaymentDueVendorCredit = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTotalPayment = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTotalPaymentBill = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTotalPaymentVendorCredit = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    ReceiveFrom = table.Column<int>(nullable: false),
                    PaymentMethodId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPayBills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPayBills_CarlErpPaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "CarlErpPaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPaymentMethodUserGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    PaymentMethodId = table.Column<Guid>(nullable: false),
                    UserGroupId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPaymentMethodUserGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPaymentMethodUserGroups_CarlErpPaymentMethods_Paymen~",
                        column: x => x.PaymentMethodId,
                        principalTable: "CarlErpPaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPaymentMethodUserGroups_CarlErpUserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "CarlErpUserGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpReceivePayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    FiFo = table.Column<bool>(nullable: false),
                    TotalOpenBalance = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalOpenBalanceCustomerCredit = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTotalPaymentCustomerCredit = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Change = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyChange = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalPayment = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTotalPayment = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalPaymentCustomerCredit = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalPaymentInvoice = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTotalPaymentInvoice = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalPaymentDue = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalPaymentDueCustomerCredit = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalCashInvoice = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalCreditInvoice = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalExpenseInvoice = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalCashCustomerCredit = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalCreditCustomerCredit = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalExpenseCustomerCredit = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    ReceiveFrom = table.Column<int>(nullable: false),
                    PaymentMethodId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpReceivePayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpReceivePayments_CarlErpPaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "CarlErpPaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpTransProductions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ProductionNo = table.Column<string>(maxLength: 128, nullable: false),
                    Reference = table.Column<string>(maxLength: 128, nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    ProductionAccountId = table.Column<Guid>(nullable: false),
                    ProductionProcessId = table.Column<long>(nullable: true),
                    ProductionPlanId = table.Column<Guid>(nullable: true),
                    ToLocationId = table.Column<long>(nullable: false),
                    FromLocationId = table.Column<long>(nullable: false),
                    ToClassId = table.Column<long>(nullable: false),
                    FromClassId = table.Column<long>(nullable: false),
                    Memo = table.Column<string>(nullable: true),
                    ShipedStatus = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    ConvertToIssueAndReceipt = table.Column<bool>(nullable: false),
                    ReceiptDate = table.Column<DateTime>(nullable: true),
                    IssueDate = table.Column<DateTime>(nullable: true),
                    SubTotalRawProduction = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    SubTotalFinishProduction = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalIssueQty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalReceiptQty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalIssueNetWeight = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalReceiptNetWeight = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    CalculationState = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpTransProductions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpTransProductions_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpTransProductions_CarlErpClasses_FromClassId",
                        column: x => x.FromClassId,
                        principalTable: "CarlErpClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpTransProductions_CarlErpLocations_FromLocationId",
                        column: x => x.FromLocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpTransProductions_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpTransProductions_CarlErpChartOfAccounts_ProductionAc~",
                        column: x => x.ProductionAccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpTransProductions_CarlErpProductionPlans_ProductionPl~",
                        column: x => x.ProductionPlanId,
                        principalTable: "CarlErpProductionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpTransProductions_CarlErpProductionProcess_Production~",
                        column: x => x.ProductionProcessId,
                        principalTable: "CarlErpProductionProcess",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpTransProductions_CarlErpClasses_ToClassId",
                        column: x => x.ToClassId,
                        principalTable: "CarlErpClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpTransProductions_CarlErpLocations_ToLocationId",
                        column: x => x.ToLocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpContactPersons",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Title = table.Column<string>(maxLength: 64, nullable: true),
                    FirstName = table.Column<string>(maxLength: 256, nullable: true),
                    LastName = table.Column<string>(maxLength: 256, nullable: true),
                    DisplayNameAs = table.Column<string>(maxLength: 512, nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 128, nullable: true),
                    VenderId = table.Column<Guid>(nullable: false),
                    IsPrimary = table.Column<bool>(nullable: false),
                    Email = table.Column<string>(maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpContactPersons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpContactPersons_CarlErpVendors_VenderId",
                        column: x => x.VenderId,
                        principalTable: "CarlErpVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpDeposits",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ReceiveFromVendorId = table.Column<Guid>(nullable: true),
                    ReceiveFromCustomerId = table.Column<Guid>(nullable: true),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    BankTransferId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpDeposits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpDeposits_CarlErpCustomers_ReceiveFromCustomerId",
                        column: x => x.ReceiveFromCustomerId,
                        principalTable: "CarlErpCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpDeposits_CarlErpVendors_ReceiveFromVendorId",
                        column: x => x.ReceiveFromVendorId,
                        principalTable: "CarlErpVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPurchaseOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    VendorId = table.Column<Guid>(nullable: false),
                    BillingAddress_Street = table.Column<string>(maxLength: 512, nullable: true),
                    BillingAddress_CityTown = table.Column<string>(maxLength: 256, nullable: true),
                    BillingAddress_Province = table.Column<string>(maxLength: 256, nullable: true),
                    BillingAddress_PostalCode = table.Column<string>(maxLength: 64, nullable: true),
                    BillingAddress_Country = table.Column<string>(maxLength: 3, nullable: true),
                    ShippingAddress_Street = table.Column<string>(maxLength: 512, nullable: true),
                    ShippingAddress_CityTown = table.Column<string>(maxLength: 256, nullable: true),
                    ShippingAddress_Province = table.Column<string>(maxLength: 256, nullable: true),
                    ShippingAddress_PostalCode = table.Column<string>(maxLength: 64, nullable: true),
                    ShippingAddress_Country = table.Column<string>(maxLength: 3, nullable: true),
                    OrderNumber = table.Column<string>(maxLength: 128, nullable: false),
                    OrderDate = table.Column<DateTime>(nullable: false),
                    ETA = table.Column<DateTime>(nullable: false),
                    Reference = table.Column<string>(maxLength: 128, nullable: true),
                    CurrencyId = table.Column<long>(nullable: false),
                    MultiCurrencyId = table.Column<long>(nullable: true),
                    SubTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencySubTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTax = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    SameAsShippingAddress = table.Column<bool>(nullable: false),
                    Memo = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    ReceiveStatus = table.Column<int>(nullable: false),
                    ApprovalStatus = table.Column<int>(nullable: false, defaultValue: 1),
                    LocationId = table.Column<long>(nullable: true),
                    ReceiveCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPurchaseOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPurchaseOrders_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPurchaseOrders_CarlErpCurrencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPurchaseOrders_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPurchaseOrders_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPurchaseOrders_CarlErpCurrencies_MultiCurrencyId",
                        column: x => x.MultiCurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPurchaseOrders_CarlErpVendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "CarlErpVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpVendorGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    VendorId = table.Column<Guid>(nullable: false),
                    UserGroupId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpVendorGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpVendorGroups_CarlErpUserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "CarlErpUserGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarlErpVendorGroups_CarlErpVendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "CarlErpVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpWithdraws",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    VendorId = table.Column<Guid>(nullable: true),
                    CustomerId = table.Column<Guid>(nullable: true),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    BankTransferId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpWithdraws", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpWithdraws_CarlErpCustomers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CarlErpCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpWithdraws_CarlErpVendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "CarlErpVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpSaleOrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    SaleOrderId = table.Column<Guid>(nullable: false),
                    TaxId = table.Column<long>(nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(6,4)", nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(6,4)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    MultiCurrencyUnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpSaleOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpSaleOrderItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpSaleOrderItems_CarlErpSaleOrders_SaleOrderId",
                        column: x => x.SaleOrderId,
                        principalTable: "CarlErpSaleOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpSaleOrderItems_CarlErpTaxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "CarlErpTaxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpInventoryCostCloseItemBatchNos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    LotId = table.Column<long>(nullable: false),
                    BatchNoId = table.Column<Guid>(nullable: false),
                    Qty = table.Column<decimal>(nullable: false),
                    InventoryCostCloseId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpInventoryCostCloseItemBatchNos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpInventoryCostCloseItemBatchNos_CarlErpBatchNos_Batch~",
                        column: x => x.BatchNoId,
                        principalTable: "CarlErpBatchNos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpInventoryCostCloseItemBatchNos_CarlErpInventoryCostC~",
                        column: x => x.InventoryCostCloseId,
                        principalTable: "CarlErpInventoryCostCloses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpInventoryCostCloseItemBatchNos_CarlErpLots_LotId",
                        column: x => x.LotId,
                        principalTable: "CarlErpLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpInventoryCostCloseItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    LotId = table.Column<long>(nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    InventoryCostCloseId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpInventoryCostCloseItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpInventoryCostCloseItems_CarlErpInventoryCostCloses_I~",
                        column: x => x.InventoryCostCloseId,
                        principalTable: "CarlErpInventoryCostCloses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpInventoryCostCloseItems_CarlErpLots_LotId",
                        column: x => x.LotId,
                        principalTable: "CarlErpLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPayBillExpense",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    PayBillId = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyAmount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPayBillExpense", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPayBillExpense_CarlErpChartOfAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPayBillExpense_CarlErpPayBills_PayBillId",
                        column: x => x.PayBillId,
                        principalTable: "CarlErpPayBills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpReceivePaymentExpense",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    ReceivePaymentId = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    MultiCurrencyAmount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpReceivePaymentExpense", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpReceivePaymentExpense_CarlErpChartOfAccounts_Account~",
                        column: x => x.AccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpReceivePaymentExpense_CarlErpReceivePayments_Receive~",
                        column: x => x.ReceivePaymentId,
                        principalTable: "CarlErpReceivePayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpFinishItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ProductionId = table.Column<Guid>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    ToLotId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpFinishItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpFinishItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpFinishItems_CarlErpTransProductions_ProductionId",
                        column: x => x.ProductionId,
                        principalTable: "CarlErpTransProductions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpFinishItems_CarlErpLots_ToLotId",
                        column: x => x.ToLotId,
                        principalTable: "CarlErpLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemIssues",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    TransferOrderId = table.Column<Guid>(nullable: true),
                    ProductionOrderId = table.Column<Guid>(nullable: true),
                    ReceiveFrom = table.Column<int>(nullable: false),
                    CustomerId = table.Column<Guid>(nullable: true),
                    ProductionProcessId = table.Column<long>(nullable: true),
                    SameAsShippingAddress = table.Column<bool>(nullable: false),
                    ConvertToInvoice = table.Column<bool>(nullable: false),
                    BillingAddress_Street = table.Column<string>(maxLength: 512, nullable: true),
                    BillingAddress_CityTown = table.Column<string>(maxLength: 256, nullable: true),
                    BillingAddress_Province = table.Column<string>(maxLength: 256, nullable: true),
                    BillingAddress_PostalCode = table.Column<string>(maxLength: 64, nullable: true),
                    BillingAddress_Country = table.Column<string>(maxLength: 3, nullable: true),
                    ShippingAddress_Street = table.Column<string>(maxLength: 512, nullable: true),
                    ShippingAddress_CityTown = table.Column<string>(maxLength: 256, nullable: true),
                    ShippingAddress_Province = table.Column<string>(maxLength: 256, nullable: true),
                    ShippingAddress_PostalCode = table.Column<string>(maxLength: 64, nullable: true),
                    ShippingAddress_Country = table.Column<string>(maxLength: 3, nullable: true),
                    Total = table.Column<decimal>(type: "decimal(28,6)", nullable: false),
                    TransactionType = table.Column<int>(nullable: false),
                    PhysicalCountId = table.Column<Guid>(nullable: true),
                    TransactionTypeSaleId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemIssues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssues_CarlErpCustomers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CarlErpCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssues_CarlErpPhysicalCounts_PhysicalCountId",
                        column: x => x.PhysicalCountId,
                        principalTable: "CarlErpPhysicalCounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssues_CarlErpTransProductions_ProductionOrderId",
                        column: x => x.ProductionOrderId,
                        principalTable: "CarlErpTransProductions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssues_CarlErpProductionProcess_ProductionProces~",
                        column: x => x.ProductionProcessId,
                        principalTable: "CarlErpProductionProcess",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssues_CarlErpTransactionTypes_TransactionTypeSa~",
                        column: x => x.TransactionTypeSaleId,
                        principalTable: "CarlErpTransactionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssues_CarlErpTransferOrders_TransferOrderId",
                        column: x => x.TransferOrderId,
                        principalTable: "CarlErpTransferOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemReceipts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ReceiveFrom = table.Column<int>(nullable: false),
                    VendorId = table.Column<Guid>(nullable: true),
                    TransactionType = table.Column<int>(nullable: false),
                    SameAsShippingAddress = table.Column<bool>(nullable: false),
                    BillingAddress_Street = table.Column<string>(maxLength: 512, nullable: true),
                    BillingAddress_CityTown = table.Column<string>(maxLength: 256, nullable: true),
                    BillingAddress_Province = table.Column<string>(maxLength: 256, nullable: true),
                    BillingAddress_PostalCode = table.Column<string>(maxLength: 64, nullable: true),
                    BillingAddress_Country = table.Column<string>(maxLength: 3, nullable: true),
                    ShippingAddress_Street = table.Column<string>(maxLength: 512, nullable: true),
                    ShippingAddress_CityTown = table.Column<string>(maxLength: 256, nullable: true),
                    ShippingAddress_Province = table.Column<string>(maxLength: 256, nullable: true),
                    ShippingAddress_PostalCode = table.Column<string>(maxLength: 64, nullable: true),
                    ShippingAddress_Country = table.Column<string>(maxLength: 3, nullable: true),
                    TransferOrderId = table.Column<Guid>(nullable: true),
                    ProductionOrderId = table.Column<Guid>(nullable: true),
                    ProductionProcessId = table.Column<long>(nullable: true),
                    PhysicalCountId = table.Column<Guid>(nullable: true),
                    Total = table.Column<decimal>(type: "decimal(28,6)", nullable: false),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemReceipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceipts_CarlErpPhysicalCounts_PhysicalCountId",
                        column: x => x.PhysicalCountId,
                        principalTable: "CarlErpPhysicalCounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceipts_CarlErpTransProductions_ProductionOrder~",
                        column: x => x.ProductionOrderId,
                        principalTable: "CarlErpTransProductions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceipts_CarlErpProductionProcess_ProductionProc~",
                        column: x => x.ProductionProcessId,
                        principalTable: "CarlErpProductionProcess",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceipts_CarlErpTransferOrders_TransferOrderId",
                        column: x => x.TransferOrderId,
                        principalTable: "CarlErpTransferOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceipts_CarlErpVendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "CarlErpVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpProductionStandardCostGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    ProductionId = table.Column<Guid>(nullable: false),
                    StandardCostGroupId = table.Column<long>(nullable: true),
                    TotalQty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalNetWeight = table.Column<decimal>(type: "decimal(19,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpProductionStandardCostGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpProductionStandardCostGroups_CarlErpTransProductions~",
                        column: x => x.ProductionId,
                        principalTable: "CarlErpTransProductions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpProductionStandardCostGroups_CarlErpPropertyValues_S~",
                        column: x => x.StandardCostGroupId,
                        principalTable: "CarlErpPropertyValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpRawMaterialItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ProductionId = table.Column<Guid>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    FromLotId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpRawMaterialItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpRawMaterialItems_CarlErpLots_FromLotId",
                        column: x => x.FromLotId,
                        principalTable: "CarlErpLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpRawMaterialItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpRawMaterialItems_CarlErpTransProductions_ProductionId",
                        column: x => x.ProductionId,
                        principalTable: "CarlErpTransProductions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpDepositItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    DepositId = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    Qty = table.Column<decimal>(nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpDepositItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpDepositItems_CarlErpChartOfAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpDepositItems_CarlErpDeposits_DepositId",
                        column: x => x.DepositId,
                        principalTable: "CarlErpDeposits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPurchaseOrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    PurchaseOrderId = table.Column<Guid>(nullable: false),
                    TaxId = table.Column<long>(nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(6,4)", nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Unit = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyUnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(6,4)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPurchaseOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPurchaseOrderItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPurchaseOrderItems_CarlErpPurchaseOrders_PurchaseOrd~",
                        column: x => x.PurchaseOrderId,
                        principalTable: "CarlErpPurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPurchaseOrderItems_CarlErpTaxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "CarlErpTaxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpWithdrawItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    WithdrawId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpWithdrawItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpWithdrawItems_CarlErpWithdraws_WithdrawId",
                        column: x => x.WithdrawId,
                        principalTable: "CarlErpWithdraws",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpCustomerCredits",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    CustomerId = table.Column<Guid>(nullable: false),
                    SameAsShippingAddress = table.Column<bool>(nullable: false),
                    BillingAddress_Street = table.Column<string>(maxLength: 512, nullable: true),
                    BillingAddress_CityTown = table.Column<string>(maxLength: 256, nullable: true),
                    BillingAddress_Province = table.Column<string>(maxLength: 256, nullable: true),
                    BillingAddress_PostalCode = table.Column<string>(maxLength: 64, nullable: true),
                    BillingAddress_Country = table.Column<string>(maxLength: 3, nullable: true),
                    ShippingAddress_Street = table.Column<string>(maxLength: 512, nullable: true),
                    ShippingAddress_CityTown = table.Column<string>(maxLength: 256, nullable: true),
                    ShippingAddress_Province = table.Column<string>(maxLength: 256, nullable: true),
                    ShippingAddress_PostalCode = table.Column<string>(maxLength: 64, nullable: true),
                    ShippingAddress_Country = table.Column<string>(maxLength: 3, nullable: true),
                    SubTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    OpenBalance = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    ShipedStatus = table.Column<int>(nullable: false),
                    PaidStatus = table.Column<int>(nullable: false),
                    TotalPaid = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    ReceiveDate = table.Column<DateTime>(nullable: true),
                    ConvertToItemReceipt = table.Column<bool>(nullable: false),
                    MultiCurrencyOpenBalance = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTotalPaid = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencySubTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTax = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    ReceiveFrom = table.Column<int>(nullable: false),
                    ItemIssueSaleId = table.Column<Guid>(nullable: true),
                    IsPOS = table.Column<bool>(nullable: false),
                    IsItem = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpCustomerCredits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpCustomerCredits_CarlErpCustomers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CarlErpCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpCustomerCredits_CarlErpItemIssues_ItemIssueSaleId",
                        column: x => x.ItemIssueSaleId,
                        principalTable: "CarlErpItemIssues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpInvoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ReceiveFrom = table.Column<int>(nullable: false),
                    ItemIssueId = table.Column<Guid>(nullable: true),
                    CustomerId = table.Column<Guid>(nullable: false),
                    SameAsShippingAddress = table.Column<bool>(nullable: false),
                    BillingAddress_Street = table.Column<string>(maxLength: 512, nullable: true),
                    BillingAddress_CityTown = table.Column<string>(maxLength: 256, nullable: true),
                    BillingAddress_Province = table.Column<string>(maxLength: 256, nullable: true),
                    BillingAddress_PostalCode = table.Column<string>(maxLength: 64, nullable: true),
                    BillingAddress_Country = table.Column<string>(maxLength: 3, nullable: true),
                    ShippingAddress_Street = table.Column<string>(maxLength: 512, nullable: true),
                    ShippingAddress_CityTown = table.Column<string>(maxLength: 256, nullable: true),
                    ShippingAddress_Province = table.Column<string>(maxLength: 256, nullable: true),
                    ShippingAddress_PostalCode = table.Column<string>(maxLength: 64, nullable: true),
                    ShippingAddress_Country = table.Column<string>(maxLength: 3, nullable: true),
                    SubTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    OpenBalance = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalPaid = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyOpenBalance = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTotalPaid = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencySubTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTax = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    ReceiveDate = table.Column<DateTime>(nullable: true),
                    ConvertToItemIssue = table.Column<bool>(nullable: false),
                    ETD = table.Column<DateTime>(nullable: false),
                    ReceivedStatus = table.Column<int>(nullable: false),
                    PaidStatus = table.Column<int>(nullable: false),
                    TransactionTypeSaleId = table.Column<long>(nullable: true),
                    IsItem = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpInvoices_CarlErpCustomers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CarlErpCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpInvoices_CarlErpItemIssues_ItemIssueId",
                        column: x => x.ItemIssueId,
                        principalTable: "CarlErpItemIssues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpInvoices_CarlErpTransactionTypes_TransactionTypeSale~",
                        column: x => x.TransactionTypeSaleId,
                        principalTable: "CarlErpTransactionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpBills",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ReceiveFrom = table.Column<int>(nullable: false),
                    ItemReceiptId = table.Column<Guid>(nullable: true),
                    VendorId = table.Column<Guid>(nullable: false),
                    SameAsShippingAddress = table.Column<bool>(nullable: false),
                    BillingAddress_Street = table.Column<string>(maxLength: 512, nullable: true),
                    BillingAddress_CityTown = table.Column<string>(maxLength: 256, nullable: true),
                    BillingAddress_Province = table.Column<string>(maxLength: 256, nullable: true),
                    BillingAddress_PostalCode = table.Column<string>(maxLength: 64, nullable: true),
                    BillingAddress_Country = table.Column<string>(maxLength: 3, nullable: true),
                    ShippingAddress_Street = table.Column<string>(maxLength: 512, nullable: true),
                    ShippingAddress_CityTown = table.Column<string>(maxLength: 256, nullable: true),
                    ShippingAddress_Province = table.Column<string>(maxLength: 256, nullable: true),
                    ShippingAddress_PostalCode = table.Column<string>(maxLength: 64, nullable: true),
                    ShippingAddress_Country = table.Column<string>(maxLength: 3, nullable: true),
                    SubTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencySubTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTax = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    OpenBalance = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalPaid = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyOpenBalance = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTotalPaid = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    ETA = table.Column<DateTime>(nullable: false),
                    ReceivedStatus = table.Column<int>(nullable: false),
                    PaidStatus = table.Column<int>(nullable: false),
                    ConvertToItemReceipt = table.Column<bool>(nullable: false),
                    ItemReceiptDate = table.Column<DateTime>(nullable: true),
                    IsItem = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpBills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpBills_CarlErpItemReceipts_ItemReceiptId",
                        column: x => x.ItemReceiptId,
                        principalTable: "CarlErpItemReceipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpBills_CarlErpVendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "CarlErpVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpVendorCredit",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ReceiveFrom = table.Column<int>(nullable: false),
                    VendorId = table.Column<Guid>(nullable: false),
                    SameAsShippingAddress = table.Column<bool>(nullable: false),
                    BillingAddress_Street = table.Column<string>(maxLength: 512, nullable: true),
                    BillingAddress_CityTown = table.Column<string>(maxLength: 256, nullable: true),
                    BillingAddress_Province = table.Column<string>(maxLength: 256, nullable: true),
                    BillingAddress_PostalCode = table.Column<string>(maxLength: 64, nullable: true),
                    BillingAddress_Country = table.Column<string>(maxLength: 3, nullable: true),
                    ShippingAddress_Street = table.Column<string>(maxLength: 512, nullable: true),
                    ShippingAddress_CityTown = table.Column<string>(maxLength: 256, nullable: true),
                    ShippingAddress_Province = table.Column<string>(maxLength: 256, nullable: true),
                    ShippingAddress_PostalCode = table.Column<string>(maxLength: 64, nullable: true),
                    ShippingAddress_Country = table.Column<string>(maxLength: 3, nullable: true),
                    SubTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    OpenBalance = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    ShipedStatus = table.Column<int>(nullable: false),
                    PaidStatus = table.Column<int>(nullable: false),
                    TotalPaid = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    IssueDate = table.Column<DateTime>(nullable: true),
                    ConvertToItemIssueVendor = table.Column<bool>(nullable: false),
                    MultiCurrencyOpenBalance = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTotalPaid = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencySubTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTax = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    ItemReceiptId = table.Column<Guid>(nullable: true),
                    IsItem = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpVendorCredit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpVendorCredit_CarlErpItemReceipts_ItemReceiptId",
                        column: x => x.ItemReceiptId,
                        principalTable: "CarlErpItemReceipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpVendorCredit_CarlErpVendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "CarlErpVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemIssueItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ItemIssueId = table.Column<Guid>(nullable: false),
                    SaleOrderItemId = table.Column<Guid>(nullable: true),
                    TransferOrderItemId = table.Column<Guid>(nullable: true),
                    RawMaterialItemId = table.Column<Guid>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(28,6)", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(9,4)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(28,6)", nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    LotId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemIssueItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueItems_CarlErpItemIssues_ItemIssueId",
                        column: x => x.ItemIssueId,
                        principalTable: "CarlErpItemIssues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueItems_CarlErpLots_LotId",
                        column: x => x.LotId,
                        principalTable: "CarlErpLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueItems_CarlErpRawMaterialItems_RawMaterialIt~",
                        column: x => x.RawMaterialItemId,
                        principalTable: "CarlErpRawMaterialItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueItems_CarlErpSaleOrderItems_SaleOrderItemId",
                        column: x => x.SaleOrderItemId,
                        principalTable: "CarlErpSaleOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueItems_CarlErpTransferOrderItems_TransferOrd~",
                        column: x => x.TransferOrderItemId,
                        principalTable: "CarlErpTransferOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemReceiptItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ItemReceiptId = table.Column<Guid>(nullable: false),
                    OrderItemId = table.Column<Guid>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    TransferOrderItemId = table.Column<Guid>(nullable: true),
                    FinishItemId = table.Column<Guid>(nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(28,6)", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(9,4)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(28,6)", nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    LotId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemReceiptItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptItems_CarlErpFinishItems_FinishItemId",
                        column: x => x.FinishItemId,
                        principalTable: "CarlErpFinishItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptItems_CarlErpItemReceipts_ItemReceiptId",
                        column: x => x.ItemReceiptId,
                        principalTable: "CarlErpItemReceipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptItems_CarlErpLots_LotId",
                        column: x => x.LotId,
                        principalTable: "CarlErpLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptItems_CarlErpPurchaseOrderItems_OrderItem~",
                        column: x => x.OrderItemId,
                        principalTable: "CarlErpPurchaseOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptItems_CarlErpTransferOrderItems_TransferO~",
                        column: x => x.TransferOrderItemId,
                        principalTable: "CarlErpTransferOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemReceiptCustomerCredit",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ReceiveFrom = table.Column<int>(nullable: false),
                    CustomerCreditId = table.Column<Guid>(nullable: true),
                    CustomerId = table.Column<Guid>(nullable: false),
                    TransactionType = table.Column<int>(nullable: false),
                    SameAsShippingAddress = table.Column<bool>(nullable: false),
                    BillingAddress_Street = table.Column<string>(maxLength: 512, nullable: true),
                    BillingAddress_CityTown = table.Column<string>(maxLength: 256, nullable: true),
                    BillingAddress_Province = table.Column<string>(maxLength: 256, nullable: true),
                    BillingAddress_PostalCode = table.Column<string>(maxLength: 64, nullable: true),
                    BillingAddress_Country = table.Column<string>(maxLength: 3, nullable: true),
                    ShippingAddress_Street = table.Column<string>(maxLength: 512, nullable: true),
                    ShippingAddress_CityTown = table.Column<string>(maxLength: 256, nullable: true),
                    ShippingAddress_Province = table.Column<string>(maxLength: 256, nullable: true),
                    ShippingAddress_PostalCode = table.Column<string>(maxLength: 64, nullable: true),
                    ShippingAddress_Country = table.Column<string>(maxLength: 3, nullable: true),
                    Total = table.Column<decimal>(type: "decimal(28,6)", nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    ItemIssueSaleId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemReceiptCustomerCredit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptCustomerCredit_CarlErpCustomerCredits_Cus~",
                        column: x => x.CustomerCreditId,
                        principalTable: "CarlErpCustomerCredits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptCustomerCredit_CarlErpCustomers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CarlErpCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptCustomerCredit_CarlErpItemIssues_ItemIssu~",
                        column: x => x.ItemIssueSaleId,
                        principalTable: "CarlErpItemIssues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpReceivePaymentDeails",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ReceivePaymentId = table.Column<Guid>(nullable: false),
                    InvoiceId = table.Column<Guid>(nullable: true),
                    CustomerCreditId = table.Column<Guid>(nullable: true),
                    CustomerId = table.Column<Guid>(nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    OpenBalance = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Payment = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Cash = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyCash = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyCredit = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Expense = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyExpense = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyOpenBalance = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyPayment = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTotalAmount = table.Column<decimal>(type: "decimal(19,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpReceivePaymentDeails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpReceivePaymentDeails_CarlErpCustomerCredits_Customer~",
                        column: x => x.CustomerCreditId,
                        principalTable: "CarlErpCustomerCredits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpReceivePaymentDeails_CarlErpCustomers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CarlErpCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpReceivePaymentDeails_CarlErpInvoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "CarlErpInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpReceivePaymentDeails_CarlErpReceivePayments_ReceiveP~",
                        column: x => x.ReceivePaymentId,
                        principalTable: "CarlErpReceivePayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemIssueVendorCredit",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ReceiveFrom = table.Column<int>(nullable: false),
                    VendorCreditId = table.Column<Guid>(nullable: true),
                    VendorId = table.Column<Guid>(nullable: false),
                    TransactionType = table.Column<int>(nullable: false),
                    SameAsShippingAddress = table.Column<bool>(nullable: false),
                    BillingAddress_Street = table.Column<string>(maxLength: 512, nullable: true),
                    BillingAddress_CityTown = table.Column<string>(maxLength: 256, nullable: true),
                    BillingAddress_Province = table.Column<string>(maxLength: 256, nullable: true),
                    BillingAddress_PostalCode = table.Column<string>(maxLength: 64, nullable: true),
                    BillingAddress_Country = table.Column<string>(maxLength: 3, nullable: true),
                    ShippingAddress_Street = table.Column<string>(maxLength: 512, nullable: true),
                    ShippingAddress_CityTown = table.Column<string>(maxLength: 256, nullable: true),
                    ShippingAddress_Province = table.Column<string>(maxLength: 256, nullable: true),
                    ShippingAddress_PostalCode = table.Column<string>(maxLength: 64, nullable: true),
                    ShippingAddress_Country = table.Column<string>(maxLength: 3, nullable: true),
                    Total = table.Column<decimal>(type: "decimal(28,6)", nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    ItemReceiptPurchaseId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemIssueVendorCredit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueVendorCredit_CarlErpItemReceipts_ItemReceip~",
                        column: x => x.ItemReceiptPurchaseId,
                        principalTable: "CarlErpItemReceipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueVendorCredit_CarlErpVendorCredit_VendorCred~",
                        column: x => x.VendorCreditId,
                        principalTable: "CarlErpVendorCredit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueVendorCredit_CarlErpVendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "CarlErpVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPayBillDeail",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    PayBillId = table.Column<Guid>(nullable: false),
                    BillId = table.Column<Guid>(nullable: true),
                    VendorCreditId = table.Column<Guid>(nullable: true),
                    VendorId = table.Column<Guid>(nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    OpenBalance = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Payment = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyOpenBalance = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyPayment = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTotalAmount = table.Column<decimal>(type: "decimal(19,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPayBillDeail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPayBillDeail_CarlErpBills_BillId",
                        column: x => x.BillId,
                        principalTable: "CarlErpBills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPayBillDeail_CarlErpPayBills_PayBillId",
                        column: x => x.PayBillId,
                        principalTable: "CarlErpPayBills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPayBillDeail_CarlErpVendorCredit_VendorCreditId",
                        column: x => x.VendorCreditId,
                        principalTable: "CarlErpVendorCredit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPayBillDeail_CarlErpVendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "CarlErpVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpCustomerCreditDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    CustomerCreditId = table.Column<Guid>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: true),
                    TaxId = table.Column<long>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    LotId = table.Column<long>(nullable: true),
                    MultiCurrencyUnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    ItemIssueSaleItemId = table.Column<Guid>(nullable: true),
                    SalePrice = table.Column<decimal>(type: "decimal(19,6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpCustomerCreditDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpCustomerCreditDetails_CarlErpCustomerCredits_Custome~",
                        column: x => x.CustomerCreditId,
                        principalTable: "CarlErpCustomerCredits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarlErpCustomerCreditDetails_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpCustomerCreditDetails_CarlErpItemIssueItems_ItemIssu~",
                        column: x => x.ItemIssueSaleItemId,
                        principalTable: "CarlErpItemIssueItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpCustomerCreditDetails_CarlErpLots_LotId",
                        column: x => x.LotId,
                        principalTable: "CarlErpLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpCustomerCreditDetails_CarlErpTaxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "CarlErpTaxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpInvoiceItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    InvoiceId = table.Column<Guid>(nullable: false),
                    ItemIssueItemId = table.Column<Guid>(nullable: true),
                    OrderItemId = table.Column<Guid>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: true),
                    TaxId = table.Column<long>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    MultiCurrencyUnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    IsItemIssue = table.Column<bool>(nullable: false),
                    LotId = table.Column<long>(nullable: true),
                    ParentId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpInvoiceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpInvoiceItems_CarlErpInvoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "CarlErpInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpInvoiceItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpInvoiceItems_CarlErpItemIssueItems_ItemIssueItemId",
                        column: x => x.ItemIssueItemId,
                        principalTable: "CarlErpItemIssueItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpInvoiceItems_CarlErpLots_LotId",
                        column: x => x.LotId,
                        principalTable: "CarlErpLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpInvoiceItems_CarlErpSaleOrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "CarlErpSaleOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpInvoiceItems_CarlErpTaxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "CarlErpTaxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemIssueItemBatchNos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    ItemIssueItemId = table.Column<Guid>(nullable: false),
                    BatchNoId = table.Column<Guid>(nullable: false),
                    Qty = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemIssueItemBatchNos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueItemBatchNos_CarlErpBatchNos_BatchNoId",
                        column: x => x.BatchNoId,
                        principalTable: "CarlErpBatchNos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueItemBatchNos_CarlErpItemIssueItems_ItemIssu~",
                        column: x => x.ItemIssueItemId,
                        principalTable: "CarlErpItemIssueItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpBillItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    BillId = table.Column<Guid>(nullable: false),
                    ItemReceiptItemId = table.Column<Guid>(nullable: true),
                    OrderItemId = table.Column<Guid>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: true),
                    TaxId = table.Column<long>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyUnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    IsItemReceipt = table.Column<bool>(nullable: false),
                    LotId = table.Column<long>(nullable: true),
                    ParentId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpBillItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpBillItems_CarlErpBills_BillId",
                        column: x => x.BillId,
                        principalTable: "CarlErpBills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpBillItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpBillItems_CarlErpItemReceiptItems_ItemReceiptItemId",
                        column: x => x.ItemReceiptItemId,
                        principalTable: "CarlErpItemReceiptItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpBillItems_CarlErpLots_LotId",
                        column: x => x.LotId,
                        principalTable: "CarlErpLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpBillItems_CarlErpPurchaseOrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "CarlErpPurchaseOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpBillItems_CarlErpTaxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "CarlErpTaxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemReceiptItemBatchNos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    ItemReceiptItemId = table.Column<Guid>(nullable: false),
                    BatchNoId = table.Column<Guid>(nullable: false),
                    Qty = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemReceiptItemBatchNos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptItemBatchNos_CarlErpBatchNos_BatchNoId",
                        column: x => x.BatchNoId,
                        principalTable: "CarlErpBatchNos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptItemBatchNos_CarlErpItemReceiptItems_Item~",
                        column: x => x.ItemReceiptItemId,
                        principalTable: "CarlErpItemReceiptItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpVendorCreditDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    VendorCreditId = table.Column<Guid>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: true),
                    TaxId = table.Column<long>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyUnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    LotId = table.Column<long>(nullable: true),
                    ItemReceiptItemId = table.Column<Guid>(nullable: true),
                    PurchaseCost = table.Column<decimal>(type: "decimal(19,6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpVendorCreditDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpVendorCreditDetails_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpVendorCreditDetails_CarlErpItemReceiptItems_ItemRece~",
                        column: x => x.ItemReceiptItemId,
                        principalTable: "CarlErpItemReceiptItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpVendorCreditDetails_CarlErpLots_LotId",
                        column: x => x.LotId,
                        principalTable: "CarlErpLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpVendorCreditDetails_CarlErpTaxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "CarlErpTaxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpVendorCreditDetails_CarlErpVendorCredit_VendorCredit~",
                        column: x => x.VendorCreditId,
                        principalTable: "CarlErpVendorCredit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpJournals",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    JournalNo = table.Column<string>(maxLength: 256, nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    DateOnly = table.Column<DateTime>(type: "Date", nullable: false),
                    Memo = table.Column<string>(nullable: true),
                    Debit = table.Column<decimal>(type: "decimal(28,6)", nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(28,6)", nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    CurrencyId = table.Column<long>(nullable: false),
                    MultiCurrencyId = table.Column<long>(nullable: true),
                    ClassId = table.Column<long>(nullable: true),
                    GeneralJournalId = table.Column<Guid>(nullable: true),
                    Reference = table.Column<string>(maxLength: 256, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    JournalType = table.Column<int>(nullable: false),
                    VendorCreditId = table.Column<Guid>(nullable: true),
                    BillId = table.Column<Guid>(nullable: true),
                    ItemReceiptId = table.Column<Guid>(nullable: true),
                    ItemReceiptCustomerCreditId = table.Column<Guid>(nullable: true),
                    ItemIssueVendorCreditId = table.Column<Guid>(nullable: true),
                    ItemIssueId = table.Column<Guid>(nullable: true),
                    PayBillId = table.Column<Guid>(nullable: true),
                    InvoiceId = table.Column<Guid>(nullable: true),
                    ReceivePaymentId = table.Column<Guid>(nullable: true),
                    CustomerCreditId = table.Column<Guid>(nullable: true),
                    WithdrawId = table.Column<Guid>(nullable: true),
                    DepositId = table.Column<Guid>(nullable: true),
                    LocationId = table.Column<long>(nullable: true),
                    CreationTimeIndex = table.Column<long>(maxLength: 19, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpJournals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpBills_BillId",
                        column: x => x.BillId,
                        principalTable: "CarlErpBills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpClasses_ClassId",
                        column: x => x.ClassId,
                        principalTable: "CarlErpClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpCurrencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpCustomerCredits_CustomerCreditId",
                        column: x => x.CustomerCreditId,
                        principalTable: "CarlErpCustomerCredits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpDeposits_DepositId",
                        column: x => x.DepositId,
                        principalTable: "CarlErpDeposits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpJournals_GeneralJournalId",
                        column: x => x.GeneralJournalId,
                        principalTable: "CarlErpJournals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpInvoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "CarlErpInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpItemIssues_ItemIssueId",
                        column: x => x.ItemIssueId,
                        principalTable: "CarlErpItemIssues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpItemIssueVendorCredit_ItemIssueVendo~",
                        column: x => x.ItemIssueVendorCreditId,
                        principalTable: "CarlErpItemIssueVendorCredit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpItemReceiptCustomerCredit_ItemReceip~",
                        column: x => x.ItemReceiptCustomerCreditId,
                        principalTable: "CarlErpItemReceiptCustomerCredit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpItemReceipts_ItemReceiptId",
                        column: x => x.ItemReceiptId,
                        principalTable: "CarlErpItemReceipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpCurrencies_MultiCurrencyId",
                        column: x => x.MultiCurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpPayBills_PayBillId",
                        column: x => x.PayBillId,
                        principalTable: "CarlErpPayBills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpReceivePayments_ReceivePaymentId",
                        column: x => x.ReceivePaymentId,
                        principalTable: "CarlErpReceivePayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpVendorCredit_VendorCreditId",
                        column: x => x.VendorCreditId,
                        principalTable: "CarlErpVendorCredit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpWithdraws_WithdrawId",
                        column: x => x.WithdrawId,
                        principalTable: "CarlErpWithdraws",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpCustomerCreditItemBatchNos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    CustomerCreditItemId = table.Column<Guid>(nullable: false),
                    BatchNoId = table.Column<Guid>(nullable: false),
                    Qty = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpCustomerCreditItemBatchNos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpCustomerCreditItemBatchNos_CarlErpBatchNos_BatchNoId",
                        column: x => x.BatchNoId,
                        principalTable: "CarlErpBatchNos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpCustomerCreditItemBatchNos_CarlErpCustomerCreditDeta~",
                        column: x => x.CustomerCreditItemId,
                        principalTable: "CarlErpCustomerCreditDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemReceiptCustomerCreditItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ItemReceiptCustomerCreditId = table.Column<Guid>(nullable: false),
                    CustomerCreditItemId = table.Column<Guid>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(28,6)", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(9,4)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(28,6)", nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    LotId = table.Column<long>(nullable: true),
                    ItemIssueSaleItemId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemReceiptCustomerCreditItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptCustomerCreditItem_CarlErpCustomerCreditD~",
                        column: x => x.CustomerCreditItemId,
                        principalTable: "CarlErpCustomerCreditDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptCustomerCreditItem_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptCustomerCreditItem_CarlErpItemIssueItems_~",
                        column: x => x.ItemIssueSaleItemId,
                        principalTable: "CarlErpItemIssueItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptCustomerCreditItem_CarlErpItemReceiptCust~",
                        column: x => x.ItemReceiptCustomerCreditId,
                        principalTable: "CarlErpItemReceiptCustomerCredit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptCustomerCreditItem_CarlErpLots_LotId",
                        column: x => x.LotId,
                        principalTable: "CarlErpLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemIssueVendorCreditItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ItemIssueVendorCreditId = table.Column<Guid>(nullable: false),
                    VendorCreditItemId = table.Column<Guid>(nullable: true),
                    ItemReceiptPurchaseItemId = table.Column<Guid>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(28,6)", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(9,4)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(28,6)", nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    LotId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemIssueVendorCreditItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueVendorCreditItem_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueVendorCreditItem_CarlErpItemIssueVendorCred~",
                        column: x => x.ItemIssueVendorCreditId,
                        principalTable: "CarlErpItemIssueVendorCredit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueVendorCreditItem_CarlErpItemReceiptItems_It~",
                        column: x => x.ItemReceiptPurchaseItemId,
                        principalTable: "CarlErpItemReceiptItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueVendorCreditItem_CarlErpLots_LotId",
                        column: x => x.LotId,
                        principalTable: "CarlErpLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueVendorCreditItem_CarlErpVendorCreditDetails~",
                        column: x => x.VendorCreditItemId,
                        principalTable: "CarlErpVendorCreditDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpVendorCreditItemBatchNos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    VendorCreditItemId = table.Column<Guid>(nullable: false),
                    BatchNoId = table.Column<Guid>(nullable: false),
                    Qty = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpVendorCreditItemBatchNos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpVendorCreditItemBatchNos_CarlErpBatchNos_BatchNoId",
                        column: x => x.BatchNoId,
                        principalTable: "CarlErpBatchNos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpVendorCreditItemBatchNos_CarlErpVendorCreditDetails_~",
                        column: x => x.VendorCreditItemId,
                        principalTable: "CarlErpVendorCreditDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpJournalItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    JournalId = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Debit = table.Column<decimal>(type: "decimal(28,6)", nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(28,6)", nullable: false),
                    Key = table.Column<int>(nullable: false),
                    Identifier = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpJournalItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpJournalItems_CarlErpChartOfAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournalItems_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournalItems_CarlErpJournals_JournalId",
                        column: x => x.JournalId,
                        principalTable: "CarlErpJournals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournalItems_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemReceiptCustomerCreditItemBatchNos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    ItemReceiptCustomerCreditItemId = table.Column<Guid>(nullable: false),
                    BatchNoId = table.Column<Guid>(nullable: false),
                    Qty = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemReceiptCustomerCreditItemBatchNos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptCustomerCreditItemBatchNos_CarlErpBatchNo~",
                        column: x => x.BatchNoId,
                        principalTable: "CarlErpBatchNos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptCustomerCreditItemBatchNos_CarlErpItemRec~",
                        column: x => x.ItemReceiptCustomerCreditItemId,
                        principalTable: "CarlErpItemReceiptCustomerCreditItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemIssueVendorCreditItemBatchNos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    ItemIssueVendorCreditItemId = table.Column<Guid>(nullable: false),
                    BatchNoId = table.Column<Guid>(nullable: false),
                    Qty = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemIssueVendorCreditItemBatchNos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueVendorCreditItemBatchNos_CarlErpBatchNos_Ba~",
                        column: x => x.BatchNoId,
                        principalTable: "CarlErpBatchNos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueVendorCreditItemBatchNos_CarlErpItemIssueVe~",
                        column: x => x.ItemIssueVendorCreditItemId,
                        principalTable: "CarlErpItemIssueVendorCreditItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbpAuditLogs_TenantId_ExecutionDuration",
                table: "AbpAuditLogs",
                columns: new[] { "TenantId", "ExecutionDuration" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpAuditLogs_TenantId_ExecutionTime",
                table: "AbpAuditLogs",
                columns: new[] { "TenantId", "ExecutionTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpAuditLogs_TenantId_UserId",
                table: "AbpAuditLogs",
                columns: new[] { "TenantId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpBackgroundJobs_IsAbandoned_NextTryTime",
                table: "AbpBackgroundJobs",
                columns: new[] { "IsAbandoned", "NextTryTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpEntityChanges_EntityChangeSetId",
                table: "AbpEntityChanges",
                column: "EntityChangeSetId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpEntityChanges_EntityTypeFullName_EntityId",
                table: "AbpEntityChanges",
                columns: new[] { "EntityTypeFullName", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpEntityChangeSets_TenantId_CreationTime",
                table: "AbpEntityChangeSets",
                columns: new[] { "TenantId", "CreationTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpEntityChangeSets_TenantId_Reason",
                table: "AbpEntityChangeSets",
                columns: new[] { "TenantId", "Reason" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpEntityChangeSets_TenantId_UserId",
                table: "AbpEntityChangeSets",
                columns: new[] { "TenantId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpEntityPropertyChanges_EntityChangeId",
                table: "AbpEntityPropertyChanges",
                column: "EntityChangeId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpFeatures_EditionId_Name",
                table: "AbpFeatures",
                columns: new[] { "EditionId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpFeatures_TenantId_Name",
                table: "AbpFeatures",
                columns: new[] { "TenantId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpLanguages_TenantId_Name",
                table: "AbpLanguages",
                columns: new[] { "TenantId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpLanguageTexts_TenantId_Source_LanguageName_Key",
                table: "AbpLanguageTexts",
                columns: new[] { "TenantId", "Source", "LanguageName", "Key" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpNotificationSubscriptions_NotificationName_EntityTypeNam~",
                table: "AbpNotificationSubscriptions",
                columns: new[] { "NotificationName", "EntityTypeName", "EntityId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpNotificationSubscriptions_TenantId_NotificationName_Enti~",
                table: "AbpNotificationSubscriptions",
                columns: new[] { "TenantId", "NotificationName", "EntityTypeName", "EntityId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpOrganizationUnits_ParentId",
                table: "AbpOrganizationUnits",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpOrganizationUnits_TenantId_Code",
                table: "AbpOrganizationUnits",
                columns: new[] { "TenantId", "Code" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpPermissions_TenantId_Name",
                table: "AbpPermissions",
                columns: new[] { "TenantId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpPermissions_RoleId",
                table: "AbpPermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpPermissions_UserId",
                table: "AbpPermissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpPersistedGrants_SubjectId_ClientId_Type",
                table: "AbpPersistedGrants",
                columns: new[] { "SubjectId", "ClientId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpRoleClaims_RoleId",
                table: "AbpRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpRoleClaims_TenantId_ClaimType",
                table: "AbpRoleClaims",
                columns: new[] { "TenantId", "ClaimType" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpRoles_CreatorUserId",
                table: "AbpRoles",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpRoles_DeleterUserId",
                table: "AbpRoles",
                column: "DeleterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpRoles_LastModifierUserId",
                table: "AbpRoles",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpRoles_TenantId_NormalizedName",
                table: "AbpRoles",
                columns: new[] { "TenantId", "NormalizedName" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpSettings_UserId",
                table: "AbpSettings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpSettings_TenantId_Name",
                table: "AbpSettings",
                columns: new[] { "TenantId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenantNotifications_TenantId",
                table: "AbpTenantNotifications",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_AccountCycleId",
                table: "AbpTenants",
                column: "AccountCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_BankTransferAccountId",
                table: "AbpTenants",
                column: "BankTransferAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_BillPaymentAccountId",
                table: "AbpTenants",
                column: "BillPaymentAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_COGSAccountId",
                table: "AbpTenants",
                column: "COGSAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ClassId",
                table: "AbpTenants",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_CreationTime",
                table: "AbpTenants",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_CreatorUserId",
                table: "AbpTenants",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_CurrencyId",
                table: "AbpTenants",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_CustomerAccountId",
                table: "AbpTenants",
                column: "CustomerAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_DeleterUserId",
                table: "AbpTenants",
                column: "DeleterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_EditionId",
                table: "AbpTenants",
                column: "EditionId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ExchangeLossAndGainId",
                table: "AbpTenants",
                column: "ExchangeLossAndGainId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ExpenseAccountId",
                table: "AbpTenants",
                column: "ExpenseAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_FinishProductionAccountId",
                table: "AbpTenants",
                column: "FinishProductionAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_FormatDateId",
                table: "AbpTenants",
                column: "FormatDateId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_FormatNumberId",
                table: "AbpTenants",
                column: "FormatNumberId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_InventoryAccountId",
                table: "AbpTenants",
                column: "InventoryAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ItemIssueAdjustmentId",
                table: "AbpTenants",
                column: "ItemIssueAdjustmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ItemIssueOtherId",
                table: "AbpTenants",
                column: "ItemIssueOtherId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ItemIssuePhysicalCountId",
                table: "AbpTenants",
                column: "ItemIssuePhysicalCountId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ItemIssueTransferId",
                table: "AbpTenants",
                column: "ItemIssueTransferId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ItemIssueVendorCreditId",
                table: "AbpTenants",
                column: "ItemIssueVendorCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ItemRecieptAdjustmentId",
                table: "AbpTenants",
                column: "ItemRecieptAdjustmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ItemRecieptCustomerCreditId",
                table: "AbpTenants",
                column: "ItemRecieptCustomerCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ItemRecieptOtherId",
                table: "AbpTenants",
                column: "ItemRecieptOtherId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ItemRecieptPhysicalCountId",
                table: "AbpTenants",
                column: "ItemRecieptPhysicalCountId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_ItemRecieptTransferId",
                table: "AbpTenants",
                column: "ItemRecieptTransferId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_LastModifierUserId",
                table: "AbpTenants",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_LocationId",
                table: "AbpTenants",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_POSCurrencyId",
                table: "AbpTenants",
                column: "POSCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_PropertyId",
                table: "AbpTenants",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_RawProductionAccountId",
                table: "AbpTenants",
                column: "RawProductionAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_RevenueAccountId",
                table: "AbpTenants",
                column: "RevenueAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_RoundDigitAccountId",
                table: "AbpTenants",
                column: "RoundDigitAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_SaleAllowanceAccountId",
                table: "AbpTenants",
                column: "SaleAllowanceAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_SubscriptionEndDateUtc",
                table: "AbpTenants",
                column: "SubscriptionEndDateUtc");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_SubscriptionId",
                table: "AbpTenants",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_TaxId",
                table: "AbpTenants",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_TenancyName",
                table: "AbpTenants",
                column: "TenancyName");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_TransitAccountId",
                table: "AbpTenants",
                column: "TransitAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpTenants_VendorAccountId",
                table: "AbpTenants",
                column: "VendorAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserAccounts_EmailAddress",
                table: "AbpUserAccounts",
                column: "EmailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserAccounts_UserName",
                table: "AbpUserAccounts",
                column: "UserName");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserAccounts_TenantId_EmailAddress",
                table: "AbpUserAccounts",
                columns: new[] { "TenantId", "EmailAddress" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserAccounts_TenantId_UserId",
                table: "AbpUserAccounts",
                columns: new[] { "TenantId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserAccounts_TenantId_UserName",
                table: "AbpUserAccounts",
                columns: new[] { "TenantId", "UserName" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserClaims_UserId",
                table: "AbpUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserClaims_TenantId_ClaimType",
                table: "AbpUserClaims",
                columns: new[] { "TenantId", "ClaimType" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserLoginAttempts_UserId_TenantId",
                table: "AbpUserLoginAttempts",
                columns: new[] { "UserId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserLoginAttempts_TenancyName_UserNameOrEmailAddress_Res~",
                table: "AbpUserLoginAttempts",
                columns: new[] { "TenancyName", "UserNameOrEmailAddress", "Result" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserLogins_UserId",
                table: "AbpUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserLogins_TenantId_UserId",
                table: "AbpUserLogins",
                columns: new[] { "TenantId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserLogins_TenantId_LoginProvider_ProviderKey",
                table: "AbpUserLogins",
                columns: new[] { "TenantId", "LoginProvider", "ProviderKey" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserNotifications_UserId_State_CreationTime",
                table: "AbpUserNotifications",
                columns: new[] { "UserId", "State", "CreationTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserOrganizationUnits_TenantId_OrganizationUnitId",
                table: "AbpUserOrganizationUnits",
                columns: new[] { "TenantId", "OrganizationUnitId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserOrganizationUnits_TenantId_UserId",
                table: "AbpUserOrganizationUnits",
                columns: new[] { "TenantId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserRoles_UserId",
                table: "AbpUserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserRoles_TenantId_RoleId",
                table: "AbpUserRoles",
                columns: new[] { "TenantId", "RoleId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserRoles_TenantId_UserId",
                table: "AbpUserRoles",
                columns: new[] { "TenantId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_CreatorUserId",
                table: "AbpUsers",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_DeleterUserId",
                table: "AbpUsers",
                column: "DeleterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_LastModifierUserId",
                table: "AbpUsers",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_TenantId_NormalizedEmailAddress",
                table: "AbpUsers",
                columns: new[] { "TenantId", "NormalizedEmailAddress" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_TenantId_NormalizedUserName",
                table: "AbpUsers",
                columns: new[] { "TenantId", "NormalizedUserName" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserTokens_UserId",
                table: "AbpUserTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserTokens_TenantId_UserId",
                table: "AbpUserTokens",
                columns: new[] { "TenantId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AppBinaryObjects_TenantId",
                table: "AppBinaryObjects",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AppChatMessages_TargetTenantId_TargetUserId_ReadState",
                table: "AppChatMessages",
                columns: new[] { "TargetTenantId", "TargetUserId", "ReadState" });

            migrationBuilder.CreateIndex(
                name: "IX_AppChatMessages_TargetTenantId_UserId_ReadState",
                table: "AppChatMessages",
                columns: new[] { "TargetTenantId", "UserId", "ReadState" });

            migrationBuilder.CreateIndex(
                name: "IX_AppChatMessages_TenantId_TargetUserId_ReadState",
                table: "AppChatMessages",
                columns: new[] { "TenantId", "TargetUserId", "ReadState" });

            migrationBuilder.CreateIndex(
                name: "IX_AppChatMessages_TenantId_UserId_ReadState",
                table: "AppChatMessages",
                columns: new[] { "TenantId", "UserId", "ReadState" });

            migrationBuilder.CreateIndex(
                name: "IX_AppFriendships_FriendTenantId_FriendUserId",
                table: "AppFriendships",
                columns: new[] { "FriendTenantId", "FriendUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AppFriendships_FriendTenantId_UserId",
                table: "AppFriendships",
                columns: new[] { "FriendTenantId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AppFriendships_TenantId_FriendUserId",
                table: "AppFriendships",
                columns: new[] { "TenantId", "FriendUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AppFriendships_TenantId_UserId",
                table: "AppFriendships",
                columns: new[] { "TenantId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_AppSubscriptionPayments_EditionId",
                table: "AppSubscriptionPayments",
                column: "EditionId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSubscriptionPayments_PaymentId_Gateway",
                table: "AppSubscriptionPayments",
                columns: new[] { "PaymentId", "Gateway" });

            migrationBuilder.CreateIndex(
                name: "IX_AppSubscriptionPayments_Status_CreationTime",
                table: "AppSubscriptionPayments",
                columns: new[] { "Status", "CreationTime" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpAccountCycles_StartDate_EndDate",
                table: "CarlErpAccountCycles",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpAccountTransactionCloses_AccountCycleId",
                table: "CarlErpAccountTransactionCloses",
                column: "AccountCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpAccountTransactionCloses_AccountId",
                table: "CarlErpAccountTransactionCloses",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpAccountTransactionCloses_CurrencyId",
                table: "CarlErpAccountTransactionCloses",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpAccountTransactionCloses_DateOnly",
                table: "CarlErpAccountTransactionCloses",
                column: "DateOnly");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpAccountTransactionCloses_LocationId",
                table: "CarlErpAccountTransactionCloses",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpAccountTransactionCloses_TenantId_CreatorUserId_Bala~",
                table: "CarlErpAccountTransactionCloses",
                columns: new[] { "TenantId", "CreatorUserId", "Balance", "Debit", "Credit" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpAccountTypes_AccountTypeName",
                table: "CarlErpAccountTypes",
                column: "AccountTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpAccountTypes_CreatorUserId",
                table: "CarlErpAccountTypes",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpAutoSequences_DocumentType_NumberFormat_YearFormat_D~",
                table: "CarlErpAutoSequences",
                columns: new[] { "DocumentType", "NumberFormat", "YearFormat", "DefaultPrefix" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBankTransfers_BankTransferFromAccountId",
                table: "CarlErpBankTransfers",
                column: "BankTransferFromAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBankTransfers_BankTransferToAccountId",
                table: "CarlErpBankTransfers",
                column: "BankTransferToAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBankTransfers_CreatorUserId",
                table: "CarlErpBankTransfers",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBankTransfers_FromLocationId",
                table: "CarlErpBankTransfers",
                column: "FromLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBankTransfers_LastModifierUserId",
                table: "CarlErpBankTransfers",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBankTransfers_ToLocationId",
                table: "CarlErpBankTransfers",
                column: "ToLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBankTransfers_TransferFromClassId",
                table: "CarlErpBankTransfers",
                column: "TransferFromClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBankTransfers_TransferToClassId",
                table: "CarlErpBankTransfers",
                column: "TransferToClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBankTransfers_TenantId_BankTransferNo",
                table: "CarlErpBankTransfers",
                columns: new[] { "TenantId", "BankTransferNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBatchNos_ExpirationDate",
                table: "CarlErpBatchNos",
                column: "ExpirationDate");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBatchNos_IsStandard",
                table: "CarlErpBatchNos",
                column: "IsStandard");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBatchNos_TenantId_Code_ItemId",
                table: "CarlErpBatchNos",
                columns: new[] { "TenantId", "Code", "ItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBillInvoiceSettings_SettingType",
                table: "CarlErpBillInvoiceSettings",
                column: "SettingType");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBillInvoiceSettings_TenantId_CreatorUserId_SettingTy~",
                table: "CarlErpBillInvoiceSettings",
                columns: new[] { "TenantId", "CreatorUserId", "SettingType" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBillItems_BillId",
                table: "CarlErpBillItems",
                column: "BillId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBillItems_ItemId",
                table: "CarlErpBillItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBillItems_ItemReceiptItemId",
                table: "CarlErpBillItems",
                column: "ItemReceiptItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBillItems_LotId",
                table: "CarlErpBillItems",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBillItems_OrderItemId",
                table: "CarlErpBillItems",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBillItems_TaxId",
                table: "CarlErpBillItems",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBillItems_TenantId_CreatorUserId",
                table: "CarlErpBillItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBillItems_TenantId_ParentId",
                table: "CarlErpBillItems",
                columns: new[] { "TenantId", "ParentId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBills_ItemReceiptId",
                table: "CarlErpBills",
                column: "ItemReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBills_VendorId",
                table: "CarlErpBills",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBills_TenantId_CreatorUserId_VendorId",
                table: "CarlErpBills",
                columns: new[] { "TenantId", "CreatorUserId", "VendorId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpChartOfAccounts_AccountTypeId",
                table: "CarlErpChartOfAccounts",
                column: "AccountTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpChartOfAccounts_ParentAccountId",
                table: "CarlErpChartOfAccounts",
                column: "ParentAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpChartOfAccounts_TaxId",
                table: "CarlErpChartOfAccounts",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpChartOfAccounts_TenantId_AccountCode",
                table: "CarlErpChartOfAccounts",
                columns: new[] { "TenantId", "AccountCode" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpChartOfAccounts_TenantId_AccountName",
                table: "CarlErpChartOfAccounts",
                columns: new[] { "TenantId", "AccountName" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpChartOfAccounts_TenantId_CreatorUserId",
                table: "CarlErpChartOfAccounts",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpClasses_ParentClassId",
                table: "CarlErpClasses",
                column: "ParentClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpClasses_TenantId_ClassName",
                table: "CarlErpClasses",
                columns: new[] { "TenantId", "ClassName" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpContactPersons_VenderId",
                table: "CarlErpContactPersons",
                column: "VenderId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpContactPersons_TenantId_CreatorUserId",
                table: "CarlErpContactPersons",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCurrencies_CreatorUserId",
                table: "CarlErpCurrencies",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerContactPersons_CustomerId",
                table: "CarlErpCustomerContactPersons",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerContactPersons_TenantId_CreatorUserId",
                table: "CarlErpCustomerContactPersons",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerCreditDetails_CustomerCreditId",
                table: "CarlErpCustomerCreditDetails",
                column: "CustomerCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerCreditDetails_ItemId",
                table: "CarlErpCustomerCreditDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerCreditDetails_ItemIssueSaleItemId",
                table: "CarlErpCustomerCreditDetails",
                column: "ItemIssueSaleItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerCreditDetails_LotId",
                table: "CarlErpCustomerCreditDetails",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerCreditDetails_TaxId",
                table: "CarlErpCustomerCreditDetails",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerCreditDetails_TenantId_CreatorUserId",
                table: "CarlErpCustomerCreditDetails",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerCreditItemBatchNos_BatchNoId",
                table: "CarlErpCustomerCreditItemBatchNos",
                column: "BatchNoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerCreditItemBatchNos_CustomerCreditItemId",
                table: "CarlErpCustomerCreditItemBatchNos",
                column: "CustomerCreditItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerCredits_CustomerId",
                table: "CarlErpCustomerCredits",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerCredits_ItemIssueSaleId",
                table: "CarlErpCustomerCredits",
                column: "ItemIssueSaleId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerCredits_TenantId_CreatorUserId_CustomerId",
                table: "CarlErpCustomerCredits",
                columns: new[] { "TenantId", "CreatorUserId", "CustomerId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerGroups_CustomerId",
                table: "CarlErpCustomerGroups",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerGroups_UserGroupId",
                table: "CarlErpCustomerGroups",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerGroups_TenantId_CreatorUserId_CustomerId_Use~",
                table: "CarlErpCustomerGroups",
                columns: new[] { "TenantId", "CreatorUserId", "CustomerId", "UserGroupId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomers_AccountId",
                table: "CarlErpCustomers",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomers_CustomerTypeId",
                table: "CarlErpCustomers",
                column: "CustomerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomers_TenantId_CustomerCode",
                table: "CarlErpCustomers",
                columns: new[] { "TenantId", "CustomerCode" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomers_TenantId_CustomerName",
                table: "CarlErpCustomers",
                columns: new[] { "TenantId", "CustomerName" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerTypeMembers_CustomerTypeId",
                table: "CarlErpCustomerTypeMembers",
                column: "CustomerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerTypeMembers_MemberId",
                table: "CarlErpCustomerTypeMembers",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerTypes_TenantId_CreatorUserId_CustomerTypeName",
                table: "CarlErpCustomerTypes",
                columns: new[] { "TenantId", "CreatorUserId", "CustomerTypeName" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpDepositItems_AccountId",
                table: "CarlErpDepositItems",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpDepositItems_DepositId",
                table: "CarlErpDepositItems",
                column: "DepositId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpDepositItems_TenantId_CreatorUserId",
                table: "CarlErpDepositItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpDeposits_ReceiveFromCustomerId",
                table: "CarlErpDeposits",
                column: "ReceiveFromCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpDeposits_ReceiveFromVendorId",
                table: "CarlErpDeposits",
                column: "ReceiveFromVendorId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpDeposits_TenantId_CreatorUserId",
                table: "CarlErpDeposits",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpExchangeItems_ExchangeId",
                table: "CarlErpExchangeItems",
                column: "ExchangeId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpExchangeItems_FromCurrencyId",
                table: "CarlErpExchangeItems",
                column: "FromCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpExchangeItems_ToCurencyId",
                table: "CarlErpExchangeItems",
                column: "ToCurencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpExchangeItems_TenantId_CreatorUserId",
                table: "CarlErpExchangeItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpExchanges_TenantId_CreatorUserId",
                table: "CarlErpExchanges",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpFinishItems_ItemId",
                table: "CarlErpFinishItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpFinishItems_ProductionId",
                table: "CarlErpFinishItems",
                column: "ProductionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpFinishItems_ToLotId",
                table: "CarlErpFinishItems",
                column: "ToLotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpFinishItems_TenantId_CreatorUserId",
                table: "CarlErpFinishItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpFormats_Name_Key",
                table: "CarlErpFormats",
                columns: new[] { "Name", "Key" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpGalleries_TenantId_Name",
                table: "CarlErpGalleries",
                columns: new[] { "TenantId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpGalleries_TenantId_UploadFrom",
                table: "CarlErpGalleries",
                columns: new[] { "TenantId", "UploadFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpGalleries_TenantId_UploadSource",
                table: "CarlErpGalleries",
                columns: new[] { "TenantId", "UploadSource" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpGroupMemberItemTamplates_MemberUserId",
                table: "CarlErpGroupMemberItemTamplates",
                column: "MemberUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpGroupMemberItemTamplates_ReportTemplateId",
                table: "CarlErpGroupMemberItemTamplates",
                column: "ReportTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpGroupMemberItemTamplates_UserGroupId",
                table: "CarlErpGroupMemberItemTamplates",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpGroupMemberItemTamplates_CreatorUserId_TenantId",
                table: "CarlErpGroupMemberItemTamplates",
                columns: new[] { "CreatorUserId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInoviceTemplates_GalleryId",
                table: "CarlErpInoviceTemplates",
                column: "GalleryId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInoviceTemplates_TenantId_IsActive",
                table: "CarlErpInoviceTemplates",
                columns: new[] { "TenantId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInoviceTemplates_TenantId_Name",
                table: "CarlErpInoviceTemplates",
                columns: new[] { "TenantId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInoviceTemplates_TenantId_TemplateType",
                table: "CarlErpInoviceTemplates",
                columns: new[] { "TenantId", "TemplateType" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCalculationItems_CreatorUserId",
                table: "CarlErpInventoryCalculationItems",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCalculationItems_ItemId",
                table: "CarlErpInventoryCalculationItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCalculationItems_LastModifierUserId",
                table: "CarlErpInventoryCalculationItems",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCostCloseItemBatchNos_BatchNoId",
                table: "CarlErpInventoryCostCloseItemBatchNos",
                column: "BatchNoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCostCloseItemBatchNos_InventoryCostCloseId",
                table: "CarlErpInventoryCostCloseItemBatchNos",
                column: "InventoryCostCloseId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCostCloseItemBatchNos_LotId",
                table: "CarlErpInventoryCostCloseItemBatchNos",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCostCloseItems_InventoryCostCloseId",
                table: "CarlErpInventoryCostCloseItems",
                column: "InventoryCostCloseId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCostCloseItems_LotId",
                table: "CarlErpInventoryCostCloseItems",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCostCloseItems_TenantId_CreatorUserId",
                table: "CarlErpInventoryCostCloseItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCostCloses_AccountCycleId",
                table: "CarlErpInventoryCostCloses",
                column: "AccountCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCostCloses_ItemId",
                table: "CarlErpInventoryCostCloses",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCostCloses_LocationId",
                table: "CarlErpInventoryCostCloses",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryCostCloses_TenantId_CreatorUserId",
                table: "CarlErpInventoryCostCloses",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryTransactionItems_Date",
                table: "CarlErpInventoryTransactionItems",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryTransactionItems_InventoryAccountId",
                table: "CarlErpInventoryTransactionItems",
                column: "InventoryAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryTransactionItems_IsItemReceipt",
                table: "CarlErpInventoryTransactionItems",
                column: "IsItemReceipt");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryTransactionItems_ItemId",
                table: "CarlErpInventoryTransactionItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryTransactionItems_JournalId",
                table: "CarlErpInventoryTransactionItems",
                column: "JournalId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryTransactionItems_JournalType",
                table: "CarlErpInventoryTransactionItems",
                column: "JournalType");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryTransactionItems_LocationId",
                table: "CarlErpInventoryTransactionItems",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryTransactionItems_LotId",
                table: "CarlErpInventoryTransactionItems",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryTransactionItems_OrderIndex",
                table: "CarlErpInventoryTransactionItems",
                column: "OrderIndex");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryTransactionItems_TransactionId",
                table: "CarlErpInventoryTransactionItems",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryTransactionItems_TransferOrProductionId",
                table: "CarlErpInventoryTransactionItems",
                column: "TransferOrProductionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInventoryTransactionItems_TransferOrProductionItemId",
                table: "CarlErpInventoryTransactionItems",
                column: "TransferOrProductionItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoiceItems_InvoiceId",
                table: "CarlErpInvoiceItems",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoiceItems_ItemId",
                table: "CarlErpInvoiceItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoiceItems_ItemIssueItemId",
                table: "CarlErpInvoiceItems",
                column: "ItemIssueItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoiceItems_LotId",
                table: "CarlErpInvoiceItems",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoiceItems_OrderItemId",
                table: "CarlErpInvoiceItems",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoiceItems_TaxId",
                table: "CarlErpInvoiceItems",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoiceItems_TenantId_CreatorUserId",
                table: "CarlErpInvoiceItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoiceItems_TenantId_ParentId",
                table: "CarlErpInvoiceItems",
                columns: new[] { "TenantId", "ParentId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoices_CustomerId",
                table: "CarlErpInvoices",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoices_ItemIssueId",
                table: "CarlErpInvoices",
                column: "ItemIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoices_TransactionTypeSaleId",
                table: "CarlErpInvoices",
                column: "TransactionTypeSaleId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoices_TenantId_CreatorUserId_CustomerId",
                table: "CarlErpInvoices",
                columns: new[] { "TenantId", "CreatorUserId", "CustomerId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoiceTemplateMaps_SaleTypeId",
                table: "CarlErpInvoiceTemplateMaps",
                column: "SaleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoiceTemplateMaps_TemplateId",
                table: "CarlErpInvoiceTemplateMaps",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoiceTemplateMaps_TenantId_TemplateType",
                table: "CarlErpInvoiceTemplateMaps",
                columns: new[] { "TenantId", "TemplateType" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueItemBatchNos_BatchNoId",
                table: "CarlErpItemIssueItemBatchNos",
                column: "BatchNoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueItemBatchNos_ItemIssueItemId",
                table: "CarlErpItemIssueItemBatchNos",
                column: "ItemIssueItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueItems_ItemId",
                table: "CarlErpItemIssueItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueItems_ItemIssueId",
                table: "CarlErpItemIssueItems",
                column: "ItemIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueItems_LotId",
                table: "CarlErpItemIssueItems",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueItems_RawMaterialItemId",
                table: "CarlErpItemIssueItems",
                column: "RawMaterialItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueItems_SaleOrderItemId",
                table: "CarlErpItemIssueItems",
                column: "SaleOrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueItems_TransferOrderItemId",
                table: "CarlErpItemIssueItems",
                column: "TransferOrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueItems_TenantId_CreatorUserId",
                table: "CarlErpItemIssueItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssues_CustomerId",
                table: "CarlErpItemIssues",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssues_PhysicalCountId",
                table: "CarlErpItemIssues",
                column: "PhysicalCountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssues_ProductionOrderId",
                table: "CarlErpItemIssues",
                column: "ProductionOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssues_ProductionProcessId",
                table: "CarlErpItemIssues",
                column: "ProductionProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssues_TransactionTypeSaleId",
                table: "CarlErpItemIssues",
                column: "TransactionTypeSaleId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssues_TransferOrderId",
                table: "CarlErpItemIssues",
                column: "TransferOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssues_TenantId_CreatorUserId_CustomerId",
                table: "CarlErpItemIssues",
                columns: new[] { "TenantId", "CreatorUserId", "CustomerId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCredit_ItemReceiptPurchaseId",
                table: "CarlErpItemIssueVendorCredit",
                column: "ItemReceiptPurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCredit_VendorCreditId",
                table: "CarlErpItemIssueVendorCredit",
                column: "VendorCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCredit_VendorId",
                table: "CarlErpItemIssueVendorCredit",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCredit_TenantId_CreatorUserId_VendorId",
                table: "CarlErpItemIssueVendorCredit",
                columns: new[] { "TenantId", "CreatorUserId", "VendorId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCreditItem_ItemId",
                table: "CarlErpItemIssueVendorCreditItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCreditItem_ItemIssueVendorCreditId",
                table: "CarlErpItemIssueVendorCreditItem",
                column: "ItemIssueVendorCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCreditItem_ItemReceiptPurchaseItemId",
                table: "CarlErpItemIssueVendorCreditItem",
                column: "ItemReceiptPurchaseItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCreditItem_LotId",
                table: "CarlErpItemIssueVendorCreditItem",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCreditItem_VendorCreditItemId",
                table: "CarlErpItemIssueVendorCreditItem",
                column: "VendorCreditItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCreditItem_TenantId_CreatorUserId",
                table: "CarlErpItemIssueVendorCreditItem",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCreditItemBatchNos_BatchNoId",
                table: "CarlErpItemIssueVendorCreditItemBatchNos",
                column: "BatchNoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCreditItemBatchNos_ItemIssueVendorCre~",
                table: "CarlErpItemIssueVendorCreditItemBatchNos",
                column: "ItemIssueVendorCreditItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemLots_ItemId",
                table: "CarlErpItemLots",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemLots_LotId",
                table: "CarlErpItemLots",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemLots_TenantId_ItemId_LotId",
                table: "CarlErpItemLots",
                columns: new[] { "TenantId", "ItemId", "LotId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemPriceItems_CurrencyId",
                table: "CarlErpItemPriceItems",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemPriceItems_ItemId",
                table: "CarlErpItemPriceItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemPriceItems_ItemPriceId",
                table: "CarlErpItemPriceItems",
                column: "ItemPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemPriceItems_TenantId_CreatorUserId",
                table: "CarlErpItemPriceItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemPrices_CustomerTypeId",
                table: "CarlErpItemPrices",
                column: "CustomerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemPrices_LocationId",
                table: "CarlErpItemPrices",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemPrices_TransactionTypeSaleId",
                table: "CarlErpItemPrices",
                column: "TransactionTypeSaleId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemPrices_TenantId_CreatorUserId",
                table: "CarlErpItemPrices",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemProperties_ItemId",
                table: "CarlErpItemProperties",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemProperties_PropertyId",
                table: "CarlErpItemProperties",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemProperties_PropertyValueId",
                table: "CarlErpItemProperties",
                column: "PropertyValueId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemProperties_CreatorUserId_TenantId",
                table: "CarlErpItemProperties",
                columns: new[] { "CreatorUserId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptCustomerCredit_CustomerCreditId",
                table: "CarlErpItemReceiptCustomerCredit",
                column: "CustomerCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptCustomerCredit_CustomerId",
                table: "CarlErpItemReceiptCustomerCredit",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptCustomerCredit_ItemIssueSaleId",
                table: "CarlErpItemReceiptCustomerCredit",
                column: "ItemIssueSaleId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptCustomerCredit_TenantId_CreatorUserId_Cus~",
                table: "CarlErpItemReceiptCustomerCredit",
                columns: new[] { "TenantId", "CreatorUserId", "CustomerId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptCustomerCreditItem_CustomerCreditItemId",
                table: "CarlErpItemReceiptCustomerCreditItem",
                column: "CustomerCreditItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptCustomerCreditItem_ItemId",
                table: "CarlErpItemReceiptCustomerCreditItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptCustomerCreditItem_ItemIssueSaleItemId",
                table: "CarlErpItemReceiptCustomerCreditItem",
                column: "ItemIssueSaleItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptCustomerCreditItem_ItemReceiptCustomerCre~",
                table: "CarlErpItemReceiptCustomerCreditItem",
                column: "ItemReceiptCustomerCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptCustomerCreditItem_LotId",
                table: "CarlErpItemReceiptCustomerCreditItem",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptCustomerCreditItem_TenantId_CreatorUserId",
                table: "CarlErpItemReceiptCustomerCreditItem",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptCustomerCreditItemBatchNos_BatchNoId",
                table: "CarlErpItemReceiptCustomerCreditItemBatchNos",
                column: "BatchNoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptCustomerCreditItemBatchNos_ItemReceiptCus~",
                table: "CarlErpItemReceiptCustomerCreditItemBatchNos",
                column: "ItemReceiptCustomerCreditItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptItemBatchNos_BatchNoId",
                table: "CarlErpItemReceiptItemBatchNos",
                column: "BatchNoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptItemBatchNos_ItemReceiptItemId",
                table: "CarlErpItemReceiptItemBatchNos",
                column: "ItemReceiptItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptItems_FinishItemId",
                table: "CarlErpItemReceiptItems",
                column: "FinishItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptItems_ItemId",
                table: "CarlErpItemReceiptItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptItems_ItemReceiptId",
                table: "CarlErpItemReceiptItems",
                column: "ItemReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptItems_LotId",
                table: "CarlErpItemReceiptItems",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptItems_OrderItemId",
                table: "CarlErpItemReceiptItems",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptItems_TransferOrderItemId",
                table: "CarlErpItemReceiptItems",
                column: "TransferOrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptItems_TenantId_CreatorUserId",
                table: "CarlErpItemReceiptItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceipts_PhysicalCountId",
                table: "CarlErpItemReceipts",
                column: "PhysicalCountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceipts_ProductionOrderId",
                table: "CarlErpItemReceipts",
                column: "ProductionOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceipts_ProductionProcessId",
                table: "CarlErpItemReceipts",
                column: "ProductionProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceipts_TransferOrderId",
                table: "CarlErpItemReceipts",
                column: "TransferOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceipts_VendorId",
                table: "CarlErpItemReceipts",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceipts_TenantId_CreatorUserId_VendorId",
                table: "CarlErpItemReceipts",
                columns: new[] { "TenantId", "CreatorUserId", "VendorId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItems_BatchNoFormulaId",
                table: "CarlErpItems",
                column: "BatchNoFormulaId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItems_ImageId",
                table: "CarlErpItems",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItems_InventoryAccountId",
                table: "CarlErpItems",
                column: "InventoryAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItems_ItemTypeId",
                table: "CarlErpItems",
                column: "ItemTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItems_PurchaseAccountId",
                table: "CarlErpItems",
                column: "PurchaseAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItems_PurchaseCurrencyId",
                table: "CarlErpItems",
                column: "PurchaseCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItems_PurchaseTaxId",
                table: "CarlErpItems",
                column: "PurchaseTaxId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItems_SaleAccountId",
                table: "CarlErpItems",
                column: "SaleAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItems_SaleCurrenyId",
                table: "CarlErpItems",
                column: "SaleCurrenyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItems_SaleTaxId",
                table: "CarlErpItems",
                column: "SaleTaxId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItems_TenantId_Barcode",
                table: "CarlErpItems",
                columns: new[] { "TenantId", "Barcode" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItems_TenantId_CreatorUserId",
                table: "CarlErpItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItems_TenantId_ItemCode",
                table: "CarlErpItems",
                columns: new[] { "TenantId", "ItemCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItems_TenantId_ItemName",
                table: "CarlErpItems",
                columns: new[] { "TenantId", "ItemName" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemTypes_CreatorUserId",
                table: "CarlErpItemTypes",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemUserGroups_ItemId",
                table: "CarlErpItemUserGroups",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemUserGroups_UserGroupId",
                table: "CarlErpItemUserGroups",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemUserGroups_CreatorUserId_TenantId_UserGroupId_It~",
                table: "CarlErpItemUserGroups",
                columns: new[] { "CreatorUserId", "TenantId", "UserGroupId", "ItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournalItems_AccountId",
                table: "CarlErpJournalItems",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournalItems_CreatorUserId",
                table: "CarlErpJournalItems",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournalItems_JournalId",
                table: "CarlErpJournalItems",
                column: "JournalId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournalItems_LastModifierUserId",
                table: "CarlErpJournalItems",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournalItems_Identifier_Key",
                table: "CarlErpJournalItems",
                columns: new[] { "Identifier", "Key" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournalItems_TenantId_CreatorUserId",
                table: "CarlErpJournalItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_BillId",
                table: "CarlErpJournals",
                column: "BillId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_ClassId",
                table: "CarlErpJournals",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_CreatorUserId",
                table: "CarlErpJournals",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_CurrencyId",
                table: "CarlErpJournals",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_CustomerCreditId",
                table: "CarlErpJournals",
                column: "CustomerCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_Date",
                table: "CarlErpJournals",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_DateOnly",
                table: "CarlErpJournals",
                column: "DateOnly");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_DepositId",
                table: "CarlErpJournals",
                column: "DepositId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_GeneralJournalId",
                table: "CarlErpJournals",
                column: "GeneralJournalId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_InvoiceId",
                table: "CarlErpJournals",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_ItemIssueId",
                table: "CarlErpJournals",
                column: "ItemIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_ItemIssueVendorCreditId",
                table: "CarlErpJournals",
                column: "ItemIssueVendorCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_ItemReceiptCustomerCreditId",
                table: "CarlErpJournals",
                column: "ItemReceiptCustomerCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_ItemReceiptId",
                table: "CarlErpJournals",
                column: "ItemReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_JournalNo",
                table: "CarlErpJournals",
                column: "JournalNo");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_JournalType",
                table: "CarlErpJournals",
                column: "JournalType");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_LastModifierUserId",
                table: "CarlErpJournals",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_LocationId",
                table: "CarlErpJournals",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_MultiCurrencyId",
                table: "CarlErpJournals",
                column: "MultiCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_PayBillId",
                table: "CarlErpJournals",
                column: "PayBillId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_ReceivePaymentId",
                table: "CarlErpJournals",
                column: "ReceivePaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_Status",
                table: "CarlErpJournals",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_VendorCreditId",
                table: "CarlErpJournals",
                column: "VendorCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_WithdrawId",
                table: "CarlErpJournals",
                column: "WithdrawId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_TenantId_CreatorUserId",
                table: "CarlErpJournals",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_TenantId_Date",
                table: "CarlErpJournals",
                columns: new[] { "TenantId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_TenantId_JournalType_JournalNo",
                table: "CarlErpJournals",
                columns: new[] { "TenantId", "JournalType", "JournalNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpLocations_ParentLocationId",
                table: "CarlErpLocations",
                column: "ParentLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpLocations_TenantId_LocationName",
                table: "CarlErpLocations",
                columns: new[] { "TenantId", "LocationName" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpLocks_TenantId_CreatorUserId_LockKey",
                table: "CarlErpLocks",
                columns: new[] { "TenantId", "CreatorUserId", "LockKey" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpLots_LocationId",
                table: "CarlErpLots",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpLots_TenantId_LotName",
                table: "CarlErpLots",
                columns: new[] { "TenantId", "LotName" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpMultiCurrencies_CreatorUserId",
                table: "CarlErpMultiCurrencies",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpMultiCurrencies_CurrencyId",
                table: "CarlErpMultiCurrencies",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPayBillDeail_BillId",
                table: "CarlErpPayBillDeail",
                column: "BillId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPayBillDeail_PayBillId",
                table: "CarlErpPayBillDeail",
                column: "PayBillId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPayBillDeail_VendorCreditId",
                table: "CarlErpPayBillDeail",
                column: "VendorCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPayBillDeail_VendorId",
                table: "CarlErpPayBillDeail",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPayBillDeail_TenantId_CreatorUserId",
                table: "CarlErpPayBillDeail",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPayBillExpense_AccountId",
                table: "CarlErpPayBillExpense",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPayBillExpense_PayBillId",
                table: "CarlErpPayBillExpense",
                column: "PayBillId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPayBillExpense_TenantId_CreatorUserId",
                table: "CarlErpPayBillExpense",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPayBills_PaymentMethodId",
                table: "CarlErpPayBills",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPayBills_TenantId_CreatorUserId",
                table: "CarlErpPayBills",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPaymentMethodBases_Name",
                table: "CarlErpPaymentMethodBases",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPaymentMethods_AccountId",
                table: "CarlErpPaymentMethods",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPaymentMethods_CreatorUserId",
                table: "CarlErpPaymentMethods",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPaymentMethods_LastModifierUserId",
                table: "CarlErpPaymentMethods",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPaymentMethods_PaymentMethodId",
                table: "CarlErpPaymentMethods",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPaymentMethodUserGroups_PaymentMethodId",
                table: "CarlErpPaymentMethodUserGroups",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPaymentMethodUserGroups_UserGroupId",
                table: "CarlErpPaymentMethodUserGroups",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPermissionLocks_LocationId",
                table: "CarlErpPermissionLocks",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPermissionLocks_LockTransaction",
                table: "CarlErpPermissionLocks",
                column: "LockTransaction");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPermissionLocks_TransactionNo",
                table: "CarlErpPermissionLocks",
                column: "TransactionNo");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPermissionLockScheduleItems_TransactionLockSheduleId",
                table: "CarlErpPermissionLockScheduleItems",
                column: "TransactionLockSheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPermissionLockSchedules_ScheduleDate",
                table: "CarlErpPermissionLockSchedules",
                column: "ScheduleDate");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPermissionLockSchedules_ScheduleType",
                table: "CarlErpPermissionLockSchedules",
                column: "ScheduleType");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPhysicalCountItems_ItemId",
                table: "CarlErpPhysicalCountItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPhysicalCountItems_PhysicalCountId",
                table: "CarlErpPhysicalCountItems",
                column: "PhysicalCountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPhysicalCountItems_TenantId_CreatorUserId",
                table: "CarlErpPhysicalCountItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPhysicalCounts_ClassId",
                table: "CarlErpPhysicalCounts",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPhysicalCounts_LocationId",
                table: "CarlErpPhysicalCounts",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPhysicalCounts_TenantId_CreatorUserId_PhysicalCountNo",
                table: "CarlErpPhysicalCounts",
                columns: new[] { "TenantId", "CreatorUserId", "PhysicalCountNo" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionLines_TenantId_ProductionLineName",
                table: "CarlErpProductionLines",
                columns: new[] { "TenantId", "ProductionLineName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionPlans_CreatorUserId",
                table: "CarlErpProductionPlans",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionPlans_LastModifierUserId",
                table: "CarlErpProductionPlans",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionPlans_LocationId",
                table: "CarlErpProductionPlans",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionPlans_ProductionLineId",
                table: "CarlErpProductionPlans",
                column: "ProductionLineId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionPlans_Reference",
                table: "CarlErpProductionPlans",
                column: "Reference");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionPlans_TenantId_DocumentNo",
                table: "CarlErpProductionPlans",
                columns: new[] { "TenantId", "DocumentNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionProcess_AccountId",
                table: "CarlErpProductionProcess",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionProcess_ProductionProcessType",
                table: "CarlErpProductionProcess",
                column: "ProductionProcessType");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionProcess_TenantId_CreatorUserId_ProcessName",
                table: "CarlErpProductionProcess",
                columns: new[] { "TenantId", "CreatorUserId", "ProcessName" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionStandardCostGroups_ProductionId",
                table: "CarlErpProductionStandardCostGroups",
                column: "ProductionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionStandardCostGroups_StandardCostGroupId",
                table: "CarlErpProductionStandardCostGroups",
                column: "StandardCostGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionStandardCosts_ProductionPlanId",
                table: "CarlErpProductionStandardCosts",
                column: "ProductionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProductionStandardCosts_StandardCostGroupId",
                table: "CarlErpProductionStandardCosts",
                column: "StandardCostGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProperties_TenantId_CreatorUserId",
                table: "CarlErpProperties",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPropertyValues_PropertyId",
                table: "CarlErpPropertyValues",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPropertyValues_TenantId_CreatorUserId",
                table: "CarlErpPropertyValues",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchaseOrderItems_ItemId",
                table: "CarlErpPurchaseOrderItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchaseOrderItems_PurchaseOrderId",
                table: "CarlErpPurchaseOrderItems",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchaseOrderItems_TaxId",
                table: "CarlErpPurchaseOrderItems",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchaseOrderItems_TenantId_CreatorUserId",
                table: "CarlErpPurchaseOrderItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchaseOrders_CreatorUserId",
                table: "CarlErpPurchaseOrders",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchaseOrders_CurrencyId",
                table: "CarlErpPurchaseOrders",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchaseOrders_LastModifierUserId",
                table: "CarlErpPurchaseOrders",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchaseOrders_LocationId",
                table: "CarlErpPurchaseOrders",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchaseOrders_MultiCurrencyId",
                table: "CarlErpPurchaseOrders",
                column: "MultiCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchaseOrders_VendorId",
                table: "CarlErpPurchaseOrders",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchaseOrders_TenantId_OrderNumber",
                table: "CarlErpPurchaseOrders",
                columns: new[] { "TenantId", "OrderNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchaseOrders_TenantId_Reference",
                table: "CarlErpPurchaseOrders",
                columns: new[] { "TenantId", "Reference" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchaseOrders_TenantId_CreatorUserId_OrderNumber",
                table: "CarlErpPurchaseOrders",
                columns: new[] { "TenantId", "CreatorUserId", "OrderNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpRawMaterialItems_FromLotId",
                table: "CarlErpRawMaterialItems",
                column: "FromLotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpRawMaterialItems_ItemId",
                table: "CarlErpRawMaterialItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpRawMaterialItems_ProductionId",
                table: "CarlErpRawMaterialItems",
                column: "ProductionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpRawMaterialItems_TenantId_CreatorUserId",
                table: "CarlErpRawMaterialItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReceivePaymentDeails_CustomerCreditId",
                table: "CarlErpReceivePaymentDeails",
                column: "CustomerCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReceivePaymentDeails_CustomerId",
                table: "CarlErpReceivePaymentDeails",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReceivePaymentDeails_InvoiceId",
                table: "CarlErpReceivePaymentDeails",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReceivePaymentDeails_ReceivePaymentId",
                table: "CarlErpReceivePaymentDeails",
                column: "ReceivePaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReceivePaymentDeails_TenantId_CreatorUserId",
                table: "CarlErpReceivePaymentDeails",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReceivePaymentExpense_AccountId",
                table: "CarlErpReceivePaymentExpense",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReceivePaymentExpense_ReceivePaymentId",
                table: "CarlErpReceivePaymentExpense",
                column: "ReceivePaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReceivePaymentExpense_TenantId_CreatorUserId",
                table: "CarlErpReceivePaymentExpense",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReceivePayments_PaymentMethodId",
                table: "CarlErpReceivePayments",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReceivePayments_TenantId_CreatorUserId",
                table: "CarlErpReceivePayments",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReportColumnTemplate_ReportTemplateId",
                table: "CarlErpReportColumnTemplate",
                column: "ReportTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReportFilterTemplate_ReportTemplateId",
                table: "CarlErpReportFilterTemplate",
                column: "ReportTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReportTemplate_CreatorUserId_TenantId",
                table: "CarlErpReportTemplate",
                columns: new[] { "CreatorUserId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrderItems_ItemId",
                table: "CarlErpSaleOrderItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrderItems_SaleOrderId",
                table: "CarlErpSaleOrderItems",
                column: "SaleOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrderItems_TaxId",
                table: "CarlErpSaleOrderItems",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrderItems_TenantId_CreatorUserId",
                table: "CarlErpSaleOrderItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrders_CreatorUserId",
                table: "CarlErpSaleOrders",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrders_CurrencyId",
                table: "CarlErpSaleOrders",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrders_CustomerId",
                table: "CarlErpSaleOrders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrders_LastModifierUserId",
                table: "CarlErpSaleOrders",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrders_LocationId",
                table: "CarlErpSaleOrders",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrders_MultiCurrencyId",
                table: "CarlErpSaleOrders",
                column: "MultiCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrders_SaleTransactionTypeId",
                table: "CarlErpSaleOrders",
                column: "SaleTransactionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrders_TenantId_OrderNumber",
                table: "CarlErpSaleOrders",
                columns: new[] { "TenantId", "OrderNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrders_TenantId_Reference",
                table: "CarlErpSaleOrders",
                columns: new[] { "TenantId", "Reference" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrders_TenantId_CreatorUserId_OrderNumber",
                table: "CarlErpSaleOrders",
                columns: new[] { "TenantId", "CreatorUserId", "OrderNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSubItems_ItemId",
                table: "CarlErpSubItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSubItems_ParentSubItemId",
                table: "CarlErpSubItems",
                column: "ParentSubItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSubscriptionPayments_EditionId",
                table: "CarlErpSubscriptionPayments",
                column: "EditionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSubscriptionPayments_SubscriptionType",
                table: "CarlErpSubscriptionPayments",
                column: "SubscriptionType");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSubscriptionPayments_TenantId",
                table: "CarlErpSubscriptionPayments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSubscriptions_CreatorUserId",
                table: "CarlErpSubscriptions",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSubscriptions_EditionId",
                table: "CarlErpSubscriptions",
                column: "EditionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSubscriptions_TenantId",
                table: "CarlErpSubscriptions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTaxes_TenantId_CreatorUserId",
                table: "CarlErpTaxes",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTaxes_TenantId_TaxName",
                table: "CarlErpTaxes",
                columns: new[] { "TenantId", "TaxName" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransactionTypes_TenantId_TransactionTypeName",
                table: "CarlErpTransactionTypes",
                columns: new[] { "TenantId", "TransactionTypeName" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransferOrderItems_FromLotId",
                table: "CarlErpTransferOrderItems",
                column: "FromLotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransferOrderItems_ItemId",
                table: "CarlErpTransferOrderItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransferOrderItems_ToLotId",
                table: "CarlErpTransferOrderItems",
                column: "ToLotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransferOrderItems_TransferOrderId",
                table: "CarlErpTransferOrderItems",
                column: "TransferOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransferOrderItems_TenantId_CreatorUserId",
                table: "CarlErpTransferOrderItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransferOrders_CreatorUserId",
                table: "CarlErpTransferOrders",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransferOrders_LastModifierUserId",
                table: "CarlErpTransferOrders",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransferOrders_TransferFromClassId",
                table: "CarlErpTransferOrders",
                column: "TransferFromClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransferOrders_TransferFromLocationId",
                table: "CarlErpTransferOrders",
                column: "TransferFromLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransferOrders_TransferToClassId",
                table: "CarlErpTransferOrders",
                column: "TransferToClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransferOrders_TransferToLocationId",
                table: "CarlErpTransferOrders",
                column: "TransferToLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransferOrders_TenantId_TransferNo",
                table: "CarlErpTransferOrders",
                columns: new[] { "TenantId", "TransferNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransProductions_CreatorUserId",
                table: "CarlErpTransProductions",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransProductions_FromClassId",
                table: "CarlErpTransProductions",
                column: "FromClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransProductions_FromLocationId",
                table: "CarlErpTransProductions",
                column: "FromLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransProductions_LastModifierUserId",
                table: "CarlErpTransProductions",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransProductions_ProductionAccountId",
                table: "CarlErpTransProductions",
                column: "ProductionAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransProductions_ProductionPlanId",
                table: "CarlErpTransProductions",
                column: "ProductionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransProductions_ProductionProcessId",
                table: "CarlErpTransProductions",
                column: "ProductionProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransProductions_ToClassId",
                table: "CarlErpTransProductions",
                column: "ToClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransProductions_ToLocationId",
                table: "CarlErpTransProductions",
                column: "ToLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransProductions_TenantId_ProductionNo",
                table: "CarlErpTransProductions",
                columns: new[] { "TenantId", "ProductionNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpUserGroupMembers_MemberId",
                table: "CarlErpUserGroupMembers",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpUserGroupMembers_UserGroupId",
                table: "CarlErpUserGroupMembers",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpUserGroupMembers_TenantId_CreatorUserId",
                table: "CarlErpUserGroupMembers",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpUserGroups_LocationId",
                table: "CarlErpUserGroups",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpUserGroups_TenantId_CreatorUserId_Name",
                table: "CarlErpUserGroups",
                columns: new[] { "TenantId", "CreatorUserId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCredit_ItemReceiptId",
                table: "CarlErpVendorCredit",
                column: "ItemReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCredit_VendorId",
                table: "CarlErpVendorCredit",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCredit_TenantId_CreatorUserId_VendorId",
                table: "CarlErpVendorCredit",
                columns: new[] { "TenantId", "CreatorUserId", "VendorId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCreditDetails_ItemId",
                table: "CarlErpVendorCreditDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCreditDetails_ItemReceiptItemId",
                table: "CarlErpVendorCreditDetails",
                column: "ItemReceiptItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCreditDetails_LotId",
                table: "CarlErpVendorCreditDetails",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCreditDetails_TaxId",
                table: "CarlErpVendorCreditDetails",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCreditDetails_VendorCreditId",
                table: "CarlErpVendorCreditDetails",
                column: "VendorCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCreditDetails_TenantId_CreatorUserId",
                table: "CarlErpVendorCreditDetails",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCreditItemBatchNos_BatchNoId",
                table: "CarlErpVendorCreditItemBatchNos",
                column: "BatchNoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCreditItemBatchNos_VendorCreditItemId",
                table: "CarlErpVendorCreditItemBatchNos",
                column: "VendorCreditItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCustomerOpenBalaces_AccountCycleId",
                table: "CarlErpVendorCustomerOpenBalaces",
                column: "AccountCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCustomerOpenBalaces_Key",
                table: "CarlErpVendorCustomerOpenBalaces",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCustomerOpenBalaces_TenantId_CreatorUserId",
                table: "CarlErpVendorCustomerOpenBalaces",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorGroups_UserGroupId",
                table: "CarlErpVendorGroups",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorGroups_VendorId",
                table: "CarlErpVendorGroups",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorGroups_TenantId_CreatorUserId_VendorId",
                table: "CarlErpVendorGroups",
                columns: new[] { "TenantId", "CreatorUserId", "VendorId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendors_AccountId",
                table: "CarlErpVendors",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendors_VendorTypeId",
                table: "CarlErpVendors",
                column: "VendorTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendors_TenantId_VendorCode",
                table: "CarlErpVendors",
                columns: new[] { "TenantId", "VendorCode" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendors_TenantId_VendorName",
                table: "CarlErpVendors",
                columns: new[] { "TenantId", "VendorName" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorTypeMembers_MemberId",
                table: "CarlErpVendorTypeMembers",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorTypeMembers_VendorTypeId",
                table: "CarlErpVendorTypeMembers",
                column: "VendorTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorTypes_TenantId_CreatorUserId_VendorTypeName",
                table: "CarlErpVendorTypes",
                columns: new[] { "TenantId", "CreatorUserId", "VendorTypeName" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpWithdrawItems_WithdrawId",
                table: "CarlErpWithdrawItems",
                column: "WithdrawId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpWithdrawItems_TenantId_CreatorUserId",
                table: "CarlErpWithdrawItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpWithdraws_CustomerId",
                table: "CarlErpWithdraws",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpWithdraws_VendorId",
                table: "CarlErpWithdraws",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpWithdraws_TenantId_CreatorUserId",
                table: "CarlErpWithdraws",
                columns: new[] { "TenantId", "CreatorUserId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbpAuditLogs");

            migrationBuilder.DropTable(
                name: "AbpBackgroundJobs");

            migrationBuilder.DropTable(
                name: "AbpEntityPropertyChanges");

            migrationBuilder.DropTable(
                name: "AbpFeatures");

            migrationBuilder.DropTable(
                name: "AbpLanguages");

            migrationBuilder.DropTable(
                name: "AbpLanguageTexts");

            migrationBuilder.DropTable(
                name: "AbpNotifications");

            migrationBuilder.DropTable(
                name: "AbpNotificationSubscriptions");

            migrationBuilder.DropTable(
                name: "AbpOrganizationUnits");

            migrationBuilder.DropTable(
                name: "AbpPermissions");

            migrationBuilder.DropTable(
                name: "AbpPersistedGrants");

            migrationBuilder.DropTable(
                name: "AbpRoleClaims");

            migrationBuilder.DropTable(
                name: "AbpSettings");

            migrationBuilder.DropTable(
                name: "AbpTenantNotifications");

            migrationBuilder.DropTable(
                name: "AbpUserAccounts");

            migrationBuilder.DropTable(
                name: "AbpUserClaims");

            migrationBuilder.DropTable(
                name: "AbpUserLoginAttempts");

            migrationBuilder.DropTable(
                name: "AbpUserLogins");

            migrationBuilder.DropTable(
                name: "AbpUserNotifications");

            migrationBuilder.DropTable(
                name: "AbpUserOrganizationUnits");

            migrationBuilder.DropTable(
                name: "AbpUserRoles");

            migrationBuilder.DropTable(
                name: "AbpUserTokens");

            migrationBuilder.DropTable(
                name: "AppBinaryObjects");

            migrationBuilder.DropTable(
                name: "AppChatMessages");

            migrationBuilder.DropTable(
                name: "AppFriendships");

            migrationBuilder.DropTable(
                name: "AppInvoices");

            migrationBuilder.DropTable(
                name: "AppSubscriptionPayments");

            migrationBuilder.DropTable(
                name: "CarlErpAccountTransactionCloses");

            migrationBuilder.DropTable(
                name: "CarlErpAutoSequences");

            migrationBuilder.DropTable(
                name: "CarlErpBankTransfers");

            migrationBuilder.DropTable(
                name: "CarlErpBillInvoiceSettings");

            migrationBuilder.DropTable(
                name: "CarlErpBillItems");

            migrationBuilder.DropTable(
                name: "CarlErpCache");

            migrationBuilder.DropTable(
                name: "CarlErpContactPersons");

            migrationBuilder.DropTable(
                name: "CarlErpCustomerContactPersons");

            migrationBuilder.DropTable(
                name: "CarlErpCustomerCreditItemBatchNos");

            migrationBuilder.DropTable(
                name: "CarlErpCustomerGroups");

            migrationBuilder.DropTable(
                name: "CarlErpCustomerTypeMembers");

            migrationBuilder.DropTable(
                name: "CarlErpDepositItems");

            migrationBuilder.DropTable(
                name: "CarlErpExchangeItems");

            migrationBuilder.DropTable(
                name: "CarlErpGroupMemberItemTamplates");

            migrationBuilder.DropTable(
                name: "CarlErpInventoryCalculationItems");

            migrationBuilder.DropTable(
                name: "CarlErpInventoryCalculationSchedules");

            migrationBuilder.DropTable(
                name: "CarlErpInventoryCostCloseItemBatchNos");

            migrationBuilder.DropTable(
                name: "CarlErpInventoryCostCloseItems");

            migrationBuilder.DropTable(
                name: "CarlErpInventoryTransactionItems");

            migrationBuilder.DropTable(
                name: "CarlErpInvoiceItems");

            migrationBuilder.DropTable(
                name: "CarlErpInvoiceTemplateMaps");

            migrationBuilder.DropTable(
                name: "CarlErpItemIssueItemBatchNos");

            migrationBuilder.DropTable(
                name: "CarlErpItemIssueVendorCreditItemBatchNos");

            migrationBuilder.DropTable(
                name: "CarlErpItemLots");

            migrationBuilder.DropTable(
                name: "CarlErpItemPriceItems");

            migrationBuilder.DropTable(
                name: "CarlErpItemProperties");

            migrationBuilder.DropTable(
                name: "CarlErpItemReceiptCustomerCreditItemBatchNos");

            migrationBuilder.DropTable(
                name: "CarlErpItemReceiptItemBatchNos");

            migrationBuilder.DropTable(
                name: "CarlErpItemUserGroups");

            migrationBuilder.DropTable(
                name: "CarlErpJournalItems");

            migrationBuilder.DropTable(
                name: "CarlErpLocks");

            migrationBuilder.DropTable(
                name: "CarlErpMultiCurrencies");

            migrationBuilder.DropTable(
                name: "CarlErpPayBillDeail");

            migrationBuilder.DropTable(
                name: "CarlErpPayBillExpense");

            migrationBuilder.DropTable(
                name: "CarlErpPaymentMethodUserGroups");

            migrationBuilder.DropTable(
                name: "CarlErpPermissionLocks");

            migrationBuilder.DropTable(
                name: "CarlErpPermissionLockScheduleItems");

            migrationBuilder.DropTable(
                name: "CarlErpPhysicalCountItems");

            migrationBuilder.DropTable(
                name: "CarlErpProductionStandardCostGroups");

            migrationBuilder.DropTable(
                name: "CarlErpProductionStandardCosts");

            migrationBuilder.DropTable(
                name: "CarlErpReceivePaymentDeails");

            migrationBuilder.DropTable(
                name: "CarlErpReceivePaymentExpense");

            migrationBuilder.DropTable(
                name: "CarlErpReportColumnTemplate");

            migrationBuilder.DropTable(
                name: "CarlErpReportFilterTemplate");

            migrationBuilder.DropTable(
                name: "CarlErpSubItems");

            migrationBuilder.DropTable(
                name: "CarlErpSubscriptionPayments");

            migrationBuilder.DropTable(
                name: "CarlErpSubscriptions");

            migrationBuilder.DropTable(
                name: "CarlErpUserGroupMembers");

            migrationBuilder.DropTable(
                name: "CarlErpVendorCreditItemBatchNos");

            migrationBuilder.DropTable(
                name: "CarlErpVendorCustomerOpenBalaces");

            migrationBuilder.DropTable(
                name: "CarlErpVendorGroups");

            migrationBuilder.DropTable(
                name: "CarlErpVendorTypeMembers");

            migrationBuilder.DropTable(
                name: "CarlErpWithdrawItems");

            migrationBuilder.DropTable(
                name: "AbpEntityChanges");

            migrationBuilder.DropTable(
                name: "AbpRoles");

            migrationBuilder.DropTable(
                name: "CarlErpExchanges");

            migrationBuilder.DropTable(
                name: "CarlErpInventoryCostCloses");

            migrationBuilder.DropTable(
                name: "CarlErpInoviceTemplates");

            migrationBuilder.DropTable(
                name: "CarlErpItemIssueVendorCreditItem");

            migrationBuilder.DropTable(
                name: "CarlErpItemPrices");

            migrationBuilder.DropTable(
                name: "CarlErpItemReceiptCustomerCreditItem");

            migrationBuilder.DropTable(
                name: "CarlErpJournals");

            migrationBuilder.DropTable(
                name: "CarlErpPermissionLockSchedules");

            migrationBuilder.DropTable(
                name: "CarlErpPropertyValues");

            migrationBuilder.DropTable(
                name: "CarlErpReportTemplate");

            migrationBuilder.DropTable(
                name: "AbpTenants");

            migrationBuilder.DropTable(
                name: "CarlErpBatchNos");

            migrationBuilder.DropTable(
                name: "CarlErpUserGroups");

            migrationBuilder.DropTable(
                name: "AbpEntityChangeSets");

            migrationBuilder.DropTable(
                name: "CarlErpVendorCreditDetails");

            migrationBuilder.DropTable(
                name: "CarlErpCustomerCreditDetails");

            migrationBuilder.DropTable(
                name: "CarlErpBills");

            migrationBuilder.DropTable(
                name: "CarlErpDeposits");

            migrationBuilder.DropTable(
                name: "CarlErpInvoices");

            migrationBuilder.DropTable(
                name: "CarlErpItemIssueVendorCredit");

            migrationBuilder.DropTable(
                name: "CarlErpItemReceiptCustomerCredit");

            migrationBuilder.DropTable(
                name: "CarlErpPayBills");

            migrationBuilder.DropTable(
                name: "CarlErpReceivePayments");

            migrationBuilder.DropTable(
                name: "CarlErpWithdraws");

            migrationBuilder.DropTable(
                name: "CarlErpAccountCycles");

            migrationBuilder.DropTable(
                name: "AbpEditions");

            migrationBuilder.DropTable(
                name: "CarlErpFormats");

            migrationBuilder.DropTable(
                name: "CarlErpProperties");

            migrationBuilder.DropTable(
                name: "CarlErpItemReceiptItems");

            migrationBuilder.DropTable(
                name: "CarlErpItemIssueItems");

            migrationBuilder.DropTable(
                name: "CarlErpVendorCredit");

            migrationBuilder.DropTable(
                name: "CarlErpCustomerCredits");

            migrationBuilder.DropTable(
                name: "CarlErpPaymentMethods");

            migrationBuilder.DropTable(
                name: "CarlErpFinishItems");

            migrationBuilder.DropTable(
                name: "CarlErpPurchaseOrderItems");

            migrationBuilder.DropTable(
                name: "CarlErpRawMaterialItems");

            migrationBuilder.DropTable(
                name: "CarlErpSaleOrderItems");

            migrationBuilder.DropTable(
                name: "CarlErpTransferOrderItems");

            migrationBuilder.DropTable(
                name: "CarlErpItemReceipts");

            migrationBuilder.DropTable(
                name: "CarlErpItemIssues");

            migrationBuilder.DropTable(
                name: "CarlErpPaymentMethodBases");

            migrationBuilder.DropTable(
                name: "CarlErpPurchaseOrders");

            migrationBuilder.DropTable(
                name: "CarlErpSaleOrders");

            migrationBuilder.DropTable(
                name: "CarlErpLots");

            migrationBuilder.DropTable(
                name: "CarlErpItems");

            migrationBuilder.DropTable(
                name: "CarlErpPhysicalCounts");

            migrationBuilder.DropTable(
                name: "CarlErpTransProductions");

            migrationBuilder.DropTable(
                name: "CarlErpTransferOrders");

            migrationBuilder.DropTable(
                name: "CarlErpVendors");

            migrationBuilder.DropTable(
                name: "CarlErpCustomers");

            migrationBuilder.DropTable(
                name: "CarlErpTransactionTypes");

            migrationBuilder.DropTable(
                name: "CarlErpBatchNoFormulas");

            migrationBuilder.DropTable(
                name: "CarlErpGalleries");

            migrationBuilder.DropTable(
                name: "CarlErpItemTypes");

            migrationBuilder.DropTable(
                name: "CarlErpCurrencies");

            migrationBuilder.DropTable(
                name: "CarlErpProductionPlans");

            migrationBuilder.DropTable(
                name: "CarlErpProductionProcess");

            migrationBuilder.DropTable(
                name: "CarlErpClasses");

            migrationBuilder.DropTable(
                name: "CarlErpVendorTypes");

            migrationBuilder.DropTable(
                name: "CarlErpCustomerTypes");

            migrationBuilder.DropTable(
                name: "AbpUsers");

            migrationBuilder.DropTable(
                name: "CarlErpLocations");

            migrationBuilder.DropTable(
                name: "CarlErpProductionLines");

            migrationBuilder.DropTable(
                name: "CarlErpChartOfAccounts");

            migrationBuilder.DropTable(
                name: "CarlErpAccountTypes");

            migrationBuilder.DropTable(
                name: "CarlErpTaxes");
        }
    }
}
