Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports OpenTK.Input
Imports System.ValueTuple

' Brett Terpstra
' 2019-06-16
' final project
' mesh drawing class
Public Class artist

    ' draws mesh with texture and postion
    Public Shared Sub drawMesh(ByRef m As Mesh, ByRef texture As Integer, x As Double, y As Double, z As Double)
        ' bind texture to buffer
        GL.BindTexture(TextureTarget.Texture2D, texture)
        ' move to the positon
        GL.Translate(x, y, z)
        ' draw the mesh
        drawMesh(m)
        ' reset to world coords. (This is done so that way every mesh is drawn from 0, 0, 0. if this is not done, then things get weird.)
        GL.Translate(-x, -y, -z)
    End Sub

    ' draws mesh
    Public Shared Sub drawMesh(m As Mesh)
        ' pushes the active view matrix off the stack.
        GL.PushMatrix()
        ' enables the vertex array and the texture array inside the internal shader of OpenGL
        GL.EnableClientState(ArrayCap.VertexArray)
        GL.EnableClientState(ArrayCap.TextureCoordArray)
        ' gets the mesh and texture verts as an array
        Dim meshVertices = m.vertices.ToArray()
        Dim meshTexture = m.textureVertices.ToArray()
        ' gives a pointer to the internal shader to the mesh and texture verts
        GL.VertexPointer(3, VertexPointerType.Float, 0, meshVertices)
        GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, meshTexture)
        ' trys to render the arrays (needs to be a try catch or else there is a crash i can't fix)
        Try
            ' draws the arrays using triagles
            GL.DrawArrays(PrimitiveType.Triangles, 0, meshVertices.Length)
        Catch e As System.AccessViolationException
            ' even when it errors it does not print. don't know why. without it errors
            Console.WriteLine(e.StackTrace)
        End Try
        ' disable the client state
        GL.DisableClientState(ArrayCap.VertexArray)
        GL.DisableClientState(ArrayCap.TextureCoordArray)
        ' pops the matrix back on
        GL.PopMatrix()
    End Sub
End Class

' this is a touple, it stores 2 values as 1 
' im not sure if its used anywhere anymore.
Public Class touple(Of X, Y)

    ' vairables
    Private _x As X
    Private _y As Y

    ' constructors
    Public Sub New(x As X, y As Y)
        Me._x = x
        Me._y = y
    End Sub

    ' bad idea to null but....
    Public Sub New()
        Me._x = Nothing
        Me._y = Nothing
    End Sub

    'getters. They get data

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

' im not using this, so im not going to explain it (i hope that is reasonable)
' took this from https://github.com/mono/opentk/blob/master/Source/Examples/OpenGL/1.x/TextRendering.cs
Public Class TextRenderer
    'Inherits IDisposable

    Private bmp As Bitmap
    Private gfx As Graphics
    Public texture As Integer
    Private dirty_region As Rectangle
    Private disposed As Boolean

    Public Sub New(ByVal width As Integer, ByVal height As Integer)
        If width <= 0 Then Throw New ArgumentOutOfRangeException("width")
        If height <= 0 Then Throw New ArgumentOutOfRangeException("height ")
        If GraphicsContext.CurrentContext Is Nothing Then Throw New InvalidOperationException("No GraphicsContext is current on the calling thread.")
        bmp = New Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
        gfx = Graphics.FromImage(bmp)
        gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias
        texture = GL.GenTexture()
        GL.BindTexture(TextureTarget.Texture2D, texture)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, CInt(TextureMinFilter.Linear))
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, CInt(TextureMagFilter.Linear))
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero)
    End Sub

    Public Sub Clear(ByVal color As Color)
        gfx.Clear(color)
        dirty_region = New Rectangle(0, 0, bmp.Width, bmp.Height)
    End Sub

    Public Sub DrawString(ByVal text As String, ByVal font As Font, ByVal brush As Brush, ByVal point As PointF)
        gfx.DrawString(text, font, brush, point)
        Dim size As SizeF = gfx.MeasureString(text, font)
        dirty_region = Rectangle.Round(RectangleF.Union(dirty_region, New RectangleF(point, size)))
        dirty_region = Rectangle.Intersect(dirty_region, New Rectangle(0, 0, bmp.Width, bmp.Height))
    End Sub

    Private Sub UploadBitmap()
        If dirty_region <> RectangleF.Empty Then
            Dim data As System.Drawing.Imaging.BitmapData = bmp.LockBits(dirty_region, System.Drawing.Imaging.ImageLockMode.[ReadOnly], System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            GL.BindTexture(TextureTarget.Texture2D, texture)
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, dirty_region.X, dirty_region.Y, dirty_region.Width, dirty_region.Height, OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0)
            bmp.UnlockBits(data)
            dirty_region = Rectangle.Empty
        End If
    End Sub

    Private Sub Dispose(ByVal manual As Boolean)
        If Not disposed Then

            If manual Then
                bmp.Dispose()
                gfx.Dispose()
                If GraphicsContext.CurrentContext IsNot Nothing Then GL.DeleteTexture(texture)
            End If

            disposed = True
        End If
    End Sub

    Public Sub Dispose()
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    Protected Overrides Sub Finalize()
        Console.WriteLine("[Warning] Resource leaked: {0}.", GetType(TextRenderer))
    End Sub
End Class