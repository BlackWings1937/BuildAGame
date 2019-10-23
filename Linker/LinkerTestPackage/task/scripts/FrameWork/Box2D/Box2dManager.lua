-- region *.lua
-- Date
-- 此文件由[BabeLua]插件自动生成
local Manager = class("Manager");

--[[ 初始化物理世界 ]]--
function Manager:ctor(physicRootNode, gravity, isDebug)
    if isDebug == nil then
        isDebug = true;
    end
    local g = b2Vec2:new(0, -gravity);
    self.world = b2World:new(g);

    -- 创建物理世界更新管理器
    self.physicManager = LuaPhysicManager:create(self.world);
    self.physicManager:setIsOpenDebug(false);
    if physicRootNode then
        physicRootNode:addChild(self.physicManager);
    end

    -- 创建物理监听器
    self.contactListener = LuaContactListener:new();
    self.world:SetContactListener(self.contactListener);

    -- 启动循环
    self.physicManager:startUpdatePhysic();
end

--[[ 清理物理世界 ]]--
function Manager:clear()
    if self.physicManager then
        self.physicManager:stopUpdatePhysic();
        self.physicManager = nil;
    end
    if self.contactListener then
        self.contactListener:delete();
        self.contactListener = nil;
    end
    if self.world then 
        self.world:delete();
        self.world = nil;
    end
end

function Manager:setIsOpenTimeStep(isOpen)
    self.physicManager:setIsOpenCustomTimeStep(isOpen);
end

function Manager:setCustomTimeStep(timeStep)
    self.physicManager:setTimeStep(timeStep);
end


g_tConfigTable.Box2dManager = { };
g_tConfigTable.manager = nil;



--[[ 初始化物理世界 ]]--
g_tConfigTable.Box2dManager.getInstance = function(physicRootNode,shapeFilePath ,gravity)
    if g_tConfigTable.manager == nil then
        g_tConfigTable.manager = Manager.new(physicRootNode, gravity);
        -- 加载形状文件
        GB2ShapeCache:sharedGB2ShapeCache():addShapesWithFile(shapeFilePath);
    end
    return g_tConfigTable.manager;
end

--[[ 清理物理世界 ]]--
g_tConfigTable.Box2dManager.clear = function()
    g_tConfigTable.Box2dManager.saveUserData = {};
    if g_tConfigTable.manager then 
        g_tConfigTable.manager:clear();
        g_tConfigTable.manager = nil;
    end
end
--[[ 停止物理更新 ]]--
g_tConfigTable.Box2dManager.stopUpdatePhysicEngine = function ()
     g_tConfigTable.manager.physicManager:stopUpdatePhysic();
end

--[[ 启动物理更新 ]]--
g_tConfigTable.Box2dManager.startUpdatePhysicEngine = function ()
     g_tConfigTable.manager.physicManager:startUpdatePhysic();
end

--[[ 添加刚体到精灵 ]]--
g_tConfigTable.Box2dManager.createBodyForSp = function(sp,bodyType,shapeName)
    if g_tConfigTable.manager then
        local bodyDef = b2BodyDef:new();
        bodyDef.type = bodyType;
        local body = g_tConfigTable.manager.world:CreateBody(bodyDef);
        GB2ShapeCache:sharedGB2ShapeCache():addFixturesToBody(body, shapeName);
        body:SetUserData(sp);
        g_tConfigTable.Box2dManager.addUserData(body:GetUserData(),sp);
        return body;
    else
        print("wrong! box manager nil");
        return nil;
    end
end

--[[
    开关自定义时间步 --setCustomTimeStep setIsOpenTimeStep
]]--
g_tConfigTable.Box2dManager.SetIsOpenCustomTimeStep = function (isOpen)
    g_tConfigTable.manager:setIsOpenTimeStep(isOpen);
end

--[[
    设置时间步长
]]--
g_tConfigTable.Box2dManager.SetCustomTimeStep = function (timeStep)
    g_tConfigTable.manager:setCustomTimeStep(timeStep);
end

--[[
    删除物理世界，和物理世界下的所有元素
]]--
g_tConfigTable.Box2dManager.Dispose = function ()
    if g_tConfigTable.Box2dManager.saveUserData ~= nil then 
        
    end
end

--[[ 删除刚体 ]]--
g_tConfigTable.Box2dManager.deleteBody = function (body)
     g_tConfigTable.Box2dManager.removeUserData(body);
     g_tConfigTable.manager.world:DestroyBody(body.body);
end


--[[ 定义碰撞回调方法 ]]--

-- 保存所有userdata指针
g_tConfigTable.Box2dManager.saveUserData = {};

--[[box2d userData 指针链保存 ]]--

--[[
    创建一条box2d userdata保存地址
    参数:
    userData:box2d中取出来的userdata地址
    physicComponent:对应的碰撞组件
]]--
g_tConfigTable.Box2dManager.addUserData = function (userData,physicComponent)
    local data = {};
    data.userData = userData;
    data.physicComponent = physicComponent;-- PhysicNode
    table.insert(g_tConfigTable.Box2dManager.saveUserData,data);
end

--[[
    移除一条box2d userdata保存地址
    参数:
    userData:box2d中取出来的userdata地址
]]--
g_tConfigTable.Box2dManager.removeUserData = function (userData)
    local i = 1;
    while g_tConfigTable.Box2dManager.saveUserData[i] ~= nil do 
        local data = g_tConfigTable.Box2dManager.saveUserData[i];
        if data.physicComponent == userData then 
            table.remove(g_tConfigTable.Box2dManager.saveUserData,i);
        else 
            i = i + 1;
        end
    end
