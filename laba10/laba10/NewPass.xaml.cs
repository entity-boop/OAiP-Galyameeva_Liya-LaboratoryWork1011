using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace laba10
{
    public partial class NewPass : Window
    {
        private string _generatedCode;
        private string _userEmail;

        public NewPass()
        {
            InitializeComponent();
            NewPassword.IsEnabled = false;
            TxtCode.IsEnabled = false;
        }

        private void BtnSendCode_Click(object sender, RoutedEventArgs e)
        {
            // email
            if (string.IsNullOrWhiteSpace(TxtEmail3.Text))
            {
                MessageBox.Show("Введите Email!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtEmail3.Focus();
                return;
            }

            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(TxtEmail3.Text.Trim(), emailPattern))
            {
                MessageBox.Show("Введите корректный Email!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtEmail3.Focus();
                TxtEmail3.SelectAll();
                return;
            }

            _userEmail = TxtEmail3.Text.Trim();

            if (SendPasswordResetCode(_userEmail, out string code))
            {
                _generatedCode = code;

                MessageBox.Show($"Код подтверждения отправлен на {_userEmail}",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                TxtCode.IsEnabled = true;
                TxtCode.Focus();
                TxtCode.Clear();
            }
            else
            {
                MessageBox.Show("Не удалось отправить код.","Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnVerifyCode_Click(object sender, RoutedEventArgs e)
        {
            //код
            if (string.IsNullOrWhiteSpace(TxtCode.Text))
            {
                MessageBox.Show("Введите код из письма!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtCode.Focus();
                return;
            }

            if (TxtCode.Text.Trim() == _generatedCode)
            {
                MessageBox.Show("Код подтверждён! Введите новый пароль.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                NewPassword.IsEnabled = true;
                TxtCode.IsEnabled = false;
                NewPassword.Focus();
            }
            else
            {
                MessageBox.Show("Неверный код! Попробуйте ещё раз.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                TxtCode.Clear();
                TxtCode.Focus();
            }
        }

        private void BtnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            if (!NewPassword.IsEnabled)
            {
                MessageBox.Show("Сначала подтвердите код!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // пароль
            if (string.IsNullOrWhiteSpace(NewPassword.Password))
            {
                MessageBox.Show("Введите новый пароль!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                NewPassword.Focus();
                return;
            }

            if (NewPassword.Password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                NewPassword.Focus();
                return;
            }

            var context = PhotographerContext.GetContext();
            var user = context.Photographers.FirstOrDefault(p => p.Email == _userEmail);

            if (user == null)
            {
                MessageBox.Show("Пользователь не найден!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                user.Password = Password.HashingPassword(NewPassword.Password);
                context.SaveChanges();

                MessageBox.Show("Пароль успешно изменён!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                new Auth().Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBackToLogin_Click(object sender, RoutedEventArgs e)
        {
            new Auth().Show();
            this.Close();
        }

        public static bool SendPasswordResetCode(string toEmail, out string code)
        {
            code = new Random().Next(100000, 999999).ToString();

            try
            {
                using (var client = new SmtpClient("smtp.mail.ru", 587))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential("lgalyameeva@mail.ru", "PfzIQgYRpLktZxQxhWoj");

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("lgalyameeva@mail.ru", "Защита Л/Р"),
                        Subject = "Код подтверждения для смены пароля",
                        Body = $"<h1>Ваш код верификации: <b>{code}</b></h1><p>Используйте его для восстановления доступа.</p>",
                        IsBodyHtml = true,
                        BodyEncoding = Encoding.UTF8
                    };
                    mailMessage.To.Add(toEmail);

                    client.Send(mailMessage);
                }
                return true; 
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ SMTP Error: {ex.Message}");
                code = null;
                return false;
            }
        }
    }
}