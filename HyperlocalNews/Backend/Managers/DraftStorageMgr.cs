using System;
using System.Collections.ObjectModel;
using PCLStorage;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace HyperlocalNews
{
	public class DraftStorageMgr
	{
		protected DraftStorageMgr ()
		{
		}

		private ObservableCollection<Report> _DraftReports = new ObservableCollection<Report>();


		public ObservableCollection<Report> GetDraftReports(Action receiver){

			DraftStorageAction dsa = new DraftStorageAction (GetDraftReportsReturned);
			dsa.actionBelongingToReciever = receiver;

			dsa.RetrieveSavedDrafts (_DraftReports);

			return _DraftReports;
		}

		private void GetDraftReportsReturned(DraftStorageAction dsa){
			//need to notify the receiver
			if(dsa.actionBelongingToReciever != null){
				dsa.actionBelongingToReciever();
			}
		}

		public void SaveDraftReport(Action receiver, Report draft){

			DraftStorageAction dsa = new DraftStorageAction (SaveDraftReportReturned);
			dsa.actionBelongingToReciever = receiver;

			_DraftReports.Add (draft);

			dsa.CommitDraftsToStorage (_DraftReports);
		}

		private void SaveDraftReportReturned(DraftStorageAction dsa){
			//need to notify the receiver
			if(dsa.actionBelongingToReciever != null){
				dsa.actionBelongingToReciever();
			}
		}

		public void RemoveDraftReport(Action receiver, Report draft){

			DraftStorageAction dsa = new DraftStorageAction (RemoveDraftReportReturned);
			dsa.actionBelongingToReciever = receiver;

			_DraftReports.Remove (draft);
			dsa.CommitDraftsToStorage (_DraftReports);
			
		}

		private void RemoveDraftReportReturned(DraftStorageAction dsa){
			//need to notify the receiver
			if(dsa.actionBelongingToReciever != null){
				dsa.actionBelongingToReciever();
			}
		}



		public void CommitDrafts(Action receiver){
			DraftStorageAction dsa = new DraftStorageAction (RemoveDraftReportReturned);
			dsa.actionBelongingToReciever = receiver;
			dsa.CommitDraftsToStorage (_DraftReports);
		}
		private void CommitDraftsReturned(DraftStorageAction dsa){
			//need to notify the receiver
			if(dsa.actionBelongingToReciever != null){
				dsa.actionBelongingToReciever();
			}
		}
	}

	public class DraftStorageAction{

		public Action actionBelongingToReciever;

		private Action<DraftStorageAction> completionMethod;
		private ObservableCollection<Report> collectionToUpdate;

		public DraftStorageAction(Action<DraftStorageAction> onCompletion){
			completionMethod = onCompletion;
		}

		public void CommitDraftsToStorage(ObservableCollection<Report> collection){

			string JSONString = JsonConvert.SerializeObject (collection, new JsonSerializerSettings(){TypeNameHandling = TypeNameHandling.Objects});

			FileStorageMgr.getInstance ().SaveFile (CommitDraftsToStorageReturned, JSONString, "DraftReports", "Drafts.json");
		}
		private void CommitDraftsToStorageReturned(){
			completionMethod(this);
		}


		public void RetrieveSavedDrafts (ObservableCollection<Report> collection){

			collectionToUpdate = collection;

			FileStorageMgr.getInstance ().RetrieveFile (RetrieveSavedDraftsReturned, "DraftReports", "Drafts.json");

		}
		private void RetrieveSavedDraftsReturned(string JSONString){

			collectionToUpdate.Clear ();

			if (JSONString != null) {

				ObservableCollection<Report> draftItems = JsonConvert.DeserializeObject<ObservableCollection<Report>> (JSONString);

				foreach (Report rep in draftItems) {
					collectionToUpdate.Add (rep);
				}

			}

			completionMethod (this);

		}


	}
}