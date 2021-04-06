using System;
using Android.App;
using Android.Content;
using Android.OS;

namespace personalFinanceManager.Droid
{
    [Activity(Label = "Stashr", Theme = "@style/MyTheme.Splash", MainLauncher = true)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            StartActivity(typeof(MainActivity));
            Finish();
        }
    }
}
