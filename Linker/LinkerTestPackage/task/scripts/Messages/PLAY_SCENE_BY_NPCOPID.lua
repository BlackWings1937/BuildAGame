
local BaseHandle = requirePack("scripts.Messages.BaseHandle");
requirePack("scripts.FrameWork.Global.GlobalFunctions");
local PLAY_SCENE_BY_NPCOPID = class("PLAY_SCENE_BY_NPCOPID",function() 
    return BaseHandle.new();
end);
g_tConfigTable.CREATE_NEW(PLAY_SCENE_BY_NPCOPID);

function PLAY_SCENE_BY_NPCOPID:Handle(ms)
    self:GetLinkerManager():PlayPlotBySceneIDAndNpcNameAndStartIndex(ms.SceneID,ms.NpcName,ms.OptionIndex);
end

return PLAY_SCENE_BY_NPCOPID;