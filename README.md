## About
This project is a high-performance, procedural terrain generation system developed in Unity.
It creates an infinite, seamless landscape using Perlin noise, featuring a chunk-based loading system and lod optimizations.
The core focus was on leveraging modern Unity features like the Job System to ensure smooth performance even with complex mesh generation.

## Technical Highlights
*   **Multithreaded Generation**: Utilizes the Unity Job System and `IJobFor` to offload heightmap calculation and mesh reconstruction from the main thread, maintaining high frame rates during terrain discovery.
*   **Asynchronous Workflow**: Built with a `Task`-based asynchronous architecture (`async/await`) to coordinate chunk spawning and LOD updates without blocking the game loop.
*   **Dynamic LOD System**: Implements a distance-based Level of Detail strategy. Chunks further from the player are generated with lower vertex density, significantly reducing GPU overhead while maintaining visual fidelity near the camera.
*   **Terrain Shader**: A custom terrain shader built with shader graph, assigning different PBR materials to the terrain depending on the terrain's height and slope.
*   **Infinite Scrolling**: A manager tracks player movement and dynamically shifts, recycles, or regenerates terrain chunks, creating a seamless "infinite world" experience.

## Architecture
*   **`HeightMapGenerator` & `MeshGenerator`**: Decoupled core logic for data generation, using `NativeArray` for efficient memory management and zero-copy data sharing with Jobs.
*   **`ProceduralTerrain`**: The central controller managing the chunk grid, player tracking, and orchestrating the lifecycle of terrain segments.
*   **`MeshJob` & `HeightMapJob`**: Burst-compiled jobs that handle the heavy lifting of generating the heightmap and mesh for each terrain chunk.