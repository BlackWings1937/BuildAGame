requirePack("scripts.FrameWork.Global.GlobalFunctions");
local ScrollView = requirePack("scripts.FrameWork.UI.ScrollView");
local LayoutGroup = requirePack("scripts.FrameWork.UI.LayoutGroup");
local Slider = requirePack("scripts.FrameWork.UI.Slider");
local CellArmature = requirePack("scripts.FrameWork.UI.CellArmature");

local ScrollViewArmatures = class("ScrollViewArmatures",function()
    return cc.Node:create(); 
end);
ScrollViewArmatures.EventType = {
}
g_tConfigTable.CREATE_NEW(ScrollViewArmatures);

function ScrollViewArmatures:ctor()

    self.cacheList_ = nil;

    self.sw_ = ScrollView.new();
    self.sw_ :InitBySize(cc.size(1092.25,302),ScrollView.ScrollType.E_VERTICAL);
    self:addChild(self.sw_ );

    self.content_ = LayoutGroup.new();
    self.content_:Init(20,20,0,0,20,LayoutGroup.LayoutType.VERTICAL);
    self.sw_:SetContent(self.content_);
    self.sw_:PlaceToTop();

    self.slider_ = Slider.new();
    self.slider_:SetType(Slider.Type.VERTICAL);
    self.slider_:Init("spSliderBg.png","spSliderBg.png","spTableViewHandle.png");
    self:addChild(self.slider_);
    self.slider_:setPosition(cc.p(1090/2 -16,300/2));
    self.slider_:AddListener(Slider.EventType.VALUE_CHANGE,function(rate)
        self.sw_:SetByRate(rate);
    end)

    self.sw_:SetScrollUpdateCallBack(function(rate) 
        self.slider_:SetStatue(rate);
    end);
end

function ScrollViewArmatures:SetEnable(v)
    self.sw_:setVisible(v);
    self.slider_:setVisible(v);
end

function ScrollViewArmatures:UpdateByList(list)
    self.content_:RemoveAllCell();
    local count = #list;
    for i = 1,count,1 do 
        local cell = CellArmature.new();
        cell:UpdateByData(list[i]);
        self.content_:AddCell(cell);
    end
    self.sw_:UpdateContentSize();
    self.sw_:PlaceToTop();
end


return ScrollViewArmatures;