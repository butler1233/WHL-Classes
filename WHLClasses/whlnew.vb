Public Class SkuCollection
    Inherits List(Of WhlSKU)
    Public Sub New()
        DownloadSKUs()
    End Sub

    Public Function DownloadSKUs()
        'Download everything we have
        Dim ProdList As ArrayList = MySql.SelectData("SELECT * FROM whldata.whlnew")
        For Each Sku As ArrayList In ProdList
            Add(New WhlSKU(Sku))
        Next
        Return Count

    End Function

    Public Function SearchSKUS(SearchTerm As String) As List(Of WhlSKU)
        Dim returningList As New List(Of WhlSKU)
        For Each SKU As WhlSKU In Me
            If SKU.SearchKeywords.Contains(SearchTerm) Then
                returningList.Add(SKU)
            End If
        Next
        Return returningList
    End Function


End Class
Public Class WhlSKU
    ''' <summary>
    ''' This function will save all properties to the database.
    ''' </summary>
    ''' <returns>True if changes were saved successfully, or false otherwise.</returns>
    Public Function SaveChanges() As Boolean
        '============================================================================ Aww shit here we go...
        Dim GottaGoFast As String = "" 'This is going to be our query. And it's going to be ungodly long. 
        'GottaGoFast = 


    End Function

    ''' <summary>
    ''' Returns the cheapest supplier object attached to this product.
    ''' </summary>
    ''' <returns></returns>
    Public Function CheapestSupplier() As SKUSupplier
        'Cool
        Dim cheapest As SKUSupplier = Suppliers(0)
        For Each supp As SKUSupplier In Suppliers
            If supp.Price < cheapest.Price Then
                cheapest = supp
            End If
        Next
        Return cheapest
    End Function


    Public Sub New(Data As ArrayList)

        'Initialate



        'Basic
        SKU = Data(0)
        ShortSku = Data(80)
        PackSize = Data(15)
        'Pieces = Data(16)

        'Misc
        DeliveryNote = Data(31)

        'SkuStock
        If Data(1) = "" Then
            Stock.Level = 0
        Else
            Try
                Stock.Level = Data(1)
            Catch ex As Exception
                Stock.Level = 0
            End Try
        End If

        If Data(2) = "" Then
            Stock.Minimum = 0
        Else
            Try
                Stock.Minimum = Data(2)
            Catch ex As Exception
                Stock.Minimum = 0
            End Try
        End If
        If Data(3) = "" Then
            Stock.Dormant = 0
        Else
            Try
                Stock.Dormant = Data(3)
            Catch ex As Exception
                Stock.Dormant = 0
            End Try
        End If
        Stock.Total = Data(4)

        'SKUPrices
        Price.Gross = Data(7)

        Price.Net = Data(6)
        Price.Retail = Data(8)
        If Data(9) = "NSL" Then
            Price.Profit = 0
        Else
            Try
                Price.Profit = Data(9)
            Catch ex As Exception
                Price.Profit = 0
            End Try
        End If
        If Data(10) = "NSL" Then
            Price.Margin = 0
        Else
            Try
                Price.Margin = Data(10)
            Catch ex As Exception
                Price.Margin = 0
            End Try
        End If


        'SKUTitles
        Title.Invoice = Data(5)
        Title.Label = Data(98)
        Title.Linnworks = Data(99)
        If IsDBNull(Data(101)) Then
            Title.NewItem = ""
        Else
            Try
                Title.NewItem = Data(101)
            Catch ex As Exception
                Title.NewItem = ""
            End Try
        End If

        'Images
        If Data(40).ToString.Length > 0 Then
            Dim NewImg As New SKUImage
            NewImg.ImagePath = Data(40)
            Images.Add(NewImg)
        End If

        If Data(41).ToString.Length > 0 Then
            Dim NewImg As New SKUImage
            NewImg.ImagePath = Data(41)
            Images.Add(NewImg)
        End If

        If Data(42).ToString.Length > 0 Then
            Dim NewImg As New SKUImage
            NewImg.ImagePath = Data(42)
            Images.Add(NewImg)
        End If

        'Suppliers
        Dim NewSupplierTable As Object = MySql.SelectData("SELECT * FROM whldata.sku_supplierdata WHERE SKU='" + SKU + "';")
        Try
            Dim NewArr As ArrayList = NewSupplierTable
            If NewArr.Count > 0 Then
                'We have records!
                For Each Supp As ArrayList In NewArr
                    Dim NewSupp As New SKUSupplier

                    If Supp(8).ToString = "True" Then
                        NewSupp.Primary = True
                    Else
                        NewSupp.Primary = False
                    End If
                    If Supp(9).ToString = "True" Then
                        NewSupp.Discontinued = True
                    Else
                        NewSupp.Discontinued = False 'Why does the name disappear here?
                    End If
                    If Supp(10).ToString = "True" Then
                        NewSupp.OutOfStock = True
                    Else
                        NewSupp.OutOfStock = False
                    End If
                    'Moved because the name keeps getting dropped
                    NewSupp.Name = Supp(2).ToString
                    NewSupp.ReOrderCode = Supp(3).ToString
                    NewSupp.Barcode = Supp(4).ToString
                    NewSupp.CaseBarcode = Supp(5).ToString
                    NewSupp.LeadTime = Supp(12)
                    NewSupp.Price = Supp(7)
                    NewSupp.LastModified = Supp(11)

                    Suppliers.Add(NewSupp)
                Next
            Else
                Throw New Exception("None exist.")
            End If
        Catch ex As Exception
            Console.Write(ex.Message + ", using fallback for SUPPLIERS")
            'Fallback for the olden method.
            If Data(19).ToString.Length > 0 Then
                Dim NewSupp As New SKUSupplier
                NewSupp.Name = Data(19)
                NewSupp.ReOrderCode = Data(20)
                NewSupp.Barcode = Data(21)
                NewSupp.CaseBarcode = Data(22)
                NewSupp.LeadTime = 4
                NewSupp.Primary = True
                NewSupp.Price = Price.Net
                NewSupp.LastModified = "Old Type- Not Found"
                Suppliers.Add(NewSupp)
            End If
            If Data(23).ToString.Length > 0 Then
                Dim NewSupp As New SKUSupplier
                NewSupp.Name = Data(23)
                NewSupp.ReOrderCode = Data(24)
                NewSupp.Barcode = Data(25)
                NewSupp.CaseBarcode = Data(26)
                NewSupp.LeadTime = Data(58)
                NewSupp.Price = Price.Net
                NewSupp.LastModified = "Old Type- Not Found"
                Suppliers.Add(NewSupp)
            End If
            If Data(27).ToString.Length > 0 Then
                Dim NewSupp As New SKUSupplier
                NewSupp.Name = Data(27)
                NewSupp.ReOrderCode = Data(28)
                Try
                    NewSupp.Barcode = Data(29)
                Catch exa As Exception
                    NewSupp.Barcode = ""
                End Try
                Try
                    NewSupp.CaseBarcode = Data(30)
                Catch exa As Exception
                    NewSupp.CaseBarcode = ""
                End Try
                NewSupp.LeadTime = Data(58)
                NewSupp.Price = Price.Net
                NewSupp.LastModified = "Old Type- Not Found"
                Suppliers.Add(NewSupp)
            End If
        End Try


        'Locations
        Dim NewLocs As Object = MySql.SelectData("SELECT * FROM whldata.sku_locations WHERE Sku='" + SKU + "';")
        Try
            Dim NewArr As ArrayList = NewLocs
            If NewArr.Count > 0 Then
                'We have records!
                For Each line As ArrayList In NewArr
                    Dim NewLoc As New SKULocation
                    NewLoc.LocationTableID = line(0)
                    NewLoc.LocalLocationName = line(1)
                    Locations.Add(NewLoc)
                Next

            Else
                Throw New Exception("None exist.")
            End If
        Catch ex As Exception
            Console.Write(ex.Message + ", using fallback for LCOATIONS")
            'Fallback for the olden method.
            If Data(51).ToString.Length > 0 Then
                'Basic Location.
                Dim NewLoc As New SKULocation
                NewLoc.LocationTableID = Nothing
                NewLoc.LocalLocationName = Data(51)
                Locations.Add(NewLoc)
            End If
        End Try


        'Extended Properties
        ExtendedProperties.GS1Barcode = Data(85)
        If Data(67).ToString.Length > 0 Then
            ExtendedProperties.Screws = Data(67)
            ExtendedProperties.HasScrews = True
        Else
            ExtendedProperties.Screws = 0
            ExtendedProperties.HasScrews = False
        End If
        Try
            ExtendedProperties.Parts = Data(66)
        Catch ex As Exception
            ExtendedProperties.Parts = 0
        End Try


        SalesData.SetSku(SKU)

        'Search Terms
        SearchKeywords.Add(ExtendedProperties.GS1Barcode)
        SearchKeywords.Add(SKU)
        SearchKeywords.Add(ShortSku)
        SearchKeywords.Add(ShortSku.Substring(2))
        For Each Supplier As SKUSupplier In Suppliers
            SearchKeywords.Add(Supplier.Barcode)
            SearchKeywords.Add(Supplier.CaseBarcode)
        Next
        For Each Shelf As SKULocation In Locations
            SearchKeywords.Add(Shelf.LocalLocationName)
        Next




    End Sub

    Public Function SearchSupplierByCode(Code As String) As SKUSupplier
        For Each supp As SKUSupplier In Suppliers
            If supp.Name.ToLower = Code.ToLower Then
                Return supp
            End If
        Next
        Return Nothing
    End Function

    'This is where we keep all of the main data.

    Public SKU As String
    Public ShortSku As String
    Public Stock As New SKUStock
    Public Price As New SKUPrices
    Public Title As New SKUTitles
    Public Images As New List(Of SKUImage)
    Public Suppliers As New List(Of SKUSupplier)
    Public Postage As New SKUPost
    Public PrepackInfo As New SKUPrepack
    Public Pieces As Integer
    Public DeliveryNote As String
    Public Costs As New SKUCosts
    Public Locations As New List(Of SKULocation)
    Public SalesData As New SKUSalesData
    Public PackSize As Integer
    Public ExtendedProperties As New SKUExtended
    Public SearchKeywords As New List(Of String)

    Public CustomData As New List(Of Object)
    'And here we have some properties. 
