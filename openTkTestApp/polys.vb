Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports OpenTK.Input

Public Class polys

    Public Shared mouseMesh As Mesh
    Public Shared pigMesh As Mesh
    Public Shared terrainMesh As Mesh
    Public Shared cubeMesh As Mesh
    Public Shared face As Mesh
    Public Shared face90 As Mesh
    Public Shared tree As Mesh
    Public Shared tree1 As Mesh

    Public Shared Sub loadPolys()
        mouseMesh = BLoad("primitives/bever.obj")
        pigMesh = BLoad("primitives/pig.obj")
        terrainMesh = BLoad("primitives/terrain1.obj")
        cubeMesh = BLoad("primitives/cube.obj")
        face = BLoad("primitives/face.obj")
        face90 = BLoad("primitives/face.obj")
        tree = BLoad("primitives/tree.obj")
        tree1 = BLoad("primitives/tree1.obj")
    End Sub

    ' this is credited to https://github.com/dabbertorres/ObjRenderer
    ' i could have made this, as i have made this in java: 
    ' (This is my obj converter for java) https://github.com/Tri11Paragon/3DJavaGame/tree/master/3d%20game/3D%20Game/src/objConverter
    ' using this as a reference as it does not work :(


    ' i made this one. ( i copied ^ and modified it)
    ' to hopefully work
    Public Shared Function BLoad(ByVal path As String) As Mesh
        Dim verts As List(Of Vector3) = New List(Of Vector3)
        Dim vertices As List(Of Vector3) = New List(Of Vector3)()
        Dim texts As List(Of Vector2) = New List(Of Vector2)
        Dim textureVertices As List(Of Vector2) = New List(Of Vector2)()
        Dim norms As List(Of Vector3) = New List(Of Vector3)
        Dim normals As List(Of Vector3) = New List(Of Vector3)()

        If Not File.Exists(path) Then
            Throw New FileNotFoundException("Unable to open " & path & ", does not exist.")
        End If

        Using streamReader As StreamReader = New StreamReader(path)

            While Not streamReader.EndOfStream
                Dim words As List(Of String) = New List(Of String)(streamReader.ReadLine().ToLower().Split(" "))
                words.RemoveAll(Function(s) s = String.Empty)
                If words.Count = 0 Then Continue While
                Dim type As String = words(0)
                words.RemoveAt(0)

                'Console.WriteLine(words.Count)

                Select Case type
                    Case "v"
                        verts.Add(New Vector3(Single.Parse(words(0)), Single.Parse(words(1)), Single.Parse(words(2))))
                    Case "vt"
                        texts.Add(New Vector2(Single.Parse(words(0)), Single.Parse(words(1))))
                    Case "vn"
                        norms.Add(New Vector3(Single.Parse(words(0)), Single.Parse(words(1)), Single.Parse(words(2))))
                    Case "f"

                        For Each w As String In words
                            'Console.WriteLine(w)
                            If w.Length = 0 Then Continue For
                            Dim comps As String() = w.Split("/")
                            vertices.Add(verts(UInteger.Parse(comps(0)) - 1))
                            If comps.Length > 1 AndAlso comps(1).Length <> 0 Then textureVertices.Add(texts(UInteger.Parse(comps(1)) - 1))
                            If comps.Length > 2 Then normals.Add(norms(UInteger.Parse(comps(2)) - 1))
                        Next

                    Case Else
                End Select
            End While
        End Using

        Return New Mesh(vertices, textureVertices, normals)
    End Function

    ' useless function
    Public Shared Function loadFromFile(primitiveName As String) As List(Of touple(Of Vector3, Vector2))
        Dim lst As List(Of touple(Of Vector3, Vector2)) = New List(Of touple(Of Vector3, Vector2))

        Dim FS As New FileStream("primitives/" & primitiveName & ".txt", FileMode.Open, FileAccess.Read)
        Dim cf As New StreamReader(FS)

        Dim pos As Int16 = 0

        Console.WriteLine(cf.Peek)

        Do While cf.Peek <> -1
            Dim line As String = o_helper.fn_1293(cf.ReadLine())
            If Not line.Equals("%") Then
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

Public Class Mesh
    Public ReadOnly vertices As List(Of Vector3)
    Public ReadOnly textureVertices As List(Of Vector2)
    Public ReadOnly normals As List(Of Vector3)

    Public Sub New(ByVal vertices As List(Of Vector3), ByVal textureVertices As List(Of Vector2), ByVal normals As List(Of Vector3))
        Me.vertices = vertices
        Me.textureVertices = textureVertices
        Me.normals = normals
    End Sub
End Class