Imports System.Windows.Forms

Public Class WindowsAuthDialog
    Public LoggedInUser As Employee
    Dim Emplist As New EmployeeCollection

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click

        'Check the login. 
        Try
            If Emplist.FindEmployeeByID(Convert.ToInt16(Username.Text)).CheckPin(Password.Text) = True Then
                LoggedInUser = Emplist.FindEmployeeByID(Username.Text)
                Me.DialogResult = System.Windows.Forms.DialogResult.OK
                Me.Close()
            End If
        Catch NoUser As Exceptions.EmployeeNotFoundException
            Me.Height = 305
            LargeIconButton.Height = 105
            ErrorText.Text = "User ID not found"
            ErrorText.Visible = True
            Username.Text = ""
            Password.Text = ""
            Username_Leave(Nothing, Nothing)
            Password_Leave(Nothing, Nothing)
            Username.Focus()
        Catch BadPass As Exceptions.WrongPinExceptoin
            Me.Height = 305
            LargeIconButton.Height = 105
            ErrorText.Text = "Password is incorrect"
            ErrorText.Visible = True
            Password.Text = ""
            Password_Leave(Nothing, Nothing)
            Password.Focus()
        Catch Badformat As FormatException
            Me.Height = 305
            LargeIconButton.Height = 105
            ErrorText.Text = "User ID contains bad characters"
            ErrorText.Visible = True
            Username.Text = ""
            Password.Text = ""
            Username_Leave(Nothing, Nothing)
            Password_Leave(Nothing, Nothing)
            Username.Focus()
        End Try



    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub Username_Enter(sender As Object, e As EventArgs) Handles Username.Enter
        Username.Font = New Drawing.Font("Segoe UI", 9.0!, Drawing.FontStyle.Regular)
        Username.ForeColor = Drawing.Color.Black
        If Username.Text = " Username" Then
            Username.Text = ""
        End If
    End Sub
    Private Sub Password_Enter(sender As Object, e As EventArgs) Handles Password.Enter
        Password.Font = New Drawing.Font("Segoe UI", 9.0!, Drawing.FontStyle.Regular)
        Password.ForeColor = Drawing.Color.Black
        If Password.Text = " Password" Then
            Password.Text = ""
        End If
    End Sub

    Private Sub Password_Leave(sender As Object, e As EventArgs) Handles Password.Leave
        If Password.Text = "" Then
            Password.Font = New Drawing.Font("Segoe UI", 9.0!, Drawing.FontStyle.Italic)
            Password.ForeColor = Drawing.Color.DimGray
            Password.Text = " Password"
        End If
    End Sub
    Private Sub Username_Leave(sender As Object, e As EventArgs) Handles Username.Leave
        If Username.Text = "" Then
            Username.Font = New Drawing.Font("Segoe UI", 9.0!, Drawing.FontStyle.Italic)
            Username.ForeColor = Drawing.Color.DimGray
            Username.Text = " Username"
        End If
    End Sub
End Class
