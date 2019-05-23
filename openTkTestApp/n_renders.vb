Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System
Imports System.Text
Imports System.Runtime.InteropServices
Imports System.IO
Imports System.Media
Imports System.Windows
Imports System.Drawing
Imports System.Drawing.Imaging

Public Class Loader

    Dim vaos As ArrayList = New ArrayList()
    Dim vbos As ArrayList = New ArrayList()
    Public textures(100) As Integer

    Private Function decodeTextureFile(path As String) As TextureData
        Dim width As Integer = 0
        Dim height As Integer = 0
        Dim buffer As BufferArray(Of Byte)
        Try
            Dim img As Image = Image.FromFile(path)
            width = img.Width
            height = img.Height
            'buffer = New BufferArray(Of Byte)(imageUtils.imgToByteArray(img))

        Catch ex As Exception

        End Try

        Return New TextureData(buffer, width, height)
    End Function

    Private Function storeDataInFloatBuffer(data As Single()) As BufferArray(Of Single)
        Return New BufferArray(Of Single)(data)
    End Function

    Private Function storeDataInIntBuffer(data As Integer()) As BufferArray(Of Integer)
        Return New BufferArray(Of Integer)(data)
    End Function

    Private Sub bindIndicesBuffer(data As Integer())
        Dim vaoID As Int32 = GL.GenBuffer()
        vaos.Add(vaoID)
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, vaoID)
        GL.BufferData(BufferTarget.ElementArrayBuffer, data.Length, data, BufferUsageHint.StaticDraw)
    End Sub

    Private Sub unbindVAO()
        GL.BindVertexArray(0)
    End Sub

    Private Sub storeDataInAttributeList(attributeNumber As Int32, data As Single())
        Dim vboID As Integer = GL.GenBuffer()
        vbos.Add(vboID)
        GL.BindBuffer(BufferTarget.ArrayBuffer, vboID)
        GL.BufferData(BufferTarget.ArrayBuffer, data.Length, data, BufferUsageHint.StaticDraw)
        GL.VertexAttribPointer(attributeNumber, 3, VertexAttribPointerType.Float, False, 0, 0)
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0)
    End Sub

    Private Sub storeDataInAttributeList(attributeNumber As Int32, coordinateSize As Integer, data As Single())
        Dim vboID As Integer = GL.GenBuffer()
        vbos.Add(vboID)
        GL.BindBuffer(BufferTarget.ArrayBuffer, vboID)
        GL.BufferData(BufferTarget.ArrayBuffer, data.Length, data, BufferUsageHint.StaticDraw)
        GL.VertexAttribPointer(attributeNumber, coordinateSize, VertexAttribPointerType.Float, False, 0, 0)
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0)
    End Sub

    Private Function createVAO() As Integer
        Dim vaoID = GL.GenVertexArray()
        vaos.Add(vaoID)
        GL.BindVertexArray(vaoID)
        Return vaoID
    End Function

    Public Sub cleanUp()
        For num As Integer = 0 To vaos.Count
            GL.DeleteVertexArray(num)
        Next
        For num As Integer = 0 To vbos.Count
            GL.DeleteBuffer(num)
        Next
        For num As Integer = 0 To textures.Count
            GL.DeleteTexture(num)
        Next
        vaos = Nothing
        vbos = Nothing
        textures = Nothing
    End Sub

    Public Function loadToVAO(positions As Single()) As RawModel
        Dim vaoID As Integer = createVAO()
        Me.storeDataInAttributeList(0, positions)
        unbindVAO()
        Return New RawModel(vaoID, positions.Length / 3)
    End Function

    Public Function loadToVAO(positions As Single(), modlename As String) As RawModel
        Dim vaoID As Integer = createVAO()
        Me.storeDataInAttributeList(0, 2, positions)
        unbindVAO()
        Return New RawModel(vaoID, positions.Length / 2, modlename)
    End Function

    Public Function loadToVAO(positions As Single(), textureCoords As Single(), normals As Single(), indices As Integer()) As RawModel
        Dim vaoID As Integer = createVAO()
        bindIndicesBuffer(indices)
        Me.storeDataInAttributeList(0, 3, positions)
        Me.storeDataInAttributeList(1, 2, textureCoords)
        Me.storeDataInAttributeList(2, 3, normals)
        unbindVAO()
        Return New RawModel(vaoID, indices.Length)
    End Function

    Public Function loadToVAO(positions As Single(), textureCoords As Single(), normals As Single(), indices As Integer(), modelName As String) As RawModel
        Dim vaoID As Integer = createVAO()
        bindIndicesBuffer(indices)
        Me.storeDataInAttributeList(0, 3, positions)
        Me.storeDataInAttributeList(1, 2, textureCoords)
        Me.storeDataInAttributeList(2, 3, normals)
        unbindVAO()
        Return New RawModel(vaoID, indices.Length, modelName)
    End Function

