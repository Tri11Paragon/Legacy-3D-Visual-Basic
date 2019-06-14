Option Explicit On

Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports OpenTK.Input

Module Module1

    'Private Declare Function GetTickCount& Lib "kernel32" ()
    Public app As New GLTexturedCube

    Public Sub Main()
        app.Run(60, 60)
    End Sub

    Public Class GLTexturedCube
        Inherits GameWindow

        Public Sub New()
            MyBase.New(800, 600, New GraphicsMode(New ColorFormat(24, 24, 24, 0), 24, 8, 4), "RMS", GameWindowFlags.FixedWindow, DisplayDevice.Default, 4, 0, GraphicsContextFlags.ForwardCompatible)

        End Sub

        Protected Sub LoadTexture(ByVal textureId As Integer, ByVal path As String)
            'Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + 
            Dim bmp As New Bitmap(path)

            Dim data As BitmapData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb)

            GL.BindTexture(TextureTarget.Texture2D, textureId)
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp.Width, bmp.Height, 0, OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0)

            bmp.UnlockBits(data)
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureMinFilter.Linear)
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureMagFilter.Linear)
            Console.WriteLine("Texture " & System.IO.Path.GetFileName(path) & " has been loaded at: " & textureId)
        End Sub

        Protected Sub LoadTextures()
            Console.WriteLine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName))
            GL.GenTextures(world.textures.Length, world.textures)
            Dim textures As List(Of String) = New List(Of String)
            For Each file As String In My.Computer.FileSystem.GetFiles("textures/")
                textures.Add(file)
            Next
            If textures.Count > 498 Then
                textures.RemoveRange(498, textures.Count - 498)
            End If
            For i As Integer = 0 To textures.Count - 1
                LoadTexture(world.textures(i), textures(i)) ' loads the found files into a texture buffer
            Next
            LoadTexture(world.textures(499), "textures/grass.png")
            LoadTexture(world.textures(500), "textures/skybox/cube.png")
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            settings.loadSettings()
            GL.ClearColor(0.5, 0.5, 0.6, 0)
            GL.Enable(EnableCap.DepthTest)
            GL.Enable(EnableCap.Texture2D)
            GL.Enable(EnableCap.Blend)
            GL.Enable(EnableCap.Multisample)
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha)

            polys.loadPolys()

            GL.Viewport(0, 0, Me.Width, Me.Height)

            Dim aspect As Single = CSng(Me.Width) / Me.Height
            Dim projMat As Matrix4 = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect, 0.1, 1000.0)

            GL.MatrixMode(MatrixMode.Projection)
            GL.LoadMatrix(projMat)

            GL.MatrixMode(MatrixMode.Modelview)
            GL.LoadIdentity()

            'camera.ShowWindow(camera.GetConsoleWindow(), camera.SW_HIDE)
            MyBase.CursorVisible = False
            camera.load()

            LoadTextures()
            world.create()
            gui.create()
        End Sub

        Protected Overrides Sub OnDisposed(e As EventArgs)
            world.saveEntities()
            gui.unload()
            settings.saveSettings()
            MyBase.OnDisposed(e)
        End Sub

        Protected Overrides Sub OnClosed(e As EventArgs)
            world.saveEntities()
            gui.unload()
            settings.saveSettings()
            MyBase.OnClosed(e)
        End Sub

        Protected Overrides Sub OnUnload(e As EventArgs)
            world.saveEntities()
            gui.unload()
            settings.saveSettings()
            MyBase.OnUnload(e)
        End Sub

        Protected Overrides Sub OnResize(ByVal e As System.EventArgs)
            MyBase.OnResize(e)

            GL.Viewport(0, 0, Me.Width, Me.Height)

            Dim aspect As Single = CSng(Me.Width) / Me.Height
            Dim projMat As Matrix4 = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect, 0.1, 1000.0)

            GL.MatrixMode(MatrixMode.Projection)
            GL.LoadMatrix(projMat)

            GL.MatrixMode(MatrixMode.Modelview)
            GL.LoadIdentity()
        End Sub

        Public Shared pitch As Double = 0
        Public Shared yaw As Double = 0

        Protected Overrides Sub OnRenderFrame(ByVal e As OpenTK.FrameEventArgs)
            MyBase.OnRenderFrame(e)
            MyBase.CursorVisible = gui.isEscapeOpen
            GL.Clear(ClearBufferMask.ColorBufferBit Or ClearBufferMask.DepthBufferBit)

            GL.LoadIdentity()
            gui.render()
            clock.update()

            If camera.dVr(4) And Not gui.isEscapeOpen Then
                yaw += camera.dVr(2) * 0.5 * settings.sensitivity * settings.flipRotate
                pitch += camera.dVr(3) * 0.3 * settings.sensitivity * settings.flipRotate
            End If

            If (-pitch > 90) Then
                pitch = -90
            End If

            If (-pitch < -90) Then
                pitch = 90
            End If

            If (yaw > 360) Then
                yaw = 0
            End If

            GL.Rotate(-pitch, 1, 0, 0)
            GL.Rotate(-yaw, 0, 1, 0)


            camera.update()

            world.update()

            SwapBuffers()
        End Sub

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
