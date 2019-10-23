--region *.lua
--Date
--此文件由[BabeLua]插件自动生成
local FactoryUtil = {}
g_tConfigTable.CREATE_NEW(FactoryUtil);

FactoryUtil.CreateEditBox = function (size,nineScaleSpriteName)
    local nineSp = cc.Scale9Sprite:create(
        nineScaleSpriteName
    );
    local editBox =  cc.EditBox:create(size,nineSp);
    return editBox;
end
return FactoryUtil;
--endregion
