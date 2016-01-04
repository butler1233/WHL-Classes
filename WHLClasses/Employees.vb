Imports System.Text

Public Class EmployeeCollection
    Public Sub New()

        Dim Data As ArrayList = MySql.SelectData("SELECT * FROM whldata.employees")
        For Each Emp As ArrayList In Data
            Dim NewEmp As New Employee(Emp)
            Employees.Add(NewEmp)
        Next
    End Sub
    Public Function FindEmployeeByID(PayrollId As Integer) As Employee
        For Each Emplo As Employee In Employees
            If Emplo.PayrollId = PayrollId Then
                Return Emplo
            End If
        Next
        Throw New Exceptions.EmployeeNotFoundException("An employee with that Payroll ID could not be found.")
    End Function


    Public Employees As New List(Of Employee) 'This needs to be moved. Well, removed and the class should inherit list of emplyee.
End Class


Public Class EmpLoginStatus
    Dim id As Integer
    Public LoggedIn As Boolean
    Public FriendlyText As String
    Public Computer As String
    Public Time As String


    Public Sub Update()
        FriendlyText = ""
        Dim Cool As ArrayList = MySql.SelectData("SELECT * FROM whldata.log_loginout WHERE UserId=" + id.ToString + " ORDER BY logid DESC LIMIT 1;")
        If Cool.Count > 0 Then
            If Cool(0)(4) = "LOGIN" Then
                LoggedIn = True
                FriendlyText = "Logged in at " + Cool(0)(3)
                Computer = Cool(0)(3)
                Time = Cool(0)(5)
            Else
                LoggedIn = False
                FriendlyText = "Last seen on " + Cool(0)(3) + " at " + Cool(0)(5)
                Computer = Cool(0)(3)
                Time = Cool(0)(5)
            End If
        Else
            LoggedIn = False
            FriendlyText = "Never logged in."
            Computer = "None"
            Time = "Never"
        End If

    End Sub
    Public Sub New(PayrollId As Integer)
        id = PayrollId
        Update()
    End Sub
End Class

Public Class Employee
    Dim empID As Integer
    Dim fname As String
    Dim sname As String
    Dim hashpin As String
    Dim timeout As Integer
    Dim AreaStart As String
    Dim AreaEnd As String
    Dim pvisible As Boolean = True
    Public Permissions As EmployeeAuthCodes
    Public OldPin As String
    Dim LogStatus As EmpLoginStatus

    Public ReadOnly Property LoginStatus As EmpLoginStatus
        Get
            Return LogStatus
        End Get
    End Property
    Public ReadOnly Property PayrollId As Integer
        Get
            Return empID
        End Get
    End Property
    Public ReadOnly Property FullName As String
        Get
            Return fname + " " + sname
        End Get
    End Property
    Public ReadOnly Property HashedPin As String
        Get
            Return hashpin
        End Get
    End Property
    Public Sub SetNewPin(Newpin As String)
        hashpin = BATS(GenerateHash(Newpin))
        'Save the new pin.
        MySql.insertupdate("UPDATE whldata.employees SET HashedPin='" + hashpin + "' WHERE PayrollNo=" + empID.ToString + ";")
    End Sub
    Private Function GenerateHash(StringToHash As String) As Byte()
        If StringToHash = Nothing Then StringToHash = " "
        Dim BA As Byte() = System.Text.ASCIIEncoding.UTF8.GetBytes(StringToHash)
        Return New System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(BA)
    End Function

    Private Function BATS(ByVal arrInput() As Byte) As String
        Dim i As Integer
        Dim sOutput As New StringBuilder(arrInput.Length)
        For i = 0 To arrInput.Length - 1
            sOutput.Append(arrInput(i).ToString("X2"))
        Next
        Return sOutput.ToString()
    End Function

    Public Function CheckPin(TryPin As String) As Boolean
        If BATS(GenerateHash(TryPin)) = hashpin Then
            Return True
        Else
            Throw New Exceptions.WrongPinExceptoin("The entered pin was incorrect.")
        End If
    End Function
    Public Property LoginTimeout As Integer
        Get
            Return timeout
        End Get
        Set(value As Integer)
            timeout = value
        End Set
    End Property
    Public Property Visible As Boolean
        Get
            Return pvisible
        End Get
        Set(value As Boolean)
            pvisible = value
        End Set
    End Property
    Public Function SaveData()
        ' ========================================================================================================= SAVE DATA
        Return True
    End Function


    Public Sub New(Raw As ArrayList)
        empID = Raw(0)
        fname = Raw(1)
        sname = Raw(2)
        If IsDBNull(Raw(3)) Then AreaStart = Nothing Else AreaStart = Raw(3)
        If IsDBNull(Raw(4)) Then AreaEnd = Nothing Else AreaEnd = Raw(4)
        If IsDBNull(Raw(6)) Then timeout = 300 Else timeout = Raw(6)
        If Raw(7) = 1 Then Visible = False Else Visible = True
        Permissions = New EmployeeAuthCodes(Raw(8).ToString)
        If IsDBNull(Raw(10)) Then hashpin = Nothing Else hashpin = Raw(10)
        If IsDBNull(Raw(5)) Then OldPin = Nothing Else OldPin = Raw(5)
        If Visible Then
            LogStatus = New EmpLoginStatus(empID)
        Else
            LogStatus = Nothing
        End If

    End Sub


