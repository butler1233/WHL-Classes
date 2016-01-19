Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary

<System.Serializable()>
Public Class SkuCollection
    Inherits List(Of WhlSKU)
    Public Sub New(Optional cancel As Boolean = False)
        If Not cancel Then
            DownloadSKUs()
        End If

    End Sub
    Public Progress As Integer = 0
    Public Total As Integer = 0

    Public Function GetItemsInBox(Box As Integer) As SkuCollection
        Dim ReturningList As New SkuCollection(True)
        For Each sku As WhlSKU In Me
            If sku.NewItem.Box = Box.ToString Then
                ReturningList.Add(sku)
            End If
        Next
        Return ReturningList
    End Function

    Public Function DownloadSKUs()
        'Download everything we have
        Dim Ink As Integer = 1
        Dim ProdList As ArrayList = MySql.SelectData("SELECT * FROM whldata.whlnew")
        Total = ProdList.Count()

        For Each Sku As ArrayList In ProdList
            CountDone = Ink.ToString + " of " + ProdList.Count.ToString
            Progress = Ink
            Add(New WhlSKU(Sku))
            System.Windows.Forms.Application.DoEvents()
            Ink += 1
        Next
        Return Count

    End Function

    Public Function SearchSKUS(SearchTerm As String) As SkuCollection
        Dim returningList As New SkuCollection(True)
        For Each SKU As WhlSKU In Me

            For Each searchy As String In SKU.SearchKeywords
                If searchy.ToLower.Contains(SearchTerm.ToLower) Then
                    returningList.Add(SKU)
                End If
            Next
        Next
        Return DupeFilter(returningList)
    End Function

    Public Function DupeFilter(InitColl As SkuCollection) As SkuCollection
        Dim SKUList As New List(Of String)
        Dim ReturnColl As New SkuCollection(True)
        For Each sku As WhlSKU In InitColl
            If SKUList.Contains(sku.SKU) Then
                'SKIP
            Else
                SKUList.Add(sku.SKU)
                ReturnColl.Add(sku)
            End If
        Next
        Return ReturnColl
    End Function

    Public CountDone As String = ""

    Public Function MakeMixdown() As SkuCollection
        Dim mixdown As New SkuCollection(True)
        For Each child As WhlSKU In Me
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
        Return mixdown
    End Function

End Class


Public Class SkusDataController
    Public Sub SaveData(Collection As SkuCollection)
        ' Create file by FileStream class
        Dim fs As FileStream = New FileStream(My.Computer.FileSystem.SpecialDirectories.CurrentUserApplicationData + "\SkusCache.bin", FileMode.OpenOrCreate)

        ' Creat binary object
        Dim bf As New BinaryFormatter()

        ' Serialize object to file
        bf.Serialize(fs, Collection)
        fs.Close()


    End Sub

    Public Function LoadDataFile() As SkuCollection
        Dim bf As New BinaryFormatter()
        ' Open file and deserialize to object again
        Dim fsRead As New FileStream(My.Computer.FileSystem.SpecialDirectories.CurrentUserApplicationData + "\SkusCache.bin", FileMode.Open)
        Dim Cool As SkuCollection = bf.Deserialize(fsRead)
        fsRead.Close()
        Return Cool
    End Function
End Class

