using System;

using Android.App;
using Android.Content;
using Android.Graphics;
using Java.IO;
using System.IO;
using Xamarin.Forms;
using Java.Util;
using System.Drawing;
using Uri = Android.Net.Uri;
using System.Collections.Generic;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Widget;
using Android.Util;


using Environment = Android.OS.Environment;


[assembly: Xamarin.Forms.Dependency(typeof(HyperlocalNews.Droid.GalleryImageService_Android))]

namespace HyperlocalNews.Droid 
{

	public static class AppRes{
		public static Java.IO.File _file;
		public static Bitmap bitmap;
	}

	public class GalleryImageService_Android : Java.Lang.Object, IGalleryImageService
	{

		public GalleryImageService_Android(){}
		private Action<String> onCompletionMethod;
		public void LaunchImageSelector(Action<String> onCompletion){

			onCompletionMethod = onCompletion;

			MainActivity androidContext = (MainActivity)Forms.Context;

			Intent imageIntent = new Intent();
			imageIntent.SetType("image/*");
			imageIntent.SetAction(Intent.ActionGetContent);
			androidContext.ConfigureActivityResultCallback(LaunchImageSelectorReturned);
			androidContext.StartActivityForResult(Intent.CreateChooser(imageIntent, "Select photo"), 0);  

		}

		private void LaunchImageSelectorReturned(int requestCode, Result resultCode, Intent data)
		{
			if (resultCode == Result.Ok)
			{
				bool isKitKat = Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat;

				Uri uri = data.Data;
				string path = "";

				if (!isKitKat)
				{
					//the usual function
					path = getRealPathFromURI(uri);
				}
				else
				{

					bool isdoc = DocumentsContract.IsDocumentUri(Android.App.Application.Context, uri);
					if (isdoc)
					{
						if (IsDownloadsDocument(uri))
						{

							string id = DocumentsContract.GetDocumentId(uri);
							Uri contentUri = ContentUris.WithAppendedId(Uri.Parse("content://downloads/public_downloads"), Convert.ToInt64(id));

							path = GetDataColumn(Android.App.Application.Context, contentUri, null, null);

						}
						else if (IsMediaDocument(uri))
						{

							string docId = DocumentsContract.GetDocumentId(uri);
							string[] split = docId.Split(':');

							string type = split[0];

							Uri contentUri = null;
							if ("image".Equals(type))
							{
								contentUri = MediaStore.Images.Media.ExternalContentUri;
							}
							else if ("video".Equals(type))
							{
								contentUri = MediaStore.Video.Media.ExternalContentUri;
							}
							else if ("audio".Equals(type))
							{
								contentUri = MediaStore.Audio.Media.ExternalContentUri;
							}

							string selection = "_id=?";
							string[] selectionArgs = new String[] {
								split[1]
							};

							path = GetDataColumn(Android.App.Application.Context, contentUri, selection, selectionArgs);

						}
					}
				}

				try{
					AppRes._file = new Java.IO.File(path);
					string uniqueName = UUID.RandomUUID()+"";

					AppRes.bitmap = AppRes._file.Path.LoadAndResizeBitmap (1000, 1000);
					string fileName = uniqueName + ".png";
					Stream fos = Forms.Context.OpenFileOutput (fileName, FileCreationMode.Private);
					AppRes.bitmap.Compress (Bitmap.CompressFormat.Jpeg, 70, fos);
					fos.Close();

					AppRes.bitmap.Recycle();
					AppRes.bitmap.Dispose();
					AppRes.bitmap = null;

					AppRes.bitmap = AppRes._file.Path.LoadAndResizeBitmap (300, 300);
					string fileNameThumb = "thumb_" + fileName;
					Stream fosThumb = Forms.Context.OpenFileOutput (fileNameThumb, FileCreationMode.Private);
					AppRes.bitmap.Compress (Bitmap.CompressFormat.Jpeg, 70, fosThumb);

					fosThumb.Close();

					AppRes.bitmap.Recycle();
					AppRes.bitmap.Dispose();
					AppRes.bitmap = null;


					onCompletionMethod ( fileName );
				} catch (Exception e) {
					System.Console.WriteLine (e);
				}

			}
		}

		//method to get path for < KitKat
		public  static string  getRealPathFromURI(Android.Net.Uri contentUri)
		{
			Android.Content.Context context = Android.App.Application.Context;
			string[] projection = new string[] {Android.Provider.MediaStore.MediaColumns.Data };
			ContentResolver cr = context.ContentResolver;
			Android.Database.ICursor cursor = cr.Query(contentUri, projection,null, null, null);
			if (cursor != null && cursor.Count > 0)
			{
				cursor.MoveToFirst();
				int index =cursor.GetColumnIndex(Android.Provider.MediaStore.MediaColumns.Data);
				return cursor.GetString(index);
			}
			//not resolved, so return original path as string
			return contentUri.Path;
		}


		public Stream GetFileStream(string imageResourcesString){

			Log.Debug ("Newstori","Attempting to open: " + imageResourcesString);
			Stream returnStream = null;

			try{
				returnStream = Forms.Context.OpenFileInput (imageResourcesString);
			}catch(Java.IO.FileNotFoundException e){
				Log.Debug ("Newstori","Couldn't get file stream for: "+imageResourcesString);
				returnStream = null;
			}

			return  returnStream;
		}


		public Stream GetThumb(string imageResourcesString){

			Log.Debug ("Newstori","Attempting to open: " + imageResourcesString);
			Stream returnStream = null;
			bool filePresent = false;

			try{
				returnStream = Forms.Context.OpenFileInput ("thumb_" + imageResourcesString);
				filePresent = true;
			}catch(Exception e){
				Log.Debug ("Newstori","Couldn't get file stream for: "+imageResourcesString);
				filePresent = false;
				returnStream = null;
			}

			if(!filePresent){
				try{
					returnStream = Forms.Context.OpenFileInput (imageResourcesString);
				}catch(Java.IO.FileNotFoundException e){
					Log.Debug ("Newstori","Couldn't get file stream for: "+imageResourcesString);
					returnStream = null;
				}
			}

			return returnStream;
		}

		public void DeleteFile(string url){
			try{
				Forms.Context.DeleteFile (url);
			}catch(Exception e){
				Log.Debug ("Newstori","Couldn't delete file: "+url);
			}
			try{
				Forms.Context.DeleteFile ("thumb_" + url);
			}catch(Exception e){
				Log.Debug ("Newstori","Couldn't delete file: "+url);
			}
		}


		//method to get path for >= KitKat
		private string GetDataColumn(Context context, Uri uri, String selection, string[] selectionArgs) {

			Android.Database.ICursor cursor = null;
			string column = "_data";
			string[] projection = {
				column
			};

			try {

				cursor = context.ContentResolver.Query(uri, projection, selection, selectionArgs, null);
				if (cursor != null && cursor.MoveToFirst()) {
					int index = cursor.GetColumnIndexOrThrow(column);
					return cursor.GetString(index);
				}
			} finally {
				if (cursor != null)
					cursor.Close();
			}
			return null;
		}

		private bool IsExternalStorageDocument(Uri uri)
		{
			return "com.android.externalstorage.documents".Equals(uri.Authority);
		}

		private bool IsDownloadsDocument(Uri uri)
		{
			return "com.android.providers.downloads.documents".Equals(uri.Authority);
		}

		private bool IsMediaDocument(Uri uri)
		{
			return "com.android.providers.media.documents".Equals(uri.Authority);
		}

		private bool IsGooglePhotosUri(Uri uri)
		{
			return "com.google.android.apps.photos.content".Equals(uri.Authority);
		}


	}
}