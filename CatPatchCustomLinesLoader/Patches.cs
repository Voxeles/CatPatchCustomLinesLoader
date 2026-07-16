using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using CatPatchspace;
using KrokoshaCasualtiesMP;

namespace CatPatchCustomLinesLoader;

internal class Patches
{
	[HarmonyPatch(typeof(CatPatchHostSettingsMenu))]
	internal static class CatPatchHostSettingsMenuPatch
	{
		[HarmonyPatch(nameof(CatPatchHostSettingsMenu.DrawWindowContents))]
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> DrawWindowContentsTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			var codeMatcher = new CodeMatcher(instructions);
			codeMatcher.MatchForward(false,
					new CodeMatch(OpCodes.Ldloc_0),
					new CodeMatch(OpCodes.Ldloc_1),
					new CodeMatch(OpCodes.Call,
						AccessTools.Method(typeof(CatPatchHostSettingsMenu),
							nameof(CatPatchHostSettingsMenu.DrawUpdaterControls))))
				.ThrowIfInvalid("CatPatchCustomLinesLoader could not find a match!");
			
			// Move the branch label from the current instruction to a new Nop one step back, to make room for my code
			var labels = codeMatcher.Labels;
			codeMatcher
				.Insert(new CodeInstruction(OpCodes.Nop))
				.AddLabels(labels)
				.Advance(1)
				.InsertAndAdvance(
					new CodeInstruction(OpCodes.Ldstr, "Filter Incoming Lines"),
					new CodeInstruction(OpCodes.Call,
						AccessTools.PropertyGetter(typeof(Plugin), nameof(Plugin.ShouldFilterIncomingLines))),
					new CodeInstruction(OpCodes.Ldloc_0),
					new CodeInstruction(OpCodes.Call,
						AccessTools.Method(typeof(CatPatchHostSettingsMenu), nameof(CatPatchHostSettingsMenu.DrawToggleRow))),
					new CodeInstruction(OpCodes.Call,
						AccessTools.PropertySetter(typeof(Plugin), nameof(Plugin.ShouldFilterIncomingLines)))
					);
			return codeMatcher.InstructionEnumeration();
		}
	}

	[HarmonyPatch(typeof(TalkerCatPatch))]
	internal static class TalkerHugPatch
	{
		[HarmonyPatch(nameof(TalkerCatPatch.ChooseHugAcceptedLine))]
		[HarmonyPrefix]
		private static bool ChooseHugAcceptedLinePatch(ref string __result, Body speakerBody, Body receiverBody)
		{
			__result = DialogueManager.ChooseHugLine(speakerBody, receiverBody);
			return false;
		}

		[HarmonyPatch(nameof(TalkerCatPatch.PlayHugAcceptedLine))]
		[HarmonyPrefix]
		private static void PlayHugAcceptedLinePatch(knetid speakerClientId, ref string line)
		{
			if (!Plugin.ShouldFilterIncomingLines)
				line = DialogueManager.ReplaceHugLine(speakerClientId, line);
		}
	}

	[HarmonyPatch(typeof(KissDialogue))]
	internal static class KissDialoguePatch
	{
		[HarmonyPatch(nameof(KissDialogue.ChooseLine))]
		[HarmonyPrefix]
		private static bool ChooseLinePatch(ref string __result)
		{
			__result = DialogueManager.ChooseKissLine();
			return false;
		}
		
		[HarmonyPatch(nameof(KissDialogue.PlayLine))]
		[HarmonyPrefix]
		private static void PlayLinePatch(ref string line)
		{
			if (!Plugin.ShouldFilterIncomingLines)
				line = DialogueManager.ReplaceKissLine(line);
		}
	}
}
