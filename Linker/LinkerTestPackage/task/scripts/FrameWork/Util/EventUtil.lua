-- region *.lua
-- Date
-- 此文件由[BabeLua]插件自动生成
local ListUtil = requirePack("scripts.FrameWork.Util.ListUtil");
local EventUtil = { };
g_tConfigTable.CREATE_NEW(EventUtil);

EventUtil.OnEvent = function(listOfEvent, ...)
    if listOfEvent ~= nil  then
        if #listOfEvent >0 then 
            for k, v in ipairs(listOfEvent) do
                v(...);
            end
        end
    end
end

EventUtil.RegisterEvent = function(listOfEvent, e)
    if listOfEvent ~= nil and e ~= nil then
        if ListUtil.isContain(listOfEvent, e) == false then
            table.insert(listOfEvent, e);
        end
    end
end

EventUtil.RemoveEvent = function(listOfEvent, e)
    if listOfEvent ~= nil and e ~= nil then
        ListUtil.Remove(listOfEvent, e);
    end
end

return EventUtil;
-- endregion
