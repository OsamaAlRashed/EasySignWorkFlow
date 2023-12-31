﻿// <auto-generated />
using System;
using Demo.Models.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Demo.Migrations
{
    [DbContext(typeof(DemoDBContext))]
    [Migration("20230922164058_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.21")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Demo.Models.TestRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Flag")
                        .HasColumnType("bit");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("TestRequests");
                });

            modelBuilder.Entity("Demo.Models.TestRequest", b =>
                {
                    b.OwnsMany("TestRequestStatus", "Statuses", b1 =>
                        {
                            b1.Property<Guid>("TestRequestId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"), 1L, 1);

                            b1.Property<DateTime?>("DateSigned")
                                .HasColumnType("datetime2");

                            b1.Property<string>("Note")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<Guid>("SignedBy")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Status")
                                .HasColumnType("int");

                            b1.HasKey("TestRequestId", "Id");

                            b1.ToTable("TestRequestStatus");

                            b1.WithOwner()
                                .HasForeignKey("TestRequestId");
                        });

                    b.Navigation("Statuses");
                });
#pragma warning restore 612, 618
        }
    }
}
