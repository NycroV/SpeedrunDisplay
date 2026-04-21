global using CastCompletedRun = (SpeedrunDisplay.DataStructures.Category category, System.Collections.ObjectModel.ReadOnlyCollection<SpeedrunDisplay.DataStructures.RunSplit> splits, System.TimeSpan IGT, System.TimeSpan RTA);

using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace SpeedrunDisplay.DataStructures;

public readonly record struct CompletedRun(Category Category, ReadOnlyCollection<RunSplit> Splits, TimeSpan IGT, TimeSpan RTA)
{
    public static implicit operator CastCompletedRun(CompletedRun completedRun) => (completedRun.Category, completedRun.Splits, completedRun.IGT, completedRun.RTA);

    public override string ToString()
    {
        string text = $"{Category.LocalizationKey.Fetch()}\n------\n";

        int splitCount = Splits.Count;
        int longestNameLength = 0;
        int longestSplitLength = 0;
        int longestRunLength = 0;

        string[] splits = [.. Splits.Select(s => s.Split.LocalizationKey.Fetch())];
        string[] splitTimes = [.. Splits.Select(s => TimeSpan.FromSeconds(s.SplitTime / 60f).Format(fractionalSeconds: true))];
        string[] runTimes = [.. Splits.Select(s => TimeSpan.FromSeconds(s.RunTime / 60f).Format(fractionalSeconds: true))];

        for (int i = 0; i < splitCount; i++)
        {
            if (splits[i].Length > longestNameLength)
            {
                longestNameLength = splits[i].Length;

                for (int j = 0; j < i; j++)
                    splits[j] = splits[j] + new string(' ', longestNameLength - splits[j].Length);
            }

            else if (splits[i].Length < longestNameLength)
                splits[i] = splits[i] + new string(' ', longestNameLength - splits[i].Length);


            if (splitTimes[i].Length > longestSplitLength)
            {
                longestSplitLength = splitTimes[i].Length;

                for (int j = 0; j < i; j++)
                    splitTimes[j] = new string(' ', longestSplitLength - splitTimes[i].Length) + splitTimes[j];
            }

            else if (splitTimes[i].Length < longestSplitLength)
                splitTimes[i] = splitTimes[i] + new string(' ', longestSplitLength - splitTimes[i].Length);

            if (runTimes[i].Length > longestRunLength)
            {
                longestRunLength = runTimes[i].Length;

                for (int j = 0; j < i; j++)
                    runTimes[j] = new string(' ', longestRunLength - runTimes[i].Length) + runTimes[j];
            }

            else if (runTimes[i].Length < longestRunLength)
                runTimes[i] = runTimes[i] + new string(' ', longestRunLength - runTimes[i].Length);
        }

        for (int i = 0; i < splitCount; i++)
            text += $"{splits[i]}  -  {splitTimes[i]}  |  {runTimes[i]}\n";

        text += $"------\n{IGT.Format(fractionalSeconds: true)} IGT  --  {RTA.Format(fractionalSeconds: true)} RTA";
        return text;
    }

    public static CompletedRun FromString(string str)
    {
        return default;
    }
}