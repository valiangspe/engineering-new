﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SupportReportAPI.Models;

#nullable disable

namespace backend.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241126071735_CreateNotificationsTable")]
    partial class CreateNotificationsTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsRead")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("TaskId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("PanelProcess", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<double>("Minutes")
                        .HasColumnType("double");

                    b.Property<string>("PanelType")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ProcessName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("PanelProcesses");
                });

            modelBuilder.Entity("SupportReportAPI.Models.BomApproval", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int?>("Id"));

                    b.Property<DateTime?>("ApprovalDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("ExtBomLeveledId")
                        .HasColumnType("int");

                    b.Property<int?>("ExtUserId")
                        .HasColumnType("int");

                    b.Property<string>("Remark")
                        .HasColumnType("longtext");

                    b.Property<int?>("Status")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("BomApprovals");
                });

            modelBuilder.Entity("SupportReportAPI.Models.BomApprovalPic", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int?>("Id"));

                    b.Property<DateTime?>("ApprovalDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("BomApprovalId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("ExtBomLeveledId")
                        .HasColumnType("int");

                    b.Property<int?>("ExtUserId")
                        .HasColumnType("int");

                    b.Property<string>("Remark")
                        .HasColumnType("longtext");

                    b.Property<int?>("Status")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("BomApprovalId");

                    b.ToTable("BomApprovalPics");
                });

            modelBuilder.Entity("SupportReportAPI.Models.CCNData", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int?>("Id"));

                    b.Property<int?>("Count")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Month")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("CCNData");
                });

            modelBuilder.Entity("SupportReportAPI.Models.Configuration", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int?>("Id"));

                    b.Property<string>("ByValuePercentage")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<double?>("Percentage")
                        .HasColumnType("double");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<double?>("Value")
                        .HasColumnType("double");

                    b.HasKey("Id");

                    b.ToTable("Configurations");
                });

            modelBuilder.Entity("SupportReportAPI.Models.DashboardMetrics", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int?>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("LastDetectedProcess")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("NumberOfCCNsThisMonth")
                        .HasColumnType("int");

                    b.Property<int?>("NumberOfECNsThisMonth")
                        .HasColumnType("int");

                    b.Property<int?>("NumberOfManpowers")
                        .HasColumnType("int");

                    b.Property<int?>("TotalBOMs")
                        .HasColumnType("int");

                    b.Property<int?>("TotalDrawings")
                        .HasColumnType("int");

                    b.Property<int?>("TotalProducts")
                        .HasColumnType("int");

                    b.Property<int?>("TotalSLDs")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("DashboardMetrics");
                });

            modelBuilder.Entity("SupportReportAPI.Models.DonePO", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int?>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Detail")
                        .HasColumnType("longtext");

                    b.Property<string>("JobCategory")
                        .HasColumnType("longtext");

                    b.Property<int?>("Outs")
                        .HasColumnType("int");

                    b.Property<double?>("TProcess")
                        .HasColumnType("double");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("DonePOs");
                });

            modelBuilder.Entity("SupportReportAPI.Models.ECNData", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int?>("Id"));

                    b.Property<int?>("Count")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Month")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("ECNData");
                });

            modelBuilder.Entity("SupportReportAPI.Models.EngDeptConfig", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int?>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("DepartmentId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("EngDeptConfigs");
                });

            modelBuilder.Entity("SupportReportAPI.Models.EngineerSupport", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int?>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Engineer")
                        .HasColumnType("longtext");

                    b.Property<string>("Project")
                        .HasColumnType("longtext");

                    b.Property<string>("Requester")
                        .HasColumnType("longtext");

                    b.Property<string>("SupportDates")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("EngineerSupports");
                });

            modelBuilder.Entity("SupportReportAPI.Models.EngineeringActivity", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int?>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Customer")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<int?>("ExtInquiryId")
                        .HasColumnType("int");

                    b.Property<int?>("ExtJobId")
                        .HasColumnType("int");

                    b.Property<int?>("ExtPanelCodeId")
                        .HasColumnType("int");

                    b.Property<int?>("ExtPurchaseOrderId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("FromCache")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("PIC")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("ToCache")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("EngineeringActivities");
                });

            modelBuilder.Entity("SupportReportAPI.Models.EngineeringDetailProblem", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int?>("Id"));

                    b.Property<DateTime?>("ApprovalDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("ApprovalExtUserId")
                        .HasColumnType("int");

                    b.Property<string>("ApprovalFileName")
                        .HasColumnType("longtext");

                    b.Property<string>("ApprovalRemark")
                        .HasColumnType("longtext");

                    b.Property<double?>("Cost")
                        .HasColumnType("double");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Cust")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<string>("DetailProblem")
                        .HasColumnType("longtext");

                    b.Property<string>("Engineering")
                        .HasColumnType("longtext");

                    b.Property<int?>("ExtItemId")
                        .HasColumnType("int");

                    b.Property<int?>("ExtJobId")
                        .HasColumnType("int");

                    b.Property<int?>("ExtPanelCodeId")
                        .HasColumnType("int");

                    b.Property<int?>("ExtPurchaseOrderId")
                        .HasColumnType("int");

                    b.Property<int?>("ExtUserId")
                        .HasColumnType("int");

                    b.Property<string>("No")
                        .HasColumnType("longtext");

                    b.Property<string>("Pn")
                        .HasColumnType("longtext");

                    b.Property<string>("PoNumber")
                        .HasColumnType("longtext");

                    b.Property<string>("ProjectName")
                        .HasColumnType("longtext");

                    b.Property<int?>("Qty")
                        .HasColumnType("int");

                    b.Property<int?>("Status")
                        .HasColumnType("int");

                    b.Property<DateTime?>("Tgl")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("Type")
                        .HasColumnType("int");

                    b.Property<int?>("TypeEcnCcn")
                        .HasColumnType("int");

                    b.Property<string>("Uom")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("EngineeringDetailProblems");
                });

            modelBuilder.Entity("SupportReportAPI.Models.EngineeringDetailProblemApproval", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int?>("Id"));

                    b.Property<DateTime?>("ApprovalDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("EngineeringDetailProblemId")
                        .HasColumnType("int");

                    b.Property<int?>("ExtUserId")
                        .HasColumnType("int");

                    b.Property<int?>("Status")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("EngineeringDetailProblemId");

                    b.ToTable("EngineeringDetailProblemApproval");
                });

            modelBuilder.Entity("SupportReportAPI.Models.EngineeringDetailProblemItem", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int?>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("EngineeringDetailProblemId")
                        .HasColumnType("int");

                    b.Property<int?>("ExtItemId")
                        .HasColumnType("int");

                    b.Property<double?>("Qty")
                        .HasColumnType("double");

                    b.Property<DateTime?>("SnapshotDate")
                        .HasColumnType("datetime(6)");

                    b.Property<double?>("SnapshotPrice")
                        .HasColumnType("double");

                    b.Property<int?>("TypeIncreaseDecrease")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("EngineeringDetailProblemId");

                    b.ToTable("EngineeringDetailProblemItem");
                });

            modelBuilder.Entity("SupportReportAPI.Models.InCharge", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int?>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Customer")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<int?>("ExtUserId")
                        .HasColumnType("int");

                    b.Property<string>("PIC")
                        .HasColumnType("longtext");

                    b.Property<int?>("TaskId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("TaskId");

                    b.ToTable("InCharges");
                });

            modelBuilder.Entity("SupportReportAPI.Models.InquiryAIGeneration", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int?>("Id"));

                    b.Property<string>("Content")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("FileName")
                        .HasColumnType("longtext");

                    b.Property<int?>("InquiryId")
                        .HasColumnType("int");

                    b.Property<string>("Model")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("InquiryAIGenerations");
                });

            modelBuilder.Entity("SupportReportAPI.Models.OutstandingPostPO", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int?>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Customer")
                        .HasColumnType("longtext");

                    b.Property<int?>("DaysDeadline")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("DoneOuts")
                        .HasColumnType("longtext");

                    b.Property<int?>("OthersOuts")
                        .HasColumnType("int");

                    b.Property<int?>("OthersProcess")
                        .HasColumnType("int");

                    b.Property<int?>("PostPoOuts")
                        .HasColumnType("int");

                    b.Property<int?>("PostPoProcess")
                        .HasColumnType("int");

                    b.Property<int?>("PrePoOuts")
                        .HasColumnType("int");

                    b.Property<int?>("PrePoProcess")
                        .HasColumnType("int");

                    b.Property<string>("Project")
                        .HasColumnType("longtext");

                    b.Property<int?>("TotalOuts")
                        .HasColumnType("int");

                    b.Property<int?>("TotalProcess")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("OutstandingPostPOs");
                });

            modelBuilder.Entity("SupportReportAPI.Models.SupportDetail", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int?>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Cust")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<string>("DetailProblem")
                        .HasColumnType("longtext");

                    b.Property<string>("Engineering")
                        .HasColumnType("longtext");

                    b.Property<string>("No")
                        .HasColumnType("longtext");

                    b.Property<string>("Pn")
                        .HasColumnType("longtext");

                    b.Property<string>("PoNumber")
                        .HasColumnType("longtext");

                    b.Property<string>("ProjectName")
                        .HasColumnType("longtext");

                    b.Property<int?>("Qty")
                        .HasColumnType("int");

                    b.Property<DateTime?>("Tgl")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Uom")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("SupportDetails");
                });

            modelBuilder.Entity("SupportReportAPI.Models.Task", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int?>("Id"));

                    b.Property<int?>("CompletedByManagerId")
                        .HasColumnType("int");

                    b.Property<int?>("CompletedByPicId")
                        .HasColumnType("int");

                    b.Property<int?>("CompletedBySpvId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CompletedDateManager")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("CompletedDatePic")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("CompletedDateSpv")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<int?>("EngineeringActivityId")
                        .HasColumnType("int");

                    b.Property<int?>("ExtPanelCodeId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("From")
                        .HasColumnType("datetime(6)");

                    b.Property<double?>("Hours")
                        .HasColumnType("double");

                    b.Property<string>("Remark")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("To")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("EngineeringActivityId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("UserRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("SupportReportAPI.Models.BomApprovalPic", b =>
                {
                    b.HasOne("SupportReportAPI.Models.BomApproval", "BomApproval")
                        .WithMany("Pics")
                        .HasForeignKey("BomApprovalId");

                    b.Navigation("BomApproval");
                });

            modelBuilder.Entity("SupportReportAPI.Models.EngineeringDetailProblemApproval", b =>
                {
                    b.HasOne("SupportReportAPI.Models.EngineeringDetailProblem", "EngineeringDetailProblem")
                        .WithMany("Approvals")
                        .HasForeignKey("EngineeringDetailProblemId");

                    b.Navigation("EngineeringDetailProblem");
                });

            modelBuilder.Entity("SupportReportAPI.Models.EngineeringDetailProblemItem", b =>
                {
                    b.HasOne("SupportReportAPI.Models.EngineeringDetailProblem", "EngineeringDetailProblem")
                        .WithMany("Items")
                        .HasForeignKey("EngineeringDetailProblemId");

                    b.Navigation("EngineeringDetailProblem");
                });

            modelBuilder.Entity("SupportReportAPI.Models.InCharge", b =>
                {
                    b.HasOne("SupportReportAPI.Models.Task", "Task")
                        .WithMany("InCharges")
                        .HasForeignKey("TaskId");

                    b.Navigation("Task");
                });

            modelBuilder.Entity("SupportReportAPI.Models.Task", b =>
                {
                    b.HasOne("SupportReportAPI.Models.EngineeringActivity", "EngineeringActivity")
                        .WithMany("Tasks")
                        .HasForeignKey("EngineeringActivityId");

                    b.Navigation("EngineeringActivity");
                });

            modelBuilder.Entity("SupportReportAPI.Models.BomApproval", b =>
                {
                    b.Navigation("Pics");
                });

            modelBuilder.Entity("SupportReportAPI.Models.EngineeringActivity", b =>
                {
                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("SupportReportAPI.Models.EngineeringDetailProblem", b =>
                {
                    b.Navigation("Approvals");

                    b.Navigation("Items");
                });

            modelBuilder.Entity("SupportReportAPI.Models.Task", b =>
                {
                    b.Navigation("InCharges");
                });
#pragma warning restore 612, 618
        }
    }
}