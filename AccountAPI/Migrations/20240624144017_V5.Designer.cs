﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AccountAPI.Migrations
{
    [DbContext(typeof(AccountsApiContext))]
    [Migration("20240624144017_V5")]
    partial class V5
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.31")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Models.Account", b =>
                {
                    b.Property<string>("Number")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AgencyNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Balance")
                        .HasColumnType("float");

                    b.Property<long?>("CreditCardNumber")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("MainClientId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Profile")
                        .HasColumnType("int");

                    b.Property<bool>("Restriction")
                        .HasColumnType("bit");

                    b.Property<string>("SavingsAccountNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SecondaryClientId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("SpecialLimit")
                        .HasColumnType("float");

                    b.HasKey("Number");

                    b.HasIndex("CreditCardNumber");

                    b.ToTable("Account");
                });

            modelBuilder.Entity("Models.CreditCard", b =>
                {
                    b.Property<long>("Number")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Number"), 1L, 1);

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Flag")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Limit")
                        .HasColumnType("float");

                    b.Property<string>("Owner")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SecurityCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Number");

                    b.ToTable("CreditCard");
                });

            modelBuilder.Entity("Models.DeletedAccount", b =>
                {
                    b.Property<string>("Number")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AgencyNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Balance")
                        .HasColumnType("float");

                    b.Property<long?>("CreditCardNumber")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("MainClientId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Profile")
                        .HasColumnType("int");

                    b.Property<bool>("Restriction")
                        .HasColumnType("bit");

                    b.Property<string>("SavingsAccountNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SecondaryClientId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("SpecialLimit")
                        .HasColumnType("float");

                    b.HasKey("Number");

                    b.HasIndex("CreditCardNumber");

                    b.ToTable("DeletedAccount", (string)null);
                });

            modelBuilder.Entity("Models.Operation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("AccountNumber")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<double>("Value")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("AccountNumber");

                    b.ToTable("Operation");
                });

            modelBuilder.Entity("Models.OperationAccount", b =>
                {
                    b.Property<string>("AccountId")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnOrder(1);

                    b.Property<int>("OperationId")
                        .HasColumnType("int")
                        .HasColumnOrder(2);

                    b.HasKey("AccountId", "OperationId");

                    b.HasIndex("OperationId");

                    b.ToTable("OperationAccount");
                });

            modelBuilder.Entity("Models.Account", b =>
                {
                    b.HasOne("Models.CreditCard", "CreditCard")
                        .WithMany()
                        .HasForeignKey("CreditCardNumber");

                    b.Navigation("CreditCard");
                });

            modelBuilder.Entity("Models.DeletedAccount", b =>
                {
                    b.HasOne("Models.CreditCard", "CreditCard")
                        .WithMany()
                        .HasForeignKey("CreditCardNumber");

                    b.Navigation("CreditCard");
                });

            modelBuilder.Entity("Models.Operation", b =>
                {
                    b.HasOne("Models.Account", "Account")
                        .WithMany("Statement")
                        .HasForeignKey("AccountNumber");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Models.OperationAccount", b =>
                {
                    b.HasOne("Models.Account", "Account")
                        .WithMany("OperationAccounts")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Models.Operation", "Operation")
                        .WithMany("OperationAccounts")
                        .HasForeignKey("OperationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Operation");
                });

            modelBuilder.Entity("Models.Account", b =>
                {
                    b.Navigation("OperationAccounts");

                    b.Navigation("Statement");
                });

            modelBuilder.Entity("Models.Operation", b =>
                {
                    b.Navigation("OperationAccounts");
                });
#pragma warning restore 612, 618
        }
    }
}
