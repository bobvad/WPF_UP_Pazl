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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1.Pages.TaskUsers
{
    /// <summary>
    /// Логика взаимодействия для GlavnaiTasks.xaml
    /// </summary>
    public partial class GlavnaiTasks : Page
    {
        public GlavnaiTasks()
        {
            InitializeComponent();
        }

        private void CreateTaskButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Pages.TaskUsers.TaskWindow());
        }

        private void ExecuteTaskButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
