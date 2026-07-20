using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using CatPatchspace;

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
				.ThrowIfInvalid("DrawWindowContentsTranspiler could not find a match!");
			
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
	}

	[HarmonyPatch(typeof(HugNetworkController))]
	internal static class HugNetworkControllerPatch
	{
		[HarmonyPatch(nameof(HugNetworkController.ReceiveStartLocal))]
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> ReceiveStartLocalTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			return new CodeMatcher(instructions).MatchForward(false,
					new CodeMatch(OpCodes.Ldarg_0),
					new CodeMatch(OpCodes.Ldfld, 
						AccessTools.Field(typeof(HugNetworkController), nameof(HugNetworkController._talkerCatPatch))),
					new CodeMatch(OpCodes.Ldarg_1),
					new CodeMatch(OpCodes.Ldarg_S, (byte)4),
					new CodeMatch(OpCodes.Callvirt,
						AccessTools.Method(typeof(TalkerCatPatch),
							nameof(TalkerCatPatch.PlayHugAcceptedLine))))
				.ThrowIfInvalid("HugNetworkControllerPatch.ReceiveStartLocalTranspiler could not find a match!")
				.RemoveInstructions(5)
				.Insert(new CodeInstruction(OpCodes.Ldarg_1),
					new CodeInstruction(OpCodes.Ldarg_2),
					new CodeInstruction(OpCodes.Ldarg_S, (byte)4),
					new CodeInstruction(OpCodes.Call,
						AccessTools.Method(typeof(DialogueManager), nameof(DialogueManager.PlayHugLine))))
				.InstructionEnumeration();
		}
	}

	[HarmonyPatch(typeof(KissNetworkController))]
	internal static class KissNetworkControllerPatch
	{
		[HarmonyPatch(nameof(KissNetworkController.StartKissOnServer))]
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> StartKissOnServerTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			var codeMatcher = new CodeMatcher(instructions).MatchForward(false,
					new CodeMatch(OpCodes.Ldarg_0),
					new CodeMatch(OpCodes.Ldfld,
						AccessTools.Field(typeof(KissNetworkController), nameof(KissNetworkController._dialogue))),
					new CodeMatch(OpCodes.Callvirt,
						AccessTools.Method(typeof(KissDialogue), nameof(KissDialogue.ChooseLine))))
				.ThrowIfInvalid("KissNetworkControllerPatch.StartKissOnServer could not find a match!");
			
			var labels = codeMatcher.Labels;
			return codeMatcher
				.Insert(new CodeInstruction(OpCodes.Nop))
				.AddLabels(labels)
				.Advance(1)
				.RemoveInstructions(3)
				.Insert(new CodeInstruction(OpCodes.Ldarg_1),
					new CodeInstruction(OpCodes.Ldarg_2),
					new CodeInstruction(OpCodes.Call,
						AccessTools.Method(typeof(DialogueManager), nameof(DialogueManager.ChooseKissLine))))
				.InstructionEnumeration();
		}
		
		[HarmonyPatch(nameof(KissNetworkController.ReceiveStartLocal))]
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> ReceiveStartLocalTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			return new CodeMatcher(instructions).MatchForward(false,
					new CodeMatch(OpCodes.Ldarg_0),
					new CodeMatch(OpCodes.Ldfld, 
						AccessTools.Field(typeof(KissNetworkController), nameof(KissNetworkController._dialogue))),
					new CodeMatch(OpCodes.Ldarg_1),
					new CodeMatch(OpCodes.Ldarg_S, (byte)4),
					new CodeMatch(OpCodes.Ldc_R4, 1.0f),
					new CodeMatch(OpCodes.Callvirt,
						AccessTools.Method(typeof(KissDialogue),
							nameof(KissDialogue.PlayLineDelayed))))
				.ThrowIfInvalid("KissNetworkControllerPatch.ReceiveStartLocalTranspiler could not find a match!")
				.RemoveInstructions(6)
				.Insert(new CodeInstruction(OpCodes.Ldarg_1),
					new CodeInstruction(OpCodes.Ldarg_2),
					new CodeInstruction(OpCodes.Ldarg_S, (byte)4),
					new CodeInstruction(OpCodes.Call,
						AccessTools.Method(typeof(DialogueManager), nameof(DialogueManager.PlayKissLine))))
				.InstructionEnumeration();
		}
	}
}
