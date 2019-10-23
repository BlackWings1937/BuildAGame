--region *.lua
--Date
--此文件由[BabeLua]插件自动生成
local Patrol = {};
local ACTION_TAG_PATROL = g_tConfigTable.Director.GetUnicActionTag();

--[[

]]--
Patrol.init = function (c,rt,sv,minIdle,maxIdle,mai,iai)
    c.patrol_ = {};
    c.patrol_.rt_ = rt;
    c.patrol_.sv_ = sv;
    c.patrol_.minIdle_ = minIdle;
    c.patrol_.maxIdle_ = maxIdle;
    c.patrol_.mai_ = mai;
    c.patrol_.iai_ = iai;
    c.patrol_.move = function (c)
        local x,y = c:getPosition();
        local posNow = cc.p(x,y);
        local posNext = cc.p(
            math.random(c.patrol_.rt_.x,c.patrol_.rt_.x+c.patrol_.rt_.width),
            math.random(c.patrol_.rt_.y,c.patrol_.rt_.y+c.patrol_.rt_.height)
            );
        local dis = cc.pGetDistance(posNow,posNext);
        local moveTime = dis/c.patrol_.sv_;
        local seq = cc.Sequence:create(cc.MoveTo:create(moveTime,posNext),cc.CallFunc:create(function ()
            c.patrol_.moveEnd(c);
        end),nil);
        seq:setTag(ACTION_TAG_PATROL); --playByIndex(11, LOOP_NO);
        c:playByIndex(c.patrol_.mai_,LOOP_YES);
        c:runAction(seq);
    end
    c.patrol_.moveEnd = function (c)
        c.patrol_.idle(c);
    end
    c.patrol_.idle = function (c)
        c:playByIndex(c.patrol_.iai_,LOOP_YES);
        local time = math.random(c.patrol_.minIdle_,c.patrol_.maxIdle_)
        if time<=0 then 
            c.patrol_.idleEnd(c);
            return ;
        end
        local seq = cc.Sequence:create(
        cc.DelayTime:create(time),
        cc.CallFunc:create(function ()
            c.patrol_.idleEnd(c);
        end),nil);
        seq:setTag(ACTION_TAG_PATROL);
        c:runAction(seq);
    end
    c.patrol_.idleEnd = function (c)
        c.patrol_.move(c)
    end
    c.patrol_.move(c);
end

Patrol.pause = function (c)
    c:stopActionByTag(ACTION_TAG_PATROL);
end

Patrol.coninue = function (c)
    Patrol.init(c,c.patrol_.rt_ ,c.patrol_.sv_ ,c.patrol_.minIdle_, c.patrol_.maxIdle_ ,c.patrol_.mai_,c.patrol_.iai_);
end

Patrol.dispose = function (c)
    if c~= nil then 
        if c.patrol_~= nil then 
            c:stopActionByTag(ACTION_TAG_PATROL);
        end
    end
end

return Patrol;
--endregion
