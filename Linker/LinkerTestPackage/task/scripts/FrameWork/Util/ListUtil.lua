--region *.lua
--Date
--此文件由[BabeLua]插件自动生成
local ListUtil = {}
g_tConfigTable.CREATE_NEW(ListUtil);

ListUtil.IsContain = function (list,element)
    for k,v in ipairs(list) do 
        if v == element then 
            return true;
        end
    end
    return false;
end

ListUtil.Remove = function (list,element)
    for i = #list,1,-1 do 
        if list[i] == element then 
            table.remove(list,i);
        end
    end
end


return ListUtil;
--endregion
