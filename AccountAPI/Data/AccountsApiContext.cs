using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;

public class AccountsApiContext : DbContext
{
    public AccountsApiContext(DbContextOptions<AccountsApiContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configura chave primária sem incremento automático
        modelBuilder.Entity<CreditCard>()
            .HasKey(c => c.Number);

        // Define a relação de muitos para muitos entre Conta e Operação
        modelBuilder.Entity<OperationAccount>()
            .HasOne(oa => oa.Account)
            .WithMany(o => o.OperationAccounts)
            .HasForeignKey(oa => oa.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OperationAccount>()
            .HasOne(oa => oa.Operation)
            .WithMany(o => o.OperationAccounts)
            .HasForeignKey(oa => oa.OperationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Define chaves compostas
        modelBuilder.Entity<OperationAccount>().HasKey(ac => new { ac.AccountId, ac.OperationId });
    }

    public DbSet<Account> Account { get; set; } = default!;
}