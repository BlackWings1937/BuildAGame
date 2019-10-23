
requirePack("scripts.FrameWork.Global.GlobalFunctions");
local BaseHandle = class("BaseHandle");
g_tConfigTable.CREATE_NEW(BaseHandle);

function BaseHandle:ctor()
    self.linkerManager_ = nil;
end


function BaseHandle:Handle(ms)
end

function BaseHandle:SetLinkerManager(v)
    self.linkerManager_ = v;
end

function BaseHandle:GetLinkerManager()

    return self.linkerManager_;
end


return BaseHandle;