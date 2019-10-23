

requirePack(g_tConfigTable.RootFolderPath.."FrameWork.Global.GlobalFunctions");
local KeyFrameParseReader = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.KeyFrameParseReader");
local Actor = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.AnimationEngineLua.Actor");

local ActorShowObj = class("ActorShowObj",function()
    return Actor.new();
end);
g_tConfigTable.CREATE_NEW(ActorShowObj);

function ActorShowObj:SetShowObjZOrder(zorder)
    if self.scriptAction_:GetIsBanZOrder() == false then 
        self.showObj_:setLocalZOrder(zorder);
        self.scriptAction_:SetRuntimeActorZorderByName(self:GetActorName(),zorder);
    end
end

function ActorShowObj:saveShowObjNowStatue()
    if self.showObj_ ~= nil then 
        self.oldShowObjStatue_ = {};
        local x,y = self.showObj_:getPosition();
        self.oldShowObjStatue_.x = x;
        self.oldShowObjStatue_.y = y;
        self.oldShowObjStatue_.sx = self.showObj_:getScaleX();
        self.oldShowObjStatue_.sy = self.showObj_:getScaleY();
        self.oldShowObjStatue_.rx = self.showObj_:getRotationSkewX();
        self.oldShowObjStatue_.ry = self.showObj_:getRotationSkewY();
        self.oldShowObjStatue_.z = self.showObj_:getLocalZOrder();
    end
end

function ActorShowObj:reInitBySaveStatue()
    if self.showObj_ ~= nil then 
        if self.oldShowObjStatue_ ~= nil then 
            self.showObj_:setPosition(cc.p(self.oldShowObjStatue_.x,self.oldShowObjStatue_.y));
            self.showObj_:setScaleX(self.oldShowObjStatue_.sx);
            self.showObj_:setScaleY(self.oldShowObjStatue_.sy);
            self.showObj_:setRotationSkewX(self.oldShowObjStatue_.rx );
            self.showObj_:setRotationSkewY(self.oldShowObjStatue_.ry );
            self.showObj_:setLocalZOrder( self.oldShowObjStatue_.z);
        end
    end
end

function ActorShowObj:initActorInfo(ai)
    Actor.initActorInfo(self,ai,KeyFrameParseReader.new());
    self.scriptAction_:GetActorOccupyInfo()[ai.layername] = true; -- 设置角色被占用
end

function ActorShowObj:Dispose()
    Actor.Dispose(self);
    self.scriptAction_:GetActorOccupyInfo()[self.actorInfo_.layername] = false; -- 设置角色被占用
end

return ActorShowObj;