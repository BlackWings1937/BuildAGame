--region *.lua
--Date
--此文件由[BabeLua]插件自动生成
local UIActionsUtil = {}
g_tConfigTable.CREATE_NEW(UIActionsUtil);

UIActionsUtil.Shake = function (node)
    if nil ~= node then 
        node:runAction(
            cc.Sequence:create(
                cc.MoveBy:create(0.10,cc.p(10 , 0)),
                cc.MoveBy:create(0.10,cc.p(-10, 0)),
                cc.MoveBy:create(0.10,cc.p(10 , 0)),
                cc.MoveBy:create(0.10,cc.p(-10, 0))
                ,nil));
    end
end
return UIActionsUtil;
--endregion