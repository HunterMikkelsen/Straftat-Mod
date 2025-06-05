# Unrandomizer

A [STRAFTAT](https://store.steampowered.com/app/2386720/STRAFTAT/) mod that extends the games gun randomization mode, built for [BepInEx](https://github.com/BepInEx/BepInEx).

## Current Features

### Weapon selection

- The host is able to select which weapons will spawn. This can be changed in-game using the [BepInEx Configuration Manager](https://github.com/BepInEx/BepInEx.ConfigurationManager).

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