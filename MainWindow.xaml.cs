using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfRefactorFramework.Helpers;
using WpfRefactorFramework.Services;

namespace WpfRefactorFramework
{
    public partial class MainWindow
    {
        private readonly KeyHelper _keyHelper = new KeyHelper();

        public MainWindow()
        {
            InitializeComponent();
            
            _keyHelper.KeyDown += KeysDows;
        }
        //Преобразование base64 в BitmapImage
        private static BitmapImage LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        // Для RFId карт через USB считыватель smartec
        private async void KeysDows(object sender, KeyEventArgs e)
        {
            if (e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9) return;
            // Преобразовывем массив в нормальный вид
            TextCode.AppendText(char.ToString((char)(e.KeyCode - Keys.D0 + '0')));

            if (TextCode.Text.Length == 8)
            {
                var person = await PersonsService.GetPerson(TextCode.Text);
                // Если найден пропуск
                if (person?.Id != 0)
                {
                    // Информация
                    FirstName.Text = person.FirstName;
                    LastName.Text = person.LastName;
                    SecondName.Text = person.SecondName;
                    Section.Text = person.Section;
                    Building.Text = person.Building;
                    Room.Text = person.Room;
                    Avatar.Source = LoadImage(person.Avatar);

                    // Изменяем статус персоны
                    var moveStatus = person.StatusPropusk == "F" ? "T" : "F";
                    // изменить статус
                    await PersonsService.UpdateStatus(moveStatus, person.Id);
                    // записать историю
                    await PersonsService.InsertMove(person.IdPropusk, person.StatusPropusk == "F" ? "Зашел" : "Вышел", DateTime.Now);
                    // Обновляем данные истории
                    DataMove.ItemsSource = await PersonsService.GetMove();
                    // Стираем код
                    TextCode.Text = string.Empty;

                    if (person.StatusPropusk == "F")
                    {
                        BorderStatus.Background = new SolidColorBrush(Colors.Green);
                        StatusPerson.Text = "Зашел";
                    }
                    else
                    {
                        BorderStatus.Background = new SolidColorBrush(Colors.Orange);
                        StatusPerson.Text = "Вышел";
                    }
                    TextCountAll.Text = $"Всего: {await PersonsService.GetCountAllPersons()}";
                    TextCountInner.Text = $"Из них находятся в общежитии : {await PersonsService.GetInnerPersons()}";
                    TextCountOther.Text = $"Из них не находятся в общежитии : {await PersonsService.GetOutherPersons()}";
                }
                else
                {
                    // Стираем код
                    TextCode.Text = string.Empty;

                    BorderStatus.Background = new SolidColorBrush(Colors.Red);
                    StatusPerson.Text = "Пропуск не найден";

                    await Task.Delay(3000);

                    BorderStatus.Background = new SolidColorBrush(Colors.Transparent);
                    StatusPerson.Text = string.Empty;
                }
            }
        }

        private async void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            DataMove.ItemsSource = await PersonsService.GetMove();
            TextCountAll.Text = $"Всего: {await PersonsService.GetCountAllPersons()}";
            TextCountInner.Text = $"Из них находятся в общежитии : {await PersonsService.GetInnerPersons()}";
            TextCountOther.Text = $"Из них не находятся в общежитии : {await PersonsService.GetOutherPersons()}";
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            new ListPersons().Show();
        }
    }
}