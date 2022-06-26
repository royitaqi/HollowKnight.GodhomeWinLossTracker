using System;
using System.Collections;
using System.Threading.Tasks;
using Modding;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeWinLossTracker
{
    internal class ModDisplay
    {
        internal static ModDisplay instance;
        private GameObject _canvas;

        private Vector2 TextSize = new(200, 100);
        public Vector2 TextPosition = new(0.12f, 0.04f);

        public string Text = "Godhome Win Loss Tracker";
        public TimeSpan NotificationDuration = TimeSpan.FromSeconds(6);
        private DateTime _fadeOutTime;

        public static void Initialize()
        {
            instance = new ModDisplay();
        }

        public void Create()
        {
            if (_canvas != null) return;
            // Create base canvas
            _canvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1920, 1080));

            CanvasGroup canvasGroup = _canvas.GetComponent<CanvasGroup>();
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            UnityEngine.Object.DontDestroyOnLoad(_canvas);

            CanvasUtil.CreateTextPanel(_canvas, Text, 24, TextAnchor.MiddleCenter,
                new CanvasUtil.RectData(TextSize, Vector2.zero, TextPosition, TextPosition),
                CanvasUtil.GetFont("Perpetua"));

            Show();
        }

        public void Destroy()
        {
            if (_canvas != null) UnityEngine.Object.Destroy(_canvas);
            _canvas = null;
        }

        public void Show()
        {
            if (_canvas == null) return;
            _canvas.SetActive(true);
        }

        public void Hide()
        {
            if (_canvas == null) return;
            _canvas.SetActive(false);
        }

        public void Redraw()
        {
            Destroy();
            Create();
        }

        public void Notify(string text)
        {
            // Set up the text and a time to remove the notification.
            Text = text;
            _fadeOutTime = DateTime.Now + NotificationDuration;

            // Show the notification.
            Redraw();

            // Schedule the unnotify for a chance to remove the notification.
            Task.Delay(NotificationDuration).ContinueWith(t => Unnotify());
        }

        private void Unnotify()
        {
            // There can be multiple notifications going on in overlapping timespan.
            // Only destroy the notification object if the current time has gone beyond the fade out time.
            if (DateTime.Now > _fadeOutTime)
            {
                Destroy();
            }
        }
    }
}
