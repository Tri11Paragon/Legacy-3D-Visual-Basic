Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports OpenTK.Input

Public Class shader_test
    Inherits GameWindow

    Dim width As Integer = 800
    Dim height As Integer = 600
    Protected textures(50) As Integer

    Public Sub New()
        MyBase.New(800, 600, GraphicsMode.Default, "RMS", GameWindowFlags.Default, DisplayDevice.Default, 3, 2, GraphicsContextFlags.ForwardCompatible)
    End Sub

    Public verticies As Single() = {' positions   //   colors
        0.5, -0.5, 0.0, 1.0, 0.0, 0.0,   ' bottom right
        -0.5, -0.5, 0.0, 0.0, 1.0, 0.0,   ' bottom left
        0.0, 0.5, 0.0, 0.0, 0.0, 1.0    ' top
    }

    Dim vbo = 0
    Dim vao = 0
    Dim shader As mainShader

    Protected Sub LoadTexture(ByVal textureId As Integer, ByVal filename As String)
        'Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + 
        Dim bmp As New Bitmap("textures/" + filename)

        Dim data As BitmapData = bmp.LockBits(New Rectangle(0, 0, bmp.Width, bmp.Height),
                                            System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                            System.Drawing.Imaging.PixelFormat.Format32bppArgb)

        GL.BindTexture(TextureTarget.Texture2D, textureId)
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp.Width, bmp.Height, 0, OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0)

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


        'camera.ShowWindow(camera.GetConsoleWindow(), camera.SW_HIDE)
        'camera.load()

        LoadTextures()

        shader = New mainShader("main.vert", "main.frag")

        shader.use()

        GL.GenBuffers(1, vbo)
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo)
        GL.BufferData(BufferTarget.ArrayBuffer, verticies.Length * 4, verticies, BufferUsageHint.StaticDraw)
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0)


        GL.GenVertexArrays(1, vao)
        GL.BindVertexArray(vao)
        GL.BindBuffer(BufferTarget.ArrayBuffer, vao)
        GL.EnableVertexAttribArray(0) ' layout zero in the shader code
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, False, 6 * 4, 0)
        GL.EnableVertexAttribArray(1)
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, False, 6 * 4, 3 * 4)
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0)
        GL.BindVertexArray(0)

    End Sub

    Protected Overrides Sub OnResize(ByVal e As System.EventArgs)
        MyBase.OnResize(e)

        Me.width = MyBase.Width
        Me.height = MyBase.Height
        Console.WriteLine(width)

        GL.Viewport(0, 0, Me.Width, Me.Height)

        Dim aspect As Single = CSng(Me.Width) / Me.Height
        Dim projMat As Matrix4 = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect, 0.1, 1000.0)

        'GL.MatrixMode(MatrixMode.Projection)
        'GL.LoadMatrix(projMat)

        'GL.MatrixMode(MatrixMode.Modelview)
        'GL.LoadIdentity()
    End Sub

    Protected Overrides Sub OnRenderFrame(ByVal e As OpenTK.FrameEventArgs)
        MyBase.OnRenderFrame(e)
        GL.Clear(ClearBufferMask.ColorBufferBit Or ClearBufferMask.DepthBufferBit)

        shader.use()
        GL.BindVertexArray(vao)
        GL.DrawArrays(PrimitiveType.Triangles, 0, 3)

        SwapBuffers()
    End Sub

    Protected Overrides Sub OnClosing(ByVal e As System.ComponentModel.CancelEventArgs)
        shader.cleanup()
    End Sub

End Class

