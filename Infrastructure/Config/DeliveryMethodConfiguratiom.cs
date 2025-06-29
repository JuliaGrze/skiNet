using CORE.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Config
{
    class DeliveryMethodConfiguratiom : IEntityTypeConfiguration<DeliveryMethod>
    {
        //DeliveryMethod entity configurator, which allows you to specify how the DeliveryMethod class should be mapped to the database
        public void Configure(EntityTypeBuilder<DeliveryMethod> builder)
        {
            builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
        }
    }
}
