using Godot;
using System;
using System.IO;
//using System.Drawing;
using System.Data;
using System.Collections.Generic;
using System.Linq;

public partial class LevelTest : Node2D
{

    [Export]
    public string levelFolder;

    [Export]
    public TileMapLayer tileMap;

    [Export]
    public Area2D levelCenterArea;
    [Export]
    public Area2D levelWholeArea;


    private string _levelName;
    public string LevelName
    {
        get => _levelName;
        set
        {
            _levelName = value;
            fileNameLevel = $"{_levelName}.bmp";
            fileNameBackground = $"{_levelName}.png";
            fileNameForeground = $"{_levelName}.jpg";
            fileNameText = $"{_levelName}.txt";
            fileNameSettings = $"{_levelName}.json";
        }
    }
    private string fileNameLevel;
    private string fileNameBackground;
    private string fileNameForeground;
    private string fileNameText;
    private string fileNameSettings;

    private Vector2I _levelGridPos = new Vector2I();
    public Vector2I LevelGridPosition
    {
        get => _levelGridPos;
        set
        {
            _levelGridPos = value;
            LevelName = String.Concat(_levelGridPos.X, "_", _levelGridPos.Y);
        }
    }


    public Dictionary<string, int> pixelKeyMap = new Dictionary<string, int>
    {
        {"0_0_0", 0 },//black = empty space
		{"255_255_255", 1 },//white = block
	};

    public int levelWidth { get; private set; }
    public int levelHeight { get; private set; }

    private string[] levelList;


    public void Initialize()
    {
        if (String.IsNullOrWhiteSpace(_levelName))
        {
            LevelGridPosition = new Vector2I(0, 0);
        }

        if (doesLevelExist(LevelName))
        {
            int[,] mapData = readBitMapData(fileNameLevel);

            for (int x = 0; x < levelWidth; x++)
            {
                for (int y = 0; y < levelHeight; y++)
                {
                    int tileId = (mapData[x, y] == 1) ? 0 : -1;
                    tileMap.SetCell(new Vector2I(x, y), tileId, new Vector2I(0, 0));
                }
            }
        }

        levelWholeArea.BodyEntered += (body) =>
        {
            LevelEntered(this, body);
        };
        levelCenterArea.BodyExited += (body) =>
        {
            LevelEdged(this, body);
        };
    }
    public event Action<LevelTest, Node2D> LevelEntered;
    public event Action<LevelTest, Node2D> LevelEdged;

    private int[,] readBitMapData(string filename)
    {
        //TODO: check to make sure file is bitmap
        int[,] mapData = new int[100, 100];
        Image bmp = Image.LoadFromFile(Path.Combine("res://", levelFolder, filename));



        //2026-06-22: copied from https://learn.microsoft.com/en-us/dotnet/api/system.drawing.bitmap?view=net-11.0-pp
        // Loop through the images pixels to reset color.
        levelWidth = bmp.GetWidth();
        levelHeight = bmp.GetHeight();
        for (int x = 0; x < levelWidth; x++)
        {
            for (int y = 0; y < levelHeight; y++)
            {
                Color pixelColor = bmp.GetPixel(x, y);
                string pixelKey = String.Concat((int)pixelColor.R * 255, "_", (int)pixelColor.G * 255, "_", (int)pixelColor.B * 255);
                mapData[x, y] = pixelKeyMap[pixelKey];
            }
        }

        return mapData;
    }

    public bool doesLevelExist(string levelName)
    {
        string[] filenameList = new string[]
        {
            fileNameLevel,
            fileNameBackground,
            fileNameForeground,
            fileNameText,
            fileNameSettings,
        };
        return filenameList.Any(fileName => !String.IsNullOrWhiteSpace(fileName) && File.Exists(Path.Combine(levelFolder, fileName)));
    }
}
