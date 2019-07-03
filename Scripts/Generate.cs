using System.Diagnostics;
using UnityEngine;
using QFSW.QC;

public class Generate : MonoBehaviour
{
    public int Seed;
    public int TreeSeed;

    public int MovementMultiplier;

    public int TerrainSize;
    public float PointDensity;
    //public Vector2Int TerrainChunkPosition;
    public Vector2Int Offset;
    public float BaseScale;
    public float HeightScale;
    public AnimationCurve HeightCurve;
    public float MaxHeight;
    public float Multiplier;
    public float RoughnessFactor;
    public float RoughnessStrength;
    public bool Terrace;
    public int TerraceAmount;
    public int TreeDensity; 
    public float MinTreeSpawnHeight;
    public GameObject TreeGO;

    private Vector3[] vertices;
    private int[] triangles;
    private Mesh mesh;

    private Stopwatch timer;

    private bool autoRegen;

    private void Awake () {
        timer = new Stopwatch();
		GenerateMesh();
	}

	public void GenerateMesh () {
        //timer.Start();
        if(mesh != null)
            mesh.Clear();
        
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        int size = Mathf.FloorToInt( (float)TerrainSize / PointDensity );
		mesh.name = "Terrain";

		vertices = new Vector3[(size + 1) * (size + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
		for (int i = 0, y = 0; y <= size; y++) {
			for (int x = 0; x <= size; x++, i++) {
                float noiseMap = GetTerrainNoise(x,y);

                vertices[i] = new Vector3(x*PointDensity, noiseMap, y*PointDensity);
                uv[i] = new Vector2(Random.Range(0f,1f), noiseMap/MaxHeight);

                /*
                float max = 0;
                if (noiseMap >= MinTreeSpawnHeight) {
                    for (int yn = y - TreeDensity; yn <= y + TreeDensity; yn++ ) {
                        for (int xn = x - TreeDensity; xn <= x + TreeDensity; xn++) {
                            float e = PerlinNoise(xn,0,yn,10,10,25,1,TreeSeed);
                            if (e > max) max = e;
                        }
                    }
                    if (PerlinNoise(x,0,y,10,10,25,1,TreeSeed) == max) {
                        GenerateTree(x*PointDensity, noiseMap, y*PointDensity);
                    }
                }
                */
			}
		}
		mesh.vertices = vertices;
        mesh.uv = uv;

		triangles = new int[size * size * 6];
		for (int ti = 0, vi = 0, y = 0; y < size; y++, vi++) {
			for (int x = 0; x < size; x++, ti += 6, vi++) {
				triangles[ti] = vi;
				triangles[ti + 3] = triangles[ti + 2] = vi + 1;
				triangles[ti + 4] = triangles[ti + 1] = vi + size + 1;
				triangles[ti + 5] = vi + size + 2;
			}
		}
		mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        //timer.Stop();
        //UnityEngine.Debug.Log("Generated in " + timer.ElapsedMilliseconds + "ms");
        //timer.Reset();
	}

    [Command("changeseed")]
    void ChangeSeed (int seed) {
        Seed = seed;
    }

    public void DeleteChunk () {
        mesh.Clear();
        Destroy(mesh);
        Destroy(gameObject);
    }

    private void GenerateTree (float x, float y, float z) {
        GameObject tree = Instantiate(TreeGO, new Vector3(x,y,z), Quaternion.identity);
    }

    private float GetTerrainNoise (int x, int y) {
        float noiseMap = PerlinNoise(x + Offset.x, 0, y + Offset.y, (BaseScale/PointDensity), HeightScale, MaxHeight, Multiplier, Seed)
                                    + (PerlinNoise(x + Offset.x, 0, y + Offset.y, (BaseScale/PointDensity)/RoughnessFactor, HeightScale, MaxHeight, Multiplier, Seed) * (1f/RoughnessFactor)
                                    + PerlinNoise(x + Offset.x, 0, y + Offset.y, (BaseScale/PointDensity)/(RoughnessFactor*RoughnessFactor), HeightScale, MaxHeight, Multiplier, Seed) * (1f/(RoughnessFactor*RoughnessFactor)) * RoughnessStrength);
        
        noiseMap /= HeightCurve.Evaluate(noiseMap/MaxHeight);
        if (Terrace) noiseMap = Mathf.Round(noiseMap*TerraceAmount)/TerraceAmount;

        return noiseMap;
    }

    static float PerlinNoise(int x, int y, int z, float scale, float yScale, float height, float power, int seed) {
        float rVal = Noise.Noise.GetNoise(((double)x+seed)/scale, ((double)y+seed)/yScale, ((double)z+seed)/scale);
        rVal *= height;

        if (System.Math.Abs(power) > 0) {
            rVal = Mathf.Pow(rVal, power);
        }
        return rVal;
    }
}
