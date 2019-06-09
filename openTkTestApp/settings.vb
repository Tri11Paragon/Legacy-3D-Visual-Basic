Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports OpenTK.Input


Public Class settings

    Public Shared useSkybox As Boolean = True
    Public Shared flipRotate As Boolean = False
    Public Shared scale As Double() = {1.0, 1.0, 1.0}
    Public Shared speed As Double = 0.2
    Public Shared sensitivity = 0.75

    ' loads settings from file
    Public Shared Sub loadSettings()
        Dim FS As New FileStream("data/settings.dat", FileMode.Open, FileAccess.Read)
        Dim cf As New StreamReader(FS)

        'read the file
        Do While cf.Peek <> -1
            Dim line As String = o_helper.fn_1293(cf.ReadLine())
            If Not line.Equals("%") Then ' remove comments
                Dim s = line.Split(":") ' split at :
                If s(0).StartsWith("useSkybox") Then ' following stuff just loads settings
                    useSkybox = Boolean.Parse(s(1))
                End If
                If s(0).StartsWith("flipRotate") Then
                    flipRotate = Boolean.Parse(s(1))
                End If
                If s(0).StartsWith("scale") Then
                    Dim e = s(1).Split(" ")
                    scale(0) = Double.Parse(e(0))
                    scale(1) = Double.Parse(e(1))
                    scale(2) = Double.Parse(e(2))
                End If
                If s(0).StartsWith("speed") Then
                    speed = Double.Parse(s(1))
                End If
                If s(0).StartsWith("sensitivity") Then
                    sensitivity = Double.Parse(s(1))
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

        cf.Close()
        FS.Close()
    End Sub

End Class
