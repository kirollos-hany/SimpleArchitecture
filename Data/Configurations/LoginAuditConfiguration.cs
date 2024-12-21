using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SimpleArchitecture.Auditing.Types;
using SimpleArchitecture.Authentication.Enums;
using SimpleArchitecture.Authentication.Types;

namespace SimpleArchitecture.Data.Configurations;

internal class LoginAuditConfiguration : IEntityTypeConfiguration<LoginAudit>
{
    public void Configure(EntityTypeBuilder<LoginAudit> builder)
    {
        builder
            .OwnsOne(loginAudit => loginAudit.DeviceInfo);

        builder
            .HasOne(loginAudit => loginAudit.User)
            .WithMany()
            .HasForeignKey(loginAudit => loginAudit.UserId);

        builder
            .ToTable("LoginAudits");

        builder
            .Property(loginAudit => loginAudit.Status)
            .HasConversion(new EnumToStringConverter<LoginStatus>());
    }
}