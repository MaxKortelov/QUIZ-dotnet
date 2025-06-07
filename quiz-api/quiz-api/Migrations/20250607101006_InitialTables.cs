using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quiz_api.Migrations
{
    /// <inheritdoc />
    public partial class InitialTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "question_type",
                columns: table => new
                {
                    uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_question_type", x => x.uuid);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<byte[]>(type: "bytea", nullable: false),
                    password_salt = table.Column<string>(type: "text", nullable: false),
                    username = table.Column<string>(type: "text", nullable: false),
                    deleted = table.Column<bool>(type: "boolean", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    reset_password_token = table.Column<string>(type: "text", nullable: true),
                    verify_email_token = table.Column<string>(type: "text", nullable: true),
                    user_confirmed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.uuid);
                });

            migrationBuilder.CreateTable(
                name: "question",
                columns: table => new
                {
                    uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    question_text = table.Column<string>(type: "text", nullable: false),
                    correct_answers = table.Column<List<string>>(type: "text[]", nullable: false),
                    question_type_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_question", x => x.uuid);
                    table.ForeignKey(
                        name: "FK_question_question_type_question_type_id",
                        column: x => x.question_type_id,
                        principalTable: "question_type",
                        principalColumn: "uuid");
                });

            migrationBuilder.CreateTable(
                name: "quiz_session",
                columns: table => new
                {
                    uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    question_sequence = table.Column<List<Guid>>(type: "uuid[]", nullable: false),
                    question_answer = table.Column<string>(type: "jsonb", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_started = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_ended = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    duration = table.Column<int>(type: "integer", nullable: false),
                    attempts = table.Column<int>(type: "integer", nullable: false),
                    attempts_used = table.Column<int>(type: "integer", nullable: false),
                    result = table.Column<int>(type: "integer", nullable: true),
                    question_type_id = table.Column<Guid>(type: "uuid", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quiz_session", x => x.uuid);
                    table.ForeignKey(
                        name: "FK_quiz_session_question_type_question_type_id",
                        column: x => x.question_type_id,
                        principalTable: "question_type",
                        principalColumn: "uuid");
                    table.ForeignKey(
                        name: "FK_quiz_session_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "uuid");
                });

            migrationBuilder.CreateTable(
                name: "answer",
                columns: table => new
                {
                    uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    answer_text = table.Column<string>(type: "text", nullable: false),
                    question_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_answer", x => x.uuid);
                    table.ForeignKey(
                        name: "FK_answer_question_question_id",
                        column: x => x.question_id,
                        principalTable: "question",
                        principalColumn: "uuid");
                });

            migrationBuilder.CreateTable(
                name: "quiz_table_results",
                columns: table => new
                {
                    uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    quiz_amount_taken = table.Column<int>(type: "integer", nullable: false),
                    correct_answers = table.Column<int>(type: "integer", nullable: false),
                    best_quiz_session_id = table.Column<Guid>(type: "uuid", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quiz_table_results", x => x.uuid);
                    table.ForeignKey(
                        name: "FK_quiz_table_results_quiz_session_best_quiz_session_id",
                        column: x => x.best_quiz_session_id,
                        principalTable: "quiz_session",
                        principalColumn: "uuid");
                    table.ForeignKey(
                        name: "FK_quiz_table_results_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "uuid");
                });

            migrationBuilder.CreateIndex(
                name: "IX_answer_question_id",
                table: "answer",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_question_question_type_id",
                table: "question",
                column: "question_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_quiz_session_question_type_id",
                table: "quiz_session",
                column: "question_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_quiz_session_user_id",
                table: "quiz_session",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_quiz_table_results_best_quiz_session_id",
                table: "quiz_table_results",
                column: "best_quiz_session_id");

            migrationBuilder.CreateIndex(
                name: "IX_quiz_table_results_user_id",
                table: "quiz_table_results",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "answer");

            migrationBuilder.DropTable(
                name: "quiz_table_results");

            migrationBuilder.DropTable(
                name: "question");

            migrationBuilder.DropTable(
                name: "quiz_session");

            migrationBuilder.DropTable(
                name: "question_type");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
