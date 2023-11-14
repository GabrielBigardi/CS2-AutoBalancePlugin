# CS2-AutoBalancePlugin
 Auto Balance Plugin for CS2 using CounterStrikeSharp


## How to change the config file
1 - Run the server with the plugin, it will auto-generate a config.  
2 - Open ``addons/counterstrikesharp/configs/plugins/AutoBalancePlugin/AutoBalancePlugin.json``  
3 - Change it however you want.    

## Available modes
Currently there is two balancing modes, they are:
* Default mode: grabs the team with the most players and change them to the other team
.
* Scramble mode: every player is randomly put in some team, while still auto-balancing, good for retake servers and so on.

## Changing modes
1 - Open the config file.  
2 - Change the variable "ScrambleMode" to 0 for default mode or 1 for scramble mode.

## Changing default colors
1 - Open the config file.  
2 - Change the variables "PluginNameColor" and "PluginMessageColor" to one of the available ones, here's the available color codes:
* Default = '\u0001';
* White = '\u0001';
* Darkred = '\u0002';
* Green = '\u0004';
* LightYellow = '\u0003';
* LightBlue = '\u0003';
* Olive = '\u0005';
* Lime = '\u0006';
* Red = '\a';
* Purple = '\u0003';
* Grey = '\b';
* Yellow = '\t';
* Gold = '\u0010';
* Silver = '\n';
* Blue = '\v';
* DarkBlue = '\f';
* BlueGrey = '\r';
* Magenta = '\u000E';
* LightRed = '\u000F';

## Contact
**Discord**: *Gabriel Bigardi#2292*  
**Twitter**: *@BigardiGabriel*  
**Email**: *gabrielbigardi@hotmail.com*

## License
AutoBalancePlugin is licensed under the MIT license, so you can comfortably use it in commercial applications.