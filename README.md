### A super simple mod for CatPatch to allow loading custom dialogue lines

CatPatch: https://www.nexusmods.com/scavprototype/mods/69

Allows you to replace the default hugging/kissing lines in CatPatch with your own.
Simply edit the CatPatchCustomLines.json file to add new lines or remove existing ones.
The file is hot reloaded, no need to restart the game.

---

Note: **The dialogue lines are decided by the host!**

In lobbies, the host's CatPatchCustomLines.json will override your own.
If the host doesn't have this mod installed, then the default CatPatch lines will be used for everyone.

On the bright side, if a host has this mod but a client doesn't, then the client will still see the custom dialogue, since the server sends the line to all clients.

Nevertheless, this mod allows you to filter host's lines via the "filterIncomingLines" setting.
Any incoming dialogue lines not present in your CatPatchCustomLines.json will be replaced by lines you do have.

So, if all you want is to remove some of the 'meme' lines from CatPatch, simply remove those lines from CatPatchCustomLines.json and enable the "filterIncomingLines" setting.
You can set this option in-game from CatPatch's settings menu (click the small square button at the bottom).

#### Doesn't CatPatch already allow you to set custom dialogue through translations?

Yes, but that's annoying to set up.
