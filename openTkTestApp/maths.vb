Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports OpenTK.Input

Public Class maths

    Public Shared Function distance(p1 As Double, p2 As Double) As Double
        Return Math.Abs(p1 - p2)
    End Function

    Public Shared Function distance(v As Vector3, v2 As Vector3) As Vector3
        Return New Vector3(Math.Abs(v.X - v2.X), Math.Abs(v.Y - v2.Y), Math.Abs(v.Z - v2.Z))
    End Function

    Public Shared Function distance(x1 As Double, y1 As Double, z1 As Double, x2 As Double, y2 As Double, z2 As Double) As Vector3
        Return New Vector3(Math.Abs(x1 - x2), Math.Abs(y1 - y2), Math.Abs(z1 - z2))
    End Function

    Public Shared Function notEmpty(vec As Vector3) As Boolean
        Return If(vec.X > 0, True, False) Or If(vec.Y > 0, True, False) Or If(vec.Z > 0, True, False)
    End Function

End Class

Public Class o_helper

    Public Shared Function removeSpaces(str As String) As String
        Dim ch As Char() = str.ToCharArray()
        Dim s As String = ""

        For Each c In ch
            If (Not (String.IsNullOrWhiteSpace(c.ToString) Or String.IsNullOrEmpty(c.ToString))) Then
                s += c
            End If
        Next

        Return s
    End Function

    Public Shared Function removeSpaces(str As String()) As String()
        Dim rtrn(str.Length) As String

        For i As Int16 = 0 To str.Length - 1
            rtrn(i) = removeSpaces(str(i))
        Next

        Return rtrn
    End Function

    Public Shared Function removeEmpty(str As String()) As String()
        Dim lines As Int64 = 0

        For i As Int64 = 0 To str.Length - 1
            If Not (String.IsNullOrEmpty(str(i))) Then
                lines += 1
            End If
        Next

        Dim f(lines) As String

        For i As Int64 = 0 To str.Length - 1
            If Not (String.IsNullOrEmpty(str(i))) Then
                f(i) = str(i)
            End If
        Next

        Return f
    End Function

    Public Shared Function removeEmpty(str As String) As String
        Dim f As String = Nothing

        If Not String.IsNullOrEmpty(str) Then
            f = str
        End If

        Return f
    End Function

    ' built for the thing
    Public Shared Function fn_1293(str As String) As String
        Dim f As String = Nothing

        If Not String.IsNullOrEmpty(str) Then
            If str.StartsWith("//") Or str.StartsWith("'") Or str.StartsWith("#") Then
                f = "skip"
            Else
                f = str
            End If
        Else
            f = "skip"
        End If

        Return f
    End Function

End Class

Public Class o_timer



End Class