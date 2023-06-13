# SimpleUtilities
## Provides simple features for your server.

## Features:
- Welcome message,
- Cassie announcement when Chaos Insurgency spawns,
- Chaos Insurgency spawn on round start instead of Facility Guards,
- Auto Friendly Fire when the round ends,
- Cuffed Nine-Tailed Fox / Chaos Insurgency changes team upon "escape".

More to come!

## Install
### LocalAdmin Install:
To install the plugin via LocalAdmin, boot up your server and type ```p install omgiamhungarian/SimpleUtilities```.

### Manual Install:
1. Download the latest released SimpleUtilities.dll from [releases](https://github.com/omgiamhungarian/SimpleUtilities/releases/).
2. Move the SimpleUtilities.dll to ```PluginAPI/plugins/(server_port)```.
3. You are done. Restart your server.

## Default Configuration file

```yml
# Whether or not the plugin is enabled.
is_enabled: true
# Whether or not to show debug messages.
debug: false
# Welcome message which is displayed for the player. (Leave it empty to disable.)
welcome_message: <color=red>Welcome to the server!</color> <color=blue>Please read our rules.</color>
# Welcome message duration.
welcome_message_time: 7
# CASSIE announcement when Chaos Insurgency spawns. Leave it empty to disable. (Please note that you can only use CASSIE approved words.)
cassie_message: Warning Chaos Insurgency has been spotted
# Whether or not to play the announcement's sound effect.
cassie_noise: true
# Whether or not CASSIE should display the text of the announcement.
cassie_text: true
# Chance (1-100) for Chaos Insurgency to spawn at round start instead of Facility Guards.
chaos_chance: 25
# Whether or not to enable friendly fire when the round ends. (You can change the friendly_fire_multiplier in your config_gameplay.txt)
f_f_on_end: true
# Whether or not cuffed NTF / CI should change teams.
cuffed_change_teams: true
```