End Class

Public Class BufferArray(Of T)
    Protected _array As T()

    Public Sub New(ByVal array As T())
        _array = New T(array.Length - 1) {}
        array.Copy(array, 0, _array, 0, _array.Length)
    End Sub

    Public Sub New(ByVal capacity As Integer)
        _array = New T(capacity - 1) {}

        For i As Integer = 0 To _array.Length - 1
            _array(i) = Nothing
        Next
    End Sub

    Public ReadOnly Property First As T
        Get
            Return _array(0)
        End Get
    End Property

    Public ReadOnly Property Last As T
        Get
            Return _array(_array.Length - 1)
        End Get
    End Property

    Public ReadOnly Property Length As Integer
        Get
            Return _array.Length
        End Get
    End Property

    Public ReadOnly Property Array As T()
        Get
            Return _array
        End Get
    End Property

    Public Function [Get](ByVal index As Integer, ByVal length As Integer) As T()
        Dim array = New T(length - 1) {}
        array.Copy(_array, index, array, 0, length)
        Return array
    End Function

    Public Function [Get]() As T()
        Dim array = New T(_array.Length - 1) {}
        array.Copy(_array, 0, array, 0, array.Length)
        Return array
    End Function

    Public Function GetValue(ByVal index As Integer) As T
        Return _array(index)
    End Function

    Public Sub Put(ByVal value As T)
        Dim array = New T(_array.Length + 1 - 1) {}
        array.Copy(_array, 0, array, 1, _array.Length)
        array(0) = value
        _array = array
    End Sub

    Public Sub Put(ByVal value As T, ByVal index As Integer)
        If index = 0 Then
            Put(value)
        ElseIf index = _array.Length - 1 Then
            Append(value)
        Else
            Dim array = New T(_array.Length + 1 - 1) {}
            array(index) = value
            array.Copy(_array, 0, array, 0, index)
            array.Copy(_array, index, array, index + 1, _array.Length - index)
            _array = array
        End If
    End Sub

    Public Sub Insert(ByVal value As T, ByVal index As Integer)
        _array(index) = value
    End Sub

    Public Sub SetA(ByVal value As T())
        _array = value
    End Sub

    Public Sub Append(ByVal value As T)
        Dim array = New T(_array.Length + 1 - 1) {}
        array.Copy(_array, 0, array, 0, _array.Length)
        array(array.Length - 1) = value
        _array = array
    End Sub

    Public Sub Put(ByVal value As T())
        Dim array = New T(_array.Length + value.Length - 1) {}
        array.Copy(value, 0, array, 0, value.Length)
        array.Copy(_array, 0, array, value.Length, _array.Length)
        _array = array
    End Sub

    Public Sub Put(ByVal value As T(), ByVal index As Integer)
        If index = 0 Then
            Put(value)
        ElseIf index = _array.Length - 1 Then
            Append(value)
        Else
            Dim array = New T(_array.Length + value.Length - 1) {}
            array.Copy(value, 0, array, index, value.Length)
            array.Copy(_array, 0, array, 0, index)
            array.Copy(_array, index, array, index + value.Length, _array.Length - index)
            _array = array
        End If
    End Sub

    Public Sub Append(ByVal value As T())
        Dim array = New T(_array.Length + value.Length - 1) {}
        array.Copy(_array, 0, array, 0, _array.Length)
        array.Copy(value, 0, array, _array.Length, value.Length)
        _array = array
    End Sub

    Public Sub Put(ByVal value As BufferArray(Of T))
        Put(value._array)
    End Sub

    Public Sub Put(ByVal value As BufferArray(Of T), ByVal index As Integer)
        Put(value._array, index)
    End Sub

    Public Sub Append(ByVal value As BufferArray(Of T))
        Append(value._array)
    End Sub

    Public Function GetBytes() As Byte()
        Dim buff = New Byte(_array.Length * Marshal.SizeOf(GetType(T)) - 1) {}
        System.Buffer.BlockCopy(_array, 0, buff, 0, buff.Length)
        Return buff
    End Function
