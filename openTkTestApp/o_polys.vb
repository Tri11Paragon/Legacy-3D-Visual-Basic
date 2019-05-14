Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports OpenTK.Input

Public Class o_polys

    Public Shared cube(23) As touple(Of Vector3, Vector2)

    Private Shared Sub loadCube()
        cube = loadFromFile("cube").ToArray()
    End Sub

    Public Shared Sub loadPolys()
        For i As Int16 = 0 To cube.Length - 1
            cube(i) = New touple(Of Vector3, Vector2)()
        Next
        loadCube()

    End Sub

    Public Shared Function loadFromFile(primitiveName As String) As List(Of touple(Of Vector3, Vector2))
        Dim lst As List(Of touple(Of Vector3, Vector2)) = New List(Of touple(Of Vector3, Vector2))

        Dim FS As New FileStream("primitives/" & primitiveName & ".txt", FileMode.Open, FileAccess.Read)
        Dim cf As New StreamReader(FS)

        Dim pos As Int16 = 0

        Console.WriteLine(cf.Peek)

        Do While cf.Peek <> -1
            Dim line As String = o_helper.fn_1293(cf.ReadLine())
            If Not line.Equals("skip") Then
                Dim texData As String = ""
                Dim vertData As String = ""
                Dim spl As String() = o_helper.removeSpaces(line.Split("|"))
                If spl(0).StartsWith("t:") Then
                    texData = spl(0).Split(":")(1)
                End If
                If spl(1).StartsWith("v:") Then
                    vertData = spl(1).Split(":")(1)
                End If

                Dim txtPosData = o_helper.removeSpaces(texData.Split(","))
                Dim vertPosData = o_helper.removeSpaces(vertData.Split(","))

                lst.Add(New touple(Of Vector3, Vector2)(New Vector3(Val(vertPosData(0)), Val(vertPosData(1)), Val(vertPosData(2))), New Vector2(Val(txtPosData(0)), Val(txtPosData(1)))))

                pos += 1
            End If
        Loop

        cf.Close()
        FS.Close()

        Return lst
    End Function

End Class
