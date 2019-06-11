﻿Imports OpenTK
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
        loadMusic()
    End Sub

    Public Shared Sub update()
        GL.BindTexture(TextureTarget.Texture2D, textures(13))
        artist.drawMesh(polys.terrainMesh)
        For Each d As entity In world.entites
            d.update()
            d.draw()
        Next
        GL.BindTexture(TextureTarget.Texture2D, 0)
        'If (20 - 1) * Rnd() = 1 Then
        'soundPlayer.SoundLocation = musics(((musics.Count - 1) - 1) * Rnd())
        'soundPlayer.Load()
        'soundPlayer.Play()
        'End If
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
            Dim msc = musics(CType(((musics.Count - 1) - 1) * Rnd(), Integer))
            Console.WriteLine("Now playing: " & msc)
            My.Computer.Audio.Play(msc, AudioPlayMode.WaitToComplete)
            Thread.Sleep(1000)
        Loop
    End Sub


End Class
