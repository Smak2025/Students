using System.Configuration;
using System.Data;
using System.Windows;

namespace Students
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                await DbHelper.CreateDbStructureAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось подключиться к БД:\n{ex.Message}",
                                "Ошибка инициализации",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(1); // Закрываем приложение с кодом ошибки
                return;
            }
        }
    }

}
