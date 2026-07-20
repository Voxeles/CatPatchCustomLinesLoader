### A super simple mod for CatPatch to allow loading custom dialogue lines

CatPatch: https://www.nexusmods.com/scavprototype/mods/69

Allows you to replace the default hugging/kissing lines in CatPatch with your own.
Simply edit the CatPatchCustomLines.json file to add new lines or remove existing ones.
The file is hot reloaded, no need to restart the game.

#### Installation

1. Download CatPatchCustomLinesLoader.zip from [Releases](https://github.com/Voxeles/CatPatchCustomLinesLoader/releases)
2. Unzip it and place the extracted CatPatchCustomLinesLoader folder in "Casualties Unknown Demo/BepInEx/plugins"
3. Edit the CatPatchCustomLinesLoader/CatPatchCustomLines.json file as you wish. By default it comes with all of CatPatch's base dialogue.

#### Usage

For Hosts, this mod allows them to set up custom dialogue for everyone on the server to see, even if they don't have this mod installed. Simply add or remove any dialogue you want in CatPatchCustomLines.json

For Clients, they can filter unwanted dialogue by enabling 'Filter Incoming Lines' in CatPatch's in-game settings. Any dialogue lines not present in your CatPatchCustomLines.json will be replaced by lines you do have. This setting is disabled by default.

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
6. IrradiatedHugLines - Receiver radiation > 10
7. SickHugLines - Speaker sickness >= 50
8. OpiateOverdoseHugLines - Speaker opiate reception > 5
9. OpiateWithdrawalHugLines - Speaker opiate reception < -5
10. SpeakerMissingArmHugLines - Speaker is missing an arm
11. ReceiverMissingArmHugLines - Receiver is missing an arm
12. HotHugLines - Speaker temperature > 38
13. ColdHugLines - Speaker temperature < 35.5
14. DirtyHugLines - Receiver dirtiness >= 75
15. OverweightHugLines - Receiver weightOffset > 15
16. CheerfulHugLines - Speaker happiness >= 40
17. HostHugLines - Receiver is host
18. NeutralHugLines - Default hug lines

Kiss lines:
1. FinalKissLines - Speaker dying
2. GoodbyeKissLines - Receiver dying
3. VulnerableKissLines - Speaker happiness <= -40
4. ComfortKissLines - Receiver happiness <= -40
5. DisfiguredKissLines - Speaker or Receiver is disfigured
6. DirtyKissLines - Receiver dirtiness >= 75
7. CheerfulKissLines - Speaker happiness >= 40
8. KissLines - Default kiss lines
