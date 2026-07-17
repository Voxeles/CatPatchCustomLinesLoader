### A super simple mod for CatPatch to allow loading custom dialogue lines

CatPatch: https://www.nexusmods.com/scavprototype/mods/69

Allows you to replace the default hugging/kissing lines in CatPatch with your own.
Simply edit the CatPatchCustomLines.json file to add new lines or remove existing ones.
The file is hot reloaded, no need to restart the game.

To install:
1. Download CatPatchCustomLinesLoader.zip from [Releases](https://github.com/Voxeles/CatPatchCustomLinesLoader/releases)
2. Unzip it and place the extracted CatPatchCustomLinesLoader folder in "Casualties Unknown Demo/BepInEx/plugins"
3. Edit the CatPatchCustomLinesLoader/CatPatchCustomLines.json file as you wish. By default it comes with all of CatPatch's base dialogue.

---

Note: **The dialogue lines are decided by the host!**

In lobbies, the host's CatPatchCustomLines.json will override your own.
If the host doesn't have this mod installed, then the default CatPatch lines will be used for everyone.

On the bright side, if a host has this mod but a client doesn't, then the client will still see the custom dialogue, since the server sends the line to all clients.

Nevertheless, this mod allows you to filter host's lines via the "filterIncomingLines" setting.
Any incoming dialogue lines not present in your CatPatchCustomLines.json will be replaced by lines you do have.

So, if all you want is to remove some of the 'meme' lines from CatPatch, simply remove those lines from CatPatchCustomLines.json and enable the "filterIncomingLines" setting.
You can set this option in-game from CatPatch's settings menu (click the small square button at the bottom).

In short: Hosts can set the custom dialogue, Clients can filter unwanted lines.

#### Doesn't CatPatch already allow you to set custom dialogue through translations?

Yes, but that's annoying to set up.

#### Extra dialogue pools

Besides the default Vulnerable/Comfort/Cheerful cases, the mod also adds extra dialogue pools for use in specific condidtions.
Adding dialogue lines for all these cases is _not_ necessary. If they're empty, that case will simply be skipped.

I've ordered them here in descending priority. Speaker is the player who initiated the hug and Receiver whoever accepted the hug.

Hug lines:
1. FinalHugLines - Speaker dying
2. GoodbyeHugLines - Receiver dying
3. GleefulPairHugLines - Speaker & Receiver happiness >= 75
4. VulnerableHugLines - Speaker happiness <= -40
5. ComfortHugLines - Receiver happiness <= -40
6. SickHugLines - Speaker sickness > 75
7. SpeakerMissingArmHugLines - Speaker is missing an arm
8. ReceiverMissingArmHugLines - Receiver is missing an arm
9. HotHugLines - Speaker temperature > 38
10. ColdHugLines - Speaker temperature < 35.5
11. DirtyHugLines - Receiver dirtiness >= 75
12. OverweightHugLines - Receiver weightOffset > 15
13. CheerfulHugLines - Speaker happiness >= 40
14. HostHugLines - Receiver is host
15. NeutralHugLines - Default hug lines

Kiss lines:
1. FinalKissLines - Speaker dying
2. GoodbyeKissLines - Receiver dying
3. DisfiguredKissLines - Speaker or Receiver is disfigured
4. DirtyKissLines - Receiver dirtiness >= 75
5. KissLines - Default kiss lines
