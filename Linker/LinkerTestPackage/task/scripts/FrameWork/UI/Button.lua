requirePack("scripts.FrameWork.Global.GlobalFunctions");
local UI = requirePack("scripts.FrameWork.UI.UI");
local SpriteUtil = requirePack("scripts.FrameWork.Util.SpriteUtil");

local Button = class("Button",function()
    return UI.new(); 
end);
--Button.EventType.Click
Button.EventType = {
    ["Click"] = "Click",
}
g_tConfigTable.CREATE_NEW(Button);
function Button:getUIRect()
    local sp = self.spNormal_;
    local size = sp:getContentSize();
    size.width = sp:getScale() * size.width;
    size.height = sp:getScale()*size.height;
    return cc.rect(-size.width/2,-size.height/2,size.width,size.height);
end
function Button:onButtonClick()
    local listOfEvents = self.events_[Button.EventType.Click];
    for i = #listOfEvents,1,-1 do 
        listOfEvents[i]();
    end
end
function Button:GetUIType()
    print("Button");
end
function Button:onTouchBegan(t,e)
    local pos = self:convertTouchToNodeSpace(t);
    self.oriWPos_ = self:convertToWorldSpace(pos);
    local result = cc.rectContainsPoint(self:getUIRect(),pos);
    self:showStateToOn();

    return result;
end
function Button:onTouchEnded(t,e)
    self:showStateToOff();
    local pos = self:convertTouchToNodeSpace(t);
    local result = cc.rectContainsPoint(self:getUIRect(),pos);
    local newWPos = self:convertToWorldSpace(pos);
    if cc.pGetDistance(newWPos,self.oriWPos_) >20 then 
        return ;
    end
    if result then 
        if self.isPlayMusic_ then 
            SoundUtil:getInstance():playLua(
                g_tConfigTable.sTaskpath.."audio//BtnClick.mp3",
                g_tConfigTable.sTaskpath.."audio//BtnClick.mp3",
                function() end);
        end
        self:emit(Button.EventType.Click);

        self:runAction(cc.Sequence:create(cc.ScaleTo:create(0.25,1.25),cc.ScaleTo:create(0.25,0.8),cc.ScaleTo:create(0.25,1),nil));
    end
end
function Button:onTouchCancled(t,e)
    self:showStateToOff();
end

function Button:showStateToOff()
    self.spNormal_:setVisible(true);
    self.spDown_:setVisible(false);
end

function Button:showStateToOn()
    self.spNormal_:setVisible(false);
    self.spDown_:setVisible(true);
end
------------------对外接口-----------------------
function Button:Init(spNormal,spDown)
    UI.Init(self,false);
    self.spNormal_ = spNormal;
    self.spDown_ = spDown;
    self:addChild(self.spNormal_);
    self:addChild(self.spDown_ );
    self.spDown_:setVisible(false);
    self.events_ = {
        [Button.EventType.Click] = {},
    }
    self.isPlayMusic_ = true;
end

--[[
    spNormalName,spDownName:放在bgimg 下的图片名，包括后缀 .png
]]--
Button.Create = function(spNormalName,spDownName,pos,parent) 
    local btn = Button.new();
    btn.spNormalName_ = spNormalName;
    btn:Init(spNormalName,spDownName);
    parent:addChild(btn);
    btn:setPosition(pos);
    --btn.oriPos = pos;
    return btn;
end

function Button:setUIVisible(v)
    self:setVisible(v);
end

function Button:AddListener(eventType,cb)
    local listOfEvents = self.events_[eventType];
    table.insert(listOfEvents,cb);
end

function Button:RemoveListener(eventType,cb)
    local listOfEvents = self.events_[eventType];
    for i = #listOfEvents,1,-1 do 
        if listOfEvents[i] == cb then 
            table.remove(listOfEvents,i);
        end
    end
end

return Button;

--[[
    -- example 1
        -- 创建arm
    self.btnCreateNewArm_ = Button.Create("btnMove.png","btnStart.png",cc.p(CFG_X(158),CFG_GL_Y(932)),self);
    self.btnCreateNewArm_:AddListener(Button.EventType.Click,function() 
        self:CreateObjAtPos("",g_tConfigTable.Director.midPos);
    end);
]]--