<System.Serializable()>
Public Class WhlSKU

    Public Sub IncludeImages()
        For Each imageitem As SKUImage In Images
            Try
                imageitem.ImageData = System.Drawing.Image.FromFile(imageitem.ImagePath)
            Catch ex As Exception

            End Try

        Next
    End Sub

    '18/01/2016     Added cost recalculation to whlsku inline, but added it as a Public sub so recalculation can be done later. This might be an awful idea. We'll try it.
    Public Function RecalculateCosts(Envelope As String, RetailPrice As Single) As Single
        Dim Envman As New EnvelopeCollection
        Dim Env As Envelope = Envman.GetEnvelope(Envelope)
        Dim Feeman As New Fees.FeeManager
        Dim Ispacket As Boolean = Envelope.StartsWith("P")
        If Not IsNothing(Env) Then
            Costs.Envelope = Feeman.GetEnvelopePrice(Env, True)
            Costs.Postage = Feeman.GetPostagePrice(Profile.Weight, Ispacket, ExtendedProperties.NeedsCourier)
        Else
            Costs.Envelope = 0
            Costs.Postage = 0
        End If



        Costs.Packing = Costs.Postage + Costs.Envelope
            Costs.Fees = Feeman.GetListingFees(RetailPrice)
            Costs.Labour = Feeman.GetLabourPrice(ExtendedProperties.LabourCode)
            Costs.VAT = Feeman.GetVATCost(RetailPrice)
            Costs.Total = Costs.Packing + Costs.Fees + Costs.Labour + Costs.VAT


            Return Costs.Total

    End Function
    ''' <summary>
    ''' This function will save all properties to the database.
    ''' </summary>
    ''' <returns>Returns a list of strings which display the update status of each element of the product in a user fiendly way.</returns>
    Public Function SaveChanges(LogUser As Employee, Optional LogReason As String = "None provided") As List(Of String)

        Dim SaveStatuses As New List(Of String)


        '============================================================================ Aww shit here we go...
        '14/01/2016     Added distinguish (ext35) to teh save, and load functions. 
        '15/01/2016     Added Inner (New_inner, 117), InitStock(initialquantity, 38), InitLevel(initiallevel, 39), InitMinimum(120), ListPriority(121), and
        '               IsListed(122) to save and load. 
        Dim WhlNewQuery As String
        WhlNewQuery = "REPLACE INTO whldata.whlnew (sku, itemtitle, Net, gross, retail, profit, margin, Envelope, weight, PackSize, labour, courier, DeliveryNote, packingcost, " _
            + " postagecost, envcost, feescost, labourcost, vatcost, totalcost, parts, screws, ext33, ShortSku, gs1, labelshort, linnshort, " _
            + " New_Brand, New_Description, New_Finish, New_Size, New_Note, New_TransferBox, New_Status, IsPair, ext35, New_Inner, initialquantity, initiallevel, InitMinimum, ListPriority, IsListed) VALUES" _
            + " ('" + SKU.ToString + "','" + Title.Invoice.ToString + "','" + Price.Net.ToString + "','" + Price.Gross.ToString + "','" + Price.Retail.ToString + "','" + Price.Profit.ToString + "','" _
            + Price.Margin.ToString + "','" + ExtendedProperties.Envelope.ToString + "','" + Profile.Weight.ToString + "','" + PackSize.ToString + "','" + ExtendedProperties.LabourCode.ToString _
            + "','" + ExtendedProperties.NeedsCourier.ToString + "','" + DeliveryNote.ToString + "','" + Costs.Packing.ToString + "','" + Costs.Postage.ToString + "','" + Costs.Envelope.ToString _
            + "','" + Costs.Fees.ToString + "','" + Costs.Labour.ToString + "','" + Costs.VAT.ToString + "','" + Costs.Total.ToString + "','" + ExtendedProperties.Parts.ToString + "','" _
            + ExtendedProperties.Screws.ToString + "','" + Category.ToString + "','" + ShortSku.ToString + "','" + ExtendedProperties.GS1Barcode.ToString + "','" + Title.Label.ToString + "','" _
            + Title.Linnworks.ToString + "','" + NewItem.Brand.ToString + "','" + NewItem.Description.ToString + "','" + NewItem.Finish.ToString + "','" + NewItem.Size.ToString + "','" _
            + NewItem.Note.ToString + "','" + NewItem.Box.ToString + "','" + NewItem.Status.ToString + "','" + ExtendedProperties.IsPair.ToString + "','" + Title.Distinguish.ToString + "','" _
            + ExtendedProperties.Inner.ToString + "','" + NewItem.InitStock.ToString + "','" + NewItem.InitLevel.ToString + "','" + NewItem.InitMinimum.ToString + "','" + NewItem.ListPriority.ToString + "','" _
            + NewItem.IsListed.ToString + "');"
        Dim Response As Object = MySql.insertupdate(WhlNewQuery)
        If Response.ToString.Length < 10 Then
            SaveStatuses.Add("Main Data saved successfully.")
        Else
            SaveStatuses.Add("Main Data Error: " + Response.ToString)
        End If

        'Now we save the locaitons to the locations table.
        For Each location As SKULocation In Locations
            Dim locationsquery As String
            If location.LocationTableID = 0 Then
                'Doesn't already exist in the table
                locationsquery = "INSERT INTO whldata.sku_locations (shelfName, Sku, additionalInfo) VALUES ('" + location.LocalLocationName.ToString + "','" + ShortSku.ToString + "','" + " " + "')"
            Else
                'Already exists and just needs updating
                locationsquery = "REPLACE INTO whldata.sku_locations (id, shelfName, Sku, additionalInfo) VALUES (" + location.LocationTableID.ToString + ",'" + location.LocalLocationName.ToString + "','" + ShortSku.ToString + "','" + " " + "')"
            End If

            Dim response1 As Object = MySql.insertupdate(locationsquery)
            If response1.ToString.Length < 10 Then
                SaveStatuses.Add("Location Data for '" + location.LocalLocationName + "' saved successfully.")
            Else
                SaveStatuses.Add("Location Data for '" + location.LocalLocationName + "' Error: " + response1.ToString)
            End If
        Next

        'Now we save the suppliers to the suppliersdata table.
        For Each supplier As SKUSupplier In Suppliers
            Dim suppquery As String
            If supplier.LastModified.Contains("Old Type") Then
                supplier.LastModified = Now.ToString("dd/MM/yyyy HH:mm:ss")
            End If
            If IsNothing(supplier.TableDataId) Then
                'Doesn't already exist in the table
                suppquery = "INSERT INTO whldata.sku_supplierdata (SKU, SupplierName, SupplierCode, SupplierBarcode, SupplierCaseBarcode, SupplierPricePer, " _
                + "isPrimary, isDiscontinued, isOutOfStock, DateModified, LeadTimeWeeks, SupplierCaseInnerBarcode, SupplierBoxCode) VALUES ('" _
                + ShortSku.ToString + "','" + supplier.Name.ToString + "','" + supplier.ReOrderCode.ToString + "','" + supplier.Barcode.ToString + "','" + supplier.CaseBarcode.ToString _
                + "','" + supplier.Price.ToString + "','" + supplier.Primary.ToString + "','" + supplier.Discontinued.ToString + "','" + supplier.OutOfStock.ToString + "','" +
                supplier.LastModified.ToString + "','" + supplier.LeadTime.ToString + "','" + supplier.CaseBarcodeInner.ToString + "','" + supplier.BoxCode.ToString + "')"
            Else
                'Already exists and just needs updating
                suppquery = "REPLACE INTO whldata.sku_supplierdata (DataId,SKU,SupplierName, SupplierCode, SupplierBarcode, SupplierCaseBarcode, SupplierPricePer, " _
                + "isPrimary, isDiscontinued, isOutOfStock, DateModified, LeadTimeWeeks, SupplierCaseInnerBarcode, SupplierBoxCode) VALUES (" + supplier.TableDataId.ToString + ",'" _
                + ShortSku.ToString + "','" + supplier.Name.ToString + "','" + supplier.ReOrderCode.ToString + "','" + supplier.Barcode.ToString + "','" + supplier.CaseBarcode.ToString _
                + "','" + supplier.Price.ToString + "','" + supplier.Primary.ToString + "','" + supplier.Discontinued.ToString + "','" + supplier.OutOfStock.ToString + "','" +
                supplier.LastModified.ToString + "','" + supplier.LeadTime.ToString + "','" + supplier.CaseBarcodeInner.ToString + "','" + supplier.BoxCode.ToString + "')"
            End If

            Dim response1 As Object = MySql.insertupdate(suppquery)
            If response1.ToString.Length < 10 Then
                SaveStatuses.Add("Supplier Data for '" + supplier.Name + "' saved successfully.")
            Else
                SaveStatuses.Add("Supplier Data for '" + supplier.Name + "' Error: " + response1.ToString)
            End If
        Next

        ''Aaaand finally, save the pictures.
        For Each img As SKUImage In Images
            Dim imgquery As String = ""
            If IsNothing(img.ImageId) Then
                'Needs adding, doesn't already exist.
                imgquery = "INSERT INTO whldata.sku_images (Path, Sku, ShortSku) VALUES ('" + img.ImagePath + "', '" + SKU + "','" + ShortSku + "');"
            Else
                'Already exist, jus needs updating.
                imgquery = "REPLACE INTO whldata.sku_images (imageId,Path, Sku, ShortSku) VALUES (" + img.ImageId.ToString + ",'" + img.ImagePath + "', '" + SKU + "','" + ShortSku + "');"
            End If
            Dim response1 As Object = MySql.insertupdate(imgquery)
            If response1.ToString.Length < 10 Then
                SaveStatuses.Add("Image Data for '" + img.ImagePath + "' saved successfully.")
            Else
                SaveStatuses.Add("Image Data for '" + img.ImagePath + "' Error: " + response1.ToString)
            End If
        Next

        '08/12/15: Added saving Dates. Event Data does not get saved here as it is considered read-only after initial write, like stock levels. 
        Dim Datesquery As String = ""

        'Create the Dates query
        Datesquery = "REPLACE INTO whldata.sku_dates (ShortSku,FirstRecorded,FirstPriced,FirstPhoto,AddedToLinnworks,FirstListed) VALUES ('" _
            + ShortSku + "','" + Dates.FirstRecorded.ToString("dd/MM/yyyy HH:mm:ss") + "', '" + Dates.FirstPriced.ToString("dd/MM/yyyy HH:mm:ss") + "','" _
            + Dates.FirstPhoto.ToString("dd/MM/yyyy HH:mm:ss") + "','" + Dates.AddedToLinnworks.ToString("dd/MM/yyyy HH:mm:ss") + "','" _
            + Dates.FirstListed.ToString("dd/MM/yyyy HH:mm:ss") + "');"

        Dim response2 As Object = MySql.insertupdate(Datesquery)
        If response2.ToString.Length < 10 Then
            SaveStatuses.Add("Dates Data for '" + ShortSku + "' saved successfully.")
        Else
            SaveStatuses.Add("Dates Data for '" + ShortSku + "' Error: " + response2.ToString)
        End If

        '17/12/15: Added save changelogging, and added the parameters of the method to allow passing change reasons and users. 
        Dim ChangesQuery As String = ""
        ChangesQuery = MySql.insertupdate("INSERT INTO whldata.sku_changelog (shortsku, payrollId, reason, datetimechanged) VALUES ('" + ShortSku + "','" + LogUser.PayrollId.ToString + "','" + LogReason + "','" + Now.ToString("dd/MM/yyyy HH:mm:ss") + "');")
        Dim response3 As Object = MySql.insertupdate(ChangesQuery)
        If response3.ToString.Length < 10 Then
            SaveStatuses.Add("Changelog for '" + ShortSku + "' saved successfully.")
        Else
            SaveStatuses.Add("Changelog for '" + ShortSku + "' Error: " + response3.ToString)
        End If

        Return SaveStatuses

    End Function

    Public Sub New(param As Object)
        If param.GetType = (New ArrayList).GetType Then
            'Its an arraylist, shock horror
            Newbie(param)
        ElseIf param.ToString.Length = 7 Then
            'Probably a sku
            ShortSku = param
            SKU = param + "xxxx"
            Dates.FirstRecorded = Now
        End If
    End Sub

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
    Public Sub Newbie(Data As ArrayList)

        'Initialate



        'Basic
        SKU = Data(0)
        ShortSku = Data(80)
        PackSize = Data(15)
        'Pieces = Data(16)

        'Misc
        DeliveryNote = Data(31)

        'SkuStock

        'Try to get live stock first, then if that doesn't work, fall back as usual.
        Try
            Dim LinnStock As ArrayList = MySql.SelectData("SELECT SKU, Stock, StockMinimum FROM whldata.inventory WHERE SKU='" + SKU + "';")(0)
            If LinnStock.Count > 0 Then
                Try
                    If IsDBNull(LinnStock(1)) Then Stock.Level = 0 Else Stock.Level = LinnStock(1)
                Catch ex As Exception
                    Stock.Level = 0
                End Try
                Try
                    If IsDBNull(LinnStock(2)) Then Stock.Minimum = 0 Else Stock.Minimum = LinnStock(2)
                Catch ex As Exception
                    Stock.Minimum = 0
                End Try
                Stock.Dormant = 0
                Stock.Total = Stock.Dormant + Stock.Level + Stock.Minimum
            Else

            End If

        Catch exe As Exception
            'Fall back because either we hit an exception or we just don't have any.
            If IsDBNull(Data(1)) Then Stock.Level = 0 Else Stock.Level = Data(1)

            If IsDBNull(Data(2)) Then Stock.Minimum = 0 Else Stock.Minimum = Data(2)

            If IsDBNull(Data(3)) Then Stock.Dormant = 0 Else Stock.Dormant = Data(3)

            If IsDBNull(Data(4)) Then Stock.Total = 0 Else Stock.Total = Data(4)

            '08/12/2015 - Rewote the stock pickup here because it was awful considering newer thigns will pull stock from Linnworks anyway.
        End Try




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
        '14/01/2016     Added distinguish to teh save, and load functions. 
        If IsDBNull(Data(79)) Then
            Title.Distinguish = ""
        Else
            Try
                Title.Distinguish = Data(101)
            Catch ex As Exception
                Title.Distinguish = ""
            End Try
        End If

        'Images
        Dim NewImageTable As Object = MySql.SelectData("SELECT * FROM whldata.sku_images WHERE SKU='" + SKU + "';")
        Try
            Dim NewArr As ArrayList = NewImageTable
            If NewArr.Count > 0 Then
                'We have records!
                For Each Img As ArrayList In NewArr
                    Dim NewImg As New SKUImage
                    NewImg.ImagePath = Img(1)
                    NewImg.ImageId = Img(0)
                    NewImg.FullImagePath = Img(1)
                    Images.Add(NewImg)
                Next
            Else
                Throw New Exception("None exist.")
            End If
        Catch ex As Exception
            Console.Write(ex.Message + ", using fallback for SUPPLIERS")
            'Fallback for the olden method.
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
        End Try




        'Suppliers
        Dim NewSupplierTable As Object = MySql.SelectData("SELECT * FROM whldata.sku_supplierdata WHERE SKU='" + ShortSku + "';")
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
                    NewSupp.TableDataId = Supp(0)
                    NewSupp.CaseBarcodeInner = Supp(13)
                    'NewSupp.BoxCode = Supp(14)     Looks obselete to me. 
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
                NewSupp.CaseBarcodeInner = ""
                NewSupp.BoxCode = ""
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
                NewSupp.CaseBarcodeInner = ""
                NewSupp.BoxCode = ""
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
                NewSupp.CaseBarcodeInner = ""
                NewSupp.BoxCode = ""
                Suppliers.Add(NewSupp)
            End If
        End Try


        'Locations
        Dim NewLocs As Object = MySql.SelectData("SELECT * FROM whldata.sku_locations WHERE Sku='" + ShortSku + "';")
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
            SearchKeywords.Add(Supplier.BoxCode)            '08/12/2015 - Events, dates and some supplier info added. 
            SearchKeywords.Add(Supplier.CaseBarcodeInner)   '08/12/2015 - Events, dates and some supplier info added.
        Next
        For Each Shelf As SKULocation In Locations
            SearchKeywords.Add(Shelf.LocalLocationName)
        Next
        SearchKeywords.Add(Title.Label)
        SearchKeywords.Add(Title.Invoice)

        'Costs, which for some reason weren't there before... 
        Try
            If IsDBNull(Data(43)) Then Costs.Packing = 0 Else Costs.Packing = Data(43)
        Catch ex As Exception
            Costs.Packing = 0
        End Try
        Try
            If IsDBNull(Data(44)) Then Costs.Postage = 0 Else Costs.Postage = Data(44)
        Catch ex As Exception
            Costs.Packing = 0
        End Try
        Try
            If IsDBNull(Data(45)) Then Costs.Envelope = 0 Else Costs.Envelope = Data(45)
        Catch ex As Exception
            Costs.Packing = 0
        End Try
        Try
            If IsDBNull(Data(46)) Then Costs.Fees = 0 Else Costs.Fees = Data(46)
        Catch ex As Exception
            Costs.Packing = 0
        End Try
        Try
            If IsDBNull(Data(47)) Then Costs.Labour = 0 Else Costs.Labour = Data(47)
        Catch ex As Exception
            Costs.Packing = 0
        End Try
        Try
            If IsDBNull(Data(48)) Then Costs.VAT = 0 Else Costs.VAT = Data(48)
        Catch ex As Exception
            Costs.Packing = 0
        End Try
        Try
            If IsDBNull(Data(49)) Then Costs.Total = 0 Else Costs.Total = Data(49)
        Catch ex As Exception
            Costs.Packing = 0
        End Try


        'Post-Save additions

        Try
            If IsDBNull(Data(13)) Then Profile.Weight = 0 Else Profile.Weight = Data(13)
        Catch ex As Exception
            Profile.Weight = 0
        End Try
        Try
            If IsDBNull(Data(12)) Then ExtendedProperties.Envelope = "" Else ExtendedProperties.Envelope = Data(12)
        Catch ex As Exception
            ExtendedProperties.Envelope = ""
        End Try
        Try
            If IsDBNull(Data(17)) Then ExtendedProperties.LabourCode = "" Else ExtendedProperties.LabourCode = Data(17)
        Catch ex As Exception
            ExtendedProperties.LabourCode = ""
        End Try
        Try
            If IsDBNull(Data(77)) Then Category = "" Else Category = Data(77)
        Catch ex As Exception
            Category = ""
        End Try
        'NEWITEM STUFF
        Try
            If IsDBNull(Data(100)) Then NewItem.Brand = "" Else NewItem.Brand = Data(100)
        Catch ex As Exception
            NewItem.Brand = ""
        End Try
        Try
            If IsDBNull(Data(101)) Then NewItem.Description = "" Else NewItem.Description = Data(101)
        Catch ex As Exception
            NewItem.Description = ""
        End Try
        Try
            If IsDBNull(Data(104)) Then NewItem.Finish = "" Else NewItem.Finish = Data(104)
        Catch ex As Exception
            NewItem.Finish = ""
        End Try
        Try
            If IsDBNull(Data(105)) Then NewItem.Size = "" Else NewItem.Size = Data(105)
        Catch ex As Exception
            NewItem.Size = ""
        End Try
        Try
            If IsDBNull(Data(106)) Then NewItem.Note = "" Else NewItem.Note = Data(106)
        Catch ex As Exception
            NewItem.Note = ""
        End Try
        Try
            If IsDBNull(Data(109)) Then NewItem.Box = "0" Else NewItem.Box = Data(109)
        Catch ex As Exception
            NewItem.Box = "0"
        End Try
        Try
            If IsDBNull(Data(110)) Then NewItem.Status = "" Else NewItem.Status = Data(110)
        Catch ex As Exception
            NewItem.Status = ""
        End Try
        Try
            If Data(118) = "True" Then ExtendedProperties.IsPair = True Else ExtendedProperties.IsPair = False
        Catch ex As Exception
            ExtendedProperties.IsPair = False
        End Try
        '18/01/2016     Modified the courier detection because it was wrong. Checked for "True", but it's actually Y or y in older formats. 
        Try
            If (Data(18) = "True") Or (Data(18).ToString.ToLower = "y") Then ExtendedProperties.NeedsCourier = True Else ExtendedProperties.NeedsCourier = False
        Catch ex As Exception
            ExtendedProperties.NeedsCourier = False
        End Try
        '21/12/2015     "Added" the "new" initial stock field. Lives in the col38 of the main table, and in NewItem in classes. 
        Try
            If IsDBNull(Data(38)) Then NewItem.InitStock = 0 Else NewItem.InitStock = Convert.ToInt32(Data(38))
        Catch ex As Exception
            NewItem.InitStock = 0
        End Try


        'The next 2 block are made 08/12/2015 - Events and Dates. 
        'Dates
        Dim DatesResponse As ArrayList = MySql.SelectData("SELECT * FROM whldata.sku_dates WHERE ShortSku='" + ShortSku + "' LIMIT 1;")
        If DatesResponse.Count = 1 Then
            Dates.FirstRecorded = Date.Parse(DatesResponse(0)(1).ToString)
            Dates.FirstPriced = Date.Parse(DatesResponse(0)(2).ToString)
            Dates.FirstPhoto = Date.Parse(DatesResponse(0)(3).ToString)
            Dates.AddedToLinnworks = Date.Parse(DatesResponse(0)(4).ToString)
            Dates.FirstListed = Date.Parse(DatesResponse(0)(5).ToString)
        Else
            'Set empty date values. 
            Try
                Dates.FirstRecorded = Data(36)
            Catch ex As Exception
                Dates.FirstRecorded = Date.MinValue
            End Try
            Dates.FirstPriced = Date.MinValue
            Dates.FirstPhoto = Date.MinValue
            Dates.AddedToLinnworks = Date.MinValue
            Dates.FirstListed = Date.MinValue
        End If


        '17/12/2015     Added the class to store and then the list of "SKUChangelog" which will be used for 
        '               audit trailing. Hopefully it won't get too big but who knows kek.
        '               
        '               Also removed events after realising that the changelog was basically the same. 

        Dim ChangesResponse As ArrayList = MySql.SelectData("SELECT * FROM whldata.sku_changelog WHERE shortsku='" + ShortSku + "';")
        For Each ChangesData As ArrayList In ChangesResponse
            Dim NewChange As New SKUChangelog
            NewChange.DateModified = Date.Parse(ChangesData(4))
            NewChange.Reason = ChangesData(3).ToString
            NewChange.UserId = Convert.ToInt32(ChangesData(2).ToString)
            Changelog.Add(NewChange)
        Next

        '15/01/2016     Adding NewItem Fields (InitStock, InitLevel, InitMinimum, ListPriority, Islisted) and Inner field in ExtendedProperties. 
        '            ...Apparently InitStock has already been added on 21/12/2015. Adding the others insread. 
        Try
            If IsDBNull(Data(39)) Then NewItem.InitLevel = 0 Else NewItem.InitLevel = Convert.ToInt32(Data(39))
        Catch ex As Exception
            NewItem.InitLevel = 0
        End Try
        Try
            If IsDBNull(Data(120)) Then NewItem.InitMinimum = 0 Else NewItem.InitMinimum = Convert.ToInt32(Data(120))
        Catch ex As Exception
            NewItem.InitMinimum = 0
        End Try
        Try
            If IsDBNull(Data(121)) Then NewItem.ListPriority = 0 Else NewItem.ListPriority = Convert.ToInt32(Data(121))
        Catch ex As Exception
            NewItem.ListPriority = 0
        End Try
        Try
            If IsDBNull(Data(122)) Then NewItem.IsListed = False Else NewItem.IsListed = Convert.ToBoolean(Data(122))
        Catch ex As Exception
            NewItem.InitStock = False
        End Try

        '18/01/2016     (Possibly temporary) Added fee recalc inline so it's fresh. See how badly it impacts loading speed. Yay timers. 
        '19/01/2016     Results on times with this added: 
        '               Time With Recalc on Dev PC:     4min 55.26sec   (295 sec)
        '               Time Without recalc on Dev PC:  2min 24.44sec   (144 sec)
        '               Recalc time difference: 105% slower.
        '               Ouch. 
        Try
            RecalculateCosts(ExtendedProperties.Envelope, Price.Retail)
        Catch ex As Exception

        End Try


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
    Public PrepackInfo As New SKUPrepack
    Public Pieces As Integer = 1
    Public DeliveryNote As String = ""
    Public Costs As New SKUCosts
    Public Locations As New List(Of SKULocation)
    Public SalesData As New SKUSalesData
    Public PackSize As Integer = 0
    Public ExtendedProperties As New SKUExtended
    Public SearchKeywords As New List(Of String)
    Public Profile As New SKUProfile
    Public CustomData As New List(Of Object)
    Public Category As String = ""
    Public NewItem As New SKUNewItem
    Public Dates As New SKUDates
    'Public Events As New List(Of SKUEvents)        Redundant before it was even used. RIP. 
    Public Changelog As New List(Of SKUChangelog)
    'And here we have some properties. 
