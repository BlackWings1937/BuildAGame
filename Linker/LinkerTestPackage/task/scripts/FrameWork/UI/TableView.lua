-- TableView，通过SetContent/GetContent设置/获取TableView中的内容物
-- 使用TableView，可以在TableView中Stencil区域内移动Content
-- 移动限制于Stencil的Size以及Content的Size

-- TableView的锚点位于(0, 1.0)
-- SetContent会将Content的锚点设为(0, 1.0)

requirePack("scripts.FrameWork.Global.GlobalFunctions");
local UI = requirePack("scripts.FrameWork.UI.UI");
local SpriteUtil = requirePack("scripts.FrameWork.Util.SpriteUtil");

local TableView = class("TableView", function()
    return UI.new();
end);
g_tConfigTable.CREATE_NEW(TableView);

-- 默认Stencil名字
TableView.DEFAULT_STENCIL_NAME = "spDefalutStencil.png";
-- TableView滑动轴
TableView.SlideAxis = {
    ["VERTICAL"] = "VERTICAL",          -- 垂直
    ["HORIZONAL"] = "HORIZONAL",        -- 水平
    ["UNLIMITED"] = "UNLIMITED",        -- 无限制
};
-------------------- 私有/帮助函数 --------------------
function TableView:Init(size,stencilName)
    -- 基类初始化
    UI.Init(self,false);

    -- 初始化ClipingNode
    self:InitClippingNode(size,stencilName);

    -- 初始化滑动（默认为垂直）
    self.slideAxis_ = TableView.SlideAxis.UNLIMITED;
    -- 初始化Content为空
    self.content_ = nil;
    self:SetContent(nil);
    -- 初始化用以Touch的信息
    self.lastTouchPos_ = cc.p(0,0);
end
function TableView:InitClippingNode(size, stencilName)
    -- 创建用以裁剪的stencil
    stencilName = stencilName or TableView.DEFAULT_STENCIL_NAME;
    self.stencil_ = cc.Sprite:create( g_tConfigTable.sTaskpath.."bgimg/"..stencilName );
    -- 初始化stencil的Rect
    size = size or self.stencil_:getContentSize();
    local rect = cc.rect( 0,0,size.width,size.height );
    self.stencil_:setTextureRect(rect);

    -- 创建裁剪
    self.clip_ = cc.ClippingNode:create();
    self.clip_:setStencil(self.stencil_);
    self.stencil_:setAnchorPoint(cc.p(0,1.0));
    self.stencil_:setPosition(cc.p(0,0));
    self:addChild(self.clip_);
    self.clip_:setPosition(cc.p(0,0));
end

function TableView:HandleTouchDeltaHorizonal(deltaX)
    local content = self:GetContent();
    local contentPosX, contentPosY = content:getPosition();
    local destPos = cc.p( contentPosX+deltaX, contentPosY );
    self:SetContentPosition( destPos );
end

function TableView:HandleTouchDeltaVertical(deltaY)
    local content = self:GetContent();
    local contentPosX, contentPosY = content:getPosition();
    local destPos = cc.p( contentPosX, contentPosY + deltaY );
    self:SetContentPosition( destPos );
end

function TableView:HandleTouchDelta(delta)
    if self.slideAxis_ == TableView.SlideAxis.HORIZONAL or self.slideAxis_ == TableView.SlideAxis.UNLIMITED then
        self:HandleTouchDeltaHorizonal(delta.x);
    end
    if self.slideAxis_ == TableView.SlideAxis.VERTICAL or self.slideAxis_ == TableView.SlideAxis.UNLIMITED then
        self:HandleTouchDeltaVertical(delta.y);
    end
end

-------------------- override --------------------

-- 获得TableView下Stencil的Rect（相对于TablieView）
function TableView:getUIRect()
    return self:GetStencil():getBoundingBox();
end
function TableView:onTouchBegan(touch, event)
    print("TableView:onTouchBegan");
    self.lastTouchPos_ = self:convertTouchToNodeSpace(touch);
    return cc.rectContainsPoint(self:getUIRect(),self.lastTouchPos_);
