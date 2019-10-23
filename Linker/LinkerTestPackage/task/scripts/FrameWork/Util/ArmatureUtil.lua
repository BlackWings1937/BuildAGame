--region *.lua
--Date
--此文件由[BabeLua]插件自动生成
local ArmatureUtil = {}

ArmatureUtil.Create = function (strName)
    local charactor = TouchArmature:create(strName, TOUCHARMATURE_NORMAL, "");
    charactor:setPosition(g_tConfigTable.Director.midPos);
    charactor:setScale(CFG_SCALE(1));--
    charactor:playByIndex(0,LOOP_YES);
    return charactor;
end

ArmatureUtil.CreateList = function (list,parent,index)
    local listAnims = {};
    --local num = #list;
    --print("num:"..num);
    for k,v in ipairs(list) do 
        local config = v;
        print("name:"..config.animName);
        local anim = ArmatureUtil.Create(config.animName);
        if anim ~=nil then 
            print("anim ~=nil")
        else 
            print("anim ==nil")
        end
        anim:setPosition(cc.pAdd(config.pos,g_tConfigTable.Director.midPos));
        parent:addChild(anim,index);
        table.insert(listAnims,anim);
    end
    return listAnims;
end


ArmatureUtil.GetSize = function (arm)
    local x,y,w,h = 0,0,0,0;
    x,y,w,h = arm:getBoundingBoxValue(x,y,w,h);
    return cc.size(w,h);
end

ArmatureUtil.GetRect= function (arm)
    local x,y,w,h = 0,0,0,0;
    x,y,w,h = arm:getBoundingBoxValue(x,y,w,h);
    return cc.rect(x,y,w,h);
end

return ArmatureUtil;
--endregion
