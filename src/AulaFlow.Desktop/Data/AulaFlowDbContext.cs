
using System;
using System.IO;

using AulaFlow.Desktop.Models;
using Microsoft.EntityFrameworkCore;

namespace AulaFlow.Desktop.Data;

public class AulaFlowDbContext : DbContext
{
    public AulaFlowDbContext() { }

    public AulaFlowDbContext(DbContextOptions<AulaFlowDbContext> options) : base(options) { }

    public DbSet<Material> Materiais { get; set; } = null!;

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;

        string pastaDados = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "AulaFlow"
        );

        Directory.CreateDirectory(pastaDados);

        string caminhoBanco = Path.Combine(
            pastaDados,
            "aulaflow.db"
        );

        optionsBuilder.UseSqlite(
            $"Data Source={caminhoBanco}"
        );
    }
}
