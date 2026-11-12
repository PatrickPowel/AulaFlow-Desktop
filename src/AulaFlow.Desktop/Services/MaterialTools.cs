using System.IO;
using System.Text;
using AulaFlow.Desktop.Models;

namespace AulaFlow.Desktop.Services;

// Regras independentes da interface: podem ser exercitadas sem abrir o WPF.
public static class MaterialTools
{
    public static readonly string[] Tipos = ["Vídeo", "Áudio", "Documento"];
    public static readonly string[] Status = ["Pendente", "Em processamento", "Concluído", "Erro"];

    public static void Validar(Material material)
    {
        if (new[] { material.Nome, material.Curso, material.Disciplina, material.CaminhoArquivo }
            .Any(string.IsNullOrWhiteSpace))
            throw new ArgumentException("Preencha todos os campos obrigatórios.");
        if (!Tipos.Contains(material.Tipo) || !Status.Contains(material.Status))
            throw new ArgumentException("Selecione um tipo e um status válidos.");
        if (!File.Exists(material.CaminhoArquivo))
            throw new ArgumentException("O arquivo informado não existe.");
    }

    public static List<Material> Filtrar(IEnumerable<Material> materiais, string busca, string status)
    {
        busca = busca.Trim();
        return materiais.Where(m => (string.IsNullOrEmpty(status) || m.Status == status) &&
            new[] { m.Nome, m.Curso, m.Disciplina, m.Tipo }.Any(c =>
                c.Contains(busca, StringComparison.OrdinalIgnoreCase))).ToList();
    }

    public static string GerarCsv(IEnumerable<Material> materiais)
    {
        // Aspas protegem delimitadores; apóstrofo impede fórmulas ao abrir em planilhas.
        static string Campo(string valor)
        {
            if (valor.TrimStart().StartsWith('=') || valor.TrimStart().StartsWith('+') ||
                valor.TrimStart().StartsWith('-') || valor.TrimStart().StartsWith('@')) valor = "'" + valor;
            return "\"" + valor.Replace("\"", "\"\"") + "\"";
        }
        var csv = new StringBuilder("Id;Nome;Tipo;Curso;Disciplina;Status;Cadastro\r\n");
        foreach (var m in materiais)
            csv.AppendLine(string.Join(";", new[] { m.Id.ToString(), m.Nome, m.Tipo, m.Curso,
                m.Disciplina, m.Status, m.DataCadastro.ToString("yyyy-MM-dd HH:mm:ss") }.Select(Campo)));
        return csv.ToString();
    }

    public static async Task<string> CopiarOrganizadoAsync(Material material, string destino)
    {
        Validar(material);
        static string Pasta(string valor)
        {
            var nome = string.Concat(valor.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c)).Trim().TrimEnd('.');
            if (string.IsNullOrWhiteSpace(nome) || nome is "." or "..") nome = "Sem nome";
            // Prefixo evita nomes reservados do Windows como CON e NUL.
            return "Aula_" + nome;
        }
        var pasta = Path.Combine(Path.GetFullPath(destino), Pasta(material.Curso), Pasta(material.Disciplina));
        Directory.CreateDirectory(pasta);
        var caminho = Path.Combine(pasta, $"{material.Id}_{Path.GetFileName(material.CaminhoArquivo)}");
        // CreateNew recusa sobrescritas. O original permanece intacto.
        using var origem = new FileStream(material.CaminhoArquivo, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, true);
        using var saida = new FileStream(caminho, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, true);
        try { await origem.CopyToAsync(saida); }
        catch { saida.Close(); File.Delete(caminho); throw; }
        return caminho;
    }
}
