using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public partial class LevelManager : Node2D
{
	[Export]
	public string fileNameLevelPrefab;

	[Export]
	public string levelSelectFileName;

	[Export]
	public Node2D explorer;

	private Vector2I currentLevelGridPos = new Vector2I(0, 0);
	private LevelTest currentLevel;

	private List<Vector2I> loadedLevels = new List<Vector2I>();

	public LevelTest LoadLevel(Vector2I levelGridPos)
	{
		loadedLevels.Add(levelGridPos);

		Node2D levelNode = (Node2D)GD.Load<PackedScene>(Path.Combine("res://", fileNameLevelPrefab)).Instantiate();
		AddChild(levelNode);
		levelNode.Position = new Vector2(levelGridPos.X * 100, levelGridPos.Y * 100);

		LevelTest level = (LevelTest)levelNode;
		level.LevelGridPosition = levelGridPos;
		level.Initialize();

		level.levelCenterArea.AreaExited += checkLoadLevel;
		level.levelWholeArea.AreaEntered += setCurrentLevel;

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
		currentLevel.GrabCamera();
	}

	private void setCurrentLevel(Area2D area)
	{
		currentLevel = area.GetParent<LevelTest>();
		currentLevelGridPos = currentLevel.LevelGridPosition;
		currentLevel.GrabCamera();
	}

	private void checkLoadLevel(Area2D area)
	{
		List<Vector2I> levelsToLoad = new List<Vector2I>();
		if (explorer.Position.X < area.Position.X)
		{
			levelsToLoad.Add(new Vector2I(
				currentLevelGridPos.X - 1,
				currentLevelGridPos.Y
				));
		}
		if (explorer.Position.X > area.Position.X)
		{
			levelsToLoad.Add(new Vector2I(
				currentLevelGridPos.X + 1,
				currentLevelGridPos.Y
				));
		}
		if (explorer.Position.Y < area.Position.Y)
		{
			levelsToLoad.Add(new Vector2I(
				currentLevelGridPos.X,
				currentLevelGridPos.Y - 1
				));
		}
		if (explorer.Position.Y > area.Position.Y)
		{
			levelsToLoad.Add(new Vector2I(
				currentLevelGridPos.X,
				currentLevelGridPos.Y + 1
				));
		}

		levelsToLoad.ForEach(v => checkLoadLevel(v));
	}
	private void checkLoadLevel(Vector2I levelGridPos)
	{
		if (!loadedLevels.Contains(levelGridPos))
		{
			LoadLevel(levelGridPos);
		}
	}

}
