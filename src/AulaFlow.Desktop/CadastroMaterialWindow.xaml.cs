
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

using AulaFlow.Desktop.Data;
using AulaFlow.Desktop.Services;
using AulaFlow.Desktop.Models;

using Microsoft.Win32;

namespace AulaFlow.Desktop;

public partial class CadastroMaterialWindow : Window
{
    private readonly int? _materialId;

    // CONSTRUTOR PARA CADASTRO
    public CadastroMaterialWindow()
    {
        InitializeComponent();

        _materialId = null;
    }

    // CONSTRUTOR PARA EDIÇÃO
    public CadastroMaterialWindow(int materialId) : this()
    {
        _materialId = materialId;

        using var banco = new AulaFlowDbContext();

        var material = banco.Materiais.Find(materialId);

        if (material == null)
        {
            throw new InvalidOperationException(
                "Material não encontrado."
            );
        }

        // Preencher os campos
        TxtNome.Text = material.Nome;
        TxtCurso.Text = material.Curso;
        TxtDisciplina.Text = material.Disciplina;
        TxtArquivo.Text = material.CaminhoArquivo;

        SelecionarCombo(CmbTipo, material.Tipo);
        SelecionarCombo(CmbStatus, material.Status);

        // Atualizar os títulos
        Title = "Editar material - AulaFlow";
        TituloFormulario.Text = "Editar material";
        BtnSalvar.Content = "Salvar alterações";
    }

    // SELECIONAR ITEM DO COMBOBOX
    private void SelecionarCombo(
        ComboBox combo, string valor)
    {
        foreach (ComboBoxItem item in combo.Items)
        {
            if (item.Content?.ToString() == valor)
            {
                combo.SelectedItem = item;
                break;
            }
        }
    }

    // PROCURAR ARQUIVO
    private void ProcurarArquivo_Click(
        object sender, RoutedEventArgs e)
    {
        OpenFileDialog dialog = new OpenFileDialog
        {
            Title = "Selecionar material audiovisual",
            Filter =
                "Arquivos de vídeo|*.mp4;*.mov;*.avi;*.mkv|" +
                "Arquivos de áudio|*.mp3;*.wav;*.aac|" +
                "Documentos|*.pdf;*.docx;*.pptx;*.txt|" +
                "Todos os arquivos|*.*"
        };

        if (dialog.ShowDialog(this) == true)
        {
            TxtArquivo.Text = dialog.FileName;
        }
    }

    // SALVAR MATERIAL
    private void SalvarMaterial_Click(
        object sender, RoutedEventArgs e)
    {
        string nome = TxtNome.Text.Trim();
        string curso = TxtCurso.Text.Trim();
        string disciplina = TxtDisciplina.Text.Trim();
        string arquivo = TxtArquivo.Text.Trim();

        string tipo = (CmbTipo.SelectedItem as ComboBoxItem)
            ?.Content?.ToString() ?? "";

        string status = (CmbStatus.SelectedItem as ComboBoxItem)
            ?.Content?.ToString() ?? "Pendente";

        // VALIDAR CAMPOS
        if (string.IsNullOrWhiteSpace(nome) ||
            string.IsNullOrWhiteSpace(curso) ||
            string.IsNullOrWhiteSpace(disciplina) ||
            string.IsNullOrWhiteSpace(arquivo) ||
            string.IsNullOrWhiteSpace(tipo) ||
            tipo == "Selecione")
        {
            MessageBox.Show(
                "Preencha todos os campos obrigatórios.",
                "Validação",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );

            return;
        }

        // VALIDAR ARQUIVO
        if (!File.Exists(arquivo))
        {
            MessageBox.Show(
                "O arquivo informado não existe.\n" +
                "Selecione um arquivo válido.",
                "Validação",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );

            return;
        }

        try
        {
            using var banco = new AulaFlowDbContext();

            Material material;

            if (_materialId == null)
            {
                // CREATE
                material = new Material
                {
                    Nome = nome,
                    Tipo = tipo,
                    Curso = curso,
                    Disciplina = disciplina,
                    CaminhoArquivo = arquivo,
                    Status = status,
                    DataCadastro = DateTime.Now
                };

                banco.Materiais.Add(material);
            }
            else
            {
                // UPDATE
                material = banco.Materiais.Find(
                    _materialId.Value
                ) ?? throw new InvalidOperationException(
                    "Material não encontrado."
                );

                material.Nome = nome;
                material.Tipo = tipo;
                material.Curso = curso;
                material.Disciplina = disciplina;
                material.CaminhoArquivo = arquivo;
                material.Status = status;
            }

            MaterialTools.Validar(material);

            // SALVAR NO SQLITE
            banco.SaveChanges();

            string mensagem = _materialId == null
                ? "Material cadastrado com sucesso!"
                : "Material atualizado com sucesso!";

            MessageBox.Show(
                $"{mensagem}\n\n" +
                $"ID: {material.Id}\n" +
                $"Nome: {material.Nome}\n" +
                $"Status: {material.Status}",
                "AulaFlow Desktop",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Erro ao salvar material:\n{ex.Message}",
                "Erro no banco de dados",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }

    // CANCELAR
    private void Cancelar_Click(
        object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}