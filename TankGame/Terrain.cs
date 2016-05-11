using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TankGame
{
    class Terrain
    {
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;
        private GraphicsDevice device;
        private Texture2D terrainTexture;
        private float textureScale;
        private float[,] heights;

        public Terrain(GraphicsDevice graphicsDevice,Texture2D heightMap,Texture2D terrainTexture, 
                       float textureScale,int terrainWidth,int terrainHeight,float heightScale)
        {
            device = graphicsDevice;
            this.terrainTexture = terrainTexture;
            this.textureScale = textureScale;

            ReadHeightMap(heightMap,terrainWidth,terrainHeight,heightScale);

            BuildVertexBuffer(terrainWidth,terrainHeight,heightScale);

            BuildIndexBuffer(terrainWidth,terrainHeight);
        }

        public float GetHeight(float x, float z)
        {
            int xmin = (int)Math.Floor(x);
            int xmax = xmin + 1;
            int zmin = (int)Math.Floor(z);
            int zmax = zmin + 1;
            if (
            (xmin < 0) || (zmin < 0) ||
            (xmax > heights.GetUpperBound(0)) ||
            (zmax > heights.GetUpperBound(1)))
            {
                return 0;
            }
            Vector3 p1 = new Vector3(xmin, heights[xmin, zmax], zmax);
            Vector3 p2 = new Vector3(xmax, heights[xmax, zmin], zmin);
            Vector3 p3;
            if ((x - xmin) + (z - zmin) <= 1)
            {
                p3 = new Vector3(xmin, heights[xmin, zmin], zmin);
            }
            else
            {
                p3 = new Vector3(xmax, heights[xmax, zmax], zmax);
            }
            Plane plane = new Plane(p1, p2, p3);
            Ray ray = new Ray(new Vector3(x, 0, z), Vector3.Up);
            float? height = ray.Intersects(plane);
            return height.HasValue ? height.Value : 0f;
        }

        private void ReadHeightMap(Texture2D heightMap,int terrainWidth,int terrainHeight,float heightScale)
        {
            float min = float.MaxValue;
            float max = float.MinValue;
            heights = new float[terrainWidth, terrainHeight];
            Color[] heightMapData = new Color[heightMap.Width * heightMap.Height];
            heightMap.GetData(heightMapData);
            for (int x = 0; x < terrainWidth; x++)
                for (int z = 0; z < terrainHeight; z++)
                {
                    byte height = heightMapData[x + z * terrainWidth].R;
                    heights[x, z] = (float)height / 255f;
                    max = MathHelper.Max(max, heights[x, z]);
                    min = MathHelper.Min(min, heights[x, z]);
                }
            float range = (max - min);
            for (int x = 0; x < terrainWidth; x++)
                for (int z = 0; z < terrainHeight; z++)
                {
                    heights[x, z] =
                    ((heights[x, z] - min) / range) * heightScale;
                }
        }

        private void BuildVertexBuffer(int width, int height, float heightScale)
        {
            VertexPositionNormalTexture[] vertices =
            new VertexPositionNormalTexture[width * height];
            for (int x = 0; x < width; x++)
                for (int z = 0; z < height; z++)
                {
                    vertices[x + (z * width)].Position =
                    new Vector3(x, heights[x, z], z);
                    vertices[x + (z * width)].TextureCoordinate =
new Vector2((float)x / textureScale, (float)z / textureScale);
                }
            vertexBuffer = new VertexBuffer(device,typeof(VertexPositionNormalTexture),vertices.Length,BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices);
        }

        private void BuildIndexBuffer(int width, int height)
        {
            int indexCount = (width - 1) * (height - 1) * 6;
            short[] indices = new short[indexCount];
            int counter = 0;
            for (short z = 0; z < height - 1; z++)
                for (short x = 0; x < height - 1; x++)
                {
                    short upperLeft = (short)(x + (z * width));
                    short upperRight = (short)(upperLeft + 1);
                    short lowerLeft = (short)(upperLeft + width);
                    short lowerRight = (short)(upperLeft + width + 1);
                    indices[counter++] = upperLeft;
                    indices[counter++] = lowerRight;
                    indices[counter++] = lowerLeft;
                    indices[counter++] = upperLeft;
                    indices[counter++] = upperRight;
                    indices[counter++] = lowerRight;
                }
            indexBuffer = new IndexBuffer(
            device,
            IndexElementSize.SixteenBits,
            indices.Length,
            BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);
        }

        public void Draw(Camera camera,Effect effect)
        {
            effect.CurrentTechnique = effect.Techniques["Technique1"];
            effect.Parameters["terrainTexture1"].SetValue(terrainTexture);
            effect.Parameters["World"].SetValue(Matrix.Identity);
            effect.Parameters["View"].SetValue(camera.View);
            effect.Parameters["Projection"].SetValue(camera.Projection);
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.SetVertexBuffer(vertexBuffer);
                device.Indices = indexBuffer;
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList,0,0,vertexBuffer.VertexCount,0,indexBuffer.IndexCount / 3);
            }
        }
    }
}
