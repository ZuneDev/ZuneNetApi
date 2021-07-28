using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Zune.DB.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AvailableBadges",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    TypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Image = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    MediaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MediaType = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvailableBadges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MemberBadge",
                columns: table => new
                {
                    MemberId = table.Column<string>(type: "TEXT", nullable: false),
                    BadgeId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberBadge", x => new { x.MemberId, x.BadgeId });
                    table.ForeignKey(
                        name: "FK_MemberBadge_AvailableBadges_MemberId",
                        column: x => x.MemberId,
                        principalTable: "AvailableBadges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    SenderId = table.Column<string>(type: "TEXT", nullable: true),
                    TextContent = table.Column<string>(type: "TEXT", nullable: true),
                    ReplyLinkId = table.Column<string>(type: "TEXT", nullable: true),
                    AltLinkId = table.Column<string>(type: "TEXT", nullable: true),
                    AlbumTitle = table.Column<string>(type: "TEXT", nullable: true),
                    ArtistName = table.Column<string>(type: "TEXT", nullable: true),
                    SongTitle = table.Column<string>(type: "TEXT", nullable: true),
                    TrackNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    PlaylistName = table.Column<string>(type: "TEXT", nullable: true),
                    PodcastName = table.Column<string>(type: "TEXT", nullable: true),
                    PodcastUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    Subject = table.Column<string>(type: "TEXT", nullable: true),
                    Received = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DetailsLink = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    Wishlist = table.Column<bool>(type: "INTEGER", nullable: false),
                    MediaId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    AuthorId = table.Column<string>(type: "TEXT", nullable: true),
                    Content = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Link",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Relation = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    Href = table.Column<string>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Updated = table.Column<string>(type: "TEXT", nullable: true),
                    MemberId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Link", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MemberMember",
                columns: table => new
                {
                    MemberAId = table.Column<string>(type: "TEXT", nullable: false),
                    MemberBId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberMember", x => new { x.MemberAId, x.MemberBId });
                });

            migrationBuilder.CreateTable(
                name: "Tuners",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tuners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ZuneTag = table.Column<string>(type: "TEXT", nullable: true),
                    PlayCount = table.Column<int>(type: "INTEGER", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    Bio = table.Column<string>(type: "TEXT", nullable: true),
                    Location = table.Column<string>(type: "TEXT", nullable: true),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Xuid = table.Column<string>(type: "TEXT", nullable: true),
                    Locale = table.Column<string>(type: "TEXT", nullable: true),
                    ParentallyControlled = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExplicitPrivilege = table.Column<bool>(type: "INTEGER", nullable: false),
                    Lightweight = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserReadID = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserWriteID = table.Column<Guid>(type: "TEXT", nullable: false),
                    UsageCollectionAllowed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TagChangeRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    AcceptedTermsOfService = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccountSuspended = table.Column<bool>(type: "INTEGER", nullable: false),
                    SubscriptionLapsed = table.Column<bool>(type: "INTEGER", nullable: false),
                    BillingUnavailable = table.Column<bool>(type: "INTEGER", nullable: false),
                    PointsBalance = table.Column<double>(type: "REAL", nullable: false),
                    SongCreditBalance = table.Column<double>(type: "REAL", nullable: false),
                    SongCreditRenewalDate = table.Column<string>(type: "TEXT", nullable: true),
                    SubscriptionOfferId = table.Column<string>(type: "TEXT", nullable: true),
                    SubscriptionRenewalOfferId = table.Column<string>(type: "TEXT", nullable: true),
                    BillingInstanceId = table.Column<string>(type: "TEXT", nullable: true),
                    SubscriptionEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    SubscriptionBillingViolation = table.Column<bool>(type: "INTEGER", nullable: false),
                    SubscriptionPendingCancel = table.Column<bool>(type: "INTEGER", nullable: false),
                    SubscriptionStartDate = table.Column<string>(type: "TEXT", nullable: true),
                    SubscriptionEndDate = table.Column<string>(type: "TEXT", nullable: true),
                    SubscriptionMeteringCertificate = table.Column<string>(type: "TEXT", nullable: true),
                    LastLabelTakedownDate = table.Column<string>(type: "TEXT", nullable: true),
                    MediaTypeTunerRegisterInfoId = table.Column<string>(type: "TEXT", nullable: true),
                    UserTile = table.Column<string>(type: "TEXT", nullable: true),
                    Background = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Members_Tuners_MediaTypeTunerRegisterInfoId",
                        column: x => x.MediaTypeTunerRegisterInfoId,
                        principalTable: "Tuners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AuthorId",
                table: "Comments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Link_MemberId",
                table: "Link",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberMember_MemberBId",
                table: "MemberMember",
                column: "MemberBId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_MediaTypeTunerRegisterInfoId",
                table: "Members",
                column: "MediaTypeTunerRegisterInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_AltLinkId",
                table: "Messages",
                column: "AltLinkId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReplyLinkId",
                table: "Messages",
                column: "ReplyLinkId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberBadge_Members_MemberId",
                table: "MemberBadge",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Link_AltLinkId",
                table: "Messages",
                column: "AltLinkId",
                principalTable: "Link",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Link_ReplyLinkId",
                table: "Messages",
                column: "ReplyLinkId",
                principalTable: "Link",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Members_Id",
                table: "Messages",
                column: "Id",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Members_SenderId",
                table: "Messages",
                column: "SenderId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Members_AuthorId",
                table: "Comments",
                column: "AuthorId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Members_Id",
                table: "Comments",
                column: "Id",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Link_Members_MemberId",
                table: "Link",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberMember_Members_MemberAId",
                table: "MemberMember",
                column: "MemberAId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberMember_Members_MemberBId",
                table: "MemberMember",
                column: "MemberBId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tuners_Members_Id",
                table: "Tuners",
                column: "Id",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tuners_Members_Id",
                table: "Tuners");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "MemberBadge");

            migrationBuilder.DropTable(
                name: "MemberMember");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "AvailableBadges");

            migrationBuilder.DropTable(
                name: "Link");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "Tuners");
        }
    }
}
