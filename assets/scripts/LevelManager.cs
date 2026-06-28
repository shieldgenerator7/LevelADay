using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class LevelManager : Node2D
{
	[Export]
	public string fileNameLevelPrefab;

	[Export]
	public string levelSelectFileName;

	[Export]
	public Node2D explorer;

	[Export]
	public Camera2D camera;

	private Vector2I currentLevelGridPos = new Vector2I(0, 0);
	private LevelTest currentLevel;

	private List<Vector2I> loadedLevels = new List<Vector2I>();

	public LevelTest LoadLevel(Vector2I levelGridPos)
	{
		loadedLevels.Add(levelGridPos);

		Node2D levelNode = (Node2D)GD.Load<PackedScene>(Path.Combine("res://", fileNameLevelPrefab)).Instantiate();
		AddChild(levelNode);
		levelNode.Position = new Vector2(levelGridPos.X * 100 * 100, levelGridPos.Y * 100 * 100);

		LevelTest level = (LevelTest)levelNode;
		level.LevelGridPosition = levelGridPos;
		level.Initialize();

		level.LevelEntered += (l, b) =>
		{
			if (b == explorer)
			{
				setCurrentLevel(l);
			}
		};
		level.LevelEdged += (l, b) =>
		{
			if (b == explorer)
			{
				checkLoadLevel(l);
			}
		};

		
		GD.Print($"Loaded level: {levelGridPos}");
		return level;
	}

	public override void _Ready()
	{
		string path = Path.Combine("levels", levelSelectFileName);

		//create level select file if not exists
		if (!File.Exists(path))
		{
			File.WriteAllText(path, "0_0");
		}

		//read current level
		string text = File.ReadAllText(path);
		string[] ts = text.Split('_');
		Vector2I v = new Vector2I(Int32.Parse(ts[0]), Int32.Parse(ts[1]));

		//initialize current level
		currentLevelGridPos = v;
		currentLevel = LoadLevel(currentLevelGridPos);
		UpdateCamera(currentLevel);
	}

	private void setCurrentLevel(LevelTest level)
	{
		GD.Print($"Entered level: {level.LevelGridPosition}");
		currentLevel = level;
		currentLevelGridPos = currentLevel.LevelGridPosition;
		UpdateCamera(currentLevel);
	}

	private void checkLoadLevel(LevelTest level)
	{
		Vector2I gridPos = level.LevelGridPosition;
		List<Vector2I> levelsToLoad = new List<Vector2I>();

		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				if (!(x == 0 || y == 0)) continue;
				Vector2I v = new Vector2I(
					gridPos.X + x,
					gridPos.Y + y
				);
				if (loadedLevels.Contains(v)) continue;
				levelsToLoad.Add(v);
			}
		}

		GD.Print("Loading levels: " + levelsToLoad.Aggregate("",(bas, v)=>bas+v+", "));
		levelsToLoad.ForEach(checkLoadLevel);
	}
	private void checkLoadLevel(Vector2I levelGridPos)
	{
		if (!loadedLevels.Contains(levelGridPos))
		{
			LoadLevel(levelGridPos);
		}
	}

	private void UpdateCamera(LevelTest level)
	{
		//update camera limits
		camera.LimitLeft = level.LevelGridPosition.X * 100 * 100;
		camera.LimitTop = level.LevelGridPosition.Y * 100 * 100;
        camera.LimitRight = camera.LimitLeft + level.levelWidth * 100;
		camera.LimitBottom = camera.LimitTop + level.levelHeight * 100;

		//unlimit camera if the level is empty
		camera.LimitEnabled = level.levelWidth > 0 && level.levelHeight > 0;
    }

}
