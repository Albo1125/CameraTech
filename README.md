# CameraTech
CameraTech is a resource for FiveM by Albo1125 that provides for an indepth Automatic Number Plate Recognition (ANPR/ALPR) system simulation.

[![CameraTechVideo](https://i.imgur.com/KXHgPBk.jpg)](https://youtu.be/LLJ7NG_gUE4)

## Installation & Usage
1. Download the latest [Release](https://github.com/Albo1125/CameraTech/releases).
2. Unzip the CameraTech folder into your resources folder on your FiveM server.
3. Create a file called `fixedanprcameras.json` in the CameraTech folder. You can usually simply rename the `fixedanprcameras.default.json` to do so. Optionally, add or remove fixed ANPR cameras on the map, following the format of the provided default file.
4. Add the following to your server.cfg file:
```text
start CameraTech
```
5. Install the [InteractSound](https://github.com/plunkettscott/interact-sound) resource. Add sound files of your choice to InteractSound called `FixedANPR.ogg` and `VehicleANPR.ogg`. Examples have been included in this release.
6. Optionally, create a new file called `anprvehicles.json` in the CameraTech folder. See `Customising your ANPR vehicles` below for further guidance.
7. Optionally, enable the ANPR whitelist in `vars.lua` and add identifiers. This only affects commands for use of police.
8. Optionally, in `sv_CameraTech.lua`, uncomment lines 5 to 7 and 16 to 18 and in `fxmanifest.lua` uncomment line 14. This will make the script insert a new row into a MySQL database whenever a fixed ANPR hit comes in (e.g. for web-based control purposes).
9. Optionally, in `fxmanifest.lua`, change the `ForceFocusedAnpr` setting to `true`. This means users will not be able to receive fixed ANPR alerts unless they have a plate focused. This is useful if you want your control room to initially circulate ANPR hits to units before the units see the hits themselves.

## Customising your vehicles
You have two options for customising your ANPR vehicles.

* Firstly, you can add a `anprvehicles.json` file to the CameraTech folder. If this file is present, it will always be used. See `anprvehicles.default.json` for an example.
* Secondly, you can load your `anprvehicles.json` contents dynamically. If no `anprvehicles.json` file is present in the resource folder, the client will trigger a server event `CameraTech:GetANPRModelsJsonString()` when they use the /anpr command for the first time. The client will listen for a `CameraTech:ANPRModelsJsonString(jsonString, runAnprCommandEnable)` event from the server. `jsonString` should be a valid JSON string in the `anprvehicles.json` format, `runAnprCommandEnable` should be true if the event was triggered by a specific player using the ANPR command, false otherwise. The `CameraTech:ANPRModelsJsonString(jsonString, runAnprCommandEnable)` event can also be triggered periodically by the server to update the ANPR vehicle models for all players (-1), make sure `runAnprCommandEnable` is set to false in that case.

You can add as many entries to the root array as you like. If a vehicle model appears in the file multiple times, all ANPR Access Types will be added on top of the others. JSON reference is as follows.
### ANPRVehicle JSON Object Entry
* "ModelName" string indicating the ingame Model Name of the vehicle.
* "ANPRAccessType" which should be one of: "full", "fixed only", "vehicle only".

## Commands
* /anpr - Toggles the ANPR interface if you are in a vehicle that has access to ANPR (Whitelisted).
* /fixedanpr - Toggles ANPR alerts from fixed ANPR cameras on the map (Whitelisted).
* /vehicleanpr (alias /vehanpr) - Toggles ANPR alerts from vehicle ANPR cameras (Whitelisted).
* /readplate (alias /rp) - Reads the plate of the vehicle in front of you and puts it in chat.
* /checkplate PLATE - Returns the ANPR markers currently active for the specified plate.
* /focusanpr PLATE - Only displays fixed ANPR alerts for the specified PLATE and automatically draws a route if any hit comes in. Leave PLATE blank to unfocus or to focus the plate that last triggered a fixed ANPR alert.
* /setplateinfo PLATE;INFO - Adds ANPR markers (INFO) for the specified plate. Leave INFO blank to remove markers. Example: /setplateinfo AB12CDE;STOLEN
* /setvehinfo INFO - Adds ANPR markers (INFO) for the plate of the vehicle you're currently in. Leave INFO blank to remove markers. Example: /setvehinfo STOLEN

## Improvements & Licencing
Please view the license. Improvements and new feature additions are very welcome, please feel free to create a pull request. As a guideline, please do not release separate versions with minor modifications, but contribute to this repository directly. However, if you really do wish to release modified versions of my work, permission & proper credit is always required and you should always link back to this original source and respect the licence.

## Libraries used (many thanks to their authors)
* [CitizenFX.Core.Client](https://github.com/citizenfx/fivem)
* [Newtonsoft.Json](https://www.newtonsoft.com/json)

## Video
[Click here (please note setup instructions have changed slightly)](https://youtu.be/LLJ7NG_gUE4)

## Screenshots
![CameraTech](https://i.imgur.com/KlhjVos.jpg)
