using System;
using System.IO;
using MobileCoreServices;
using System.Drawing;
using Foundation;
using UIKit;


[assembly: Xamarin.Forms.Dependency(typeof(HyperlocalNews.iOS.GalleryImageService_iOS))]

namespace HyperlocalNews.iOS
{
	public class GalleryImageService_iOS : IGalleryImageService
	{
		public GalleryImageService_iOS (){}

		public UIImagePickerController imagePicker;

		private Action<String> onCompletionMethod;
		public void LaunchImageSelector(Action<String> onCompletion){

			onCompletionMethod = onCompletion;

			imagePicker = new UIImagePickerController ();
			imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;

			//restrict image picker to images (not videos)
			imagePicker.MediaTypes = new string[]{ UTType.Image };

			imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
			imagePicker.Canceled += Handle_Canceled;

			UIWindow window = UIApplication.SharedApplication.KeyWindow;
			UIViewController viewController = window.RootViewController;

			viewController.PresentViewController (imagePicker, true, null);


		}

		protected void Handle_FinishedPickingMedia (object sender, UIImagePickerMediaPickedEventArgs e)
		{

			NSUrl referenceURL = e.Info[new NSString("UIImagePickerControllerReferenceUrl")] as NSUrl;
			if (referenceURL != null)
				Console.WriteLine("Url:"+referenceURL.ToString ());

				UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;
				if(originalImage != null) {

					//resize image
					UIImage resizedImage = MaxResizeImage(originalImage, 1024, 1024);
					
					//store image in application filespace
					var urls = NSFileManager.DefaultManager.GetUrls (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User);
					var path = urls [0].Path;
					string randomName = System.Guid.NewGuid().ToString();
					randomName = randomName + ".png";

					var filename = Path.Combine (path, randomName);
					string PNGFilename = System.IO.Path.Combine (path, filename);
					
					NSData imgData = resizedImage.AsPNG(); // Image conversion to png here
					NSError err = null;

					if (imgData.Save(PNGFilename, false, out err)) {
						Console.WriteLine("saved as " + PNGFilename);
						onCompletionMethod ( PNGFilename );
					} else {
						Console.WriteLine("NOT saved as " + PNGFilename + " because" + err.LocalizedDescription);
					}
				}
			//dismiss popup
			imagePicker.DismissViewController(true, null);
		}


		//handle cancel in image picker
		void Handle_Canceled (object sender, EventArgs e) {
			//dismiss popup
			imagePicker.DismissViewController(true, null);
		}


		//return file stream for post request
		public Stream GetFileStream(string url){

			Console.WriteLine ("Attempting to open: " + url);
			Stream returnStream = null;

			try{
				returnStream = File.OpenRead (url);
			}catch(IOException e){
				Console.WriteLine ("Couldn't get file stream for: "+url);
			}
			return  returnStream;
		}


		public Stream GetThumb(string url){

			Console.WriteLine ("Attempting to open: " + url);
			Stream returnStream = null;

			try{
				returnStream = File.OpenRead (url);
			}catch(IOException e){
				Console.WriteLine ("Couldn't get file stream for: "+url);
			}
			return  returnStream;
		}

		//delete file
		public void DeleteFile(string url){

			try{
				File.Delete (url);
			}catch(IOException e){
				Console.WriteLine ("Couldn't delete file: "+url);
			}
		}

		// resize the image to be contained within a maximum width and height, keeping aspect ratio
		public UIImage MaxResizeImage(UIImage sourceImage, float maxWidth, float maxHeight)
		{
			var sourceSize = sourceImage.Size;
			float maxResizeFactor = (float) Math.Min(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
			if (maxResizeFactor > 1) return sourceImage;
			float width = (float)maxResizeFactor * (float)sourceSize.Width;
			float height = (float) maxResizeFactor * (float)sourceSize.Height;
			UIGraphics.BeginImageContext(new SizeF(width, height));
			sourceImage.Draw(new RectangleF(0, 0, width, height));
			var resultImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return resultImage;
		}

	}
}