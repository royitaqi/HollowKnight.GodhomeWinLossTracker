# HollowKnight.GodhomeWinLossTracker

GodhomeWinLossTracker is a mod for the game Hollow Knight. Its goal is to improve boss fight training experience in Godhome by tracking per-boss win/loss counts for Hall of Gods and pantheons.

The mod is fully automatic. It recognizes boss fights, understands which boss is involved, detects win/loss events and tracks running stats.


## Win/Loss events

Whenever the mod detects a win/loss, it shows a small notification at the bottom left corner of the game to indicate that the event has been recorded.

![Hollow Knight 2022-06-21 17-48-03 a](https://user-images.githubusercontent.com/14790745/174921467-d980e3f8-1230-45ba-a8b9-acfed7b93d56.png)


## Find (and update) your stats

When "Save & Quit", the mod automatically saves stats into files `Data.SaveX.json` (X=1..4) in the game's save folder.

**To find on a PC:** Press `Windows + R`, put in `"%AppData%/../LocalLow/Team Cherry/Hollow Knight/GodhomeWinLossTracker"`, press `ENTER`).

**To update the stats:** First "Save & Quit", then modify the `Data.SaveX.json` files and save the files. The updated stats will be automatically loaded the next time the same game save is loaded.


## Automatic (local) backup of your stats

Whenever a game save is loaded, its corresponding `Data.SaveX.json` file is backed up into the `backup` subfolder.


## Export your stats in TSV format

The mod supports exporting your stats into tab-separated values (TSV) format for further data analysis.

**How to:** When a game save is loaded in the game, go into the "Mods" menu, select "GodhomeWinLossTracker", then "Export stats as TSV". A notification will show up when the export is success.

![Hollow Knight 2022-06-21 17-48-32 a](https://user-images.githubusercontent.com/14790745/174921490-2089c19b-f5cb-420e-b2df-724bf16e68ed.png)

This will generate `GodhomeWinLossTracker/Export.SaveX.txt` (in TSV format). This file can then be viewed directly in any text editing app or be imported into spreadsheet apps for data analysis ([an example](https://docs.google.com/spreadsheets/d/1_hglw_48YHSVsaKsA3nuqnbMoC0DbbKKl-uB-i44FbM/edit?usp=sharing) data analysis spreadsheet).


## Turn mod on/off

The mod can be turned on/off from the "Mods" menu.


## Known bugs / limitations

* Bug: In some situations, win/loss detection can be wrong.
  * Dying in a fight after killing the boss might be detected as a win. E.g. source of damanage: hazards, minions, projectiles.
* Limitation: Boss names are displayed in English. No localization yet.
