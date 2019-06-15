Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports OpenTK.Input


Public Class settings

    Public Shared useSkybox As Boolean = True ' use skybox
    Public Shared flipRotate As Integer = 1 ' flip how the camera rotates (makes going left go right and going up down) (this is needed on windows 10 because of an unknown error with OPENTK)
    Public Shared scale As Double() = {1.0, 1.0, 1.0} ' scale of the universe
    Public Shared speed As Double = 0.2 ' speed of the camera
    Public Shared sensitivity = 0.75 ' how fast the camera should rotate when mouse moved
    Public Shared entityInterval As Long = 1000 ' how long before running birth/death checks

    ' loads settings from file
    Public Shared Sub loadSettings()
        Dim FS As New FileStream("data/settings.dat", FileMode.Open, FileAccess.Read)
        Dim cf As New StreamReader(FS)

        'read the file
        Do While cf.Peek <> -1
            Dim line As String = o_helper.fn_1293(cf.ReadLine())
            If Not line.Equals("%") Then ' remove comments
                Dim s = line.Split(":")
                Dim spaces As List(Of String) = New List(Of String)(s(1).Split(" "))
                If String.IsNullOrWhiteSpace(spaces(0)) Or String.IsNullOrEmpty(spaces(0)) Then
                    spaces.RemoveAt(0)
                End If
                If s(0).StartsWith("useSkybox") Then ' following stuff just loads settings
                    useSkybox = Boolean.Parse(spaces(0))
                End If
                If s(0).StartsWith("flipRotate") Then
                    flipRotate = Integer.Parse(spaces(0))
                End If
                If s(0).StartsWith("scale") Then
                    scale(0) = Double.Parse(spaces(0))
                    scale(1) = Double.Parse(spaces(1))
                    scale(2) = Double.Parse(spaces(2))
                End If
                If s(0).StartsWith("speed") Then
                    speed = Double.Parse(spaces(0))
                End If
                If s(0).StartsWith("sensitivity") Then
                    sensitivity = Double.Parse(spaces(0))
                End If
                If s(0).StartsWith("cameraPosition") Then
                    camera.x = Double.Parse(spaces(0))
                    camera.y = Double.Parse(spaces(1))
                    camera.z = Double.Parse(spaces(2))
                End If
                If s(0).StartsWith("cameraRotation") Then
                    Module1.GLTexturedCube.pitch = Double.Parse(spaces(0))
                    Module1.GLTexturedCube.yaw = Double.Parse(spaces(1))
                End If
                If s(0).StartsWith("entityInterval") Then
                    entityInterval = Long.Parse(spaces(0))
                End If
            End If
        Loop

        cf.Close()
        FS.Close()
    End Sub

    ' saves the settings
    Public Shared Sub saveSettings()
        Dim FS As New FileStream("data/settings.dat", FileMode.Create, FileAccess.ReadWrite)
        Dim cf As New StreamWriter(FS)

        'adds to file
        cf.WriteLine("useSkybox:" & useSkybox)
        cf.WriteLine("flipRotate:" & flipRotate)
        cf.WriteLine("scale:" & scale(0) & " " & scale(1) & " " & scale(2))
        cf.WriteLine("speed:" & speed)
        cf.WriteLine("sensitivity:" & sensitivity)
        cf.WriteLine("cameraPosition:" & camera.x & " " & camera.y & " " & camera.z)
        cf.WriteLine("cameraRotation:" & Module1.GLTexturedCube.pitch & " " & -Module1.GLTexturedCube.yaw)
        cf.WriteLine("entityInterval:" & entityInterval)

        cf.Close()
        FS.Close()
    End Sub

End Class
