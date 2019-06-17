Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports OpenTK.Input

' Brett Terpstra
' 2019-06-16
' final project
' entity class. does jump things n stuff
Public Class entity

    ' poistion
    Private x As Double = 0
    Private y As Double = 0
    Private z As Double = 0
    Private rotation As Double = 0 ' rotation
    Private texture As Integer = 0 ' texture number
    Private velocity As New Vector3d(0, 0, 0) ' velocity
    Private mesh As Mesh ' mesh of the entity
    Private acceleration As New Vector3d(0, 0, 0) ' acceleration
    Private isOnGround As Boolean = False ' is the entity on ground?
    Private friction As Double = 5.23D ' not used varaible
    Public deathChance As Double = 10 ' im not going to use getters and setters for this stuff. its pointless as there will be no math with it
    Public birthChance As Double = 15 ' im also the only developer so like i don't really need access rules

    ' start time (time since last entity interval)
    Dim start As Long = 0

    ' this is the constructor
    Public Sub New(ByRef mesh As Mesh, ByRef texture As Integer, x As Double, y As Double, z As Double)
        Me.x = x
        Me.y = y
        Me.z = z
        Me.texture = texture
        Me.mesh = mesh
        ' tell the game what time it is when starting
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
        ' if we are below 0 then we are coliding with the world
        If y <= 0.0 Then
            isOnGround = True ' tell the game we are on the ground 
            y = 0.0 ' make sure we don't keep going down
            If velocity.Y <= 0 Then ' prevents moving more then we need to just to get reset
                velocity.Y = 0
            End If
            If acceleration.Y <= 0 Then ' same as ^ but removes even more math that needs to be done
                acceleration.Y = 0
            End If
        Else
            isOnGround = False
        End If
        If isOnGround Then
            If ((101) * Rnd() <= 10) Then
                bounce()
            End If
        End If
            ' this runs once per entity interval which is defined inside the settings file.
            If CType((DateTime.Now - New DateTime(1970, 1, 1)).TotalMilliseconds, Int64) - start >= settings.entityInterval Then
            start = CType((DateTime.Now - New DateTime(1970, 1, 1)).TotalMilliseconds, Int64) ' reset stuff
            ' do death and birth checks.
            doAI()
            birth()
            death()
        End If

    End Sub

    ' BOING
    Public Sub bounce()
        acceleration.Y += 75
    End Sub


    Public Sub doAI()
        ' reset the velocity because entity is now done moving
        If velocity.X <> 0 Then
            velocity.X = 0
        End If
        If velocity.Z <> 0 Then
            velocity.Z = 0
        End If
        ' 50% chance each interval to move
        If isOnGround And ((100) * Rnd() <= 50) Then
            ' just how much to move
            Dim dist As Double = (10 - 1 + 1) * Rnd() + 1

            ' adds some movement in a random directon
            ' 50% chance to be forward or backwards
            If ((100) * Rnd() <= 50) Then
                velocity.Z += dist
            Else
                velocity.Z -= dist
            End If
            ' this handles x direction
            If ((100) * Rnd() <= 50) Then
                velocity.X += dist
            Else
                velocity.X -= dist
            End If
        End If
        ' THIS IS DISABLED DUE TO A BUG THAT WAS FOUND AT LAST MINUTE.
        ' I DON'T KNOW IF THIS IS THE CAUSE OF IT OR IF IT WAS ACUTALLY A BUG
        ' not like this changes anything in the game.
        ' adds some random rotaiton (rotation does not change movement)
        'rotation += (10 - (-10) + 1) * Rnd() - 10
        'reset rotation if we give >360 (prevents large numbers)
        'If rotation >= 360 Then
        '    rotation = 0
        'End If
    End Sub

    ' birth stuff
    Public Sub birth()
        'prevents running if the entity is unable to birth
        If Me.birthChance <= 0 Then
            Return
        End If
        ' this next 2 lines just gets the death values from the world loader
        Dim valFromBirth As Double = 0
        world.deathChances.TryGetValue(Me.texture, valFromBirth)
        ' generates a random number between 0 and 100. 
        ' if the chance is 50, that means there should be a 50% chance of this being executed
        If ((101) * Rnd() <= (Me.birthChance + valFromBirth)) Then
            ' finds closest entity to birth with
            Dim ent = world.findClosestEntity(x, y, z)
            'prevents running if breeding entity is unable to birth
            If ent.birthChance <= 0 Or ent.deathChance <= 0 Then
                Console.WriteLine("Unable to birth! Birth chance: " & ent.birthChance & " Death Chance: " & ent.deathChance)
                Return
            End If
            Dim t As Integer = (Math.Max(ent.texture, texture) - Math.Min(ent.texture, texture) + 1) * Rnd() + Math.Min(ent.texture, texture) ' select random texture between the parents
            Dim m = {ent.mesh, Me.mesh}((2) * Rnd()) ' select mesh between the 2 parents
            Dim bc = ((Math.Max(Me.birthChance, ent.birthChance) - Math.Min(Me.birthChance, ent.birthChance) + 1) * Rnd() + Math.Min(Me.birthChance, ent.birthChance)) ' pick random between the 2 entties for birthchance
            bc += (0.1 - (-0.1) + 1) * Rnd() + (-0.1) ' this adds some randomness to the traits
            Dim dc = ((Math.Max(Me.deathChance, ent.deathChance) - Math.Min(Me.deathChance, ent.deathChance) + 1) * Rnd() + Math.Min(Me.deathChance, ent.deathChance)) ' pick random death chance
            dc += (0.1 - (-0.1) + 1) * Rnd() + (-0.1) ' this adds some randomness to the traits

            'setup the new child with ^ traits
            Dim child As New entity(m, t, Me.x, Me.y, Me.z)
            child.setRotation(0)
            ' set birth chance
            child.birthChance = bc
            child.deathChance = dc

            'add to world
            world.entites.Add(child)
            Console.WriteLine("Made a child! There are now " & world.entites.Count() & " in the world!")

            'Throw New Exception("Test!")
        End If
    End Sub

    ' death stuff
    Public Sub death()
        'prevents running code if the entity is not able to die.
        If Me.deathChance <= 0 Then
            Return
        End If
        ' this next 2 lines just gets the death values from the world loader
        Dim valFromDeath As Double = 0
        world.deathChances.TryGetValue(Me.texture, valFromDeath)
        ' generates a random number between 0 and 100. 
        ' if the chance is 50, that means there should be a 50% chance of this being executed
        If ((101) * Rnd() <= (Me.deathChance + valFromDeath)) Then
            world.entites.Remove(Me) ' i like how its just "remove(me)" kek
        End If
    End Sub

    Public Sub draw()
        'Dim d As Vector3 = maths.distance(-camera.x, camera.y, -camera.z, Me.x, Me.y, Me.z)
        'If (d.X < 40 And d.Z < 40) And d.Y < 40 Then

        ' THIS IS DISABLED DUE TO A BUG THAT WAS FOUND AT LAST MINUTE.
        ' I DON'T KNOW IF THIS IS THE CAUSE OF IT OR IF IT WAS ACUTALLY A BUG
        ' not like this changes anything in the game.
        ' (the rotation is disabled)

        'GL.Rotate(rotation, 0, 1, 0)
        artist.drawMesh(mesh, texture, x, y, z)
        'GL.Rotate(rotation, 0, -1, 0)

        'End If
    End Sub

    '
    ' THE FOLLOW SUBS ARE JUST GETTERS / SETERS
    ' They get information / set information about the entity
    ' they explain themselves

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
