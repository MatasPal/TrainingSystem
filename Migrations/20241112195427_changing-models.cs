using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingSystem.Migrations
{
    /// <inheritdoc />
    public partial class changingmodels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Trainers_TrainerId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Workouts_Trainers_TrainerId",
                table: "Workouts");

            migrationBuilder.RenameColumn(
                name: "TrainerId",
                table: "Workouts",
                newName: "TrProgramId");

            migrationBuilder.RenameIndex(
                name: "IX_Workouts_TrainerId",
                table: "Workouts",
                newName: "IX_Workouts_TrProgramId");

            migrationBuilder.RenameColumn(
                name: "TypeTr",
                table: "Trainers",
                newName: "Trainer");

            migrationBuilder.RenameColumn(
                name: "Experience",
                table: "Trainers",
                newName: "Difficulty");

            migrationBuilder.RenameColumn(
                name: "TrainerId",
                table: "Comments",
                newName: "TrProgramId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_TrainerId",
                table: "Comments",
                newName: "IX_Comments_TrProgramId");

            migrationBuilder.AddColumn<string>(
                name: "Descr",
                table: "Trainers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "Trainers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Trainers_TrProgramId",
                table: "Comments",
                column: "TrProgramId",
                principalTable: "Trainers",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Trainers_TrProgramId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Workouts_Trainers_TrProgramId",
                table: "Workouts");

            migrationBuilder.DropColumn(
                name: "Descr",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Trainers");

            migrationBuilder.RenameColumn(
                name: "TrProgramId",
                table: "Workouts",
                newName: "TrainerId");

            migrationBuilder.RenameIndex(
                name: "IX_Workouts_TrProgramId",
                table: "Workouts",
                newName: "IX_Workouts_TrainerId");

            migrationBuilder.RenameColumn(
                name: "Trainer",
                table: "Trainers",
                newName: "TypeTr");

            migrationBuilder.RenameColumn(
                name: "Difficulty",
                table: "Trainers",
                newName: "Experience");

            migrationBuilder.RenameColumn(
                name: "TrProgramId",
                table: "Comments",
                newName: "TrainerId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_TrProgramId",
                table: "Comments",
                newName: "IX_Comments_TrainerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Trainers_TrainerId",
                table: "Comments",
                column: "TrainerId",
                principalTable: "Trainers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Workouts_Trainers_TrainerId",
                table: "Workouts",
                column: "TrainerId",
                principalTable: "Trainers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
