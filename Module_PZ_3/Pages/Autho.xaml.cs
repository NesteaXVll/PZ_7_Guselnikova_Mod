using Module_PZ_3.Modules;
using Module_PZ_3.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Module_PZ_3.Pages
{
    /// <summary>
    /// Логика взаимодействия для Autho.xaml
    /// </summary>
    public partial class Autho : Page
    {
        int click;
        DispatcherTimer timer = new DispatcherTimer();
        
        int time = 10;
        public Autho()
        {
            InitializeComponent();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 1);
        }

        private void btnEnterGuests_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Client(null, null));
        }
        private void GenerateCapctcha()
        {
            tbCaptcha.Visibility = Visibility.Visible;
            tblCaptcha.Visibility = Visibility.Visible;

            string capctchaText = CaptchaGeneratot.GenerateCaptchaText(6);
            tblCaptcha.Text = capctchaText;
            tblCaptcha.TextDecorations = TextDecorations.Strikethrough;
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            click += 1;
            string login = tbLogin.Text.Trim();
            string password = tbPassword.Text.Trim();

            var db = Helper.GetContext();

            var user = db.Authorization.Where(x => x.Login == login && x.Password == password).FirstOrDefault();
            if (click == 1)
            {
                if (user != null)
                {
                    MessageBox.Show("Вы вошли под: " + user.Roles.Name.ToString());
                    LoadPage(user.Roles.Name.ToString(), user);
                }
                else
                {
                    MessageBox.Show("Вы ввели логин или пароль неверно!");
                    GenerateCapctcha();
                    tbPassword.Clear();
                }
            }
            else if (click > 1)
            {
                if (user != null && tbCaptcha.Text == tblCaptcha.Text)
                {
                    MessageBox.Show("Вы вошли под: " + user.Roles.Name.ToString());
                    LoadPage(user.Roles.Name.ToString(), user);
                }
                else
                {
                    if (click >= 3)
                    {
                        Blocking();
                        MessageBox.Show($"Количество неудачных попыток: {click}\nПодождите некоторое время, для повторного ввода");
                        timer.Start();
                    }
                    else
                    {
                        MessageBox.Show("Введите данные заново!");
                        GenerateCapctcha();
                        tbPassword.Clear();
                    }
                }
            }
        }
        private void LoadPage(string _role, Authorization authorization)
        {
            click = 0;
            switch (_role)
            {
                case "Администратор":
                    NavigationService.Navigate(new Client(authorization, _role));
                    break;
            }
        }
        private void Blocking()
        {
            btnEnter.IsEnabled = false;
            btnEnterGuests.IsEnabled = false;
            tbLogin.IsEnabled = false;
            tbPassword.IsEnabled = false;
            tbCaptcha.IsEnabled = false;

            txtBlockTimer.Visibility = Visibility.Visible;
            txtBlockTimer.Foreground = Brushes.Red;
        }
        private void Unblocking()
        {
            btnEnter.IsEnabled = true;
            btnEnterGuests.IsEnabled = true;
            tbLogin.IsEnabled = true;
            tbPassword.IsEnabled = true;
            tbCaptcha.IsEnabled = true;

            txtBlockTimer.Visibility = Visibility.Hidden;
        }

        private void timer_Tick(Object sender, EventArgs e) 
        {
            txtBlockTimer.Text = $"Повторите через {time}";
            time--;
            if (time < 0) 
            {
                timer.Stop();
                time = 10;
                Unblocking();
            }
        }
    }
}