End Class

Public Class SKUSalesData
    'Remember, we don't download this automatically
    Dim Sku As String
    Dim Weighted As Integer
    Dim Raw8 As Integer
    Dim Raw4 As Integer
    Dim Raw1 As Integer
    Dim Avg8 As Integer
    Dim Avg4 As Integer
    Dim Avg1 As Integer

    Public ReadOnly Property WeightedAverage
        Get
            Return Weighted
        End Get
    End Property
    Public ReadOnly Property EightWeekTotal
        Get
            Return Raw8
        End Get
    End Property
    Public ReadOnly Property FourWeekTotal
        Get
            Return Raw4
        End Get
    End Property
    Public ReadOnly Property OneWeekTotal
        Get
            Return Raw1
        End Get
    End Property
    Public ReadOnly Property EightWeekAverage
        Get
            Return Avg8
        End Get
    End Property
    Public ReadOnly Property FourWeekAverage
        Get
            Return Avg4
        End Get
    End Property
    Public ReadOnly Property OneWeekAverage
        Get
            Return Avg1
        End Get
    End Property

    Public Sub SetSku(NewSKU As String)
        Sku = NewSKU
    End Sub
    Public Sub DownloadData()
        Dim Resp As ArrayList = MySql.SelectData("SELECT * FROM whldata.salesdata WHERE Sku='" + Sku + "';")
        Raw8 = Resp(0)(1)
        Raw4 = Resp(0)(2)
        Raw1 = Resp(0)(3)
        Weighted = Resp(0)(4)
        Avg8 = Resp(0)(5)
        Avg4 = Resp(0)(6)
        Avg1 = Resp(0)(7)

    End Sub
