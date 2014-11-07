using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


namespace ShowCaseTakeScreenshot
{
    public sealed partial class MainPage : Page
    {
        private readonly StorageFolder photoStorage = KnownFolders.PicturesLibrary;
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed += GetOutOfHere;
        }

        private void GetOutOfHere(object sender, object e)
        {
            App.Current.Exit();
        }

        async private void ScreenshotBtn_Click(object sender, RoutedEventArgs e)
        {
            ScreenshotBtn.IsEnabled = false;

            //Create the file you want to save your screenshot
            var photo = await photoStorage.CreateFileAsync("MyScreenshot.jpg", CreationCollisionOption.ReplaceExisting);

            RenderTargetBitmap bitmap = new RenderTargetBitmap();
            //Render the Page
            var width =(int) Math.Floor(MyPage.ActualWidth);
            var height = (int) Math.Floor(MyPage.ActualHeight);
            await bitmap.RenderAsync(MyPage, width, height);
            
            MyImage.Source = bitmap;

            var buffer = await bitmap.GetPixelsAsync();
            var bytearray = buffer.ToArray();

            var randomaccessstream = await photo.OpenAsync(FileAccessMode.ReadWrite);
            randomaccessstream.Seek(0);

            var encoderId = BitmapEncoder.JpegEncoderId;
            var encoder = await BitmapEncoder.CreateAsync(encoderId, randomaccessstream);

            encoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Straight, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, 96, 96, bytearray);
            await encoder.FlushAsync();
            await randomaccessstream.FlushAsync();
            randomaccessstream.Dispose();

            ScreenshotBtn.IsEnabled = true;
        }

        private void Navigate_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ScndPage));
        }
    }
}
