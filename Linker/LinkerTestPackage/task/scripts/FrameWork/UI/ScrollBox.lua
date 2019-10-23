requirePack("scripts.FrameWork.Global.GlobalFunctions");
local SpriteUtil = requirePack("scripts.FrameWork.Util.SpriteUtil");
local UI = requirePack("scripts.FrameWork.UI.UI");
local ScrollBox = class("ScrollBox",function()
    return UI.new(); 
end);
ScrollBox.EventType = {
}
g_tConfigTable.CREATE_NEW(ScrollBox);

ScrollBox.TAG_ACTION_SCROLL_PAGE = 1001;

function ScrollBox:ctor()
    self.width_ = 0;
    self.height_ = 0;
    self.maxCow_ = 0;
    self.maxRow_ = 0;
    self.origTouchPos_ = cc.p(0,0);
    self.pageCount_ = 0;
end

function ScrollBox:GetPageCount()
    return self.pageCount_;
end

function ScrollBox:GetNowForcusPage()
    return self.nowForcusPage_;
end

function ScrollBox:GetUIType()
    print("ScrollBox");
end
function ScrollBox:getPosByIndex(index)
    local count = index;
    local pageCount = self.maxCow_ * self.maxRow_;
    local page = math.modf(count/pageCount);
    if count %pageCount ~= 0 then 
        page = page + 1;
    end
    local pagePos = cc.p((page - 1)*self.width_,0);
    local pageIndex = count%pageCount;
    if pageIndex == 0 then 
        pageIndex = pageCount;
    end
    local pageIndexPos = self.listOfPoes_[pageIndex];
    return cc.pAdd(pageIndexPos,pagePos);
end