End Class
Public Class SKUExtended
    Public Parts As Integer
    Public Screws As Integer
    Public HasScrews As Boolean
    Public GS1Barcode As String

End Class
Public Class SKULocation
    Public LocationTableID As Integer
    Public LocalLocationName As String


End Class
Public Class SKUCosts
    Public Packing As Single
    Public Postage As Single
    Public Envelope As Single
    Public Fees As Single
    Public Labour As Single
    Public VAT As Single
    Public Total As Single
End Class
Public Class SKUSupplier
    Dim SuppName As String
    Dim SuppCode As String
    Dim SuppPrice As Single
    Dim SuppCaseCode As String
    Dim SuppBarCode As String
    Dim SuppDisCon As Boolean = False
    Dim SuppPrimary As Boolean = False
    Dim SuppLead As Integer
    Dim SuppNoStock As Boolean
    Dim Modified As String


    ''' <summary>
    ''' Weeks which are the "lead time"
    ''' </summary>
    ''' <returns></returns>
    Public Property LastModified As String
        Get
            Return Modified
        End Get
        Set(value As String)
            Modified = value
        End Set
    End Property

    ''' <summary>
    ''' Weeks which are the "lead time"
    ''' </summary>
    ''' <returns></returns>
    Public Property LeadTime As Integer
        Get
            Return SuppLead
        End Get
        Set(value As Integer)
            SuppLead = value
        End Set
    End Property

    ''' <summary>
    ''' The 3 character supplier code.
    ''' </summary>
    ''' <returns></returns>
    Public Property OutOfStock As Boolean
        Get
            Return SuppNoStock
        End Get
        Set(value As Boolean)
            SuppNoStock = value
        End Set
    End Property

    ''' <summary>
    ''' The 3 character supplier code.
    ''' </summary>
    ''' <returns></returns>
    Public Property Name As String
        Get
            Return SuppName
        End Get
        Set(value As String)
            SuppName = value
        End Set
    End Property

    ''' <summary>
    ''' The re-order code for the supplier which identifies the product on the supplier's site. 
    ''' </summary>
    ''' <returns></returns>
    Public Property ReOrderCode As String
        Get
            Return SuppCode
        End Get
        Set(value As String)
            SuppCode = value
        End Set
    End Property


    ''' <summary>
    ''' The last recorded price that the item was ordered for.
    ''' </summary>
    ''' <returns></returns>
    Public Property Price As Single
        Get
            Return SuppPrice
        End Get
        Set(value As Single)
            SuppPrice = value
        End Set
    End Property

    ''' <summary>
    ''' The barcode on the outer box. this is sometimes the same as the normal barcode. 
    ''' </summary>
    ''' <returns></returns>
    Public Property CaseBarcode As String
        Get
            Return SuppCaseCode
        End Get
        Set(value As String)
            SuppCaseCode = value
        End Set
    End Property

    ''' <summary>
    ''' The barcode on the product itself.
    ''' </summary>
    ''' <returns></returns>
    Public Property Barcode As String
        Get
            Return SuppBarCode
        End Get
        Set(value As String)
            SuppBarCode = value
        End Set
    End Property

    ''' <summary>
    ''' This value is set to true of the product has been discontinued. 
    ''' </summary>
    ''' <returns></returns>
    Public Property Discontinued As Boolean
        Get
            Return SuppDisCon
        End Get
        Set(value As Boolean)
            SuppDisCon = value
        End Set
    End Property
    ''' <summary>
    ''' This values states whether this supplier is the primary one or not.
    ''' </summary>
    ''' <returns></returns>
    Public Property Primary As Boolean
        Get
            Return SuppPrimary
        End Get
        Set(value As Boolean)
            SuppPrimary = value
        End Set
    End Property
