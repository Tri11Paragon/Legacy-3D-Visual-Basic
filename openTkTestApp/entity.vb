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
    Private r As Double ' rotation
    Private texture As Integer
    Private velocity As Vector3
    Private mesh As Mesh

    Public Sub New(ByRef mesh As Mesh, ByRef texture As Integer, x As Double, y As Double, z As Double)
        Me.x = x
        Me.y = y
        Me.z = z
        Me.texture = texture
        Me.mesh = mesh
    End Sub

    Public Function isMoving() As Boolean
        Return If(velocity.X > 0, True, False) Or If(velocity.Y > 0, True, False) Or If(velocity.Z > 0, True, False)
    End Function

    Public Sub update()
        If (isMoving()) Then
            x += velocity.X
            y += velocity.Y
            z += velocity.Z
        End If
    End Sub

    Public Sub draw()
        'Dim d As Vector3 = maths.distance(-camera.x, camera.y, -camera.z, Me.x, Me.y, Me.z)
        'If (d.X < 40 And d.Z < 40) And d.Y < 40 Then
        artist.drawMesh(mesh, texture, x, y, z)
        'End If
    End Sub

    Public Sub accelerate(vel As Vector3)
        velocity += vel
    End Sub

    Public Sub birth(e As entity)

    End Sub

    Public Sub death()
        If ((20 - 1) * Rnd()) = 1 Then

        End If
    End Sub

    Public Sub accelerate(x As Double, y As Double, z As Double)
        velocity.X += x
        velocity.Y += y
        velocity.Z += z
    End Sub

    Public Sub setVelocity(vel As Vector3)
        velocity = vel
    End Sub

    Public Sub setVelocity(x As Double, y As Double, z As Double)
        velocity.X = x
        velocity.Y = y
        velocity.Z = z
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

    Public Function getTexture() As Double
        Return Me.texture
    End Function

    Public Function getVelocity() As Vector3
        Return Me.velocity
    End Function

    Public Function getRotation() As Double
        Return Me.r
    End Function

    Public Function getMesh() As Mesh
        Return Me.mesh
    End Function

End Class
