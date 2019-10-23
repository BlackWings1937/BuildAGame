-- region *.lua
-- Date
-- 此文件由[BabeLua]插件自动生成
local LookAtUtil = { }

LookAtUtil.OriToAngle = function (ori)
    if ori.y== 0 then 
        if ori.x<0 then 
            return 270;
        elseif ori.x>0 then 
            return 90;
        end
        return 0;
    end
    if ori.x == 0 then 
        if ori.y>0 then 
           return 0;
        elseif ori.y<0 then 
           return 180;
        end
        return 0;
    end
    local tan = math.abs(ori.y)/math.abs(ori.x);
    local angle = 0;
    if ori.y>0 and ori.x<0 then 
        angle = 270 + math.atan(tan)*180/3.14;
    elseif ori.y>0 and ori.x>0 then 
        angle = 90 - math.atan(tan)*180/3.14;
    elseif ori.y<0 and ori.x<0 then 
        angle = 270 - math.atan(tan)*180/3.14;
    elseif ori.y<0 and ori.y>0 then 
        angle = 90 + math.atan(tan)*180/3.14;
    end
    return angle;
end

LookAtUtil.LookAt = function(looker, aim)
    looker.aim_ = aim;
    looker.updateLookAngle = function()
        if looker.aim_ ~= nil then
            local x,y = looker.aim_:getPosition();
            local aimPos =cc.p(x,y);
            x,y= looker:getPosition();
            local lookerPos = cc.p(x,y);
            local ori = cc.pNormalize(cc.pSub(aimPos,lookerPos));
            local angle = LookAtUtil.OriToAngle(ori);
            looker:setRotation(angle);
        end
    end
    local rep = cc.RepeatForever:create(
        cc.Sequence:create(cc.DelayTime:create(0.016), cc.CallFunc:create( function()
            looker.updateLookAngle();
        end ), nil));
    rep:setTag(2001);
    looker:runAction(rep);
end
LookAtUtil.StopLookAt = function (looker)
    looker:stopActionByTag(2001);
end


return LookAtUtil;
-- endregion
