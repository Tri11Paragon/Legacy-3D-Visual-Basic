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
    Public Shared textures(500) As Integer
    Public Shared deathChances As Dictionary(Of Integer, Double) = New Dictionary(Of Integer, Double)
    Public Shared birthChances As Dictionary(Of Integer, Double) = New Dictionary(Of Integer, Double)

    Shared soundPlayer As SoundPlayer
    Public Shared musics As List(Of String) = New List(Of String)

    Public Shared Sub create()
        Randomize()
        loadEntites()
        loadMusic()
    End Sub

    Public Shared Sub update()
        GL.BindTexture(TextureTarget.Texture2D, textures(499))
        artist.drawMesh(polys.terrainMesh)
        GL.BindTexture(TextureTarget.Texture2D, 0)
        For Each d As entity In world.entites
            d.update()
            d.draw()
        Next
    End Sub

    ' find the clostest entity to a position
    Public Shared Function findClosestEntity(x As Double, y As Double, z As Double) As entity
        Dim ent As entity = entites(0) ' return entity
        Dim lastDistance As Double = maths.distanceD(ent.getPosition, New Vector3(x, y, z)) ' last shortest distance

        For Each e As entity In entites
            Dim dist = maths.distanceD(e.getPosition(), New Vector3(x, y, z)) ' get the distance between this entity and the pos we are finding
            If dist < lastDistance Then ' check to see if the entity distance is less then this distance
                lastDistance = dist ' set this as the last distance
                ent = e ' set the return entity to this found entity
            End If
        Next

        Return ent
    End Function

    Public Shared Sub reload()
        entites = New List(Of entity)
        loadEntites()
    End Sub

    Public Shared Sub loadEntites()
        Try ' catches erros
            ' loads the file into memory
            Dim FS As New FileStream("data/world.dat", FileMode.Open, FileAccess.Read)
            Dim cf As New StreamReader(FS)

            ' tells the program if its already reading an entity
            Dim enityStarted As Boolean = False
            Dim traitStarted As Boolean = False

            Dim ents As Dictionary(Of String, Mesh) = New Dictionary(Of String, Mesh)
            Dim pos As New Vector3(0, 0, 0)
            Dim rotation As Double = 0
            Dim velocity As New Vector3d(0, 0, 0)
            Dim acceleration As New Vector3d(0, 0, 0)
            Dim texture As Integer = 0
            Dim mesh As String = "pig.obj"
            Dim birth As Double = 15
            Dim death As Double = 10

            Dim traitChance As Double = 0
            Dim traitTexture As Integer = 0
            Dim traitType As Boolean = False

            'read the file
            Do While cf.Peek <> -1
                Dim line As String = o_helper.fn_1293(cf.ReadLine())
                If Not line.Equals("%") Then ' remove comments
                    If line.StartsWith("{") And enityStarted Then
                        Console.WriteLine("Error loading trait! INVALID FORMAT!")
                    End If
                    If line.StartsWith("{") And Not enityStarted Then
                        enityStarted = True
                    End If
                    If enityStarted And Not line.StartsWith("{") And Not line.StartsWith("}") Then
                        ' splits to setup format for the interpreter below
                        Dim tmp = line.Split(":")
                        ' splits spaces (after each space should be a variable)
                        Dim spaces As List(Of String) = New List(Of String)(tmp(1).Split(" "))

                        ' this removes spaces to make parsing easier
                        If String.IsNullOrWhiteSpace(spaces(0)) Or String.IsNullOrEmpty(spaces(0)) Then
                            spaces.RemoveAt(0)
                        End If

                        ' these explain themselves. find if the current line stats with "x" if it does, then assigen "x"'s corrasponding variable
                        If line.StartsWith("position:") Then
                            pos = New Vector3(Double.Parse(spaces(0)), Double.Parse(spaces(1)), Double.Parse(spaces(2)))
                        End If
                        If line.StartsWith("rotation:") Then
                            rotation = Double.Parse(spaces(0))
                        End If
                        If line.StartsWith("velocity:") Then
                            velocity = New Vector3d(Double.Parse(spaces(0)), Double.Parse(spaces(1)), Double.Parse(spaces(2)))
                        End If
                        If line.StartsWith("acceleration:") Then
                            acceleration = New Vector3d(Double.Parse(spaces(0)), Double.Parse(spaces(1)), Double.Parse(spaces(2)))
                        End If
                        If line.StartsWith("texture:") Then
                            texture = Integer.Parse(spaces(0))
                        End If
                        If line.StartsWith("mesh:") Then
                            mesh = spaces(0)
                        End If
                        If line.StartsWith("birthChance:") Then
                            birth = Double.Parse(spaces(0))
                        End If
                        If line.StartsWith("deathChance:") Then
                            death = Double.Parse(spaces(0))
                        End If
                    End If
                    ' ends the current entity then spawns in the world
                    If line.StartsWith("}") And enityStarted Then
                        Console.WriteLine("Loading Enity!")
                        ' error out because no mesh has been found
                        If String.IsNullOrEmpty(mesh) Then
                            Throw New Exception("MESH NAME NOT FOUND!")
                        End If
                        Dim m As Mesh

                        ' try to load up the entity model
                        If Not ents.TryGetValue(mesh, m) Then
                            m = polys.BLoad("primitives/" & mesh)
                            ents.Add(mesh, m)
                        End If

                        Dim entity As New entity(m, texture, pos.X, pos.Y, pos.Z)
                        ' apply all loaded variables
                        entity.setRotation(rotation)
                        entity.setVelocity(velocity)
                        entity.setAcceleration(acceleration)
                        entity.deathChance = death
                        entity.birthChance = birth

                        ' add entity to world
                        entites.Add(entity)
                        enityStarted = False

                        'reset the values of the loader
                        pos = New Vector3(0, 0, 0)
                        rotation = 0
                        velocity = New Vector3d(0, 0, 0)
                        acceleration = New Vector3d(0, 0, 0)
                        texture = 0
                        mesh = "pig.obj"
                        death = 10
                        birth = 15
                    End If
                    If line.StartsWith("(") And traitStarted Then
                        Console.WriteLine("Error loading trait! INVALID FORMAT!")
                    End If
                    If line.StartsWith("(") And Not traitStarted Then
                        traitStarted = True
                    End If
                    If traitStarted And Not line.StartsWith("(") And Not line.StartsWith(")") Then
                        ' splits to setup format for the interpreter below
                        Dim tmp = line.Split(":")
                        ' splits spaces (after each space should be a variable)
                        Dim spaces As List(Of String) = New List(Of String)(tmp(1).Split(" "))

                        ' this removes spaces to make parsing easier
                        If String.IsNullOrWhiteSpace(spaces(0)) Or String.IsNullOrEmpty(spaces(0)) Then
                            spaces.RemoveAt(0)
                        End If

                        ' again, these just load variables from each line into the game. nothing special
                        If line.StartsWith("traitChance:") Then
                            traitChance = Double.Parse(spaces(0))
                        End If
                        If line.StartsWith("traitTexture:") Then
                            traitTexture = Double.Parse(spaces(0))
                        End If
                        If line.StartsWith("traitType:") Then
                            If spaces(0).ToLower.StartsWith("birth") Then
                                traitType = False
                            End If
                            If spaces(0).ToLower.StartsWith("death") Then
                                traitType = True
                            End If
                        End If
                    End If
                    If line.StartsWith(")") Then
                        traitStarted = False ' we have reached end
                        ' loads the traits
                        If traitType Then
                            deathChances.Add(traitTexture, traitChance)
                        Else
                            birthChances.Add(traitTexture, traitChance)
                        End If
                        ' reset the traits
                        traitChance = 0
                        traitTexture = 0
                    End If
                End If
            Loop

            cf.Close()
            FS.Close()
        Catch e As FileNotFoundException
            ' world file not found :/
            Console.WriteLine("failed to load world!")
        End Try
    End Sub

    Public Shared Sub saveEntities()
        Dim FS As New FileStream("data/world.dat", FileMode.Create, FileAccess.ReadWrite)
        Dim cf As New StreamWriter(FS)

        ' saves death chances to a file
        For Each k In deathChances
            cf.WriteLine("(")
            cf.WriteLine("traitChance:" & k.Value)
            cf.WriteLine("traitTexture:" & k.Key)
            cf.WriteLine("traitType: death")
            cf.WriteLine(")")
        Next
        ' saves birth chances to the world file
        For Each k In birthChances
            cf.WriteLine("(")
            cf.WriteLine("traitChance:" & k.Value)
            cf.WriteLine("traitTexture:" & k.Key)
            cf.WriteLine("traitType: birth")
            cf.WriteLine(")")
        Next

        'saves enttites to file
        For Each e As entity In entites
            cf.WriteLine("{") ' these explain themselves
            cf.WriteLine("position: " & e.getX() & " " & e.getY() & " " & e.getZ())
            cf.WriteLine("rotation: " & e.getRotation())
            cf.WriteLine("velocity: " & e.getVelocity().X & " " & e.getVelocity().Y & " " & e.getVelocity().Z)
            cf.WriteLine("acceleration: " & e.getAcceleration().X & " " & e.getAcceleration().Y & " " & e.getAcceleration().Z)
            cf.WriteLine("texture: " & e.getTexture())
            cf.WriteLine("mesh: " & e.getMesh().name)
            cf.WriteLine("birthChance: " & e.birthChance)
            cf.WriteLine("deathChance: " & e.deathChance)
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
            Console.WriteLine("Music found: " & System.IO.Path.GetFileName(file))
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
            Dim msc = musics(CType(Math.Round(((musics.Count - 1) - 1) * Rnd(), 0), Integer))
            Console.WriteLine("Now playing: " & msc)
            My.Computer.Audio.Play(msc, AudioPlayMode.WaitToComplete)
            Thread.Sleep(1000)
        Loop
    End Sub


End Class
