﻿// <auto-generated />
using System;
using CC98.Achievement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CC98.Achievement.Migrations
{
    [DbContext(typeof(AchievementDbContext))]
    [Migration("20230609084050_ChangeKeyToCodeName")]
    partial class ChangeKeyToCodeName
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CC98.Achievement.Data.AchievementCategory", b =>
                {
                    b.Property<string>("CodeName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("AppId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserCount")
                        .HasColumnType("int");

                    b.HasKey("CodeName");

                    b.HasIndex("AppId")
                        .IsUnique()
                        .HasFilter("[AppId] IS NOT NULL");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("CC98.Achievement.Data.AchievementItem", b =>
                {
                    b.Property<string>("CategoryName")
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("CodeName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Hint")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IconUri")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDynamic")
                        .HasColumnType("bit");

                    b.Property<int?>("MaxValue")
                        .HasColumnType("int");

                    b.Property<string>("Reward")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SortOrder")
                        .HasColumnType("int");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.HasKey("CategoryName", "CodeName");

                    b.HasIndex("CategoryName", "SortOrder");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("CC98.Achievement.Data.AchievementRecord", b =>
                {
                    b.Property<string>("CategoryName")
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("AchievementName")
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<int?>("CurrentValue")
                        .HasColumnType("int");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset>("Time")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("CategoryName", "AchievementName", "UserName");

                    b.HasIndex("UserName");

                    b.HasIndex("UserName", "IsCompleted", "Time");

                    b.HasIndex("CategoryName", "AchievementName", "IsCompleted", "Time");

                    b.ToTable("Records");
                });

            modelBuilder.Entity("CC98.Achievement.Data.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("UserId");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Gender")
                        .HasColumnType("int")
                        .HasColumnName("Sex");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("UserName");

                    b.Property<string>("PortraitUri")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("face");

                    b.HasKey("Id");

                    b.HasAlternateKey("Name");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CC98.Achievement.Data.AchievementItem", b =>
                {
                    b.HasOne("CC98.Achievement.Data.AchievementCategory", "Category")
                        .WithMany("Items")
                        .HasForeignKey("CategoryName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("CC98.Achievement.Data.AchievementRecord", b =>
                {
                    b.HasOne("CC98.Achievement.Data.AchievementItem", "Achievement")
                        .WithMany("Records")
                        .HasForeignKey("CategoryName", "AchievementName")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Achievement");
                });

            modelBuilder.Entity("CC98.Achievement.Data.AchievementCategory", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("CC98.Achievement.Data.AchievementItem", b =>
                {
                    b.Navigation("Records");
                });
#pragma warning restore 612, 618
        }
    }
}
