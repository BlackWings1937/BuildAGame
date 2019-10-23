requirePack(g_tConfigTable.RootFolderPath.."FrameWork.Global.GlobalFunctions");

local ZOrderPackage = class("ZOrderPackage");

g_tConfigTable.CREATE_NEW(ZOrderPackage)

function ZOrderPackage:ctor()
    self.listOfActors_ = {}; -- json 中的角色名

    self.zOrder_ = -1;

    self.zOrderChange_ = -1;

    self.name_ = "";

    self.count_ = 0;
end

-- 对外接口

function ZOrderPackage:Init(index,name,count)
    self:SetZOrderChange(index);
    self:SetZOrder(index);
    self:SetName(name);
    self:SetCount(count);
    return self;
end

--[[
    添加actor
]]--
function ZOrderPackage:AddActor(a)
    table.insert(self.listOfActors_,a);
end

function ZOrderPackage:GetActors()
    return self.listOfActors_;
end

function ZOrderPackage:SetZOrder(v)
    self.zOrder_ = v;
end

function ZOrderPackage:GetZOrder()
    return self.zOrder_;
end

function ZOrderPackage:SetZOrderChange(v)
    self.zOrderChange_ = v;
end

function ZOrderPackage:GetZOrderChange()
    return self.zOrderChange_;
end

function ZOrderPackage:GetDiss()
    return self.zOrderChange_ - self.zOrder_;
end

function ZOrderPackage:SetName(n)
    self.name_ = n;
end

function ZOrderPackage:GetName()
    return self.name_;
end

function ZOrderPackage:Count()
    return self.count_;
end

function ZOrderPackage:SetCount(v)
    self.count_ = v;
end
return ZOrderPackage;