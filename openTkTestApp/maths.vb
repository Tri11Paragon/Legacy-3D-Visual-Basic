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
' math classes for doing math stuff
Public Class maths

    ' returns distance between 2 points
    Public Shared Function distance(p1 As Double, p2 As Double) As Double
        Return Math.Abs(p1 - p2)
    End Function

    'returns distance between 2 vectors
    Public Shared Function distance(v As Vector3, v2 As Vector3) As Vector3
        Return New Vector3(Math.Abs(v.X - v2.X), Math.Abs(v.Y - v2.Y), Math.Abs(v.Z - v2.Z))
    End Function

    ' distance between 2 vectors as a single double
    Public Shared Function distanceD(v As Vector3, v2 As Vector3) As Double
        Return Math.Abs((v.X + v.Y + v.Z) - (v2.X + v2.Y + v2.Z))
    End Function
    ' returns distance between 6 points (as vector)
    Public Shared Function distance(x1 As Double, y1 As Double, z1 As Double, x2 As Double, y2 As Double, z2 As Double) As Vector3
        Return New Vector3(Math.Abs(x1 - x2), Math.Abs(y1 - y2), Math.Abs(z1 - z2))
    End Function
    ' tells if the vector is empty
    Public Shared Function notEmpty(vec As Vector3) As Boolean
        Return If(vec.X > 0, True, False) Or If(vec.Y > 0, True, False) Or If(vec.Z > 0, True, False)
    End Function

End Class

' helps with strings and stuff
Public Class o_helper

    ' removes spaces from a single string
    Public Shared Function removeSpaces(str As String) As String
        Dim ch As Char() = str.ToCharArray()
        Dim s As String = ""

        ' removes spaces from string
        For Each c In ch
            If (Not (String.IsNullOrWhiteSpace(c.ToString) Or String.IsNullOrEmpty(c.ToString))) Then
                s += c
            End If
        Next

        Return s
    End Function

    'removes spaces from the string array
    Public Shared Function removeSpaces(str As String()) As String()
        'return string array
        Dim rtrn(str.Length) As String

        For i As Int16 = 0 To str.Length - 1
            rtrn(i) = removeSpaces(str(i))
        Next

        ' return string
        Return rtrn
    End Function

    ' removes empty strings from an array
    Public Shared Function removeEmpty(str As String()) As String()
        Dim lines As Int64 = 0

        ' loops through the amount of lines to return
        For i As Int64 = 0 To str.Length - 1
            If Not (String.IsNullOrEmpty(str(i))) Then
                lines += 1
            End If
        Next
        ' array to return
        Dim f(lines) As String

        ' adds to the array if its not empty
        For i As Int64 = 0 To str.Length - 1
            If Not (String.IsNullOrEmpty(str(i))) Then
                f(i) = str(i)
            End If
        Next

        ' return
        Return f
    End Function

    ' if its empty it returns nothing
    Public Shared Function removeEmpty(str As String) As String
        Dim f As String = Nothing

        If Not String.IsNullOrEmpty(str) Then
            f = str
        End If

        Return f
    End Function

    ' built for the thing
    Public Shared Function fn_1293(str As String) As String
        ' the string to return
        Dim f As String = Nothing
        ' returns a % if we are not to read this line of code
        If Not String.IsNullOrEmpty(str) Then
            If str.StartsWith("//") Or str.StartsWith("'") Or str.StartsWith("#") Then
                f = "%"
            Else
                f = str
            End If
        Else
            f = "%"
        End If

        ' return the string
        Return f
    End Function

End Class

'clock class
Class clock
    Private Shared paused As Boolean = False
    Public Shared lastFrame, totalTime, startTime As Long
    Public Shared d As Double = 0, multiplier As Double = 1


    ' gets the delta
    Public Shared Function getDelta() As Double
        Dim currentTime As Long = CType((DateTime.Now - New DateTime(1970, 1, 1)).TotalMilliseconds, Int64) ' yes this is very long and is not needed, you could just use datetime.now.millisconds but this is the proper way for getting milliseconds
        Dim delta As Double = (currentTime - lastFrame) ' get the change between last renderering frame and current renderering frame
        lastFrame = CType((DateTime.Now - New DateTime(1970, 1, 1)).TotalMilliseconds, Int64) ' update the last frame time

        ' prevent overflow of delta (Prevents too much frame-skipping)
        If delta * 0.001F > 0.05F Then
            Return 0.05F
        End If

        Return delta * 0.001F
    End Function

    ' this returns the delta times the multiplier which allows for speededing up the game
    Public Shared Function Delta() As Double
        If paused Then
            Return 0
        Else
            Return d * multiplier
        End If
    End Function

    'returns the total time of the program
    Public Shared Function getTotalTime() As Double
        Return totalTime
    End Function

    'retruns the muiltiplier
    Public Shared Function getMultiplier() As Double
        Return multiplier
    End Function

    'update the clock
    Public Shared Sub update()
        d = getDelta()
        totalTime += d
    End Sub

    'change multiplier
    Public Shared Sub ChangeMultiplier(ByVal change As Double)
        multiplier += change
    End Sub

    'pause the game(not used)
    Public Shared Sub Pause()
        If paused Then
            paused = False
        Else
            paused = True
        End If
    End Sub
End Class