requirePack("scripts.FrameWork.Global.GlobalFunctions");
local SpriteUtil = requirePack("scripts.FrameWork.Util.SpriteUtil");
local UI = requirePack("scripts.FrameWork.UI.UI");
local TouchBlock = requirePack("scripts.FrameWork.UI.TouchBlock");
local ScrollView = class("ScrollView",function()
    return UI.new(); 
end);
ScrollView.EventType = {
}
g_tConfigTable.CREATE_NEW(ScrollView);

ScrollView.TAG_ACTION_SCROLL_PAGE = 1001;

ScrollView.ScrollType = {
    ["E_HORIZONTAL"] = 1,
    ["E_VERTICAL"] = 2,
}

function ScrollView:ctor()
    self.sizeOfContent_ = cc.size(0,0);
    --[[
    self.tb_ = TouchBlock.new();
    self.tb_:Init();
    self:addChild(self.tb_);
    self.tb_:setZOrder(1);
    ]]--
end

function ScrollView:onTouchBegan(t,e)
    local pos = self:convertTouchToNodeSpace(t);
    local result = cc.rectContainsPoint(self:getUIRect(),pos);
    self.startPos_ = pos;
    local x,y = self.content_:getPosition();
    self.startContentPos_ = cc.p(x,y);
    return true;
end

function ScrollView:onTouchMove(t,e)
    local pos = self:convertTouchToNodeSpace(t);
    local result = cc.rectContainsPoint(self:getUIRect(),pos);
    if result then 
        if self.mt_  == ScrollView.ScrollType.E_HORIZONTAL then 
            local ori = cc.pSub(pos,self.startPos_);
            local aimX = self.startContentPos_.x + ori.x;
            aimX = math.max(self:contentRightValue(),aimX);
            aimX = math.min(self:contentLeftValue(),aimX);
            self.content_:setPositionX(aimX);
        elseif self.mt_ == ScrollView.ScrollType.E_VERTICAL then 
            local ori = cc.pSub(pos,self.startPos_);
            local aimY = self.startContentPos_.y + ori.y;
            aimY = math.max(self:contentTopValue(),aimY);
            aimY = math.min(self:contentBottomValue(),aimY);
            self.content_:setPositionY(aimY);
        end
        if self.updateScrollCb_ ~= nil then 
            self.updateScrollCb_(self:getRate());
        end

        self:updateCellSvRect();
    end
    

end

function ScrollView:getRate()
    local dis = self:contentBottomValue() - self:contentTopValue();
    local upY = self.content_:getPositionY() - self:contentTopValue();
    return upY/dis;
end

function ScrollView:updateCellSvRect()
    if self.content_ ~= nil then 
        if self.content_.UpdateScrollViewMoved ~= nil then 
            self.content_:UpdateScrollViewMoved(function(worldPos)
                local swPos = self:convertToNodeSpace(worldPos);
                return cc.rectContainsPoint(self:getUIRect(),swPos);
            end);
        end
    end
end

function ScrollView:onTouchEnded(t,e)

end

function ScrollView:onTouchCancled(t,e)
 
end
function ScrollView:getUIRect()
    local size = self:getContentSize();
    return cc.rect(-size.width/2,-size.height/2,size.width,size.height);
end


function ScrollView:contentTopValue()
    local sizeOfContent =self.sizeOfContent_ ;
    local sizeOfScroll = self:getContentSize();
    return sizeOfScroll.height/2-sizeOfContent.height/2;
end

function ScrollView:contentBottomValue()
    local sizeOfContent =self.sizeOfContent_ ;
    local sizeOfScroll = self:getContentSize();
    return sizeOfContent.height/2-sizeOfScroll.height/2;
end

function ScrollView:contentLeftValue()
    local sizeOfContent =self.sizeOfContent_ ;
    local sizeOfScroll = self:getContentSize();
    return sizeOfContent.width/2 - sizeOfScroll.width/2; 
end