End Class

Public Class RawModel

    Dim vaoID As Integer
    Dim vertexCount As Integer
    Dim modelName As String

    Public Sub New(vaoID As Integer, vertexCount As Integer, modelName As String)
        Me.vaoID = vaoID
        Me.vertexCount = vertexCount
        Me.modelName = modelName
    End Sub

    Public Sub New(vaoId As Integer, vertexCount As Integer)
        Me.vaoID = vaoId
        Me.vertexCount = vertexCount
        Me.modelName = Nothing
    End Sub

    Public Function getVaoID() As Integer
        Return vaoID
    End Function

    Public Function getVertexCount() As Integer
        Return vertexCount
    End Function

    Public Function getName() As String
        Return modelName
    End Function

End Class

Public Class ModelTexture

    Dim textureID As Integer
    Dim textureName As String

    Public Sub New(texture As Integer, name As String)
        Me.textureID = texture
        Me.textureName = name
    End Sub

    Public Sub New(texture As Integer)
        Me.textureID = texture
        Me.textureName = Nothing
    End Sub

    Public Function getTextureID() As Integer
        Return textureID
    End Function

    Public Function getName() As String
        Return textureName
    End Function

End Class

Public Class TextureData

    Dim width As Integer
    Dim height As Integer
    Dim buffer As BufferArray(Of Byte)

    Public Sub New(buffer As BufferArray(Of Byte), width As Integer, height As Integer)
        Me.width = width
        Me.height = height
        Me.buffer = buffer
    End Sub

    Public Function getWidth() As Integer
        Return width
    End Function

    Public Function getHeight() As Integer
        Return height
    End Function

    Public Function getBuffer() As BufferArray(Of Byte)
        Return buffer
    End Function

End Class

Public Class TexturedModel

    Dim rawModel As RawModel
    Dim texture As ModelTexture

    Public Sub New(rawModel As RawModel, modelTexutre As ModelTexture)
        Me.rawModel = rawModel
        Me.texture = modelTexutre
    End Sub

    Public Function getRawModel() As RawModel
        Return rawModel
    End Function

    Public Function getTexture() As ModelTexture
        Return texture
    End Function

End Class

Public Class ModelData

    Dim vertices As Double()
    Dim textureCoords As Double()
    Dim normals As Double()
    Dim indices As Integer()
    Dim furthestPoint As Double
    Dim modelName As String

    Public Sub New(vertices As Double(), textureCoords As Double(), normals As Double(), indices As Integer(),
                   furthestPoint As Double, modelName As String)
        Me.vertices = vertices
        Me.textureCoords = textureCoords
        Me.normals = normals
        Me.indices = indices
        Me.furthestPoint = furthestPoint
        Me.modelName = modelName
    End Sub

    Public Function getVertices() As Double()
        Return vertices
    End Function

    Public Function getTextureCoords() As Double()
        Return textureCoords
    End Function

    Public Function getNormals() As Double()
        Return normals
    End Function

    Public Function getIndices() As Integer()
        Return indices
    End Function

End Class

