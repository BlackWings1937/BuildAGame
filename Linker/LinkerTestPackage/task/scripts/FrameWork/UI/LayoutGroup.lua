requirePack("scripts.FrameWork.Global.GlobalFunctions");
local UI = requirePack("scripts.FrameWork.UI.UI");
local SpriteUtil = requirePack("scripts.FrameWork.Util.SpriteUtil");
local LayoutGroup = class("LayoutGroup",function()
    return UI.new(); 
end);
LayoutGroup.EventType = {
}
g_tConfigTable.CREATE_NEW(LayoutGroup);

LayoutGroup.LayoutType = {
    ["NoneInit"] = 1,
    ["HORIZONTAL"] = 2,
    ["VERTICAL"] = 3,
};

function LayoutGroup:ctor()
    self.paddingTop_ = 0;
    self.paddingBottom_ = 0;
    self.paddingLeft_ = 0;
    self.paddingRight_ = 0;
    self.rear_ = 0;
    self.listOfCell_ = {};

    self.mt_ = LayoutGroup.LayoutType.NoneInit;
    self.content_ = nil;
end

function LayoutGroup:getUIRect()
    return cc.rect(0,0,0,0);
end

function LayoutGroup:GetUIType()
end


function LayoutGroup:getMaxHeightCellValue()
    local cellCount = #self.listOfCell_;
    local maxCellHeight = 0;
    for i = 1,cellCount,1 do
        local cell = self.listOfCell_[i];
        local cellSize = cell:getContentSize();
        maxCellHeight = math.max(maxCellHeight,cellSize.height);
    end
    return maxCellHeight;
end

function LayoutGroup:getMaxWidthCellValue()
    local cellCount = #self.listOfCell_;
    local maxCellWidth = 0;
    for i = 1,cellCount,1 do 
        local cell = self.listOfCell_[i];
        local cellSize = cell:getContentSize();
        maxCellWidth = math.max(maxCellWidth,cellSize.width);
    end
    return maxCellWidth;
end

function LayoutGroup:updateContentSize()
    local cellCount = #self.listOfCell_;
    local size = cc.size(0,0);
    if self.mt_ == LayoutGroup.LayoutType.HORIZONTAL then 
        size = cc.size(self.paddingLeft_+self.paddingRight_, self.paddingBottom_+self.paddingTop_+self:getMaxHeightCellValue());
        for i = 1,cellCount,1 do 
            local cell = self.listOfCell_[i];
            local cellSize = cell:getContentSize();
            size.width = size.width + cellSize.width;
        end

    elseif self.mt_ == LayoutGroup.LayoutType.VERTICAL then 
        size = cc.size(self.paddingLeft_+self.paddingRight_+self:getMaxWidthCellValue(),self.paddingBottom_+self.paddingTop_);
        for i = 1,cellCount,1 do 
            local cell = self.listOfCell_[i];
            local cellSize = cell:getContentSize();
            size.height = size.height+cellSize.height +  self.dis_;
        end
    end
    self:setContentSize(size);
end

function LayoutGroup:initRear()
    if self.mt_ == LayoutGroup.LayoutType.HORIZONTAL then 
        self.rear_ = self.paddingLeft_;
    elseif self.mt_ == LayoutGroup.LayoutType.VERTICAL then 
        self.rear_ = -self.paddingTop_;
    end
end

function LayoutGroup:updateContent()
    local size = self:getContentSize();
    if self.mt_ == LayoutGroup.LayoutType.HORIZONTAL then 
        self.content_:setPositionX(-size.width/2);
    elseif self.mt_ == LayoutGroup.LayoutType.VERTICAL then 
        self.content_:setPositionY(size.height/2);
    end
end


------------------对外接口-----------------------
function LayoutGroup:UpdateScrollViewMoved(cb)
    local list = self.content_:getChildren();
    for i=1,#list , 1 do 
        local obj = list[i];
        if obj ~= nil then
            if obj.SWUpdateCallBack ~= nil then 
                obj:SWUpdateCallBack(cb);
            end
        end
    end
end
function LayoutGroup:Init(paddingTop,paddingBottom,paddingLeft,paddingRight,dis,mt)
    UI.Init(self,false,-1);
    self.mt_ = mt;
    self.paddingTop_    = paddingTop;
    self.paddingBottom_ = paddingBottom;
    self.paddingLeft_   = paddingLeft;
    self.paddingRight_  = paddingRight;
    self.dis_ = dis;
    self:SetEnabled(false);
    self:initRear();
    self:updateContentSize();

    self.content_ = cc.Node:create();
    self:addChild(self.content_);
end

function LayoutGroup:AddCell(cell)
    table.insert(self.listOfCell_,cell);
    self.content_:addChild(cell);
    local sizeOfCell = cell:getContentSize();
    if self.mt_ == LayoutGroup.LayoutType.HORIZONTAL then 
        cell:setPositionX(self.rear_+sizeOfCell.width/2);
        self.rear_ = self.rear_ + self.dis_ + sizeOfCell.width;
    elseif self.mt_ == LayoutGroup.LayoutType.VERTICAL then 
        cell:setPositionY(self.rear_ - sizeOfCell.height/2);
        self.rear_ = self.rear_ -  self.dis_  - sizeOfCell.height;
    end
    self:updateContentSize();
    self:updateContent();
end

function LayoutGroup:RemoveAllCell()
    local count = #self.listOfCell_;
    for i = count,1,-1 do 
        local cell = self.listOfCell_[i];
        table.remove(self.listOfCell_,i);
        print("delete time..");
        --cell:removeAllChildren();
        cell:removeFromParent();
    end
    self:initRear();
    self:updateContentSize();
    self:updateContent();
end

return LayoutGroup;
