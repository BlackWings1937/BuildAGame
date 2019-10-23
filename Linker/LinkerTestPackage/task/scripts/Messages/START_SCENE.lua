
local BaseHandle = requirePack("scripts.Messages.BaseHandle");
requirePack("scripts.FrameWork.Global.GlobalFunctions");
local START_SCENE = class("START_SCENE",function() 
    return BaseHandle.new();
end);
g_tConfigTable.CREATE_NEW(START_SCENE);

function START_SCENE:Handle(ms)
    local sceneId = ms.SceneID;
    self:GetLinkerManager():StartSceneBySceneID(sceneId);
end

return START_SCENE;