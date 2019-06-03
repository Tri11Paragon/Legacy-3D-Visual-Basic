Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports OpenTK.Input
Imports System.ValueTuple

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

    Public Shared Sub drawPoly(ByRef vt As List(Of o_polys.BFace), ByRef texture As Integer, x As Double, y As Double, z As Double)
        GL.BindTexture(TextureTarget.Texture2D, texture)
        GL.Translate(x, y, z)
        drawPoly(vt)
        GL.Translate(-x, -y, -z)
    End Sub

    Public Shared Sub drawMesh(ByRef m As o_polys.Mesh, ByRef texture As Integer, x As Double, y As Double, z As Double)
        GL.BindTexture(TextureTarget.Texture2D, texture)
        GL.Translate(x, y, z)
        drawMesh(m)
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

    ' does not work
    Private Shared Sub drawPoly(tu As List(Of o_polys.BFace))
        GL.Begin(PrimitiveType.Triangles)
        For Each f In tu
            GL.TexCoord2(f.t1) : GL.Vertex3(f.v1)
            GL.TexCoord2(f.t2) : GL.Vertex3(f.v2)
            GL.TexCoord2(f.t3) : GL.Vertex3(f.v3)
        Next
        GL.End()
    End Sub

    Private Shared Sub drawMesh(m As o_polys.Mesh)
        GL.PushMatrix()
        GL.EnableClientState(ArrayCap.VertexArray)
        GL.EnableClientState(ArrayCap.TextureCoordArray)
        Dim meshVertices = m.vertices.ToArray()
        Dim meshTexture = m.textureVertices.ToArray()
        GL.VertexPointer(3, VertexPointerType.Float, 0, meshVertices)
        GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, meshTexture)
        Try
            GL.DrawArrays(PrimitiveType.Triangles, 0, meshVertices.Length)
        Catch e As System.AccessViolationException
            Console.WriteLine(e.StackTrace)
        End Try
        GL.DisableClientState(ArrayCap.VertexArray)
        GL.DisableClientState(ArrayCap.TextureCoordArray)
        GL.PopMatrix()
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