# TODOs

* [x] Add a mod menu containing one button "Export Stats".
  * [x] Gather win/loss stats into internal state.
  * [/] At the press of "Export Stats", log stats into into ModLog.txt
* [x] Detect boss fight results and debug log into ModLog.txt
* [x] Save stats to local settings at save&quit and load back at initialization
* Detect sequence name (HoG, P1-P5)
  * [x] Detect HoG
  * [x] Detect pantheons
  * Detect p1-p5
* [x] Display UI when registering a win/loss.
  * [x] Hide UI after a few seconds
  * Add a gray half-transparent background (similar to PantheonsHitCounter)
* Display stats in mod menu.
  * Allow changing stats in mod menu.
* Add a UI in HoG and next to each pantheon to display win rates.
* [/] See if TKDeath can be tracked by using the `AfterPlayerDeadHook`. See following example.

```
ModHooks.AfterPlayerDeadHook += ModHooks_AfterPlayerDeadHook;
        }

        private void ModHooks_AfterPlayerDeadHook()
        {
            PlayerData.instance.SetBool("soulLimited", false);
            //PlayMakerFSM.BroadcastEvent("SOUL LIMITER DOWN");
        }
```
