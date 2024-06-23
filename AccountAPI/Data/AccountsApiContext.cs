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

        // Define a relação de muitos para muitos entre Agência e Funcionário
        modelBuilder.Entity<AgencyEmployee>()
            .HasOne(oa => oa.Agency)
            .WithMany(o => o.AgencyEmployees)
            .HasForeignKey(oa => oa.AgencyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AgencyEmployee>()
            .HasOne(oa => oa.Employee)
            .WithMany(o => o.AgencyEmployees)
            .HasForeignKey(oa => oa.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Define a relação de um para muitos entre Conta e Cliente
        modelBuilder.Entity<Account>()
            .HasOne(a => a.SecundaryCustomer)
            .WithMany()
            .HasForeignKey(a => a.SecundaryCustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Account>()
            .HasOne(a => a.MainCustomer)
            .WithMany()
            .HasForeignKey(a => a.MainCustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Define chaves compostas
        modelBuilder.Entity<OperationAccount>().HasKey(ac => new { ac.AccountId, ac.OperationId });
        modelBuilder.Entity<AgencyEmployee>().HasKey(ac => new { ac.AgencyId, ac.EmployeeId });

        // Remove 'DELETE CASCADE' para evitar comportamento ciclico
    }

    public DbSet<Account> Account { get; set; } = default!;
}