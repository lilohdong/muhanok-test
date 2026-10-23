using System.Globalization;
using Muhanok.Domain;
using UnityEngine;

namespace Muhanok.Presentation
{
    // Distance meter with the best record and the route bar (mine → cart → prison → zoo) with the lap count.
    internal sealed class ProgressHud
    {
        static readonly string[] SectionNames = { "광산", "광산 수레", "감옥 복도", "사육장" };
        static readonly Color Accent = Palette.Gold;
        const float MeterTop = 78f;
        static readonly Rect BarArea = new Rect(280, 612, 720, 52);
        static readonly CultureInfo Num = CultureInfo.InvariantCulture;

        readonly OverlaySkin skin;
        float distance, best, progress;
        int section, lap = 1, lastSection = -1;
        float sectionChangedAt = -10f;
        string meterText, meterOutline, bestText;
        int shownMeters = -1, shownBest = -1;

        public ProgressHud(OverlaySkin overlaySkin) { skin = overlaySkin; }

        public void Draw(RunSession session, float savedBest)
        {
            SetDistance(session.Score.Distance, savedBest);
            FollowRoute(session.Runner.Z);
            DrawMeter();
            DrawBar();
        }

        // Map chunk types to bar sections. Stairs and the facility link count as part of the prison.
        static int SectionOf(int type)
        {
            if (type == 0) return 0;
            if (type == 1 || type == 2) return 1;
            if (type >= 11) return 3;
            return 2;
        }

        void FollowRoute(float z)
        {
            int seq = RouteSurface.SequenceAt(z);
            int cycle = RouteSurface.CycleAt(seq);
            int sec = SectionOf(RouteSurface.TypeAt(seq));

            int start = seq, end = seq;
            while (start > 0 && seq - start < 64 && RouteSurface.CycleAt(start - 1) == cycle && SectionOf(RouteSurface.TypeAt(start - 1)) == sec) start--;
            while (end - seq < 64 && RouteSurface.CycleAt(end + 1) == cycle && SectionOf(RouteSurface.TypeAt(end + 1)) == sec) end++;

            float length = (end - start + 1) * RouteSurface.Length;
            float within = (z - start * RouteSurface.Length) / length;
            SetSectionProgress(sec, within, cycle + 1);
        }

        // Texts are rebuilt only when the shown whole meters change.
        void SetDistance(float meters, float savedBest)
        {
            distance = meters;
            best = Mathf.Max(savedBest, meters);
            int m = Mathf.FloorToInt(distance), b = Mathf.FloorToInt(best);
            if (m != shownMeters)
            {
                shownMeters = m;
                string n = m.ToString("N0", Num);
                meterText = n + "<size=38><color=" + Palette.Hex(Accent) + "> m</color></size>";
                meterOutline = n + "<size=38> m</size>";
            }
            if (b != shownBest)
            {
                shownBest = b;
                bestText = "최고 기록 " + b.ToString("N0", Num) + "m";
            }
        }

        void SetSectionProgress(int index, float withinSection, int lapCount)
        {
            section = Mathf.Clamp(index, 0, SectionNames.Length - 1);
            progress = Mathf.Clamp01((section + Mathf.Clamp01(withinSection)) / SectionNames.Length);
            lap = Mathf.Max(1, lapCount);
            if (section != lastSection)
            {
                lastSection = section;
                sectionChangedAt = Time.unscaledTime;
            }
        }

        void DrawMeter()
        {
            skin.OutlinedText(new Rect(290, MeterTop, 700, 90), meterOutline, meterText, 80, Palette.Cream, Palette.Outline, 2.5f, TextAnchor.UpperCenter);

            var pill = new Rect(640 - 115, MeterTop + 92, 230, 34);
            skin.BorderBox(pill, Palette.A(Palette.Ground, 0.85f), Palette.Wood, 2);
            skin.Text(pill, bestText, 16, Palette.MeNote, TextAnchor.MiddleCenter, false, true);
        }

        void DrawBar()
        {
            var r = BarArea;
            int count = SectionNames.Length;
            skin.BorderBox(r, Palette.A(Palette.Ground, 0.85f), Palette.Wood, 3);

            var track = new Rect(r.x + 18, r.y + 9, r.width - 36 - 64, 16);
            float cell = track.width / count;

            skin.BorderBox(track, Palette.Track, Palette.Dark, 2);
            // current section glows a little so it reads as "you are here"
            skin.Box(new Rect(track.x + cell * section + 2, track.y + 2, cell - 4, track.height - 4), Palette.A(Accent, 0.18f));
            if (progress > 0.01f)
                skin.Box(new Rect(track.x + 2, track.y + 2, (track.width - 4) * progress, track.height - 4), Accent);

            for (int i = 1; i < count; i++)
                skin.Fill(new Rect(track.x + cell * i - 1.5f, track.y, 3, track.height), Palette.Dark);

            float pop = Mathf.Clamp01(1f - (Time.unscaledTime - sectionChangedAt) / 0.4f);
            float size = 30 + 8 * pop;
            float mx = track.x + track.width * progress;
            var marker = new Rect(mx - size / 2, track.center.y - size / 2, size, size);
            skin.Circle(new Rect(marker.x, marker.y + 2, marker.width, marker.height), new Color(0, 0, 0, 0.4f));
            skin.Circle(marker, Palette.Cream);
            skin.Circle(new Rect(marker.x + 3, marker.y + 3, size - 6, size - 6), Palette.Monkey);

            for (int i = 0; i < count; i++)
            {
                Color c = i == section ? Accent : (i < section ? Palette.Sand : Palette.Muted);
                skin.Text(new Rect(track.x + cell * i, track.yMax + 4, cell, 18), SectionNames[i], i == section ? 14 : 13, c, TextAnchor.MiddleCenter, false, true);
            }

            skin.Text(new Rect(track.xMax + 8, r.y + 4, 56, r.height - 8), lap + "바퀴", 16, Palette.Sand, TextAnchor.MiddleCenter, true);
        }
    }
}
