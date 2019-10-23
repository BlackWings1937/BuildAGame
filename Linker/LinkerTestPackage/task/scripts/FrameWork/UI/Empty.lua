requirePack("scripts.FrameWork.Global.GlobalFunctions");
local UI = requirePack("scripts.FrameWork.UI.UI");
local SpriteUtil = requirePack("scripts.FrameWork.Util.SpriteUtil");
local Empty = class("Empty",function()
    return UI.new(); 
end);
Empty.EventType = {
}
g_tConfigTable.CREATE_NEW(Empty);

function Empty:ctor()

end
function Empty:getUIRect()
    return cc.rect(0,0,0,0);
end

function Empty:GetUIType()
    print("Empty");
end
function Empty:onTouchBegan(t,e)
    return false;
end
function Empty:onTouchEnded(t,e)

end
function Empty:onTouchCancled(t,e)

end
------------------对外接口-----------------------
function Empty:Init()
    UI.Init(self);

end

return Empty;

--[[
    -- example 1
        -- 创建arm
    self.btnCreateNewArm_ = Empty.Create("btnMove.png","btnStart.png",cc.p(CFG_X(158),CFG_GL_Y(932)),self);
    self.btnCreateNewArm_:AddListener(Empty.EventType.Click,function() 
        self:CreateObjAtPos("",g_tConfigTable.Director.midPos);
    end);
]]--