resource_manifest_version '44febabe-d386-4d18-afbe-5e627f4af937'
name 'CameraTech'
description 'ANPR Camera Technology System'
author 'Albo1125 (https://www.youtube.com/albo1125)'
version 'v1.3.0'
url 'https://github.com/Albo1125/CameraTech'
files {
	'Newtonsoft.Json.dll',
	'anprvehicles.txt',
	'fixedanprcameras.json'
}
--server_script '@mysql-async/lib/MySQL.lua'
server_script "vars.lua"
server_script 'sv_CameraTech.lua'
client_script 'CameraTech.net.dll'