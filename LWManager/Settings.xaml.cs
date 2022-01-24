using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();

            debtLimitTxtBox.Text = Properties.Settings.Default.DebtLimit.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (currentPasswordTxtBox.Text.Length == 0 || newPasswordTxtBox.Text.Length == 0)
            {
                MessageBox.Show("Заполните все поля!!!");
                return;
            }    
            if (currentPasswordTxtBox.Text == Properties.Settings.Default.Password)
            {
                Properties.Settings.Default.Password = newPasswordTxtBox.Text;
                Properties.Settings.Default.Save();
                MessageBox.Show("Успешно сохранен!!!");
            }
            else
            {
                MessageBox.Show("Неправильный пароль");
                return;
            }
            DialogResult = true;
        }

        private void ExportBtn(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "SQLite (*.db)|*.db";
            if (saveFileDialog.ShowDialog() == true)
                File.Copy(".\\LesaDataBase.db", saveFileDialog.FileName, true);
        }

        private void ImportBtn(object sender, RoutedEventArgs e)
        {
            string dir = @".\\OldDB";
            // If directory does not exist, create it
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.Copy(".\\LesaDataBase.db", dir + "\\LesaDataBase.db_" + DateTime.Now.ToString("yyMMddHHmmss"));

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "SQLite (*.db)|*.db";
            if (openFileDialog.ShowDialog() == true)
            {
                File.Copy(openFileDialog.FileName, ".\\LesaDataBase.db", true);
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
                

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(debtLimitTxtBox.Text))
            {
                MessageBox.Show("Заполните поля!!!");
                return;
            }
            if (debtLimitTxtBox.Text[0] != '-')
            {
                Properties.Settings.Default.DebtLimit = Convert.ToInt32(debtLimitTxtBox.Text);
                Properties.Settings.Default.Save();
                MessageBox.Show("Успешно сохранен!!!");
            }
            else
            {
                MessageBox.Show("Долговой лимит не может быть отрицательным");
                return;
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {

            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);

        }
    }
}
