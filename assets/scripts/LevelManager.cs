using Godot;
using System.IO;

public partial class LevelManager : Node2D
{
	[Export]
	public string fileNameLevelPrefab;

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
		LoadLevel(currentLevelGridPos);
	}
}
