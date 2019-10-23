--region *.lua
--Date
--[[
	b2_staticBody = 0,
	b2_kinematicBody,
	b2_dynamicBody
    ]]--
--此文件由[BabeLua]插件自动生成
--local XSafeSchedule = requirePack("scripts.Tools.XSafeSchedule");
local PhysicNode = class("PhysicNode",function ()
    return cc.Node:create();
end);
g_tConfigTable.CREATE_NEW(PhysicNode);
--[[
PhysicNode.new = function(...)
    local instance
    if PhysicNode.__create then
        instance = PhysicNode.__create 
    else
        instance ={};-- class("Bird", PhysicNode);
    end
    -- setmetatableindex(instance, AnimalBengChuang)
    for k,v in pairs(PhysicNode) do 
        instance[k] = v 
    end
    instance.class = PhysicNode
    instance:ctor(...)
    return instance
end
]]--
local PTM = 32;

function PhysicNode:ctor(bodyType,shapeName)
    -- 销毁标记
    self.isBeDestroyed = false;
    -- 创建刚体
    self.body = g_tConfigTable.Box2dManager.createBodyForSp(self,bodyType,shapeName);
end

--[[设置刚体角度的方法]]--
function PhysicNode:setBodyRotate(rota)
    local position = self.body:GetPosition();
    local angle = (rota * 3.14)/180;
    self.body:SetTransform(position,angle);
    self:setRotation(rota)
end

--[[设置刚体位置的方法]]--
function PhysicNode:setBodyPosition(pos)
    local vPos = b2Vec2:new(pos.x/PTM,pos.y/PTM);
    local angle = self.body:GetAngle();
    self.body:SetTransform(vPos,angle);
    self:setPosition(pos);
end

--[[向刚体施力]]--
function PhysicNode:addForce(force)
    self.body:ApplyForceToCenter(b2Vec2:new(force.x,force.y),true);
end

--[[设置刚体线速度]]--
function PhysicNode:setSpeed(speed)
    self.body:SetLinearVelocity(b2Vec2:new(speed.x,speed.y));
end

--[[设置刚体沉睡与否]]--
function PhysicNode:setIsWake(isWake)
    self.body:SetAwake(isWake);
end

--[[ 获取刚体目前速度 ]]--
function PhysicNode:getSpeed()
    local speed = self.body:GetLinearVelocity();
    return cc.p(speed.x,speed.y);
end

--[[开始碰撞的响应方法]]--
function PhysicNode:beginContact(contact)

end

--[[结束碰撞的响应方法]]--
function PhysicNode:endContact(contact)

end

--[[前结算的响应方法]]--
function PhysicNode:preSolve(contact,m)

end

--[[后解算子的响应方法]]--
function PhysicNode:postSolve(contact,i)

end

--[[安全销毁自身的方法]]--
function PhysicNode:safeDestroyedSelf()
--[[
    XSafeSchedule.XSchedule(self,function() 
        local world = self.body:GetWorld();
        world:DestroyBody(self.body);
        self:removeFromParent();
        g_tConfigTable.Box2dManager.removeUserData(self);
    end,0.016,1,1);
    self.isBeDestroyed = true;
    ]]--
end



return PhysicNode;
--endregion
