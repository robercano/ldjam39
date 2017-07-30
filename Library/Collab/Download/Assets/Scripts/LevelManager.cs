using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
	public int currentLevel = 1;
	public int gameLevelWidthInTiles = 41;
	public int gameLevelHeightInTiles = 21;

	public GameObject floorTilePrefab;
	public GameObject wallTilePrefab;

 	const int FloorTileID = 0;
 	const int WallTileID = 1;

	const string FloorTileTag = "Floor";
	const string WallTileTag = "Wall";
	const string TilesParent = "Tiles";

	private GameObject[,] stageTiles;
	private Color[] floorTileColors;
	private Color[] wallTileColors;

	void Awake()
	{
		floorTileColors = new Color[4];

		floorTileColors [0] = new Color (176/255.0f, 97/255.0f, 255/255.0f);
		floorTileColors [1] = new Color (158/255.0f, 87/255.0f, 230/255.0f);
		floorTileColors [2] = new Color (140/255.0f, 78/255.0f, 204/255.0f);
		floorTileColors [3] = new Color (123/255.0f, 68/255.0f, 179/255.0f);

		wallTileColors = new Color[4];

		wallTileColors [0] = new Color (241/255.0f, 50/255.0f, 143/255.0f);
		wallTileColors [1] = new Color (217/255.0f, 45/255.0f, 129/255.0f);
		wallTileColors [2] = new Color (199/255.0f, 40/255.0f, 114/255.0f);
		wallTileColors [3] = new Color (179/255.0f, 37/255.0f, 100/255.0f);

		InitializeGameTiles ();
	}

	void Start () {
		int[,] levelConfig;

		LoadLevelConfig (currentLevel, out levelConfig);
		GenerateLevelFromConfig (levelConfig);
	}

	void InitializeGameTiles()
	{
		GameObject levelParent = GameObject.Find (TilesParent);
		stageTiles = new GameObject[gameLevelWidthInTiles, gameLevelHeightInTiles];

		for (int column = 0; column < stageTiles.GetLength (0); ++column) {
			for (int row=0; row < stageTiles.GetLength(1); ++row) {
				GameObject tile = InstantiateTile(row, column, levelParent);

				SetTile(tile, row, column);
			}
		}
	}

	void LoadLevelConfig(int level, out int[,] levelConfig)
	{
		// TODO: Read config from file
		levelConfig = new int[gameLevelWidthInTiles, gameLevelHeightInTiles];

		for (int column = 0; column < levelConfig.GetLength (0); ++column) {
			for (int row = 0; row < levelConfig.GetLength (1); ++row) {
				if (row == 0 || row == gameLevelHeightInTiles - 1 ||
				    column == 0 || column == gameLevelWidthInTiles - 1) {
					levelConfig [column, row] = WallTileID;
				} else {
					levelConfig [column, row] = FloorTileID;
				}
			}
		}
	}

	void GenerateLevelFromConfig(int[,] levelConfig)
	{
		for (int column = 0; column < levelConfig.GetLength (0); ++column) {
			for (int row=0; row<levelConfig.GetLength(1); ++row) {
				GameObject tile = GetTile (row, column);
				int tileID = levelConfig [column, row];

				ApplyBaseConfigToTile (tile, tileID);

				RandomizeTileMaterialColor (tile, row, column);

				if (IsWallTile (tile)) {
					RandomizeTileHeight (tile, row, column);
				}

			}
		}
	}

	GameObject InstantiateTile(int row, int column, GameObject parent)
	{
		GameObject tile;

		if (row == 0 || row == gameLevelHeightInTiles - 1 ||
		    column == 0 || column == gameLevelWidthInTiles - 1) {
			tile = Instantiate (wallTilePrefab, parent.transform);
		} else {
			tile = Instantiate (floorTilePrefab, parent.transform);
		}

		tile.transform.position = new Vector3 (column - (gameLevelWidthInTiles - 1.0f)/2.0f, 0.0f, row - (gameLevelHeightInTiles - 1.0f)/2.0f);

		return tile;
	}

	GameObject GetTile(int row, int column)
	{
		return stageTiles [column, row];
	}
	void SetTile(GameObject tile, int row, int column)
	{
		stageTiles [column, row] = tile;
	}

	void ApplyBaseConfigToTile(GameObject tile, int tileID)
	{
		ApplyTagToTile (tile, tileID);
		ApplyHeightToTile (tile, tileID);
	}

	void ApplyHeightToTile(GameObject tile, int tileID)
	{
		if (IsFloorTile (tileID)) {
			tile.transform.position = new Vector3 (tile.transform.position.x, -0.5f, tile.transform.position.z);
		} else {
			tile.transform.position = new Vector3 (tile.transform.position.x, 0.5f, tile.transform.position.z);
		}
	}

	void ApplyTagToTile(GameObject tile, int tileID)
	{
		tile.gameObject.tag = GetTagFromTileID (tileID);
	}

	string GetTagFromTileID(int tileID)
	{
		if (IsFloorTile (tileID)) {
			return FloorTileTag;
		} else {
			return WallTileTag;
		}
	}

	bool IsFloorTile(int tileID)
	{
		if (tileID == FloorTileID) {
			return true;
		} else {
			return false;
		}
	}

	bool IsFloorTile(GameObject tile)
	{
		if (tile.gameObject.tag == FloorTileTag) {
			return true;
		} else {
			return false;
		}
	}
	bool IsWallTile(GameObject tile)
	{
		if (tile.gameObject.tag == WallTileTag) {
			return true;
		} else {
			return false;
		}
	}

	#region - Tile configuration
	Color[] GetColorSelectionForTile(GameObject tile)
	{
		if (IsFloorTile (tile)) {
			return floorTileColors;
		} else {
			return wallTileColors;
		}
	}
						
	void RandomizeTileMaterialColor(GameObject tile, int row, int column)
	{
		Color[] colorSelection = GetColorSelectionForTile(tile);

		int colorIndex = 0;

		float randomChoice = Random.Range (0.0f, 1.0f);
		if (randomChoice < 0.15f) {
			colorIndex = 0;
		} else if (randomChoice > 0.15f && randomChoice < 0.30f) {
			colorIndex = 1;
		} else if (randomChoice >= 0.3f && randomChoice < 0.45f) {
			colorIndex = 2;
		} else if (randomChoice >= 0.45f) {
			colorIndex = 3;
		}
		MeshRenderer meshRenderer = tile.GetComponent<MeshRenderer> ();

		Material newMaterial = Instantiate (meshRenderer.material);
		newMaterial.color = colorSelection[colorIndex];

		meshRenderer.material = newMaterial;
	}

	void RandomizeTileHeight (GameObject tile, int row, int column)
	{
		float heightVariation = 0.0f;

		if (column != 0 && column != gameLevelWidthInTiles - 1) {
			heightVariation = Random.Range (-0.4f, 0.0f);
		}
		tile.transform.position = new Vector3 (tile.transform.position.x, tile.transform.position.y + heightVariation, tile.transform.position.z);
	}
	#endregion // Tile configuration
}
