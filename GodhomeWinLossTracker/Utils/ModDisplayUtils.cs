using Modding;
using UnityEngine;

namespace GodhomeWinLossTracker.Utils
{
    internal static class ModDisplayUtils
    {
        // Enable this can make the FPS drop from 270 to 200
        public static void Initialize()
        {
            ModHooks.HeroUpdateHook += OnHeroUpdate;
        }

        private static void OnHeroUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                ModDisplay.instance.Create();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                ModDisplay.instance.Destroy();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                ModDisplay.instance.Redraw();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                ModDisplay.instance.Show();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                ModDisplay.instance.Hide();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                ModDisplay.instance.Notify("Won against Soul Warrior in HoG (PB 1:32, hitless)");
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                _mode = _mode switch
                {
                    "pan" => "scale",
                    "scale" => "pan",
                    _ => throw new AssertionFailedException("Should never arrive here"),
                };
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                if (_mode == "pan")
                {
                    ModDisplay.instance.TextPosition = new Vector2(
                        ModDisplay.instance.TextPosition.x - 0.01f,
                        ModDisplay.instance.TextPosition.y
                    );
                    GodhomeWinLossTracker.instance.LogMod($"{ModDisplay.instance.TextPosition.x,0:F2} {ModDisplay.instance.TextPosition.y,0:F2}");
                }
                else if (_mode == "scale")
                {
                    ModDisplay.instance.TextSize = new Vector2(
                        ModDisplay.instance.TextSize.x - 10,
                        ModDisplay.instance.TextSize.y
                    );
                    GodhomeWinLossTracker.instance.LogMod($"{ModDisplay.instance.TextSize.x,0:F0} {ModDisplay.instance.TextSize.y,0:F0}");
                }
                ModDisplay.instance.Redraw();
            }
            else if (Input.GetKeyDown(KeyCode.H))
            {
                if (_mode == "pan")
                {
                    ModDisplay.instance.TextPosition = new Vector2(
                        ModDisplay.instance.TextPosition.x + 0.01f,
                        ModDisplay.instance.TextPosition.y
                    );
                    GodhomeWinLossTracker.instance.LogMod($"{ModDisplay.instance.TextPosition.x,0:F2} {ModDisplay.instance.TextPosition.y,0:F2}");
                }
                else if (_mode == "scale")
                {
                    ModDisplay.instance.TextSize = new Vector2(
                        ModDisplay.instance.TextSize.x + 10,
                        ModDisplay.instance.TextSize.y
                    );
                    GodhomeWinLossTracker.instance.LogMod($"{ModDisplay.instance.TextSize.x,0:F0} {ModDisplay.instance.TextSize.y,0:F0}");
                }
                ModDisplay.instance.Redraw();
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                if (_mode == "pan")
                {
                    ModDisplay.instance.TextPosition = new Vector2(
                        ModDisplay.instance.TextPosition.x,
                        ModDisplay.instance.TextPosition.y + 0.01f
                    );
                    GodhomeWinLossTracker.instance.LogMod($"{ModDisplay.instance.TextPosition.x,0:F2} {ModDisplay.instance.TextPosition.y,0:F2}");
                }
                else if (_mode == "scale")
                {
                    ModDisplay.instance.TextSize = new Vector2(
                        ModDisplay.instance.TextSize.x,
                        ModDisplay.instance.TextSize.y + 10
                    );
                    GodhomeWinLossTracker.instance.LogMod($"{ModDisplay.instance.TextSize.x,0:F0} {ModDisplay.instance.TextSize.y,0:F0}");
                }
                ModDisplay.instance.Redraw();
            }
            else if (Input.GetKeyDown(KeyCode.G))
            {
                if (_mode == "pan")
                {
                    ModDisplay.instance.TextPosition = new Vector2(
                        ModDisplay.instance.TextPosition.x,
                        ModDisplay.instance.TextPosition.y - 0.01f
                    );
                    GodhomeWinLossTracker.instance.LogMod($"{ModDisplay.instance.TextPosition.x,0:F2} {ModDisplay.instance.TextPosition.y,0:F2}");
                }
                else if (_mode == "scale")
                {
                    ModDisplay.instance.TextSize = new Vector2(
                        ModDisplay.instance.TextSize.x,
                        ModDisplay.instance.TextSize.y - 10
                    );
                    GodhomeWinLossTracker.instance.LogMod($"{ModDisplay.instance.TextSize.x,0:F0} {ModDisplay.instance.TextSize.y,0:F0}");
                }
                ModDisplay.instance.Redraw();
            }
        }

        private static string _mode = "pan";
    }
}
