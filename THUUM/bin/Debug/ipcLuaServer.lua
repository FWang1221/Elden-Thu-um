function vocalMagic()
    --if env(GetFP) < 5 then
    --    return FALSE
    --end

    if pipeInate() then
        local response = pipe:read("*l")
        print(response)
    end
end

pipe = false
waitServer = 100

function pipeInate()
    if pipe then
        return true
    else
        pipe = io.open("\\\\.\\pipe\\thuuminator", "w+")
        if waitServer > 0 then
            waitServer = waitServer - 1
        else
            waitServer = 100
            pipeInate()
        end
    end
end

while (true) do
    vocalMagic()
end