End Class

<System.Serializable()>
Public Class SKUDates
    Public Sub New()
        FirstRecorded = Date.MinValue
        FirstPriced = Date.MinValue
        FirstPhoto = Date.MinValue
        FirstListed = Date.MinValue
        AddedToLinnworks = Date.MinValue
    End Sub

    Public FirstRecorded As Date
    Public FirstPriced As Date
    Public FirstPhoto As Date
    Public AddedToLinnworks As Date
    Public FirstListed As Date
End Class
'17/12/2015     SKUEvents has been removed due to redundancy. 
<System.Serializable()>
Public Class SKUSalesData
    'Remember, we don't download this automatically
    Public Sub New()
        Sku = ""
        Weighted = 0
        Raw8 = 0
        Raw4 = 0
        Raw1 = 0
        Avg8 = 0
        Avg4 = 0
        Avg1 = 0
    End Sub

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
<System.Serializable()>
Public Class SKUExtended
    Public Sub New()
        Parts = 1
        Screws = 0
        HasScrews = False
        GS1Barcode = ""
        Envelope = "x"
        LabourCode = ""
        NeedsCourier = False
        IsPair = False
        Inner = ""
    End Sub

    Public Parts As Integer
    Public Screws As Integer
    Public HasScrews As Boolean
    Public GS1Barcode As String
    Public Envelope As String
    Public LabourCode As String
    Public NeedsCourier As Boolean
    Public IsPair As Boolean
    Public Inner As String
