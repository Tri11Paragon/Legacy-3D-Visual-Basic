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
' camera class used to do camera/player things
Public Class camera

    ' these are windows function which i wish i did not need to use but have to use :(
    ' idk what this one does or if i still use it
    Public Declare Auto Function ShowWindow Lib "user32.dll" (ByVal hWnd As IntPtr, ByVal nCmdShow As Integer) As Boolean
    ' gets the state of the console windows (not used anymore i think)
    Public Declare Auto Function GetConsoleWindow Lib "kernel32.dll" () As IntPtr
    ' sets the cursor pos based on the screen
    Public Declare Auto Function SetCursorPos Lib "user32.dll" (x As Integer, y As Integer) As Boolean
    ' these are not used
    Public Const SW_HIDE As Integer = 0
    Public Const SW_SHOW As Integer = 1

    ' stores data on the info about the keyboard state (so it can be accessed by other classes)
    Public Shared keysDown(500) As Boolean
    Public Shared dVr(5) As Int16 ' stores data of boolean varabiles in a int16

    ' Position of the camera
    Public Shared x, y, z As Double
    ' how much to move in a direction (x/z)
    Shared moveAtX, moveAtY As Double
    ' enable virtual movement when looking up (DO NOT USE. THE MATH DOES NOT WORK)
    Public Shared enableVirtical As Boolean = False

    ' not currently used
    Public Shared Sub load()
        'y = -5
    End Sub

    ' registers keypresses on the keyboard
    Public Shared Sub keyPressed(e As KeyPressEventArgs)
        gui.keyPressed(e)
    End Sub

    ' called when key is released
    Public Shared Sub keyReleased(e As KeyboardKeyEventArgs)
        ' tell the game that we are not longer pressing on this key
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

        '  not used and i don't remember what this was used for (im not going to remove it incase it causes an issue)
        If e.ScanCode = 19 Then
            If dVr(1) = 0 Then
                dVr(1) = 1
            Else
                dVr(1) = 0
            End If
        End If

        ' not used
        If dVr(1) And dVr(0) Then
            'Console.WriteLine(e.ScanCode)
        End If

        ' unlocks mouse when key is pressed
        If e.ScanCode = Key.Escape Then
            Console.WriteLine("Unlocking mouse on program!")
            gui.isEscapeOpen = Not gui.isEscapeOpen
            'Process.GetCurrentProcess().CloseMainWindow()
        End If

        ' resets the position to the center of the world
        If e.ScanCode = Key.F12 Then
            x = 0
            y = -1
            z = 0
        End If

        ' takes a screenshot
        If e.ScanCode = Key.F2 Then
            takePicture(Date.Now.Year & "-" & Date.Now.Month & "-" & Date.Now.Day & "-" & Date.Now.Hour & "-" & Date.Now.Minute & "-" & Date.Now.Second & "-" & Date.Now.Millisecond & ".png", ImageFormat.Png, 0, 0, Module1.app.Width, Module1.app.Height)
        End If

        ' was used to test memory
        If e.ScanCode = Key.F10 Then
            'Throw New Exception("Illegal Button Press!")
        End If

        ' this does not work and is no longer needed ( was used to save settings ) 
        If e.ScanCode = Key.AltLeft Or e.ScanCode = Key.AltRight Or e.ScanCode = Key.WinLeft Or e.ScanCode = Key.WinRight Then
            settings.saveSettings()
        End If
    End Sub

    ' called when keys are down
    Public Shared Sub keyDown(e As KeyboardKeyEventArgs)
        'tells game that key is down
        keysDown(e.ScanCode) = True
        ' flips rotateion when shift alt f is pressed
        If isSpecialEnt() And keysDown(Key.F) Then
            If settings.flipRotate = 1 Then
                settings.flipRotate = -1
            Else
                settings.flipRotate = 1
            End If
            Console.WriteLine("Flip camera rotation: " & settings.flipRotate)
        End If
        ' increases speed when shift alt plus is pressed
        If isSpecialEnt() And keysDown(Key.S) And keysDown(Key.Plus) Then
            settings.speed += 0.1D
            Console.WriteLine("Speed is now: " & settings.speed)
        End If
        ' decreases speed when shift alt minus is pressed
        If isSpecialEnt() And keysDown(Key.S) And keysDown(Key.Minus) Then
            settings.speed -= 0.1D
            Console.WriteLine("Speed is now: " & settings.speed)
        End If
        ' saves the world entitys when alt s is pressed (not need but is nice)
        If isEntDwn() And keysDown(Key.S) Then
            world.saveEntities()
        End If
        ' reloads the entites from file when shift alt c is pressed
        If isSpecialEnt() And keysDown(Key.C) Then
            world.reload()
            Console.WriteLine("Reloading from file")
        End If
    End Sub

    ' updates camera
    Public Shared Sub update()
        ' checks to make sure we are not pressing special keys
        If Not isUsrDwn() And Not isEntDwn() Then
            ' moves forward if w is pressed
            If keysDown(Key.W) Then
                moveAtX = settings.speed
            Else
                moveAtX = 0
            End If
            ' moves backwards if s is pressed
            If keysDown(Key.S) Then
                moveAtX = -settings.speed
            ElseIf Not keysDown(Key.W) Then
                moveAtX = 0
            End If
            ' moves left if a is pressed
            If keysDown(Key.A) Then
                moveAtY = settings.speed
            Else
                moveAtY = 0
            End If
            ' moves right if d is pressed
            If keysDown(Key.D) Then
                moveAtY = -settings.speed
            ElseIf Not keysDown(Key.A) Then
                moveAtY = 0
            End If
            ' moves camera down if shift is pressed
            If keysDown(Key.LShift) Then
                camera.y += settings.speed
            End If
            ' moves camera up if shift is pressed
            If keysDown(Key.Space) Then
                camera.y -= settings.speed
            End If
        End If

        ' moves the camera if escape is not open
        If Not gui.isEscapeOpen Then
            ' does the math required to move in the x direction. This was a pain to make.
            Dim dx As Double = -(moveAtX * -Math.Sin(rai(Module1.GLTexturedCube.yaw))) + (moveAtY * Math.Cos(rai(Module1.GLTexturedCube.yaw)))
            ' DOES NOT WORK. (only the math, it does work but like it does not move correctly)
            If enableVirtical Then
                Dim dy As Double = (moveAtX * -Math.Sin(rai(Module1.GLTexturedCube.pitch)))
                camera.y += dy
            End If
            ' does the math required to move in the z direction. This was a pain to make.
            Dim dz As Double = (moveAtX * Math.Cos(rai(Module1.GLTexturedCube.yaw))) + (moveAtY * -Math.Sin(rai(Module1.GLTexturedCube.yaw)))

            ' move the camera based on the math
            camera.x += dx
            camera.z += dz
        End If

        ' renders the skybox if the user wants to
        If settings.useSkybox Then
            ' binds texture to the active texture buffer inside the internal shader
            GL.BindTexture(TextureTarget.Texture2D, world.textures(500))
            ' draws the cube
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

    ' these are not used and explain themselves
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
