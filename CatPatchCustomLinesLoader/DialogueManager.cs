using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using KrokoshaCasualtiesMP;
using Newtonsoft.Json;
using Random = UnityEngine.Random;

namespace CatPatchCustomLinesLoader;

public static class DialogueManager
{
    public class Data
    {
        public List<string> VulnerableHugLines = [];
        public List<string> ComfortHugLines = [];
        public List<string> CheerfulHugLines = [];
        public List<string> NeutralHugLines = [];
        public List<string> KissLines = [];
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
            foreach (var line in data.VulnerableHugLines)
                linesSet.Add(line);
            foreach (var line in data.ComfortHugLines)
                linesSet.Add(line);
            foreach (var line in data.CheerfulHugLines)
                linesSet.Add(line);
            foreach (var line in data.NeutralHugLines)
                linesSet.Add(line);
            foreach (var line in data.KissLines)
                linesSet.Add(line);
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
        return "Found no dialogue lines. Add them to CatPatchCustomLinesLoader/CatPatchCustomLines.json";
    }

    public static string ChooseHugLine(Body speakerBody, Body receiverBody)
    {
        LoadLines();
        if (speakerBody != null && speakerBody.totalHappiness <= -40.0)
            return ChooseLine(data.VulnerableHugLines);
        if (receiverBody != null && receiverBody.totalHappiness <= -40.0)
            return ChooseLine(data.ComfortHugLines);
        if (speakerBody != null && speakerBody.totalHappiness >= 40.0)
            return ChooseLine(data.CheerfulHugLines);
        return ChooseLine(data.NeutralHugLines);
    }

    public static string ReplaceHugLine(knetid speakerClientId, string line)
    {
        LoadLines();
        if (linesSet.Contains(line))
            return line;
        NetPlayer.ClientIdToPlayerDict.TryGetValue(speakerClientId, out var playerId);
        if (playerId == null)
            return "";
        if (playerId.body.totalHappiness <= -40.0)
            return ChooseLine(data.VulnerableHugLines);
        if (playerId.body.totalHappiness >= 40.0)
            return ChooseLine(data.CheerfulHugLines);
        return ChooseLine(data.NeutralHugLines);
    }

    public static string ChooseKissLine()
    {
        LoadLines();
        return ChooseLine(data.KissLines);
    }

    public static string ReplaceKissLine(string line)
    {
        LoadLines();
        return linesSet.Contains(line) ? line : ChooseLine(data.KissLines);
    }
}