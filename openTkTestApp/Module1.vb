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

        Protected angle As Single

        Public Sub New()
            MyBase.New(800, 600, New GraphicsMode(New ColorFormat(8, 8, 8, 0), 24, 8, 4), "RMS", GameWindowFlags.FixedWindow, DisplayDevice.Default, 4, 0, GraphicsContextFlags.ForwardCompatible)

        End Sub

        Protected Sub LoadTexture(ByVal textureId As Integer, ByVal filename As String)
            'Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + 
            Dim bmp As New Bitmap("textures/" + filename)

            Dim data As BitmapData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height),
                                                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                System.Drawing.Imaging.PixelFormat.Format32bppArgb)

            GL.BindTexture(TextureTarget.Texture2D, textureId)
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                      bmp.Width, bmp.Height, 0, OpenGL.PixelFormat.Bgra,
                      PixelType.UnsignedByte, data.Scan0)

            bmp.UnlockBits(data)
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureMinFilter.Linear)
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureMagFilter.Linear)
        End Sub

        Protected Sub LoadTexture(ByVal textureId As Integer, ByVal filename As String, cubemap As Integer)
            'Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + 
            Dim bmp As New Bitmap("textures/" + filename)

            Dim data As BitmapData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height),
                                            System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                            System.Drawing.Imaging.PixelFormat.Format32bppArgb)

            GL.BindTexture(TextureTarget.Texture2D, textureId)
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                  bmp.Width, bmp.Height, 0, OpenGL.PixelFormat.Bgra,
                  PixelType.UnsignedByte, data.Scan0)

            bmp.UnlockBits(data)
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, TextureMinFilter.Linear)
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, TextureMagFilter.Linear)
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, TextureWrapMode.ClampToEdge)
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, TextureWrapMode.ClampToEdge)
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, TextureWrapMode.ClampToEdge)

        End Sub

        Protected Sub LoadTextures()
            Console.WriteLine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName))
            GL.GenTextures(world.textures.Length, world.textures)
            LoadTexture(world.textures(0), "rms.png")
            LoadTexture(world.textures(1), "emoji_code.png")
            LoadTexture(world.textures(2), "gilmie.jpg")
            LoadTexture(world.textures(3), "goldfish_car.jpg")
            LoadTexture(world.textures(4), "icon.png")
            LoadTexture(world.textures(5), "lunx.png")
            LoadTexture(world.textures(6), "http___i.huffpost.com_gen_5334752_images_n-GIANT-DUCK-628x314.jpg")
            LoadTexture(world.textures(7), "Tux_Mono.svg.png")
            LoadTexture(world.textures(8), "download.jpg")
            LoadTexture(world.textures(9), "l3lmzb0a2xw21.jpg")
            LoadTexture(world.textures(10), "ab4pjx3bnww21.jpg")
            LoadTexture(world.textures(11), "pig.png")
            LoadTexture(world.textures(12), "skybox/cube.png")
            LoadTexture(world.textures(100), "skybox/back.png")
            LoadTexture(world.textures(101), "skybox/bottom.png")
            LoadTexture(world.textures(102), "skybox/front.png")
            LoadTexture(world.textures(103), "skybox/left.png")
            LoadTexture(world.textures(104), "skybox/right.png")
            LoadTexture(world.textures(105), "skybox/top.png")
            LoadTexture(world.textures(106), "skybox/nightBack.png")
            LoadTexture(world.textures(107), "skybox/nightBottom.png")
            LoadTexture(world.textures(108), "skybox/nightFront.png")
            LoadTexture(world.textures(109), "skybox/nightLeft.png")
            LoadTexture(world.textures(110), "skybox/nightRight.png")
            LoadTexture(world.textures(111), "skybox/nightTop.png")
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
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

            camera.ShowWindow(camera.GetConsoleWindow(), camera.SW_HIDE)
            MyBase.CursorVisible = False
            camera.load()

            LoadTextures()

            world.entites.Add(New entity(polys.pigMesh, world.textures(0), 0, 0, 5))
            world.entites.Add(New entity(polys.pigMesh, world.textures(1), 0, 5, 0))
            world.entites.Add(New entity(polys.pigMesh, world.textures(2), 0, 0, 0))
            world.entites.Add(New entity(polys.pigMesh, world.textures(3), 0, 0, -5))
            world.entites.Add(New entity(polys.pigMesh, world.textures(4), 5, 5, 0))
            world.entites.Add(New entity(polys.pigMesh, world.textures(5), 5, 5, 0))
            world.entites.Add(New entity(polys.pigMesh, world.textures(6), 5, 5, -5))
            world.entites.Add(New entity(polys.pigMesh, world.textures(7), 5, 0, 0))
            world.entites.Add(New entity(polys.pigMesh, world.textures(8), 5, -5, 0))
            world.entites.Add(New entity(polys.pigMesh, world.textures(9), -5, -5, 5))
            world.entites.Add(New entity(polys.pigMesh, world.textures(10), 5, -5, 5))
            world.entites.Add(New entity(polys.pigMesh, world.textures(11), 5, -5, 5))

            gui.create()

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
            MyBase.CursorVisible = camera.dVr(0)
            GL.Clear(ClearBufferMask.ColorBufferBit Or ClearBufferMask.DepthBufferBit)

            GL.LoadIdentity()
            gui.render()

            If camera.dVr(4) And Not gui.isEscapeOpen Then
                yaw += camera.dVr(2) * 0.5 * settings.sensitivity
                pitch += camera.dVr(3) * 0.3 * settings.sensitivity
            End If

            If (-pitch > 90) Then
                pitch = 90
            End If

            If (-pitch < -90) Then
                pitch = -90
            End If

            GL.Rotate(-pitch, 1, 0, 0)
            GL.Rotate(-yaw, 0, 1, 0)

            camera.update()

            angle += 1.5

            artist.drawMesh(polys.mouseMesh, world.textures(11), 10, 10, 0)

            For Each d As entity In world.entites
                d.update()
                d.draw()
            Next

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
