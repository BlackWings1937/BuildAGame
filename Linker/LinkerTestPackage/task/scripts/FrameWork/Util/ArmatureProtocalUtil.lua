
local FileUtil = requirePack("scripts.FrameWork.Util.FileUtil");
local ArmatureProtocal = requirePack("scripts.Data.ArmatureProtocal");
local ActionProtocal = requirePack("scripts.Data.ActionProtocal");
local ProtocalData = requirePack("scripts.Data.ProtocalData");

local ArmatureProtocalUtil = {};

ArmatureProtocalUtil.jsonDict_ = nil;
ArmatureProtocalUtil.armatureProtocals_ = {};

function ArmatureProtocalUtil.LoadProtocalConfig(configName)
    local path = g_tConfigTable.sTaskpath.."config/"..configName;
    local result = FileUtil.LoadFileContent(path);
    ArmatureProtocalUtil.jsonDict_ = json.decode(result)["Animation"];
    ArmatureProtocalUtil.parseProtocalConfig();
end

function ArmatureProtocalUtil.parseProtocalConfig()
    ArmatureProtocalUtil.armatureProtocals_ = {};
    -- 解析每一个ArmatureProtocal
    for k, v in pairs(ArmatureProtocalUtil.jsonDict_) do
        local armatureProtocalData = ProtocalData:create(k, v.ArmName, v.programerComment, v.artComment);
        local armatureProtocal = ArmatureProtocal:create(armatureProtocalData);
        -- 解析此时的armatureProtocal中的Actions
        local actionDict = v.Actions;
        for k_, v_ in pairs(actionDict) do
            local actionProtoalData = ProtocalData:create(k_, v_.Index, v_.programerComment, v_.artComment);
            local actionProtocal = ActionProtocal:create(actionProtoalData);
            armatureProtocal:AddAction(actionProtocal);
        end
        -- 添加解析好的protocal
        ArmatureProtocalUtil.armatureProtocals_[k] = armatureProtocal;
        print(armatureProtocal:ToString());
    end
end

-- 判断是否有指定龙骨名字的ArmatureProtocal，如果存在返回true，否则返回false
function ArmatureProtocalUtil.IsHaveArmatureProtocal(progArmatureName)
    for k,v in pairs(ArmatureProtocalUtil.armatureProtocals_) do
        if k == progArmatureName then
            return true;
        end
    end
    return false;
end

-- 获取指定程序龙骨名字的ArmatureProtocal，如果不存在则返回nil
function ArmatureProtocalUtil.GetArmatureProtocal(progArmatureName)
    local result = nil;
    if ArmatureProtocalUtil.IsHaveArmatureProtocal(progArmatureName) then 
        result = ArmatureProtocalUtil.armatureProtocals_[progArmatureName];
    end
    return result;
end

-- 获取程序龙骨名字所对应的动画填写龙骨名字，如果不存在则返回nil
function ArmatureProtocalUtil.GetArmatureNameMapWith(progArmatureName)
    local result = nil;
    local armatureProtocal = ArmatureProtocalUtil.GetArmatureProtocal(progArmatureName);
    if armatureProtocal ~= nil then
        result = armatureProtocal:GetArtName();
    end
    return result;
end
-- 获取程序龙骨名字+程序Action编号对应的动画填写Action编号，如果不存在则返回nil
function ArmatureProtocalUtil.GetActionIndexMapWith(progArmatureName, progActionIndex )
    local result = nil;
    local armatureProtocal = ArmatureProtocalUtil.GetArmatureProtocal(progArmatureName);
    if armatureProtocal ~= nil then
        result = armatureProtocal:GetActionIndexMapWith(progActionIndex);
    end
    return result;
end

return ArmatureProtocalUtil;