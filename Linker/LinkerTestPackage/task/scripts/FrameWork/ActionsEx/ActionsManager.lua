--region *.lua
--Date
--此文件由[BabeLua]插件自动生成
local EventUtil = requirePack("scripts.FrameWork.Util.EventUtil");
local ActionsManager = {}
g_tConfigTable.CREATE_NEW(ActionsManager);

function ActionsManager:ctor()

end

---------------------私有方法-----------------------

function ActionsManager:process()
    if #self.listOfPoses_ >0 then 
        local pos = self.listOfPoses_[1];
        table.remove(self.listOfPoses_,1);
        local x,y = self.node_:getPosition();
        local posNow = cc.p(x,y)
        local distance = cc.pGetDistance(posNow,pos);
        
        self.node_:runAction(cc.Sequence:create(cc.MoveTo:create(distance/self.speedValue_,pos),
        cc.CallFunc:create(
            function() 
               self:processEnd()
            end
        ),nil));
        
        local ori = cc.pNormalize(cc.pSub(pos,posNow));
         EventUtil.OnEvent(self:OnReachEvertPos(),ori);
    else
         EventUtil.OnEvent(self:OnReachEndPos(),nil);
    end
    
       
end

function ActionsManager:processEnd()
   self:process();
end

function ActionsManager:init()
    self:Dispose();
end
---------------------对外接口-----------------------

--[[
    工厂方法
]]--
ActionsManager.Create = function()
    local a = ActionsManager.new() 
    a:init();
    return a;
end


--[[
    按照路径移动
    参数:
    listOfPath:一个list 的Pos 
]]--
function ActionsManager:MoveByListOfPoses(list)
    if self.node_ ~= nil then 
        self.listOfPoses_ = list;
        self:process();
    end

end

--[[
    获取到达点事件列表
    返回值:
    list 到达每一个目标点的时候回调
]]--
function ActionsManager:OnReachEvertPos()
    return self.listOfEventOnReachEvertPos_; 
end

--[[
    返回到达终点的事件列表
    返回值:
    list 到达最后一个点的时候回调事件
]]--
function ActionsManager:OnReachEndPos()
    return self.listOfEventOnReachEndPos_;
end

--[[
    设置控制的目标节点
    参数:
    node：Node 要移动的目标节点
]]--
function ActionsManager:SetNode(node)
    self.node_ = node;
end


--[[
    获取目前是否有动作在执行
    返回值
    bool 
]]--
function ActionsManager:InOnAction()
    return self.IsOnAction_ ;
end


--[[
    停止正在运行的动作
]]--
function ActionsManager:Stop()
    self.IsOnAction_ = false;
end

--[[
    重新部署ActionManager
]]--
function ActionsManager:Dispose()
    self.listOfEventOnReachEvertPos_ = {};
    self.listOfEventOnReachEndPos_ = {};
    self.IsOnAction_ = false;
    self.node_ = nil;
    self.listOfPoses_ = nil;
    self.speedValue_ = 1;
end

--[[
    设置移动速度
    参数:
    speedValue：速度标量
]]--
function ActionsManager:SetSpeedValue(sv)
    self.speedValue_ = sv;
end





return ActionsManager;
--endregion
