requirePack("scripts.FrameWork.Global.GlobalFunctions");
local UI = requirePack("scripts.FrameWork.UI.UI");
local SpriteUtil = requirePack("scripts.FrameWork.Util.SpriteUtil");
local ArmatureUtil = requirePack("scripts.FrameWork.Util.ArmatureUtil");

local DragOut = class("DragOut",function()
    return UI.new(); 
end);
DragOut.EventType = {
    [""] = "",
}
g_tConfigTable.CREATE_NEW(DragOut);

function DragOut:getUIRect()
    local sp = self.spNormal_;
    local size = sp:getContentSize();
    size.width = sp:getScale() * size.width;
    size.height = sp:getScale()*size.height;
    return cc.rect(-size.width/2,-size.height/2,size.width,size.height);
end
function DragOut:GetUIType()
    print("DragOut");
end
function DragOut:createArm(v)
    if self.arm_ == nil then 
        self.arm_ = ArmatureUtil.Create(self.armName_);
        self.addAim_:addChild(self.arm_);
        self.arm_:setScale(self.arm_:getScale()*self.scale);
        self.arm_:setPosition(v);
    end
end

function DragOut:deleteArm()
    if self.arm_ ~= nil then 
        self.arm_:removeFromParent();
        self.arm_ = nil;
    end
end

function DragOut:onTouchBegan(t,e)
    local pos = self:convertTouchToNodeSpace(t);
    local result = cc.rectContainsPoint(self:getUIRect(),pos);
    if result then 
        return true;
    end
end

function DragOut:onTouchMove(t,e)
    local pos = self:convertTouchToNodeSpace(t);
    local pos2 = self.addAim_:convertTouchToNodeSpace(t);
    pos2.x = math.min(pos2.x,g_tConfigTable.Director.winSize.width*0.8);
    pos2.x = math.max(pos2.x,g_tConfigTable.Director.winSize.width*0.2);
    pos2.y = math.min(pos2.y,g_tConfigTable.Director.winSize.height*0.8);
    pos2.y = math.max(pos2.y,g_tConfigTable.Director.winSize.height*0.2);
    local result = cc.rectContainsPoint(self:getUIRect(),pos);
    if not result then 
        -- 创建 跟随动画
        self:createArm(pos2);
    else
        -- 销毁 跟随动画
        self:deleteArm();
    end
    if self.arm_ ~= nil then 
        self.arm_:setPosition(pos2);
    end
end

function DragOut:onTouchEnded(t,e)
    local pos = self.addAim_:convertTouchToNodeSpace(t);
    pos.x = math.min(pos.x,g_tConfigTable.Director.winSize.width*0.8);
        pos.x = math.max(pos.x,g_tConfigTable.Director.winSize.width*0.2);
        pos.y = math.min(pos.y,g_tConfigTable.Director.winSize.height*0.8);
        pos.y = math.max(pos.y,g_tConfigTable.Director.winSize.height*0.2);
    if self.arm_ ~= nil then 
        self.arm_:removeFromParent();
        self.arm_ = nil;
        if self.dragOutCb_ ~= nil then 
            self.dragOutCb_(self.armName_, pos);
        end
    end
end

function DragOut:onTouchCancled(t,e)
    local pos = self.addAim_:convertTouchToNodeSpace(t);
    pos.x = math.min(pos.x,g_tConfigTable.Director.winSize.width*0.8);
        pos.x = math.max(pos.x,g_tConfigTable.Director.winSize.width*0.2);
        pos.y = math.min(pos.y,g_tConfigTable.Director.winSize.height*0.8);
        pos.y = math.max(pos.y,g_tConfigTable.Director.winSize.height*0.2);
    if self.arm_ ~= nil then 
        self.arm_:removeFromParent();
        self.arm_ = nil;
        if self.dragOutCb_ ~= nil then 
            self.dragOutCb_(self.armName_, pos);
        end
    end
end
------------------对外接口-----------------------
function DragOut:Init(spNormal,v,armName,cb)
    UI.Init(self);
    self.spNormal_ = spNormal;
    self.addAim_ = v;
    self.arm_ = nil;
    self.armName_ = armName;
    self.dragOutCb_ = cb;
    self:addChild(spNormal);
end



return DragOut;


    --[[
local DragOut = requirePack("scripts.FrameWork.UI.DragOut");

    local d = DragOut.new();
    d:Init(SpriteUtil.Create("btnBig.png"),self,"null_wll1",function(armName,pos) 
        local arm = ArmatureUtil.Create(armName);
        self:addChild(arm);
        arm:setPosition(pos);
    end);
    self:addChild(d);
    d:setPosition(VisibleRect:winSize().width/2,VisibleRect:winSize().height/2);
    ]]--