end
g_tConfigTable.Box2dManager.findOtherOnThisContact = function(c, pcSelf)
    local pcB = g_tConfigTable.Box2dManager.findPhysicComponentByUserData(c:GetFixtureB():GetBody():GetUserData())
    if pcB ~= pcSelf then
        return pcB;
    else 
        return g_tConfigTable.Box2dManager.findPhysicComponentByUserData(c:GetFixtureA():GetBody():GetUserData());
    end
end 

--[[
    查找一个精灵 userdata 是否在地址库,返回它对应的物理组件
    参数:
    userData:box2d中取出来的userdata地址
]]--
g_tConfigTable.Box2dManager.findPhysicComponentByUserData = function (userData)
    local i = 1;
    while g_tConfigTable.Box2dManager.saveUserData[i] ~= nil do 
        local data = g_tConfigTable.Box2dManager.saveUserData[i];
        if data.userData == userData then 
            return data.physicComponent;
        else 
            i = i + 1;
        end
    end
    return nil;
end


--[[
    碰撞监听的回调
]]--
g_tConfigTable.ContactListener = {};
g_tConfigTable.ContactListener.BeginContact = function (contact)
    contact = tolua.cast(contact,"b2Contact");
    local fixA = tolua.cast(contact:GetFixtureA(),"b2Fixture");
    local bodyA = tolua.cast(fixA:GetBody(),"b2Body");
    local dataA = bodyA:GetUserData();
    local fixB = tolua.cast(contact:GetFixtureB(),"b2Fixture");
    local bodyB = tolua.cast(fixB:GetBody(),"b2Body");
    local dataB = bodyB:GetUserData();

    local pcA = g_tConfigTable.Box2dManager.findPhysicComponentByUserData(dataA);
    local pcB = g_tConfigTable.Box2dManager.findPhysicComponentByUserData(dataB);
    if pcA then 
       -- 组件分发碰撞世界
       pcA:beginContact(contact);
    end
    if pcB then 
       -- 组件分发碰撞事件
       pcB:beginContact(contact);
    end
end
--[[
    碰撞结束的方法
    参数:
    contact:接触信息
]]--
g_tConfigTable.ContactListener.EndContact = function (contact)
    contact = tolua.cast(contact,"b2Contact");
    local fixA = tolua.cast(contact:GetFixtureA(),"b2Fixture");
    local bodyA = tolua.cast(fixA:GetBody(),"b2Body");
    local dataA = bodyA:GetUserData();
    local fixB = tolua.cast(contact:GetFixtureB(),"b2Fixture");
    local bodyB = tolua.cast(fixB:GetBody(),"b2Body");
    local dataB = bodyB:GetUserData();

    local pcA = g_tConfigTable.Box2dManager.findPhysicComponentByUserData(dataA);
    local pcB = g_tConfigTable.Box2dManager.findPhysicComponentByUserData(dataB);
    if pcA then 
       -- 组件分发碰撞世界
       pcA:endContact(contact);
    end
    if pcB then 
       -- 组件分发碰撞事件
       pcB:endContact(contact);
    end
end

--[[
    预先解算子
    参数:
    contact:接触信息
    oldManifold：
]]--
g_tConfigTable.ContactListener.PreSolve = function (contact,m)
    contact = tolua.cast(contact,"b2Contact");
    local fixA = tolua.cast(contact:GetFixtureA(),"b2Fixture");
    local bodyA = tolua.cast(fixA:GetBody(),"b2Body");
    local dataA = bodyA:GetUserData();
    local fixB = tolua.cast(contact:GetFixtureB(),"b2Fixture");
    local bodyB = tolua.cast(fixB:GetBody(),"b2Body");
    local dataB = bodyB:GetUserData();

    local pcA = g_tConfigTable.Box2dManager.findPhysicComponentByUserData(dataA);
    local pcB = g_tConfigTable.Box2dManager.findPhysicComponentByUserData(dataB);
    if pcA then 
       -- 组件分发碰撞世界
       pcA:preSolve(contact,m);
    end
    if pcB then 
       -- 组件分发碰撞事件
       pcB:preSolve(contact,m);
    end
end

--[[
    后解算子
    参数:
    contact:接触信息
    i
]]--
g_tConfigTable.ContactListener.PostSolve = function (contact,i)
    contact = tolua.cast(contact,"b2Contact");
    local fixA = tolua.cast(contact:GetFixtureA(),"b2Fixture");
    local bodyA = tolua.cast(fixA:GetBody(),"b2Body");
    local dataA = bodyA:GetUserData();
    local fixB = tolua.cast(contact:GetFixtureB(),"b2Fixture");
    local bodyB = tolua.cast(fixB:GetBody(),"b2Body");
    local dataB = bodyB:GetUserData();

    local pcA = g_tConfigTable.Box2dManager.findPhysicComponentByUserData(dataA);
    local pcB = g_tConfigTable.Box2dManager.findPhysicComponentByUserData(dataB);
    if pcA then 
       -- 组件分发碰撞世界
       pcA:postSolve(contact,i);
    end
    if pcB then 
       -- 组件分发碰撞事件
       pcB:postSolve(contact,i);
    end
end



-- endregion
