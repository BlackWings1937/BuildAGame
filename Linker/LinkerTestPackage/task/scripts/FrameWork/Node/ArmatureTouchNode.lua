-- region *.lua
-- Date
-- 此文件由[BabeLua]插件自动生成
local ArmatureUtil = requirePack("scripts.FrameWork.Util.ArmatureUtil");
local ArmatureTouchNode = class("ArmatureTouchNode", function()
    return cc.Node:create();
end );
g_tConfigTable.CREATE_NEW(ArmatureTouchNode);

function ArmatureTouchNode:ctor()
    print("ctor1");
    self:initTouch();
end

function ArmatureTouchNode:SetTouchArmatureList(list)
    self.listOfTouchArmature_ = list;
end

function ArmatureTouchNode:AddTouchArmature(arm)
    if self.listOfTouchArmature_ == nil then
        self.listOfTouchArmature_ = { };
    end
    table.insert(self.listOfTouchArmature_, arm);
end

function ArmatureTouchNode:RemoveTouchArmature(arm)
    if self.listOfTouchArmature_ ~= nil then
        for i = #self.listOfTouchArmature_, 1, -1 do
            if arm == self.listOfTouchArmature_[i] then
                table.remove(self.listOfTouchArmature_, i);
            end
        end
    end
end

function ArmatureTouchNode:say(audio,arm,cb)
    g_tConfigTable.Director.CurrentScene:say(audio,arm,cb);
end

function ArmatureTouchNode:sayAuto(audio,arm,sayIndex,idleIndex,cb)
    arm:playByIndex(sayIndex,LOOP_YES);
    self:say(audio,nil,function ()
        arm:playByIndex(idleIndex,LOOP_YES);
        if cb~= nil then 
            cb();
        end
    end);
end


-----------------触碰注册方法---------------------
function ArmatureTouchNode:setIsTouchOn(isOn)
    self.isTouchOn_ = isOn;
end

function ArmatureTouchNode:initTouch()
    print("initTouch1");
    self.isTouchOn_ = true;
    local listener = cc.EventListenerTouchOneByOne:create();
    listener:setSwallowTouches(false);
    listener:registerScriptHandler( function(t, v)
        if self.isTouchOn_ then
            self:onTouchBegan(t, v);
        end
        return self.isTouchOn_;
    end , cc.Handler.EVENT_TOUCH_BEGAN)
    listener:registerScriptHandler( function(t, v)
        if self.isTouchOn_ then
            self:onTouchMoved(t, v);
        end

    end , cc.Handler.EVENT_TOUCH_MOVED)
    listener:registerScriptHandler( function(t, v)
        if self.isTouchOn_ then
            self:onTouchEnded(t, v);
        end
    end , cc.Handler.EVENT_TOUCH_ENDED)
    listener:registerScriptHandler( function(t, v)
        if self.isTouchOn_ then
            self:onTouchCancle(t, v);
        end
    end , cc.Handler.EVENT_TOUCH_CANCELLED)
    local eventDispatcher = self:getEventDispatcher();
    eventDispatcher:addEventListenerWithFixedPriority(listener, -1);
    self.listener_ = listener;
end

function ArmatureTouchNode:disposeTouch()
    local eventDispatcher = self:getEventDispatcher();
    eventDispatcher:removeEventListener(self.listener_);
end

function ArmatureTouchNode:onExit()
    self:disposeTouch();
    if self.listOfTouchArmature_ ~= nil then
        self.listOfTouchArmature_ = nil;
    end
end


function ArmatureTouchNode:onTouchBegan(t, e)
    local touchResult = false;
    if self.listOfTouchArmature_ ~= nil then
        for i = 1, #self.listOfTouchArmature_, 1 do
            local touchArmature = self.listOfTouchArmature_[i];
            local pos = touchArmature:convertTouchToNodeSpace(t);
            print("pos:"..pos.x.."y:"..pos.y);
            if touchArmature ~= nil then
                local rect = ArmatureUtil.GetRect(touchArmature);
                print("width:"..rect.width.."height:"..rect.height);
                if cc.rectContainsPoint(cc.rect(-rect.width/2,-rect.height/2,rect.width,rect.height), pos) then
                    touchResult = true;
                end
            end
        end
    end
    return touchResult;
end

function ArmatureTouchNode:onTouchMoved(t, e)
    if self.listOfTouchArmature_ ~= nil then
        for i = 1, #self.listOfTouchArmature_, 1 do
            local touchArmature = self.listOfTouchArmature_[i];
            if touchArmature ~= nil then

            end
        end
    end
end

function ArmatureTouchNode:onTouchCancle (t, e)
    if self.listOfTouchArmature_ ~= nil then
        for i = 1, #self.listOfTouchArmature_, 1 do
            local touchArmature = self.listOfTouchArmature_[i];
            if touchArmature ~= nil then

            end
        end
    end
end

function ArmatureTouchNode:onTouchEnded(t, e)
    
    if self.listOfTouchArmature_ ~= nil then
        for i = 1, #self.listOfTouchArmature_, 1 do
            local touchArmature = self.listOfTouchArmature_[i];
            local pos = touchArmature:convertTouchToNodeSpace(t);
            if touchArmature ~= nil then
                if touchArmature.OnClick ~= nil then
                    local rect = ArmatureUtil.GetRect(touchArmature);
                    if cc.rectContainsPoint(cc.rect(-rect.width/2,-rect.height/2,rect.width,rect.height), pos) then
                        touchArmature.OnClick(touchArmature);
                    end
                end
            end
        end
    end
end

return ArmatureTouchNode;


