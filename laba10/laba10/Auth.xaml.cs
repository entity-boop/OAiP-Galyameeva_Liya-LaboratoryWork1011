using System.Windows;

namespace laba10
{
    /// <summary>
    /// Логика взаимодействия для Auth.xaml
    /// </summary>
    public partial class Auth : Window
    {
        public Auth()
        {
            InitializeComponent();
        }

        private void BtnIn_Click(object sender, RoutedEventArgs e)
        {
            // email
            if (string.IsNullOrWhiteSpace(TxtEmail2.Text))
            {
                MessageBox.Show("Введите Email!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtEmail2.Focus();
                return;
            }

            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(TxtEmail2.Text.Trim(), emailPattern))
            {
                MessageBox.Show("Введите корректный Email!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtEmail2.Focus();
                return;
            }

            // пароль
            if (string.IsNullOrWhiteSpace(Password2.Password))
            {
                MessageBox.Show("Введите пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                Password2.Focus();
                return;
            }

            if (Password2.Password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                Password2.Focus();
                return;
            }


            string email = TxtEmail2.Text.Trim().ToLower();
            var photographer = PhotographerContext.GetContext().Photographers.FirstOrDefault(p => p.Email.ToLower() == email);


            bool isAdminChecked = ChkIsAdmin.IsChecked == true;
            string expectedRole = isAdminChecked ? "Admin" : "User";
            if (photographer != null && Password.VerifyPassword(Password2.Password, photographer.Password))
            {
                MessageBox.Show("Успешная авторизация!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Information);

                if (isAdminChecked)
                    new adminInfo(photographer.Id).Show();
                else
                    new userInfo(photographer.Id).Show();

                this.Close();
                return;
            }
            else
            {
                MessageBox.Show("Неверный email или пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Password2.Focus();
                return;
            }


        }
        private void BtnGoToReg_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }

        private void BtnSetNewPas_Click(object sender, RoutedEventArgs e)
        {
            new NewPass().Show();
            this.Close();
        }
    }
}
