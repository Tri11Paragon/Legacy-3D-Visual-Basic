Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports OpenTK.Input

Public Class o_artist

    Public Shared Sub drawFace(ByRef texture As Integer, x As Double, y As Double, z As Double)
        GL.BindTexture(TextureTarget.Texture2D, texture)
        GL.Translate(x, y, z)
        drawFace()
        GL.Translate(-x, -y, -z)
    End Sub

    Public Shared Sub drawCube(ByRef texture As Integer, x As Double, y As Double, z As Double)
        GL.BindTexture(TextureTarget.Texture2D, texture)
        GL.Translate(x, y, z)
        drawCube()
        GL.Translate(-x, -y, -z)
    End Sub

    Public Shared Sub drawTriangle(ByRef texture As Integer, x As Double, y As Double, z As Double)
        GL.BindTexture(TextureTarget.Texture2D, texture)
        GL.Translate(x, y, z)
        drawTriangle()
        GL.Translate(-x, -y, -z)
    End Sub

    Public Shared Sub drawPoly(ByRef vt As List(Of touple(Of Vector3, Vector2)), ByRef texture As Integer, x As Double, y As Double, z As Double)
        GL.BindTexture(TextureTarget.Texture2D, texture)
        GL.Translate(x, y, z)
        drawPoly(vt)
        GL.Translate(-x, -y, -z)
    End Sub

    Public Shared Sub drawPolyQuad(ByRef vt() As touple(Of Vector3, Vector2), ByRef texture As Integer, x As Double, y As Double, z As Double)
        GL.BindTexture(TextureTarget.Texture2D, texture)
        GL.Translate(x, y, z)
        drawPolyQuad(vt)
        GL.Translate(-x, -y, -z)
    End Sub

    Private Shared Sub drawCube()
        GL.Begin(PrimitiveType.Quads)


        'side
        GL.TexCoord2(0.0, 0.0) : GL.Vertex3(-1, 1, 1)
        GL.TexCoord2(1.0, 0.0) : GL.Vertex3(1, 1, 1)
        GL.TexCoord2(1.0, 1.0) : GL.Vertex3(1, -1, 1)
        GL.TexCoord2(0.0, 1.0) : GL.Vertex3(-1, -1, 1)

        ' oppsite side
        GL.TexCoord2(0.0, 0.0) : GL.Vertex3(1, 1, -1)
        GL.TexCoord2(1.0, 0.0) : GL.Vertex3(-1, 1, -1)
        GL.TexCoord2(1.0, 1.0) : GL.Vertex3(-1, -1, -1)
        GL.TexCoord2(0.0, 1.0) : GL.Vertex3(1, -1, -1)

        'front
        GL.TexCoord2(0.0, 0.0) : GL.Vertex3(1, 1, 1)
        GL.TexCoord2(1.0, 0.0) : GL.Vertex3(1, 1, -1)
        GL.TexCoord2(1.0, 1.0) : GL.Vertex3(1, -1, -1)
        GL.TexCoord2(0.0, 1.0) : GL.Vertex3(1, -1, 1)

        'back
        GL.TexCoord2(0.0, 0.0) : GL.Vertex3(-1, 1, -1)
        GL.TexCoord2(0.0, 1.0) : GL.Vertex3(-1, -1, -1)
        GL.TexCoord2(1.0, 1.0) : GL.Vertex3(-1, -1, 1)
        GL.TexCoord2(1.0, 0.0) : GL.Vertex3(-1, 1, 1)

        ' top
        GL.TexCoord2(0.0, 0.0) : GL.Vertex3(-1, 1, -1)
        GL.TexCoord2(1.0, 0.0) : GL.Vertex3(1, 1, -1)
        GL.TexCoord2(1.0, 1.0) : GL.Vertex3(1, 1, 1)
        GL.TexCoord2(0.0, 1.0) : GL.Vertex3(-1, 1, 1)

        ' bottom
        GL.TexCoord2(0.0, 0.0) : GL.Vertex3(-1, -1, -1)
        GL.TexCoord2(1.0, 0.0) : GL.Vertex3(-1, -1, 1)
        GL.TexCoord2(1.0, 1.0) : GL.Vertex3(1, -1, 1)
        GL.TexCoord2(0.0, 1.0) : GL.Vertex3(1, -1, -1)

        GL.End()
    End Sub

    Private Shared Sub drawFace()
        GL.Begin(PrimitiveType.Quads)

        GL.TexCoord2(0.0, 0.0) : GL.Vertex3(1, 1, 1)
        GL.TexCoord2(1.0, 0.0) : GL.Vertex3(1, 1, -1)
        GL.TexCoord2(1.0, 1.0) : GL.Vertex3(1, -1, -1)
        GL.TexCoord2(0.0, 1.0) : GL.Vertex3(1, -1, 1)

        GL.End()
    End Sub

    Private Shared Sub drawTriangle()
        GL.Begin(PrimitiveType.Quads)

        GL.TexCoord2(0.0, 0.0) : GL.Vertex3(-1, -1, -1)
        GL.TexCoord2(1.0, 0.0) : GL.Vertex3(-1, -1, 1)
        GL.TexCoord2(1.0, 1.0) : GL.Vertex3(1, -1, 1)
        GL.TexCoord2(0.0, 1.0) : GL.Vertex3(1, -1, -1)


        GL.End()

        GL.Begin(PrimitiveType.Triangles)

        GL.TexCoord2(0.0, 1.0) : GL.Vertex3(-1, -1, -1)
        GL.TexCoord2(0.5, 0.0) : GL.Vertex3(0, 1, 0)
        GL.TexCoord2(1.0, 1.0) : GL.Vertex3(-1, -1, 1)

        GL.TexCoord2(0.0, 1.0) : GL.Vertex3(-1, -1, 1)
        GL.TexCoord2(0.5, 0.0) : GL.Vertex3(0, 1, 0)
        GL.TexCoord2(1.0, 1.0) : GL.Vertex3(1, -1, 1)

        GL.TexCoord2(0.0, 1.0) : GL.Vertex3(1, -1, 1)
        GL.TexCoord2(0.5, 0.0) : GL.Vertex3(0, 1, 0)
        GL.TexCoord2(1.0, 1.0) : GL.Vertex3(1, -1, -1)

        GL.TexCoord2(0.0, 1.0) : GL.Vertex3(1, -1, -1)
        GL.TexCoord2(0.5, 0.0) : GL.Vertex3(0, 1, 0)
        GL.TexCoord2(1.0, 1.0) : GL.Vertex3(-1, -1, -1)

        GL.End()
    End Sub

    Private Shared Sub drawPoly(tu As List(Of touple(Of Vector3, Vector2)))
        GL.Begin(PrimitiveType.Triangles)
        For Each d In tu
            GL.TexCoord2(d.Y_().X, d.Y_.Y) : GL.Vertex3(d.X_.X, d.X_.Y, d.X_.Z)
        Next
        GL.End()
    End Sub

    Private Shared Sub drawPolyQuad(tu() As touple(Of Vector3, Vector2))
        GL.Begin(PrimitiveType.Quads)
        For Each d In tu
            GL.TexCoord2(d.Y_().X, d.Y_.Y) : GL.Vertex3(d.X_.X, d.X_.Y, d.X_.Z)
        Next
        GL.End()
    End Sub

End Class

Public Class touple(Of X, Y)

    Private _x As X
    Private _y As Y

    Public Sub New(x As X, y As Y)
        Me._x = x
        Me._y = y
    End Sub

    ' bad idea to null but....
    Public Sub New()
        Me._x = Nothing
        Me._y = Nothing
    End Sub

    Public Function getX() As X
        Return _x
    End Function

    Public Function X_() As X
        Return _x
    End Function

    Public Function getY() As Y
        Return _y
    End Function

    Public Function Y_() As Y
        Return _y
    End Function

    Public Sub setY(y As Y)
        _y = y
    End Sub

    Public Sub setX(x As X)
        _x = x
    End Sub

End Class