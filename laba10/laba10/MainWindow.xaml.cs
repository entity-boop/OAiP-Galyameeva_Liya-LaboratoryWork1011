using System.Text;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Konscious.Security.Cryptography;

namespace laba10
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        // k0220265@gmail.com
        private void BtnRegister_Click(object sender, RoutedEventArgs e)
            {
                // фио
                if (string.IsNullOrWhiteSpace(TxtFIO.Text))
                {
                    MessageBox.Show("Введите ФИО!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TxtFIO.Focus();
                    return;
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(TxtFIO.Text.Trim(), @"^[a-zA-Zа-яА-ЯёЁ\s\-]+$"))
                {
                    MessageBox.Show("ФИО должно содержать только буквы, пробелы и дефисы!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtFIO.Focus();
                return;
            }

            // email
            if (string.IsNullOrWhiteSpace(TxtEmail.Text))
            {
                MessageBox.Show("Введите Email!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtEmail.Focus();
                return;
            }

            string emailValid = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(TxtEmail.Text.Trim(), emailValid))
            {
                MessageBox.Show("Введите корректный Email!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtEmail.Focus();
                return;
            }

            // телефон
            if (string.IsNullOrWhiteSpace(TxtPhone.Text))
            {
                MessageBox.Show("Введите номер телефона!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtPhone.Focus();
                return;
            }

            string phoneValid = @"^\+7\d{10}$";
            string phoneClean = TxtPhone.Text.Trim();
            if (!System.Text.RegularExpressions.Regex.IsMatch(phoneClean, phoneValid))
            {
                MessageBox.Show("Телефон должен начинаться с +7 и содержать ровно 10 цифр после него (формат: +79991234567)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtPhone.Focus();
                return;
            }

            // направление
            if (CbDirection.SelectedItem == null)
            {
                MessageBox.Show("Выберите направление фотографии!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                CbDirection.Focus();
                return;
            }

            // дата
            if (!DpFirstPost.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите дату первой публикации!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                DpFirstPost.Focus();
                return;
            }

            if (DpFirstPost.SelectedDate.Value.Date > DateTime.Today)
            {
                MessageBox.Show("Дата публикации не может быть в будущем!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                DpFirstPost.Focus();
                return;
            }

            // пароль
            if (string.IsNullOrWhiteSpace(PbPassword.Password))
            {
                MessageBox.Show("Введите пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                PbPassword.Focus();
                return;
            }

            if (PbPassword.Password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                PbPassword.Focus();
                return;
            }


            var photographer = new Photographer()
            {
                FIO = TxtFIO.Text.Trim(),
                Email = TxtEmail.Text.Trim(),
                Phone = phoneClean,
                Genre = (CbDirection.SelectedItem as ComboBoxItem)?.Content.ToString() ?? string.Empty,
                DateOfFirstPost = DateOnly.FromDateTime(DpFirstPost.SelectedDate.Value),
                Password = Password.HashingPassword(PbPassword.Password),
                Role = IsAdmin.IsChecked == true ? "Admin" : "User"
            }; 

            try
            {
                var context = PhotographerContext.GetContext();
                context.Photographers.Add(photographer);
                context.SaveChanges();

                MessageBox.Show("Регистрация успешно завершена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                Window nextWindow;

                if (photographer.Role == "Admin")
                {
                    nextWindow = new adminInfo(photographer.Id);
                }
                else
                {
                    nextWindow = new userInfo(photographer.Id);
                }

                nextWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnGoToAuth_Click(object sender, RoutedEventArgs e)
        {
            new Auth().Show();
            this.Close();
        }

    }
}