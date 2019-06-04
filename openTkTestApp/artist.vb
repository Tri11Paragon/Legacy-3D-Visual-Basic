Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports OpenTK.Input
Imports System.ValueTuple

Public Class artist

    Public Shared Sub drawMesh(ByRef m As Mesh, ByRef texture As Integer, x As Double, y As Double, z As Double)
        GL.BindTexture(TextureTarget.Texture2D, texture)
        GL.Translate(x, y, z)
        drawMesh(m)
        GL.Translate(-x, -y, -z)
    End Sub

    Public Shared Sub drawMesh(ByRef m As Mesh, x As Double, y As Double, z As Double)
        GL.Translate(x, y, z)
        drawMesh(m)
        GL.Translate(-x, -y, -z)
    End Sub

    Public Shared Sub drawMesh(ByRef m As Mesh, c As Color, x As Double, y As Double, z As Double)
        GL.Color3(c)
        GL.Translate(x, y, z)
        drawMesh(m)
        GL.Translate(-x, -y, -z)
        GL.Color3(255, 255, 255)
    End Sub

    Private Shared Sub drawMesh(m As Mesh)
        GL.EnableClientState(ArrayCap.VertexArray)
        GL.EnableClientState(ArrayCap.TextureCoordArray)
        GL.VertexPointer(3, VertexPointerType.Float, 0, m.vertices.ToArray())
        GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, m.textureVertices.ToArray())
        GL.DrawArrays(PrimitiveType.Triangles, 0, m.vertices.ToArray().Length)
        GL.DisableClientState(ArrayCap.VertexArray)
        GL.DisableClientState(ArrayCap.TextureCoordArray)
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