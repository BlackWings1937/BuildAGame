requirePack("scripts.FrameWork.Global.GlobalFunctions");
local UI = requirePack("scripts.FrameWork.UI.UI");
local SpriteUtil = requirePack("scripts.FrameWork.Util.SpriteUtil");

local Slider = class("Toggle",function()
    return UI.new(); 
end);

Slider.Type = {
    ["HORIZONTAL"] = 1,
    ["VERTICAL"] = 2,
}

Slider.EventType = { --Slider.EventType.VALUE_CHANGE
    ["VALUE_CHANGE"] = "VALUE_CHANGE",
    ["COMFIRM_VALUE_CHANGE"] = "COMFIRM_VALUE_CHANGE",
}
g_tConfigTable.CREATE_NEW(Slider);

function Slider:ctor()
    self.oldRate_ = 0;
    self.type_ = Slider.Type.HORIZONTAL;
end

function Slider:SetType(v)
    self.type_ = v;
end

function Slider:GetType()
    return self.type_;
end

function Slider:GetUIType()
    print("Slider");
end
function Slider:getUIRect()
    if self.type_ == Slider.Type.HORIZONTAL then 
        return cc.rect(0,-self.height_/2 - 45,self.width_,self.height_);
    else 
        return cc.rect(-self.width_/2,-self.height_,self.width_,self.height_);
    end
end

function Slider:updateRate(rate)
    rate = math.min(1,rate);
    rate = math.max(0,rate);
    if math.abs(self.oldRate_-rate)<0.01 then 
        return; 
    end
    self:emit(Slider.EventType.VALUE_CHANGE,rate);
    self:xsetStatue(rate);
    self.oldRate_ = rate;
end

function Slider:onTouchBegan(t,e)
    local pos = self:convertTouchToNodeSpace(t);
    local rect = self:getUIRect();
    if cc.rectContainsPoint(self:getUIRect(),pos) then 
        self.isUserDraging_ = true;
        local rate = 0;
        if self.type_ == Slider.Type.HORIZONTAL then 
            rate = pos.x/self:GetWidth();

        else 
            rate = (-pos.y)/self:GetHeight();
        end
        self:updateRate(rate);
        -- todo event
        return true;
    end
    return false;
end

function Slider:onTouchMove(t,e)
    local pos = self:convertTouchToNodeSpace(t);
    local rate = 0;
    if self.type_ == Slider.Type.HORIZONTAL then 
        rate = pos.x/self:GetWidth();

    else 
        rate = (-pos.y)/self:GetHeight();
    end
    -- print("rate:"..rate);
    self:updateRate(rate);
end

function Slider:onTouchEnded(t,e)
    local pos = self:convertTouchToNodeSpace(t);
    local rate = 0;
    if self.type_ == Slider.Type.HORIZONTAL then 
        rate = pos.x/self:GetWidth();

    else 
        rate = (-pos.y)/self:GetHeight();
    end

    self:updateRate(rate);

    self:emit(Slider.EventType.COMFIRM_VALUE_CHANGE,self.oldRate_ );
    self.isUserDraging_ = false;
end

function Slider:onTouchCancled(t,e)
    local pos = self:convertTouchToNodeSpace(t);
    local rate = 0;
    if self.type_ == Slider.Type.HORIZONTAL then 
        rate = pos.x/self:GetWidth();

    else 
        rate = (-pos.y)/self:GetHeight();
    end
    self:updateRate(rate);
    self.isUserDraging_ = false;
end

-----------对外接口----------------

function Slider:Init(spBgName,spFillName,spHeadName)
    UI.Init(self);
    local spBg = SpriteUtil.Create(spBgName);
    self:addChild(spBg);

    self.stencil_ = SpriteUtil.Create(spBgName);
    self.oriScale_ = self.stencil_:getScale();
    self.clip_ = cc.ClippingNode:create(self.stencil_);
    if self.type_ == Slider.Type.VERTICAL then 
        self.stencil_:setAnchorPoint(cc.p(0.5,1));
        spBg:setAnchorPoint(cc.p(0.5,1));
        spBg:setPosition(cc.p(0,0));
    else 
        self.stencil_:setAnchorPoint(cc.p(0,0.5));
        spBg:setAnchorPoint(cc.p(0,0.5));
        spBg:setPosition(cc.p(0,0));
    end
    self.stencil_:setPosition(cc.p(0,0));
    self.width_ = self.stencil_:getScale()*self.stencil_:getContentSize().width;
    self.height_ = self.stencil_:getScale()*self.stencil_:getContentSize().height;
    self:addChild(self.clip_);
    local spFill = SpriteUtil.Create(spFillName);
    self.clip_:addChild(spFill);
    spFill:setAnchorPoint(cc.p(0,0.5));
    spFill:setPosition(cc.p(0,0));
    self.head_ = SpriteUtil.Create(spHeadName);
    self:addChild(self.head_);
    if self.type_ == Slider.Type.VERTICAL then 
        self.head_:setPosition(cc.p(0,0));

    else 
        self.head_:setPosition(cc.p(self.width_,0));
    end
    self.events_[Slider.EventType.VALUE_CHANGE] = {}; 
    self.events_[Slider.EventType.COMFIRM_VALUE_CHANGE] = {}; 
    self.isUserDraging_ = false;
end


function Slider:xsetStatue(rate) 
    rate = math.min(1,rate);
    rate = math.max(0,rate);
    if  self.type_ == Slider.Type.HORIZONTAL then 
        self.head_:setPosition(cc.p(rate*self:GetWidth(),0));
        self.stencil_:setScaleX(self.oriScale_ * rate);
    else 
        self.head_:setPosition(cc.p(0,-rate*self:GetHeight()));
        self.stencil_:setScaleY(self.oriScale_ * rate);
    end
    
end
--[[
    rate [0~1]
]]--
function Slider:SetStatue(rate)
    if  self.isUserDraging_  == false then 
        self:xsetStatue(rate);
    end
end

function Slider:GetWidth()
    return self.width_;
end

function Slider:GetHeight()
    return self.height_;
end

return Slider

--[[

    local stencil = SpriteUtil.Create("btnMove.png");
    local clip = cc.ClippingNode:create(stencil);
    self:addChild(clip);
    clip:setPosition(g_tConfigTable.Director.midPos);
    local sp = SpriteUtil.Create("btnMove.png");
    clip:addChild(sp);
    sp:setPosition(cc.p(30,30));
]]--

--[[
    exmaple 1
    local slider = Slider.new();
    slider:Init("spBg.png","spFill.png","spHead.png");
    self:addChild(slider);
    slider:setPosition(cc.pSub(g_tConfigTable.Director.midPos,cc.p(slider:GetWidth()/2,0)));
    slider:SetStatue(0.5);
    slider:AddListener(Slider.EventType.VALUE_CHANGE,function(rate) 
        print("now rate:"..rate);
    end);
    ]]--