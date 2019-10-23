--[[
local TestPlayMent = {};
testPlayMent.Start = function(root) 
    print("testPlayMent:123123");
end
return testPlayMent;
]]--
--[[]]--
requirePack("baseScripts.homeUI.FrameWork.Global.GlobalFunctions");
local PlayMentBase = requirePack("baseScripts.LinkerBase.PlayMentBase");

local TestPlayMent = class("TestPlayMent",function() return PlayMentBase.new(); end)
g_tConfigTable.CREATE_NEW(TestPlayMent);

function TestPlayMent:Start(rootNode)
    print("testPlayMent:123123");
end


return TestPlayMent;