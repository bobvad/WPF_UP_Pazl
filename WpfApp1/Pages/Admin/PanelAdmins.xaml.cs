using Microsoft.Win32;
using System;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfApp1.Pages.Admin
{
    public partial class PanelAdmins : Page
    {
        private string _selectedImagePath = string.Empty;

        public PanelAdmins()
        {
            InitializeComponent();
        }

        private void TxtName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TxtName.Text == "Название пазла")
                TxtName.Text = "";
        }

        private void TxtName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
                TxtName.Text = "Название пазла";
        }

        private void NumberValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(text, @"^[0-9]+$");
        }

        private void BtnBrowseImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png|All files (*.*)|*.*"
            };

            if (dlg.ShowDialog() == true)
            {
                _selectedImagePath = dlg.FileName;
                TxtImageUrl.Text = _selectedImagePath;
                LoadImagePreview(_selectedImagePath);
            }
        }

        private void TxtImageUrl_TextChanged(object sender, TextChangedEventArgs e)
        {
            var url = TxtImageUrl.Text.Trim();
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                LoadImagePreview(url);
            }
            else if (File.Exists(url))
            {
                LoadImagePreview(url);
            }
            else
            {
                ImgPreview.Source = null;
            }

            UpdateAddButtonState();
        }

        private void LoadImagePreview(string source)
        {
            try
            {
                if (Uri.IsWellFormedUriString(source, UriKind.Absolute))
                {
                    ImgPreview.Source = new BitmapImage(new Uri(source));
                }
                else
                {
                    ImgPreview.Source = new BitmapImage(new Uri(source));
                }
            }
            catch
            {
                ImgPreview.Source = null;
            }
        }

        private void UpdateAddButtonState()
        {
            bool validName = !string.IsNullOrWhiteSpace(TxtName.Text) && TxtName.Text != "Название пазла";
            bool validImage = ImgPreview.Source != null;

            BtnAdd.IsEnabled = validName && validImage;
        }

        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = TxtName.Text.Trim();
                string imageUrl = TxtImageUrl.Text.Trim();

                if (string.IsNullOrWhiteSpace(name) || name == "Название пазла")
                {
                    MessageBox.Show("Укажите название пазла.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(imageUrl))
                {
                    MessageBox.Show("Укажите URL или выберите изображение.");
                    return;
                }

                using (var client = new HttpClient())
                {
                    var formData = new MultipartFormDataContent
                    {
                       { new StringContent(name), "Name" },
                       { new StringContent(imageUrl), "FullImage" }
                    };

                    string apiUrl = "https://localhost:7294/api/Puzzles/";

                    var response = await client.PostAsync(apiUrl, formData);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        MessageBox.Show("Пазл успешно добавлен!");

                        TxtName.Text = "";
                        TxtImageUrl.Text = "";
                        ImgPreview.Source = null;
                    }
                    else
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Ошибка сервера: {response.StatusCode}\n{error}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}");
            }
        }
    }
}