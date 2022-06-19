using System;
using System.Linq;
using Modding;
using UnityEngine;
using UnityEngine.UI;

namespace GodhomeWinLossTracker
{
    internal class ModDisplay
    {
        internal static ModDisplay instance;
        private GameObject _canvas;

        public string Text { get; set; }
        private Vector2 TextSize = new(200, 100);
        private Vector2 TextPosition = new(0.1f, 0.1f); //  + new Vector2(-0.025f, 0.05f)

        public static void Initialize()
        {
            instance = new ModDisplay();
            instance.Text = "";
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



        //public void BuildUI()
        //{
        //    canvas = new GameObject();
        //    canvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        //    var scaler = canvas.AddComponent<CanvasScaler>();
        //    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        //    scaler.referenceResolution = new Vector2(1920f, 1080f);
        //    canvas.AddComponent<GraphicRaycaster>();

        //    text = new UI.CanvasText(canvas, new Vector2(500, 500), new Vector2(200, 200), arial, "SOME EXAMPLE TEXT");

        //    DontDestroyOnLoad(canvas);
        //}
    }
}
