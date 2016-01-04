Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary

Public Class Envelope
    Public Code As String
    Public Name As String
    Public Weight As Integer
    Public Brand As String
    Public Size As String
    Public BoxQuantity As Integer
    Public BoxPrice As Single
    Public IndividualPrice As Single
End Class

Public Class GenericDataController

    Public Sub SaveDataToFile(Filename As String, ObjectToSave As Object, Optional folder As String = Nothing)
        ' Create file by FileStream class
        Dim fs As FileStream
        If IsNothing(folder) Then
            fs = New FileStream(My.Computer.FileSystem.SpecialDirectories.CurrentUserApplicationData + "\" + Filename + ".dca", FileMode.OpenOrCreate)
        Else
            fs = New FileStream(folder + "\" + Filename + ".dca", FileMode.OpenOrCreate)
        End If


        ' Creat binary object
        Dim bf As New BinaryFormatter()

        ' Serialize object to file
        Try
            bf.Serialize(fs, ObjectToSave)
            fs.Close()
        Catch ex As Exception

        End Try

    End Sub



End Class

Public Class Supplier
    Dim DBID As Integer
    Public Code As String
    Public Name As String
    Public SupportsEmbeddedOrdering As Boolean
    Dim EmbeddedOrderString As String
    Public Active As Boolean
    Public Children As New List(Of WhlSKU)
    Public Website As String

    Public Mixdown As New List(Of WhlSKU)

    Public Sub New(Raw As ArrayList)
        DBID = Raw(0)
        Code = Raw(1)
        Name = Raw(2)
        If IsDBNull(Raw(3)) Then
            Website = "http://ad.whitehinge.com/reorder/nosite.html"
        Else
            Website = Raw(3)
        End If
        If IsDBNull(Raw(4)) Then
            SupportsEmbeddedOrdering = False
            EmbeddedOrderString = Nothing
        Else
            SupportsEmbeddedOrdering = True
            EmbeddedOrderString = Raw(4)
        End If
        If Raw(5) = 1 Then
            Active = True
        Else
            Active = False
        End If
    End Sub

    Dim ShortSkusAndTotalsWeighted As New ArrayList

    Public Sub MakeMixdown()
        ShortSkusAndTotalsWeighted.Clear()
        Mixdown.Clear()
        For Each child As WhlSKU In Children
            'Search to find the short sku if it's there.
            Dim DoesExist = False
            For Each shorty As ArrayList In ShortSkusAndTotalsWeighted
                Try
                    If shorty(0) = child.ShortSku Then
                        'Found it, so add it. 
                        'Field 0 is the short sku, Field 1 is the data, which gets accumulated.
                        Dim Additied As Integer
                        'Multiply weighted by Packsize.
                        Additied = child.PackSize * child.SalesData.WeightedAverage

                        'Then add it on
                        shorty(1) = shorty(1) + Additied

                        'Finish up and record it finished. 
                        DoesExist = True

                    End If
                Catch ex As Exception
                    'Carry on.
                End Try
            Next
            If DoesExist = False Then
                'Add a new one.
                'First, create the new arraylist
                Dim newshorty As New ArrayList
                newshorty.Add(child.ShortSku) 'Add the shortsku
                newshorty.Add(child.PackSize * child.SalesData.WeightedAverage) 'Add the intial value for this child.
                ShortSkusAndTotalsWeighted.Add(newshorty)
            End If

            'Now we've got every data recorded in the dirty arraylist of doom, and we can refer to that when we need some sales figures. I'm gonna make it public so they can do that,. 

            Dim Addpls As Boolean = True
            For Each shortsku As WhlSKU In Mixdown
                If child.ShortSku = shortsku.ShortSku Then
                    Addpls = False
                End If
            Next
            If Addpls Then
                Mixdown.Add(child)
            End If

        Next
    End Sub

    Public Function RetrieveTheForce(Shortsku As String) As ArrayList
        For Each shorty As ArrayList In ShortSkusAndTotalsWeighted
            If shorty(0).ToString = Shortsku.ToString Then
                Return shorty
            End If
        Next
        Return Nothing
    End Function

    Public Function GenerateURL(Query As String)
        Return EmbeddedOrderString.Replace("{query}", Query)
    End Function

End Class


Public Class SupplierCollection
    Inherits List(Of Supplier)

    Public Skus As SkuCollection

    Public Sub New()
        'Get the suppliers data.
        Dim Data As Object = MySql.SelectData("SELECT * FROM whldata.suppliers")
        Try
            For Each Supp As ArrayList In Data
                Add(New Supplier(Supp))
            Next
        Catch ex As Exception
            MsgBox(Data.ToString)
        End Try

    End Sub

    Public StatusString As String = "Downloading Items"
    Public StatusProgress As Integer
    Public StatusTotal As Integer

    Public Sub SortItemsBySupplier(Optional sender As Object = Nothing, Optional e As System.ComponentModel.DoWorkEventArgs = Nothing, Optional SkuColl As SkuCollection = Nothing)

        For Each supp As Supplier In Me
            supp.Children.Clear()
            supp.Mixdown.Clear()
        Next

        Dim WarningShown As Boolean = False
        If IsNothing(SkuColl) Then
            Skus = New SkuCollection
        Else
            Skus = SkuColl
        End If
        StatusTotal = Skus.Count
        Dim prog As Integer = 0
        For Each TempItem As WhlSKU In Skus
            prog = prog + 1
            StatusString = "Sorting Items (" + prog.ToString + " of " + Skus.Count.ToString + ")"
            StatusProgress = prog
            Try
                TempItem.SalesData.DownloadData()
            Catch ex As Exception
                If Not WarningShown Then
                    MsgBox("There Is some sales data missing For at least one product. You should re-run the sales data compiler For better results.")

                    WarningShown = True
                End If
            End Try

            'Save it to each supplier.
            For Each supp As Supplier In Me
                For Each skus As SKUSupplier In TempItem.Suppliers
                    If supp.Code.ToLower = skus.Name.ToLower Then
                        'Add it
                        supp.Children.Add(TempItem)
                    End If
                Next
            Next
            System.Windows.Forms.Application.DoEvents()

        Next

        'Go through again, find out how many weeks worth of stock we have left. 

        For Each supp As Supplier In Me
            supp.MakeMixdown()
            StatusString = "Mixing down for " + supp.Code
            For Each Sku As WhlSKU In supp.Mixdown
                Sku.CustomData.Clear()
                Sku.CustomData.Add(supp.RetrieveTheForce(Sku.ShortSku)(1)) 'Add the raw one.
                Dim weekstock As Single
                Try
                    weekstock = Sku.Stock.Level / Sku.CustomData(0)
                Catch ex As Exception
                    weekstock = 999
                End Try
                If Single.IsInfinity(weekstock) Then
                    weekstock = 999
                ElseIf Single.IsNaN(weekstock) Then
                    weekstock = 999

                End If
                Sku.CustomData.Add(weekstock) 'Then add the week one

                System.Windows.Forms.Application.DoEvents()
            Next
        Next
        StatusString = "Finishing Up"
    End Sub
End Class


Public Class PasswordObj
    Dim User As String
    Dim Passworda As String
    Dim UserElementa As String
    Dim PasswordElementa As String
    Dim FormSubmitElementa As String
    Dim LoginPageFragmenta As String
    Dim Websitea As String

    Public Property UserName() As String
        Get
            Return User
        End Get
        Set(value As String)
            User = value
        End Set
    End Property
    Public Property Password() As String
        Get
            Return Passworda
        End Get
        Set(value As String)
            Passworda = value
        End Set
    End Property
    Public Property UserNameBox() As String
        Get
            Return UserElementa
        End Get
        Set(value As String)
            UserElementa = value
        End Set
    End Property
    Public Property PasswordBox() As String
        Get
            Return PasswordElementa
        End Get
        Set(value As String)
            PasswordElementa = value
        End Set
    End Property
    Public Property SubmitButton() As String
        Get
            Return FormSubmitElementa
        End Get
        Set(value As String)
            FormSubmitElementa = value
        End Set
    End Property
    Public Property LoginPage() As String
        Get
            Return LoginPageFragmenta
        End Get
        Set(value As String)
            LoginPageFragmenta = value
        End Set
    End Property
    Public Property Website() As String
        Get
            Return Websitea
        End Get
        Set(value As String)
            Websitea = value
        End Set
    End Property

End Class
Public Class PasswordCollection
    Inherits List(Of PasswordObj)

    Public Function GetPasswordForSite(URL As String) As PasswordObj
        For Each obj As PasswordObj In Me
            If obj.Website = URL Then
                Return obj
            End If
        Next
        Return Nothing
    End Function

End Class
