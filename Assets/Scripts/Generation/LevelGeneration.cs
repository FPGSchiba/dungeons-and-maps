using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

public class LevelGeneration : MonoBehaviour {

	[Header("References")]
	[SerializeField]
	private GameObject tilePrefab;
	public GameObject viewer;
	public GameObject generatingCanvas;
	public Slider generationProgress;

	[Header("Settings")]
	[Min(0)]
	public int viewDistance;
	[Min(0)]
	public int worldSize;
	[Header("Privates")]
	[GreyOut] public int mapWidthInTiles, mapDepthInTiles;

	void Start()
	{
		StartCoroutine(nameof(GenerateMap));
	}

	IEnumerator GenerateMap()
	{
		generatingCanvas.SetActive(true);
		// get the tile dimensions from the tile Prefab
		Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;
		int tileWidth = (int)tileSize.x;
		int tileDepth = (int)tileSize.z;
		mapDepthInTiles = worldSize;
		mapWidthInTiles = worldSize;

		TileGenerationSettings settings = new TileGenerationSettings();
		Vector3 startingPosition = new Vector3(viewer.transform.position.x - worldSize * tileWidth / 2, transform.position.y, viewer.transform.position.z - worldSize * tileWidth / 2);
		int count = 0;
		// for each Tile, instantiate a Tile in the correct position
		for (int xTileIndex = 0; xTileIndex < mapWidthInTiles; xTileIndex++) {
			for (int zTileIndex = 0; zTileIndex < mapDepthInTiles; zTileIndex++) {
				// calculate the tile position based on the X and Z indices
				Vector3 tilePosition = new Vector3(startingPosition.x + xTileIndex * tileWidth, transform.position.y, startingPosition.z + zTileIndex * tileDepth);
				// instantiate a new Tile
				GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
				TileGeneration tileGeneration = tile.GetComponent<TileGeneration>();
				tileGeneration.heightMultiplier = settings.heightMultiplier;
				tileGeneration.levelScale = settings.levelScale;
				tileGeneration.heightCurve = settings.heightCurve;
				tileGeneration.waves = settings.waves;
				count++;
				generationProgress.value = (float)count * 100f / (float)(worldSize*worldSize);
				yield return null;
			}
		}

		generatingCanvas.SetActive(false);
		yield return null;
	}

	public void RegenerateMap()
	{
		var objects = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "Test_Tile(Clone)");
		foreach(var tile in objects)
		{
			Destroy(tile);
		}
		StartCoroutine(nameof(GenerateMap));
	}
	
	void OnDrawGizmos()
	{
		Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;
		int tileWidth = (int)tileSize.x;
		Gizmos.DrawWireSphere(viewer.transform.position, viewDistance * tileWidth);
		Gizmos.color = Color.black;
		Vector3 startingPosition = new Vector3(viewer.transform.position.x - worldSize * tileWidth / 2, transform.position.y, viewer.transform.position.z - worldSize * tileWidth / 2);
		Vector3 currentPosition = startingPosition;
		for(int i = 0; i <= worldSize; i++)
		{
			Vector3 opposite = currentPosition + Vector3.right * worldSize * tileWidth;
			Gizmos.DrawLine(currentPosition, opposite);
			currentPosition += Vector3.forward * tileWidth;
		}
		currentPosition -= Vector3.forward * tileWidth;
		for(int i = 0; i <= worldSize; i++)
		{
			Vector3 opposite = currentPosition + Vector3.back * worldSize * tileWidth;
			Gizmos.DrawLine(currentPosition, opposite);
			currentPosition += Vector3.right * tileWidth;
		}
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
