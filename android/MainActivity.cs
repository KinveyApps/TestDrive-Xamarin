﻿// Copyright (c) 2014, Kinvey, Inc. All rights reserved.
//
// This software is licensed to you under the Kinvey terms of service located at
// http://www.kinvey.com/terms-of-use. By downloading, accessing and/or using this
// software, you hereby accept such terms of service  (and any agreement referenced
// therein) and agree that you have read, understand and agree to be bound by such
// terms of service and are of legal age to agree to such terms with Kinvey.
//
// This software contains valuable confidential and proprietary information of
// KINVEY, INC and is subject to applicable licensing agreements.
// Unauthorized reproduction, transmission or distribution of this file and its
// contents is a violation of applicable laws.

using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using RestSharp;
using System.Threading;
using KinveyXamarin;
using System.Threading.Tasks;
using System.IO;
using SQLite.Net.Platform.XamarinAndroid;
using System.Linq;
using LinqExtender;
using System.Text;
using KinveyUtils;

namespace AndroidTestDrive
{
	[Activity (Label = "Android-TestDrive", MainLauncher = true)]
	public class MainActivity : Activity
	{
		int count = 1;

		private string appKey = "kid_eV220fVYa9";
		private string appSecret = "98b40ad7a65d4655859f2e7b1432e0a1";

		private static string COLLECTION = "myCollection";
		private static string STABLE_ID = "testdriver";

		Client kinveyClient;
		InMemoryCache<MyEntity> myCache = new InMemoryCache<MyEntity>();

		protected override async void OnCreate (Bundle bundle)
		{
		
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			kinveyClient = new Client.Builder(appKey, appSecret)
				.setFilePath(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal))
				.setOfflinePlatform(new SQLitePlatformAndroid())
				.setLogger(delegate(string msg) { Console.WriteLine(msg);})
				.build();

			Logger.Log ("---------------------------------------------logger");

			kinveyClient.Ping (new KinveyDelegate<PingResponse>{
				onSuccess = (response) => {
					RunOnUiThread (() => {
						Toast.MakeText(this, "Ping successful!", ToastLength.Short).Show();
					});
				},
				onError = (error) => {
					RunOnUiThread (() => {
						Toast.MakeText(this, "something went wrong: " + error.Message, ToastLength.Short).Show();
					});
				}
			});

			User user = await kinveyClient.User ().LoginAsync ();
			Toast.MakeText(this, "logged in as: " + user.Id, ToastLength.Short).Show();

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);
			button.Click += delegate {
				button.Text = string.Format ("{0} clicks!", count++);
			};

			Button save = FindViewById<Button> (Resource.Id.saveButton);
			save.Click += delegate {
				new Thread (() =>
					saveAndToast ()
				).Start ();
			};

			Button load = FindViewById<Button> (Resource.Id.loadButton);
			load.Click += delegate {
				new Thread(() =>
					loadAndToast()	
				).Start();
			};

			Button loadCache = FindViewById<Button> (Resource.Id.loadWithCacheButton);
			loadCache.Click += delegate {
				new Thread(() =>
					loadFromCacheAndToast()	
				).Start();
			};

			Button loadQuery = FindViewById<Button> (Resource.Id.loadWithQuery);
			loadQuery.Click += delegate {
				new Thread(() => 
					loadFromQuery()
				).Start();
			};





		}



		private async void saveAndToast(){
		
			AsyncAppData<MyEntity> entityCollection = kinveyClient.AppData<MyEntity>(COLLECTION, typeof(MyEntity));

			MyEntity ent = new MyEntity();
			ent.Email = "test@tester.com";
			ent.Name = "James Dean";
			ent.ID = STABLE_ID;
			entityCollection.setOffline(new SQLiteOfflineStore(), OfflinePolicy.LOCAL_FIRST);

			MyEntity entity = await entityCollection.SaveAsync (ent);
			Toast.MakeText (this, "saved: " + entity.Name, ToastLength.Short).Show ();
		}

		private async void loadAndToast(){
			AsyncAppData<MyEntity> entityCollection = kinveyClient.AppData<MyEntity>(COLLECTION, typeof(MyEntity));
			entityCollection.setOffline(new SQLiteOfflineStore(), OfflinePolicy.LOCAL_FIRST);

			MyEntity res = await entityCollection.GetEntityAsync (STABLE_ID);
			Toast.MakeText(this, "got: " + res.Name, ToastLength.Short).Show();
		}

		private async void loadFromCacheAndToast(){
			AsyncAppData<MyEntity> entityCollection = kinveyClient.AppData<MyEntity>(COLLECTION, typeof(MyEntity));
			entityCollection.setCache (myCache, CachePolicy.CACHE_FIRST);


			MyEntity entity = await entityCollection.GetEntityAsync (STABLE_ID);
			Toast.MakeText (this, "got: " + entity.Name, ToastLength.Short).Show ();
					
		}


		private void loadFromQuery(){

			AsyncAppData<MyEntity> query = kinveyClient.AppData<MyEntity>(COLLECTION, typeof(MyEntity));

			query.setOffline (new SQLiteOfflineStore(), OfflinePolicy.LOCAL_FIRST);
			var query1 = from cust in query
			             where cust.Name == "James Dean"
			             select cust;



			Task.Run (() => {
				foreach (MyEntity e in query1){
					Console.WriteLine("got -> " + e.Name);
				}
				Console.WriteLine("total at: " + query1.Count());
			});

		}


	}
}


