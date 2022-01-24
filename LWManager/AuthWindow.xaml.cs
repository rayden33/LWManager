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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LWManager
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();
            pwdPassBox.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (pwdPassBox.Password == Properties.Settings.Default.Password)
            {

                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Неправильный пароль");
            }
        }

        private void pwdPassBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Button_Click(sender, e);
            }
        }

        private void OpenSplashWindow()
        {
            SplashWindow splashScreen = new SplashWindow();
            splashScreen.Show();
        }
    }
}
