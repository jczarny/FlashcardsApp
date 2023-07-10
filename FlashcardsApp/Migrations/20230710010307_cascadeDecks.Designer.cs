﻿// <auto-generated />
using System;
using FlashcardsApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FlashcardsApp.Migrations
{
    [DbContext(typeof(FlashcardsContext))]
    [Migration("20230710010307_cascadeDecks")]
    partial class cascadeDecks
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("FlashcardsApp.Entities.Card", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DeckId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Front")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Reverse")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DeckId");

                    b.ToTable("Cards");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DeckId = 1,
                            Description = "Meat is neat",
                            Front = "Kiełbasa",
                            Reverse = "Sausage"
                        },
                        new
                        {
                            Id = 2,
                            DeckId = 1,
                            Description = "Sky is the limit",
                            Front = "Niebo",
                            Reverse = "Sky"
                        },
                        new
                        {
                            Id = 3,
                            DeckId = 2,
                            Description = "Mięso jest spoko",
                            Front = "Sausage",
                            Reverse = "Kiełbasa"
                        },
                        new
                        {
                            Id = 4,
                            DeckId = 2,
                            Description = "Limitem jest niebo",
                            Front = "Sky",
                            Reverse = "Niebo"
                        },
                        new
                        {
                            Id = 5,
                            DeckId = 3,
                            Description = "Ja voll!",
                            Front = "Ja",
                            Reverse = "Yes"
                        });
                });

            modelBuilder.Entity("FlashcardsApp.Entities.Deck", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CreatorId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<bool>("IsPrivate")
                        .HasColumnType("bit");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.ToTable("Decks");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreatorId = 3,
                            Description = "Test123",
                            IsPrivate = false,
                            Title = "Polish to English vocab"
                        },
                        new
                        {
                            Id = 2,
                            CreatorId = 3,
                            Description = "Test123",
                            IsPrivate = false,
                            Title = "English to Polish vocab"
                        },
                        new
                        {
                            Id = 3,
                            CreatorId = 3,
                            Description = "Test123",
                            IsPrivate = false,
                            Title = "German to English vocab"
                        });
                });

            modelBuilder.Entity("FlashcardsApp.Entities.RevisionLog", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("DeckId")
                        .HasColumnType("int");

                    b.Property<int>("CardId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int>("Ease")
                        .HasColumnType("int");

                    b.Property<int>("Stage")
                        .HasColumnType("int");

                    b.HasKey("UserId", "DeckId", "CardId");

                    b.HasIndex("CardId");

                    b.HasIndex("DeckId");

                    b.ToTable("RevisionLog");

                    b.HasData(
                        new
                        {
                            UserId = 1,
                            DeckId = 1,
                            CardId = 1,
                            Date = new DateTime(2008, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Ease = 2,
                            Stage = 1
                        },
                        new
                        {
                            UserId = 2,
                            DeckId = 1,
                            CardId = 1,
                            Date = new DateTime(2008, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Ease = 2,
                            Stage = 1
                        },
                        new
                        {
                            UserId = 3,
                            DeckId = 1,
                            CardId = 2,
                            Date = new DateTime(2008, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Ease = 2,
                            Stage = 1
                        });
                });

            modelBuilder.Entity("FlashcardsApp.Entities.Statistic", b =>
                {
                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("CorrectGuesses")
                        .HasColumnType("int");

                    b.Property<int>("TotalGuesses")
                        .HasColumnType("int");

                    b.Property<int>("WrongGuesses")
                        .HasColumnType("int");

                    b.HasKey("Date", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("Statistics", t =>
                        {
                            t.HasCheckConstraint("CK_TotalGuesses", "[TotalGuesses] = [CorrectGuesses] + [WrongGuesses]");
                        });

                    b.HasData(
                        new
                        {
                            Date = new DateTime(2008, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            UserId = 1,
                            CorrectGuesses = 0,
                            TotalGuesses = 0,
                            WrongGuesses = 0
                        },
                        new
                        {
                            Date = new DateTime(2008, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            UserId = 1,
                            CorrectGuesses = 1,
                            TotalGuesses = 1,
                            WrongGuesses = 0
                        },
                        new
                        {
                            Date = new DateTime(2008, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            UserId = 2,
                            CorrectGuesses = 0,
                            TotalGuesses = 1,
                            WrongGuesses = 1
                        },
                        new
                        {
                            Date = new DateTime(2008, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            UserId = 2,
                            CorrectGuesses = 0,
                            TotalGuesses = 0,
                            WrongGuesses = 0
                        },
                        new
                        {
                            Date = new DateTime(2008, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            UserId = 3,
                            CorrectGuesses = 0,
                            TotalGuesses = 0,
                            WrongGuesses = 0
                        },
                        new
                        {
                            Date = new DateTime(2008, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            UserId = 3,
                            CorrectGuesses = 0,
                            TotalGuesses = 1,
                            WrongGuesses = 1
                        });
                });

            modelBuilder.Entity("FlashcardsApp.Entities.UserDeck", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("DeckId")
                        .HasColumnType("int");

                    b.HasKey("UserId", "DeckId");

                    b.HasIndex("DeckId");

                    b.ToTable("UserDecks");

                    b.HasData(
                        new
                        {
                            UserId = 1,
                            DeckId = 1
                        },
                        new
                        {
                            UserId = 2,
                            DeckId = 1
                        },
                        new
                        {
                            UserId = 3,
                            DeckId = 1
                        },
                        new
                        {
                            UserId = 3,
                            DeckId = 2
                        });
                });

            modelBuilder.Entity("FlashcardsApp.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("TokenCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("TokenExpires")
                        .HasColumnType("datetime2");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.HasKey("Id");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            PasswordHash = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                            PasswordSalt = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                            Username = "Michal15"
                        },
                        new
                        {
                            Id = 2,
                            PasswordHash = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                            PasswordSalt = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                            Username = "Krzychu7"
                        },
                        new
                        {
                            Id = 3,
                            PasswordHash = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                            PasswordSalt = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                            Username = "admin"
                        });
                });

            modelBuilder.Entity("FlashcardsApp.Entities.Card", b =>
                {
                    b.HasOne("FlashcardsApp.Entities.Deck", "Deck")
                        .WithMany("Cards")
                        .HasForeignKey("DeckId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Deck");
                });

            modelBuilder.Entity("FlashcardsApp.Entities.Deck", b =>
                {
                    b.HasOne("FlashcardsApp.Models.User", "Creator")
                        .WithMany("OwnerDecks")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("FlashcardsApp.Entities.RevisionLog", b =>
                {
                    b.HasOne("FlashcardsApp.Entities.Card", "Card")
                        .WithMany("RevisionLogs")
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FlashcardsApp.Entities.Deck", "Deck")
                        .WithMany("RevisionLogs")
                        .HasForeignKey("DeckId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FlashcardsApp.Models.User", "User")
                        .WithMany("RevisionLogs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Card");

                    b.Navigation("Deck");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FlashcardsApp.Entities.Statistic", b =>
                {
                    b.HasOne("FlashcardsApp.Models.User", "User")
                        .WithMany("Statistics")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("FlashcardsApp.Entities.UserDeck", b =>
                {
                    b.HasOne("FlashcardsApp.Entities.Deck", "Deck")
                        .WithMany("UserDecks")
                        .HasForeignKey("DeckId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FlashcardsApp.Models.User", "User")
                        .WithMany("UserDecks")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Deck");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FlashcardsApp.Entities.Card", b =>
                {
                    b.Navigation("RevisionLogs");
                });

            modelBuilder.Entity("FlashcardsApp.Entities.Deck", b =>
                {
                    b.Navigation("Cards");

                    b.Navigation("RevisionLogs");

                    b.Navigation("UserDecks");
                });

            modelBuilder.Entity("FlashcardsApp.Models.User", b =>
                {
                    b.Navigation("OwnerDecks");

                    b.Navigation("RevisionLogs");

                    b.Navigation("Statistics");

                    b.Navigation("UserDecks");
                });
#pragma warning restore 612, 618
        }
    }
}
