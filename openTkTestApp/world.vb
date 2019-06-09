Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports OpenTK.Input

Public Class world

    Public Shared entites As List(Of entity) = New List(Of entity)
    Public Shared textures(125) As Integer

    Private Shared spaceBetweenPoints As Integer = 1
    Private Shared size As Integer = 500
    Private Shared cap As Integer = 10
    Private Shared v As List(Of Vector3) = New List(Of Vector3)

    Public Shared Sub create()
        Randomize()
        'For i As Integer = 0 To size
        '    For j As Integer = 0 To size ' (cap - 1) * Rnd()
        '        v.Add(New Vector3(i * spaceBetweenPoints, i + j, j * spaceBetweenPoints))
        '    Next
        'Next
        'world.entites.Add(New entity(polys.pigMesh, world.textures(0), 0, 0, 5))
        'world.entites.Add(New entity(polys.pigMesh, world.textures(1), 0, 5, 0))
        'world.entites.Add(New entity(polys.pigMesh, world.textures(2), 0, 0, 0))
        'world.entites.Add(New entity(polys.pigMesh, world.textures(3), 0, 0, -5))
        'world.entites.Add(New entity(polys.pigMesh, world.textures(4), 5, 5, 0))
        'world.entites.Add(New entity(polys.pigMesh, world.textures(5), 5, 5, 0))
        'world.entites.Add(New entity(polys.pigMesh, world.textures(6), 5, 5, -5))
        'world.entites.Add(New entity(polys.pigMesh, world.textures(7), 5, 0, 0))
        'world.entites.Add(New entity(polys.pigMesh, world.textures(8), 5, -5, 0))
        'world.entites.Add(New entity(polys.pigMesh, world.textures(9), -5, -5, 5))
        'world.entites.Add(New entity(polys.pigMesh, world.textures(10), 5, -5, 5))
        'world.entites.Add(New entity(polys.pigMesh, world.textures(11), 5, -5, 5))

    End Sub

    Public Shared Sub update()
        GL.BindTexture(TextureTarget.Texture2D, textures(13))
        artist.drawMesh(polys.terrainMesh)
        For Each d As entity In world.entites
            d.update()
            d.draw()
        Next
        GL.BindTexture(TextureTarget.Texture2D, 0)
    End Sub

End Class
