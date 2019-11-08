
local BaseHandle = requirePack("scripts.Messages.BaseHandle");
requirePack("scripts.FrameWork.Global.GlobalFunctions");
local STOP_SCENE = class("STOP_SCENE",function() 
    return BaseHandle.new();
end);
g_tConfigTable.CREATE_NEW(STOP_SCENE);

function STOP_SCENE:Handle(ms)
    local sceneId = ms.SceneID;
    self:GetLinkerManager():StopScene();
end

return STOP_SCENE; 