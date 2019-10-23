--region *.lua
--Date
--此文件由[BabeLua]插件自动生成
local SpriteUtil = {}
g_tConfigTable.CREATE_NEW(SpriteUtil);

SpriteUtil.Create = function (imageName)
    local result = string.find(imageName,".png");
    if result == nil then 
        print("waring "..imageName.." miss .png rear...");
        imageName = imageName..".png"
    end
    local sp = cc.Sprite:create(g_tConfigTable.sTaskpath.."bgimg/"..imageName);
    if sp ~= nil then 
        -- sp:setScale(CFG_SCALE(1));
    end
    return sp;
end

SpriteUtil.CreateList = function (list,parent,index)
    local listAnims = {};
    for i = 1,#list ,1 do 
        local config = list[i];
        local anim = SpriteUtil.Create(config.name);
        anim:setPosition(config.pos);
        parent:addChild(anim,index);
        table.insert(listAnims,anim);
    end
    return listAnims;
end

return SpriteUtil;
--endregion
