using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace SplashScreenAutomator.Editor
{
    [InitializeOnLoad]
    public class SplashScreenAutomator : IPreprocessBuildWithReport
    {
        private const string LOGO_PATH = "Packages/com.havengrind.webgl-automation/Editor/CompanyLogo/Logo.png";

        // Interface requirement: execution order (0 means first execution)
        public int callbackOrder => 0;

        static SplashScreenAutomator()
        {
            EditorApplication.delayCall += () => EnsureSettings(false);
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log("<color=#00FF00>[Haven Grind]</color> Pre-build check: checking Splash Screen...");
            EnsureSettings(true);
        }

        private static void EnsureSettings(bool isForcing)
        {
            Sprite companyLogo = AssetDatabase.LoadAssetAtPath<Sprite>(LOGO_PATH);
            
            if (companyLogo == null)
            {
                string path = AssetDatabase.GUIDToAssetPath("663e506a625cf4a03ad45ace15fab71e");
                companyLogo = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }

            if (companyLogo == null) return;

            PlayerSettings.SplashScreen.show = true;
            PlayerSettings.SplashScreen.drawMode = PlayerSettings.SplashScreen.DrawMode.AllSequential;

            var currentLogos = PlayerSettings.SplashScreen.logos;
            
            List<PlayerSettings.SplashScreenLogo> logoList = PlayerSettings.SplashScreen.logos.ToList();

            logoList.RemoveAll(
                l => l.logo == null || 
                l.logo == companyLogo || 
                l.logo == PlayerSettings.SplashScreenLogo.unityLogo
            );

            // ----- INSERT LOGO -----
            logoList.Insert(0, new PlayerSettings.SplashScreenLogo
            {
                logo = PlayerSettings.SplashScreenLogo.unityLogo,
                duration = 2f
            });

            logoList.Insert(1, new PlayerSettings.SplashScreenLogo
            {
                logo = companyLogo,
                duration = 2f
            });

            PlayerSettings.SplashScreen.logos = logoList.ToArray();
            
            if (isForcing)
            {
                AssetDatabase.SaveAssets();
                Debug.Log("<color=#00FF00>[Haven Grind]</color> Settings Locked & Saved for Build.");
            }
        }
    }
}