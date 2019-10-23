requirePack("scripts.FrameWork.Global.GlobalFunctions");
local UI = requirePack("scripts.FrameWork.UI.UI");
local SpriteUtil = requirePack("scripts.FrameWork.Util.SpriteUtil");
local ArmatureUtil = requirePack("scripts.FrameWork.Util.ArmatureUtil");
local DragOut = requirePack("scripts.FrameWork.UI.DragOut");

local DragOutAtScrollBox = class("DragOutAtScrollBox",function()
    return DragOut.new(); 
end);
DragOutAtScrollBox.EventType = {
    [""] = "",
}
g_tConfigTable.CREATE_NEW(DragOutAtScrollBox);

function DragOutAtScrollBox:onTouchBegan(t,e)
    local pos = self:convertTouchToNodeSpace(t);
    local result = cc.rectContainsPoint(self:getUIRect(),pos);
    if result then 
        return true;
    end
end
function DragOutAtScrollBox:onTouchMove(t,e)
    local pos2 = self.addAim_:convertTouchToNodeSpace(t);
    local pos3 = self.scrollBox_:convertTouchToNodeSpace(t);
    pos2.x = math.min(pos2.x,g_tConfigTable.Director.winSize.width*0.8);
    pos2.x = math.max(pos2.x,g_tConfigTable.Director.winSize.width*0.2);
    pos2.y = math.min(pos2.y,g_tConfigTable.Director.winSize.height*0.8);
    pos2.y = math.max(pos2.y,g_tConfigTable.Director.winSize.height*0.2);
    local result = cc.rectContainsPoint(self:getScrollUIRect(),pos3);
    if not result then 
        -- 创建 跟随动画
        self:createArm(pos2);
        self.scrollBox_:Skip();
    else
        -- 销毁 跟随动画
        self:deleteArm();
        self.scrollBox_:NotSkip();
    end
    if self.arm_ ~= nil then 
        self.arm_:setPosition(pos2);
    end
end--Skip

function DragOutAtScrollBox:onTouchEnded(t,e)
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

function DragOutAtScrollBox:onTouchCancled(t,e)
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

function DragOutAtScrollBox:getScrollUIRect()
    return self.scrollBox_:getUIRect();
end

function DragOutAtScrollBox:SetScrollBox(v)
    self.scrollBox_ = v;
end

return DragOutAtScrollBox;