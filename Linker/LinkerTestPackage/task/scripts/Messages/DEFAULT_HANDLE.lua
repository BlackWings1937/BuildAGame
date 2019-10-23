
local BaseHandle = requirePack("scripts.Messages.BaseHandle");
requirePack("scripts.FrameWork.Global.GlobalFunctions");
local DEFAULT_HANDLE = class("DEFAULT_HANDLE",function() 
    return BaseHandle.new();
end);
g_tConfigTable.CREATE_NEW(DEFAULT_HANDLE);

function DEFAULT_HANDLE:Handle(ms)
end

return DEFAULT_HANDLE;