Imports MySql.Data.MySqlClient

Module MySql

    Function TestConn()
        'From heere
        Dim cs As String = "Database=whldata;Data Source=192.168.1.86;" _
            & "User Id=appuser;Password=apppassword;ConnectionTimeout=1"

        Dim conn As New MySqlConnection(cs)

        Try
            conn.Open()
            'To here should be the same every time. After here in this little block is where we actually do the work.

            ' Do nothing with the connection.

            'Then return at the end with the data.
            Return "Connection to " + conn.Database + " is active (" + My.Computer.Clock.LocalTime.ToLongTimeString + ")"

        Catch ex As MySqlException
            Return ex.Message
        Finally
            conn.Close()
        End Try

    End Function

    Function SelectData(query As String)
        'From heere
        Dim cs As String = "Database=whldata;Data Source=192.168.1.86;" _
            & "User Id=appuser;Password=apppassword;ConnectionTimeout=1"

        Dim conn As New MySqlConnection(cs)

        Try
            conn.Open()
            'To here should be the same every time. After here in this little block is where we actually do the work.

            Dim sqlquery As MySqlCommand = New MySqlCommand(query, conn)
            Dim returneddata As MySqlDataReader = sqlquery.ExecuteReader()

            Dim returnme As New ArrayList
            Dim looper As Integer = 0
            While returneddata.Read()

                Dim fieldloop As Integer = 0
                Dim row As New ArrayList
                row.Clear()
                While fieldloop < returneddata.FieldCount
                    row.Add(returneddata.Item(fieldloop))
                    fieldloop = fieldloop + 1
                End While
                returnme.Add(row)


            End While


            'Then return at the end with the data.
            Return returnme
        Catch ex As MySqlException
            Return ex.Message

        Finally
            conn.Close()
        End Try

    End Function
    Function insertupdate(query As String) As String

        'From heere
        Dim cs As String = "Database=whldata;Data Source=192.168.1.86;" _
            & "User Id=appuser;Password=apppassword;ConnectionTimeout=1"

        Dim conn As New MySqlConnection(cs)

        Try

            conn.Open()
            'To here should be the same every time. After here in this little block is where we actually do the work.

            Dim sqlquery As MySqlCommand = New MySqlCommand(query, conn)

            Return sqlquery.ExecuteNonQuery()



        Catch ex As MySqlException
            Return ex.Message
        Finally
            conn.Close()
        End Try

        Return "cooL"
    End Function
End Module
