requirePack("scripts.FrameWork.Global.GlobalFunctions");
local ScrollView = requirePack("scripts.FrameWork.UI.ScrollView");
local LayoutGroup = requirePack("scripts.FrameWork.UI.LayoutGroup");

local CellBase = class("CellBase",function()
    return cc.Node:create(); 
end);
CellBase.EventType = {
}
g_tConfigTable.CREATE_NEW(CellBase);

function CellBase:ctor()
    self:setContentSize(cc.size(1046,137));
    local bg = cc.Sprite:create(g_tConfigTable.sTaskpath.."bgimg/spCellBg.png");
    self:addChild(bg);
end

function CellBase:UpdateByData(data)

end

return CellBase;