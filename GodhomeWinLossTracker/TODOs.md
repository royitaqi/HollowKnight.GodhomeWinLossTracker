* [x] Add a mod menu containing one button "Export Stats".
  * [x] Gather win/loss stats into internal state.
  * [/] At the press of "Export Stats", log stats into into ModLog.txt
* [x] Detect boss fight results and debug log into ModLog.txt
* [x] Save stats to local settings at save&quit and load back at initialization
* Detect sequence name (HoG, P1-P5)
  * [x] Detect HoG
  * [x] Detect pantheons
  * [x] Detect p1-p5
* [x] Display UI when registering a win/loss.
  * [x] Hide UI after a few seconds
  * Add a gray half-transparent background (similar to PantheonsHitCounter)
* Display stats in mod menu.
  * Allow changing stats in mod menu.
* Add a UI in HoG and next to each pantheon to display win rates.
  * Predict chance of winning pantheons by putting per-boss win rate together.
* [/] See if TKDeath can be tracked by using the `AfterPlayerDeadHook`. See following example.
  * [/] Checked. This event doesn't trigger when failing Godhome boss fights. Probably because it's not considered a real death.
* Check alternative ways to implement sequence/boss/winloss detection logic:
  * Use `On.BossSequenceController.SetupNewSequence` to detect pantheons
  * [x] Use `BossSceneController.IsBossScene` to detect boss scene
    * [/] This works differently than the message bus way. This function checks the actual current scene, while my implementation checks the given scene name (no matter it's a current scene or not).
  * Use `BossSceneController.OnBossesDead` to detect both deaths. This replaces `OnEndBossScene`.
    * Check if `OnBossesDead` is triggered at the instance of boss death (TK can still die), or right before transiting out of the boss fight.
  * REF https://github.com/Clazex/HollowKnight.GodSeekerPlus/issues/30
* REF @dewin working on "adding Deathlink to Archipelago (so if you die with it on, all other players with it on die too and vice versa)".
  * REF Chat: https://discord.com/channels/879125729936298015/879130756146954240/988525314118385706
