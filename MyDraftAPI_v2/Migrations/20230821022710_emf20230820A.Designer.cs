﻿// <auto-generated />
using System;
using DbData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MyDraftAPI_v2.Migrations
{
    [DbContext(typeof(AppDataContext))]
    [Migration("20230821022710_emf20230820A")]
    partial class emf20230820A
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Database.Model.AAV", b =>
                {
                    b.Property<decimal?>("PPRValue")
                        .HasColumnType("decimal(5,2)");

                    b.Property<int?>("PlayerID")
                        .HasColumnType("int");

                    b.Property<decimal?>("StandardValue")
                        .HasColumnType("decimal(5,2)");

                    b.HasIndex("PlayerID");

                    b.ToTable("AAV", (string)null);
                });

            modelBuilder.Entity("Database.Model.ADP", b =>
                {
                    b.Property<decimal?>("PPRValue")
                        .HasColumnType("decimal(5,2)");

                    b.Property<int?>("PlayerID")
                        .HasColumnType("int");

                    b.Property<decimal?>("StandardValue")
                        .HasColumnType("decimal(5,2)");

                    b.HasIndex("PlayerID");

                    b.ToTable("ADP", (string)null);
                });

            modelBuilder.Entity("Database.Model.DVDB", b =>
                {
                    b.Property<int?>("LeagueID")
                        .HasColumnType("int");

                    b.Property<int?>("PlayerID")
                        .HasColumnType("int");

                    b.Property<int?>("Segment")
                        .HasColumnType("int");

                    b.Property<decimal?>("Value")
                        .HasColumnType("decimal(5,2)");

                    b.Property<int?>("Year")
                        .HasColumnType("int");

                    b.HasIndex("PlayerID");

                    b.ToTable("DVDB", (string)null);
                });

            modelBuilder.Entity("Database.Model.DepthChart", b =>
                {
                    b.Property<int>("PlayerID")
                        .HasColumnType("int");

                    b.Property<int>("PositionID")
                        .HasColumnType("int");

                    b.Property<int>("TeamID")
                        .HasColumnType("int");

                    b.Property<int>("Rank")
                        .HasColumnType("int");

                    b.Property<string>("TeamAbbr")
                        .HasMaxLength(3)
                        .HasColumnType("nvarchar(3)");

                    b.HasKey("PlayerID", "PositionID", "TeamID");

                    b.HasIndex("PositionID");

                    b.HasIndex("TeamID");

                    b.ToTable("DepthChart", (string)null);
                });

            modelBuilder.Entity("Database.Model.Injury", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("BodyPart")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Details")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("PlayerId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.ToTable("Injuries", (string)null);
                });

            modelBuilder.Entity("Database.Model.Player", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("College")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("Experience")
                        .HasColumnType("int");

                    b.Property<string>("FirstName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Height")
                        .HasMaxLength(6)
                        .HasColumnType("nvarchar(6)");

                    b.Property<bool?>("IsRookie")
                        .HasColumnType("bit");

                    b.Property<string>("LastName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PhotoUrl")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Position")
                        .HasMaxLength(3)
                        .HasColumnType("nvarchar(3)");

                    b.Property<string>("PositionGroup")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("ProTeamID")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("TeamAbbr")
                        .HasMaxLength(5)
                        .HasColumnType("nvarchar(5)");

                    b.Property<int?>("Weight")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("ProTeamID");

                    b.ToTable("Players", (string)null);
                });

            modelBuilder.Entity("Database.Model.PlayerNews", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Analysis")
                        .HasMaxLength(350)
                        .HasColumnType("nvarchar(350)");

                    b.Property<string>("ImageURL")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("InjuryType")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("NewsDescription")
                        .HasMaxLength(550)
                        .HasColumnType("nvarchar(550)");

                    b.Property<int>("PlayerID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("PubDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Reccomendation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("ID");

                    b.HasIndex("PlayerID");

                    b.ToTable("PlayerNews", (string)null);
                });

            modelBuilder.Entity("Database.Model.PlayerPosition", b =>
                {
                    b.Property<int?>("PlayerID")
                        .HasColumnType("int");

                    b.Property<int?>("PositionID")
                        .HasColumnType("int");

                    b.HasKey("PlayerID", "PositionID");

                    b.ToTable("PlayerPosition", (string)null);
                });

            modelBuilder.Entity("Database.Model.Points", b =>
                {
                    b.Property<string>("GroupAbbr")
                        .HasMaxLength(5)
                        .HasColumnType("nvarchar(5)");

                    b.Property<int>("LeagueID")
                        .HasColumnType("int");

                    b.Property<int>("PlayerID")
                        .HasColumnType("int");

                    b.Property<string>("Tag")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<DateTime?>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.Property<decimal?>("Value")
                        .HasColumnType("decimal(5,2)");

                    b.Property<int?>("Week")
                        .HasColumnType("int");

                    b.Property<int?>("Year")
                        .HasColumnType("int");

                    b.HasIndex("LeagueID");

                    b.HasIndex("PlayerID");

                    b.ToTable("Points", (string)null);
                });

            modelBuilder.Entity("Database.Model.Position", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Abbr")
                        .HasMaxLength(3)
                        .HasColumnType("nvarchar(3)");

                    b.Property<string>("FullName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ID");

                    b.ToTable("Positions", (string)null);
                });

            modelBuilder.Entity("Database.Model.ProTeam", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Abbr")
                        .HasMaxLength(3)
                        .HasColumnType("nvarchar(3)");

                    b.Property<int?>("ByeWeek")
                        .HasColumnType("int");

                    b.Property<string>("City")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Conference")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Division")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("HeadCoach")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("NickName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ID");

                    b.ToTable("ProTeams", (string)null);
                });

            modelBuilder.Entity("Database.Model.Schedule", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int?>("AwayTeamID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("GameDate")
                        .HasMaxLength(50)
                        .HasColumnType("datetime2");

                    b.Property<int?>("HomeTeamID")
                        .HasColumnType("int");

                    b.Property<int?>("Season")
                        .HasColumnType("int");

                    b.Property<int?>("Week")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("AwayTeamID");

                    b.HasIndex("HomeTeamID");

                    b.ToTable("Schedule", (string)null);
                });

            modelBuilder.Entity("Database.Model.UserDraftSelections", b =>
                {
                    b.Property<int>("LeagueID")
                        .HasColumnType("int");

                    b.Property<int>("PlayerID")
                        .HasColumnType("int");

                    b.Property<int>("TeamID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("IsKeeper")
                        .HasColumnType("bit");

                    b.Property<int?>("OverallPick")
                        .HasColumnType("int");

                    b.Property<int?>("Pick")
                        .HasColumnType("int");

                    b.Property<int?>("PositionPick")
                        .HasColumnType("int");

                    b.Property<int?>("PositionRound")
                        .HasColumnType("int");

                    b.Property<int?>("Round")
                        .HasColumnType("int");

                    b.HasKey("LeagueID", "PlayerID", "TeamID");

                    b.ToTable("UserDraftSelections", (string)null);
                });

            modelBuilder.Entity("Database.Model.UserLeague", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Abbr")
                        .HasMaxLength(5)
                        .HasColumnType("nvarchar(5)");

                    b.Property<string>("DraftOrder")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<int>("DraftType")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastActiveDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Mode")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("NumberOfRounds")
                        .HasColumnType("int");

                    b.Property<int>("NumberOfTeams")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.ToTable("UserLeague", (string)null);
                });

            modelBuilder.Entity("Database.Model.UserLeagueTeams", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Abbreviation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DraftPosition")
                        .HasColumnType("int");

                    b.Property<int>("LeagueID")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Owner")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("LeagueID");

                    b.ToTable("UserLeagueTeams", (string)null);
                });

            modelBuilder.Entity("Database.Model.AAV", b =>
                {
                    b.HasOne("Database.Model.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerID");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("Database.Model.ADP", b =>
                {
                    b.HasOne("Database.Model.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerID");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("Database.Model.DVDB", b =>
                {
                    b.HasOne("Database.Model.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerID");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("Database.Model.DepthChart", b =>
                {
                    b.HasOne("Database.Model.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Database.Model.Position", "Position")
                        .WithMany()
                        .HasForeignKey("PositionID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Database.Model.ProTeam", "ProTeam")
                        .WithMany()
                        .HasForeignKey("TeamID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");

                    b.Navigation("Position");

                    b.Navigation("ProTeam");
                });

            modelBuilder.Entity("Database.Model.Injury", b =>
                {
                    b.HasOne("Database.Model.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("Database.Model.Player", b =>
                {
                    b.HasOne("Database.Model.ProTeam", "ProTeam")
                        .WithMany()
                        .HasForeignKey("ProTeamID");

                    b.Navigation("ProTeam");
                });

            modelBuilder.Entity("Database.Model.PlayerNews", b =>
                {
                    b.HasOne("Database.Model.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("Database.Model.Points", b =>
                {
                    b.HasOne("Database.Model.UserLeague", "League")
                        .WithMany()
                        .HasForeignKey("LeagueID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Database.Model.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("League");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("Database.Model.Schedule", b =>
                {
                    b.HasOne("Database.Model.ProTeam", "AwayTeam")
                        .WithMany()
                        .HasForeignKey("AwayTeamID");

                    b.HasOne("Database.Model.ProTeam", "HomeTeam")
                        .WithMany()
                        .HasForeignKey("HomeTeamID");

                    b.Navigation("AwayTeam");

                    b.Navigation("HomeTeam");
                });

            modelBuilder.Entity("Database.Model.UserLeagueTeams", b =>
                {
                    b.HasOne("Database.Model.UserLeague", "League")
                        .WithMany()
                        .HasForeignKey("LeagueID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("League");
                });
#pragma warning restore 612, 618
        }
    }
}