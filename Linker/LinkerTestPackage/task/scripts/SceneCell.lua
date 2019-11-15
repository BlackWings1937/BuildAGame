requirePack("scripts.FrameWork.Global.GlobalFunctions");
local ArmtureFactory = requirePack("scripts.FrameWork.Factory.ArmtureFactory");
local SpriteUtil = requirePack("scripts.FrameWork.Util.SpriteUtil");--ArmatureUtil.Create
local FileUtil = requirePack("scripts.FrameWork.Util.FileUtil");--ArmatureUtil.Create
local ArmatureUtil = requirePack("scripts.FrameWork.Util.ArmatureUtil");--ArmatureUtil.Create
local Button = requirePack("scripts.FrameWork.UI.Button");--ArmatureUtil.Create
local ArmatureProtocalUtil = requirePack("scripts.FrameWork.Util.ArmatureProtocalUtil");
local MagpieBridgeUtil = requirePack("scripts.FrameWork.Util.MagpieBridgeUtil");
local SceneTouches = requirePack("scripts.FrameWork.Scenes.SceneTouches");
--local TableView = requirePack("scripts.FrameWork.UI.TableView");

local MBUtil = requirePack("scripts.FrameWork.Util.MBUtil");
local LinkerManager = requirePack("scripts.LinkerManager");
local MBArmatureUtil = requirePack("scripts.FrameWork.Util.MBArmatureUtil");

local PlayArmatureHandler = requirePack("scripts.Behaviour.PlayArmatureHandler");
requirePack("baseScripts.homeUI.FrameWork.AnimationEngineLua.AnimationEngine");

local SceneCell = class("SceneCell", function(...)
    -- 创建sceneBase基类
    return SceneTouches.new(...);
end )
g_tConfigTable.CREATE_NEW(SceneCell);
function SceneCell:removeUI()
    self.editBox_:removeFromParent();
    self.label_:removeFromParent();
    self.b_:removeFromParent();
end
function SceneCell:initView()
    
    local size = VisibleRect:winSize();
    local editBox = cc.EditBox:create(cc.size(200,30), cc.Scale9Sprite:create(g_tConfigTable.sTaskpath.. "bgimg/EditBox.png"));
    editBox:setPosition(cc.p(size.width/2,size.height/2));
    self:addChild(editBox);
    self.editBox_ = editBox;

    local label = cc.Sprite:create(g_tConfigTable.sTaskpath.. "bgimg/LpInPut.png");
    self:addChild(label);
    label:setPosition(cc.p(size.width/2,size.height/2+200));
    self.label_ = label;

    local b = Button.Create( 
         cc.Sprite:create(g_tConfigTable.sTaskpath.. "bgimg/btnOk.png"),
         cc.Sprite:create(g_tConfigTable.sTaskpath.. "bgimg/btnOk.png"),
         cc.p(size.width/2,size.height/2-200),
         self
        );
    b:AddListener(Button.EventType.Click,function() 
        print("hit:"..self.editBox_:getText());
        self.linkerManager_ = LinkerManager.new();
        self.linkerManager_:SetRootNode(self.node);
        self.linkerManager_:SetScene(self);
        self.linkerManager_:StartWithIP(self.editBox_:getText(),true);
        self:removeUI();
    end);
    self.b_ = b;
end

function SceneCell:ctor()
    self.node = cc.Node:create();
    self:addChild(self.node);
    self:initView();
end

return SceneCell;