End Class
<System.Serializable()>
Public Class SKUProfile
    Public Sub New()
        Weight = 0
    End Sub
    Public Weight As Single
End Class
<System.Serializable()>
Public Class SKUNewItem
    Public Sub New()
        Brand = ""
        Finish = ""
        Description = ""
        Size = ""
        Note = ""
        Box = ""
        Status = ""
        InitShelf = ""
    End Sub

    Public Brand As String
    Public Finish As String
    Public Description As String
    Public Size As String
    Public Note As String
    Public Box As String
    Public Status As String
    Public InitShelf As String
    Public InitStock As Integer = 0
    Public InitLevel As Integer = 0
    Public InitMinimum As Integer = 0
    Public ListPriority As Integer = 1
    Public IsListed As Boolean = True

End Class
<System.Serializable()>
Public Class SKULocation
    Public LocationTableID As Integer
    Public LocalLocationName As String

End Class
<System.Serializable()>
Public Class SKUCosts
    Public Sub New()
        Packing = 0
        Postage = 0
        Envelope = 0
        Fees = 0
        Labour = 0
        VAT = 0
        Total = 0
    End Sub

    Public Packing As Single
    Public Postage As Single
    Public Envelope As Single
    Public Fees As Single
    Public Labour As Single
    Public VAT As Single
    Public Total As Single
