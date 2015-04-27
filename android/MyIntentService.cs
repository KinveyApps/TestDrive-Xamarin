using System;
using Android.App;
using Android.OS;
using Android.Content;
using KinveyXamarinAndroid;
using Android.Support.V4.App;
using Android.Gms.Gcm;

namespace AndroidTestDrive
{
	[Service]
	public class MyIntentService : KinveyGCMService
	{
		public override void onMessage(String message) {
			displayNotification(message);
		}
		public override void onError(String error) {
			displayNotification(error);
		}
		public override void onDelete(int deleted) {
			displayNotification(deleted.ToString());
		}
		public override void onRegistered(String gcmID) {
			displayNotification(gcmID);
		}
		public override void onUnregistered(String oldID) {
			displayNotification(oldID);
		}
		private void displayNotification(String message){
			Console.WriteLine(message);

			NotificationCompat.Builder builder = new NotificationCompat.Builder(this)
				.SetContentTitle("Push Received!") // Set the title
				.SetSmallIcon(Resource.Drawable.Icon) // This is the icon to display
				.SetContentText(message); // the message to display.

			NotificationManager notificationManager = (NotificationManager)GetSystemService(NotificationService);
			notificationManager.Notify(100, builder.Build());

		}
	}
}