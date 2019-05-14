Imports OpenTK
Imports OpenTK.Input
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System
Imports System.Configuration
Imports System.Data

Public Class o_camera

    Public Declare Auto Function ShowWindow Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal nCmdShow As Integer) As Boolean
    Public Declare Auto Function GetConsoleWindow Lib "kernel32.dll" () As IntPtr
    Public Declare Auto Function SetCursorPos Lib "user32.dll" (x As Integer, y As Integer) As Boolean
    Public Const SW_HIDE As Integer = 0
    Public Const SW_SHOW As Integer = 1

    Public Shared keysDown(230) As Boolean
    Public Shared dVr(5) As Int16 ' stores data of boolean varabiles in a int16

    Public Shared x, y, z As Double
    Shared moveAtX, moveAtY As Double
    Shared speed As Double = 0.2
    Public Shared enableVirtical As Boolean = False

    Public Shared Sub load()

    End Sub

    Public Shared Sub keyPressed(e As KeyPressEventArgs)

    End Sub

    Public Shared Sub keyReleased(e As KeyboardKeyEventArgs)
        keysDown(e.ScanCode) = False
        If e.ScanCode = 20 Then
            If dVr(0) = 0 Then
                dVr(0) = 1
            Else
                dVr(0) = 0
            End If
            o_camera.ShowWindow(o_camera.GetConsoleWindow(), dVr(0))
        End If
        If e.ScanCode = 19 Then
            If dVr(1) = 0 Then
                dVr(1) = 1
            Else
                dVr(1) = 0
            End If
        End If
        If dVr(1) And dVr(0) Then
            'Console.WriteLine(e.ScanCode)
        End If
        If e.ScanCode = Key.Escape Then
            Console.WriteLine("Exiting Program!")
            Process.GetCurrentProcess().CloseMainWindow()
        End If
    End Sub

    Public Shared Sub keyDown(e As KeyboardKeyEventArgs)
        keysDown(e.ScanCode) = True
    End Sub

    Public Shared Sub update()
        If keysDown(Key.W) Then
            moveAtX = speed
        Else
            moveAtX = 0
        End If
        If keysDown(Key.S) Then
            moveAtX = -speed
        ElseIf Not keysDown(Key.W) Then
            moveAtX = 0
        End If
        If keysDown(Key.A) Then
            moveAtY = speed
        Else
            moveAtY = 0
        End If
        If keysDown(Key.D) Then
            moveAtY = -speed
        ElseIf Not keysDown(Key.A) Then
            moveAtY = 0
        End If
        If keysDown(Key.LShift) Then
            y += speed
        End If
        If keysDown(Key.Space) Then
            y -= speed
        End If

        If keysDown(Key.T) Then
            o_Module1.GLTexturedCube.entites(1).accelerate(0, 0.01, 0)
        End If

        'Console.WriteLine(CType(moveAtX, String) + " : " + CType(moveAtY, String))

        Dim dx As Double = -(moveAtX * -Math.Sin(rai(o_Module1.GLTexturedCube.yaw))) + (moveAtY * Math.Cos(rai(o_Module1.GLTexturedCube.yaw)))
        If enableVirtical Then
            Dim dy As Double = (moveAtX * -Math.Sin(rai(o_Module1.GLTexturedCube.pitch)))
            y += dy
        End If
        Dim dz As Double = (moveAtX * Math.Cos(rai(o_Module1.GLTexturedCube.yaw))) + (moveAtY * -Math.Sin(rai(o_Module1.GLTexturedCube.yaw)))

        x += dx
        z += dz

        GL.Translate(x, y, z)

        dVr(4) = 0

    End Sub

    Public Shared Function rai(degrees As Double) As Double
        Return degrees * Math.PI / 180
    End Function

    Public Shared Sub mouseDown(e As MouseButtonEventArgs)

    End Sub

    Public Shared Sub mouseUp(e As MouseButtonEventArgs)

    End Sub

    Public Shared Sub mouseMove(e As MouseMoveEventArgs)
        dVr(2) = e.XDelta
        dVr(3) = e.YDelta

        'Console.WriteLine(CType(e.XDelta, String) + " : " + CType(e.YDelta, String))

        dVr(4) = 1

        SetCursorPos(DisplayDevice.Default.Width / 2, DisplayDevice.Default.Height / 2)

        'dir(New Vector3(0, dVr(3), dVr(2)), 30)
    End Sub

    Public Shared Sub mouseWheel(e As MouseWheelEventArgs)

    End Sub

End Class
