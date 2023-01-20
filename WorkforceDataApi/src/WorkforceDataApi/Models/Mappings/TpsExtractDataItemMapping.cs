using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkforceDataApi.Models.Mappings;

public class TpsExtractDataItemMapping : IEntityTypeConfiguration<TpsExtractDataItem>
{
    public void Configure(EntityTypeBuilder<TpsExtractDataItem> builder)
    {
        builder.ToTable("tps_extract_data_item");
        builder.HasKey(u => u.TpsExtractDataItemId);
        builder.Property(u => u.TeachingStatus).HasMaxLength(TpsExtractDataItem.TeachingStatusFixedLength).IsFixedLength().IsRequired();
        builder.Property(u => u.Trn).HasMaxLength(TpsExtractDataItem.TrnFixedLength).IsFixedLength().IsRequired();
        builder.HasIndex(u => u.Trn);
        builder.Property(u => u.FirstName).HasMaxLength(TpsExtractDataItem.FirstNameMaxLength).IsRequired();
        builder.Property(u => u.LastName).HasMaxLength(TpsExtractDataItem.LastNameMaxLength).IsRequired();
        builder.Property(u => u.Nino).HasMaxLength(TpsExtractDataItem.NinoFixedLength).IsFixedLength().IsRequired();
        builder.Property(u => u.DateOfBirth).IsRequired();
        builder.Property(u => u.EmailAddress).HasMaxLength(TpsExtractDataItem.EmailAddressMaxLength).IsRequired().UseCollation("case_insensitive");
        builder.Property(u => u.MemberPostcode).HasMaxLength(TpsExtractDataItem.PostcodeMaxLength).IsRequired();
        builder.Property(u => u.LocalAuthorityNumber).HasMaxLength(TpsExtractDataItem.LocalAuthorityNumberFixedLength).IsFixedLength().IsRequired();
        builder.Property(u => u.EstablishmentNumber).HasMaxLength(TpsExtractDataItem.EstablishmentNumberFixedLength).IsFixedLength().IsRequired();
        builder.Property(u => u.EstablishmentPostcode).HasMaxLength(TpsExtractDataItem.PostcodeMaxLength).IsRequired();
        builder.Property(u => u.EmploymentPeriodStartDate).IsRequired();
        builder.Property(u => u.EmploymentPeriodEndDate).IsRequired();
        builder.Property(u => u.Created).IsRequired();
        builder.Property(u => u.Updated).IsRequired();
    }
}
