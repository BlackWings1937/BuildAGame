requirePack("scripts.FrameWork.Global.GlobalFunctions");
local UI = requirePack("scripts.FrameWork.UI.UI");
local SpriteUtil = requirePack("scripts.FrameWork.Util.SpriteUtil");

local DragOut = class("Toggle",function()
    return UI.new(); 
end);
DragOut.EventType = {
    ["Click"] = "Click",
}
g_tConfigTable.CREATE_NEW(DragOut);
------------------对外接口-----------------------
function DragOut:Init(spNormal)
    UI.Init(self);
    self:addChild(spNormal);
end

return DragOut;