--region *.lua
--Date
--此文件由[BabeLua]插件自动生成
local TouchPosFinder = {};

TouchPosFinder.init = function (t)
    t.onTouchBegan = function (s,t,v)
        local midPos = g_tConfigTable.Director.midPos;
        local pos = s:convertTouchToNodeSpace(t);
        local posNow = cc.pSub(pos,midPos);
        print("x:"..posNow.x.."y:"..posNow.y);
    end
end

return TouchPosFinder;
--endregion
