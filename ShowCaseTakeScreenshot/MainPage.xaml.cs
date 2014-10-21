using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
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
        }

        async private void ScreenshotBtn_Click(object sender, RoutedEventArgs e)
        {
            //Create the file you want to save your screenshot
            var photo = await photoStorage.CreateFileAsync("MyScreenshot.jpg", CreationCollisionOption.ReplaceExisting);

            RenderTargetBitmap bitmap = new RenderTargetBitmap();
            //Render the outer Grid
            await bitmap.RenderAsync(MyOuterGrid);
            
            MyImage.Source = bitmap;

            var buffer = await bitmap.GetPixelsAsync();
            var bytearray = buffer.ToArray();

            var randomaccessstream = await photo.OpenAsync(FileAccessMode.ReadWrite);
            randomaccessstream.Seek(0);

            var encoderId = BitmapEncoder.JpegEncoderId;
            var encoder = await BitmapEncoder.CreateAsync(encoderId, randomaccessstream);

            encoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Straight, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, 96, 96, bytearray);
            await encoder.FlushAsync();            
        }
    }
}
