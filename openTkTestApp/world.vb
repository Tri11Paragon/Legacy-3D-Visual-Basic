Public Class world

    Public Shared entites As List(Of entity) = New List(Of entity)
    Public Shared textures(125) As Integer

    Public Shared Sub create()

    End Sub

    Public Shared Sub update()
        For Each d As entity In world.entites
            d.update()
            d.draw()
        Next
    End Sub

End Class
