--region *.lua
--Date
--此文件由[BabeLua]插件自动生成

local ArmatureGroup = class("ArmatureGroup",function ()
    return cc.Node:create();
end);
g_tConfigTable.CREATE_NEW(ArmatureGroup);

function ArmatureGroup:ctor()

end

------------------对外接口---------------------

--[[
    执行脚本
    参数:
    fileName:脚本文件名
]]--
function ArmatureGroup:RunScript(fileName)
    
end

--[[
    获取正在执行的脚本名如果没有脚本执行返回空字符串
]]--
function ArmatureGroup:GetRunningScript()
    return "";
end

--[[
    停止正在执行的脚本
]]--
function ArmatureGroup:StopRunningScript()

end

--[[
    跳过目前正在执行的动作
]]--
function ArmatureGroup:SkipLine()

end


return ArmatureGroup;
--endregion
