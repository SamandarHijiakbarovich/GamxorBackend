using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GamxorOila.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "devices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceKey = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsRegistered = table.Column<bool>(type: "boolean", nullable: false),
                    IsLoggedIn = table.Column<bool>(type: "boolean", nullable: false),
                    IsPhoneVerified = table.Column<bool>(type: "boolean", nullable: false),
                    LastSyncAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SelectedMemberId = table.Column<int>(type: "integer", nullable: true),
                    TrustedPlacesCount = table.Column<int>(type: "integer", nullable: false),
                    NextCheckInLabel = table.Column<string>(type: "text", nullable: false),
                    SosIsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SosIsSending = table.Column<bool>(type: "boolean", nullable: false),
                    SosSummary = table.Column<string>(type: "text", nullable: false),
                    SosSentAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Profile_FullName = table.Column<string>(type: "text", nullable: false),
                    Profile_Phone = table.Column<string>(type: "text", nullable: false),
                    Profile_Email = table.Column<string>(type: "text", nullable: false),
                    Profile_FamilyLabel = table.Column<string>(type: "text", nullable: false),
                    Profile_Address = table.Column<string>(type: "text", nullable: false),
                    Profile_EmergencyContact = table.Column<string>(type: "text", nullable: false),
                    Profile_AvatarSeed = table.Column<int>(type: "integer", nullable: false),
                    Profile_AvatarUri = table.Column<string>(type: "text", nullable: false),
                    Profile_Bio = table.Column<string>(type: "text", nullable: false),
                    Profile_MemberCode = table.Column<string>(type: "text", nullable: false),
                    Profile_Permissions_LocationEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    Profile_Permissions_MicrophoneEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    Profile_Permissions_NotificationsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    Profile_Permissions_PreciseLocationEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    Profile_Permissions_BackgroundRefreshEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_devices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "activity_feed",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    AppId = table.Column<int>(type: "integer", nullable: false),
                    MemberAppId = table.Column<int>(type: "integer", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Subtitle = table.Column<string>(type: "text", nullable: false),
                    OccurredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Severity = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activity_feed", x => x.Id);
                    table.ForeignKey(
                        name: "FK_activity_feed_devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "family_members",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    AppId = table.Column<int>(type: "integer", nullable: false),
                    IsSelf = table.Column<bool>(type: "boolean", nullable: false),
                    MemberCode = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Relation = table.Column<string>(type: "text", nullable: false),
                    Age = table.Column<int>(type: "integer", nullable: false),
                    Lat = table.Column<double>(type: "double precision", nullable: false),
                    Lon = table.Column<double>(type: "double precision", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    PlaceLabel = table.Column<string>(type: "text", nullable: false),
                    Battery = table.Column<int>(type: "integer", nullable: false),
                    Steps = table.Column<int>(type: "integer", nullable: false),
                    HeartRate = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Note = table.Column<string>(type: "text", nullable: false),
                    SafeZone = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Schedule = table.Column<string>(type: "text", nullable: false),
                    AvatarSeed = table.Column<int>(type: "integer", nullable: false),
                    AvatarUri = table.Column<string>(type: "text", nullable: false),
                    DistanceKm = table.Column<double>(type: "double precision", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_family_members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_family_members_devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "invitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    AppId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Relation = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    SentAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    IsPlatformUser = table.Column<bool>(type: "boolean", nullable: false),
                    CanAccept = table.Column<bool>(type: "boolean", nullable: false),
                    FamilyName = table.Column<string>(type: "text", nullable: false),
                    InvitedByName = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_invitations_devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    AppId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    OccurredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Category = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    IsDismissed = table.Column<bool>(type: "boolean", nullable: false),
                    InviteAppId = table.Column<int>(type: "integer", nullable: true),
                    ActionLabel = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_notifications_devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "otp_requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Consumed = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_otp_requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_otp_requests_devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sos_alerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberAppId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Relation = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    AvatarUri = table.Column<string>(type: "text", nullable: false),
                    RaisedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sos_alerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sos_alerts_devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sos_contacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberAppId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sos_contacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sos_contacts_devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_activity_feed_DeviceId_AppId",
                table: "activity_feed",
                columns: new[] { "DeviceId", "AppId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_devices_DeviceKey",
                table: "devices",
                column: "DeviceKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_family_members_DeviceId_AppId",
                table: "family_members",
                columns: new[] { "DeviceId", "AppId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_invitations_DeviceId_AppId",
                table: "invitations",
                columns: new[] { "DeviceId", "AppId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_notifications_DeviceId_AppId",
                table: "notifications",
                columns: new[] { "DeviceId", "AppId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_otp_requests_DeviceId",
                table: "otp_requests",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_sos_alerts_DeviceId",
                table: "sos_alerts",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_sos_contacts_DeviceId",
                table: "sos_contacts",
                column: "DeviceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "activity_feed");

            migrationBuilder.DropTable(
                name: "family_members");

            migrationBuilder.DropTable(
                name: "invitations");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "otp_requests");

            migrationBuilder.DropTable(
                name: "sos_alerts");

            migrationBuilder.DropTable(
                name: "sos_contacts");

            migrationBuilder.DropTable(
                name: "devices");
        }
    }
}
