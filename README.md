# UnRandomizer

A [STRAFTAT](https://store.steampowered.com/app/2386720/STRAFTAT/) mod that extends the games gun randomization mode, built for [BepInEx](https://github.com/BepInEx/BepInEx).

## Current Features

### Per weapon selection

The host is able to select which weapons will spawn. This can be changed in-game using the [BepInEx Configuration Manager](https://github.com/BepInEx/BepInEx.ConfigurationManager).

### Additional Ammo

Each gun can have additional ammo included in the magazine (0-1000). This change is applied universally to every gun including throwables **Be warned grenades do not leave your hand when you have multiple of them**

### Unlimited ammo (Host only)
This option applies to the host only and gives every weapon unlimited ammo. (Maybe if everyone in the lobby has this mod it will work differently ?)

##  Dependencies
There aren't any hard dependencies but it is strongly recommended to also have [BepInEx Configuration Manager](https://github.com/BepInEx/BepInEx.ConfigurationManager).

## How to make changes without the Configuration Manager
Locate the CFG at STRAFTAT\BepInEx\config\ChoccyMewks-Unrandomizer.cfg
Find the weapon you want to change and set the value to **true** for weapons to spawn and **false** to disable the weapon spawning.