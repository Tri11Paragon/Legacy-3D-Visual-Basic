Option Explicit On

Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports OpenTK.Input

' Brett Terpstra
' 2019-06-16
' final project
' main module for the game
Module Module1

    'Private Declare Function GetTickCount& Lib "kernel32" ()
    'create the game as an app
    Public app As New GLTexturedCube

    Public Sub Main()
        ' starts the program with 60 fps and 60 updates per second
        app.Run(60, 60)
    End Sub

    Public Class GLTexturedCube
        Inherits GameWindow ' extends from the built-in game window

        Public Sub New()
            'creates the window with 24 bit color resolution and 4x anti-allisaing. Title is RMS. Fixed window size, default displace deivce and forward compatible opengl.
            ' MAKE SURE THE RESOLUTION IS in 4:3 it will work on other but some things will not look right
            MyBase.New(800, 600, New GraphicsMode(New ColorFormat(24, 24, 24, 0), 24, 8, 4), "RMS", GameWindowFlags.FixedWindow, DisplayDevice.Default, 2, 0, GraphicsContextFlags.ForwardCompatible)

        End Sub

        Protected Sub LoadTexture(ByVal textureId As Integer, ByVal path As String)
            'Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + 
            ' opens a bitmap
            Dim bmp As New Bitmap(path)

            ' locks the bitmap data into a bit buffer to be loaded into the texture buffers inside the game
            Dim data As BitmapData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb)

            ' bind to what texture id we are assigning
            GL.BindTexture(TextureTarget.Texture2D, textureId)
            ' generates and loads data into the texture buffer at the ID
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp.Width, bmp.Height, 0, OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0)

            ' unlock the bitmap data
            bmp.UnlockBits(data)
            ' tells opengl the texture parameters that are needed for proper texture renderering
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureMinFilter.Linear)
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureMagFilter.Linear)
            ' tells user that we have loaded texture and where
            Console.WriteLine("Texture " & System.IO.Path.GetFileName(path) & " has been loaded at: " & textureId)
        End Sub

        ' loads all the texutes
        Protected Sub LoadTextures()
            Console.WriteLine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName))
            ' generates the required texture array inside the shader
            GL.GenTextures(world.textures.Length, world.textures)
            ' list of texture paths inside the textures folder
            Dim textures As List(Of String) = New List(Of String)
            ' loads all texture paths inside the texture folders
            For Each file As String In My.Computer.FileSystem.GetFiles("textures/")
                textures.Add(file)
            Next
            ' make sure we don't have too many textures
            If textures.Count > 498 Then
                textures.RemoveRange(498, textures.Count - 498)
            End If
            ' load all the texute paths into the game
            For i As Integer = 0 To textures.Count - 1
                LoadTexture(world.textures(i), textures(i)) ' loads the found files into a texture buffer
            Next
            ' make sure that the grass and skybox textures are loaded.
            LoadTexture(world.textures(499), "textures/grass.png")
            LoadTexture(world.textures(500), "textures/skybox/cube.png")
        End Sub

        ' called when loaded game
        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            ' load all settings
            settings.loadSettings()
            ' setup background behind the skybox. (not needed anymore)
            GL.ClearColor(0.5, 0.5, 0.6, 0)
            ' enable depth testing
            GL.Enable(EnableCap.DepthTest)
            ' enable textures
            GL.Enable(EnableCap.Texture2D)
            ' enable blending
            GL.Enable(EnableCap.Blend)
            ' enable multisampling (FXAA)
            GL.Enable(EnableCap.Multisample)
            ' enable transparentcy
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha)

            ' load all polys that need to be loaded
            polys.loadPolys()

            ' create the viewing area on the screen
            GL.Viewport(0, 0, Me.Width, Me.Height)

            ' creates apsect ratio
            Dim aspect As Single = CSng(Me.Width) / Me.Height
            ' creates projection matrix
            Dim projMat As Matrix4 = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect, 0.1, 1000.0)

            ' loads the projection matrix
            GL.MatrixMode(MatrixMode.Projection)
            GL.LoadMatrix(projMat)

            ' sets to view matrix mode for rendering stuff
            GL.MatrixMode(MatrixMode.Modelview)
            ' load the matrix into the shader
            GL.LoadIdentity()

            'camera.ShowWindow(camera.GetConsoleWindow(), camera.SW_HIDE)
            ' makes cursor hidden
            MyBase.CursorVisible = False
            ' load camera stuff
            camera.load()

            ' load all textures
            LoadTextures()
            ' load all things inside the world class
            world.create()
            ' do gui creation
            gui.create()
        End Sub

        ' cleans up world and saves the entites and settings to a file
        ' called on exiting of the application
        Protected Overrides Sub OnDisposed(e As EventArgs)
            world.saveEntities()
            gui.unload()
            settings.saveSettings()
            MyBase.OnDisposed(e)
        End Sub

        ' cleans up world and saves the entites and settings to a file
        ' called on exiting of the application
        Protected Overrides Sub OnClosed(e As EventArgs)
            world.saveEntities()
            gui.unload()
            settings.saveSettings()
            MyBase.OnClosed(e)
        End Sub

        ' cleans up world and saves the entites and settings to a file
        ' called on exiting of the application
        Protected Overrides Sub OnUnload(e As EventArgs)
            world.saveEntities()
            gui.unload()
            settings.saveSettings()
            MyBase.OnUnload(e)
        End Sub

        ' called when resizing the game
        Protected Overrides Sub OnResize(ByVal e As System.EventArgs)
            MyBase.OnResize(e)

            ' create view out of new dimensions
            GL.Viewport(0, 0, Me.Width, Me.Height)

            ' create aspect ratios
            Dim aspect As Single = CSng(Me.Width) / Me.Height
            ' create projection matrix
            Dim projMat As Matrix4 = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect, 0.1, 1000.0)

            ' load projection matrix
            GL.MatrixMode(MatrixMode.Projection)
            GL.LoadMatrix(projMat)

            ' tells to set to view matrix mode
            GL.MatrixMode(MatrixMode.Modelview)
            GL.LoadIdentity()
        End Sub

        ' camera yaw and pitch
        Public Shared pitch As Double = 0
        Public Shared yaw As Double = 0

        ' called once per 16 milliseconds
        Protected Overrides Sub OnRenderFrame(ByVal e As OpenTK.FrameEventArgs)
            MyBase.OnRenderFrame(e)
            ' sets cursor visiable if the isescapeopen chances
            MyBase.CursorVisible = gui.isEscapeOpen
            'clear the screen with these params. 
            GL.Clear(ClearBufferMask.ColorBufferBit Or ClearBufferMask.DepthBufferBit)

            ' load the matrix
            GL.LoadIdentity()
            ' render the UI
            gui.render()
            ' update the clock
            clock.update()

            ' moves the camera if the mouse is in focus
            If camera.dVr(4) And Not gui.isEscapeOpen Then
                ' does math to figure out how much the mouse moved and how much to rotate
                ' the 0.5 and 0.3 were made to work well on the win 7 computers at school
                yaw += camera.dVr(2) * 0.5 * settings.sensitivity * settings.flipRotate
                pitch += camera.dVr(3) * 0.3 * settings.sensitivity * settings.flipRotate
            End If

            ' prevents looking more then directly down
            If (-pitch > 90) Then
                pitch = -90
            End If

            ' prevents looking more then direcly up
            If (-pitch < -90) Then
                pitch = 90
            End If

            ' prevents large numbers if we at 360 then we are basiclly 0 so set 0
            If (yaw >= 360) Then
                yaw = 0
            End If

            ' rotate the world based on the camera yaw/pitch
            GL.Rotate(-pitch, 1, 0, 0)
            GL.Rotate(-yaw, 0, 1, 0)

            ' update the camera
            camera.update()

            ' update the world
            world.update()

            ' swaps active buffer with the back buffer
            SwapBuffers()
        End Sub

        ' THESE HANDLE KEY/MOUSE STUFF. They explain themselves and call funcs inside cameras
        Private Sub GLTexturedCube_KeyDown(sender As Object, e As KeyboardKeyEventArgs) Handles Me.KeyDown
            camera.keyDown(e)
        End Sub

        Private Sub GLTexturedCube_KeyUp(sender As Object, e As KeyboardKeyEventArgs) Handles Me.KeyUp
            camera.keyReleased(e)
        End Sub

        Private Sub GLTexturedCube_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Me.KeyPress
            camera.keyPressed(e)
        End Sub

        Private Sub GLTexturedCube_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Me.MouseDown
            camera.mouseDown(e)
        End Sub

        Private Sub GLTexturedCube_MouseUp(sender As Object, e As MouseButtonEventArgs) Handles Me.MouseUp
            camera.mouseUp(e)
        End Sub

        Private Sub GLTexturedCube_MouseMove(sender As Object, e As MouseMoveEventArgs) Handles Me.MouseMove
            camera.mouseMove(e)
        End Sub

        Private Sub GLTexturedCube_MouseWheel(sender As Object, e As MouseWheelEventArgs) Handles Me.MouseWheel
            camera.mouseWheel(e)
        End Sub
    End Class

End Module
