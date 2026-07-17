using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using BepInEx;
using CatPatchspace;
using HarmonyLib;
using JetBrains.Annotations;
using KrokoshaCasualtiesMP;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CatPatchCustomLinesLoader;

public static class DialogueManager
{
    public class Data
    {
        public List<string> FinalHugLines = []; // Speaker dying
        public List<string> GoodbyeHugLines = []; // Receiver dying
        public List<string> GleefulPairHugLines = []; // Speaker & Receiver happiness >= 75
        public List<string> VulnerableHugLines = []; // Speaker happiness <= -40
        public List<string> ComfortHugLines = []; // Receiver happiness <= -40
        public List<string> SickHugLines = []; // Speaker sickness > 75
        public List<string> SpeakerMissingArmHugLines = []; // Speaker is missing an arm
        public List<string> ReceiverMissingArmHugLines = []; // Receiver is missing an arm
        public List<string> HotHugLines = []; // Speaker temperature > 38
        public List<string> ColdHugLines = []; // Speaker temperature < 35.5
        public List<string> DirtyHugLines = []; // Receiver dirtiness >= 75
        public List<string> OverweightHugLines = []; // Receiver weightOffset > 15
        public List<string> CheerfulHugLines = []; // Speaker happiness >= 40
        public List<string> HostHugLines = []; // Receiver is host
        public List<string> NeutralHugLines = []; // Default hug lines
        
        public List<string> FinalKissLines = []; // Speaker dying
        public List<string> GoodbyeKissLines = []; // Receiver dying
        public List<string> DisfiguredKissLines = []; // Speaker or Receiver is disfigured
        public List<string> DirtyKissLines = []; // Receiver dirtiness >= 75
        public List<string> KissLines = []; // Default kiss lines
    }

    public static Data data = new();
    public static DateTime lastWriteTime = DateTime.MinValue;
    public static HashSet<string> linesSet = [];

    public static void LoadLines()
    {
        var linesPath = Path.Combine(Paths.PluginPath, "CatPatchCustomLinesLoader", "CatPatchCustomLines.json");
        try
        {
            if (!File.Exists(linesPath))
            {
                data = new Data();
                linesSet.Clear();
                File.WriteAllText(linesPath, JsonConvert.SerializeObject(data, Formatting.Indented));
                Plugin.Logger.LogWarning("Found no dialogue lines. Add them to CatPatchCustomLinesLoader/CatPatchCustomLines.json");
                ConsoleScript.instance.LogToConsole("[CatPatchCustomLinesLoader] Found no dialogue lines. Add them to CatPatchCustomLinesLoader/CatPatchCustomLines.json");
                return;
            }

            var writeTime = File.GetLastWriteTimeUtc(linesPath);
            if (writeTime == lastWriteTime)
                return;
            lastWriteTime = writeTime;

            data = JsonConvert.DeserializeObject<Data>(File.ReadAllText(linesPath)) ?? new Data();
            linesSet.Clear();
            foreach (var list in typeof(Data).GetFields(AccessTools.all).Select(field => (List<string>)field.GetValue(data)))
                list.ForEach(line => linesSet.Add(line));
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogWarning("Failed to load CatPatchCustomLines.json: " + ex.Message);
            ConsoleScript.instance.LogToConsole("[CatPatchCustomLinesLoader] Failed to load CatPatchCustomLines.json: " + ex.Message);
            data = new Data();
            linesSet.Clear();
        }
    }

    public static string ChooseLine(List<string> lines)
    {
        if (lines.Count > 0)
            return lines[Random.Range(0, lines.Count)];
        if (data.NeutralHugLines.Count > 0)
            return data.NeutralHugLines[Random.Range(0, lines.Count)];
        return "Found no dialogue lines. Add them to CatPatchCustomLinesLoader/CatPatchCustomLines.json";
    }

    [CanBeNull]
    public static string ChooseLineIf(List<string> lines, Func<bool> predicate)
    {
        if (lines.Count > 0 && predicate.Invoke())
            return ChooseLine(lines);
        return null;
    }

