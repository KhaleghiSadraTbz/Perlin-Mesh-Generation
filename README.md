# 🌄 Procedural Terrain Generator (Unity + Perlin Noise)

This project generates fully procedural 3D terrains in Unity using **stacked Perlin noise profiles**.  
By combining multiple Perlin layers with different tiling, offsets, and amplitudes, you can create rich, varied landscapes — from rolling hills to jagged mountains.  

![Terrain Example](docs/screenshot.png)  
*(Example terrain generated with layered Perlin noise)*

🎥 **[Watch the Showcase Video](https://your-video-link-here.com)**

---

## ✨ Features
- 🌀 **Perlin Noise Profiles** – Combine multiple layers of Perlin noise with adjustable tiling, offset, and amplitude.  
- 🛠️ **Custom Mesh Generation** – Terrain is generated at runtime using Unity’s mesh system.  
- 🎛️ **Inspector Controls** – Intuitive UI to tweak noise layers, mesh size, subdivisions, and base height.  
- 🎨 **UV-Mapped Texturing** – Uses a UV grid texture for clear visualization.  
- ⚡ **Realtime Updates** – Regenerate terrain instantly with one click.

---

## 🖼️ Inspector Options
- **Size (X, Y)** – Controls overall width and depth of the terrain.  
- **Subdivisions (X, Y)** – Mesh density (higher values = smoother terrain).  
- **Height Adjustment** – Vertical offset for the base.  
- **Draw Base** – Option to generate a bottom surface for the mesh.  
- **Perlin Profiles** – List of Perlin layers you can stack:
  - `Tiling` (Vector2) – Controls frequency of noise.  
  - `Offset` (Vector2) – Shifts the noise pattern.  
  - `Amplitude` – Strength/height of the noise layer.  
