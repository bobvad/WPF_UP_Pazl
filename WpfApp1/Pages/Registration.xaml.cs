using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Newtonsoft.Json;

namespace WpfApp1.Pages
{
    public partial class Registration : Page
    {
        private const string ApiBaseUrl = "https://localhost:7294";
        private static readonly HttpClient _httpClient = new HttpClient();

        public Registration()
        {
            InitializeComponent();
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginBox.Text))
            {
                MessageBox.Show("Введите логин", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                LoginBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                MessageBox.Show("Введите пароль", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                PasswordBox.Focus();
                return;
            }
            try
            {
                RegisterButton.IsEnabled = false;
                RegisterButton.Content = "Регистрация...";

                var formData = new Dictionary<string, string>
                {
                    {"Login", LoginBox.Text},
                    {"Password", PasswordBox.Password}
                };

                var response = await _httpClient.PostAsync(
                    $"{ApiBaseUrl}/api/UsersControls/Reg",
                    new FormUrlEncodedContent(formData)
                );

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var user = JsonConvert.DeserializeObject<dynamic>(responseContent);

                    MessageBox.Show($"Пользователь {LoginBox.Text} успешно зарегистрирован!",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    NavigationService?.Navigate(new Authtorization());
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        MessageBox.Show("Логин и пароль не могут быть пустыми",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        if (responseContent == "500")
                        {
                            MessageBox.Show("Пользователь с таким логином уже существует",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            MessageBox.Show("Ошибка сервера при регистрации",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Ошибка: {response.StatusCode}",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("Не удалось подключиться к серверу\n\nПроверьте:\n" +
                    "1. Запущен ли API проект\n" +
                    "2. Правильный ли порт: 7294\n" +
                    "3. Адрес: https://localhost:7294",
                    "Ошибка подключения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoginText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.init.frame.Navigate(new Pages.Authtorization());
        }
    }
}