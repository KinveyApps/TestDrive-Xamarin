using System;
using Android.Content;
using Android.App;

namespace AndroidTestDrive
{
	[BroadcastReceiver(Permission= "com.google.android.c2dm.permission.SEND")]
	[IntentFilter(new string[] {"com.google.android.c2dm.intent.RECEIVE"})]
	[IntentFilter(new string[] {"com.google.android.c2dm.intent.REGISTRATION"})]
	[IntentFilter(new string[] {"com.google.android.gcm.intent.RETRY"})]
	[IntentFilter(new string[] {"com.kinvey.xamarin.android.ERROR"})]
	public class MyGCMBroadcastReceiver : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			Intent i = new Intent (context, typeof(MyIntentService));
			i.SetAction(intent.Action);
			i.PutExtras (intent.Extras);
			context.StartService (i);

			SetResult(Result.Ok, null, null);
		}
	}
}