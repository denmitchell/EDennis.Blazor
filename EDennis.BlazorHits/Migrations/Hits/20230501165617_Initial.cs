using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EDennis.BlazorHits.Migrations.Hits
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "seqArtist");

            migrationBuilder.CreateSequence<int>(
                name: "seqSong");

            migrationBuilder.CreateTable(
                name: "Artist",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, defaultValueSql: "(NEXT VALUE FOR [seqArtist])")
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "Artist")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "Artist")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart"),
                    IsSolo = table.Column<bool>(type: "bit", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "Artist")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart"),
                    SysUser = table.Column<string>(type: "nvarchar(max)", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "Artist")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart"),
                    SysGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "Artist")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart"),
                    SysEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "Artist")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart"),
                    SysStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "Artist")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artist", x => x.Id);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "Artist")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart");

            migrationBuilder.CreateTable(
                name: "Song",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, defaultValueSql: "(NEXT VALUE FOR [seqSong])")
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "Song")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "Song")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart"),
                    ArtistId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "Song")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart"),
                    ReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "Song")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart"),
                    SysUser = table.Column<string>(type: "nvarchar(max)", nullable: true)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "Song")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart"),
                    SysGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "Song")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart"),
                    SysEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "Song")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart"),
                    SysStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:IsTemporal", true)
                        .Annotation("SqlServer:TemporalHistoryTableName", "Song")
                        .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                        .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                        .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Song", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Song_Artist_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artist",
                        principalColumn: "Id");
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "Song")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart");

            migrationBuilder.InsertData(
                table: "Artist",
                columns: new[] { "Id", "IsSolo", "Name", "SysGuid", "SysUser" },
                values: new object[,]
                {
                    { -8, false, "Aerosmith", new Guid("00000008-0000-0000-0000-000000000008"), "SYSTEM" },
                    { -7, true, "Aretha Franklin", new Guid("00000007-0000-0000-0000-000000000007"), "SYSTEM" },
                    { -6, false, "Queen", new Guid("00000006-0000-0000-0000-000000000006"), "SYSTEM" },
                    { -5, true, "Marvin Gaye", new Guid("00000005-0000-0000-0000-000000000005"), "SYSTEM" },
                    { -4, false, "Eagles", new Guid("00000004-0000-0000-0000-000000000004"), "SYSTEM" },
                    { -3, false, "Led Zeppelin", new Guid("00000003-0000-0000-0000-000000000003"), "SYSTEM" },
                    { -2, true, "Billy Joel", new Guid("00000002-0000-0000-0000-000000000002"), "SYSTEM" },
                    { -1, false, "Beatles", new Guid("00000001-0000-0000-0000-000000000001"), "SYSTEM" }
                });

            migrationBuilder.InsertData(
                table: "Song",
                columns: new[] { "Id", "ArtistId", "ReleaseDate", "SysGuid", "SysUser", "Title" },
                values: new object[,]
                {
                    { -16, -8, new DateTime(1975, 5, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000016-0000-0000-0000-000000000016"), "SYSTEM", "Sweet Emotion" },
                    { -15, -8, new DateTime(1973, 6, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000015-0000-0000-0000-000000000015"), "SYSTEM", "Dream On" },
                    { -14, -7, new DateTime(1968, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000014-0000-0000-0000-000000000014"), "SYSTEM", "Think" },
                    { -13, -7, new DateTime(1967, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000013-0000-0000-0000-000000000013"), "SYSTEM", "Respect" },
                    { -12, -6, new DateTime(1977, 10, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000012-0000-0000-0000-000000000012"), "SYSTEM", "We Will Rock You" },
                    { -11, -6, new DateTime(1975, 10, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000011-0000-0000-0000-000000000011"), "SYSTEM", "Bohemian Rhapsody" },
                    { -10, -5, new DateTime(1971, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000010-0000-0000-0000-000000000010"), "SYSTEM", "What's Going On" },
                    { -9, -5, new DateTime(1967, 9, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000009-0000-0000-0000-000000000009"), "SYSTEM", "I Heard It Through the Grapevine" },
                    { -8, -4, new DateTime(1972, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000008-0000-0000-0000-000000000008"), "SYSTEM", "Take It Easy" },
                    { -7, -4, new DateTime(1976, 12, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000007-0000-0000-0000-000000000007"), "SYSTEM", "Hotel California" },
                    { -6, -3, new DateTime(1979, 12, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000006-0000-0000-0000-000000000006"), "SYSTEM", "Fool in the Rain" },
                    { -5, -3, new DateTime(1975, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000005-0000-0000-0000-000000000005"), "SYSTEM", "Kashmir" },
                    { -4, -2, new DateTime(1978, 10, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000004-0000-0000-0000-000000000004"), "SYSTEM", "My Life" },
                    { -3, -2, new DateTime(1973, 11, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000003-0000-0000-0000-000000000003"), "SYSTEM", "Piano Man" },
                    { -2, -1, new DateTime(1968, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000002-0000-0000-0000-000000000002"), "SYSTEM", "Hey Jude" },
                    { -1, -1, new DateTime(1965, 9, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("00000001-0000-0000-0000-000000000001"), "SYSTEM", "Yesterday" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Song_ArtistId",
                table: "Song",
                column: "ArtistId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Song")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "Song")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart");

            migrationBuilder.DropTable(
                name: "Artist")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "Artist")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "dbo_history")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "SysEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "SysStart");

            migrationBuilder.DropSequence(
                name: "seqArtist");

            migrationBuilder.DropSequence(
                name: "seqSong");
        }
    }
}