end
function TableView:onTouchMove(touch, event)
    print("TableView:onTouchMove");

    local newTouchPos = self:convertTouchToNodeSpace(touch);
    if self:GetContent() ~= nil then
        self:HandleTouchDelta(cc.pSub( newTouchPos, self.lastTouchPos_ ));
    end
    self.lastTouchPos_ = newTouchPos;
end
function TableView:onTouchEnded(touch, event)
    print("TableView:onTouchEnded");
end

-------------------- 外部接口 --------------------

-- 创建一个TableView的实例，参数为裁剪区域大小和裁剪模板
TableView.Create = function(size,stencilName)
    local tableView = TableView.new();
    tableView:Init(size,stencilName);
    return tableView;
end

-- 设置裁剪区域
function TableView:SetStencil(stencil)
    self.clip_:setStencil(stencil);
end
-- 获取裁剪区域
function TableView:GetStencil()
    return self.clip_:getStencil();
end
-- 设置裁剪区域的Size
function TableView:SetStencilSize(size)
    local rect = cc.rect(0,0,size.width,size.height);
    self:GetStencil():setTextureRect(rect);
end
-- 获取裁剪区域的Size
function TableView:GetStencilSize()
    local rect = self:getUIRect();
    local size = cc.size(rect.width,rect.height);
    return size;
end


-- 设置滑动轴方向
function TableView:SetSlideAxis(slideAxis)
    self.slideAxis_ = slideAxis;
end
-- 得到滑动轴方向
function TableView:GetSlideAxis()
    return self.slideAxis_;
end


-- 设置TableView的Content
function TableView:SetContent(content)
    if self.content_ ~= nil then
        self.content_:removeFromParent();
    end
    
    self.content_ = content;
    if self.content_ ~= nil then
        self.clip_:addChild(self.content_);
        self.content_:setAnchorPoint(cc.p(0,1.0));
        self.content_:setPosition(cc.p(0,0));
    end
end
-- 获取TableView的Content
function TableView:GetContent()
    return self.content_;
end

-- 设置Content的Position，将会检查合法性
function TableView:SetContentPosition(pos)
    local content = self:GetContent();
    if content == nil then
        return;
    end

    local contentRect = content:getBoundingBox();
    local contentSize = cc.size(contentRect.width, contentRect.height);

    local stencilSize = self:GetStencilSize();

    if pos.x + contentSize.width < stencilSize.width then
        pos.x = stencilSize.width - contentSize.width;
    end
    if pos.x > 0 then
        pos.x = 0;
    end
    if pos.y - contentSize.height > -stencilSize.height then
        pos.y = -stencilSize.height + contentSize.height;
    end
    if pos.y < 0 then
        pos.y = 0;
    end
    content:setPosition(pos);
end

-- 移动到顶部
function TableView:MoveToTop()
    local content = self:GetContent();
    if content == nil then
        return;
    end

    local contentPosX, contentPosY = content:getPosition();
    self:SetContentPosition( cc.p( contentPosX,0 ) );
end
-- 移动到底部
function TableView:MoveToBottom()
    local content = self:GetContent();
    if content == nil then
        return;
    end

    local contentPosX, contentPosY = content:getPosition();
    local contentRect = content:getBoundingBox();
    local contentSize = cc.size(contentRect.width, contentRect.height);
    local stencilSize = self:GetStencilSize();
    self:SetContentPosition( cc.p( contentPosX,-stencilSize.height + contentSize.height ) );
end
-- 移动到
function TableView:MoveToLeft()
    local content = self:GetContent();
    if content == nil then
        return;
    end

    local contentPosX, contentPosY = content:getPosition();
    self:SetContentPosition( cc.p(0,contentPosY) );
end
function TableView:MoveToRight()
    local content = self:GetContent();
    if content == nil then
        return;
    end

    local contentPosX, contentPosY = content:getPosition();
    local contentRect = content:getBoundingBox();
    local contentSize = cc.size(contentRect.width, contentRect.height);
    local stencilSize = self:GetStencilSize();
    self:SetContentPosition( cc.p( -contentSize.width + stencilSize.width, contentPosY ) );
end

return TableView;