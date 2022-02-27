﻿using System;
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
    public partial class Form1 : Form
    {
        private Device device = null;

        private VertexBuffer vertexBuffer = null;

        private CustomVertex.PositionColored[] vertices = null;

        private float angle = 0.0f;


        public Form1()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);

            InitializeComponent();

            InitializeGraphics();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            device.Clear(ClearFlags.Target, Color.CornflowerBlue, 1, 0);

            SetupCamera();

            device.BeginScene();

            device.VertexFormat = CustomVertex.PositionColored.Format;
            device.SetStreamSource(0, vertexBuffer, 0);
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);

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

            device = new Device(0, DeviceType.Hardware, this, CreateFlags.HardwareVertexProcessing, pp);

            vertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionColored), 3, device, Usage.WriteOnly, CustomVertex.PositionColored.Format, Pool.Default);
            vertexBuffer.Created += new EventHandler(this.OnVertexBufferCreate);
            OnVertexBufferCreate(vertexBuffer, null);

            vertexBuffer.SetData(vertices, 0, LockFlags.None);
        }

        private void OnVertexBufferCreate(object sender, EventArgs e)
        {
            VertexBuffer butter = (VertexBuffer)sender;

            vertices = new CustomVertex.PositionColored[3];

            vertices[0].Position = new Vector3(0, 1, 1);
            vertices[0].Color = Color.Green.ToArgb();

            vertices[1].Position = new Vector3(-1, -1, 1);
            vertices[1].Color = Color.Blue.ToArgb();

            vertices[2].Position = new Vector3(1, -1, 1);
            vertices[2].Color = Color.Red.ToArgb();

            butter.SetData(vertices, 0, LockFlags.None);

        }

        private void SetupCamera()
        {
            device.Transform.Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4, this.Width / this.Height, 1.0f, 100.0f);
            device.Transform.View = Matrix.LookAtLH(new Vector3(0, 0, 5), new Vector3(), new Vector3(0, 1, 0));

            device.Transform.World = Matrix.RotationX(angle);

            device.RenderState.Lighting = false;
        }
    }
}