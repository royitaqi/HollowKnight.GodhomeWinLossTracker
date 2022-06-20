# HollowKnight.GodhomeWinLossTracker

GodhomeWinLossTracker is a mod for the game Hollow Knight. Its goal is to improve boss fight training experience in Godhome by tracking per-boss win/loss counts for Hall of Gods and pantheons.

The mod is fully automatic. It recognizes boss fights, understands which boss is involved, detects win/loss events and tracks running stats.


## Win/Loss events

Whenever the mod detects a win/loss, it shows a small notification at the bottom left corner of the game to indicate that the event has been recorded.

![example](https://user-images.githubusercontent.com/14790745/174503620-b0abda40-e43f-4e45-bbdb-0d59eb18007d.png)


## Find your win/loss stats

When "Save & Quit", the mod automatically saves stats into the `userX.modded.json` files in your save folder (`X = 1..4`).

![example2](https://user-images.githubusercontent.com/14790745/174503737-971c36de-980c-406c-b050-cae6fba8f90f.png)


## Turn mod on/off

The mod can be turned on/off from the "Mods" menu.


## Known bugs / limitations

* Bug: In some situations, win/loss detection can be wrong.
  * Dying in a fight after killing the boss might be detected as a win. E.g. source of damanage: hazards, minions, projectiles.
  * When a boss fight contains multiple bosses, win might be detected pre-maturely and multiple times. E.g. Oro & Mato, Vengefly Kings, Oblobbles, Mantis Lords/SoB, God Tamer, Watcher Knights.
* Limitation: Boss names are displayed in English. No localization yet.
