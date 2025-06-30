using CORE.Entities.OrderAggreagte;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Config
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            //OwnsOne(...) – informuje EF Core, że ShippingAddress to owned type (czyli value object) zagnieżdżony w Order, a nie osobna encja z własną tabelą
            //.WithOwner() – potwierdza, że Order jest właścicielem ShippingAddress, i że jego dane zostaną zapisane w tej samej tabeli co Order
            builder.OwnsOne(x => x.ShippingAddress, o => o.WithOwner());
            builder.OwnsOne(x => x.PaymentSummarry, o => o.WithOwner());
            //convert enum to string
            builder.Property(x => x.Status).HasConversion<String>();
            builder.Property(x => x.Subtotal).HasColumnType("decimal(18,2)");

            //wymusza stworzenie relacji jeden-do-wielu, mimo że nie ma odwrotnej nawigacji w OrderItem
            //Jeśli usuniesz Order, to wszystkie jego OrderItems zostaną automatycznie usunięte z bazy danych.
            builder.HasMany(x => x.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.OrderDate).HasConversion(
                d => d.ToUniversalTime(), //zapis do bazy
                d => DateTime.SpecifyKind(d, DateTimeKind.Utc) //odczytu z bazy  - to był czas w UTC – traktuj go jako UTC
            );
        }
    }
}
