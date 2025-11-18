using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CorarlERP.Migrations
{
    public partial class QCTest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarlErpQCSamples",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    SampleId = table.Column<string>(nullable: false),
                    SourceDoc = table.Column<string>(nullable: true),
                    SampleDate = table.Column<DateTime>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false),
                    LocationId = table.Column<long>(nullable: false),
                    Remark = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpQCSamples", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpQCSamples_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpQCSamples_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpQCTestTemplates",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 512, nullable: false),
                    TestSource = table.Column<int>(nullable: false),
                    DetailEntryRequired = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpQCTestTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpTestParameters",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 512, nullable: false),
                    TestSource = table.Column<int>(nullable: false),
                    LimitReferenceNote = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpTestParameters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpLabTestRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    QCTestTemplateId = table.Column<long>(nullable: false),
                    QCSampleId = table.Column<Guid>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    LabId = table.Column<Guid>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Remark = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpLabTestRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpLabTestRequests_CarlErpVendors_LabId",
                        column: x => x.LabId,
                        principalTable: "CarlErpVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpLabTestRequests_CarlErpQCSamples_QCSampleId",
                        column: x => x.QCSampleId,
                        principalTable: "CarlErpQCSamples",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpLabTestRequests_CarlErpQCTestTemplates_QCTestTemplat~",
                        column: x => x.QCTestTemplateId,
                        principalTable: "CarlErpQCTestTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpQCTestTemplateParameters",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    QCTestTemplateId = table.Column<long>(nullable: false),
                    LimitReferenceNoteOverride = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpQCTestTemplateParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpQCTestTemplateParameters_CarlErpQCTestTemplates_QCTe~",
                        column: x => x.QCTestTemplateId,
                        principalTable: "CarlErpQCTestTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpLabTestResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    LabTestRequestId = table.Column<Guid>(nullable: false),
                    ReferenceNo = table.Column<string>(nullable: true),
                    LabId = table.Column<Guid>(nullable: true),
                    DetailEntry = table.Column<bool>(nullable: false),
                    FinalPassFail = table.Column<bool>(nullable: false),
                    ResultDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpLabTestResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpLabTestResults_CarlErpVendors_LabId",
                        column: x => x.LabId,
                        principalTable: "CarlErpVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpLabTestResults_CarlErpLabTestRequests_LabTestRequest~",
                        column: x => x.LabTestRequestId,
                        principalTable: "CarlErpLabTestRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpLabTestResultDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    LabTestResultId = table.Column<Guid>(nullable: false),
                    TestParameterId = table.Column<long>(nullable: false),
                    LimitReferenceNote = table.Column<string>(nullable: true),
                    ActualValueNote = table.Column<string>(nullable: true),
                    PassFail = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpLabTestResultDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpLabTestResultDetails_CarlErpLabTestResults_LabTestRe~",
                        column: x => x.LabTestResultId,
                        principalTable: "CarlErpLabTestResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpLabTestResultDetails_CarlErpTestParameters_TestParam~",
                        column: x => x.TestParameterId,
                        principalTable: "CarlErpTestParameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpLabTestRequests_LabId",
                table: "CarlErpLabTestRequests",
                column: "LabId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpLabTestRequests_QCSampleId",
                table: "CarlErpLabTestRequests",
                column: "QCSampleId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpLabTestRequests_QCTestTemplateId",
                table: "CarlErpLabTestRequests",
                column: "QCTestTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpLabTestResultDetails_LabTestResultId",
                table: "CarlErpLabTestResultDetails",
                column: "LabTestResultId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpLabTestResultDetails_TestParameterId",
                table: "CarlErpLabTestResultDetails",
                column: "TestParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpLabTestResults_LabId",
                table: "CarlErpLabTestResults",
                column: "LabId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpLabTestResults_LabTestRequestId",
                table: "CarlErpLabTestResults",
                column: "LabTestRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpLabTestResults_ReferenceNo_TenantId_LabId",
                table: "CarlErpLabTestResults",
                columns: new[] { "ReferenceNo", "TenantId", "LabId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpQCSamples_ItemId",
                table: "CarlErpQCSamples",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpQCSamples_LocationId",
                table: "CarlErpQCSamples",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpQCSamples_SampleId",
                table: "CarlErpQCSamples",
                column: "SampleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpQCTestTemplateParameters_QCTestTemplateId",
                table: "CarlErpQCTestTemplateParameters",
                column: "QCTestTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpQCTestTemplates_Name",
                table: "CarlErpQCTestTemplates",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTestParameters_Name",
                table: "CarlErpTestParameters",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpLabTestResultDetails");

            migrationBuilder.DropTable(
                name: "CarlErpQCTestTemplateParameters");

            migrationBuilder.DropTable(
                name: "CarlErpLabTestResults");

            migrationBuilder.DropTable(
                name: "CarlErpTestParameters");

            migrationBuilder.DropTable(
                name: "CarlErpLabTestRequests");

            migrationBuilder.DropTable(
                name: "CarlErpQCSamples");

            migrationBuilder.DropTable(
                name: "CarlErpQCTestTemplates");
        }
    }
}