function ScrollBox:updatePageCount()
    local pageCount = math.modf(#self.listOfItems_/(self.maxCow_ * self.maxRow_));
    if (#self.listOfItems_)%(self.maxCow_ * self.maxRow_)~= 0 then 
        pageCount = pageCount + 1;
    end
    self.pageCount_ = pageCount;
end

function ScrollBox:getUIRect()
    return cc.rect(-self.width_/2,-self.height_/2,self.width_,self.height_);
end


function ScrollBox:onTouchBegan(t,e)
    local pos = self:convertTouchToNodeSpace(t);
    local result = cc.rectContainsPoint(self:getUIRect(),pos);
    if result then 
        self.origTouchPos_ = pos;
    end

    return result;
end

function ScrollBox:onTouchEnded(t,e)
    local pos = self:convertTouchToNodeSpace(t);
    if pos.x> self.origTouchPos_.x then 
        -- move right
        self:moveRightPage();
    elseif pos.x< self.origTouchPos_.x then 
        -- move left
        self:moveLeftPage();
    end
end

function ScrollBox:onTouchCancled(t,e)
    local pos = self:convertTouchToNodeSpace(t);
    if pos.x> self.origTouchPos_.x then 
        -- move right
        self:moveRightPage();
    elseif pos.x< self.origTouchPos_.x then 
        -- move left
        self:moveLeftPage();
    end
end

function ScrollBox:moveRightPage()
    if self.nowForcusPage_ > 1  then 
        local action = self.content_:getActionByTag(ScrollBox.TAG_ACTION_SCROLL_PAGE);
        if action == nil then 
            action = cc.MoveBy:create(0.5,cc.p(self.width_,0));
            self.content_:runAction(action);
            self.nowForcusPage_ = self.nowForcusPage_ - 1;
        end
    end
end

function ScrollBox:moveLeftPage()
    print("self.nowForcusPage_:"..self.nowForcusPage_.."self.pageCount_:"..self.pageCount_);
    if self.nowForcusPage_< self.pageCount_ then 
        local action = self.content_:getActionByTag(ScrollBox.TAG_ACTION_SCROLL_PAGE);
        if action == nil then 
            action = cc.MoveBy:create(0.5,cc.p(-self.width_,0));
            self.content_:runAction(action);
            self.nowForcusPage_ = self.nowForcusPage_ + 1;
        end
    end
end

----------对外接口------------

function ScrollBox:MoveRightPage()
    self:moveRightPage();
end

function ScrollBox:MoveLeftPage()
    self:moveLeftPage();
end
function ScrollBox:Init(spStencilName)
    UI.Init(self);
    self.stencil_ = SpriteUtil.Create(spStencilName);
    self.width_ = self.stencil_:getContentSize().width * self.stencil_:getScale();
    self.height_ = self.stencil_:getContentSize().height * self.stencil_:getScale();
    self.clip_ = cc.ClippingNode:create(self.stencil_);
    self:addChild(self.clip_);
    self.content_ = cc.Node:create();
    self.clip_:addChild(self.content_);
    self.listOfItems_ = {};
    self.pageCount_ = 1;
    self.nowForcusPage_ = 1;
    self.contentWidthScale_ = 0.6;
    self.contentHeightScale_ = 0.6;
    self.tempPos_ = cc.p(0,0);
    self:SetSwallowTouches(false);
end

function ScrollBox:setTempPos(v)
    self.tempPos_ = v;
end

function ScrollBox:SetContentWidthScale(v)
    self.contentWidthScale_ = v;
end

function ScrollBox:SetContentHeightScale(v)
    self.contentHeightScale_ = v;
end

function ScrollBox:SetMaxCowRow(cow,row)
    self.maxCow_ = cow;
    self.maxRow_ = row;
    local groupWidth = self.width_ * self.contentWidthScale_;
    local groupHeight = self.height_* self.contentHeightScale_;
    local startPos = cc.p(-groupWidth/2,-groupHeight/2);
    local partWidth = 0;
    if cow>1 then 
        partWidth = groupWidth/(cow-1);
    else
        startPos.x = 0;
    end
    local partHeight = 0;
    if row>1 then 
        partHeight = groupHeight/(row-1);
    else 
        startPos.y = 0;
    end
    self.listOfPoes_ = {}; 
    for i = self.maxRow_,1,-1 do 
        for z = 1,self.maxCow_,1 do 
            table.insert(self.listOfPoes_,cc.pAdd(startPos,cc.p(partWidth*(z-1),partHeight*(i-1))));
        end
    end
end

function ScrollBox:AddItem(item)
    table.insert(self.listOfItems_,item);
    self.content_:addChild(item);
    local pos = self:getPosByIndex(#self.listOfItems_);
    item:setPosition(cc.pAdd(pos,self.tempPos_) );
    self:updatePageCount();
end

function ScrollBox:RemoveItem(item)
    for i = #self.listOfItems_,1,-1 do 
        if self.listOfItems_[i] == item then 
            table.remove(self.listOfItems_,i);
        end
    end
    item:removeFromParent();
    for i = #self.listOfItems_,1,-1 do 
        self.listOfItems_[i]:setPosition(self:getPosByIndex(i));
    end
    self:updatePageCount();
end

function ScrollBox:GetListOfItems()
    return self.listOfItems_;
end

return ScrollBox;

--[[  
    example 1
    self.scrollBox_ = ScrollBox.new();
    self.scrollBox_:Init("spJiazi.png");
    self.scrollBox_:SetMaxCowRow(3,2);
    self:addChild(self.scrollBox_);
    self.scrollBox_:setPosition(cc.p(CFG_X(382.1),CFG_GL_Y(302.05)));
    self.listOfMovie_ = {};
    for i = 1,8,1 do 
        local sp = SpriteUtil.Create("spMovieBoxBg.png");
        table.insert( self.listOfMovie_,sp);
        self.scrollBox_:AddItem(sp);
    end

            self.scrollBox_:RemoveItem(self.listOfMovie_[#self.listOfMovie_]);
        table.remove(self.listOfMovie_,#self.listOfMovie_);
      ]]--