using Godot;
using System;

public partial class LevelTest : Node2D
{
	[Export]
	public TileMapLayer tileMap;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		int[,] mapData = getMapData();
		for (int x = 0; x < mapData.GetLength(0); x++)
		{
			for (int y= 0; y < mapData.GetLength(1); y++)
			{
				//x and y are switched on purpose here, idk why it has to be this way, but it works
				int tileId = (mapData[y,x] == 1) ? 0 : -1;
				tileMap.SetCell(new Vector2I(x, y), tileId, new Vector2I(0,0));
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private int[,] getMapData()
	{
		int[,] mapData = new int[3,3] { { 1, 1, 1}, { 1, 0, 0}, { 0, 1, 0} };
		return mapData;
	}
}
