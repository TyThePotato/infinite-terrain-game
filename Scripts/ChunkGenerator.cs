using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    public Vector2Int CurrentChunk;
    public int ChunkSize = 64;
    public int ChunkRenderRadius = 4;

    private List<Generate> loadedChunks;

    public GameObject chunkGO;

    private void Awake() {
        loadedChunks = new List<Generate>();
        LoadChunks();
    }

    private void Update() {
        Vector2Int currentChunk = new Vector2Int(Mathf.FloorToInt(transform.position.x / ChunkSize), Mathf.FloorToInt(transform.position.z / ChunkSize));

        // If moved to another chunk
        if(CurrentChunk != currentChunk) {
            CurrentChunk = currentChunk;
            LoadChunks();
            UnloadFarChunks();
        }
    }

    private void LoadChunks () {
        for (int x = CurrentChunk.x - ChunkRenderRadius; x < CurrentChunk.x + ChunkRenderRadius; x++) {
            for (int y = CurrentChunk.y - ChunkRenderRadius; y < CurrentChunk.y + ChunkRenderRadius; y++) {
                GenerateChunk(x,y);
            } 
        }
    }

    private void UnloadFarChunks () {
        for(int i = 0; i < loadedChunks.Count; i++) {
            bool shouldChunkBeLoaded = false;
            for (int x = CurrentChunk.x - ChunkRenderRadius; x < CurrentChunk.x + ChunkRenderRadius; x++) {
                for (int y = CurrentChunk.y - ChunkRenderRadius; y < CurrentChunk.y + ChunkRenderRadius; y++) {
                    if(loadedChunks[i].Offset == new Vector2Int(x*ChunkSize, y*ChunkSize)) shouldChunkBeLoaded = true;
                } 
            }
            if (!shouldChunkBeLoaded) {
                loadedChunks[i].DeleteChunk();
                loadedChunks.Remove(loadedChunks[i]);
            }
        }
    }

    private void GenerateChunk (int chunkX, int chunkY) {
        for (int i = 0; i < loadedChunks.Count; i++) {
            if (loadedChunks[i].GetComponent<Generate>().Offset == new Vector2Int(chunkX*ChunkSize, chunkY*ChunkSize)) return;
        }

        GameObject _chunk = Instantiate(chunkGO, new Vector3(chunkX*ChunkSize, 0, chunkY*ChunkSize), Quaternion.identity);
        Generate chunkGenerator = _chunk.GetComponent<Generate>();
        chunkGenerator.Offset = new Vector2Int(chunkX*ChunkSize, chunkY*ChunkSize);
        chunkGenerator.GenerateMesh();

        loadedChunks.Add(chunkGenerator);
    }
}
