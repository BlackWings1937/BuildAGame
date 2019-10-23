local FileUtil = requirePack(g_tConfigTable.RootFolderPath.."FrameWork.Util.FileUtil");

local ScriptUtil = {};

ScriptUtil.CacheScriptList = {};

ScriptUtil.deepcopy = function(object)
    local lookup_table = {}
    local function _copy(object)
        if type(object) ~= "table" then
            return object
        elseif lookup_table[object] then
            return lookup_table[object]
        end
        local new_table = {}
        lookup_table[object] = new_table
        for index, value in pairs(object) do
            new_table[_copy(index)] = _copy(value)
        end
        return setmetatable(new_table, getmetatable(object))
    end
    return _copy(object)
end

ScriptUtil.LoadScript= function(filePath) 
    if ScriptUtil.CacheScriptList[filePath] == nil then 
        local str = cc.FileUtils:getInstance():getStringFromFile(filePath);
        local zOrderInfo = globalJsonDecode(str);--json.decode(str);
        ScriptUtil.CacheScriptList[filePath] = zOrderInfo;
    else 

    end
    return ScriptUtil.CacheScriptList[filePath];
end

ScriptUtil.LoadScriptCopyMode = function(filePath)
    return ScriptUtil.deepcopy(ScriptUtil.LoadScript(filePath));
end

ScriptUtil.Dispose = function()
    print("ScriptUtil.Dispose");
    ScriptUtil.CacheScriptList = {};
end


return ScriptUtil;