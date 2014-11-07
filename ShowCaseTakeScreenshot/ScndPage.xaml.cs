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
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkID=390556 dokumentiert.

namespace ShowCaseTakeScreenshot
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class ScndPage : Page
    {
        private readonly StorageFolder photoStorage = KnownFolders.PicturesLibrary;
        public ScndPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Wird aufgerufen, wenn diese Seite in einem Frame angezeigt werden soll.
        /// </summary>
        /// <param name="e">Ereignisdaten, die beschreiben, wie diese Seite erreicht wurde.
        /// Dieser Parameter wird normalerweise zum Konfigurieren der Seite verwendet.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MyWebViewControl.Navigate(new Uri("ms-appx-web:///Website/index.html"));
            MyWebViewControl.DOMContentLoaded += TakeScreenshot;
            
        }

        private void GetOutOfHere(object sender, object e)
        {
            App.Current.Exit();
        }

        async private void TakeScreenshot(object sender, object e)
        {
            //Create the file you want to save your screenshot
            var photo = await photoStorage.CreateFileAsync("MyScreenshot.jpg", CreationCollisionOption.GenerateUniqueName);

            RenderTargetBitmap bitmap = new RenderTargetBitmap();
            //Render the Page
            var width = (int)Math.Floor(MyPage.ActualWidth);
            var height = (int)Math.Floor(MyPage.ActualHeight);
            await bitmap.RenderAsync(MyPage, width, height);

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
            MessageDialog dialog = new MessageDialog("Hey I took a Screenshot!");
            await dialog.ShowAsync();
            HardwareButtons.BackPressed += GetOutOfHere;
        }
    }
}
