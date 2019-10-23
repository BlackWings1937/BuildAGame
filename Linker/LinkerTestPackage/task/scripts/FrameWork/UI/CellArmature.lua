requirePack("scripts.FrameWork.Global.GlobalFunctions");
local ScrollView = requirePack("scripts.FrameWork.UI.ScrollView");
local LayoutGroup = requirePack("scripts.FrameWork.UI.LayoutGroup");
local CellBase = requirePack("scripts.FrameWork.UI.CellBase");
local Button = requirePack("scripts.FrameWork.UI.Button");
local FactoryUtil = requirePack("scripts.FrameWork.Util.FactoryUtil");
local ArmatureUtil = requirePack("scripts.FrameWork.Util.ArmatureUtil");--ArmatureUtil.Create
local SpriteUtil = requirePack("scripts.FrameWork.Util.SpriteUtil");--ArmatureUtil.Create
local UIActionsUtil = requirePack("scripts.FrameWork.Util.UIActionsUtil");

local CellArmature = class("CellArmature",function()
    return CellBase.new(); 
end);
CellArmature.EventType = {
}
g_tConfigTable.CREATE_NEW(CellArmature);

function CellArmature:ctor()

    -- 四个编辑Box对应程序/动画名
    -- 程序/动画备注
    self.editBox1_ = FactoryUtil.CreateEditBox(cc.size(197,45),g_tConfigTable.sTaskpath.."bgimg/sp9Scale.png");
    self:addChild(self.editBox1_);
    self.editBox1_:setPosition(cc.p(-330,35));

    self.editBox2_ = FactoryUtil.CreateEditBox(cc.size(197,45),g_tConfigTable.sTaskpath.."bgimg/sp9Scale.png");
    self:addChild(self.editBox2_);
    self.editBox2_:setPosition(cc.p(-330,-35));

    self.editBox3_ = FactoryUtil.CreateEditBox(cc.size(197,45),g_tConfigTable.sTaskpath.."bgimg/sp9Scale.png");
    self:addChild(self.editBox3_);
    self.editBox3_:setPosition(cc.p(244.95, 35));

    self.editBox4_ = FactoryUtil.CreateEditBox(cc.size(197,45),g_tConfigTable.sTaskpath.."bgimg/sp9Scale.png");
    self:addChild(self.editBox4_);
    self.editBox4_:setPosition(cc.p(244.95,-35));

    self.editBox1_:registerScriptEditBoxHandler(function(eventType)
        if eventType == "ended" then 
            local str = self.editBox1_:getText();
            if g_tConfigTable.SceneEditor:IsContentProgramerArmatureName(str) then 
                self.editBox1_:setText("名字已经被使用");
                UIActionsUtil.Shake(self.editBox1_);
            else
                self.data_.programerName_ = self.editBox1_:getText();
                g_tConfigTable.SceneEditor:SaveProj();
            end
        end
    end);
    self.editBox2_:registerScriptEditBoxHandler(function(eventType)
        if eventType == "ended" then 
            self.data_.programerComment_ = self.editBox2_:getText();
            g_tConfigTable.SceneEditor:SaveProj();

        end
    end);
    self.editBox3_:registerScriptEditBoxHandler(function(eventType)
        if eventType == "ended" then 
            self.data_.artName_ = self.editBox3_:getText();
            self:updatePreview();
            g_tConfigTable.SceneEditor:SaveProj();
        end
    end);
    self.editBox4_:registerScriptEditBoxHandler(function(eventType)
        if eventType == "ended" then 
            self.data_.artComment_ = self.editBox4_:getText();
            g_tConfigTable.SceneEditor:SaveProj();

        end
    end);

    -- 进入标签的按钮
    self.btnGoToIndexView_ = Button.Create(
        "spBtnCheckIndex.png","spBtnCheckIndex.png",
        cc.p(-85.3,0),
        self);
    self.btnGoToIndexView_:AddListener(Button.EventType.Click,function()
        g_tConfigTable.SceneEditor:EnterLayerPlayIndexs(self.data_);
    end);

    -- 删除按钮
    self.btnDeleteData_ = Button.Create(
        "spBtnDeleteCell.png","spBtnDeleteCell.png",
        cc.p(-519.45,65.2),
        self);
    self.btnDeleteData_:AddListener(Button.EventType.Click,function()
        if self.data_ ~= nil then 
            g_tConfigTable.SceneEditor:DeleteProtocolByData(self.data_);
        end
    end);
    self.btnDeleteData_:setVisible(false);
end

function CellArmature:filterEditBox(obj,cb)
    if obj ~= nil then 
        if cb ~= nil then 
            local worldPos = obj:convertToWorldSpace(cc.p(obj:getContentSize().width/2,obj:getContentSize().height/2));
            obj:setVisible(cb(worldPos));
        end
    end
end

function CellArmature:filter(obj,cb)
    if obj ~= nil then 
        if cb ~= nil then 
            local worldPos = obj:convertToWorldSpace(cc.p(0,0));
            obj:setVisible(cb(worldPos));
        end
    end
end

function CellArmature:previewByName(name)
    if self.arm_  ~= nil then 
        self.arm_:removeFromParent()
        self.arm_ = nil;
    end
    self.arm_ = ArmatureUtil.Create(name);
    self:addChild(self.arm_);
    self.arm_:setPosition(cc.p(460,0));
    local size = self.arm_:getContentSize();
    print("armatureContentSize:"..self.arm_:getContentSize().width.."height:"..self.arm_:getContentSize().height);
    local scaleValue = 135 / size.height;
    self.arm_:setScale(scaleValue);
end

function CellArmature:noneArmature()
    if self.arm_  ~= nil then 
        self.arm_:removeFromParent()
        self.arm_ = nil;
    end
    self.arm_ = SpriteUtil.Create("spCellArmaturePreviewNone");
    self:addChild(self.arm_);
    self.arm_:setPosition(cc.p(460,0));
end

function CellArmature:SWUpdateCallBack(cb)
    self:filterEditBox(self.editBox1_,cb);
    self:filterEditBox(self.editBox2_,cb);
    self:filterEditBox(self.editBox3_,cb);
    self:filterEditBox(self.editBox4_,cb);
    self:filter(self.btnGoToIndexView_,cb);
    self:filter(self.btnDeleteData_,cb);
end

function CellArmature:updatePreview() 
    if self.data_.artName_ ~= "" then 
        if g_tConfigTable.SceneEditor:IsContentArmaturesName(self.data_.artName_) then 
            self:previewByName(self.data_.artName_);
        else 
            self:noneArmature();
        end
    else
        self:noneArmature();
    end
end

function CellArmature:UpdateByData(data)
    self.editBox1_:setText(data.programerName_);
    self.editBox2_:setText(data.programerComment_);
    self.editBox3_:setText(data.artName_);
    self.editBox4_:setText(data.artComment_);
    self.data_ = data;
    self:updatePreview();
end

return CellArmature;