﻿Imports OpenTK
Imports OpenTK.Input
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System
Imports System.Configuration
Imports System.Data
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO

Public Class camera

    Public Declare Auto Function ShowWindow Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal nCmdShow As Integer) As Boolean
    Public Declare Auto Function GetConsoleWindow Lib "kernel32.dll" () As IntPtr
    Public Declare Auto Function SetCursorPos Lib "user32.dll" (x As Integer, y As Integer) As Boolean
    Public Const SW_HIDE As Integer = 0
    Public Const SW_SHOW As Integer = 1

    Public Shared keysDown(230) As Boolean
    Public Shared dVr(5) As Int16 ' stores data of boolean varabiles in a int16

    Public Shared x, y, z As Double
    Shared moveAtX, moveAtY As Double
    Public Shared enableVirtical As Boolean = False

    Public Shared Sub load()

    End Sub

    Public Shared Sub keyPressed(e As KeyPressEventArgs)
        gui.keyPressed(e)
    End Sub

    Public Shared Sub keyReleased(e As KeyboardKeyEventArgs)
        keysDown(e.ScanCode) = False
        If e.ScanCode = 20 Then
            If dVr(0) = 0 Then
                dVr(0) = 1
            Else
                dVr(0) = 0
            End If
            camera.ShowWindow(camera.GetConsoleWindow(), dVr(0))

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
            gui.isEscapeOpen = Not gui.isEscapeOpen
            'Process.GetCurrentProcess().CloseMainWindow()
        End If
        If e.ScanCode = Key.F12 Then
            x = 0
            y = 0
            z = 0
        End If
        If e.ScanCode = Key.F2 Then
            takePicture(Date.Now.Year & "-" & Date.Now.Month & "-" & Date.Now.Day & "-" & Date.Now.Hour & "-" & Date.Now.Minute & "-" & Date.Now.Second & "-" & Date.Now.Millisecond & ".png", ImageFormat.Png, 0, 0, Module1.app.Width, Module1.app.Height)
        End If
        If e.ScanCode = Key.F10 Then
            Throw New Exception("Illegal Button Press!")
        End If
    End Sub

    Public Shared Sub keyDown(e As KeyboardKeyEventArgs)
        keysDown(e.ScanCode) = True
    End Sub

    Public Shared Sub update()
        If keysDown(Key.W) Then
            moveAtX = settings.speed
        Else
            moveAtX = 0
        End If
        If keysDown(Key.S) Then
            moveAtX = -settings.speed
        ElseIf Not keysDown(Key.W) Then
            moveAtX = 0
        End If
        If keysDown(Key.A) Then
            moveAtY = settings.speed
        Else
            moveAtY = 0
        End If
        If keysDown(Key.D) Then
            moveAtY = -settings.speed
        ElseIf Not keysDown(Key.A) Then
            moveAtY = 0
        End If
        If keysDown(Key.LShift) Then
            y += settings.speed
        End If
        If keysDown(Key.Space) Then
            y -= settings.speed
        End If

        If keysDown(Key.T) Then
            world.entites(1).accelerate(0, 0.01, 0)
        End If

        If Not gui.isEscapeOpen Then
            Dim dx As Double = -(moveAtX * -Math.Sin(rai(Module1.GLTexturedCube.yaw))) + (moveAtY * Math.Cos(rai(Module1.GLTexturedCube.yaw)))
            If enableVirtical Then
                Dim dy As Double = (moveAtX * -Math.Sin(rai(Module1.GLTexturedCube.pitch)))
                y += dy
            End If
            Dim dz As Double = (moveAtX * Math.Cos(rai(Module1.GLTexturedCube.yaw))) + (moveAtY * -Math.Sin(rai(Module1.GLTexturedCube.yaw)))

            x += dx
            z += dz
        End If

        If settings.useSkybox Then
            GL.BindTexture(TextureTarget.Texture2D, world.textures(12))
            artist.drawMesh(polys.cubeMesh)
            GL.BindTexture(TextureTarget.Texture2D, 0)
        End If
        GL.Translate(x, y, z)
        GL.Scale(settings.scale(0), settings.scale(1), settings.scale(2))
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

        If (dVr(0) <> 1) Then
            SetCursorPos(DisplayDevice.Default.Width / 2, DisplayDevice.Default.Height / 2)
        End If
        dVr(4) = 1

        'dir(New Vector3(0, dVr(3), dVr(2)), 30)
    End Sub

    Public Shared Sub takePicture(file As String, format As ImageFormat, x As Integer, y As Integer, w As Integer, h As Integer)
        Module1.app.SwapBuffers()
        Dim pixels(w * h * 4) As Byte
        Dim bi As Bitmap = New Bitmap(w, h)
        Dim rect As Rectangle = New Rectangle(x, y, bi.Width, bi.Height)

        GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4)
        GL.ReadPixels(x, y, w, h, OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, pixels)


        Dim bit As BitmapData = bi.LockBits(rect, ImageLockMode.ReadWrite, bi.PixelFormat)
        System.Runtime.InteropServices.Marshal.Copy(pixels, 0, bit.Scan0, pixels.Length())
        bi.UnlockBits(bit)
        bi.RotateFlip(RotateFlipType.RotateNoneFlipY)
        bi.Save("screenshots/" & file, ImageFormat.Png)
        Module1.app.SwapBuffers()
    End Sub

    Public Shared Sub mouseWheel(e As MouseWheelEventArgs)

    End Sub

End Class
