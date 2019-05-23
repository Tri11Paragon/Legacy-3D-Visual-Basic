Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports OpenTK.Input

Public Class camera

    Public x, y, z As Double

    Public Sub New(x As Double, y As Double, z As Double)
        Me.x = x
        Me.y = y
        Me.z = z
    End Sub

End Class

Public Class entityRenderer

End Class

Public Class masterRenderer

    Private projectionMatrix As Matrix4d



    Public Sub New()

    End Sub

    Public Sub prepare()
        GL.Enable(EnableCap.DepthTest)
        GL.Clear(ClearBufferMask.ColorBufferBit) : GL.Clear(ClearBufferMask.DepthBufferBit)
        GL.ClearColor(0.5, 0.5, 0.6, 0)
    End Sub

    Public Sub createProjectionMatrix()

    End Sub

End Class