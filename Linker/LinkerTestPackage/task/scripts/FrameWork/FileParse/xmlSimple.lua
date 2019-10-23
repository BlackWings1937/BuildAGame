local XmlParser = {};

function XmlParser:ToXmlString(value) --xml转换为字符串
	value = string.gsub(value, "&", "&amp;"); -- '&' -> "&amp;"
	value = string.gsub(value, "<", "&lt;"); -- '<' -> "&lt;"
	value = string.gsub(value, ">", "&gt;"); -- '>' -> "&gt;"
	value = string.gsub(value, "\"", "&quot;"); -- '"' -> "&quot;"
	value = string.gsub(value, "([^%w%&%;%p%\t% ])", --特殊字符替换为&#x十六进制数字
		function(c)
			return string.format("&#x%X;", string.byte(c))
		end);
	return value;
end

function XmlParser:FromXmlString(value)
	value = string.gsub(value, "&#x([%x]+)%;",
		function(h)
			return string.char(tonumber(h, 16)) --&#x十六进制数字替换为对应字符串
		end);
	value = string.gsub(value, "&#([0-9]+)%;",
		function(h)
			return string.char(tonumber(h, 10))
		end);
	value = string.gsub(value, "&quot;", "\"");
	value = string.gsub(value, "&apos;", "'");
	value = string.gsub(value, "&gt;", ">");
	value = string.gsub(value, "&lt;", "<");
	value = string.gsub(value, "&amp;", "&");
	return value;
end

function XmlParser:ParseArgs(node, s)
	string.gsub(s, "(%w+)=([\"'])(.-)%2", function(w, _, a)
		node:addProperty(w, self:FromXmlString(a))
	end)
end

function XmlParser:ParseXmlText(xmlText)
	local stack = {}
	local top = self:newNode()
	table.insert(stack, top)
	local ni, c, label, xarg, empty
	local i, j = 1, 1
	while true do
		ni, j, c, label, xarg, empty = string.find(xmlText, "<(%/?)([%w_:]+)(.-)(%/?)>", i)  --< c= 或/ label=字母和数字_: xarg=... empty= 或/ >
		if not ni then break end
		local text = string.sub(xmlText, i, ni - 1);
		if not string.find(text, "^%s*$") then
			local lVal = (top:value() or "") .. self:FromXmlString(text)
			stack[#stack]:setValue(lVal)
		end
		if empty == "/" then -- empty element tag
			local lNode = self:newNode(label)
			self:ParseArgs(lNode, xarg)
			top:addChild(lNode)
		elseif c == "" then -- start tag
			local lNode = self:newNode(label)
			self:ParseArgs(lNode, xarg)
			table.insert(stack, lNode)
			top = lNode
		else -- end tag
			local toclose = table.remove(stack) -- remove top

			top = stack[#stack]
			if #stack < 1 then
				error("XmlParser: nothing to close with " .. label)
			end
			if toclose:name() ~= label then
				error("XmlParser: trying to close " .. toclose.name .. " with " .. label)
			end
			top:addChild(toclose)
		end
		i = j + 1
	end
	local text = string.sub(xmlText, i);
	if #stack > 1 then
		error("XmlParser: unclosed " .. stack[#stack]:name())
	end
	return top
end

function XmlParser:loadFile(xmlFilename, base)
	--[[
	if not base then
		base = system.ResourceDirectory
	end
	local path = system.pathForFile(xmlFilename, base)
	local hFile, err = io.open(path, "r");]]
	local hFile, err = io.open(xmlFilename, "r")
	if hFile and not err then
		local xmlText = hFile:read("*a"); -- read file content
		io.close(hFile);
		return self:ParseXmlText(xmlText), nil;
	else
		print(err)
		return nil
	end
end




function XmlParser:newNode(name)
	local node = {}
	node.___value = nil
	node.___name = name
	node.___children = {}
	node.___props = {}

	function node:value() return node.___value end
	function node:setValue(val) node.___value = val end
	function node:name() return node.___name end
	function node:setName(name) node.___name = name end
	function node:children() return node.___children end
	function node:numChildren() return #node.___children end
	function node:addChild(child)
		if node[child:name()] ~= nil then
			if type(node[child:name()].name) == "function" then
				local tempTable = {}
				table.insert(tempTable, node[child:name()])
				node[child:name()] = tempTable
			end
			table.insert(node[child:name()], child)
		else
			node[child:name()] = child
		end
		table.insert(node.___children, child)
	end

	function node:properties() return node.___props end
	function node:numProperties() return #node.___props end
	function node:addProperty(name, value)
		local lName = "@" .. name
		if node[lName] ~= nil then
			if type(node[lName]) == "string" then
				local tempTable = {}
				table.insert(tempTable, node[lName])
				node[lName] = tempTable
			end
			table.insert(node[lName], value)
		else
			node[lName] = value
		end
		table.insert(node.___props, { name = name, value = node[name] })
	end
	return node
end

return XmlParser