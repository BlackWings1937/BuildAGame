local CustomActualNameConfig = {
--   角色名称       实际显示对象名称
    ["npc_xbl"] = "XBL";
}
local config = {};
config.ToActualNameDic = {};
config.ToActorNameDic = {};

for k,v in pairs(CustomActualNameConfig) do 
    config.ToActorNameDic[v] = "AESOP*"..k;
    config.ToActualNameDic["AESOP*"..k] = v;
end

-- 注册角色的prework 动作
config.ToActorPreWorkAction ={
    ["npc_xbl"] = {
        ["walkAnimation"] = "XBL2019_CH_type3_normal",
        ["walkIndex"] = "1",-- 动画标签一定要定义成字符串
        ["runAnimation"] = "XBL2019_CH_type3_normal",
        ["runIndex"] = "5",--5,
        ["idleAnimation"] = "XBL2019_CH_type3_normal",
        ["idleIndex"] = "0",
    }
}

config.ListOfFixNpcs = {
   "XBL",
}

return config;