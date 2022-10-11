fx_version 'cerulean'
games { 'gta5' }

name 'CameraTech'
description 'ANPR Camera Technology System'
author 'Albo1125 (https://www.youtube.com/albo1125)'
version 'v1.5.0'
url 'https://github.com/Albo1125/CameraTech'
files {
	'Newtonsoft.Json.dll',
	'anprvehicles.json',
	'fixedanprcameras.json'
}
--server_script '@mysql-async/lib/MySQL.lua'
server_script "vars.lua"
server_script 'sv_CameraTech.lua'
client_script 'CameraTech.net.dll'