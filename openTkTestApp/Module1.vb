﻿Option Explicit On

Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports OpenTK.Input

Module Module1

    Private Declare Function GetTickCount& Lib "kernel32" ()

    Public Sub Main()
        'Dim app As New GLTexturedCube
        'app.Run(60, 60)
        Dim app As New shader_test
        app.Run(60, 60)
    End Sub

    Public Class GLTexturedCube
        Inherits GameWindow

        Protected angle As Single
        Protected textures(50) As Integer

        Public Sub New()
            MyBase.New(800, 600, GraphicsMode.Default, "RMS")
        End Sub

        'Protected Overrides Sub OnKeyPress(OpenTK.KeyPressEventArgs e)

        'End Sub

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

        Protected Sub LoadTextures()
            Console.WriteLine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName))
            GL.GenTextures(textures.Length, textures)
            LoadTexture(textures(0), "rms.png")
            LoadTexture(textures(1), "emoji_code.png")
            LoadTexture(textures(2), "gilmie.jpg")
            LoadTexture(textures(3), "goldfish_car.jpg")
            LoadTexture(textures(4), "icon.png")
            LoadTexture(textures(5), "lunx.png")
            LoadTexture(textures(6), "http___i.huffpost.com_gen_5334752_images_n-GIANT-DUCK-628x314.jpg")
            LoadTexture(textures(7), "Tux_Mono.svg.png")
            LoadTexture(textures(8), "download.jpg")
            LoadTexture(textures(9), "l3lmzb0a2xw21.jpg")
            LoadTexture(textures(10), "ab4pjx3bnww21.jpg")
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            GL.ClearColor(0.5, 0.5, 0.6, 0)
            GL.Enable(EnableCap.DepthTest)
            GL.Enable(EnableCap.Texture2D)
            GL.Enable(EnableCap.Blend)
            GL.Enable(EnableCap.Multisample)
            'GL.Enable(EnableCap.CullFace)
            'GL.CullFace(CullFaceMode.Front)
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha)

            polys.loadPolys()

            camera.ShowWindow(camera.GetConsoleWindow(), camera.SW_HIDE)
            camera.load()

            LoadTextures()

            entites.Add(New entity(textures(0), 0, 0, 5))
            entites.Add(New entity(textures(1), 0, 5, 0))
            entites.Add(New entity(textures(2), 0, 0, 0))
            entites.Add(New entity(textures(3), 0, 0, -5))
            entites.Add(New entity(textures(4), 5, 5, 0))
            entites.Add(New entity(textures(5), 5, 5, 0))
            entites.Add(New entity(textures(6), 5, 5, -5))
            entites.Add(New entity(textures(7), 5, 0, 0))
            entites.Add(New entity(textures(8), 5, -5, 0))
            entites.Add(New entity(textures(9), -5, -5, 5))
            entites.Add(New entity(textures(10), 5, -5, 5))

        End Sub

        Protected Overrides Sub OnResize(ByVal e As System.EventArgs)
            MyBase.OnResize(e)

            GL.Viewport(0, 0, Me.Width, Me.Height)

            Dim aspect As Single = CSng(Me.Width) / Me.Height
            Dim projMat As Matrix4 = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect, 0.1, 100.0)

            GL.MatrixMode(MatrixMode.Projection)
            GL.LoadMatrix(projMat)

            GL.MatrixMode(MatrixMode.Modelview)
            GL.LoadIdentity()
        End Sub

        Public Shared sensitivity = 0.75
        Public Shared pitch As Double = 0
        Public Shared yaw As Double = 0

        Public Shared entites As List(Of entity) = New List(Of entity)

        Dim start As Long

        Protected Overrides Sub OnRenderFrame(ByVal e As OpenTK.FrameEventArgs)
            MyBase.OnRenderFrame(e)
            start = GetTickCount()
            GL.Clear(ClearBufferMask.ColorBufferBit Or ClearBufferMask.DepthBufferBit)

            GL.LoadIdentity()

            If camera.dVr(4) Then
                yaw += camera.dVr(2) * 0.5 * sensitivity
                pitch += camera.dVr(3) * 0.3 * sensitivity
            End If

            If (pitch > 90) Then
                pitch = 90
            End If

            If (pitch < -90) Then
                pitch = -90
            End If

            GL.Rotate(-pitch, 1, 0, 0)
            GL.Rotate(-yaw, 0, 1, 0)

            camera.update()

            angle += 1.5

            'GL.Enable(EnableCap.CullFace)
            artist.drawTriangle(textures(7), -5, 0, 5)

            'GL.Disable(EnableCap.CullFace)
            For Each d As entity In entites
                d.update()
                d.draw()
            Next

            SwapBuffers()
            'Console.WriteLine(GetTickCount() - start)
        End Sub

        Public Sub bindRotation()
            GL.Rotate(angle, 0, 0.1, 0.1)
            GL.Rotate(angle, 0, 1, 0)
            GL.Rotate(angle, 0.1, 0, 0)
            GL.Rotate(angle, 0, 0.1, 0)
            GL.Rotate(angle, 0, 0, 0.1)
        End Sub

        Public Sub unbindRotation()
            GL.Rotate(-angle, 0, 0.1, 0.1)
            GL.Rotate(-angle, 0, 1, 0)
            GL.Rotate(-angle, 0.1, 0, 0)
            GL.Rotate(-angle, 0, 0.1, 0)
            GL.Rotate(-angle, 0, 0, 0.1)
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
