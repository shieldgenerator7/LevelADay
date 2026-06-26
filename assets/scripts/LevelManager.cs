using Godot;
using System;
using System.IO;

public partial class LevelManager : Node2D
{
	[Export]
	public string fileNameLevelPrefab;

	[Export]
	public string levelSelectFileName;

	private Vector2I currentLevelGridPos = new Vector2I(0,0);

	public void LoadLevel(Vector2I levelGridPos)
	{
		Node2D levelNode = (Node2D)GD.Load<PackedScene>(Path.Combine("res://", fileNameLevelPrefab)).Instantiate();
		AddChild(levelNode);
		levelNode.Position = new Vector2(levelGridPos.X*100, levelGridPos.Y*100);

		LevelTest level = (LevelTest)levelNode;
		level.LevelGridPosition = levelGridPos;
		level.Initialize();
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
		LoadLevel(currentLevelGridPos);
	}
}
