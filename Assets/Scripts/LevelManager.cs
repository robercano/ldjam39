using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class LevelManager : MonoBehaviour {
	public GameObject RedPlayerPrefab;
	public GameObject BluePlayerPrefab;
	public GameObject RedBasePrefab;
	public GameObject BlueBasePrefab;
	private string RedPlayerGameTag = "Red";
	private string BluePlayerGameTag = "Blue";
	private string RedPlayerName = "RedPlayer";
	private string BluePlayerName = "BluePlayer";

	private int gameLevelWidthInTiles;
	private int gameLevelHeightInTiles;

	Dictionary<string, GameObject> tilesPrefabsMap;

	string[] level1Config = {
		"W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W",
		"W   F   F   F   F   W   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   W",
		"W   F   RB  F   F   W   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   W",
		"W   F   F   F   F   W   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   W",
		"W   F   F   F   F   W   F   F   F   F   F   F   F   F   F   F   F   W   F   F   F   W   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   W",
		"W   F   F   F   F   W   F   F   F   F   F   F   F   F   F   F   F   W   F   B   F   W   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   W",
		"W   F   F   F   F   W   F   F   F   F   F   F   F   F   F   F   F   W   F   F   F   W   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   W",
		"W   F   F   F   F   W   F   F   F   F   F   F   F   F   F   F   F   W   F   F   F   W   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   W",
		"W   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   W   F   F   F   W   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   W",
		"W   F   RP  F   F   F   F   F   F   F   F   F   F   F   F   F   F   W   F   F   F   W   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   W",
		"W   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   W   F   B   F   W   F   F   F   F   F   F   F   F   F   F   F   F   F   F   BP  F   W",
		"W   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   W   F   F   F   W   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   W",
		"W   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   W   F   F   F   W   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   W",
		"W   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   W   F   F   F   W   F   F   F   F   F   F   F   F   F   F   F   W   F   F   F   F   W",
		"W   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   W   F   F   F   W   F   F   F   F   F   F   F   F   F   F   F   W   F   F   F   F   W",
		"W   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   W   F   B   F   W   F   F   F   F   F   F   F   F   F   F   F   W   F   F   F   F   W",
		"W   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   W   F   F   F   W   F   F   F   F   F   F   F   F   F   F   F   W   F   F   F   F   W",
		"W   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   W   F   BB  F   F   W",
		"W   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   W   F   F   F   F   W",
		"W   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   F   W   F   F   F   F   W",
		"W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W   W"
	};

	private int currentLevel = 0;

	public const float AboveFloorHeight = 0.5f;
	public const float FloorHeight = -0.5f;

	public const string ResourcesTilesFolder = "StageTiles";

    const string FloorTilePrefabName = "FloorTile";
    const string FloorTileConfigTag = "F";
	public const string FloorTileGameTag = "Floor";

	const string WallTilePrefabName = "WallTile";
	const string WallTileConfigTag = "W";
	public const string WallTileGameTag = "Wall";

	const string BatterySpawnTilePrefabName = "BatterySpawnTile";
	const string BatterySpawnTileConfigTag = "B";
	public const string BatterySpawnTileGameTag = "BatterySpawn";

	const string RedPlayerSpawnTilePrefabName = "RedPlayerSpawnTile";
	const string RedPlayerSpawnTileConfigTag = "RP";
	public const string RedPlayerSpawnTileGameTag = "RedPlayerSpawn";

	const string BluePlayerSpawnTilePrefabName = "BluePlayerSpawnTile";
	const string BluePlayerSpawnTileConfigTag = "BP";
	public const string BluePlayerSpawnTileGameTag = "BluePlayerSpawn";

	const string RedBaseSpawnTilePrefabName = "RedBaseSpawnTile";
	const string RedBaseSpawnTileConfigTag = "RB";
	public const string RedBaseSpawnTileGameTag = "RedBaseSpawn";

	const string BlueBaseSpawnTilePrefabName = "BlueBaseSpawnTile";
	const string BlueBaseSpawnTileConfigTag = "BB";
	public const string BlueBaseSpawnTileGameTag = "BlueBaseSpawn";

	const string TilesParent = "Tiles";
	GameObject TilesParentGO;
	const string LevelParent = "Level";
	GameObject LevelParentGO;

	private Color[] floorTileColors;
	private Color[] wallTileColors;

	public void LoadNextLevel()
	{
		currentLevel++;

		LoadLevel(currentLevel);
	}

	void Awake()
	{
		LoadTilesMapInfo ();
		LoadTilesColorsVariations ();

		TilesParentGO = GameObject.Find (TilesParent);
		LevelParentGO = GameObject.Find (LevelParent);
	}

	void Start () {

	}

	#region - Configuration setup methods
	void SpawnPlayer(GameObject playerPrefab, string playerName, string playerTag, string spawnTileGameTag)
	{
		GameObject spawnTile = GameObject.FindGameObjectWithTag (spawnTileGameTag);

		Vector3 playerPosition = spawnTile.transform.position;
		playerPosition.y = 1.0f;

		GameObject player = Instantiate (playerPrefab, LevelParentGO.transform);

		player.transform.position =	playerPosition;

		player.gameObject.tag = playerTag;
		player.gameObject.name = playerName;
	}

	void SpawnBase(GameObject basePrefab, string baseTag, string spawnTileGameTag)
	{
		GameObject spawnTile = GameObject.FindGameObjectWithTag (spawnTileGameTag);

		Vector3 basePosition = spawnTile.transform.position;
	
		basePosition.x += 0.5f;
		basePosition.y = AboveFloorHeight;
		basePosition.z -= 0.5f;

		GameObject baseObject = Instantiate (basePrefab, LevelParentGO.transform);

		baseObject.transform.position =	basePosition;

		baseObject.gameObject.tag = baseTag;
	}

	void LoadTilesMapInfo()
	{
		tilesPrefabsMap = new Dictionary<string, GameObject>();

		string tileResourcePath = ResourcesTilesFolder + "/";

		tilesPrefabsMap[FloorTileConfigTag] = Resources.Load(tileResourcePath + FloorTilePrefabName) as GameObject;
		tilesPrefabsMap[WallTileConfigTag] = Resources.Load(tileResourcePath + WallTilePrefabName) as GameObject;
		tilesPrefabsMap[BatterySpawnTileConfigTag] = Resources.Load(tileResourcePath + BatterySpawnTilePrefabName) as GameObject;
		tilesPrefabsMap[RedPlayerSpawnTileConfigTag] = Resources.Load(tileResourcePath + RedPlayerSpawnTilePrefabName) as GameObject;
		tilesPrefabsMap[BluePlayerSpawnTileConfigTag] = Resources.Load(tileResourcePath + BluePlayerSpawnTilePrefabName) as GameObject;
		tilesPrefabsMap[RedBaseSpawnTileConfigTag] = Resources.Load(tileResourcePath + RedBaseSpawnTilePrefabName) as GameObject;
		tilesPrefabsMap[BlueBaseSpawnTileConfigTag] = Resources.Load(tileResourcePath + BlueBaseSpawnTilePrefabName) as GameObject;
	}

	void LoadTilesColorsVariations()
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
	}

	#endregion // Configuration setup methods

	void LoadLevel(int level)
	{
		string[,] levelConfig;

		LoadLevelConfig (level, out levelConfig);
		GenerateLevelFromConfig (levelConfig);

		SpawnPlayer (RedPlayerPrefab, RedPlayerName, RedPlayerGameTag, RedPlayerSpawnTileGameTag);
		SpawnPlayer (BluePlayerPrefab, BluePlayerName, BluePlayerGameTag, BluePlayerSpawnTileGameTag);

		SpawnBase (RedBasePrefab, RedPlayerGameTag, RedBaseSpawnTileGameTag);
		SpawnBase (BlueBasePrefab, BluePlayerGameTag, BlueBaseSpawnTileGameTag);
	}

	GameObject GetTilePrefabByConfigTag(string configTag)
	{
		return tilesPrefabsMap [configTag];
	}

	void LoadLevelConfig(int level, out string[,] levelOutConfig)
	{
		if (level == 1) {
			DoLoadLevelConfig (level1Config, out levelOutConfig);
		} else {
			// TODO: default level
			DoLoadLevelConfig (level1Config, out levelOutConfig);
		}
	}

	void DoLoadLevelConfig(string[] levelInputConfig, out string[,] levelConfig)
	{
		string configLine = Regex.Replace (levelInputConfig[0], @"\s+", " ");
		var configCheck = configLine.Split ();

		gameLevelWidthInTiles = configCheck.Length;
		gameLevelHeightInTiles= levelInputConfig.Length;

		levelConfig = new string[gameLevelWidthInTiles, gameLevelHeightInTiles];

		for (int row=0; row<gameLevelHeightInTiles; ++row) {
			configLine = Regex.Replace (levelInputConfig[gameLevelHeightInTiles - row - 1], @"\s+", " ");
			var splitLine = configLine.Split ();

			for (int column = 0; column < gameLevelWidthInTiles; ++column) {
				levelConfig [column, row] = splitLine [column];
			}
		}
	}

	void GenerateLevelFromConfig(string[,] levelConfig)
	{
		for (int column = 0; column < levelConfig.GetLength (0); ++column) {
			for (int row=0; row<levelConfig.GetLength(1); ++row) {
				string tileConfigTag = levelConfig [column, row];

				GameObject tile = InstantiateTile (tileConfigTag, row, column);

				ApplyBaseConfigToTile (tile, tileConfigTag);

				RandomizeTileMaterialColor (tile, row, column);

				if (IsWallTile (tile)) {
					RandomizeTileHeight (tile, row, column);
				}

			}
		}
	}

	GameObject InstantiateTile(string tileConfigTag, int row, int column)
	{
		GameObject tilePrefab = GetTilePrefabByConfigTag (tileConfigTag);

		GameObject tile = Instantiate (tilePrefab, TilesParentGO.transform);

		ApplyTagToTile (tile, tileConfigTag);

		tile.transform.position = new Vector3 (column - (gameLevelWidthInTiles - 1.0f)/2.0f, 0.0f, row - (gameLevelHeightInTiles - 1.0f)/2.0f);;
	
		return tile;
	}

	void ApplyBaseConfigToTile(GameObject tile, string tileConfigTag)
	{
		ApplyHeightToTile (tile);
	}

	void ApplyHeightToTile(GameObject tile)
	{
		float height = 0.0f;

		if (IsWallTile (tile)) {
			height = AboveFloorHeight;
		} else {
			height = FloorHeight;
		}

		tile.transform.position = new Vector3 (tile.transform.position.x, height, tile.transform.position.z);
	}

	void ApplyTagToTile(GameObject tile, string tileConfigTag)
	{
		tile.gameObject.tag = GetGameTagFromTileConfigTag (tileConfigTag);
	}

	string GetGameTagFromTileConfigTag(string tileConfigTag)
	{
		switch (tileConfigTag) {
		case FloorTileConfigTag:
			return FloorTileGameTag;
		case WallTileConfigTag:
			return WallTileGameTag;
		case BatterySpawnTileConfigTag:
			return BatterySpawnTileGameTag;
		case RedPlayerSpawnTileConfigTag:
			return RedPlayerSpawnTileGameTag;
		case BluePlayerSpawnTileConfigTag:
			return BluePlayerSpawnTileGameTag;
		case RedBaseSpawnTileConfigTag:
			return RedBaseSpawnTileGameTag;
		case BlueBaseSpawnTileConfigTag:
			return BlueBaseSpawnTileGameTag;
		default:
			return "";
		}
	}
	string GetLayerFromTileConfigTag(string tileConfigTag)
	{
		switch (tileConfigTag) {
		case FloorTileConfigTag:
			return FloorTileGameTag;
		case WallTileConfigTag:
			return WallTileGameTag;
		case BatterySpawnTileConfigTag:
			return BatterySpawnTileGameTag;
		case RedPlayerSpawnTileConfigTag:
			return RedPlayerSpawnTileGameTag;
		case BluePlayerSpawnTileConfigTag:
			return BluePlayerSpawnTileGameTag;
		case RedBaseSpawnTileConfigTag:
			return RedBaseSpawnTileGameTag;
		case BlueBaseSpawnTileConfigTag:
			return BlueBaseSpawnTileGameTag;
		default:
			return "";
		}
	}

	bool IsFloorTile(GameObject tile)
	{
		if (tile.gameObject.tag == GetGameTagFromTileConfigTag(FloorTileConfigTag)) {
			return true;
		} else {
			return false;
		}
	}
	bool IsWallTile(GameObject tile)
	{
		if (tile.gameObject.tag == GetGameTagFromTileConfigTag(WallTileConfigTag)) {
			return true;
		} else {
			return false;
		}
	}
	bool IsBatterySpawnTile(GameObject tile)
	{
		if (tile.gameObject.tag == GetGameTagFromTileConfigTag(BatterySpawnTileConfigTag)) {
			return true;
		} else {
			return false;
		}
	}
	bool IsRedPlayerSpawnTile(GameObject tile)
	{
		if (tile.gameObject.tag == GetGameTagFromTileConfigTag(RedPlayerSpawnTileConfigTag)) {
			return true;
		} else {
			return false;
		}
	}
	bool IsBluePlayerSpawnTile(GameObject tile)
	{
		if (tile.gameObject.tag == GetGameTagFromTileConfigTag(BluePlayerSpawnTileConfigTag)) {
			return true;
		} else {
			return false;
		}
	}
	bool IsRedBaseSpawnTile(GameObject tile)
	{
		if (tile.gameObject.tag == GetGameTagFromTileConfigTag(RedBaseSpawnTileConfigTag)) {
			return true;
		} else {
			return false;
		}
	}
	bool IsBlueBaseSpawnTile(GameObject tile)
	{
		if (tile.gameObject.tag == GetGameTagFromTileConfigTag(BlueBaseSpawnTileConfigTag)) {
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
		if (IsFloorTile (tile) == false &&
		    IsWallTile (tile) == false) {
			return;
		}
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
