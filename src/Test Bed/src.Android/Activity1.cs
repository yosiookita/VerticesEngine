using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Content.PM;
using Android.Views;
using Android.Widget;
using Android.OS;

using Microsoft.Xna.Framework;
using Virtex.App.VerticesTechDemo;

namespace MGTemplate.Android
{
	[Activity(Label = "MGTemplate.Android",
			   MainLauncher = true,
			   Icon = "@drawable/icon",
			   Theme = "@style/Theme.Splash",
			   AlwaysRetainTaskState = true,
			   LaunchMode = LaunchMode.SingleInstance,
              ScreenOrientation = ScreenOrientation.Landscape,
			   ConfigurationChanges = ConfigChanges.Orientation |
									  ConfigChanges.KeyboardHidden |
									  ConfigChanges.Keyboard |
									  ConfigChanges.ScreenSize)]
	public class Activity1 : AndroidGameActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

            var g = new VerticesTechDemoGame();
			//SetContentView(g.Services.GetService<View>());
			g.Run();
		}

	}
}

