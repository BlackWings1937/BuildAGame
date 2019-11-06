requirePack("baseScripts.homeUI.FrameWork.Global.GlobalFunctions");
local PlayMentBase = requirePack("baseScripts.LinkerBase.PlayMentBase");
print("load--------------------------------------------------------------");
dump(PlayMentBase);

print("load--------------------------------------------------------------2");

local ScriptUtil = requirePack("baseScripts.homeUI.FrameWork.AnimationEngineLua.ScriptUtil");
dump(ScriptUtil);


print("load--------------------------------------------------------------2");
local test = requirePack("baseScripts.LinkerBase.test");
dump(test);



local TestPlayMent = class("TestPlayMent",function()
     return PlayMentBase.new();
end)
g_tConfigTable.CREATE_NEW(TestPlayMent);

function TestPlayMent:Start(rootNode)
    print("testPlayMent:2222222222222222222222222222222222222222222222222222222222222222");
    print(debug.traceback());
    dump(TouchArmature)
    print("111111111111111111111111111111111111");
    local npc = TouchArmature:create("A",TOUCHARMATURE_NORMAL);
    print("22222222222222222222222222222222222222222");
    dump(npc);
    dump(rootNode);
    rootNode:addChild(npc);
    npc:setPosition(cc.p(300,300));
    print("33333333333333333333333333333333333333");
    
    rootNode:runAction(cc.Sequence:create(cc.DelayTime:create(5),cc.CallFunc:create(function()
        self:SceneEnd("PROT2");
    end)));


end


return TestPlayMent;