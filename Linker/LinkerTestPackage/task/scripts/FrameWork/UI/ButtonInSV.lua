requirePack("scripts.FrameWork.Global.GlobalFunctions");
local Button = requirePack("scripts.FrameWork.UI.Button");
local SpriteUtil = requirePack("scripts.FrameWork.Util.SpriteUtil");
local ButtonInSV = class("ButtonInSV",function()
    return Button.new(); 
end);
ButtonInSV.EventType = {
}
g_tConfigTable.CREATE_NEW(ButtonInSV);

function ButtonInSV:onTouchBegan(t,e)
    local nowScene = cc.Director:getInstance():getRunningScene();
    local pos = nowScene:convertTouchToNodeSpace(t);
    self.posBefore_ = pos;
    return Button.onTouchBegan(self,t,e);
end

function ButtonInSV:onTouchEnded(t,e)
    local nowScene = cc.Director:getInstance():getRunningScene();
    local pos = nowScene:convertTouchToNodeSpace(t);
    if cc.pGetDistance(pos,self.posBefore_)>1 then 
        self:showStateToOff();
    else
        Button.onTouchEnded(self,t,e);
    end
end

-------------------对外接口-------------------------------
--[[
    spNormalName,spDownName:放在bgimg 下的图片名，包括后缀 .png
]]--
ButtonInSV.Create = function(spNormalName,spDownName,pos,parent) 
    local btn = ButtonInSV.new();
    btn.spNormalName_ = spNormalName;
    btn:Init(SpriteUtil.Create(spNormalName),SpriteUtil.Create(spDownName));
    parent:addChild(btn);
    btn:setPosition(pos);
    --btn.oriPos = pos;
    return btn;
end

return ButtonInSV;

--[[
    -- example 1
        -- 创建arm
    self.btnCreateNewArm_ = ButtonInSV.Create("btnMove.png","btnStart.png",cc.p(CFG_X(158),CFG_GL_Y(932)),self);
    self.btnCreateNewArm_:AddListener(ButtonInSV.EventType.Click,function() 
        self:CreateObjAtPos("",g_tConfigTable.Director.midPos);
    end);
]]--