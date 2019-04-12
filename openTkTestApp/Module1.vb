﻿Option Explicit On

Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO

Module Module1

    Public Sub Main()
        Dim app As New GLTexturedCube
        app.Run(60, 60)
    End Sub

    Public Class GLTexturedCube
        Inherits GameWindow

        Protected angle As Single
        Protected textures(1) As Integer

        Public Sub New()
            MyBase.New(468, 360, GraphicsMode.Default, "Textured Cube with OpenTK")
        End Sub

        Protected Sub LoadTexture(ByVal textureId As Integer, ByVal filename As String)
            Dim bmp As New Bitmap(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\..\..\" + filename)

            Dim data As BitmapData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height),
                                                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                System.Drawing.Imaging.PixelFormat.Format32bppArgb)

            GL.BindTexture(TextureTarget.Texture2D, textureId)
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                      bmp.Width, bmp.Height, 0, OpenGL.PixelFormat.Bgra,
                      PixelType.UnsignedByte, data.Scan0)

            bmp.UnlockBits(data)
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureMinFilter.Linear)
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureMagFilter.Linear)
        End Sub

        Protected Sub LoadTextures()
            Console.WriteLine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName))
            GL.GenTextures(textures.Length, textures)
            LoadTexture(textures(0), "texture.png")
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            GL.ClearColor(0.5, 0.5, 0.6, 0)
            GL.Enable(EnableCap.DepthTest)
            GL.Enable(EnableCap.Texture2D)

            LoadTextures()
        End Sub

        Protected Overrides Sub OnResize(ByVal e As System.EventArgs)
            MyBase.OnResize(e)

            GL.Viewport(0, 0, Me.Width, Me.Height)

            Dim aspect As Single = CSng(Me.Width) / Me.Height
            Dim projMat As Matrix4 = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect, 0.1, 100.0)

            GL.MatrixMode(MatrixMode.Projection)
            GL.LoadMatrix(projMat)

            GL.MatrixMode(MatrixMode.Modelview)
            GL.LoadIdentity()
        End Sub

        Protected Overrides Sub OnRenderFrame(ByVal e As OpenTK.FrameEventArgs)
            MyBase.OnRenderFrame(e)
            GL.Clear(ClearBufferMask.ColorBufferBit Or ClearBufferMask.DepthBufferBit)

            GL.LoadIdentity()
            GL.Translate(0, 0, -5)
            GL.Rotate(angle, 0, 0.1, 0.1)
            GL.Rotate(angle, 0, 1, 0)
            GL.Rotate(angle, 0.1, 0, 0)
            GL.Rotate(angle, 0, 0.1, 0)
            GL.Rotate(angle, 0, 0, 0.1)

            angle += 0.5

            GL.BindTexture(TextureTarget.Texture2D, textures(0))

            drawCube()

        End Sub

        Public Sub drawCube()
            GL.Begin(BeginMode.Quads)

            GL.TexCoord2(0.0, 0.0) : GL.Vertex3(-1, 1, 1)
            GL.TexCoord2(1.0, 0.0) : GL.Vertex3(1, 1, 1)
            GL.TexCoord2(1.0, 1.0) : GL.Vertex3(1, -1, 1)
            GL.TexCoord2(0.0, 1.0) : GL.Vertex3(-1, -1, 1)


            GL.TexCoord2(0.0, 0.0) : GL.Vertex3(1, 1, -1)
            GL.TexCoord2(1.0, 0.0) : GL.Vertex3(-1, 1, -1)
            GL.TexCoord2(1.0, 1.0) : GL.Vertex3(-1, -1, -1)
            GL.TexCoord2(0.0, 1.0) : GL.Vertex3(1, -1, -1)


            GL.TexCoord2(0.0, 0.0) : GL.Vertex3(1, 1, 1)
            GL.TexCoord2(1.0, 0.0) : GL.Vertex3(1, 1, -1)
            GL.TexCoord2(1.0, 1.0) : GL.Vertex3(1, -1, -1)
            GL.TexCoord2(0.0, 1.0) : GL.Vertex3(1, -1, 1)


            'GL.TexCoord2(0.0, 0.0) : GL.Vertex3(-1, 1, -1)
            'GL.TexCoord2(1.0, 0.0) : GL.Vertex3(-1, -1, -1)
            'GL.TexCoord2(1.0, 1.0) : GL.Vertex3(-1, -1, 1)
            'GL.TexCoord2(0.0, 1.0) : GL.Vertex3(-1, 1, 1)


            GL.TexCoord2(0.0, 0.0) : GL.Vertex3(-1, 1, -1)
            GL.TexCoord2(1.0, 0.0) : GL.Vertex3(1, 1, -1)
            GL.TexCoord2(1.0, 1.0) : GL.Vertex3(1, 1, 1)
            GL.TexCoord2(0.0, 1.0) : GL.Vertex3(-1, 1, 1)


            GL.TexCoord2(0.0, 0.0) : GL.Vertex3(-1, -1, -1)
            GL.TexCoord2(1.0, 0.0) : GL.Vertex3(-1, -1, 1)
            GL.TexCoord2(1.0, 1.0) : GL.Vertex3(1, -1, 1)
            GL.TexCoord2(0.0, 1.0) : GL.Vertex3(1, -1, -1)
            GL.End()

            SwapBuffers()
        End Sub

    End Class

End Module
