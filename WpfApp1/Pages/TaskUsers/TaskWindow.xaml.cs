using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
    /// Логика взаимодействия для TaskWindow.xaml
    /// </summary>
    public partial class TaskWindow : Window
    {
        public TaskWindow()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Pages.TaskUsers.GlavnaiTasks());
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string apiBaseUrl = "https://localhost:7031/api/task"; 

                string title = "Моя новая задача";
                string description = "Описание задачи";
                int puzzleId = 1;
                int pieceNumber = 100;

                bool result = await AddTask(apiBaseUrl, title, description, puzzleId, pieceNumber);

                if (result)
                {
                    MessageBox.Show("Задача успешно добавлена!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении задачи: {ex.Message}");
            }
        }

        private async Task<bool> AddTask(string apiUrl, string title, string description, int puzzleId, int pieceNumber)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var formData = new MultipartFormDataContent();
                    formData.Add(new StringContent(title), "Title");
                    formData.Add(new StringContent(description), "Description");
                    formData.Add(new StringContent(puzzleId.ToString()), "PuzzleId");
                    formData.Add(new StringContent(pieceNumber.ToString()), "PieceNumber");

                    HttpResponseMessage response = await client.PostAsync($"{apiUrl}/AddTask", formData);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Ответ от сервера: {responseBody}");
                        return true;
                    }
                    else
                    {
                        string errorBody = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Ошибка сервера: {response.StatusCode}\n{errorBody}");
                        return false;
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"Ошибка HTTP запроса: {httpEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Общая ошибка: {ex.Message}");
                return false;
            }
        }
    }
}
