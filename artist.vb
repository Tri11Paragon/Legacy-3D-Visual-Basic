Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports OpenTK.Input

Public Class Class1

    Public Shared Sub drawCube(texture As Integer, x As Double, y As Double, z As Double)
        GL.BindTexture(TextureTarget.Texture2D, texture)

    End Sub

    Private Shared Sub drawCube()
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

        GL.TexCoord2(0.0, 0.0) : GL.Vertex3(-1, 1, -1)
        GL.TexCoord2(0.0, 1.0) : GL.Vertex3(-1, -1, -1)
        GL.TexCoord2(1.0, 1.0) : GL.Vertex3(-1, -1, 1)
        GL.TexCoord2(1.0, 0.0) : GL.Vertex3(-1, 1, 1)


        GL.TexCoord2(0.0, 0.0) : GL.Vertex3(-1, 1, -1)
        GL.TexCoord2(1.0, 0.0) : GL.Vertex3(1, 1, -1)
        GL.TexCoord2(1.0, 1.0) : GL.Vertex3(1, 1, 1)
        GL.TexCoord2(0.0, 1.0) : GL.Vertex3(-1, 1, 1)


        GL.TexCoord2(0.0, 0.0) : GL.Vertex3(-1, -1, -1)
        GL.TexCoord2(1.0, 0.0) : GL.Vertex3(-1, -1, 1)
        GL.TexCoord2(1.0, 1.0) : GL.Vertex3(1, -1, 1)
        GL.TexCoord2(0.0, 1.0) : GL.Vertex3(1, -1, -1)
        GL.End()
    End Sub

End Class
