using AulaFlow.Desktop.Models;
using AulaFlow.Desktop.Services;
using Xunit;

namespace AulaFlow.Tests;

public class MaterialToolsTests
{
    [Fact]
    public void BuscaCombinaTextoEStatus()
    {
        var materiais = new[] {
            new Material { Nome = "Aula 1", Disciplina = "Direito", Status = "Pendente" },
            new Material { Nome = "Aula 2", Disciplina = "Direito", Status = "Concluído" } };
        var resultado = MaterialTools.Filtrar(materiais, " DIREITO ", "Pendente");
        Assert.Single(resultado);
        Assert.Equal("Aula 1", resultado[0].Nome);
        Assert.Empty(MaterialTools.Filtrar(materiais, "inexistente", ""));
    }

    [Fact]
    public void CsvEscapaAspasSeparadoresEFormulas()
    {
        var csv = MaterialTools.GerarCsv([new Material { Nome = "=1+1", Curso = "Curso; \"A\"" }]);
        Assert.Contains("\"'=1+1\"", csv);
        Assert.Contains("\"Curso; \"\"A\"\"\"", csv);
    }

    [Fact]
    public async Task CopiaPreservaOriginalERecusaSobrescrita()
    {
        var pasta = Path.Combine(Path.GetTempPath(), "AulaFlowTests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(pasta);
        try
        {
            var origem = Path.Combine(pasta, "aula.txt");
            await File.WriteAllTextAsync(origem, "conteúdo de teste");
            var material = new Material { Id = 7, Nome = "Aula", Tipo = "Documento", Curso = "../Curso", Disciplina = "CON", CaminhoArquivo = origem };
            var destino = Path.Combine(pasta, "saida");
            var copia = await MaterialTools.CopiarOrganizadoAsync(material, destino);
            Assert.StartsWith(Path.GetFullPath(destino) + Path.DirectorySeparatorChar, copia);
            Assert.Equal(await File.ReadAllTextAsync(origem), await File.ReadAllTextAsync(copia));
            await Assert.ThrowsAsync<IOException>(() => MaterialTools.CopiarOrganizadoAsync(material, destino));
            Assert.True(File.Exists(origem));
            Assert.Equal("conteúdo de teste", await File.ReadAllTextAsync(copia));
        }
        finally { Directory.Delete(pasta, true); }
    }

    [Fact]
    public void ValidacaoRecusaCadastroIncompleto()
    {
        Assert.Throws<ArgumentException>(() => MaterialTools.Validar(new Material()));
    }
}
