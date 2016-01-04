Namespace Authentication

    Public Class AuthClass
        Dim AuthdUser As Employee

        Public ReadOnly Property AuthenticatedUser() As Employee
            Get
                If IsNothing(AuthdUser) Then
                    'Need to get an authorisation
                    AuthdUser = Authorise()
                    If IsNothing(AuthdUser) Then
                        Return Nothing
                    End If
                    Return AuthdUser
                Else
                    'Already logged in
                    Return AuthdUser
                End If
            End Get
        End Property


        Public Function Authorise() As Employee
            Dim NewAuthDialog As New WindowsAuthDialog
            Dim Result As MsgBoxResult = NewAuthDialog.ShowDialog()
            If Result = MsgBoxResult.Ok Then
                'Must have succeeded.
                Return NewAuthDialog.LoggedInUser
            Else
                'Failed
                Return Nothing
            End If
        End Function
    End Class

End Namespace