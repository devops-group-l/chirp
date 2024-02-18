﻿// <auto-generated />
using System;
using Chirp.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Chirp.Infrastructure.Migrations
{
    [DbContext(typeof(ChirpDbContext))]
    partial class ChirpDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AuthorAuthor", b =>
                {
                    b.Property<Guid>("FollowedByAuthorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("FollowsAuthorId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("FollowedByAuthorId", "FollowsAuthorId");

                    b.HasIndex("FollowsAuthorId");

                    b.ToTable("AuthorAuthor");
                });

            modelBuilder.Entity("Chirp.Infrastructure.Models.Author", b =>
                {
                    b.Property<Guid>("AuthorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AvatarUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.HasKey("AuthorId");

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("Chirp.Infrastructure.Models.Cheep", b =>
                {
                    b.Property<Guid>("CheepId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(160)
                        .HasColumnType("nvarchar(160)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.HasKey("CheepId");

                    b.HasIndex("AuthorId");

                    b.ToTable("Cheeps");
                });

            modelBuilder.Entity("Chirp.Infrastructure.Models.Comment", b =>
                {
                    b.Property<Guid>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AuthorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CheepId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(160)
                        .HasColumnType("nvarchar(160)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.HasKey("CommentId");

                    b.HasIndex("AuthorId");

                    b.HasIndex("CheepId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Chirp.Infrastructure.Models.Like", b =>
                {
                    b.Property<Guid>("LikeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AuthorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CheepId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("LikeId");

                    b.HasIndex("AuthorId");

                    b.HasIndex("CheepId");

                    b.ToTable("Likes");
                });

            modelBuilder.Entity("AuthorAuthor", b =>
                {
                    b.HasOne("Chirp.Infrastructure.Models.Author", null)
                        .WithMany()
                        .HasForeignKey("FollowedByAuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Chirp.Infrastructure.Models.Author", null)
                        .WithMany()
                        .HasForeignKey("FollowsAuthorId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Chirp.Infrastructure.Models.Cheep", b =>
                {
                    b.HasOne("Chirp.Infrastructure.Models.Author", "Author")
                        .WithMany("Cheeps")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("Chirp.Infrastructure.Models.Comment", b =>
                {
                    b.HasOne("Chirp.Infrastructure.Models.Author", "CommentAuthor")
                        .WithMany("Comments")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Chirp.Infrastructure.Models.Cheep", "Cheep")
                        .WithMany("Comments")
                        .HasForeignKey("CheepId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Cheep");

                    b.Navigation("CommentAuthor");
                });

            modelBuilder.Entity("Chirp.Infrastructure.Models.Like", b =>
                {
                    b.HasOne("Chirp.Infrastructure.Models.Author", "LikedByAuthor")
                        .WithMany("Likes")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Chirp.Infrastructure.Models.Cheep", "Cheep")
                        .WithMany("Likes")
                        .HasForeignKey("CheepId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Cheep");

                    b.Navigation("LikedByAuthor");
                });

            modelBuilder.Entity("Chirp.Infrastructure.Models.Author", b =>
                {
                    b.Navigation("Cheeps");

                    b.Navigation("Comments");

                    b.Navigation("Likes");
                });

            modelBuilder.Entity("Chirp.Infrastructure.Models.Cheep", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Likes");
                });
#pragma warning restore 612, 618
        }
    }
}
