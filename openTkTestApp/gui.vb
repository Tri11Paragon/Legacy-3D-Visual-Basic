Imports OpenTK
Imports OpenTK.Input
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System
Imports System.Configuration
Imports System.Data
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO

Public Class gui

    Public Shared isEscapeOpen = False

    Public Shared Sub create()

    End Sub

    Public Shared Sub render()
        If isEscapeOpen Then
            drawTexture(world.textures(10), 0, 0, 64, 64)
        End If
    End Sub

    Public Shared Sub keyPressed(e As KeyPressEventArgs)

    End Sub

    Public Shared Sub drawTexture(texture As Integer, x As Double, y As Double, width As Double, height As Double)
        GL.BindTexture(TextureTarget.Texture2D, texture)
        'GL.Translate(-camera.x, -camera.y, -camera.z)
        GL.Translate(x, y, 0)
        GL.Begin(PrimitiveType.Quads)

        GL.TexCoord2(0, 0)
        GL.Vertex2(0, 0)

        GL.TexCoord2(1, 0)
        GL.Vertex2(width, 0)

        GL.TexCoord2(1, 1)
        GL.Vertex2(width, height)

        GL.TexCoord2(0, 1)
        GL.Vertex2(0, height)

        GL.End()
        'GL.Translate(-x, -y, 0)
        'GL.Translate(camera.x, camera.y, camera.z)
    End Sub

End Class
