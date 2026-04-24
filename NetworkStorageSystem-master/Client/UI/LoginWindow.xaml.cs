using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Collections.Specialized.BitVector32;

namespace Client.UI
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly NetworkClient network = new NetworkClient();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var result = network.Login(UsernameBox.Text, PasswordBox.Password);

            if (result == null)
            {
                MessageBox.Show("Invalid login");
                return;
            }

            Session.CurrentUser = result;

            new MainWindow().Show();
            this.Close();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            bool ok = network.Register(UsernameBox.Text, PasswordBox.Password);

            MessageBox.Show(ok ? "Registered" : "User exists");
        }
    }
}
