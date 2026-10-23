#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.IO;
using Muhanok.Domain;
using Muhanok.Presentation;
using UnityEngine;

namespace Muhanok.Bootstrap
{
    // Only runs with --muhanok-uicapture: walks through every menu screen, a run and its result, and saves
    // screenshots to --muhanok-out <dir>. Use with --muhanok-data <dir> so real player data stays untouched.
    public sealed class RuntimeUiReview : MonoBehaviour
    {
        private GameBootstrap game;
        private string destination;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "--muhanok-uicapture") < 0) return;
            new GameObject("UI Review").AddComponent<RuntimeUiReview>();
        }
        private void Start()
        {
            UnityEngine.Application.runInBackground = true;
            var args = Environment.GetCommandLineArgs(); int at = Array.IndexOf(args, "--muhanok-out");
            destination = at >= 0 && at + 1 < args.Length ? args[at + 1] : Path.Combine(UnityEngine.Application.persistentDataPath, "UiReview");
            Directory.CreateDirectory(destination);
            StartCoroutine(Run());
        }
        private IEnumerator Run()
        {
            float started = Time.realtimeSinceStartup;
            while ((game = FindFirstObjectByType<GameBootstrap>()) == null || game.Session == null || game.Session.Flow.State != GameFlowState.TitleIdle) yield return null;
            game.SetDeveloperMode(false, false);
            yield return new WaitForSecondsRealtime(3f);
            var hud = game.Hud; var data = (IHudData)game;

            yield return Shot("01_Title");
            game.OpenMainMenu(); yield return Shot("02_MainMenu_NewPlayer");
            hud.ShowPage(MenuPage.Profile, true); yield return Shot("03_Profile_FirstRun");

            Seed(data);
            hud.ShowPage(MenuPage.Profile); yield return Shot("04_Profile");
            hud.ShowPage(MenuPage.Shop); yield return Shot("05_Shop");
            hud.ShowPage(MenuPage.Ranking); yield return Shot("06_Ranking");
            game.OpenMainMenu(); yield return Shot("07_MainMenu");
            hud.ShowPage(MenuPage.Setup); yield return Shot("08_Setup");

            game.StartPlayerRun(new PlayerProfile { Name = "탈옥왕", Weight = "65", Style = PlayStyle.Keyboard });
            while (game.IsLoading || game.IsRevealing || game.Session.Flow.State != GameFlowState.Running) yield return null;
            // Nobody steers: a shield kept active lets the run go on long enough for the HUD and pause shots.
            shielded = true; StartCoroutine(KeepShield());
            yield return new WaitForSecondsRealtime(3f);
            yield return Shot("09_RunHud");
            game.TogglePause(); yield return Shot("10_Pause"); game.TogglePause();
            shielded = false;

            // The next obstacle ends the run; a hit is forced if the route is clear for too long.
            float waited = 0;
            while (game.Session.Flow.State != GameFlowState.Result)
            {
                waited += Time.unscaledDeltaTime;
                if (waited > 8f && game.Session.Flow.State == GameFlowState.Running) game.Session.Hit(true);
                if (Time.realtimeSinceStartup - started > 180) { Fail("result did not appear"); yield break; }
                yield return null;
            }
            yield return Shot("11_Result");

            game.SetDeveloperMode(true, false); game.ExitToMenu(); yield return Shot("12_MainMenu_Developer");
            game.OpenStartSetup(); yield return Shot("13_Setup_Developer");
            Debug.Log("MUHANOK_UI_CAPTURE_PASS: " + destination);
            UnityEngine.Application.Quit(0);
        }
        private bool shielded;
        private IEnumerator KeepShield()
        {
            while (shielded) { if (game.Session.Flow.State == GameFlowState.Running) game.Session.Items.Activate(ItemKind.Shield); yield return null; }
        }
        // A profile, coins, one unlocked item and runs from today and yesterday.
        private void Seed(IHudData data)
        {
            data.SaveProfile(new PlayerProfile { Name = "탈옥왕", Weight = "65", Style = PlayStyle.Keyboard });
            var today = DateTime.Now; var yesterday = today.AddDays(-1);
            float[] todayRuns = { 2310, 1530, 980, 640, 455 }, earlier = { 1820, 1240 };
            for (int i = 0; i < todayRuns.Length; i++) game.Store.Record(new RankingEntry { Name = "탈옥왕", Distance = todayRuns[i], Score = (int)todayRuns[i] * 3, Style = "키보드", Ticks = today.AddMinutes(-i * 7).Ticks }, todayRuns[i] / 25);
            for (int i = 0; i < earlier.Length; i++) game.Store.Record(new RankingEntry { Name = "탈옥왕", Distance = earlier[i], Score = (int)earlier[i] * 3, Style = "전신", Ticks = yesterday.AddMinutes(-i * 9).Ticks }, earlier[i] / 18);
            game.Store.DepositCoins(240); game.Store.Unlock(ItemKind.Magnet);
        }
        private IEnumerator Shot(string name)
        {
            yield return new WaitForSecondsRealtime(.9f);
            string path = Path.Combine(destination, name + ".png");
            ScreenCapture.CaptureScreenshot(path);
            yield return new WaitForSecondsRealtime(.5f);
            if (!File.Exists(path)) { Fail("missing " + path); yield break; }
            Debug.Log("UI_CAPTURE: " + name);
        }
        private static void Fail(string reason)
        {
            Debug.LogError("MUHANOK_UI_CAPTURE_FAIL: " + reason);
            UnityEngine.Application.Quit(1);
        }
    }
}
#endif
