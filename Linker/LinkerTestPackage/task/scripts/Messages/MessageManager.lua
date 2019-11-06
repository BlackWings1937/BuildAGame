requirePack("scripts.FrameWork.Global.GlobalFunctions");
local MessageManager = class("MessageManager");
g_tConfigTable.CREATE_NEW(MessageManager);

MessageManager.STR_MN_RELOAD_RES_COMPLIE = "RELOAD_RES_COMPLIE";
MessageManager.STR_MN_PLAY_STATUE_CHANGE = "PLAY_STATUE_CHANGE";
MessageManager.STR_MN_LOAD_RES_STATUE_UPDATE = "LOAD_RES_STATUE_UPDATE";
MessageManager.STR_MN_CELL_HELLO = "CELL_HELLO";
MessageManager.STR_MN_LINKER_HELLO = "LINKER_HELLO";
function MessageManager:ctor()
    self.linkerManager_ = nil;
end

-- ----- 对外接口 -----

function MessageManager:SetLinkerManager(v)
    if v~= nil then 
        self.linkerManager_ = v;
    end
end

function MessageManager:HandleMessage(m)
    local handle = requirePack("scripts.Messages."..m.EventName).new();
    if handle == nil then 
        print("Error:undefine message:");
        dump(m);
        handle = requirePack("scripts.Messages.DEFAULT_HANDLE").new();-- 忘记写 new
    end
    handle:SetLinkerManager( self.linkerManager_);
    handle:Handle(m);
end


return MessageManager;

--[[
    public const string STR_MN_RELOAD_RES = "RELOAD_RES";
    public const string STR_MN_RELOAD_RES_COMPLIE = "RELOAD_RES_COMPLIE";
EventName

]]--