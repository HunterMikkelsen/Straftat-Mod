# Unrandomizer

A [STRAFTAT](https://store.steampowered.com/app/2386720/STRAFTAT/) mod that extends the games gun randomization mode, built for [BepInEx](https://github.com/BepInEx/BepInEx).

## Current Features

### Weapon selection

- The host is able to select which weapons will spawn. This can be changed in-game using the [BepInEx Configuration Manager](https://github.com/BepInEx/BepInEx.ConfigurationManager).
- There are two buttons that allow you to quickly enable or disable the entire weapon list

### Control of weapon respawn time

#### Universal weapon respawn time
- The host can control the specific amount of time each pedestal will take to spawn a new weapon.

#### Random respawn time
- If the checkbox for "Toggle random respawn time" is set to true it will choose a random amount of time for each pedestal to respawn a weapon.
- The Minimum and Maximum time sliders dictate the range of time. If the min slider is set to 0 seconds and the max is set to 60 seconds a random number between 0 and 60 is selected.


##  Dependencies
There are not any hard dependencies but it is strongly recommended to also have [BepInEx Configuration Manager](https://github.com/BepInEx/BepInEx.ConfigurationManager).

## How to make changes without the Configuration Manager

### Without the Configuration Manager
1.  Locate the CFG at **STRAFTAT\BepInEx\config\ChoccyMewks-Unrandomizer.cfg**
2.  Find the weapon you want to change and set the value to **true** for weapons to spawn and **false** to disable the weapon spawning.

For example you might want to make the Bayshore the only weapon that spawns, to do this turn all other weapons to false and the Bayshore to true.
This will prevent all weapons except the Bayshore from spawning.

### With the Configuration Manager

1. When in game press F1 to open the configuration menu.
2. Select which weapons you want to enable /disable by clicking the checkbox next to their name.

**Note:** There are convenient toggles that act like buttons at the top of the list to disable / enable all the weapons at once.

## Got an idea or a bug?
Message [ChoccyMewk](https://discord.gg/dNnYrC6PfB) on discord.