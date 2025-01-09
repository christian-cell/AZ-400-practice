using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using User.Repository.Entities;

namespace User.Repository.EntityConfiguration
{
    public abstract class BaseEntityConfiguration<TEntityBase> : IEntityTypeConfiguration<TEntityBase> where TEntityBase : class, IEntityBase
    {
        public virtual void Configure(EntityTypeBuilder<TEntityBase> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.CreatedOn).IsRequired();
            builder.Property(e => e.DeletedOn).IsRequired(required: false);
            builder.Property(e => e.ModifiedOn).IsRequired(required: false);

            ConfigureDomainEntity(builder);
        }

        protected abstract void ConfigureDomainEntity(EntityTypeBuilder<TEntityBase> builder);
    }
};