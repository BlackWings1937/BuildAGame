requirePack("scripts.FrameWork.Global.GlobalFunctions");
local UI = requirePack("scripts.FrameWork.UI.UI");
local SpriteUtil = requirePack("scripts.FrameWork.Util.SpriteUtil");

local Toggle = class("Toggle",function()
    return UI.new(); 
end);
Toggle.EventType = {
    ["ToggleOn"] = "ToggleOn",
    ["ToggleOff"] = "ToggleOff",
}
g_tConfigTable.CREATE_NEW(Toggle);

function Toggle:GetUIType()
    print("Toggle");
end
function Toggle:getUIRect()
    local sp = nil;
    if self.isOn_ then 
        sp = self.spToggleOn_;
    else 
        sp = self.spToggleOff_;
    end
    local size = sp:getContentSize();
    size.width = sp:getScale() * size.width;
    size.height = sp:getScale()*size.height;
    return cc.rect(-size.width/2,-size.height/2,size.width,size.height);
end

function Toggle:onToggleTouch() 
    if self.isOn_ then 
        self.isOn_ = false;
        self:emit(Toggle.EventType.ToggleOff);
    else 
        self.isOn_ = true;
        self:emit(Toggle.EventType.ToggleOn);
    end
    self.spToggleOn_:setVisible(self.isOn_);
    self.spToggleOff_:setVisible(not self.isOn_);
end


function Toggle:onTouchBegan(t,e)
    local pos = self:convertTouchToNodeSpace(t);
    local result = cc.rectContainsPoint(self:getUIRect(),pos);
    return result;
end
function Toggle:onTouchEnded(t,e)
    local pos = self:convertTouchToNodeSpace(t);
    local result = cc.rectContainsPoint(self:getUIRect(),pos);
    if result then 
        self:onToggleTouch();
    end
end
--------------对外接口----------------
function Toggle:Init(spToggleOn,spToggleOff)
    UI.Init(self);
    self.spToggleOn_ = spToggleOn;
    self.spToggleOff_ =spToggleOff;
    self.isOn_ = false;
    self.events_ = {
        [Toggle.EventType.ToggleOn] = {},
        [Toggle.EventType.ToggleOff] = {},
    }
    self:addChild(self.spToggleOn_);
    self:addChild(self.spToggleOff_);
end

Toggle.Create = function(spToggleOnName,spToggleOffName,pos,parent)
    local tg = Toggle.new();
    tg:Init(SpriteUtil.Create(spToggleOnName),SpriteUtil.Create(spToggleOffName));
    parent:addChild(tg);
    tg:setPosition(pos);
    return tg;
end


function Toggle:AddListener(eventType,cb)
    table.insert(self.events_[eventType],cb)
end

function Toggle:RemoveListener(eventType,cb)
    local listOfEvents = self.events_[eventType];
    for i = #listOfEvents,1,-1 do 
        if listOfEvents[i] == cb then 
            table.remove(listOfEvents,i);
        end
    end
end

function Toggle:SetStatue(isOn)
    self.isOn_  = isOn;
    self.spToggleOn_:setVisible(self.isOn_);
    self.spToggleOff_:setVisible(not self.isOn_);
end
return Toggle;

--[[
    -- example 1
    -- 录制开始暂停开关
    self.tgRecord_ = Toggle.Create("btnPlay.png","btnStart.png",cc.p(CFG_X(380),CFG_GL_Y(60)),self);
    self.tgRecord_:AddListener(Toggle.EventType.ToggleOn,function() 
         print("start record here..");
         if self.startMovie_ == false then 
            self:StartRecord();
         end
         self:ResumeRecord();
    end);
    self.tgRecord_:AddListener(Toggle.EventType.ToggleOff,function() 
        print("pause record here...");
        self:PauseRecord();
    end);
]]--