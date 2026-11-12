
using System;

namespace AulaFlow.Desktop.Models;

public class Material
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Tipo { get; set; } = string.Empty;

    public string Curso { get; set; } = string.Empty;

    public string Disciplina { get; set; } = string.Empty;

    public string CaminhoArquivo { get; set; } = string.Empty;

    public string Status { get; set; } = "Pendente";

    public DateTime DataCadastro { get; set; } = DateTime.Now;
}