Public Class mainShader
    Inherits shader

    Private location_projectionMatrix As Integer
    Private location_transformationMatrix As Integer
    Private location_viewMatrix As Integer

    Public Sub New(vert As String, frag As String)
        MyBase.New(vert, frag)
    End Sub

    Public Overrides Sub bindAttributes()
        bindAttribute(0, "position")
        bindAttribute(1, "textureCoordinates")
        bindAttribute(2, "normal")
    End Sub

    Public Overrides Sub getAllUniformLocations()
        location_projectionMatrix = MyBase.getUniformLocation("projectionMatrix")
        location_transformationMatrix = MyBase.getUniformLocation("transformationMatrix")
        location_viewMatrix = MyBase.getUniformLocation("viewMatrix")
    End Sub


End Class

Public Class shader

    Private programID As Integer
    Private vertexID As Integer
    Private fragmentID As Integer

    Public Sub New(vert As String, frag As String)
        vertexID = Me.loadShaderData(vert, ShaderType.VertexShader)
        fragmentID = Me.loadShaderData(frag, ShaderType.FragmentShader)
        programID = Me.loadProgram(vertexID, fragmentID)
        getAllUniformLocations()
    End Sub

    Public Function getUniformLocation(name As String) As Integer
        Return GL.GetUniformLocation(programID, name)
    End Function

    Public Sub bindAttribute(attrib As Integer, varname As String)
        GL.BindAttribLocation(programID, attrib, varname)
    End Sub

    Public Sub use()
        GL.UseProgram(programID)
    End Sub

    Public Sub cleanup()
        GL.UseProgram(0)
        GL.DetachShader(programID, vertexID)
        GL.DetachShader(programID, fragmentID)
        GL.DeleteShader(vertexID)
        GL.DeleteShader(fragmentID)
        GL.DeleteProgram(programID)
    End Sub


    Public Sub loadInt(location As Integer, i As Integer)
        GL.Uniform1(location, i)
    End Sub

    Public Sub loadDouble(location As Integer, d As Double)
        GL.Uniform1(location, d)
    End Sub

    Public Sub loadFloat(location As Integer, f As Single)
        GL.Uniform1(location, f)
    End Sub

    Public Sub loadBoolean(location As Integer, b As Boolean)
        If b Then
            GL.Uniform1(location, 1)
        Else
            GL.Uniform1(location, 0)
        End If
    End Sub

    Public Sub loadMatrix(location As Integer, matrix As Matrix4d)
        GL.UniformMatrix4(location, False, matrix)
    End Sub

    Public Overridable Sub bindAttributes()

    End Sub

    Public Overridable Sub getAllUniformLocations()

    End Sub

    Public Function loadShaderData(shdr As String, type As ShaderType) As Integer
        Dim FS As New FileStream("shaders/" & shdr, FileMode.Open, FileAccess.Read)
        Dim cf As New StreamReader(FS)

        Dim str As String = ""

        str = cf.ReadToEnd()

        cf.Close()
        FS.Close()

        Dim shaderID = GL.CreateShader(type)
        GL.ShaderSource(shaderID, str)
        GL.CompileShader(shaderID)
        Dim shaderResult As Integer = 0
        GL.GetShader(shaderID, ShaderParameter.CompileStatus, shaderResult)
        If (shaderResult = 0) Then
            Console.WriteLine(GL.GetShaderInfoLog(shaderID))
        End If

        Console.WriteLine(str)

        Return shaderID

    End Function

    Public Function loadProgram(vert As Integer, frag As Integer) As Integer
        Dim programID = GL.CreateProgram()
        GL.AttachShader(programID, vert)
        GL.AttachShader(programID, frag)
        bindAttributes()
        GL.LinkProgram(programID)
        GL.ValidateProgram(programID)

        Dim suf As Integer
        GL.GetProgram(programID, GetProgramParameterName.ValidateStatus, suf)
        If suf = 0 Then
            Console.WriteLine(GL.GetProgramInfoLog(programID))
        End If

        Return programID
    End Function

    Public Function getProgramID() As Integer
        Return programID
    End Function

    Public Function getVertexID() As Integer
        Return vertexID
    End Function

    Public Function getFragmentID() As Integer
        Return fragmentID
    End Function

End Class