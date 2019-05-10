local plateinfos = {}
local mysqlready = false

-- Uncomment this if using MySQL async to insert ANPR hits into the database.
--MySQL.ready(function() 
--	mysqlready = true
--end)

RegisterServerEvent("CameraTech:FixedANPRAlert")
AddEventHandler('CameraTech:FixedANPRAlert', function(colour, model, anprname, dir, plate)
	local marker = plateinfos[plate]
	if marker ~= nil then
		TriggerClientEvent("CameraTech:ClFixedANPRAlert", -1, colour, model, anprname, dir, plate)
		if mysqlready == true then
			-- Uncomment this if using MySQL async to insert ANPR hits into the database.
			--local timenow = os.time(os.date("!*t"))
			--local logquery = MySQL.Async.execute("INSERT into anpr (colour, model, anprname, dir, plate, marker, time) VALUES (@colour, @model, @anprname, @dir, @plate, @marker, @time)", 
			--{['@colour'] = colour, ['@model'] = model, ['@anprname'] = anprname, ['@dir'] = dir, ['@plate'] = plate, ['@marker'] = marker, ['@time'] = timenow})
		end
	end
end)

function anprfunc(source, args, rawCommand)
	if isAuthorized(source) then
		TriggerClientEvent("CameraTech:ToggleVehicleANPR", source)
	else
		print(GetPlayerName(source).. " you are not ANPR trained.")
	end
end

RegisterCommand('anpr', anprfunc, false)

function fixedanprfunc(source, args, rawCommand)
	if isAuthorized(source) then
		TriggerClientEvent("CameraTech:FixedANPRToggle", source)
	else
		print(GetPlayerName(source).. " you are not ANPR trained.")
	end
end

RegisterCommand('fixedanpr', fixedanprfunc, false)

function readplatefunc(source, args, rawCommand)
	TriggerClientEvent("CameraTech:ReadPlateInFront", source)
end

RegisterCommand('readplate', readplatefunc, false)

function checkplatefunc(source, args, rawCommand)
	local plate = string.upper(table.concat(args, " "))
	plate = string.gsub(plate, "%s+", "")
	local markers = plateinfos[plate]
	if markers ~= nil then
		TriggerClientEvent("chatMessage", source, "SYSTEM", {0, 0, 0 }, "Plate: " .. plate .. ". Markers: ".. markers)
	else
		TriggerClientEvent("chatMessage", source, "SYSTEM", {0, 0, 0 }, "Plate: " .. plate .. ". No markers.")
	end	
end

RegisterCommand('checkplate', checkplatefunc, false)

function focusanprfunc(source, args, rawCommand)
	if next(args) == nil then
		TriggerClientEvent("CameraTech:FocusANPR", source, nil)
	else
		local plate = string.upper(table.concat(args, " "))
		plate = string.gsub(plate, "%s+", "")
		TriggerClientEvent("CameraTech:FocusANPR", source, plate)
	end
end

RegisterCommand('focusanpr', focusanprfunc, false)
RegisterCommand('focusplate', focusanprfunc, false)

function anprinterfacefunc(source, args, rawCommand)
	TriggerClientEvent("CameraTech:MasterInterfaceToggle", source)
end

RegisterCommand('anprinterface', anprinterfacefunc, false)

function setplateinfofunc(source, args, rawCommand)	
	local plateinfo = stringsplit(table.concat(args, " "), ";")
	plateinfo[1] = string.upper(plateinfo[1])
	plateinfo[1] = string.gsub(plateinfo[1], "%s+", "")
	if plateinfo[2] ~= nil then
		plateinfos[plateinfo[1]] = plateinfo[2]
		TriggerClientEvent("chatMessage", source, "SYSTEM", {0, 0, 0 }, "Plate: " .. plateinfo[1] .. ". Info: " .. plateinfo[2])
		TriggerClientEvent("CameraTech:SyncPlateInfo", -1, plateinfos)
	elseif plateinfo[1] ~= nil then
		plateinfos = removeKey(plateinfos, plateinfo[1])
		if plateinfos == nil then
			plateinfos = {}
		end
		TriggerClientEvent("chatMessage", source, "SYSTEM", {0, 0, 0 }, "Plate: " .. plateinfo[1] .. ". No info.")
		TriggerClientEvent("CameraTech:SyncPlateInfo", -1, plateinfos)
	end
end

RegisterCommand('setplateinfo', setplateinfofunc, false)

function setvehinfofunc(source, args, rawCommand)	
	if next(args) == nil then
		TriggerClientEvent("CameraTech:ClUpdateVehicleInfo", source, nil)
	else
		local plateinfo = table.concat(args, " ")
		TriggerClientEvent("CameraTech:ClUpdateVehicleInfo", source, plateinfo)
	end
end

RegisterCommand('setvehinfo', setvehinfofunc, false)

RegisterServerEvent("CameraTech:UpdateVehicleInfo")
AddEventHandler('CameraTech:UpdateVehicleInfo', function(plate, info)
	if info ~= nil then
		plateinfos[plate] = info
		TriggerClientEvent("CameraTech:SyncPlateInfo", -1, plateinfos)
	else
		plateinfos = removeKey(plateinfos, plate)
		if plateinfos == nil then
			plateinfos = {}
		end
		TriggerClientEvent("CameraTech:SyncPlateInfo", -1, plateinfos)
	end
end)

print("CameraTech by Albo1125 (FiveM)")

function platesync()
	TriggerClientEvent("CameraTech:SyncPlateInfo", -1, plateinfos)
	SetTimeout(30000, platesync)
end
SetTimeout(1000, platesync)

function stringsplit(inputstr, sep)
    if sep == nil then
        sep = "%s"
    end
    local t={} ; i=1
    for str in string.gmatch(inputstr, "([^"..sep.."]+)") do
        t[i] = str
        i = i + 1
    end
    return t
end

-- Remove key k (and its value) from table t. Return a new (modified) table.
function removeKey(t, k)
	local i = 0
	local keys, values = {},{}
	for k,v in pairs(t) do
		i = i + 1
		keys[i] = k
		values[i] = v
	end
 
	while i>0 do
		if keys[i] == k then
			table.remove(keys, i)
			table.remove(values, i)
			break
		end
		i = i - 1
	end
 
	local a = {}
	for i = 1,#keys do
		a[keys[i]] = values[i]
	end
 
	return a
end