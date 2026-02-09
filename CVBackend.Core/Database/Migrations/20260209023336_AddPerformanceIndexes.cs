using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVBackend.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "idx_skills_name",
                table: "skills",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "idx_skills_proficiency_level",
                table: "skills",
                column: "proficiency_level");

            migrationBuilder.CreateIndex(
                name: "idx_projects_name",
                table: "projects",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "idx_projects_start_date",
                table: "projects",
                column: "start_date");

            migrationBuilder.CreateIndex(
                name: "idx_education_degree",
                table: "education",
                column: "degree");

            migrationBuilder.CreateIndex(
                name: "idx_education_institution",
                table: "education",
                column: "institution");

            migrationBuilder.CreateIndex(
                name: "idx_companies_name",
                table: "companies",
                column: "name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_skills_name",
                table: "skills");

            migrationBuilder.DropIndex(
                name: "idx_skills_proficiency_level",
                table: "skills");

            migrationBuilder.DropIndex(
                name: "idx_projects_name",
                table: "projects");

            migrationBuilder.DropIndex(
                name: "idx_projects_start_date",
                table: "projects");

            migrationBuilder.DropIndex(
                name: "idx_education_degree",
                table: "education");

            migrationBuilder.DropIndex(
                name: "idx_education_institution",
                table: "education");

            migrationBuilder.DropIndex(
                name: "idx_companies_name",
                table: "companies");
        }
    }
}
