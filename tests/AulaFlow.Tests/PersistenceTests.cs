using AulaFlow.Desktop.Data;
using AulaFlow.Desktop.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AulaFlow.Tests;

public class PersistenceTests
{
    [Fact]
    public void SqlitePersisteEditaEExcluiEntreContextos()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<AulaFlowDbContext>().UseSqlite(connection).Options;
        int id;
        using (var db = new AulaFlowDbContext(options))
        {
            db.Database.EnsureCreated();
            var material = new Material { Nome = "Aula SQL", Tipo = "Vídeo", Curso = "Teste", Disciplina = "Dados" };
            db.Materiais.Add(material);
            db.SaveChanges();
            id = material.Id;
            Assert.True(id > 0);
        }
        using (var db = new AulaFlowDbContext(options))
        {
            var material = db.Materiais.Single(m => m.Id == id);
            Assert.Equal("Aula SQL", material.Nome);
            material.Status = "Concluído";
            db.SaveChanges();
        }
        using (var db = new AulaFlowDbContext(options))
        {
            var material = db.Materiais.Single();
            Assert.Equal("Concluído", material.Status);
            db.Materiais.Remove(material);
            db.SaveChanges();
            Assert.Empty(db.Materiais);
        }
    }
}
