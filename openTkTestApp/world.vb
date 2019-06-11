Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports OpenTK.Input
Imports System.Media
Imports System.Threading

Public Class world

    Public Shared entites As List(Of entity) = New List(Of entity)
    Public Shared textures(125) As Integer

    Shared soundPlayer As SoundPlayer
    Public Shared musics As List(Of String) = New List(Of String)

    Public Shared Sub create()
        Randomize()
        'For i As Integer = 0 To size
        '    For j As Integer = 0 To size ' (cap - 1) * Rnd()
        '        v.Add(New Vector3(i * spaceBetweenPoints, i + j, j * spaceBetweenPoints))
        '    Next
        'Next
        loadEntites()
        loadMusic()
    End Sub

    Public Shared Sub update()
        GL.BindTexture(TextureTarget.Texture2D, textures(13))
        artist.drawMesh(polys.terrainMesh)
        GL.BindTexture(TextureTarget.Texture2D, 0)
        For Each d As entity In world.entites
            d.update()
            d.draw()
        Next
        'If (20 - 1) * Rnd() = 1
    End Sub

    Public Shared Sub loadEntites()
        Try ' catches erros
            ' loads the file into memory
            Dim FS As New FileStream("data/settings.dat", FileMode.Open, FileAccess.Read)
            Dim cf As New StreamReader(FS)

            ' tells the program if its already reading an entity
            Dim enityStarted As Boolean = False

            'read the file
            Do While cf.Peek <> -1
                Dim line As String = o_helper.fn_1293(cf.ReadLine())
                If Not line.Equals("%") Then ' remove comments
                    If line.StartsWith("{") Then

                    End If
                End If
            Loop

            cf.Close()
            FS.Close()
        Catch e As FileNotFoundException
            Console.WriteLine("failed to load world!")
        End Try
    End Sub

    Public Shared Sub saveEntities()
        Dim FS As New FileStream("data/world.dat", FileMode.Create, FileAccess.ReadWrite)
        Dim cf As New StreamWriter(FS)

        'saves to file
        For Each e As entity In entites
            cf.WriteLine("{") ' these explain themselves
            cf.WriteLine("position:" & e.getX() & " " & e.getY() & " " & e.getZ())
            cf.WriteLine("rotation:" & e.getRotation())
            cf.WriteLine("velocity:" & e.getVelocity().X & " " & e.getVelocity().Y & " " & e.getVelocity().Z)
            cf.WriteLine("texture:" & e.getTexture())
            cf.WriteLine("mesh:" & e.getMesh().name)
            cf.WriteLine("}")
        Next

        cf.Close()
        FS.Close()
    End Sub

    'loops through the array of vertices and checks to see if any vertex is closest and that must be the height
    ' (does not work)
    Public Shared Function getHeight(x As Double, z As Double) As Double
        Dim h As Double = 0
        For Each v In polys.terrainMesh.vertices
            If (Math.Floor(v.X) = Math.Floor(x)) Then
                If (Math.Floor(v.Z) = Math.Floor(z)) Then
                    h = v.Y
                End If
            End If
        Next
        Return h
    End Function

    ' sound player stuff came from https://docs.microsoft.com/en-us/dotnet/api/system.media.soundplayer?view=netframework-4.8
    Public Shared Sub loadMusic() ' https://stackoverflow.com/questions/31814130/play-sounds-in-a-visual-studio-application
        For Each file As String In My.Computer.FileSystem.GetFiles("music/")
            musics.Add(file)
            Console.WriteLine("Music found: " & file)
        Next
        Dim thread As Thread = New Thread(AddressOf playMusic)
        thread.IsBackground = True
        thread.Start()
        'My.Computer.Audio.Play(musics(0), AudioPlayMode.WaitToComplete)
    End Sub

    ' plays music
    Public Shared Sub playMusic()
        Randomize()
        Do
            ' loads the music, tells the user what it is and then waits.
            Dim msc = musics(CType(((musics.Count - 1) - 1) * Rnd(), Integer))
            Console.WriteLine("Now playing: " & msc)
            My.Computer.Audio.Play(msc, AudioPlayMode.WaitToComplete)
            Thread.Sleep(1000)
        Loop
    End Sub


End Class
