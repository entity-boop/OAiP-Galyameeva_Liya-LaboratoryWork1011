using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace laba10
{
    public partial class adminInfo : Window
    {
        private Photographer _selectedUser;

        public adminInfo(int adminId)
        {
            InitializeComponent();
            LoadUsers(); 
        }

        private void LoadUsers()
        {
            var context = PhotographerContext.GetContext();
            var users = context.Photographers.ToList();
            UsersDataGrid.ItemsSource = users;
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
            ClearFields();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields(isUpdate: false)) return;

            var context = PhotographerContext.GetContext();

            try
            {
                var newUser = new Photographer
                {
                    FIO = TxtFIO.Text.Trim(),
                    Email = TxtEmail.Text.Trim().ToLower(),
                    Phone = TxtPhone.Text.Trim(),
                    Genre = (CbGenre.SelectedItem as ComboBoxItem)?.Content.ToString() ?? string.Empty,
                    DateOfFirstPost = DateOnly.FromDateTime(DateTime.Today),
                    Password = Password.HashingPassword(AddPassword.Password),
                    Role = "User"
                };

                context.Photographers.Add(newUser);
                context.SaveChanges();

                MessageBox.Show("Пользователь успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                ClearFields();
                LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser == null)
            {
                MessageBox.Show("Выберите строку в таблице для обновления!", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidateFields(isUpdate: true)) return;

            var context = PhotographerContext.GetContext();

            try
            {
                var userToUpdate = context.Photographers.FirstOrDefault(p => p.Id == _selectedUser.Id);

                if (userToUpdate == null)
                {
                    MessageBox.Show("Пользователь не найден!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                userToUpdate.FIO = TxtFIO.Text.Trim();
                userToUpdate.Email = TxtEmail.Text.Trim();
                userToUpdate.Phone = TxtPhone.Text.Trim();
                userToUpdate.Genre = (CbGenre.SelectedItem as ComboBoxItem)?.Content.ToString() ?? string.Empty;
                userToUpdate.Role = (CbRole.SelectedItem as ComboBoxItem)?.Content.ToString() ?? string.Empty;

                context.SaveChanges();

                MessageBox.Show("Данные пользователя обновлены!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                ClearFields();
                LoadUsers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser == null)
            {
                MessageBox.Show("Выберите строку в таблице для удаления!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Удалить пользователя {_selectedUser.FIO}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var context = PhotographerContext.GetContext();

                try
                {
                    context.Photographers.Remove(_selectedUser);
                    context.SaveChanges();

                    MessageBox.Show("Пользователь удалён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    ClearFields();
                    LoadUsers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UsersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is Photographer selected)
            {
                _selectedUser = selected;
                TxtFIO.Text = selected.FIO;
                TxtEmail.Text = selected.Email;
                TxtPhone.Text = selected.Phone;

                var genreItem = CbGenre.Items.Cast<ComboBoxItem>()
                    .FirstOrDefault(item => (item.Content as string) == selected.Genre);
                if (genreItem != null) CbGenre.SelectedItem = genreItem;
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            new Auth().Show();
            this.Close();
        }

        private void ClearFields()
        {
            TxtFIO.Clear();
            TxtEmail.Clear();
            TxtPhone.Clear();
            CbGenre.SelectedItem = null;
            _selectedUser = null;
            UsersDataGrid.SelectedItem = null;
        }

        private bool ValidateFields(bool isUpdate)
        {
            //фио
            if (string.IsNullOrWhiteSpace(TxtFIO.Text)) { MessageBox.Show("Введите ФИО!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); TxtFIO.Focus(); return false; }
            if (!Regex.IsMatch(TxtFIO.Text.Trim(), @"^[a-zA-Zа-яА-ЯёЁ\s\-]+$")) { MessageBox.Show("ФИО: только буквы, пробелы и дефисы!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); TxtFIO.Focus(); return false; }
            //email
            if (string.IsNullOrWhiteSpace(TxtEmail.Text)) { MessageBox.Show("Введите Email!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); TxtEmail.Focus(); return false; }
            if (!Regex.IsMatch(TxtEmail.Text.Trim(), @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")) { MessageBox.Show("Некорректный Email!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); TxtEmail.Focus(); return false; }
            //телфон
            if (string.IsNullOrWhiteSpace(TxtPhone.Text)) { MessageBox.Show("Введите телефон!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); TxtPhone.Focus(); return false; }
            if (!Regex.IsMatch(TxtPhone.Text.Trim(), @"^\+7\d{10}$")) { MessageBox.Show("Телефон: формат +7XXXXXXXXXX", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); TxtPhone.Focus(); return false; }
            //направление
            if (CbGenre.SelectedItem == null) { MessageBox.Show("Выберите направление!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); CbGenre.Focus(); return false; }
            return true;
        }
    }
}