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

' Brett Terpstra
' 2019-06-16
' final project
'useless gui class
Public Class gui

    ' MOST OF THIS STUFF IS UESLESS AND DOES NOT WORK

    Public Shared renderer As TextRenderer ' does not work
    Private Shared serif As Font = New Font(FontFamily.GenericSerif, 24) ' useless
    Private Shared sans As Font = New Font(FontFamily.GenericSansSerif, 24) ' useless
    Private Shared mono As Font = New Font(FontFamily.GenericMonospace, 24) ' uesless

    ' is the escape open? (used in other classes)
    Public Shared isEscapeOpen = False

    Public Shared Sub create()
        ' not used
        renderer = New TextRenderer(Module1.app.Width, Module1.app.Height)
        ' does not work
        renderer.DrawString("Hello There!", sans, Brushes.Black, New PointF(0, 0))
    End Sub

    Public Shared Sub render()
        ' draw only if escape was pressed
        If isEscapeOpen Then
            ' does not draw the font on the screen :( makes a neat square though
            drawTexture(renderer.texture, 0, 0, 64, 64)
        End If
    End Sub

    Public Shared Sub unload()
        ' deletes stuff thats not needed.
        renderer.Dispose()
    End Sub

    ' not used keypress register
    Public Shared Sub keyPressed(e As KeyPressEventArgs)

    End Sub

    ' does not work that well. just pushes the object we are trying to render to the center of the screen. does not matter what x/y is. its always in the center.
    Public Shared Sub drawTexture(texture As Integer, x As Double, y As Double, width As Double, height As Double)
        ' width / height from the main module
        Dim w = Module1.app.Width
        Dim h = Module1.app.Height
        ' this is the same as in the artist class it just was made for GUI renedering and does not work :(
        ' pushes the active view matrix off the stack.
        GL.PushMatrix()
        'binds textures
        GL.BindTexture(TextureTarget.Texture2D, texture)
        ' translates to scren space ( does not work)
        GL.Translate(x / w, y / h, -1)
        'rotates to make face right
        GL.Rotate(90, 0, 1, 0)
        GL.Rotate(-90, 1, 0, 0)
        ' scales the object
        GL.Scale(width / w, height / h, 0.1)
        artist.drawMesh(polys.face)
        ' unsclaes back to world
        GL.Scale(1 / (width / w), 1 / (height / h), 1)
        ' unrotates back to world space
        GL.Rotate(90, 1, 0, 0)
        GL.Rotate(-90, 0, 1, 0)
        ' untranslates to enable world space
        GL.Translate(-x * w, -y * h, 1)
        ' pops matrix back on the stack
        GL.PopMatrix()
    End Sub

End Class
