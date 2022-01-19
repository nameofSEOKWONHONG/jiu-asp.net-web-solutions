using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    public partial class _20220118_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_MIGRAION",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MIGRATION_YN = table.Column<bool>(type: "bit", nullable: false),
                    COMPLETE_YN = table.Column<bool>(type: "bit", nullable: false),
                    WRITE_ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WRITE_DT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATE_ID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UPDATE_DT = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_MIGRAION", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TB_ROLE_PERMISSION",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ROLE_PERMISSION_TYPES = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WRITE_ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WRITE_DT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATE_ID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UPDATE_DT = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_ROLE_PERMISSION", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TB_TODO",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CONTENTS = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NOTIFY_DT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WRITE_ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WRITE_DT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATE_ID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UPDATE_DT = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_TODO", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TB_ROLE",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ROLE_TYPE = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ROLE_PERMISSIONID = table.Column<int>(type: "int", nullable: true),
                    WRITE_ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WRITE_DT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATE_ID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UPDATE_DT = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_ROLE", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TB_ROLE_TB_ROLE_PERMISSION_ROLE_PERMISSIONID",
                        column: x => x.ROLE_PERMISSIONID,
                        principalTable: "TB_ROLE_PERMISSION",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "TB_USER",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EMAIL = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PASSWORD = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PHONE_NUM = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ACTIVE_USER_YN = table.Column<bool>(type: "bit", nullable: false),
                    AUTO_CONFIRM_EMAIL_YN = table.Column<bool>(type: "bit", nullable: false),
                    ROLEID = table.Column<int>(type: "int", nullable: true),
                    WRITE_ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WRITE_DT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPDATE_ID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UPDATE_DT = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_USER", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TB_USER_TB_ROLE_ROLEID",
                        column: x => x.ROLEID,
                        principalTable: "TB_ROLE",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_MIGRAION_WRITE_ID_WRITE_DT_UPDATE_ID_UPDATE_DT",
                table: "TB_MIGRAION",
                columns: new[] { "WRITE_ID", "WRITE_DT", "UPDATE_ID", "UPDATE_DT" });

            migrationBuilder.CreateIndex(
                name: "IX_TB_ROLE_ROLE_PERMISSIONID",
                table: "TB_ROLE",
                column: "ROLE_PERMISSIONID");

            migrationBuilder.CreateIndex(
                name: "IX_TB_ROLE_WRITE_ID_WRITE_DT_UPDATE_ID_UPDATE_DT",
                table: "TB_ROLE",
                columns: new[] { "WRITE_ID", "WRITE_DT", "UPDATE_ID", "UPDATE_DT" });

            migrationBuilder.CreateIndex(
                name: "IX_TB_ROLE_PERMISSION_WRITE_ID_WRITE_DT_UPDATE_ID_UPDATE_DT",
                table: "TB_ROLE_PERMISSION",
                columns: new[] { "WRITE_ID", "WRITE_DT", "UPDATE_ID", "UPDATE_DT" });

            migrationBuilder.CreateIndex(
                name: "IX_TB_TODO_CONTENTS",
                table: "TB_TODO",
                column: "CONTENTS");

            migrationBuilder.CreateIndex(
                name: "IX_TB_TODO_WRITE_ID_WRITE_DT_UPDATE_ID_UPDATE_DT",
                table: "TB_TODO",
                columns: new[] { "WRITE_ID", "WRITE_DT", "UPDATE_ID", "UPDATE_DT" });

            migrationBuilder.CreateIndex(
                name: "IX_TB_USER_EMAIL_ACTIVE_USER_YN_AUTO_CONFIRM_EMAIL_YN",
                table: "TB_USER",
                columns: new[] { "EMAIL", "ACTIVE_USER_YN", "AUTO_CONFIRM_EMAIL_YN" });

            migrationBuilder.CreateIndex(
                name: "IX_TB_USER_ROLEID",
                table: "TB_USER",
                column: "ROLEID");

            migrationBuilder.CreateIndex(
                name: "IX_TB_USER_WRITE_ID_WRITE_DT_UPDATE_ID_UPDATE_DT",
                table: "TB_USER",
                columns: new[] { "WRITE_ID", "WRITE_DT", "UPDATE_ID", "UPDATE_DT" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_MIGRAION");

            migrationBuilder.DropTable(
                name: "TB_TODO");

            migrationBuilder.DropTable(
                name: "TB_USER");

            migrationBuilder.DropTable(
                name: "TB_ROLE");

            migrationBuilder.DropTable(
                name: "TB_ROLE_PERMISSION");
        }
    }
}
