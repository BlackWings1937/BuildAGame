
local BaseHandle = requirePack("scripts.Messages.BaseHandle");
requirePack("scripts.FrameWork.Global.GlobalFunctions");
local HEART_BEAT = class("HEART_BEAT",function() 
    return BaseHandle.new();
end);
g_tConfigTable.CREATE_NEW(HEART_BEAT);

function HEART_BEAT:Handle(ms)
    self:GetLinkerManager():SMHeartBeatComplie();
end

return HEART_BEAT;