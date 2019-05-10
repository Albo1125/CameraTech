local useWhitelist = false
local authorizedIdentifiers = {
	--"steam:1234",
	-- Add more identifiers here
	
}

function isAuthorized(player)
	if not useWhitelist then return true end
    for i,id in ipairs(authorizedIdentifiers) do
        for x,pid in ipairs(GetPlayerIdentifiers(player)) do
            if pid == id then
                return true
            end
        end
    end
    return false
end