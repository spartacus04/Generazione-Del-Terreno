using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColorMap, Mesh}
    public DrawMode drawMode;

    const int mapChunkSize = 241;
    [Range(0, 6)]
    public int LevelOfDetail;
    public float noiseScale;

    public int Octaves;
    [Range(0, 1)] public float Persistance;
    public float Lacunarity;

    public int Seed;
    public Vector2 Offset;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public TerrainType[] Regions;

    public bool autoUpdate;

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, noiseScale, Seed,Octaves, Persistance, Lacunarity, Offset);

        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < Regions.Length; i++)
                {
                    if(currentHeight <= Regions[i].height)
                    {
                        colorMap[y * mapChunkSize + x] = Regions[i].color;
                        break;
                    }
                }
            }
        }

        

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if(drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if(drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, LevelOfDetail), TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
        }
    }


    public void OnValidate()
    {
        if(Lacunarity < 1)
        {
            Lacunarity = 1;
        }
        if(Octaves < 0)
        {
            Octaves = 0;
        }
    }

    [Serializable]
    public struct TerrainType
    {
        public string Name;
        public float height;
        public Color color;
    }
}
