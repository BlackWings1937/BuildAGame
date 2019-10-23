
requirePack(g_tConfigTable.RootFolderPath.."FrameWork.Global.GlobalFunctions");
local UniTagsManager = class("UniTagsManager");
g_tConfigTable.CREATE_NEW(UniTagsManager);

function UniTagsManager:ctor()
    self.listOfFreeIds_ = {};
    self.listOfUsingIds_ = {};
    self.rear_ = 1;
end

--[[
    获取一个唯一的id
    返回值:
    id: int 唯一id
]]--
function UniTagsManager:GetUniId()
    if #self.listOfFreeIds_<=0 then 
        local id = self.rear_ ;
        self.rear_ = self.rear_ + 1;
        table.insert(self.listOfUsingIds_,id);
        return id;
    else 
        local id = self.listOfFreeIds_[1];
        table.remove(self.listOfFreeIds_,1);
        return id;
    end
end


--[[
    回收一个唯一id
    参数:
    id: int 唯一id
]]--
function UniTagsManager:RecycleUniId(id)
    local count = #self.listOfUsingIds_;
    for i = count, 1 , -1 do 
        if id == self.listOfUsingIds_[i] then 
            table.remove(self.listOfUsingIds_,i);
            table.insert(self.listOfFreeIds_,id);
        end
    end
end

return UniTagsManager;