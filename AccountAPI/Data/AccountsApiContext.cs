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
        // Configura as tabelas da Account
        modelBuilder.Entity<DisabledAccount>()
            .ToTable("DisabledAccount");

        // Configura chave primária sem incremento automático
        modelBuilder.Entity<CreditCard>(entity =>
        {
            entity.HasKey(c => c.Number);
            entity.Property(c => c.Number).ValueGeneratedNever();
        });

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
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Account> Account { get; set; } = default!;
    public DbSet<DisabledAccount> DisabledAccount { get; set; } = default!;
    public DbSet<Operation> Operation { get; set; } = default!;
    public DbSet<OperationAccount> OperationAccount { get; set; } = default!;
    public DbSet<Loan> Loan { get; set; } = default!;
}