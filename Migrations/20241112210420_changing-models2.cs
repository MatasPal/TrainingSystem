using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingSystem.Migrations
{
    /// <inheritdoc />
    public partial class changingmodels2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Trainers_TrProgramId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Trainers_AspNetUsers_UserId",
                table: "Trainers");

            migrationBuilder.DropForeignKey(
                name: "FK_Workouts_Trainers_TrProgramId",
                table: "Workouts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Trainers",
                table: "Trainers");

            migrationBuilder.RenameTable(
                name: "Trainers",
                newName: "TrPrograms");

            migrationBuilder.RenameIndex(
                name: "IX_Trainers_UserId",
                table: "TrPrograms",
                newName: "IX_TrPrograms_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrPrograms",
                table: "TrPrograms",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_TrPrograms_TrProgramId",
                table: "Comments",
                column: "TrProgramId",
                principalTable: "TrPrograms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrPrograms_AspNetUsers_UserId",
                table: "TrPrograms",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Workouts_TrPrograms_TrProgramId",
                table: "Workouts",
                column: "TrProgramId",
                principalTable: "TrPrograms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_TrPrograms_TrProgramId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_TrPrograms_AspNetUsers_UserId",
                table: "TrPrograms");

            migrationBuilder.DropForeignKey(
                name: "FK_Workouts_TrPrograms_TrProgramId",
                table: "Workouts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TrPrograms",
                table: "TrPrograms");

            migrationBuilder.RenameTable(
                name: "TrPrograms",
                newName: "Trainers");

            migrationBuilder.RenameIndex(
                name: "IX_TrPrograms_UserId",
                table: "Trainers",
                newName: "IX_Trainers_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Trainers",
                table: "Trainers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Trainers_TrProgramId",
                table: "Comments",
                column: "TrProgramId",
                principalTable: "Trainers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trainers_AspNetUsers_UserId",
                table: "Trainers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Workouts_Trainers_TrProgramId",
                table: "Workouts",
                column: "TrProgramId",
                principalTable: "Trainers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
