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
	public string filename;

	[Export]
	public string levelFolder;

	[Export]
	public TileMapLayer tileMap;


	public Dictionary<string, int> pixelKeyMap = new Dictionary<string, int>
	{
		{"0_0_0", 0 },//black = empty space
		{"255_255_255", 1 },//white = block
	};

	private int levelWidth = 0;
	private int levelHeight = 0;

	private string[] levelList;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		levelList = getLevelList();

		RandomNumberGenerator rng = new RandomNumberGenerator();
		rng.Seed = (ulong)System.DateTime.Now.Ticks;
		int index = rng.RandiRange(0, levelList.Length-1);
		string levelName = levelList[index];

		int[,] mapData = readBitMapData($"{levelName}.bmp");

		for (int x = 0; x < levelWidth; x++)
		{
			for (int y= 0; y < levelHeight; y++)
			{
				//x and y are switched on purpose here, idk why it has to be this way, but it works
				int tileId = (mapData[x,y] == 1) ? 0 : -1;
				tileMap.SetCell(new Vector2I(x, y), tileId, new Vector2I(0,0));
			}
		}

		//update camera limits
		Camera2D camera = (Camera2D)GetNode("../").FindChild("Camera2D");
		camera.LimitRight = levelWidth*100;
		camera.LimitBottom = levelHeight*100;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private int[,] getMapData()
	{
		int[,] mapData = new int[3, 3] { { 1, 1, 1 }, { 1, 0, 0 }, { 0, 1, 0 } };
		return mapData;
	}
	
	private int[,] readBitMapData(string filename)
	{
		//TODO: check to make sure file is bitmap
		//Bitmap bmp = new Bitmap();
		int[,] mapData = new int[100, 100];
		Image bmp = Image.LoadFromFile(Path.Combine("res://",levelFolder, filename));

		

		//2026-06-22: copied from https://learn.microsoft.com/en-us/dotnet/api/system.drawing.bitmap?view=net-11.0-pp
		// Loop through the images pixels to reset color.
		levelWidth = bmp.GetWidth();
		levelHeight = bmp.GetHeight();
		for (int x = 0; x < levelWidth; x++)
		{
			for (int y = 0; y < levelHeight; y++)
			{
				Color pixelColor = bmp.GetPixel(x, y);
				string pixelKey = String.Concat((int)pixelColor.R*255, "_", (int)pixelColor.G*255, "_", (int)pixelColor.B*255);
				GD.Print(pixelKey);
				mapData[x, y] = pixelKeyMap[pixelKey];
				//List<int> a = new List<int>();
				//a.Any(x=>x>0);
			}
		}

		return mapData;
	}

	public string[] getLevelList()
	{
		//TODO: check for levels in the folder that are not in the file
		string text = File.ReadAllText(Path.Combine(levelFolder, "levels.txt"));
		return text.Split("\n").ToList()
			.FindAll(t=>!String.IsNullOrEmpty(t))
			.ConvertAll(t=>t.Trim())
			.ToArray();
	}
}