function ScrollView:contentRightValue()
    local sizeOfContent = self.sizeOfContent_ ;
    local sizeOfScroll = self:getContentSize();
    return sizeOfScroll.width/2 - sizeOfContent.width/2;
end
----------对外接口------------

function ScrollView:SetByRate(rate)
    local dis = self:contentBottomValue() - self:contentTopValue();
    dis = dis * rate;
    local y = self:contentTopValue() + dis;
    self.content_:setPositionY(y);
    self:updateCellSvRect();
end

function ScrollView:SetScrollUpdateCallBack(v)
    self.updateScrollCb_ = v;
end

function ScrollView:Init(stencilName,mt)
    UI.Init(self,true,0);
    self.mt_ = mt;
    local stencil = SpriteUtil.Create(stencilName);
    local clipNode =  cc.ClippingNode:create(stencil);
    self:addChild(clipNode);
    self.clipNode_ = clipNode;
    self:setContentSize(cc.size(
        stencil:getContentSize().width*stencil:getScale(),
        stencil:getContentSize().height*stencil:getScale()
    ));
end

function ScrollView:InitBySize(size,mt)
    UI.Init(self,true,1);
    local bg =  cc.Sprite:create(g_tConfigTable.sTaskpath .. "bgimg/sp9Scale.png",cc.rect(0,0,size.width,size.height));
    self:addChild(bg);
    self.mt_ = mt;
    local stencil = cc.Sprite:create(g_tConfigTable.sTaskpath .."bgimg/sp9Scale.png",cc.rect(0,0,size.width,size.height));
    local clipNode =  cc.ClippingNode:create(stencil);
    self:addChild(clipNode);
    self.clipNode_ = clipNode;
    self:setContentSize(cc.size(
        stencil:getContentSize().width*stencil:getScale(),
        stencil:getContentSize().height*stencil:getScale()
    ));--cc.rect(-size.width/2,-size.height/2,size.width,size.height)
    --self.tb_:SetUIRect(cc.rect(-1000,-1000,100000,100000));
end

function ScrollView:SetContent(content)
    if self.content_ ~= nil then 
        self.content_:removeFromParent();
        self.content_ = nil;
    end
    self.content_ = content;
    self:UpdateContentSize();
    self.clipNode_:addChild(self.content_);
    self.content_:setZOrder(10000);
end

function ScrollView:UpdateContentSize()
    local sizeOfScroll = self:getContentSize();
    self.sizeOfContent_ = cc.size(self.content_:getContentSize().width*self.content_:getScale(),self.content_:getContentSize().height*self.content_:getScale()) ;
    self.sizeOfContent_.width = math.max(self.sizeOfContent_.width,sizeOfScroll.width);
    self.sizeOfContent_.height = math.max(self.sizeOfContent_.height,sizeOfScroll.height);

end


function ScrollView:PlaceToTop()
    if self.content_ ~= nil then 
        self.content_:setPositionY(self:contentTopValue());
        self:updateCellSvRect();
        if self.updateScrollCb_ ~= nil then 
            self.updateScrollCb_(self:getRate());
        end
    end
end

function ScrollView:PlaceToBottom()
    if self.content_ ~= nil then 
        self.content_:setPositionY(self:contentBottomValue());
        self:updateCellSvRect();
        if self.updateScrollCb_ ~= nil then 
            self.updateScrollCb_(self:getRate());
        end
    end
end

function ScrollView:PlaceToLeft()
    if self.content_ ~= nil then 
        self.content_:setPositionX(self:contentLeftValue());
        self:updateCellSvRect();
        if self.updateScrollCb_ ~= nil then 
            self.updateScrollCb_(self:getRate());
        end
    end
end

function ScrollView:PlaceToRight()
    if self.content ~= nil then 
        self.content_:setPositionX(self:contentRightValue());
        self:updateCellSvRect();
        if self.updateScrollCb_ ~= nil then 
            self.updateScrollCb_(self:getRate());
        end
    end
end

return ScrollView;