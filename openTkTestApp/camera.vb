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

Public Class camera

    Public Declare Auto Function ShowWindow Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal nCmdShow As Integer) As Boolean
    Public Declare Auto Function GetConsoleWindow Lib "kernel32.dll" () As IntPtr
    Public Declare Auto Function SetCursorPos Lib "user32.dll" (x As Integer, y As Integer) As Boolean
    Public Const SW_HIDE As Integer = 0
    Public Const SW_SHOW As Integer = 1

    Public Shared keysDown(500) As Boolean
    Public Shared dVr(5) As Int16 ' stores data of boolean varabiles in a int16

    Public Shared x, y, z As Double
    Shared moveAtX, moveAtY As Double
    Public Shared enableVirtical As Boolean = False

    Public Shared Sub load()
        'y = -5
    End Sub

    Public Shared Sub keyPressed(e As KeyPressEventArgs)
        gui.keyPressed(e)
    End Sub

    Public Shared Sub keyReleased(e As KeyboardKeyEventArgs)
        keysDown(e.ScanCode) = False

        'If e.ScanCode = 20 Then
        'If dVr(0) = 0 Then
        'dVr(0) = 1
        'Else
        'dVr(0) = 0
        'End If
        'camera.ShowWindow(camera.GetConsoleWindow(), dVr(0))
        '
        'End If

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
            y = -1
            z = 0
        End If

        If e.ScanCode = Key.F2 Then
            takePicture(Date.Now.Year & "-" & Date.Now.Month & "-" & Date.Now.Day & "-" & Date.Now.Hour & "-" & Date.Now.Minute & "-" & Date.Now.Second & "-" & Date.Now.Millisecond & ".png", ImageFormat.Png, 0, 0, Module1.app.Width, Module1.app.Height)
        End If

        If e.ScanCode = Key.F10 Then
            Throw New Exception("Illegal Button Press!")
        End If

        If e.ScanCode = Key.AltLeft Or e.ScanCode = Key.AltRight Or e.ScanCode = Key.WinLeft Or e.ScanCode = Key.WinRight Then
            settings.saveSettings()
        End If
    End Sub

    Public Shared Sub keyDown(e As KeyboardKeyEventArgs)
        keysDown(e.ScanCode) = True
        If isSpecialEnt() And keysDown(Key.F) Then
            If settings.flipRotate = 1 Then
                settings.flipRotate = -1
            Else
                settings.flipRotate = 1
            End If
            Console.WriteLine("Flip camera rotation: " & settings.flipRotate)
        End If
        If isSpecialEnt() And keysDown(Key.S) And keysDown(Key.Plus) Then
            settings.speed += 0.1D
            Console.WriteLine("Speed is now: " & settings.speed)
        End If
        If isSpecialEnt() And keysDown(Key.S) And keysDown(Key.Minus) Then
            settings.speed -= 0.1D
            Console.WriteLine("Speed is now: " & settings.speed)
        End If
        If isEntDwn() And keysDown(Key.S) Then
            world.saveEntities()
        End If
        If isSpecialEnt() And keysDown(Key.C) Then
            world.reload()
            Console.WriteLine("Reloading from file")
        End If
    End Sub

    Public Shared Sub update()
        If Not isUsrDwn() And Not isEntDwn() Then
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
                camera.y += settings.speed
            End If
            If keysDown(Key.Space) Then
                camera.y -= settings.speed
            End If
        End If

        If Not gui.isEscapeOpen Then
            Dim dx As Double = -(moveAtX * -Math.Sin(rai(Module1.GLTexturedCube.yaw))) + (moveAtY * Math.Cos(rai(Module1.GLTexturedCube.yaw)))
            If enableVirtical Then
                Dim dy As Double = (moveAtX * -Math.Sin(rai(Module1.GLTexturedCube.pitch)))
                camera.y += dy
            End If
            Dim dz As Double = (moveAtX * Math.Cos(rai(Module1.GLTexturedCube.yaw))) + (moveAtY * -Math.Sin(rai(Module1.GLTexturedCube.yaw)))

            camera.x += dx
            camera.z += dz
        End If

        If settings.useSkybox Then
            GL.BindTexture(TextureTarget.Texture2D, world.textures(500))
            artist.drawMesh(polys.cubeMesh)
        End If

        ' checks collion with the ground
        If camera.y >= -0.1 Then
            camera.y = -0.1
        End If

        ' translates the entire world according the the camera postion
        GL.Translate(camera.x, camera.y, camera.z)
        'scales the rest of the universe
        GL.Scale(settings.scale(0), settings.scale(1), settings.scale(2))
        ' i don't remeber what this does, but im going to leave it because it sounds important
        ' i think it has something to do with being something to store if the mouse is currently moving
        dVr(4) = 0

    End Sub

    ' convert to raidians
    Public Shared Function rai(degrees As Double) As Double
        Return degrees * Math.PI / 180
    End Function

    Public Shared Sub mouseDown(e As MouseButtonEventArgs)

    End Sub

    Public Shared Sub mouseUp(e As MouseButtonEventArgs)

    End Sub

    Public Shared Sub mouseMove(e As MouseMoveEventArgs)
        ' i don't think these are used but just global storage of the delta x and y of the mouse. 
        ' please ignore these
        dVr(2) = e.XDelta
        dVr(3) = e.YDelta

        ' if we don't have the escape menu open, then reset the mouse cursor to the center of the screen because OpenTk / VISUAL BASIC 
        ' SUCKS. This is also what causes the issues on windows 10 but THIS IS THE ONLY WAY OF DOING THIS. Try removing this line and you will understand
        ' how important this line of code is. While im on a visual basic rant, visual basic should not be taugh in schools, it sucks
        ' it does not have cross-platform support so that if i had a mac/linux computer i would not be able to run windows forms applications on it.
        ' the only thing that works on mac/linux is the console application and that only works because some guys on the internet made a compiler for linux.
        ' i run a linux computer and trying to get this to run has been a nightmare. good thing im not using windows forms applications.
        ' I should not have to use an OS that invades my privacy to pass a course... /rant
        If gui.isEscapeOpen <> True Then
            SetCursorPos(DisplayDevice.Default.Width / 2, DisplayDevice.Default.Height / 2)
        End If
        dVr(4) = 1
    End Sub

    ' takes a screenshot
    Public Shared Sub takePicture(file As String, format As ImageFormat, x As Integer, y As Integer, w As Integer, h As Integer)
        ' ok explaining this is going to be hard :/
        ' flips the current renderering buffer so that the screen is not being rendered
        Module1.app.SwapBuffers()
        ' pixes taken from the screen
        Dim pixels(w * h * 4) As Byte
        'bitmap holder
        Dim bi As Bitmap = New Bitmap(w, h)
        ' the screen
        Dim rect As Rectangle = New Rectangle(x, y, bi.Width, bi.Height)

        ' sets pixel alignment to the 4 defined in the ^ pixels (DO NOT CHANGE)
        GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4)
        ' reads the pixels off the screen. DO NOT CHANGE THIS. stores them in the pixel buffer defined above
        GL.ReadPixels(x, y, w, h, OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, pixels)

        ' create a bitmap with the screen rect
        Dim bit As BitmapData = bi.LockBits(rect, ImageLockMode.ReadWrite, bi.PixelFormat)
        ' copys pixels into the bitmap
        System.Runtime.InteropServices.Marshal.Copy(pixels, 0, bit.Scan0, pixels.Length())
        ' unlocks the bitmap
        bi.UnlockBits(bit)
        ' this is needed because of alignment issues (openGl takes from one rotation, while bitmaps have another so i use this function to realign them)
        bi.RotateFlip(RotateFlipType.RotateNoneFlipY)
        ' save to a file
        bi.Save("screenshots/" & file, ImageFormat.Png)
        ' make it so that the user can see the screen again
        Module1.app.SwapBuffers()
    End Sub

    Public Shared Sub mouseWheel(e As MouseWheelEventArgs)

    End Sub

    ' used to check if running a user command
    Public Shared Function isUsrDwn() As Boolean
        Dim [return] As Boolean = False

        If keysDown(Key.WinLeft) Then
            [return] = True
        End If

        Return [return]
    End Function

    ' used to check if running a special user command. (Usually used to change player settings)
    Public Shared Function isSpecialUsr() As Boolean
        Dim [return] As Boolean = False

        If keysDown(Key.WinLeft) And keysDown(Key.LShift) Then
            [return] = True
        End If

        Return [return]
    End Function

    ' used to check if mod is down(entity key)
    Public Shared Function isEntDwn() As Boolean
        Dim [return] As Boolean = False

        If keysDown(Key.AltLeft) Then
            [return] = True
        End If

        Return [return]
    End Function

    ' used to check if a special mod is down(special entity function key)
    Public Shared Function isSpecialEnt() As Boolean
        Dim [return] As Boolean = False

        If keysDown(Key.AltLeft) And keysDown(Key.LShift) Then
            [return] = True
        End If

        Return [return]
    End Function

End Class
