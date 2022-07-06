# HollowKnight.GodhomeWinLossTracker

GodhomeWinLossTracker is a mod for the game Hollow Knight. Its goal is to improve boss fight training experience in Godhome by tracking per-boss win/loss counts for Hall of Gods and pantheons.


## Stats tracking

The mod automatically tracks your boss fight results, be it in Hall of Gods or pantheons.

In Hall of Gods, different boss variants (dream version or different forms) are tracked separately. For the same boss variant/form, if it has different arenas for Attuned and Ascended/Radiant, they are tracked separately, too. Stats are displayed in the "Challenge" menu.

![hog_stats](https://user-images.githubusercontent.com/14790745/177464675-5db99441-65d8-4602-b30c-f38993e9f92d.png)

In pantheons, the number of runs, your PB (personal best, i.e. how far you go in a pantheon) and the bosses that gave you the most difficulty are tracked and displayed in the "Challenge" menu.

![p1_stats](https://user-images.githubusercontent.com/14790745/177464694-32a143dd-b9a7-4421-a2a5-6a932e42d906.png)


## In-game notifications

Whenever the mod detects that a boss fight has ended, it shows a small notification at the bottom left corner of the game to indicate that the result has been recorded.

![Hollow Knight 2022-06-21 17-48-03 a](https://user-images.githubusercontent.com/14790745/174921467-d980e3f8-1230-45ba-a8b9-acfed7b93d56.png)

You can turn this notification on/off in the mod's menu. See more details below.


## Menu

You can find the mod's menu by pausing the game -> "Options" -> "Mods" -> "GodhomeWinLossTracker".

![Hollow Knight 6_26_2022 9_28_33 AM](https://user-images.githubusercontent.com/14790745/175827615-de51d8c0-44f0-4b66-83ab-aa3168c466d0.png)

* **Show stats in challenge menus**: Display stats (mentioned above) in "Challenge" menus in Hall of Gods and pantheons. "On" by default. Turn this off will return to the normal "Challenge" menus.

* **Notify win/loss**: Display an in-game notification whenever the mod detects the result of boss fights. "On" by default. This helps you to confirm that the detections are working properly. Turn this off to prevent the notifications from showing up.

* **Notify exports**: Display an in-game notification whenever stats have been exported. "Off" by default. Turn this on will you to confirm the filename and the successfulness of expoerts.

* **Auto export stats**: See more details below.

* **Export stats now**: See more details below.


## Export your stats for data analysis

**Where to find**: All stats can be found in the `GodhomeWinLossTracker` subfolder in your game's save folder. On a PC, you can press `Windows + R`, put in `"%AppData%/../LocalLow/Team Cherry/Hollow Knight/GodhomeWinLossTracker"`, and then press `ENTER`.

**Export as TSV**: While playing (i.e. a game save is loaded), select **Export stats now** in the mod's menu. This will write stats into `Export.SaveX.txt` in TSV format (X being the current game slot ID, from 1 to 4). The exported file can then be imported into spreadsheet apps for data analysis ([an example](https://docs.google.com/spreadsheets/d/1_hglw_48YHSVsaKsA3nuqnbMoC0DbbKKl-uB-i44FbM/edit?usp=sharing) data analysis spreadsheet).

**Automatic export when saving games**: You can turn on **Auto export stats** in the mod's menu. This makes the mod export every time the game is being saved. This helps if you plan to constantly pull stats into another app.
