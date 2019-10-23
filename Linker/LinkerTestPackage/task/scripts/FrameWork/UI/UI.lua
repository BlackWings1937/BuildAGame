requirePack("scripts.FrameWork.Global.GlobalFunctions");
local UI = class("UI",function()
    return cc.Node:create(); 
end);
UI.EventType = {
}
g_tConfigTable.CREATE_NEW(UI);

function UI:getUIRect()
    print("warning! need override UI method getUIRect");
    return cc.rect(0,0,0,0);
end

function UI:GetUIType()
    print("UI");
end

function UI:Init(isSwall,priority)
    self.enable_ = true;
    self.events_ = {};
    self:enableNodeEvents();
    local listener = cc.EventListenerTouchOneByOne:create();
    listener:setSwallowTouches(false);
    listener:registerScriptHandler( function(t, v)
        if self:isVisible() == false then 
            return false;
        end
        local pos = self:convertTouchToNodeSpace(t);
        if self.enable_ then 
            return self:onTouchBegan(t,v);
        else 
            return false;
        end
    end , cc.Handler.EVENT_TOUCH_BEGAN)
    listener:registerScriptHandler( function(t, v)
        self:onTouchMove(t,v);
    end , cc.Handler.EVENT_TOUCH_MOVED)
    listener:registerScriptHandler( function(t, v)
        self:onTouchEnded(t,v);
    end , cc.Handler.EVENT_TOUCH_ENDED)
    listener:registerScriptHandler( function(t, v)
        self:onTouchCancled(t,v);
    end , cc.Handler.EVENT_TOUCH_CANCELLED)
    local eventDispatcher = self:getEventDispatcher();
    if isSwall ~= nil then 
        listener:setSwallowTouches(isSwall);
    else
        listener:setSwallowTouches(true);
    end
    if priority ~= nil then 
        eventDispatcher:addEventListenerWithFixedPriority(listener, priority);
    else
        eventDispatcher:addEventListenerWithFixedPriority(listener, -10);
    end
    self.listener_ = listener;
end

function UI:SetSwallowTouches(v)
    self.listener_:setSwallowTouches(v);
end

function UI:SetFixedPriority(index)--setPriority
    self:getEventDispatcher():setPriority(self.listener_,index);
end

function UI:onExit() 
    local eventDispatcher = self:getEventDispatcher();
    eventDispatcher:removeEventListener(self.listener_);
    self.events_ = {};
    print("UI:OnExit....");
end

function UI:onTouchBegan(t,e)
    return false;
end

function UI:onTouchMove(t,e)

end

function UI:onTouchEnded(t,e)

end

function UI:onTouchCancled(t,e)

end

function UI:AddListener(eventType,cb)
    local listOfEvents = self.events_[eventType];
    table.insert(listOfEvents,cb);
end

function UI:RemoveListener(eventType,cb)
    local listOfEvents = self.events_[eventType];
    for i = #listOfEvents,1,-1 do 
        if listOfEvents[i] == cb then 
            table.remove(listOfEvents,i);
        end
    end
end

function UI:SetPos(pos)
    pos.x =( pos.x /g_tConfigTable.Director.ContentSize.x ) * g_tConfigTable.Director.winSize.width ;
    pos.y = ((g_tConfigTable.Director.ContentSize.y - pos.y)/g_tConfigTable.Director.ContentSize.y)*g_tConfigTable.Director.winSize.height;
    self:setPosition(pos);
end

function UI:emit(eventType,...)
    if self.events_ ~= nil then 
        local listOfEvents = self.events_[eventType];
        if listOfEvents ~= nil then 
            for i = #listOfEvents,1,-1 do 
                listOfEvents[i](...);
            end
        end
    end
end

function UI:SetEnabled(v)
    self.enable_ = v;
end

function UI:GetEnabled()
    return self.enable_;
end


return UI;