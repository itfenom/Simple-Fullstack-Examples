namespace Playground.WpfApp.Forms.Main
{
    public class SplashScreenViewModel
    {
        public string SplashScreenText1 => "Developer: Kashif Mubarak.";

        public string SplashScreenText2 => "Version: " + ApplicationInfo.Version;

        public string SplashScreenText3 => "© All Rights Reserved.";

        public string SplashScreenStatusBar => "Initializing... Please Wait!";
    }
}
