Open the console application in the THUUM_ACTIVATOR folder before launching the game. There will be a list of spells for you to shout out.

To shout out a spell, say in clear and unmuddled words, "Cast <your spell here>", so "Cast Urgent Heal" or "Cast Glintstone Pebble" works.

To merge this into other setups, just take the reg bin stuff and merge (csv folder), then take the following code from your c0000.hks.

Put this "vocalMagic()" function call into function Update and put this function into your c0000.hks (disregard the quotations):
"
function vocalMagic()
    if env(GetFP) < 5 then
        return FALSE
    end

    if pipe then
        local response = pipe:read("*l")
        act(2002, response)
    else
        pipe = io.open("\\\\.\\pipe\\thuuminator", "r")
    end
end
pipe = io.open("\\\\.\\pipe\\thuuminator", "r")
"

If the console app crashes or is closed, just reopen it and reload your character (fast travel or die).