End Class
Public Class SKUStock
    Dim slevel As Integer
    Dim smin As Integer
    Dim sdormant As Integer
    Dim stotal As Integer

    Public Property Level As Integer
        Get
            Return slevel
        End Get
        Set(value As Integer)
            slevel = value
        End Set
    End Property
    Public Property Minimum As Integer
        Get
            Return smin
        End Get
        Set(value As Integer)
            smin = value
        End Set
    End Property
    Public Property Dormant As Integer
        Get
            Return sdormant
        End Get
        Set(value As Integer)
            sdormant = value
        End Set
    End Property
    Public Property Total As Integer
        Get
            Return stotal
        End Get
        Set(value As Integer)
            stotal = value
        End Set
    End Property
End Class
Public Class SKUPrices
    Public Net As Single
    Public Gross As Single
    Public Retail As Single
    Public Profit As Single
    Public Margin As Single
End Class
Public Class SKUTitles
    Public Invoice As String
    Public Label As String
    Public Linnworks As String
    Public NewItem As String
End Class
Public Class SKUImage
    Public ImagePath As String
    Public FullImagePath As String
    Public ImageData As System.Drawing.Image

End Class
Public Class SKUPost
    'ER..
    Public UseCourier As Boolean
    Public Weight As Integer
    'Public Envelope As Envelope


End Class
Public Class SKUPrepack
    Public Bag As String
    Public Notes As String
    Public PrepackLog As List(Of PrepackLog)
End Class
