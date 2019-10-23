requirePack("scripts.FrameWork.Global.GlobalFunctions");
local SpriteUtil = requirePack("scripts.FrameWork.Util.SpriteUtil");
local UI = requirePack("scripts.FrameWork.UI.UI");
local TouchBlock = class("TouchBlock",function()
    return UI.new(); 
end);
TouchBlock.EventType = {
}
g_tConfigTable.CREATE_NEW(TouchBlock);

function TouchBlock:getUIRect()
    return self.rect_ ;
end

function TouchBlock:Init()
    self.rect_ = cc.rect(0,0,0,0);
    UI.Init(self,true,-2);
    print("TouchBlock:Init");
end

function TouchBlock:SetUIRect(v)
    self.rect_ = v;
end

function TouchBlock:onTouchBegan(t,e)
    
    local pos = self:convertTouchToNodeSpace(t);
    local result = cc.rectContainsPoint(self:getUIRect(),pos);
    if result then 
        print("not block");
        return false;
    else 
        print("block here..");
        return true;
    end
end

function TouchBlock:onTouchMove(t,e)

end

function TouchBlock:onTouchEnded(t,e)

end

function TouchBlock:onTouchCancled(t,e)

end

return TouchBlock;