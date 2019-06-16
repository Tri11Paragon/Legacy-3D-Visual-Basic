Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports OpenTK.Input
Imports System.Media
Imports System.Threading

' Brett Terpstra
' 2019-06-16
' final project
' world class that loads entites and the music for the game
' is the biggest in the game
Public Class world

    Public Shared entites As List(Of entity) = New List(Of entity) ' list of all the entities in the world
    Public Shared textures(500) As Integer ' array of all possible textures
    ' these contain loaded instances of the death/birth chances
    Public Shared deathChances As Dictionary(Of Integer, Double) = New Dictionary(Of Integer, Double)
    Public Shared birthChances As Dictionary(Of Integer, Double) = New Dictionary(Of Integer, Double)

    ' this is used to prevent crashes ( NOT CURRENTLY USED )
    Private Shared dyingEntites As New List(Of entity)
    Private Shared birthingEntites As New List(Of entity)

    Shared soundPlayer As SoundPlayer ' the sound player
    Public Shared musics As List(Of String) = New List(Of String) ' list of paths to the musics in the musics folder

    ' create all things needed for the world
    Public Shared Sub create()
        Randomize() ' RANDOMNESS
        loadEntites() ' load entities from a file
        loadMusic() ' load music from a file
    End Sub

    ' updates stuff
    Public Shared Sub update()
        ' draw the terrain
        GL.BindTexture(TextureTarget.Texture2D, textures(499))
        artist.drawMesh(polys.terrainMesh)
        GL.BindTexture(TextureTarget.Texture2D, 0)
        ' loop through the entties, then draw and update the entites
        For i As Integer = 0 To entites.Count - 1
            Try
                ' draw
                entites(i).draw()
                ' update
                entites(i).update()
            Catch e As Exception

            End Try
        Next
        'Next
    End Sub

    ' THESE ARE NOT USED
    Public Shared Sub addEntity(e As entity)
        birthingEntites.Add(e)
    End Sub

    Public Shared Sub removeEntity(e As entity)
        dyingEntites.Add(e)
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

    ' reload the entties with shift + alt + c
    Public Shared Sub reload()
        entites = New List(Of entity)
        loadEntites()
    End Sub

    ' loads entites from the world file
    Public Shared Sub loadEntites()
        Try ' catches erros
            ' loads the file into memory
            Dim FS As New FileStream("data/world.dat", FileMode.Open, FileAccess.Read)
            Dim cf As New StreamReader(FS)

            ' tells the program if its already reading an entity
            Dim enityStarted As Boolean = False
            Dim traitStarted As Boolean = False

            ' these are just pre-defined to allow for creation of entites. They explain themselves
            Dim entsMeshs As Dictionary(Of String, Mesh) = New Dictionary(Of String, Mesh)
            Dim pos As New Vector3(0, 0, 0)
            Dim rotation As Double = 0
            Dim velocity As New Vector3d(0, 0, 0)
            Dim acceleration As New Vector3d(0, 0, 0)
            Dim texture As Integer = 0
            Dim mesh As String = "pig.obj"
            Dim birth As Double = 15
            Dim death As Double = 10

            ' this is for traits . they explain themselves
            Dim traitChance As Double = 0
            Dim traitTexture As Integer = 0
            Dim traitType As Boolean = False

            'read the file
            Do While cf.Peek <> -1
                ' reads the line and then filters out comments / unwanted stuff (comments don't get resaved so they are useless but i have this here just in case)
                Dim line As String = o_helper.fn_1293(cf.ReadLine())
                If Not line.Equals("%") Then ' remove comments
                    If line.StartsWith("{") And enityStarted Then ' checks to make sure that the user did not make an error. Tells the user
                        Console.WriteLine("Error loading trait! INVALID FORMAT!")
                    End If
                    ' checks if we are starting entity, then tell interpreter that we have started entity
                    If line.StartsWith("{") And Not enityStarted Then
                        enityStarted = True
                    End If
                    ' do all the entity loading stuff
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
                            If birth = 0 Then ' helps prevent an issue with 0 birth?
                                birth += 0.1
                            End If
                        End If
                        If line.StartsWith("deathChance:") Then
                            death = Double.Parse(spaces(0))
                            If death = 0 Then ' helps prevent an issue with 0 death
                                death += 0.1
                            End If
                        End If
                    End If
                    ' ends the current entity then spawns in the world
                    If line.StartsWith("}") And enityStarted Then
                        Console.WriteLine("Loading Enity!")
                        ' error out because no mesh has been found
                        If String.IsNullOrEmpty(mesh) Then
                            Throw New Exception("MESH NAME NOT FOUND!")
                        End If
                        ' mesh to be loaded / got from the dictonary
                        Dim m As Mesh

                        ' try to load up the entity model
                        If Not entsMeshs.TryGetValue(mesh, m) Then
                            m = polys.BLoad("primitives/" & mesh)
                            entsMeshs.Add(mesh, m)
                        End If

                        ' create a new entity instance
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
                    'checks if invalid format
                    If line.StartsWith("(") And traitStarted Then
                        Console.WriteLine("Error loading trait! INVALID FORMAT!")
                    End If
                    ' start the trait loading.
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

            ' close the file stream
            cf.Close()
            FS.Close()
        Catch e As FileNotFoundException
            ' world file not found :/
            Console.WriteLine("failed to load world!")
        End Try
    End Sub

    Public Shared Sub saveEntities()
        ' load the file into a save buffer
        Dim FS As New FileStream("data/world.dat", FileMode.Create, FileAccess.ReadWrite)
        Dim cf As New StreamWriter(FS)

        ' saves death chances to a file
        For Each k In deathChances
            ' writes all the death chances (in proper format)
            cf.WriteLine("(")
            cf.WriteLine("traitChance:" & k.Value)
            cf.WriteLine("traitTexture:" & k.Key)
            cf.WriteLine("traitType: death")
            cf.WriteLine(")")
        Next
        ' saves birth chances to the world file
        For Each k In birthChances
            ' writes all the birth chances (in the proper format)
            cf.WriteLine("(")
            cf.WriteLine("traitChance:" & k.Value)
            cf.WriteLine("traitTexture:" & k.Key)
            cf.WriteLine("traitType: birth")
            cf.WriteLine(")")
        Next

        'saves enttites to file
        For Each e As entity In entites
            ' saves the entity data with proper format
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

        ' close the save buffer
        cf.Close()
        FS.Close()
    End Sub

    'loops through the array of vertices and checks to see if any vertex is closest and that must be the height
    ' (does not work) / IS NOT USED (IGNORE THIS)
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
        'prevents loading no music
        If musics.Count = 0 Then
            Return
        End If
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
