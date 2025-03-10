﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace hub.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240729175245_AddCompositeKeyToUser")]
    partial class AddCompositeKeyToUser
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Chat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTimeOffset>("LastUpdated")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Chats");
                });

            modelBuilder.Entity("ChatUser", b =>
                {
                    b.Property<int>("ChatsId")
                        .HasColumnType("integer");

                    b.Property<int>("UsersSystem")
                        .HasColumnType("integer");

                    b.Property<string>("UsersExternalId")
                        .HasColumnType("text");

                    b.HasKey("ChatsId", "UsersSystem", "UsersExternalId");

                    b.HasIndex("UsersSystem", "UsersExternalId");

                    b.ToTable("ChatUser");
                });

            modelBuilder.Entity("Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ChatId")
                        .HasColumnType("integer");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ReactionData")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SenderExternalId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("SenderSystem")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("SentAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.HasIndex("SenderSystem", "SenderExternalId");

                    b.ToTable("Message");
                });

            modelBuilder.Entity("User", b =>
                {
                    b.Property<int>("System")
                        .HasColumnType("integer");

                    b.Property<string>("ExternalId")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("MessageId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("System", "ExternalId");

                    b.HasIndex("MessageId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("ChatUser", b =>
                {
                    b.HasOne("Chat", null)
                        .WithMany()
                        .HasForeignKey("ChatsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("User", null)
                        .WithMany()
                        .HasForeignKey("UsersSystem", "UsersExternalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Message", b =>
                {
                    b.HasOne("Chat", "Chat")
                        .WithMany("Messages")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("User", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderSystem", "SenderExternalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chat");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("User", b =>
                {
                    b.HasOne("Message", null)
                        .WithMany("ReadBy")
                        .HasForeignKey("MessageId");
                });

            modelBuilder.Entity("Chat", b =>
                {
                    b.Navigation("Messages");
                });

            modelBuilder.Entity("Message", b =>
                {
                    b.Navigation("ReadBy");
                });
#pragma warning restore 612, 618
        }
    }
}
