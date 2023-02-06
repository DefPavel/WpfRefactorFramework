using System.Linq;
using System.Windows;
using WpfRefactorFramework.Services;

namespace WpfRefactorFramework
{
    public partial class ListPersons
    {
        public ListPersons()
        {
            InitializeComponent();
        }
        private async void ListPersons_OnLoaded(object sender, RoutedEventArgs e)
        {
            var items = await PersonsService.GetPersonList();
            PersonList.ItemsSource = items.OrderBy(x => x.Section).ThenBy(x => x.Room);
        }
    }
}