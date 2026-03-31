using SpeedrunTimer.Config;
using SpeedrunTimer.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SpeedrunTimer.Systems;

public class AutoRestartPlayerCheck : ModPlayer
{
    public const string LoadedPlayerKey = "SpeedrunPlayerID";

    public static int LoadedPlayerID { get; private set; } = -1;

    public static int TotalLoadedPlayers { get; private set; } = 0;

    public static int LastLoadedPlayer { get; private set; } = -1;

    public override void LoadData(TagCompound tag)
    {
        if (tag.TryGet(LoadedPlayerKey, out int id))
            LoadedPlayerID = id;

        else
            LoadedPlayerID = -1;
    }

    public override void SaveData(TagCompound tag)
    {
        if (LoadedPlayerID == -1)
            LoadedPlayerID = TotalLoadedPlayers++;

        tag.Set(LoadedPlayerKey, LoadedPlayerID);
        LastLoadedPlayer = LoadedPlayerID;
    }

    public override void OnEnterWorld()
    {
        if (LastLoadedPlayer == -1 || AutoRestartWorldCheck.LastLoadedWorld == -1)
            return;

        if (LoadedPlayerID == LastLoadedPlayer || AutoRestartWorldCheck.LoadedWorldID == AutoRestartWorldCheck.LastLoadedWorld)
            return;

        RunTracker.CancelRun();

        foreach (KeyValuePair<string, Category> kvp in SpeedrunTimer.AllCategories)
        {
            if (SpeedrunConfig.Instance.DefaultRunCategory != Language.GetTextValue(kvp.Value.LocalizationKey))
                continue;

            RunTracker.StartRun(kvp.Key);
            break;
        }
    }
}

public class AutoRestartWorldCheck : ModSystem
{
    public const string LoadedWorldKey = "SpeedrunWorldID";

    public static int LoadedWorldID { get; private set; } = -1;

    public static int TotalLoadedWorlds { get; private set; } = 0;

    public static int LastLoadedWorld { get; private set; } = -1;

    public override void LoadWorldData(TagCompound tag)
    {
        if (tag.TryGet(LoadedWorldKey, out int id))
            LoadedWorldID = id;

        else
            LoadedWorldID = -1;
    }

    public override void SaveWorldData(TagCompound tag)
    {
        if (LoadedWorldID == -1)
            LoadedWorldID = TotalLoadedWorlds++;

        tag.Set(LoadedWorldKey, LoadedWorldID);
        LastLoadedWorld = LoadedWorldID;
    }
}

public class RunUpdater : ModSystem
{
    public override void PostUpdatePlayers()
    {
        // Obvious...
        if (!RunTracker.RunActive)
            return;

        // We don't want to start the timer until the player enters the world,
        // so we do this here instead of in the mod system.
        if (RunTracker.IGT_FrameCounter == 0)
            RunTracker.RTA_RunStart = DateTime.UtcNow;

        // Retrieve the current run category and the run end criteria.
        var runCategory = SpeedrunTimer.AllCategories[RunTracker.RunCategory!];
        var completionSplit = runCategory.CompletionSplit;

        // Complete the run if the run end criteria has been met.
        if (completionSplit.CompletionCheck())
        {
            completionSplit.CreateSplit().Register();
            RunTracker.CompleteRun();
            return;
        }

        // Clone the collection as it may be modified during enumeration
        var splits = RunTracker.AvailableSplits.ToArray();

        // Update splits
        foreach (var split in splits)
        {
            if (!split.CompletionCheck())
                continue;

            split.CreateSplit().Register();
        }

        // Update IGT
        RunTracker.IGT_FrameCounter++;

        // We do not actually keep track of RTA, instead subtracting current
        // UTC DateTime from the cached start-of-run UTC DateTime when we need the run length.
    }
}
