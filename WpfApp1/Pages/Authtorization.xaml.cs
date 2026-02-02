using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace WpfApp1.Pages
{
    public partial class Authtorization : Page
    {
        private const string ApiBaseUrl = "https://localhost:7294";
        private static readonly HttpClient _httpClient = new HttpClient();

        public Authtorization()
        {
            InitializeComponent();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                MessageBox.Show("Заполните все поля", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                LoginButton.IsEnabled = false;
                LoginButton.Content = "Вход...";

                var formData = new Dictionary<string, string>
                {
                    {"Login", LoginBox.Text},
                    {"Password", PasswordBox.Password}
                };

                var response = await _httpClient.PostAsync(
                    $"{ApiBaseUrl}/api/UsersControls/Auth",
                    new FormUrlEncodedContent(formData)
                );

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<dynamic>(json);

                    MessageBox.Show($"Успешный вход! Привет, {user.Login}",
                        "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                    MainWindow.init.frame.Navigate(new Pages.TaskUsers.GlavnaiTasks());

                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    MessageBox.Show("Неверный логин или пароль",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    MessageBox.Show("Ошибка сервера. Попробуйте позже",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show($"Ошибка: {response.StatusCode}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("Не удалось подключиться к серверу.\n\n" +
                    "Проверьте:\n" +
                    "1. Запущен ли API проект\n" +
                    "2. Правильный ли порт (7294)\n" +
                    "3. Адрес: https://localhost:7294",
                    "Ошибка подключения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LoginButton.IsEnabled = true;
                LoginButton.Content = "Войти";
            }
        }

        private void Register_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Registration());
        }
    }
}