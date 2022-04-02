using System;
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
	[Min(0)]
	public float checkInterval;
	[Min(1)]
	public float checkSphereRadius;
	public LayerMask layerMask;

	[Header("Privates")]
	[GreyOut] public int mapWidthInTiles, mapDepthInTiles;
	[GreyOut] public TileGenerationSettings settings;
	[GreyOut] public int tileDepth;
	[GreyOut] public float timeCounter;

	void Update()
	{
		if (!generatingCanvas.activeSelf)
		{
			if (timeCounter >= checkInterval)
			{
				deleteNotVisibleTiles();
				generateNewMap();
				timeCounter = 0f;
			}

			timeCounter += Time.deltaTime;
		}
	}

	void Start()
	{
		StartCoroutine(nameof(GenerateMap));
	}

	IEnumerator GenerateMap()
	{
		generatingCanvas.SetActive(true);
		Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;
		int tileWidth = (int)tileSize.x;
		tileDepth = (int)tileSize.z;
		mapDepthInTiles = worldSize;
		mapWidthInTiles = worldSize;

		settings = new TileGenerationSettings();
		Vector3 startingPosition = new Vector3(viewer.transform.position.x - worldSize * tileWidth / 2, transform.position.y, viewer.transform.position.z - worldSize * tileWidth / 2);
		int count = 0;
		for (int xTileIndex = 0; xTileIndex < mapWidthInTiles; xTileIndex++) {
			for (int zTileIndex = 0; zTileIndex < mapDepthInTiles; zTileIndex++) {
				Vector3 tilePosition = new Vector3(startingPosition.x + xTileIndex * tileWidth, transform.position.y, startingPosition.z + zTileIndex * tileDepth);
				float distance = Vector2.Distance(new Vector2(viewer.transform.position.x, viewer.transform.position.z), new Vector2(tilePosition.x, tilePosition.z)) / tileWidth;
				if (distance <= viewDistance)
				{
					createNewTile(tilePosition);
				}
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
		Gizmos.DrawWireSphere(new Vector3(viewer.transform.position.x, 0f, viewer.transform.position.z), viewDistance * tileWidth);
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

	private void createNewTile(Vector3 position)
	{
		if(Physics.CheckSphere(position, checkSphereRadius, layerMask))
		{
			return;
		}
		GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity) as GameObject;
		TileGeneration tileGeneration = tile.GetComponent<TileGeneration>();
		tileGeneration.heightMultiplier = settings.heightMultiplier;
		tileGeneration.levelScale = settings.levelScale;
		tileGeneration.heightCurve = settings.heightCurve;
		tileGeneration.waves = settings.waves;
	}

	private void deleteNotVisibleTiles()
	{
		var objects = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "Test_Tile(Clone)");
		foreach(var tile in objects)
		{
			Vector2 viewerPosition = new Vector2(viewer.transform.position.x, viewer.transform.position.z);
			Vector2 tilePosition = new Vector2(tile.transform.position.x, tile.transform.position.z);
			float distance = Vector2.Distance(viewerPosition, tilePosition) / tileDepth;
			if (distance >= viewDistance)
			{
				Destroy(tile);
			}
		}
	}

	private Vector3 getCurrentTile()
	{
		RaycastHit tileHit;
		if (Physics.Raycast(viewer.transform.position, transform.TransformDirection(Vector3.down), out tileHit, Mathf.Infinity, layerMask))
		{
			return tileHit.transform.position;
		}
		else
		{
			return new Vector3();
		}
	}

	private void generateNewMap()
	{
		Vector3 currentTile = getCurrentTile();
		int smallWorldSize = viewDistance - 1;
		Vector3 startingPosition = new Vector3(currentTile.x - smallWorldSize * tileDepth / 2, transform.position.y, currentTile.z - smallWorldSize * tileDepth / 2);
		var objects = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "Test_Tile(Clone)");
		for (int xTileIndex = 0; xTileIndex < smallWorldSize; xTileIndex++) {
			for (int zTileIndex = 0; zTileIndex < smallWorldSize; zTileIndex++) {
				Vector3 tilePosition = new Vector3(startingPosition.x + xTileIndex * tileDepth, transform.position.y, startingPosition.z + zTileIndex * tileDepth);
				float distance = Vector2.Distance(new Vector2(viewer.transform.position.x, viewer.transform.position.z), new Vector2(tilePosition.x, tilePosition.z)) / tileDepth;
				if (distance <= viewDistance)
				{
					createNewTile(tilePosition);
				}
			}
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
		this.heightMultiplier = UnityEngine.Random.Range(1f, 3f);
		this.levelScale = UnityEngine.Random.Range(5f, 30f);
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
			newWave.seed = UnityEngine.Random.Range(100, 99999);
			newWave.amplitude = UnityEngine.Random.Range(0.3f, 1f);
			newWave.frequency = UnityEngine.Random.Range(0.3f, 1f);
			waves[i] = newWave;
		}
		return waves;
	}
}
