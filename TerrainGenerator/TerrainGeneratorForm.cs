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
        private float angle = 0.0f;

        private Device device = null;

        private VertexBuffer vertexBuffer = null;
        
        private IndexBuffer indexBuffer = null;

        private CustomVertex.PositionColored[] vertices = null;

        // Indices for the cube corners coordinates
        private static readonly short[] indices =
        {
            0, 1, 2,  1, 3, 2,   // frontside
            4, 5, 6,  6, 5, 7,   // backside
            0, 5, 4,  0, 2, 5,   // upside
            1, 6, 7,  1, 7, 3,   // bottom
            0, 6, 1,  4, 6, 0,   // left side
            2, 3, 7,  5, 2, 7    // right side
        };


        public TerrainGeneratorForm()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);

            InitializeComponent();

            InitializeGraphics();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.CornflowerBlue, 1, 0);

            SetupCamera();

            device.BeginScene();

            device.VertexFormat = CustomVertex.PositionColored.Format;
            device.SetStreamSource(0, vertexBuffer, 0);
            device.Indices = indexBuffer;

            DrawBox(angle/(float) Math.PI, angle / (float)Math.PI, angle / (float)Math.PI, 0, 0, 0);
            DrawBox(angle / (float)Math.PI / 2, angle / (float)Math.PI * 3, angle / (float)Math.PI / 3*2, -0.5f, 0, 0);

            angle += 0.04f;

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

            // Initialize the veritices buffer
            vertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionColored), 8, device, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionColored.Format, Pool.Default);
            vertexBuffer.Created += new EventHandler(this.OnVertexBufferCreate);
            OnVertexBufferCreate(vertexBuffer, null);

            vertexBuffer.SetData(vertices, 0, LockFlags.None);

            // Initialize the index buffer
            indexBuffer = new IndexBuffer(typeof(short), indices.Length, device, Usage.WriteOnly, Pool.Default);
            indexBuffer.Created += new EventHandler(this.OnIndexBufferCreate);
            OnIndexBufferCreate(indexBuffer, null);
        }

        private void OnIndexBufferCreate(object sender, EventArgs e)
        {
            IndexBuffer buffer = (IndexBuffer)sender;

            buffer.SetData(indices, 0, LockFlags.None);
        }

        private void OnVertexBufferCreate(object sender, EventArgs e)
        {
            VertexBuffer buffer = (VertexBuffer)sender;

            vertices = new CustomVertex.PositionColored[8];

            vertices[0] = new CustomVertex.PositionColored(-1, 1, 1, Color.Red.ToArgb());
            vertices[1] = new CustomVertex.PositionColored(-1, -1, 1, Color.Blue.ToArgb());
            vertices[2] = new CustomVertex.PositionColored(1, 1, 1, Color.Green.ToArgb());
            vertices[3] = new CustomVertex.PositionColored(1, -1, 1, Color.Gold.ToArgb());
            vertices[4] = new CustomVertex.PositionColored(-1, 1, -1, Color.Gold.ToArgb());
            vertices[5] = new CustomVertex.PositionColored(1, 1, -1, Color.Green.ToArgb());
            vertices[6] = new CustomVertex.PositionColored(-1, -1, -1, Color.Blue.ToArgb());
            vertices[7] = new CustomVertex.PositionColored(1, -1, -1, Color.Red.ToArgb());

            buffer.SetData(vertices, 0, LockFlags.None);
        }

        private void SetupCamera()
        {
            device.Transform.Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4, this.Width / this.Height, 1.0f, 100.0f);
            device.Transform.View = Matrix.LookAtLH(new Vector3(0, 0, 10), new Vector3(), new Vector3(0, 1, 0));

            device.RenderState.Lighting = false;
            device.RenderState.CullMode = Cull.None;
        }

        private void DrawBox(float yaw, float pitch, float roll, float x, float y, float z)
        {
            device.Transform.World = Matrix.RotationYawPitchRoll(yaw, pitch, roll) * (Matrix.Translation(x, y, z));
            
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 8, 0, indices.Length/3);
        }
    }
}
