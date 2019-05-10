# CameraTech
CameraTech is a resource for FiveM by Albo1125 that provides for an indepth Automatic Number Plate Recognition (ANPR/ALPR) system simulation.

[![CameraTechVideo](https://i.imgur.com/KXHgPBk.jpg)](https://youtu.be/LLJ7NG_gUE4)

## Installation & Usage
1. Download the latest release.
2. Unzip the CameraTech folder into your resources folder on your FiveM server.
3. Add the following to your server.cfg file:
```text
start CameraTech
```
4. Add (police) vehicle models that are equipped with ANPR to `anprvehicles.txt`, each new model should be on a new line.
5. Optionally, enable the ANPR whitelist in `vars.lua` and add identifiers. This only affects commands for use of police.
6. Optionally, add or remove fixed ANPR cameras on the map in `fixedanprcameras.json`, following the format of the provided file.
7. Optionally, in `sv_CameraTech.lua`, uncomment lines 5 to 7 and 16 to 18 and in `_resource.lua` uncomment line 12. This will make the script insert a new row into a MySQL database whenever a fixed ANPR hit comes in (e.g. for web-based control purposes).

## Commands
* /anpr - Toggles the ANPR interface if you are in a specified vehicle with equipped ANPR (Whitelisted).
* /fixedanpr - Toggles ANPR alerts from fixed ANPR cameras on the map (Whitelisted).
* /readplate - Reads the plate of the vehicle in front of you and puts it in chat.
* /checkplate PLATE - Returns the ANPR markers currently active for the specified plate.
* /focusanpr PLATE - Only displays fixed ANPR alerts for the specified PLATE and automatically draws a route if any hit comes in. Leave PLATE blank to unfocus.
* /setplateinfo PLATE;INFO - Adds ANPR markers (INFO) for the specified plate. Leave INFO blank to remove markers. Example: /setplateinfo AB12CDE;STOLEN
* /setvehinfo INFO - Adds ANPR markers (INFO) for the plate of the vehicle you're currently in. Leave INFO blank to remove markers. Example: /setvehinfo STOLEN
* /anprinterface - Toggles the ANPR interface.

## Improvements & Licencing
Please view the license. Improvements and new feature additions are very welcome, please feel free to create a pull request. As a guideline, please do not release separate versions with minor modifications, but contribute to this repository directly. However, if you really do wish to release modified versions of my work, permission & proper credit is always required and you should always link back to this original source and respect the licence.

## Libraries used (many thanks to their authors)
* [CitizenFX.Core.Client](https://github.com/citizenfx/fivem)
* [Newtonsoft.Json](https://www.newtonsoft.com/json)

## Video
[Click here](https://youtu.be/LLJ7NG_gUE4)

## Screenshots
![CameraTech](https://i.imgur.com/KlhjVos.jpg)