Public Class Entity
    Protected model As TexturedModel
    Protected position As Vector3d
    Protected rotX, rotY, rotZ As Single
    Protected scale As Single
    Public entityColor As Vector3d
    Private colorChanged As Boolean
    Private renderEntity As Boolean = True

    Public Sub New(ByVal model As TexturedModel, ByVal position As Vector3d, ByVal rotX As Single, ByVal rotY As Single, ByVal rotZ As Single, ByVal scale As Single, ByVal entityColor As Vector3d)
        Me.model = model
        Me.position = position
        Me.rotX = rotX
        Me.rotY = rotY
        Me.rotZ = rotZ
        Me.scale = scale
        Me.entityColor = entityColor
        Me.colorChanged = True
    End Sub

    Public Sub New(ByVal model As TexturedModel, ByVal position As Vector3d, ByVal rotX As Single, ByVal rotY As Single, ByVal rotZ As Single, ByVal scale As Single)
        Me.model = model
        Me.position = position
        Me.rotX = rotX
        Me.rotY = rotY
        Me.rotZ = rotZ
        Me.scale = scale
        Me.entityColor = New Vector3d(1, 1, 1)
        Me.colorChanged = True
    End Sub

    Public Sub New(ByVal model As TexturedModel, ByVal position As Vector3d, ByVal rotX As Single, ByVal rotY As Single, ByVal rotZ As Single)
        Me.model = model
        Me.position = position
        Me.rotX = rotX
        Me.rotY = rotY
        Me.rotZ = rotZ
        Me.scale = 1
        Me.entityColor = New Vector3d(1, 1, 1)
        Me.colorChanged = True
    End Sub

    Public Sub New(ByVal model As TexturedModel, ByVal position As Vector3d)
        Me.model = model
        Me.position = position
        Me.rotX = 0
        Me.rotY = 0
        Me.rotZ = 0
        Me.scale = 1
        Me.entityColor = New Vector3d(1, 1, 1)
        Me.colorChanged = True
    End Sub

    Public Sub New(ByVal model As TexturedModel, ByVal position As Vector3d, ByVal rotX As Single, ByVal rotY As Single, ByVal rotZ As Single, ByVal scale As Single, ByVal entityColor As Vector3d, ByVal renderEntity As Boolean)
        Me.model = model
        Me.position = position
        Me.rotX = rotX
        Me.rotY = rotY
        Me.rotZ = rotZ
        Me.scale = scale
        Me.entityColor = entityColor
        Me.colorChanged = True
        Me.renderEntity = renderEntity
    End Sub

    Public Sub increasePosition(ByVal dx As Single, ByVal dy As Single, ByVal dz As Single)
        Me.position.X += dx
        Me.position.Y += dy
        Me.position.Z += dz
    End Sub

    Public Sub increaseRotation(ByVal dx As Single, ByVal dy As Single, ByVal dz As Single)
        Me.rotX += dx
        Me.rotY += dy
        Me.rotZ += dz
    End Sub

    Public Sub increasePosition(ByVal pos As Vector3d)
        Me.position.X += pos.X
        Me.position.Y += pos.Y
        Me.position.Z += pos.Z
    End Sub

    Public Sub translate(ByVal x As Single, ByVal y As Single, ByVal z As Single)
        Me.position.X += x
        Me.position.Y += y
        Me.position.Z += z
    End Sub

    Public Sub setPos(ByVal x As Single, ByVal y As Single, ByVal z As Single)
        Me.position.X = x
        Me.position.Y = y
        Me.position.Z = z
    End Sub

    Public Sub rotate(ByVal rx As Single, ByVal ry As Single, ByVal rz As Single)
        If Me.rotY + ry <= 360 Then
            Me.rotY += ry
        Else
            Me.rotY = 0
        End If

        Me.rotX += rx
        Me.rotZ += rz
    End Sub

    Public Function getModel() As TexturedModel
        Return model
    End Function

    Public Sub setModel(ByVal model As TexturedModel)
        Me.model = model
    End Sub

    Public Function getPosition() As Vector3d
        Return position
    End Function

    Public Sub setPosition(ByVal position As Vector3d)
        Me.position = position
    End Sub

    Public Function getRotX() As Single
        Return rotX
    End Function

    Public Sub setRotX(ByVal rotX As Single)
        Me.rotX = rotX
    End Sub

    Public Function getRotY() As Single
        Return rotY
    End Function

    Public Sub setRotY(ByVal rotY As Single)
        Me.rotY = rotY
    End Sub

    Public Function getRotZ() As Single
        Return rotZ
    End Function

    Public Sub setRotZ(ByVal rotZ As Single)
        Me.rotZ = rotZ
    End Sub

    Public Function getScale() As Single
        Return scale
    End Function

    Public Sub setScale(ByVal scale As Single)
        Me.scale = scale
    End Sub

    Public Function getEntityColor() As Vector3d
        Return entityColor
    End Function

    Public Sub setEntityColor(ByVal entityColor As Vector3d)
        Me.entityColor = entityColor
        Me.colorChanged = True
    End Sub

    Public Function isColorChanged() As Boolean
        Return colorChanged
    End Function

    Public Sub setColorChanged(ByVal colorChanged As Boolean)
        Me.colorChanged = colorChanged
    End Sub

    Public Function getShouldRenderEntity() As Boolean
        Return renderEntity
    End Function

    Public Sub setRenderEntity(ByVal renderEntity As Boolean)
        Me.renderEntity = renderEntity
    End Sub
