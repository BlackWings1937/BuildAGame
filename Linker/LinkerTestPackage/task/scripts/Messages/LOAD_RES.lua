
local BaseHandle = requirePack("scripts.Messages.BaseHandle");
requirePack("scripts.FrameWork.Global.GlobalFunctions");
local LOAD_RES = class("LOAD_RES",function() 
    return BaseHandle.new();
end);
g_tConfigTable.CREATE_NEW(LOAD_RES);

function LOAD_RES:Handle(ms)
    self:GetLinkerManager():TestPostDownloadRes();
end

return LOAD_RES;