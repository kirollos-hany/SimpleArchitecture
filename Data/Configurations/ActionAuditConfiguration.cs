using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SimpleArchitecture.Auditing.Enums;
using SimpleArchitecture.Auditing.Types;
using SimpleArchitecture.Authentication.Types;

namespace SimpleArchitecture.Data.Configurations;

public class ActionAuditConfiguration : IEntityTypeConfiguration<ActionAudit>
{
    public void Configure(EntityTypeBuilder<ActionAudit> builder)
    {
        builder
            .ToTable("ActionsAudits");

        builder
            .HasOne(audit => audit.User)
            .WithMany()
            .HasForeignKey(audit => audit.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .OwnsOne(audit => audit.DeviceInfo);

        builder
            .Property(audit => audit.Type)
            .HasConversion(new EnumToStringConverter<AuditActionType>());
    }
}