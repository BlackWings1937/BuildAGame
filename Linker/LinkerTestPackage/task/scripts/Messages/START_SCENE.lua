
local BaseHandle = requirePack("scripts.Messages.BaseHandle");
requirePack("scripts.FrameWork.Global.GlobalFunctions");
local START_SCENE = class("START_SCENE",function() 
    return BaseHandle.new();
end);
g_tConfigTable.CREATE_NEW(START_SCENE);

function START_SCENE:Handle(ms)
    print("SceneStartHandle");
    dump(ms);
    local IsReloadDragonBoneData = ms.IsReloadDragonBoneData;
    self:GetLinkerManager():StartScene(IsReloadDragonBoneData);
end

return START_SCENE;