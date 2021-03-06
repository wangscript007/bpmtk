﻿using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Bpmtk.Engine.Models;

namespace Bpmtk.Engine.Storage.Builders
{
    public class TokenConfiguration : EntityTypeConfiguration<Token>
    {
        public override void Configure(EntityTypeBuilder<Token> builder)
        {
            //ignore IsEnded/Node.
            builder.Ignore(x => x.IsEnded);
            builder.Ignore(x => x.Node);

            builder.HasKey(x => x.Id);

            builder.HasMany(x => x.Children)
                .WithOne(x => x.Parent)
                .HasForeignKey("ParentId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ProcessInstance)
                .WithMany()
                .HasForeignKey("ProcessInstanceId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(true);

            builder.HasMany(x => x.Variables)
                .WithOne()
                .HasForeignKey("TokenId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.SubProcessInstance)
                .WithMany()
                .HasForeignKey("SubProcessInstanceId")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ActivityInstance)
                .WithMany()
                .HasForeignKey("ActivityInstanceId")
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(x => x.ActivityId).HasMaxLength(64);
            builder.Property(x => x.TransitionId).HasMaxLength(64);
            builder.Property(x => x.IsActive).IsRequired(true);
                //.HasConversion(new BoolToZeroOneConverter<Int16>());
            builder.Property(x => x.IsSuspended).IsRequired(true);
                //.HasConversion(new BoolToZeroOneConverter<Int16>());
            builder.Property(x => x.IsScope).IsRequired(true); 
                //.HasConversion(new BoolToZeroOneConverter<Int16>());
            builder.Property(x => x.IsMIRoot).IsRequired(true);
                //.HasConversion(new BoolToZeroOneConverter<Int16>());
        }
    }
}
