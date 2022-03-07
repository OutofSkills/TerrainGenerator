using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TerrainGenerator
{
    public partial class TerrainGeneratorForm : Form
    {
        private Device device = null;

        private VertexBuffer vertexBuffer = null;
        
        private IndexBuffer indexBuffer = null;

        // Size of the terain
        private static int terrainWidth = 10;
        private static int terrainLenght = 10;

        private static int verticesCount = terrainLenght * terrainLenght;
        private static int indexCount = (terrainWidth - 1) * (terrainLenght - 1) * 6;

        private Vector3 cameraPosition, cameraLookAt, cameraUp;

        private CustomVertex.PositionColored[] vertices = null;

        // Indices for the cube corners coordinates
        private static int[] indices = null;


        public TerrainGeneratorForm()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);

            InitializeComponent();

            InitializeGraphics();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1, 0);

            SetupCamera();

            device.BeginScene();

            device.VertexFormat = CustomVertex.PositionColored.Format;
            device.SetStreamSource(0, vertexBuffer, 0);
            device.Indices = indexBuffer;

            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, verticesCount, 0, indexCount/3);

            device.EndScene();

            device.Present();

            this.Invalidate();
        }

        private void InitializeGraphics()
        {
            PresentParameters pp = new PresentParameters();
            // True = window form, False = Full screen
            pp.Windowed = true;
            pp.SwapEffect = SwapEffect.Discard;

            pp.EnableAutoDepthStencil = true;
            pp.AutoDepthStencilFormat = DepthFormat.D16;

            device = new Device(0, DeviceType.Hardware, this, CreateFlags.HardwareVertexProcessing, pp);

            // Generate the vertices and it's indices
            GenerateVertex();
            GenerateIndex();

            // Initialize the veritices buffer
            vertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionColored), verticesCount, device, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionColored.Format, Pool.Default);
            vertexBuffer.Created += new EventHandler(this.OnVertexBufferCreate);
            OnVertexBufferCreate(vertexBuffer, null);

            // Initialize the index buffer
            indexBuffer = new IndexBuffer(typeof(int), indexCount, device, Usage.WriteOnly, Pool.Default);
            indexBuffer.Created += new EventHandler(this.OnIndexBufferCreate);
            OnIndexBufferCreate(indexBuffer, null);

            // Set initial camera position
            cameraPosition = new Vector3(2, 4.5f, -3.5f);
            cameraLookAt = new Vector3(2, 3.5f, -2.5f);
            cameraUp = new Vector3(0, 1, 0);
        }

        private void OnIndexBufferCreate(object sender, EventArgs e)
        {
            IndexBuffer buffer = (IndexBuffer)sender;
            buffer.SetData(indices, 0, LockFlags.None);
        }

        private void OnVertexBufferCreate(object sender, EventArgs e)
        {
            VertexBuffer buffer = (VertexBuffer)sender;
            buffer.SetData(vertices, 0, LockFlags.None);
        }

        private void SetupCamera()
        {
            device.Transform.Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4, this.Width / this.Height, 1.0f, 100.0f);
            device.Transform.View = Matrix.LookAtLH(cameraPosition, cameraLookAt, cameraUp);

            device.RenderState.Lighting = false;
            device.RenderState.CullMode = Cull.CounterClockwise;
            device.RenderState.FillMode = FillMode.WireFrame;
        }

        // Used to draw a cube
        private void DrawBox(float yaw, float pitch, float roll, float x, float y, float z)
        {
            device.Transform.World = Matrix.RotationYawPitchRoll(yaw, pitch, roll) * (Matrix.Translation(x, y, z));
            
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 8, 0, indices.Length/3);
        }

        private void GenerateVertex()
        {
            vertices = new CustomVertex.PositionColored[verticesCount];
            int k = 0;

            for (int z = 0; z < terrainWidth; z++)
            {
                for(int x = 0; x < terrainLenght; x++)
                {
                    vertices[k].Position = new Vector3(x, 0, z);
                    vertices[k].Color = Color.White.ToArgb();
                    k++;
                }
            }
        }

        private void GenerateIndex()
        {
            indices = new int[indexCount];

            int k = 0;
            int length = 0;

            for (int i = 0; i < indexCount; i +=6)
            {
                indices[i] = k;
                indices[i + 1] = k + terrainLenght;
                indices[i + 2] = k + terrainLenght + 1;
                indices[i + 3] = k;
                indices[i + 4] = k + terrainLenght + 1;
                indices[i + 5] = k + 1;

                k++;
                length++;

                if(length == terrainLenght - 1)
                {
                    length = 0;
                    k++;
                }
            }

        }
    }
}