    public static string ChooseHugLine(Body speakerBody, Body receiverBody)
    {
        LoadLines();
        if (speakerBody == null || receiverBody == null)
            return ChooseLine(data.NeutralHugLines);
        return
            ChooseLineIf(data.FinalHugLines, () => speakerBody.isDying || speakerBody.isCriticallyDying || speakerBody.brainDying)
            ?? ChooseLineIf(data.GoodbyeHugLines, () => receiverBody.isDying || receiverBody.isCriticallyDying || receiverBody.brainDying)
            ?? ChooseLineIf(data.GleefulPairHugLines, () => speakerBody.totalHappiness >= 75.0f && receiverBody.totalHappiness >= 75.0f)
            ?? ChooseLineIf(data.VulnerableHugLines, () => speakerBody.totalHappiness <= -40.0f)
            ?? ChooseLineIf(data.ComfortHugLines, () => receiverBody.totalHappiness <= -40.0f)
            ?? ChooseLineIf(data.SpeakerMissingArmHugLines, () => speakerBody.limbs[3].dismembered || speakerBody.limbs[6].dismembered)
            ?? ChooseLineIf(data.ReceiverMissingArmHugLines, () => receiverBody.limbs[3].dismembered || receiverBody.limbs[6].dismembered)
            ?? ChooseLineIf(data.SickHugLines, () => speakerBody.sicknessAmount > 75.0f)
            ?? ChooseLineIf(data.HotHugLines, () => speakerBody.temperature > 38.0f)
            ?? ChooseLineIf(data.ColdHugLines, () => speakerBody.temperature < 35.5f)
            ?? ChooseLineIf(data.DirtyHugLines, () => receiverBody.dirtyness >= 75.0f)
            ?? ChooseLineIf(data.OverweightHugLines, () => receiverBody.weightOffset > 15.0f)
            ?? ChooseLineIf(data.CheerfulHugLines, () => speakerBody.totalHappiness >= 40.0f)
            ?? ChooseLineIf(data.HostHugLines, () => NetPlayer.GetNetPlayerFromBody(receiverBody)?.is_host == true)
            ?? ChooseLine(data.NeutralHugLines);
    }

    public static void PlayHugLine(knetid speakerClientId, knetid receiverClientId, string line)
    {
        if (Plugin.ShouldFilterIncomingLines && !linesSet.Contains(line))
        {
            NetPlayer.TryGetNetPlayerAndBodyFromClientId(speakerClientId, out _, out var speakerBody);
            NetPlayer.TryGetNetPlayerAndBodyFromClientId(receiverClientId, out _, out var receiverBody);
            line = ChooseHugLine(speakerBody, receiverBody);
        }
        HugNetworkController._instance._talkerCatPatch.PlayHugAcceptedLine(speakerClientId, line);
    }

    public static string ChooseKissLine(knetid speakerClientId, knetid receiverClientId)
    {
        LoadLines();
        NetPlayer.TryGetNetPlayerAndBodyFromClientId(speakerClientId, out _, out var speakerBody);
        NetPlayer.TryGetNetPlayerAndBodyFromClientId(receiverClientId, out _, out var receiverBody);
        if (speakerBody == null || receiverBody == null)
            return ChooseLine(data.KissLines);
        return
            ChooseLineIf(data.FinalKissLines, () => speakerBody.isDying || speakerBody.isCriticallyDying || speakerBody.brainDying)
            ?? ChooseLineIf(data.GoodbyeKissLines, () => receiverBody.isDying || receiverBody.isCriticallyDying || receiverBody.brainDying)
            ?? ChooseLineIf(data.DisfiguredKissLines, () => speakerBody.disfigured || receiverBody.disfigured)
            ?? ChooseLineIf(data.DirtyKissLines, () => receiverBody.dirtyness >= 75.0f)
            ?? ChooseLine(data.KissLines);
    }

    public static void PlayKissLine(knetid speakerClientId, knetid receiverClientId, string line)
    {
        if (Plugin.ShouldFilterIncomingLines && !linesSet.Contains(line))
        {
            line = ChooseKissLine(speakerClientId, receiverClientId);
        }
        KissNetworkController._instance._dialogue.PlayLineDelayed(speakerClientId, line, 1.0f);
    }
}