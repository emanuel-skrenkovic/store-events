﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Store.Catalogue.Infrastructure;

namespace Store.Catalogue.Infrastructure.Migrations
{
    [DbContext(typeof(StoreCatalogueDbContext))]
    [Migration("20211107144852_subscription_checkpoint_table")]
    partial class subscription_checkpoint_table
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("public")
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Store.Catalogue.Infrastructure.Entity.ProductDisplayEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Data")
                        .HasColumnType("jsonb")
                        .HasColumnName("data");

                    b.HasKey("Id");

                    b.ToTable("product_display");
                });

            modelBuilder.Entity("Store.Core.Infrastructure.EntityFramework.Entity.SubscriptionCheckpointEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<decimal>("Position")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("position");

                    b.Property<string>("SubscriptionId")
                        .HasColumnType("text")
                        .HasColumnName("subscription_id");

                    b.HasKey("Id");

                    b.HasIndex("SubscriptionId");

                    b.ToTable("subscription_checkpoint");
                });
#pragma warning restore 612, 618
        }
    }
}
