# HollowKnight.GodhomeWinLossTracker

GodhomeWinLossTracker is a mod for the game Hollow Knight. It's goal is to improve boss fight training experience in Godhome by tracking per-boss win/loss counts for Hall of Gods and pantheons.

The mod is fully automated. It recognizes boss fights, understands which boss is involved, detects win/loss events and tracks running stats.


## Win/Loss events

Whenever the mod detects a win/loss, it shows a small notification at the bottom left corner of the game to indicate that the event has been recorded.

![example](https://user-images.githubusercontent.com/14790745/174503620-b0abda40-e43f-4e45-bbdb-0d59eb18007d.png)


## Find your win/loss stats

When you "Save & Quite", the mod automatically saves stats into the `userX.modded.json` files (X = 1-4) in your save folder.

![example2](https://user-images.githubusercontent.com/14790745/174503737-971c36de-980c-406c-b050-cae6fba8f90f.png)


## Turn mod on/off

The mod can be turned on/off from the "Mods" menu.


## Known bugs / limitations

* Cannot distinguish between p1-5. Planned to support.
* Boss names (both displayed and saved) are from scene names. They can be different from in-game boss names.
  * E.g. Soul Warrior reads "Mage Knight", Oro & Mato reads "Nailmasters", Hornet Protector reads "Hornet 1", etc
