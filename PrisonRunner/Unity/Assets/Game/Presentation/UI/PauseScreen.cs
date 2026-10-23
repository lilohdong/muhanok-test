using System;
using UnityEngine;

namespace Muhanok.Presentation
{
    internal sealed class PauseScreen
    {
        private readonly HudSkin skin;
        public PauseScreen(HudSkin hudSkin) { skin = hudSkin; }
        // Leaving to the menu ends the run without a record.
        public void Draw(Action resume, Action settings, Action toMenu)
        {
            skin.Card(new Rect(0, 0, 1280, 720), Palette.A(Palette.Ground, .75f));
            skin.Panel(new Rect(420, 210, 440, 330), .97f);
            GUI.color = Color.white; GUI.Label(new Rect(470, 240, 350, 65), "잠깐 쉬어가기", skin.Style(34, FontStyle.Bold));
            if (skin.PrimaryButton(new Rect(470, 325, 340, 58), "계속하기", 26)) resume?.Invoke();
            if (skin.FlatButton(new Rect(470, 397, 340, 48), "설정", false)) settings?.Invoke();
            if (skin.FlatButton(new Rect(470, 457, 340, 48), "메인으로", false)) toMenu?.Invoke();
        }
    }
}
