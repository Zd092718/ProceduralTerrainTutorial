using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]

public class CustomTerrain : MonoBehaviour
{
    //Height Map Properties ---------------------------------------------------
    [SerializeField] private Vector2 randomHeightRange = new Vector2(0, 0.1f);
    [SerializeField] private Texture2D heightMapImage;
    [SerializeField] private Vector3 heightMapScale = new Vector3(1, 1, 1);

    //PERLIN NOISE ----------------------------------------------------------
    [SerializeField] private float perlinXScale = 0.01f;
    [SerializeField] private float perlinYScale = 0.01f;
    [SerializeField] private int perlinOffsetX = 0;
    [SerializeField] private int perlinOffsetY = 0;
    [SerializeField] private int perlinOctaves = 3;
    [SerializeField] private float perlinPersistence = 8;
    [SerializeField] private float perlinHeightScale = 0.09f;



    [SerializeField] private Terrain terrain;
    [SerializeField] private TerrainData terrainData;

    //public Vector2 RandomHeightRange { get => randomHeightRange; set => randomHeightRange = value; }
    //public Terrain Terrain { get => terrain; set => terrain = value; }
    //public TerrainData TerrainData { get => terrainData; set => terrainData = value; }

    private void OnEnable()
    {
        Debug.Log("Initializing Terrain Data");
        terrain = this.GetComponent<Terrain>();
        terrainData = terrain.terrainData;
    }
    public void Perlin()
    {
        int heightMapResolution = terrainData.heightmapResolution;
        float[,] heightMap = terrainData.GetHeights(0, 0, heightMapResolution, heightMapResolution);
        for (int y = 0; y < heightMapResolution; y++)
        {
            for (int x = 0; x < heightMapResolution; x++)
            {
                heightMap[x, y] = Utils.fBM(x * perlinXScale,
                                            y * perlinYScale,
                                            perlinOctaves,
                                            perlinPersistence,
                                            perlinOffsetX,
                                            perlinOffsetY) * perlinHeightScale;
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    public void RandomTerrain()
    {
        float[,] heightMap;
        int heightMapResolution = terrainData.heightmapResolution;
        heightMap = terrainData.GetHeights(0, 0, heightMapResolution, heightMapResolution);
        for (int x = 0; x < heightMapResolution; x++)
        {
            for (int y = 0; y < heightMapResolution; y++)
            {
                heightMap[x, y] += Random.Range(randomHeightRange.x, randomHeightRange.y);
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }


    public void LoadTexture()
    {
        float[,] heightMap;
        int heightMapResolution = terrainData.heightmapResolution;
        heightMap = terrainData.GetHeights(0, 0, heightMapResolution, heightMapResolution);
        for (int x = 0; x < heightMapResolution; x++)
        {
            for (int z = 0; z < heightMapResolution; z++)
            {
                heightMap[x, z] = heightMapImage.GetPixel((int)(x * heightMapScale.x),
                                                          (int)(z * heightMapScale.z))
                                                          .grayscale * heightMapScale.y;
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    public void ResetTerrain()
    {
        float[,] heightMap;
        int heightMapResolution = terrainData.heightmapResolution;
        heightMap = terrainData.GetHeights(0, 0, heightMapResolution, heightMapResolution);
        for (int x = 0; x < heightMapResolution; x++)
        {
            for (int y = 0; y < heightMapResolution; y++)
            {
                heightMap[x, y] = 0;
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }
    private void Awake()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        AddTag(tagsProp, "Terrain");
        AddTag(tagsProp, "Cloud");
        AddTag(tagsProp, "Shore");

        //apply tag changes to tag database
        tagManager.ApplyModifiedProperties();

        //take this object
        this.gameObject.tag = "Terrain";
    }

    private void AddTag(SerializedProperty tagsProp, string newTag)
    {
        bool found = false;
        //ensure the tag isn't already found
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(newTag)) { found = true; break; }
        }
        //add new tag
        if (!found)
        {
            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty newTagProp = tagsProp.GetArrayElementAtIndex(0);
            newTagProp.stringValue = newTag;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
