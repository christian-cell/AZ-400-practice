using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using User.Repository.EntityConfiguration;

namespace User.Domain.EntityConfigurations
{
    public class UserEntityConfiguration: BaseEntityConfiguration<Entities.User>
    {
        protected override void ConfigureDomainEntity(EntityTypeBuilder<Entities.User> builder)
        {
            builder.Property(x => x.FirstName).IsRequired();
            builder.Property(x => x.LastName).IsRequired();
            builder.Property(x => x.DocumentNumber).IsRequired();
            builder.Property(x => x.Email).IsRequired();
            
            builder.ToTable("Users" , "core");
        }
    }
};

