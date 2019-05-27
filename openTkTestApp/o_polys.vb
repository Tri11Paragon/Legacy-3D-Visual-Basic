Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports OpenTK.Input

Public Class o_polys

    Public Shared cube(23) As touple(Of Vector3, Vector2)
    Public Shared dd As Mesh

    Private Shared Sub loadCube()
        cube = loadFromFile("cube").ToArray()
    End Sub

    Public Shared Sub loadPolys()
        For i As Int16 = 0 To cube.Length - 1
            cube(i) = New touple(Of Vector3, Vector2)()
        Next
        loadCube()
        dd = Load("primitives/cubeie.obj")
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

    'this is my function
    Public Function heyGetOBJ(p As String) As Mesh
        Dim vertices As List(Of Vector3) = New List(Of Vector3)()
        Dim textureVertices As List(Of Vector3) = New List(Of Vector3)()
        Dim normals As List(Of Vector3) = New List(Of Vector3)()
        Dim vertexIndices As List(Of Integer) = New List(Of Integer)()
        Dim textureIndices As List(Of Integer) = New List(Of Integer)()
        Dim normalIndices As List(Of Integer) = New List(Of Integer)()

        If Not File.Exists(p) Then
            Throw New FileNotFoundException("Unable to open " & p & ", does not exist.")
        End If

        Dim FS As New FileStream("primitives/" & p & ".obj", FileMode.Open, FileAccess.Read)
        Dim cf As New StreamReader(FS)

        Do While cf.Peek <> -1
            Dim line As String = o_helper.fn_1293(cf.ReadLine())
            If Not line.Equals("skip") Then

                If line.StartsWith("v") Then
                    Dim textData() = line.Split(" ")
                    vertices.Add(New Vector3(Val(textData(1)), Val(textData(2)), Val(textData(3))))
                End If

            End If
        Loop


        cf.Close()
        FS.Close()

    End Function

    'basic Gl for rendering. Does not work with my engine?
    'GL.EnableClientState(ArrayCap.VertexArray)
    'Dim meshVertices = o_polys.dd.vertices.ToArray()
    '   GL.VertexPointer(3, VertexPointerType.Float, 0, meshVertices)
    ' Dim meshVertexIndices = o_polys.dd.vertexIndices.ToArray()
    '    GL.DrawElements(PrimitiveType.Triangles, o_polys.dd.vertexIndices.Count, DrawElementsType.UnsignedInt, meshVertexIndices)
    '    GL.DisableClientState(ArrayCap.VertexArray)

    ' this is credited to https://github.com/dabbertorres/ObjRenderer
    ' i could have made this, as i have made this in java: 
    ' (This is my obj converter for java) https://github.com/Tri11Paragon/3DJavaGame/tree/master/3d%20game/3D%20Game/src/objConverter
    ' using this as a reference as it does not work :(
    Public Shared Function Load(ByVal path As String) As Mesh
        Dim vertices As List(Of Vector3) = New List(Of Vector3)()
        Dim textureVertices As List(Of Vector2) = New List(Of Vector2)()
        Dim normals As List(Of Vector3) = New List(Of Vector3)()
        Dim vertexIndices As List(Of Integer) = New List(Of Integer)()
        Dim textureIndices As List(Of Integer) = New List(Of Integer)()
        Dim normalIndices As List(Of Integer) = New List(Of Integer)()

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

                Console.WriteLine(words.Count)

                Select Case type
                    Case "v"
                        'vertices.Add(New Vector3(Single.Parse(words(1)), Single.Parse(words(2)), Single.Parse(words(3))))
                    Case "vt"
                        'textureVertices.Add(New Vector2(Single.Parse(words(1)), Single.Parse(words(2))))
                    Case "vn"
                        'normals.Add(New Vector3(Single.Parse(words(1)), Single.Parse(words(2)), Single.Parse(words(3))))
                    Case "f"

                        For Each w As String In words
                            If w.Length = 0 Then Continue For
                            Dim comps As String() = w.Split("/")
                            vertexIndices.Add(UInteger.Parse(comps(0)) - 1)
                            If comps.Length > 1 AndAlso comps(1).Length <> 0 Then textureIndices.Add(UInteger.Parse(comps(1)) - 1)
                            If comps.Length > 2 Then normalIndices.Add(UInteger.Parse(comps(2)) - 1)
                        Next

                    Case Else
                End Select
            End While
        End Using

        Return New Mesh(vertices, textureVertices, normals, vertexIndices, textureIndices, normalIndices)
    End Function

    Public Class Mesh
        Public ReadOnly vertices As List(Of Vector3)
        Public ReadOnly textureVertices As List(Of Vector2)
        Public ReadOnly normals As List(Of Vector3)
        Public ReadOnly vertexIndices As List(Of Integer)
        Public ReadOnly textureIndices As List(Of Integer)
        Public ReadOnly normalIndices As List(Of Integer)

        Public Sub New(ByVal vertices As List(Of Vector3), ByVal textureVertices As List(Of Vector2), ByVal normals As List(Of Vector3), ByVal vertexIndices As List(Of Integer), ByVal textureIndices As List(Of Integer), ByVal normalIndices As List(Of Integer))
            Me.vertices = vertices
            Me.textureVertices = textureVertices
            Me.normals = normals
            Me.vertexIndices = vertexIndices
            Me.textureIndices = textureIndices
            Me.normalIndices = normalIndices

        End Sub
    End Class


End Class
