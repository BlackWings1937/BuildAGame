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

local SceneEmpty = class("SceneEmpty", function(...)
    -- 创建sceneBase基类
    return SceneTouches.new(...);
end )
g_tConfigTable.CREATE_NEW(SceneEmpty);

function SceneEmpty:ctor()
    print("LinkerTestPackage123123");
    print(debug.traceback());
    self.linkerManager_ = LinkerManager.new();
    self.linkerManager_:SetRootNode(self);
    self.linkerManager_:Start();
end

return SceneEmpty;