
## v1.1.1
    - Added slider to adjust weapon respawn time.
    - Added section to enable random weapon respawn time.
        - This allows the host to set the min and maximum number of seconds it will take for a weapon to respawn. When a weapon is respawning it will pick a random number of seconds between the min and max.
        - For example if the slider's are set to Min: 0 and Max: 60 a random number between 0-60 will be selected and it could take 14.2 seconds or 59.2 seconds for a weapon to spawn.
    - Fixed Blank State continuing to spawn even though it was disabled (Thank you ImmortalChickens and Kevnator for finding this).

## v1.0.1

    Initial Release
    - Added options to enable / disable weapons from spawning.
    - Added option for additional ammo to appear when picking up new guns (0-1000)
    - Added option for host to have unlimited ammo.
    - Removed unlimited ammo option
    - Removed additional ammo slider
    
    These where removed because Straftat doesn't currently have a method to filter mod using players from the vanilla users in public lobbies. They might see a return in the future once better modding support is added.