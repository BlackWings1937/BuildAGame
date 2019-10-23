--region *.lua
--Date
--此文件由[BabeLua]插件自动生成
local RandUtil = {}

RandUtil.GetRandPosInRect = function (rect)

end

RandUtil.GetRandPosBewteenTwoPos = function (posMin,posMax)
    return cc.p(math.random(posMin.x,posMax.x),math.random(posMin.y,posMax.y));
end

return RandUtil;
--endregion
