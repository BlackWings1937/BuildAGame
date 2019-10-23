
local BaseHandle = requirePack("scripts.Messages.BaseHandle");
requirePack("scripts.FrameWork.Global.GlobalFunctions");
local RELOAD_RES = class("RELOAD_RES",function() 
    return BaseHandle.new();
end);
g_tConfigTable.CREATE_NEW(RELOAD_RES);

function RELOAD_RES:Handle(ms)
    self:GetLinkerManager():ReloadRes();
end

return RELOAD_RES;