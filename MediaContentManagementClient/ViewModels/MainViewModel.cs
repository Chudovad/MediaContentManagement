using MediaContentManagementClient.Models;
using MediaContentManagementClient.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MediaContentManagementClient.ViewModels
{
    internal class MainViewModel : BaseViewModel
    {
        private string _selectedImagePath;
        private string _text;
        private string _error;
        private int pageNumber = 1;
        private readonly int pageSize = 5;
        private MediaContent mediaContent = new MediaContent();
        private ApiService apiService = new ApiService();
        private ObservableCollection<ImageInfo> _images;

        public ICommand SelectImageCommand { get; }
        public ICommand SendMediaContentCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }

        public MainViewModel()
        {
            SelectImageCommand = new DelegateCommand(SelectImage);
            SendMediaContentCommand = new DelegateCommand(SendMediaContent);
            Images = new ObservableCollection<ImageInfo>();
            LoadImages();
            PreviousPageCommand = new DelegateCommand(LoadPreviousPage, CanLoadPreviousPage);
            NextPageCommand = new DelegateCommand(LoadNextPage, CanLoadNextPage);
        }

        private async void LoadImages()
        {
            try
            {
                Error = "";
                Images.Clear();
                var images = await apiService.GetImagesAsync(pageNumber, pageSize);
                foreach (var item in images)
                {
                    Images.Add(item);
                }
            }
            catch (Exception ex)
            {
                Error = $"Ошибка при загрузке изображений: {ex.Message}";
            }
        }

        public ObservableCollection<ImageInfo> Images
        {
            get { return _images; }
            set
            {
                _images = value;
                OnPropertyChanged(nameof(Images));
            }
        }

        public string SelectedImagePath
        {
            get { return _selectedImagePath; }
            set
            {
                _selectedImagePath = value;
                OnPropertyChanged(nameof(SelectedImagePath));
            }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        public string Error
        {
            get { return _error; }
            set
            {
                _error = value;
                OnPropertyChanged(nameof(Error));
            }
        }

        private async void SendMediaContent(object parameter)
        {
            if (mediaContent.ImageBytes == null)
            {
                Error = "Выберите изображение";
                return;
            }

            if (!string.IsNullOrEmpty(_text))
            {
                mediaContent.Text = _text;
                Error = await apiService.SaveImageAsync(mediaContent);

                if (Error.Contains("Изображение успешно сохранено"))
                {
                    Images.Insert(0, new ImageInfo { text = _text, filePath = SelectedImagePath });
                    pageNumber = 1;
                    LoadImages();
                }
            }
            else
            {
                Error = "Заполните поле текста";
            }
        }

        private void SelectImage(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png, *.bmp, *.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string imagePath = openFileDialog.FileName;
                byte[] imageBytes = File.ReadAllBytes(imagePath);

                if (IsValidImage(imageBytes))
                {
                    SelectedImagePath = imagePath;
                    mediaContent.ImageBytes = imageBytes;
                    Error = "";
                }
                else
                {
                    Error = "Выбранный файл не является изображением";
                }
            }
        }

        private bool IsValidImage(byte[] imageDate)
        {
            try
            {
                Image image = new Image();
                using (MemoryStream stream = new MemoryStream(imageDate))
                {
                    image.Source = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void LoadPreviousPage(object parameter)
        {
            pageNumber--;
            LoadImages();
        }

        private bool CanLoadPreviousPage(object parameter)
        {
            return pageNumber > 1;
        }

        private void LoadNextPage(object parameter)
        {
            pageNumber++;
            LoadImages();
        }

        private bool CanLoadNextPage(object parameter)
        {
            if (!Error.Contains("Ошибка при загрузке изображений"))
            {
                var totalImagesCount = apiService.GetAllImagesCount();

                if (totalImagesCount.Success)
                {
                    return pageNumber * pageSize < int.Parse(totalImagesCount.Count);
                }
                else
                {
                    Error = totalImagesCount.Count;
                    return false;
                }
            }
            return false;
        }
    }
}