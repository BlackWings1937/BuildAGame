
local BaseHandle = requirePack("scripts.Messages.BaseHandle");
requirePack("scripts.FrameWork.Global.GlobalFunctions");
local CELL_HELLO = class("CELL_HELLO",function() 
    return BaseHandle.new();
end);
g_tConfigTable.CREATE_NEW(CELL_HELLO);

function CELL_HELLO:Handle(ms)
    self:GetLinkerManager():SMHelloLinker();
end

return CELL_HELLO;