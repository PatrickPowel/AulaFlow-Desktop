
using System.Windows;
using AulaFlow.Desktop.Data;

namespace AulaFlow.Desktop;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            using var banco = new AulaFlowDbContext();
            banco.Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Não foi possível inicializar o banco local. Verifique o acesso à pasta de dados.\n{ex.Message}",
                "AulaFlow", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
        }
    }
}