End Class
Public Class EmployeeAuthCodes
    Dim EmpId As Integer

    'Premissions
    Dim puseAutobag As Boolean              'C
    Dim pApprHoliday As Boolean             'F
    Dim RotaMana As Boolean                 'H
    Dim EditEmp As Boolean                  'I
    Dim DelEmp As Boolean                   'J
    Dim HolNotifTarget As Boolean           'K
    Dim RotaHistory As Boolean              'L
    Dim ModHolAllowance As Boolean          'M
    Dim pSaveChangesToSku As Boolean        'D
    '30/12/2015     Added the following 2 as detailed below. 
    Dim pResetPin As Boolean                'A
    Dim pSendBatchMessage As Boolean        'B


    '17/12/2015     Added the SaveChangesToSku permission with code "D". Reset Pin and Batch Message sending are yet to be implemented, but are on the
    '               UAC sheet noted with AW. 

    'New constructor
    Public Sub New(Codes As String)
        If Codes.Contains("C") Then puseAutobag = True
        If Codes.Contains("F") Then pApprHoliday = True
        If Codes.Contains("H") Then RotaMana = True
        If Codes.Contains("I") Then EditEmp = True
        If Codes.Contains("J") Then DelEmp = True
        If Codes.Contains("K") Then HolNotifTarget = True
        If Codes.Contains("L") Then RotaHistory = True
        If Codes.Contains("M") Then ModHolAllowance = True
        If Codes.Contains("D") Then pSaveChangesToSku = True

        '30/12/2015     Added "Reset Pin" and "Send Batch Messages" permissions which have been on the auth sheet for like 2 weeks. 
        If Codes.Contains("A") Then pResetPin = True
        If Codes.Contains("B") Then pSendBatchMessage = True
    End Sub

    '30/12/2015     Added the nfollowing 2 properties for the reason detailed just above. 

    'Save changes to SKU data. 
    Public Property AllowedToResetPin() As Boolean
        Get
            Return pResetPin
        End Get
        Set(value As Boolean)
            pResetPin = value
        End Set
    End Property

    'Save changes to SKU data. 
    Public Property CanSendBatchMessages() As Boolean
        Get
            Return pSendBatchMessage
        End Get
        Set(value As Boolean)
            pSendBatchMessage = value
        End Set
    End Property

    'Save changes to SKU data. 
    Public Property SaveChangesToSKU() As Boolean
        Get
            Return pSaveChangesToSku
        End Get
        Set(value As Boolean)
            pSaveChangesToSku = value
        End Set
    End Property

    'The employee ID for saving
    Public Property EmployeeId() As Integer
        Get
            Return EmpId
        End Get
        Set(value As Integer)
            EmpId = value
        End Set
    End Property


    'Properties for permissions
    Public Property UsePrepackAutobag As Boolean
        Get
            Return puseAutobag
        End Get
        Set(value As Boolean)
            puseAutobag = value
            SaveAuth()
        End Set
    End Property
    Public Property ManageRota As Boolean
        Get
            Return RotaMana
        End Get
        Set(value As Boolean)
            RotaMana = value
            SaveAuth()
        End Set
    End Property
    Public Property ApproveHolidays As Boolean
        Get
            Return pApprHoliday
        End Get
        Set(value As Boolean)
            pApprHoliday = value
            SaveAuth()
        End Set
    End Property
    Public Property EditEmployee As Boolean
        Get
            Return EditEmp
        End Get
        Set(value As Boolean)
            EditEmp = value
            SaveAuth()
        End Set
    End Property
    Public Property DeleteEmployee As Boolean
        Get
            Return DelEmp
        End Get
        Set(value As Boolean)
            DelEmp = value
            SaveAuth()
        End Set
    End Property
    Public Property RecieveHolidayNotifications As Boolean
        Get
            Return HolNotifTarget
        End Get
        Set(value As Boolean)
            HolNotifTarget = value
            SaveAuth()
        End Set
    End Property
    Public Property RotaHistoryView As Boolean
        Get
            Return RotaHistory
        End Get
        Set(value As Boolean)
            RotaHistory = value
            SaveAuth()
        End Set
    End Property
    Public Property ChangeHolidayAllowance As Boolean
        Get
            Return ModHolAllowance
        End Get
        Set(value As Boolean)
            ModHolAllowance = value
            SaveAuth()
        End Set
    End Property


    'Save it all
    Private Sub SaveAuth()
        Dim AuthStr As String = ""
        If puseAutobag Then AuthStr = AuthStr + "C"
        If pApprHoliday Then AuthStr = AuthStr + "F"
        If EditEmp Then AuthStr = AuthStr + "I"
        If DelEmp Then AuthStr = AuthStr + "J"
        If HolNotifTarget Then AuthStr = AuthStr + "K"
        If RotaHistory Then AuthStr = AuthStr + "L"
        If ModHolAllowance Then AuthStr = AuthStr + "M"
        If RotaMana Then AuthStr = AuthStr + "H"
    End Sub

End Class
