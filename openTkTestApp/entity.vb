Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports OpenTK.Input

Public Class entity

    Private x As Double
    Private y As Double
    Private z As Double
    Private rotation As Double ' rotation
    Private texture As Integer
    Private velocity As Vector3d
    Private mesh As Mesh
    Private acceleration As Vector3d
    Private isOnGround As Boolean = False
    Private friction As Double = 5.23D
    Public deathChance As Double = 10 ' im not going to use getters and setters for this stuff. its pointless as there will be no math with it
    Public birthChance As Double = 15 ' im also the only developer so like i don't really need access rules

    ' start time
    Dim start As Long = 0

    Public Sub New(ByRef mesh As Mesh, ByRef texture As Integer, x As Double, y As Double, z As Double)
        Me.x = x
        Me.y = y
        Me.z = z
        Me.texture = texture
        Me.mesh = mesh
        start = CType((DateTime.Now - New DateTime(1970, 1, 1)).TotalMilliseconds, Int64)
    End Sub

    ' returns true if the entity's velocity is more then 0
    Public Function isMoving() As Boolean '
        Return If(velocity.X <> 0, True, False) Or If(velocity.Y <> 0, True, False) Or If(velocity.Z <> 0, True, False)
    End Function

    ' returns true if the entity is accelerating
    Public Function isAccelerating() As Boolean
        Return If(acceleration.X <> 0, True, False) Or If(acceleration.Y <> 0, True, False) Or If(acceleration.Z <> 0, True, False)
    End Function

    Public Sub update()
        If (isMoving()) Then
            ' adds the velocity to the entity each update
            ' it also adjusts for the clock delta
            x += velocity.X * clock.Delta()
            y += velocity.Y * clock.Delta()
            z += velocity.Z * clock.Delta()
        End If
        If isAccelerating() Then
            ' this moves the entity according to the clock delta. (basiclly means if the game lags, then the movement will account for the lag)
            velocity += (acceleration * clock.Delta())
        End If
        ' if the acceleration is greater then 42 then we have hit our terminal velocity and therefore will not have more velocity more
        If velocity.Y <= -42 Then
            ' sets to make sure we have this correct
            acceleration.Y = 0
            velocity.Y = -42
        Else
            acceleration.Y += -9.81
        End If
        If y <= 0.0 Then
            isOnGround = True
            y = 0.0
            If velocity.Y <= 0 Then
                velocity.Y = 0
            End If
            If acceleration.Y <= 0 Then
                acceleration.Y = 0
            End If
        Else
            isOnGround = False
        End If
        If isOnGround Then
            bounce()
        End If
        ' this runs once per entity interval which is defined inside the settings file.
        If CType((DateTime.Now - New DateTime(1970, 1, 1)).TotalMilliseconds, Int64) - start >= settings.entityInterval Then
            start = CType((DateTime.Now - New DateTime(1970, 1, 1)).TotalMilliseconds, Int64) ' reset stuff
            ' do death and birth checks.
            death()
            birth()
        End If
    End Sub

    Public Sub bounce()
        acceleration.Y += 75
    End Sub

    Public Sub birth()
        'world.findClosestEntity(x, y, z)
        Console.WriteLine("birth!")
    End Sub

    Public Sub death()

    End Sub

    Public Sub draw()
        'Dim d As Vector3 = maths.distance(-camera.x, camera.y, -camera.z, Me.x, Me.y, Me.z)
        'If (d.X < 40 And d.Z < 40) And d.Y < 40 Then
        GL.Rotate(rotation, 0, 1, 0)
        artist.drawMesh(mesh, texture, x, y, z)
        GL.Rotate(rotation, 0, -1, 0)
        'End If
    End Sub

    '
    ' THE FOLLOW SUBS ARE JUST GETTERS / SETERS
    ' They get information / set information about the entity

    Public Sub accelerate(vel As Vector3d)
        velocity += vel
    End Sub
    Public Sub accelerate(x As Double, y As Double, z As Double)
        velocity.X += x
        velocity.Y += y
        velocity.Z += z
    End Sub

    Public Sub setAcceleration(a As Vector3d)
        acceleration = a
    End Sub

    Public Sub setAcceleration(x As Double, y As Double, z As Double)
        acceleration.X += x
        acceleration.Y += y
        acceleration.Z += z
    End Sub

    Public Function getAcceleration() As Vector3d
        Return acceleration
    End Function

    Public Sub setVelocity(vel As Vector3d)
        velocity = vel
    End Sub

    Public Sub setVelocity(x As Double, y As Double, z As Double)
        velocity.X = x
        velocity.Y = y
        velocity.Z = z
    End Sub

    Public Sub setRotation(rotation As Double)
        Me.rotation = rotation
    End Sub

    Public Function getX() As Double
        Return Me.x
    End Function

    Public Function getY() As Double
        Return Me.y
    End Function

    Public Function getZ() As Double
        Return Me.z
    End Function

    Public Function getPosition() As Vector3
        Return New Vector3(Me.x, Me.y, Me.z)
    End Function

    Public Function getTexture() As Double
        Return Me.texture
    End Function

    Public Function getVelocity() As Vector3d
        Return Me.velocity
    End Function

    Public Function getRotation() As Double
        Return Me.rotation
    End Function

    Public Function getMesh() As Mesh
        Return Me.mesh
    End Function

End Class
