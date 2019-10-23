requirePack("scripts.FrameWork.Global.GlobalFunctions");
local UI = requirePack("scripts.FrameWork.UI.UI");
local SpriteUtil = requirePack("scripts.FrameWork.Util.SpriteUtil");
local Switchs = class("Switchs",function()
    return UI.new();
end);
Switchs.EventType = {
    ["SWITCHON"] = "SWITCHON",
}
g_tConfigTable.CREATE_NEW(Switchs);

function Switchs:GetUIType()
    print("Switchs");
end
function Switchs:getUIRect()
    return cc.rect(
        -self.size_.width/2,
        -self.size_.height/2,
        self.size_.width,
        self.size_.height);
end

function Switchs:onTouchBegan(t,e)
    local pos = self:convertTouchToNodeSpace(t);
    local result = cc.rectContainsPoint(self:getUIRect(),pos);
    return result;
end
function Switchs:onTouchEnded(t,e)
    local pos = self:convertTouchToNodeSpace(t);
    local result = cc.rectContainsPoint(self:getUIRect(),pos);
    if result then 
        SoundUtil:getInstance():playLua(
    g_tConfigTable.sTaskpath.."audio//BtnClick.mp3",
    g_tConfigTable.sTaskpath.."audio//BtnClick.mp3",
    function() end);
        self:switchNext();
    end
end

function Switchs:switchNext()
    self.index_ = self.index_ + 1;
    local countOfIcons = #self.listOfIcons_;
    if self.index_> countOfIcons then 
        self.index_ = 1;
    end
    for i = countOfIcons,1,-1 do 
        self.listOfIcons_[i]:setVisible(false);
    end
    self.listOfIcons_[self.index_]:setVisible(true);
    self:emit(Switchs.EventType.SWITCHON,self.index_);
end

-----------------对外接口---------------

function Switchs:SetStatue(index)
    local countOfIcons = #self.listOfIcons_;
    index = math.max(index,0);
    index = math.min(index,countOfIcons);
    self.index_ = index;
    for i = countOfIcons,1,-1 do 
        self.listOfIcons_[i]:setVisible(false);
    end
    self.listOfIcons_[self.index_]:setVisible(true);
end

function Switchs:Init(listOfIcons)
    UI.Init(self);
    self.listOfIcons_ = listOfIcons;
    for i = 1,#self.listOfIcons_,1 do 
        self:addChild(self.listOfIcons_[i]);
    end
    self.index_ = 1;
    self.events_ = {
        [Switchs.EventType.SWITCHON] = {},
    }
    self.size_ = cc.size(
        self.listOfIcons_[1]:getContentSize().width * self.listOfIcons_[1]:getScale(),
        self.listOfIcons_[1]:getContentSize().height * self.listOfIcons_[1]:getScale()
    );
    self:SetStatue(1);
end



return Switchs;