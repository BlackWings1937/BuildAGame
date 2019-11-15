requirePack("scripts.FrameWork.Global.GlobalFunctions");
local ArmtureFactory = requirePack("scripts.FrameWork.Factory.ArmtureFactory");
local SpriteUtil = requirePack("scripts.FrameWork.Util.SpriteUtil");--ArmatureUtil.Create
local FileUtil = requirePack("scripts.FrameWork.Util.FileUtil");--ArmatureUtil.Create
local ArmatureUtil = requirePack("scripts.FrameWork.Util.ArmatureUtil");--ArmatureUtil.Create
local ArmatureProtocalUtil = requirePack("scripts.FrameWork.Util.ArmatureProtocalUtil");
local MagpieBridgeUtil = requirePack("scripts.FrameWork.Util.MagpieBridgeUtil");
local SceneTouches = requirePack("scripts.FrameWork.Scenes.SceneTouches");
--local TableView = requirePack("scripts.FrameWork.UI.TableView");

local MBUtil = requirePack("scripts.FrameWork.Util.MBUtil");
local LinkerManager = requirePack("scripts.LinkerManager");
local MBArmatureUtil = requirePack("scripts.FrameWork.Util.MBArmatureUtil");

local PlayArmatureHandler = requirePack("scripts.Behaviour.PlayArmatureHandler");
requirePack("baseScripts.homeUI.FrameWork.AnimationEngineLua.AnimationEngine");

local SceneWin = class("SceneWin", function(...)
    -- 创建sceneBase基类
    return SceneTouches.new(...);
end )
g_tConfigTable.CREATE_NEW(SceneWin);

function SceneWin:ctor()
    local str = FileUtil.LoadFileContent(g_tConfigTable.sTaskpath.."IpConfig.json");
    print("json:"..str);
    local m = json.decode(str);
    self.node = cc.Node:create();
    self:addChild(self.node);
    self.linkerManager_ = LinkerManager.new();
    self.linkerManager_:SetRootNode(self.node );
    self.linkerManager_:SetScene(self);
    self.linkerManager_:StartWithIP(m.IP,false);
end

return SceneWin;