End Class

Public Class Maths

    'a=a+b;
    'b=a-b;
    'a=a-b;

    Shared width As Integer
    Shared height As Integer

    Public Shared Function getWidth() As Integer
        Return width
    End Function

    Public Shared Function getHeight() As Integer
        Return height
    End Function

    Public Shared Sub setHeight(h As Integer)
        height = h
    End Sub

    Public Shared Sub setWidth(w As Integer)
        width = w
    End Sub

    Public Shared Function DegreesToRadians(ByVal degrees As Double) As Double
        Return degrees * Math.PI / 180
    End Function

    Public Shared Function RadiansToDegrees(ByVal radians As Double) As Double
        Return radians * 180 / Math.PI
    End Function

    Public Enum colors
        NULL
        COLOR_RED
        COLOR_BLUE
        COLOR_GREEN
    End Enum

    Public Shared Function lerp(ByVal point1 As Single, ByVal point2 As Single, ByVal alpha As Single) As Single
        Return point1 + alpha * (point2 - point1)
    End Function

    Public Shared Function lerp(ByVal point1 As Vector3d, ByVal point2 As Vector3d, ByVal alpha As Single) As Vector3d
        Dim x As Single = point1.X + alpha * (point2.X - point1.X)
        Dim y As Single = point1.Y + alpha * (point2.Y - point1.Y)
        Dim z As Single = point1.Z + alpha * (point2.Z - point1.Z)
        Return New Vector3d(x, y, z)
    End Function

    Public Shared Function lerpA(ByVal point1 As Vector3d, ByVal point2 As Vector3d, ByVal alpha As Single) As Vector3d
        Dim x As Single = point1.X + alpha * (point2.X - point1.X)
        Dim y As Single = point1.Y + alpha * (point2.Y - point1.Y)
        Dim z As Single = point1.Z + alpha * (point2.Z - point1.Z)
        point1.X += x
        point1.Y += y
        point1.Z += z
        Return New Vector3d(x, y, z)
    End Function

    Public Shared Function distance(ByVal point1 As Vector3d, ByVal point2 As Vector3d) As Vector3d
        Dim vec As Vector3d = New Vector3d(point1.X - point2.X, point1.Y - point2.Y, point1.Z - point2.Z)
        Return vec
    End Function

    Public Shared Function distanceABS(ByVal point1 As Vector3d, ByVal point2 As Vector3d) As Vector3d
        Dim vec As Vector3d = New Vector3d(Math.Abs(point1.X - point2.X), Math.Abs(point1.Y - point2.Y), Math.Abs(point1.Z - point2.Z))
        Return vec
    End Function

    Public Shared Function decodeRGB255(ByVal r As Integer, ByVal g As Integer, ByVal b As Integer) As Vector3d
        Return New Vector3d(r / 255, g / 255, b / 255)
    End Function

    Public Shared Function decodeRGB255(ByVal b As Integer) As Single
        Return b / 255
    End Function

    Public Shared Function roundDown5(ByVal d As Double) As Double
        Return CLng((d * 100000.0)) / 100000.0
    End Function

    Public Shared Function roundDown4(ByVal d As Double) As Double
        Return CLng((d * 10000.0)) / 10000.0
    End Function

    Public Shared Function roundDown3(ByVal d As Double) As Double
        Return CLng((d * 1000.0)) / 1000.0
    End Function

    Public Shared Function roundDown2(ByVal d As Double) As Double
        Return CLng((d * 100.0)) / 100.0
    End Function

    Public Shared Function randInt(ByVal min As Integer, ByVal max As Integer) As Integer
        Dim rand As Random = New Random()
        Dim randomNum As Integer = rand.Next((max - min) + 1) + min
        Return randomNum
    End Function

    Public Shared Function getEntityColor(ByVal color As Vector3d) As colors
        If color.X > color.Y AndAlso color.X > color.Z Then
            Return colors.COLOR_RED
        ElseIf color.Y > color.X AndAlso color.Y > color.Z Then
            Return colors.COLOR_GREEN
        ElseIf color.Z > color.Y AndAlso color.Z > color.X Then
            Return colors.COLOR_BLUE
        End If

        'System.err.println("We are at the end of the line " & vbLf & " Please fix this method!")
        Return colors.NULL
    End Function

    Public Shared Function distance2(ByVal first As Vector3d, ByVal last As Vector3d) As Vector3d
        Return New Vector3d(first.X - last.X, first.Y - last.Y, first.Z - last.Z)
    End Function

    Public Shared Function randomFloat(ByVal min As Single, ByVal max As Single) As Single
        Return CSng((min + New Random().Next * (max - min)))
    End Function

    Public Shared Function randomFloat(ByVal r As Random, ByVal min As Single, ByVal max As Single) As Single
        Return CSng((min + r.NextDouble() * (max - min)))
    End Function

    Public Shared Function createViewMatrix(ByVal x As Single, ByVal y As Single, ByVal z As Single) As Matrix4d
        Dim viewMatrix As Matrix4d = New Matrix4d()
        'viewMatrix.setIdentity()
        Matrix4d.CreateRotationX(CSng(DegreesToRadians(0)), viewMatrix)
        Matrix4d.CreateRotationY(CSng(DegreesToRadians(0)), viewMatrix)
        Dim cameraPos As Vector3d = New Vector3d(x, y, z)
        Dim negativeCameraPos As Vector3d = New Vector3d(-cameraPos.X, -cameraPos.Y, -cameraPos.Z)
        Matrix4d.CreateTranslation(negativeCameraPos, viewMatrix)
        Return viewMatrix
    End Function

    Public Shared Function createTransformationMatrix(ByVal translation As Vector3d, ByVal rx As Single, ByVal ry As Single, ByVal rz As Single, ByVal scale As Single) As Matrix4d
        Dim matrix As Matrix4d = New Matrix4d()
        Matrix4d.CreateTranslation(translation, matrix)
        Matrix4d.CreateRotationX(CDbl(DegreesToRadians(rx)), matrix)
        Matrix4d.CreateRotationY(CDbl(DegreesToRadians(ry)), matrix)
        Matrix4d.CreateRotationZ(CDbl(DegreesToRadians(rz)), matrix)
        matrix = Matrix4d.Scale(New Vector3d(scale, scale, scale))
        Return matrix
    End Function
End Class

Public Class Player
    Inherits Entity

    Public speed As Single = 12
    Public zoom As Single = 10
    Private model As TexturedModel

    Public Sub New(ByVal modeld As TexturedModel, ByVal modelRight As TexturedModel, ByVal position As Vector3)
        MyBase.New(modeld, position, 0, 0, 0, 1)
        Me.model = modeld
    End Sub

    Public Sub translate(ByVal x As Single, ByVal y As Single, ByVal z As Single)
        MyBase.translate(x, y, z)
        Me.increasePosition(x, y, 0)
    End Sub

    Public Sub update()

    End Sub
End Class