﻿// <auto-generated />
using System;
using ESCS.Infrastructure.Persistences;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Oracle.EntityFrameworkCore.Metadata;

#nullable disable

namespace ESCS.Infrastructure.Migrations
{
    [DbContext(typeof(EscsDbContext))]
    [Migration("20241119031811_initDb")]
    partial class initDb
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            OracleModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("ESCS.Domain.Models.KeyAllowedEndpoint", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(19)");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<long>("EndpointId")
                        .HasColumnType("NUMBER(19)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("NUMBER(1)");

                    b.Property<long>("UserApiKeyId")
                        .HasColumnType("NUMBER(19)");

                    b.HasKey("Id");

                    b.HasIndex("EndpointId");

                    b.HasIndex("UserApiKeyId");

                    b.ToTable("KeyAllowedEndpoints");
                });

            modelBuilder.Entity("ESCS.Domain.Models.Role", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(19)");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("ESCS.Domain.Models.Service", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(19)");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.HasKey("Id");

                    b.ToTable("Services");
                });

            modelBuilder.Entity("ESCS.Domain.Models.ServiceEndpoint", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(19)");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("NUMBER(1)");

                    b.Property<string>("Method")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<long>("ServiceId")
                        .HasColumnType("NUMBER(19)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.HasKey("Id");

                    b.HasIndex("ServiceId");

                    b.ToTable("ServiceEndpoints");
                });

            modelBuilder.Entity("ESCS.Domain.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(19)");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<int?>("AccessFailedCount")
                        .HasColumnType("NUMBER(10)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<bool?>("LockoutEnabled")
                        .HasColumnType("NUMBER(1)");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("TIMESTAMP(7) WITH TIME ZONE");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<long>("RoleId")
                        .HasColumnType("NUMBER(19)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ESCS.Domain.Models.UserApiKey", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(19)");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<long>("ServiceId")
                        .HasColumnType("NUMBER(19)");

                    b.Property<long>("UserId")
                        .HasColumnType("NUMBER(19)");

                    b.HasKey("Id");

                    b.HasIndex("ServiceId");

                    b.HasIndex("UserId");

                    b.ToTable("UserApiKeys");
                });

            modelBuilder.Entity("ESCS.Domain.Models.UserEmailServiceConfiguration", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(19)");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<long>("ServiceId")
                        .HasColumnType("NUMBER(19)");

                    b.Property<string>("SmtpEmail")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("SmtpPassword")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<int>("SmtpPort")
                        .HasColumnType("NUMBER(10)");

                    b.Property<long>("UserId")
                        .HasColumnType("NUMBER(19)");

                    b.HasKey("Id");

                    b.HasIndex("ServiceId");

                    b.HasIndex("UserId");

                    b.ToTable("UserEmailServiceConfigurations");
                });

            modelBuilder.Entity("ESCS.Domain.Models.KeyAllowedEndpoint", b =>
                {
                    b.HasOne("ESCS.Domain.Models.ServiceEndpoint", "ServiceEndpoint")
                        .WithMany()
                        .HasForeignKey("EndpointId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ESCS.Domain.Models.UserApiKey", "UserApiKey")
                        .WithMany()
                        .HasForeignKey("UserApiKeyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ServiceEndpoint");

                    b.Navigation("UserApiKey");
                });

            modelBuilder.Entity("ESCS.Domain.Models.ServiceEndpoint", b =>
                {
                    b.HasOne("ESCS.Domain.Models.Service", "Service")
                        .WithMany()
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Service");
                });

            modelBuilder.Entity("ESCS.Domain.Models.User", b =>
                {
                    b.HasOne("ESCS.Domain.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("ESCS.Domain.Models.UserApiKey", b =>
                {
                    b.HasOne("ESCS.Domain.Models.Service", "Service")
                        .WithMany()
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ESCS.Domain.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Service");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ESCS.Domain.Models.UserEmailServiceConfiguration", b =>
                {
                    b.HasOne("ESCS.Domain.Models.Service", "Service")
                        .WithMany()
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ESCS.Domain.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Service");

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
