-- region *.lua
-- Date
-- 此文件由[BabeLua]插件自动生成
local MovingIndex = { }
MovingIndex.EnumHVType = {
    ["HORIZONTAL"] = 1,
    ["VERTICAL"] = 2
}
MovingIndex.Init = function(obj, bgPos, bgSize, padding, IndexNum, hvType, IndexSpeed)

    obj.speedValue_ = IndexSpeed;
    obj.indexNum = IndexNum;
    obj.startPos_ = cc.p(0, 0);
    obj.endPos_ = cc.p(0, 0);
    obj.hvType_ = hvType;
    local x, y = obj:getPosition();
    if hvType == MovingIndex.EnumHVType.HORIZONTAL then
        obj.startPos_ = cc.p(bgPos.x - bgSize.width / 2 + padding, y);
        obj.endPos_ = cc.p(bgPos.x + bgSize.width / 2 - padding, y);
    elseif hvType == MovingIndex.EnumHVType.VERTICAL then
        obj.startPos_ = cc.p(x, bgPos.y - bgSize.height / 2 + padding.y);
        obj.endPos_ = cc.p(x, bgPos.y + bgSize.height / 2 - padding.y);
    end
    local seDistance = cc.pGetDistance(obj.startPos_,obj.endPos_);
    obj.partDis_ = seDistance/obj.indexNum;
    obj.resumeToStart = function (cb)
        local x,y = obj:getPosition();
        local distance = cc.pGetDistance(obj.startPos_,cc.p(x,y));
        obj:runAction(cc.Sequence:create(cc.MoveTo:create(distance/obj.speedValue_,obj.startPos_),cc.CallFunc:create(function ()
            if cb~= nil then 
                cb();
            end
        end),nil));
    end
    obj.startToEndRepeat = function ()
        local distance = cc.pGetDistance(obj.startPos_,obj.endPos_);
        local rep = cc.RepeatForever:create(cc.Sequence:create(
        cc.MoveTo:create(distance/obj.speedValue_,obj.endPos_),
        cc.MoveTo:create(distance/obj.speedValue_,obj.startPos_),
        nil));
        obj:runAction(rep);
    end
end

MovingIndex.StartMove = function(obj)
    obj.resumeToStart(function ()
        obj.startToEndRepeat();
    end);
end

MovingIndex.PauseMove = function(obj)
    obj:stopAllActions();
end

MovingIndex.ResumeMove = function(obj)
    obj.resumeToStart(function ()
        obj.startToEndRepeat();
    end);
end

MovingIndex.Index = function (obj)
    local x,y = obj:getPosition();
    local nowValue = cc.pGetDistance(obj.startPos_,cc.p(x,y));
    local num = nowValue/ obj.partDis_ ;
    if num<=0 then 
        return 1;
    end
    return math.ceil(num) ;
end


return MovingIndex;
-- endregion
