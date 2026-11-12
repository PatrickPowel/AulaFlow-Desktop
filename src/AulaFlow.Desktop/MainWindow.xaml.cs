
using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Windows.Controls;
using Microsoft.Win32;
using AulaFlow.Desktop.Services;
using System.Linq;
using System.Windows;

using AulaFlow.Desktop.Data;
using AulaFlow.Desktop.Models;

using Microsoft.EntityFrameworkCore;

namespace AulaFlow.Desktop;

public partial class MainWindow : Window
{
    private List<Material> _materiais = [];
    private List<Material> _visiveis = [];

    public MainWindow()
    {
        InitializeComponent();

        CarregarMateriais();
    }

    // CONSULTAR MATERIAIS
    private void CarregarMateriais()
    {
        try
        {
            using var banco = new AulaFlowDbContext();

            var materiais = banco.Materiais
                .AsNoTracking()
                .OrderByDescending(m => m.DataCadastro)
                .ThenByDescending(m => m.Id)
                .ToList();

            _materiais = materiais;
            AplicarFiltros();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Erro ao carregar materiais:\n{ex.Message}",
                "Erro",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }

    // CADASTRAR MATERIAL
    private void CadastrarMaterial_Click(
        object sender, RoutedEventArgs e)
    {
        CadastroMaterialWindow cadastro =
            new CadastroMaterialWindow();

        cadastro.Owner = this;

        if (cadastro.ShowDialog() == true)
        {
            CarregarMateriais();
        }
    }

    // EDITAR MATERIAL
    private void EditarMaterial_Click(
        object sender, RoutedEventArgs e)
    {
        if (GridMateriais.SelectedItem is not Material selecionado)
        {
            MessageBox.Show(
                "Selecione um material para editar.",
                "AulaFlow Desktop",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );

            return;
        }

        try
        {
            CadastroMaterialWindow cadastro =
                new CadastroMaterialWindow(selecionado.Id);

            cadastro.Owner = this;

            if (cadastro.ShowDialog() == true)
            {
                CarregarMateriais();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Erro ao abrir o material:\n{ex.Message}",
                "AulaFlow Desktop",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }

    // EXCLUIR MATERIAL
    private void ExcluirMaterial_Click(
        object sender, RoutedEventArgs e)
    {
        if (GridMateriais.SelectedItem is not Material selecionado)
        {
            MessageBox.Show(
                "Selecione um material para excluir.",
                "AulaFlow Desktop",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );

            return;
        }

        var resposta = MessageBox.Show(
            $"Deseja realmente excluir o material?\n\n" +
            $"{selecionado.Nome}",
            "Confirmar exclusão",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question
        );

        if (resposta != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            using var banco = new AulaFlowDbContext();

            var material = banco.Materiais.Find(selecionado.Id);

            if (material == null)
            {
                MessageBox.Show(
                    "Material não encontrado.",
                    "AulaFlow Desktop",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                CarregarMateriais();
                return;
            }

            banco.Materiais.Remove(material);
            banco.SaveChanges();

            MessageBox.Show(
                "Material excluído com sucesso!",
                "AulaFlow Desktop",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            CarregarMateriais();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Erro ao excluir material:\n{ex.Message}",
                "Erro",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }
    }

    // ATUALIZAR A LISTAGEM
    private void AtualizarLista_Click(
        object sender, RoutedEventArgs e)
    {
        CarregarMateriais();
    }

    private void Filtros_Changed(object sender, RoutedEventArgs e)
    {
        if (IsInitialized && TxtResultado != null) AplicarFiltros();
    }
    private void AplicarFiltros()
    {
        var status = FiltroStatus.SelectedIndex <= 0 ? "" : (FiltroStatus.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
        _visiveis = MaterialTools.Filtrar(_materiais, TxtBusca.Text, status);
        GridMateriais.ItemsSource = _visiveis;
        TxtResumo.Text = $"{_materiais.Count} materiais  •  {_materiais.Count(m => m.Status == "Pendente")} pendentes  •  {_materiais.Count(m => m.Status == "Concluído")} concluídos";
        TxtResultado.Text = _visiveis.Count == 0 ? "Nenhum material encontrado. Cadastre um material ou ajuste os filtros." : $"{_visiveis.Count} materiais exibidos";
    }
    private void AbrirArquivo_Click(object sender, RoutedEventArgs e)
    {
        if (GridMateriais.SelectedItem is not Material material) { MessageBox.Show(this, "Selecione um material."); return; }
        try
        {
            if (!File.Exists(material.CaminhoArquivo)) throw new FileNotFoundException("Arquivo movido ou removido. Edite o cadastro para atualizar o caminho.");
            Process.Start(new ProcessStartInfo(material.CaminhoArquivo) { UseShellExecute = true });
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Não foi possível abrir"); }
    }
    private async void Organizar_Click(object sender, RoutedEventArgs e)
    {
        if (GridMateriais.SelectedItem is not Material material) { MessageBox.Show(this, "Selecione um material."); return; }
        var dialog = new OpenFolderDialog { Title = "Pasta de destino da cópia" };
        if (dialog.ShowDialog(this) != true) return;
        BtnOrganizar.IsEnabled = false;
        try
        {
            var caminho = await MaterialTools.CopiarOrganizadoAsync(material, dialog.FolderName);
            MessageBox.Show(this, $"Cópia criada em:\n{caminho}\n\nO cadastro continua apontando para o original.", "Organização concluída");
        }
        catch (Exception ex) { MessageBox.Show(this, $"Não foi possível copiar. Arquivos existentes não são substituídos.\n{ex.Message}", "Organização"); }
        finally { BtnOrganizar.IsEnabled = true; }
    }
    private void Exportar_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog { Filter = "CSV|*.csv", FileName = "aulaflow-materiais.csv" };
        if (dialog.ShowDialog(this) != true) return;
        try
        {
            File.WriteAllText(dialog.FileName, MaterialTools.GerarCsv(_visiveis), new UTF8Encoding(true));
            MessageBox.Show(this, $"{_visiveis.Count} materiais exportados. Os filtros atuais foram respeitados.", "Exportação concluída");
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "Erro na exportação"); }
    }
}
