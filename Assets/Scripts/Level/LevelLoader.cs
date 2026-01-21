using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class LevelLoader
{
    public static LevelStructure Load(string levelName, int partNumber)
    {
        TextAsset txt = Resources.Load<TextAsset>("Levels/" + levelName);
        if(txt == null)
        {
            Debug.Log("Level not found: " + levelName);
            return null;
        }

        string[] rawLines = txt.text
            .Replace("\r", "")
            .Split('\n');

        int playfieldDepth = 0;
        bool readingPart = false;
        List<string> partLines = new List<string>();

        foreach(string raw in rawLines)
        {
            string line = raw.Trim();
            if(line.Length == 0)
                continue;

            if (line.StartsWith("//"))
            {
                int.TryParse(line.Substring(2), out playfieldDepth);
                continue;
            }

            if (line.StartsWith("$"))
            {
                int index = int.Parse(line.Substring(1));

                if(index == partNumber)
                {
                    readingPart = true;
                    partLines.Clear();
                }
                else if (readingPart)
                {
                    break;
                }

                continue;
            }

            if (readingPart)
            {
                partLines.Add(line);
            }
        }

        if(partLines.Count == 0)
        {
            Debug.LogError($"Part ${partNumber} not found in {levelName}");
            return null;
        }

        int depth = partLines.Count;
        int width = partLines[0].Length;

        CubeModel.CubeType[,] grid = new CubeModel.CubeType[width, depth];

        for(int z=0; z<depth; z++)
        {
            string line = partLines[depth-1-z];
            for(int x=0; x<width; x++)
            {
                grid[x, z] = line[x] switch
                {
                    'G' => CubeModel.CubeType.Green,
                    'X' => CubeModel.CubeType.Gray,
                    'B' => CubeModel.CubeType.Black,
                    _ => CubeModel.CubeType.Gray,
                };
            }
        }

        return new LevelStructure{
            width = width,
            depth = depth,
            playfieldDepth = playfieldDepth,
            partIndex = partNumber,
            cells = grid
        };
    }
}
