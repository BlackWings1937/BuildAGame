-- region *.lua
-- Date
-- 此文件由[BabeLua]插件自动生成
local ArmtureFactory = { }
local FileUtil = requirePack("scripts.FrameWork.Util.FileUtil");
local StringUtil = requirePack("scripts.FrameWork.Util.StringUtil");
local SpriteUtil = requirePack("scripts.FrameWork.Util.SpriteUtil");
ArmtureFactory.Create = function(fileName)
    local node = cc.Node:create();
    local f = FileUtil.LoadFile(fileName);
    for line in f:lines() do
        local list = StringUtil.Split(line, '|');
        local armName = list[1];
        local armPos = ArmtureFactory.strToPos(list[3]);
        local armScale = tonumber(list[4]);
        local armture = TouchArmature:create(armName, TOUCHARMATURE_NORMAL, "");
        armture:setPosition(armPos);
        armture:setScale(CFG_SCALE(1.0) * armScale);
        node:addChild(armture);
        node[armName] = armture;
    end
    return node;
end
-- SoundUtil:getInstance():playBackgroundMusic(self.sTaskpath.."audio/".."BGM2.mp3",true);
ArmtureFactory.CreateByNode = function(node, fileName)
    local f = FileUtil.LoadFile(fileName);
    local index = 1;
    for line in f:lines() do
        if index <= 1 then
            local list = StringUtil.Split(line, '|');
            if #list >= 3 then
                local bgPath = list[1];
                print("bgPath:" .. bgPath);
                if bgPath ~= "null" then
                    local bg = SpriteUtil.Create(bgPath);
                    bg:setPosition(g_tConfigTable.Director.midPos);
                    node:addChild(bg);
                else
                end
                local bgMusicName = list[2];
                SoundUtil:getInstance():playBackgroundMusic(g_tConfigTable.sTaskpath .. "audio/" .. bgMusicName, true);
            end
        else
            local list = StringUtil.Split(line, '|');
            local armName = list[1];
            local armPos = ArmtureFactory.strToPos(list[3]);
            local listOfPlayIndex = StringUtil.Split(list[6], ',');

            local armScale = tonumber(list[4]);
            local armture = TouchArmature:create(armName, TOUCHARMATURE_NORMAL, "");
            armture:playByIndex(tonumber(listOfPlayIndex[1]), LOOP_YES);
            armture:setPosition(armPos);
            armture:setScale(CFG_SCALE(1.0) * armScale);
            armture.name = armName;
            node:addChild(armture);
            node[armName] = armture;
        end
        index = index + 1;
    end
    return node;
end

ArmtureFactory.CreateByNodeSameParent = function(node, fileName)
    local f = FileUtil.LoadFile(fileName);
    local index = 1;
    for line in f:lines() do
        local list = StringUtil.Split(line, '|');
        local armName = list[1];
        local armPos = ArmtureFactory.strToPos(list[3]);
        local listOfScalePos = StringUtil.Split(list[7], ',');
        local scalePos = cc.p(tonumber(listOfScalePos[1]), tonumber(listOfScalePos[2]));
        local listOfPlayIndex = StringUtil.Split(list[6], ',');
        local armScale = tonumber(list[4]);
        local armture = TouchArmature:create(armName, TOUCHARMATURE_NORMAL, "");
        armture:playByIndex(tonumber(listOfPlayIndex[1]), LOOP_YES);
        armture:setPosition(armPos);
        armture:setScale(CFG_SCALE(1.0) * armScale);
        print("scaleX:" .. scalePos.x);
        print("scaley:" .. scalePos.y);
        if scalePos.y == 0 then
            armture:setScaleX(CFG_SCALE(1.0) * armScale);
        else
            armture:setScaleX(- CFG_SCALE(1.0) * armScale);
        end
        if scalePos.x == 0 then
            armture:setScaleY(CFG_SCALE(1.0) * armScale);
        else
            armture:setScaleY(- CFG_SCALE(1.0) * armScale);
        end
        armture.name = armName;
        node:getParent():addChild(armture);
        node[armName] = armture;
        index = index + 1;
    end
    return node;
end

ArmtureFactory.strToPos = function(str)
    local list = StringUtil.Split(str, ',');
    return cc.p(CFG_X(tonumber(list[1])), CFG_GL_Y(tonumber(list[2])));
end

return ArmtureFactory
-- endregion
-- local ArmtureFactory = requirePack("scripts.FrameWork.Factory.ArmtureFactory");