End Class
<System.Serializable()>
Public Class SKUSupplier
    Dim SuppName As String = ""
    Dim SuppCode As String = ""
    Dim SuppPrice As Single = 0.00
    Dim SuppCaseCode As String = ""
    Dim SuppBarCode As String = ""
    Dim SuppDisCon As Boolean = False
    Dim SuppPrimary As Boolean = False
    Dim SuppLead As Integer = 4
    Dim SuppNoStock As Boolean = False
    Dim Modified As String = "New Item"
    Dim SuppBoxCode As String = ""
    Dim SuppCaseInnerBarcode As String = ""
    Public TableDataId As String

    ''' <summary>
    ''' barcode which can be found on inners. For some reason this is occasionally different. 
    ''' </summary>
    ''' <returns></returns>
    Public Property CaseBarcodeInner As String
        Get
            Return SuppCaseInnerBarcode
        End Get
        Set(value As String)
            SuppCaseInnerBarcode = value
        End Set
    End Property

    ''' <summary>
    ''' The code which appears on the box. May not be the same as the reorder code for some suppliers. 
    ''' </summary>
    ''' <returns></returns>
    Public Property BoxCode As String
        Get
            Return SuppBoxCode
        End Get
        Set(value As String)
            SuppBoxCode = value
        End Set
    End Property

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
<System.Serializable()>
Public Class SKUStock
    Public Sub New()
        slevel = 0
        smin = 0
        sdormant = 0
        stotal = 0
    End Sub

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
<System.Serializable()>
Public Class SKUPrices
    Public Sub New()
        Net = 0
        Gross = 0
        Retail = 0
        Profit = 0
        Margin = 0
    End Sub

    Public Net As Single
    Public Gross As Single
    Public Retail As Single
    Public Profit As Single
    Public Margin As Single
End Class
<System.Serializable()>
Public Class SKUTitles
    Public Sub New()
        Invoice = ""
        Label = ""
        Linnworks = ""
        NewItem = ""
        Distinguish = ""

    End Sub
    Public Invoice As String
    Public Label As String
    Public Linnworks As String
    Public NewItem As String
    Public Distinguish As String
End Class
<System.Serializable()>
Public Class SKUImage
    Public ImagePath As String
    Public FullImagePath As String
    Public ImageData As System.Drawing.Image = Nothing
    Public ImageId As String

End Class
'Pskupost has been removed because it was redundant.

<System.Serializable()>
Public Class SKUPrepack
    Public Sub New()
        Bag = ""
        Notes = ""
        PrepackLog = Nothing
    End Sub

    Public Bag As String
    Public Notes As String
    Public PrepackLog As List(Of PrepackLog)
End Class
<System.Serializable()>
Public Class SKUChangelog
    Public UserId As Integer
    Public DateModified As Date
    Public Reason As String
End Class
