using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Utitily
{
    public partial class RandomName
    {
        public static string returnJsonNames(string filePath, string[] keyPair)
        {
            var json = new Json { Data = Json.ParseString(Godot.FileAccess.GetFileAsString(filePath)) };
            Godot.Collections.Dictionary<string, string[]> nodeDict = new((Godot.Collections.Dictionary)json.Data);
            RandomNumberGenerator nameRNG = new();
            return nodeDict[keyPair.First()][nameRNG.RandiRange(0, nodeDict[keyPair.First()].Length-1)] + " " + nodeDict[keyPair.Last()][nameRNG.RandiRange(0, nodeDict[keyPair.Last()].Length-1)];
        }
        public static string returnDictNames(Godot.Collections.Dictionary<string,string[]> dict, string[] keyPair)
        {
            RandomNumberGenerator nameRNG = new();
            return dict[keyPair.First()][nameRNG.RandiRange(0, dict[keyPair.First()].Length-1)] + " " + dict[keyPair.Last()][nameRNG.RandiRange(0, dict[keyPair.Last()].Length-1)];
        }
    }
}
