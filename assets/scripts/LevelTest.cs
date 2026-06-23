using Godot;
using System;

public partial class LevelTest : Node2D
{
	[Export]
	public TileMapLayer tileMap;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		tileMap.SetCell(new Vector2I(1,1), 0, new Vector2I(0,0));
		tileMap.SetCell(new Vector2I(0,0), -1);
		tileMap.SetCell(new Vector2I(1,0), -1);
		tileMap.SetCell(new Vector2I(0, 2), -1);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
