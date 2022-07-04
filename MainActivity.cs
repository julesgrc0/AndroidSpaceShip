using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Microsoft.Xna.Framework;


namespace AndroidSpaceShip
{
  
    [Activity(
        Label = "@string/app_name",
        Theme = "@style/AppTheme.Splash",
        MainLauncher = true,
        Icon = "@drawable/icon",
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.Portrait,
        Immersive = true,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize | ConfigChanges.ScreenLayout | ConfigChanges.UiMode | ConfigChanges.SmallestScreenSize
    )]
    
    public class MainActivity : AndroidGameActivity
    {
        private GameRoot gameRoot;
        private View gameView;

        private Vibrator vibrator;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
           
            if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
            {
                Window.Attributes.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.Always;

                Window.AddFlags(WindowManagerFlags.Fullscreen);
                Window.AddFlags(WindowManagerFlags.HardwareAccelerated);
                Window.AddFlags(WindowManagerFlags.KeepScreenOn);
                
            }

            gameRoot = new GameRoot();
            gameView = gameRoot.Services.GetService(typeof(View)) as View;

            try
            {
                this.vibrator = (Vibrator)this.GetSystemService("vibrator");
                gameRoot.Vibrate = this.Vibrate;
            }
            catch { }

            SetContentView(gameView);
            gameRoot.Run();

        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            if(hasFocus)
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
                {
                    StatusBarVisibility visibility = (StatusBarVisibility)(SystemUiFlags.LayoutStable | SystemUiFlags.LayoutHideNavigation | SystemUiFlags.LayoutFullscreen | SystemUiFlags.HideNavigation | SystemUiFlags.Fullscreen | SystemUiFlags.ImmersiveSticky);
                    this.gameView.SystemUiVisibility = visibility;
                }
            }
            
        }
        
        public void Vibrate(long duration)
        {
            if(this.vibrator != null)
            {
                VibrationEffect vibrationEffect = VibrationEffect.CreateOneShot(duration, VibrationEffect.EffectDoubleClick);
                this.vibrator.Vibrate(vibrationEffect);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            gameRoot.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            gameRoot.OnResume();
        }

        protected override void OnRestart()
        {
            base.OnRestart();
            gameRoot.OnRestart();
        }
    }
}
