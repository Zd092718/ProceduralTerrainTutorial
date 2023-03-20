using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]

public class CustomTerrain : MonoBehaviour
{

    #region Height Map Properties
    [SerializeField] private Vector2 randomHeightRange = new Vector2(0, 0.1f);
    [SerializeField] private Texture2D heightMapImage;
    [SerializeField] private Vector3 heightMapScale = new Vector3(1, 1, 1);

    #endregion

    #region Perlin Noise
    [SerializeField] private float perlinXScale = 0.01f;
    [SerializeField] private float perlinYScale = 0.01f;
    [SerializeField] private int perlinOffsetX = 0;
    [SerializeField] private int perlinOffsetY = 0;
    [SerializeField] private int perlinOctaves = 3;
    [SerializeField] private float perlinPersistence = 8;
    [SerializeField] private float perlinHeightScale = 0.09f;

    #endregion

    #region Multiple Perlin
    [System.Serializable]
    public class PerlinParameters
    {
        public float mPerlinXScale = 0.01f;
        public float mPerlinYScale = 0.01f;
        public int mPerlinOffsetX = 0;
        public int mPerlinOffsetY = 0;
        public int mPerlinOctaves = 3;
        public float mPerlinPersistence = 8;
        public float mPerlinHeightScale = 0.09f;
        public bool remove = false;
    }

    public List<PerlinParameters> perlinParameters = new List<PerlinParameters>()
    {
        new PerlinParameters()
    };

    #endregion

    [SerializeField] private Terrain terrain;
    [SerializeField] private TerrainData terrainData;
    [SerializeField] private bool resetTerrain;

    //public Vector2 RandomHeightRange { get => randomHeightRange; set => randomHeightRange = value; }
    //public Terrain Terrain { get => terrain; set => terrain = value; }
    //public TerrainData TerrainData { get => terrainData; set => terrainData = value; }

    private void OnEnable()
    {
        Debug.Log("Initializing Terrain Data");
        terrain = this.GetComponent<Terrain>();
        terrainData = terrain.terrainData;
    }


    float[,] GetHeightMap()
    {
        if (!resetTerrain)
        {
            return terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        }
        else
            return new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
    }

    public void Perlin()
    {
        int heightMapResolution = terrainData.heightmapResolution;
        float[,] heightMap = GetHeightMap();
        for (int y = 0; y < heightMapResolution; y++)
        {
            for (int x = 0; x < heightMapResolution; x++)
            {
                heightMap[x, y] += Utils.fBM((x + perlinOffsetX) * perlinXScale,
                                            (y + perlinOffsetY) * perlinYScale,
                                            perlinOctaves,
                                            perlinPersistence) * perlinHeightScale;
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    public void MultiplePerlin()
    {
        int heightMapResolution = terrainData.heightmapResolution;
        float[,] heightMap = GetHeightMap();
        for (int y = 0; y < heightMapResolution; y++)
        {
            for (int x = 0; x < heightMapResolution; x++)
            {
                foreach (PerlinParameters p in perlinParameters)
                {
                    heightMap[x, y] += Utils.fBM((x + p.mPerlinOffsetX) * p.mPerlinXScale,
                                                (y + p.mPerlinOffsetY) * p.mPerlinYScale,
                                                p.mPerlinOctaves,
                                                p.mPerlinPersistence) * p.mPerlinHeightScale;
                }
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    public void AddNewPerlin()
    {
        perlinParameters.Add(new PerlinParameters());
    }

    public void RemovePerlin()
    {
        perlinParameters.RemoveAll(p => p.remove);

        if (perlinParameters.Count == 0)
        {
            perlinParameters.Add(new PerlinParameters());
        }
    }


    public void RandomTerrain()
    {

        int heightMapResolution = terrainData.heightmapResolution;
        float[,] heightMap = GetHeightMap();
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

        int heightMapResolution = terrainData.heightmapResolution;
        float[,] heightMap = GetHeightMap();
        for (int x = 0; x < heightMapResolution; x++)
        {
            for (int z = 0; z < heightMapResolution; z++)
            {
                heightMap[x, z] += heightMapImage.GetPixel((int)(x * heightMapScale.x),
                                                          (int)(z * heightMapScale.z))
                                                          .grayscale * heightMapScale.y;
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    public void ResetTerrain()
    {
        int heightMapResolution = terrainData.heightmapResolution;
        float[,] heightMap = GetHeightMap();
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
