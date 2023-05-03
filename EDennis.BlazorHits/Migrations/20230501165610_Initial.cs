using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EDennis.BlazorHits.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "seqAppRole");

            migrationBuilder.CreateSequence<int>(
                name: "seqAppUser");

            migrationBuilder.CreateTable(
                name: "AppRole",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, defaultValueSql: "(NEXT VALUE FOR [seqAppRole])")
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "AppRole")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "AppRole")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart"),
                    SysUser = table.Column<string>(type: "nvarchar(max)", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "AppRole")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart"),
                    SysGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "AppRole")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart"),
                    SysEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "AppRole")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart"),
                    SysStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "AppRole")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppRole", x => x.Id);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "AppRole")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart");

            migrationBuilder.CreateTable(
                name: "AppUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, defaultValueSql: "(NEXT VALUE FOR [seqAppUser])")
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "AppUser")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "AppUser")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart"),
                    RoleId = table.Column<int>(type: "int", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "AppUser")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart"),
                    SysUser = table.Column<string>(type: "nvarchar(max)", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "AppUser")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart"),
                    SysGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "AppUser")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart"),
                    SysEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "AppUser")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart"),
                    SysStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "AppUser")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppUser_AppRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AppRole",
                        principalColumn: "Id");
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "AppUser")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart");

            migrationBuilder.InsertData(
                table: "AppRole",
                columns: new[] { "Id", "RoleName", "SysGuid", "SysUser" },
                values: new object[,]
                {
                    { -5, "disabled", new Guid("00000005-0000-0000-0000-000000000005"), "SYSTEM" },
                    { -4, "readonly", new Guid("00000004-0000-0000-0000-000000000004"), "SYSTEM" },
                    { -3, "user", new Guid("00000003-0000-0000-0000-000000000003"), "SYSTEM" },
                    { -2, "admin", new Guid("00000002-0000-0000-0000-000000000002"), "SYSTEM" },
                    { -1, "IT", new Guid("00000001-0000-0000-0000-000000000001"), "SYSTEM" }
                });

            migrationBuilder.InsertData(
                table: "AppUser",
                columns: new[] { "Id", "RoleId", "SysGuid", "SysUser", "UserName" },
                values: new object[,]
                {
                    { -5, -5, new Guid("00000005-0000-0000-0000-000000000005"), "SYSTEM", "Jack" },
                    { -4, -4, new Guid("00000004-0000-0000-0000-000000000004"), "SYSTEM", "Huan" },
                    { -3, -3, new Guid("00000003-0000-0000-0000-000000000003"), "SYSTEM", "Darius" },
                    { -2, -2, new Guid("00000002-0000-0000-0000-000000000002"), "SYSTEM", "Maria" },
                    { -1, -1, new Guid("00000001-0000-0000-0000-000000000001"), "SYSTEM", "Starbuck" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUser_RoleId",
                table: "AppUser",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUser")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "AppUser")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart");

            migrationBuilder.DropTable(
                name: "AppRole")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "AppRole")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart");

            migrationBuilder.DropSequence(
                name: "seqAppRole");

            migrationBuilder.DropSequence(
                name: "seqAppUser");
        }
    }
}
