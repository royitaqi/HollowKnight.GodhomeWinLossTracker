# HollowKnight.GodhomeWinLossTracker

A **mod** for the game **Hollow Knight**.

The mod's **goal** is to **improve Godhome boss fight training experience by stats tracking and data analysis**.

## How to reach out?

If you have any questions, suggestions or bugs, please feel free to reach out.

Either create an issue here in github.com, or join discord server: https://discord.gg/vqKTF26VGX

## Features and screenshots

All in Godhome (both Hall of Gods and pantheons):
* [**Win/loss and hit stats tracking**](#winloss-and-hit-stats-tracking) happens automatically during boss fights.
  * Win/loss stats
    <kbd>![win_loss_stats](https://user-images.githubusercontent.com/14790745/178205066-a55fed00-2781-4d1c-add4-19cb3e1befc1.png)</kbd>

  * Hit stats
    <kbd>![hit_stats](https://user-images.githubusercontent.com/14790745/178205079-78a5285e-7a53-42a7-81f1-18fb1c7cf51d.png)</kbd>

* [**Localized stats display**](#localized-stats-display) are available in "Challenge" menu.
  <kbd>![hog_stats](https://user-images.githubusercontent.com/14790745/177464675-5db99441-65d8-4602-b30c-f38993e9f92d.png)</kbd>

  <kbd>![p1_stats](https://user-images.githubusercontent.com/14790745/177464694-32a143dd-b9a7-4421-a2a5-6a932e42d906.png)</kbd>

* [**Data analysis**](#data-analysis) can be done by exporting stats into google spreadsheets.
  * [**Win/loss analyzer**](https://docs.google.com/spreadsheets/d/1_hglw_48YHSVsaKsA3nuqnbMoC0DbbKKl-uB-i44FbM/edit#gid=668467742)
    <kbd>![stats_1](https://user-images.githubusercontent.com/14790745/178206742-7ddff7cc-1ab5-4fbb-8bfb-533627cae93c.png)</kbd>

    <kbd>![stats_2](https://user-images.githubusercontent.com/14790745/178206748-c86a4ce6-734c-4073-a5d6-8aa144b829e9.png)</kbd>

    <kbd>![stats_3](https://user-images.githubusercontent.com/14790745/178206759-2d83ce27-359a-4d2c-b587-c8c7d37a47aa.png)</kbd>

  * [**Hit analyzer**](https://docs.google.com/spreadsheets/d/1xsUuBEHeK0b4EGq_4CI5zIcj9u66XZsUfC3Oq_eTMAw/edit#gid=668467742)
    <kbd>![stats_4](https://user-images.githubusercontent.com/14790745/178206768-b99b3525-9ed5-406a-bbbc-be18495b0a71.png)</kbd>


Other features:
* [**In-game notifications**](#in-game-notifications)
  <kbd>![Hollow Knight 2022-06-21 17-48-03 a](https://user-images.githubusercontent.com/14790745/174921467-d980e3f8-1230-45ba-a8b9-acfed7b93d56.png)</kbd>

* [**Mod menu**](#mod-menu)
  <kbd>![Hollow Knight 6_26_2022 9_28_33 AM](https://user-images.githubusercontent.com/14790745/175827615-de51d8c0-44f0-4b66-83ab-aa3168c466d0.png)</kbd>


---


## Win/loss and hit stats tracking

The mod automatically tracks your boss fights' win/loss and hit stats.

**Win/loss records** (one line of record per boss fight):
* Timestamp (of the record)
* Sequence name (HoG, P1, P2, P3, P4, P5)
* Boss name
* Scene name (some boss has different arenas for Attuned vs. Ascneded+)
* Wins (0 or 1)
* Losses (0 or 1)
* Heals
* Heal amount
* Hits (the number of hits your hero took)
* Hit amount (the amount of masks your hero lost)
* Boss HP (0~1, the amount of remaining HP the boss has at the end of the fight)
* Fight length (in milliseconds, from entering to leaving boss scene)
* See [example records](https://docs.google.com/spreadsheets/d/1_hglw_48YHSVsaKsA3nuqnbMoC0DbbKKl-uB-i44FbM/edit?usp=sharing)

**Hit records** (one line of record per hit your hero took):
* Timestamp
* Sequence name
* Boss name
* Scene name
* Hero status (standing/walking, airborne, dashing, etc)
* Hero health before took hit (in number of masks)
* Damage amount (of the hit)
* Boss HP (0~1, at the time of the hit)
* Boss state (the action the boss was performing at the time of the hit; can usually tell what attack the boss was using)
* Time into fight (in milliseconds, from entering boss scene to the time of the hit)
* See [example records](https://docs.google.com/spreadsheets/d/1xsUuBEHeK0b4EGq_4CI5zIcj9u66XZsUfC3Oq_eTMAw/edit?usp=sharing)


## Localized stats display

In Hall of Gods, different boss variants (dream version or different forms) are tracked separately. For the same boss variant/form, if it has different arenas for Attuned and Ascended/Radiant, they are tracked separately, too. Stats are displayed in the "Challenge" menu.

In pantheons, the number of runs, your PB (personal best, i.e. how far you go in a pantheon) and the bosses that gave you the most difficulty are tracked and displayed in the "Challenge" menu.


## Data analysis

**Step 1: Export stats**

In the **mod's menu** (see below), either choose **Auto export stats** or **Export stats now**.

**Step 2: Find the export**

All stats can be found in the `GodhomeWinLossTracker` subfolder in your game's save folder.
* On a PC: `"%AppData%/../LocalLow/Team Cherry/Hollow Knight/GodhomeWinLossTracker"`

The exported files are:
* Win/loss stats: `Export.WinLoss.Save1.txt` ~ `Export.WinLoss.Save4.txt`
* Hit stats: `Export.Hit.Save1.txt` ~ `Export.Hit.Save4.txt`

**Step 3: Clone data analysis sheet**

Example data analysis sheets are available:
* [Win/loss analyzer](https://docs.google.com/spreadsheets/d/1_hglw_48YHSVsaKsA3nuqnbMoC0DbbKKl-uB-i44FbM/edit#gid=668467742)
* [Hit analyzer](https://docs.google.com/spreadsheets/d/1xsUuBEHeK0b4EGq_4CI5zIcj9u66XZsUfC3Oq_eTMAw/edit#gid=668467742)

**Step 4: Import exported files into data analysis sheets**

In google spreadsheet, you can do "(switch to first tab in sheet -> File -> Import -> Upload -> (choose/drag your exported file) -> Replacec current sheet -> Import data".

**Step 5: View pre-configured analysis**

In google spreadsheet, switch to any tab that has a "👁" symbol in their name to see pre-configured analysis.


## In-game notifications

Whenever the mod detects a boss fight's win/loss event, it shows a small notification at the bottom left corner of the game to indicate that the result has been recorded.

You can turn this notification on/off in the mod's menu. See more details below.


## Mod menu

You can find the mod's menu by pausing the game -> "Options" -> "Mods" -> "GodhomeWinLossTracker".

* **Show stats in challenge menus**: Display stats (mentioned above) in "Challenge" menus in Hall of Gods and pantheons. "On" by default. Turn this off will return to the normal "Challenge" menus.

* **Notify win/loss**: Display an in-game notification whenever the mod detects the result of boss fights. "On" by default. This helps you to confirm that the detections are working properly. Turn this off to prevent the notifications from showing up.

* **Notify exports**: Display an in-game notification whenever stats have been exported. "Off" by default. Turn this on will you to confirm the filename and the successfulness of expoerts.

* **Export stats now**: While playing (i.e. when a game save is loaded), this will write stats into `Export.WinLoss.SaveX.txt` and `Export.Hit.SaveX.txt` in TSV format. They each contain win/loss and hit records. The exported files can then be imported into spreadsheet apps for data analysis (see examples above).

* **Auto export stats**: This makes the mod export every time the game is being saved. This helps if you plan to constantly pull stats into data analysis.
