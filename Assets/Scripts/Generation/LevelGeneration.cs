using UnityEngine;
using System.Linq;

public class LevelGeneration : MonoBehaviour {

	[SerializeField]
	private int mapWidthInTiles, mapDepthInTiles;

	[SerializeField]
	private GameObject tilePrefab;

	void Start() {
		GenerateMap();
	}

	void GenerateMap() {
		// get the tile dimensions from the tile Prefab
		Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;
		int tileWidth = (int)tileSize.x;
		int tileDepth = (int)tileSize.z;

		TileGenerationSettings settings = new TileGenerationSettings();

		// for each Tile, instantiate a Tile in the correct position
		for (int xTileIndex = 0; xTileIndex < mapWidthInTiles; xTileIndex++) {
			for (int zTileIndex = 0; zTileIndex < mapDepthInTiles; zTileIndex++) {
				// calculate the tile position based on the X and Z indices
				Vector3 tilePosition = new Vector3(this.gameObject.transform.position.x + xTileIndex * tileWidth, 
					this.gameObject.transform.position.y, 
					this.gameObject.transform.position.z + zTileIndex * tileDepth);
				// instantiate a new Tile
				GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
				TileGeneration tileGeneration = tile.GetComponent<TileGeneration>();
				tileGeneration.heightMultiplier = settings.heightMultiplier;
				tileGeneration.levelScale = settings.levelScale;
				tileGeneration.heightCurve = settings.heightCurve;
				tileGeneration.waves = settings.waves;
			}
		}
	}

	public void RegenerateMap()
	{
		var objects = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "Test_Tile(Clone)");
		foreach(var tile in objects)
		{
			Destroy(tile);
		}

		GenerateMap();
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawSphere(transform.position, 12f);
	}
	
}

[System.Serializable]
public class TileGenerationSettings
{
	public float heightMultiplier;
	public float levelScale;
	public AnimationCurve heightCurve;
	public Wave[] waves;

	public TileGenerationSettings(int numWaves = 3)
	{
		this.heightMultiplier = Random.Range(1f, 3f);
		this.levelScale = Random.Range(5f, 30f);
		this.heightCurve = this.GetAnimationCurve();
		this.waves = this.GetWaves(numWaves);
	}

	private AnimationCurve GetAnimationCurve()
	{
		Keyframe[] keyframes = new Keyframe[5];

        for (var i = 0; i < keyframes.Length; i++)
        {
            keyframes[i] = new Keyframe(i, Mathf.Sin(i) * 2, 30, 30);
        }

		return new AnimationCurve(keyframes);
	}

	private Wave[] GetWaves(int numWaves)
	{
		Wave[] waves = new Wave[numWaves];
		for (int i = 0; i < waves.Length; i++)
		{
			Wave newWave = new Wave();
			newWave.seed = Random.Range(100, 99999);
			newWave.amplitude = Random.Range(0.3f, 1f);
			newWave.frequency = Random.Range(0.3f, 1f);
			waves[i] = newWave;
		}
		return waves;
	}
}
