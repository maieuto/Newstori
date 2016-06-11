using System;

using Xamarin.Forms;

using PCLStorage;
using System.Threading.Tasks;


namespace HyperlocalNews
{
	public class FileStorageMgr
	{
		/* singleton manager */
		private static FileStorageMgr instance;

		private FileStorageMgr () {}

		public static FileStorageMgr getInstance(){

			if(instance == null){
				instance = new FileStorageMgr();
			}
			return instance;

		}


		public async void SaveFile(Action completedAction, string FileContent, string FolderName, string FileName){

			IFolder rootFolder = FileSystem.Current.LocalStorage;

			Task<IFolder> openFolderTask = rootFolder.CreateFolderAsync(FolderName,
				CreationCollisionOption.OpenIfExists);

			IFolder folder = await openFolderTask;

			Task<IFile> writeFileTask = folder.CreateFileAsync(FileName,
				CreationCollisionOption.ReplaceExisting);

			IFile file = await writeFileTask;

			await file.WriteAllTextAsync(FileContent);

			completedAction ();
		}


		public async void RetrieveFile (Action<string> completedAction, string FolderName, string FileName){

			string resultString = null;

			IFolder rootFolder = FileSystem.Current.LocalStorage;
			
			IFile file = null;

			try{

				Task<IFolder> openFolderTask = rootFolder.CreateFolderAsync(FolderName,
					CreationCollisionOption.OpenIfExists);
				IFolder folder = await openFolderTask;

				Task<IFile> getFileTask = folder.GetFileAsync(FileName);
				file = await getFileTask;

			}catch (Exception e){
				//caught an exception because of the file.
			}

			if (file != null) {
				Task<string> t = file.ReadAllTextAsync();
				resultString = await t;
			}

			completedAction (resultString);
		}


	}
}

