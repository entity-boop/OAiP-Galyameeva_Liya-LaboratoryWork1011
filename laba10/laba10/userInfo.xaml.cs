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

namespace laba10
{
    /// <summary>
    /// Логика взаимодействия для userInfo.xaml
    /// </summary>
    public partial class userInfo : Window
    {
        public userInfo(int userId)
        {
            InitializeComponent();
            LoadUserData(userId);
        }

        private void LoadUserData(int userId)
        {
            var context = PhotographerContext.GetContext();
            var user = context.Photographers.FirstOrDefault(p => p.Id == userId);

            if (user == null)
            {
                MessageBox.Show("Не удалось загрузить данные пользователя.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            TbFIO.Text = user.FIO;
            TbEmail.Text = user.Email;
            TbPhone.Text = user.Phone;
            TbGenre.Text = user.Genre;
            TbDate.Text = user.DateOfFirstPost.ToString("dd.MM.yyyy");
            TbRole.Text = user.Role == "Admin" ? "Администратор" : "Пользователь";
        }
    }
}
