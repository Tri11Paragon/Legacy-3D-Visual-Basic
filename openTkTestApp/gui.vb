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

    Public Shared renderer As TextRenderer
    Private Shared serif As Font = New Font(FontFamily.GenericSerif, 24)
    Private Shared sans As Font = New Font(FontFamily.GenericSansSerif, 24)
    Private Shared mono As Font = New Font(FontFamily.GenericMonospace, 24)

    Public Shared isEscapeOpen = False
    Shared test As renderObject

    Public Shared Sub create()
        test = New renderObject(1, 1)
        renderer = New TextRenderer(Module1.app.Width, Module1.app.Height)
        renderer.DrawString("Hello There!", sans, Brushes.Black, New PointF(0, 0))
    End Sub

    Public Shared Sub render()
        If isEscapeOpen Then
            drawTexture(renderer.texture, 0, 0, 64, 64)
        End If
    End Sub

    Public Shared Sub unload()
        renderer.Dispose()
    End Sub

    Public Shared Sub keyPressed(e As KeyPressEventArgs)

    End Sub

    Public Shared Sub drawTexture(texture As Integer, x As Double, y As Double, width As Double, height As Double)
        Dim w = Module1.app.Width
        Dim h = Module1.app.Height
        GL.PushMatrix()
        GL.BindTexture(TextureTarget.Texture2D, texture)
        GL.Translate(x / w, y / h, -1)
        GL.Rotate(90, 0, 1, 0)
        GL.Rotate(-90, 1, 0, 0)
        GL.Scale(width / w, height / h, 0.1)
        artist.drawMesh(polys.face)
        GL.Scale(1 / (width / w), 1 / (height / h), 1)
        GL.Rotate(90, 1, 0, 0)
        GL.Rotate(-90, 0, 1, 0)
        GL.Translate(-x * w, -y * h, 1)
        GL.PopMatrix()
    End Sub

End Class

Public Class renderObject

    Dim verts As List(Of Vector3) = New List(Of Vector3)
    Dim textures As List(Of Vector2) = New List(Of Vector2)
    Dim mesh As Mesh

    Public Sub New(width As Double, height As Double)
        verts.Add(New Vector3(0, 0, 0))
        verts.Add(New Vector3(0, height, 0))
        verts.Add(New Vector3(width, height, 0))

        verts.Add(New Vector3(width, height, 0))
        verts.Add(New Vector3(width, 0, 0))
        verts.Add(New Vector3(0, 0, 0))

        textures.Add(New Vector2(0, 0))
        textures.Add(New Vector2(0, 1))
        textures.Add(New Vector2(1, 1))

        textures.Add(New Vector2(1, 1))
        textures.Add(New Vector2(1, 0))
        textures.Add(New Vector2(0, 0))

        mesh = New Mesh(verts, textures, Nothing)
    End Sub

    Public Sub render(texture As Integer, x As Double, y As Double)
        artist.drawMesh(mesh, texture, x, y, -1)
    End Sub

End Class
