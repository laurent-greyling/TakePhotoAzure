using Microsoft.WindowsAzure.Storage;
using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TakePhotoAzure.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : ContentPage
	{
        private const string _connectionString = "";
        private string _photoPath = "";

		public MainPage ()
		{
			InitializeComponent ();
        }

        private async Task Take_Photo(object sender, EventArgs e)
        {
            var photo = await Plugin.Media.CrossMedia.Current.
                TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() { });

            if (photo != null)
            {
                _photoPath = photo.Path;
                PhotoImage.Source = ImageSource.FromFile(photo.Path);
            }
        }

        private async Task Save_Cloud(object sender, EventArgs e)
        {
            try
            {
                var storageAccount = CloudStorageAccount.Parse(_connectionString);
                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference("myphotos");

                //TODO: why does this work but not await container.CreateIfNotExistsAsync();
                container.CreateIfNotExistsAsync().Wait();

                var blockBlob = container.GetBlockBlobReference($"{PhotoImage.Source.Id}");

                if (PhotoImage.Source != null)
                    using (var fileStream = System.IO.File.OpenRead(_photoPath))
                    {
                        await blockBlob.UploadFromStreamAsync(fileStream);
                    }
            }
            catch (Exception xe)
            {
                var msg = xe.Message;
                throw;
            }
            
        }
    }
}