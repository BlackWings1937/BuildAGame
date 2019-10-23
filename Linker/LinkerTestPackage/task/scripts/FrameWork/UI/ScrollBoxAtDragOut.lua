requirePack("scripts.FrameWork.Global.GlobalFunctions");
local SpriteUtil = requirePack("scripts.FrameWork.Util.SpriteUtil");
local UI = requirePack("scripts.FrameWork.UI.UI");
local ScrollBox = requirePack("scripts.FrameWork.UI.ScrollBox");
local ScrollBoxAtDragOut = class("ScrollBoxAtDragOut",function()
    return ScrollBox.new(); 
end);
g_tConfigTable.CREATE_NEW(ScrollBoxAtDragOut);

function ScrollBoxAtDragOut:Skip()
    self.isSkip_ = true;
end

function ScrollBoxAtDragOut:NotSkip()
    self.isSkip_ = nil;
end

function ScrollBoxAtDragOut:onTouchEnded(t,e)
    if self.isSkip_ == true then 
        self.isSkip_ = nil;
    else 
        ScrollBox.onTouchEnded(self,t,e);
    end
end

function ScrollBoxAtDragOut:onTouchCancled(t,e)
    if self.isSkip_ == true then 
        self.isSkip_ = nil;
    else
        ScrollBox.onTouchCancled(self,t,e);
    end
end


return ScrollBoxAtDragOut;