Namespace Fees

    '15/01/2016     FeeExceptions contains Exceptions which can occur related to fee calculations. 
    Namespace FeeExceptions

        '15/01/2016     This one is for throwing when the item is over the threshold for postage and must be tried with courier instead. 
        Public Class TooHeavyForPostException
            Inherits Exception

            Public Sub New()
                MyBase.New("The item is too heavy to be sent by standard post. The item will be automatically upgraded to courier, and costs recalculated. ")
            End Sub
        End Class

        '15/01/2016     This one is for throwing when the item is over the threshold for Courier and an error must be shown or something. 
        Public Class TooHeavyForCourierException
            Inherits Exception

            Public Sub New()
                MyBase.New("The item is over the threshold for sending courier. Consider a lighter packaging, or smaller size packs. ")
            End Sub
        End Class

        '15/01/2016     This one is for throwing when the item is over the threshold for Courier and an error must be shown or something. 
        Public Class LabourDoesntExistException
            Inherits Exception

            Public Sub New()
                MyBase.New("An item with this labour code does not exist. ")
            End Sub
        End Class

        '18/01/2016     This is for recommending upgrading to packet from letter, because I don't think we can do that from here, but we can handle the exception to switch over. 
        Public Class UpgradeToPacketException
            Inherits Exception

            Public Sub New()
                MyBase.New("The given data cannot be sent as a letter - It must be sent as a packet as it is too heavy. Upgrading to packet envelopes should fix this. ")
            End Sub
        End Class

    End Namespace


    '15/01/2016     This is such a stupid sounding class. FeeManager is here to process getting relevant costs for items. 
    Public Class FeeManager
        Public Sub New()

        End Sub
        ''' <summary>
        ''' Gets the postage price form whldata.postagecosts for items based on the supplied data. 
        ''' </summary>
        ''' <param name="Weight">The weight in grams of the pack. </param>
        ''' <param name="Packet">True if the item is being posted as packet, false if as large letter. </param>
        ''' <param name="Courier">True if courier is to be used, false if not. </param>
        ''' <returns></returns>
        Public Function GetPostagePrice(Weight As Integer, Optional Packet As Boolean = False, Optional Courier As Boolean = False) As Single
            If Courier Then
                If Not Weight > Convert.ToInt32(MySql.SelectData("SELECT value FROM whldata.feesandsurcharges WHERE `desc`='MaxCourierWeight';")(0)(0)) Then
                    Return Convert.ToSingle(MySql.SelectData("SELECT value FROM whldata.feesandsurcharges WHERE `desc`='Courier';")(0)(0))
                Else
                    Throw New FeeExceptions.TooHeavyForCourierException
                End If
            Else
                If Not Weight > Convert.ToInt32(MySql.SelectData("SELECT value FROM whldata.feesandsurcharges WHERE `desc`='MaxPacketWeight';")(0)(0)) Then
                    'Do the actual calculation. 
                    If Weight > Convert.ToInt32(MySql.SelectData("SELECT value FROM whldata.feesandsurcharges WHERE `desc`='MaxLetterWeight';")(0)(0)) Then
                        If Packet Then
                            'Packet
                            Return Convert.ToSingle(MySql.SelectData("SELECT Cost FROM whldata.postagecosts WHERE Weight > " + Weight.ToString + " AND Type='Packet' ORDER BY Weight ASC LIMIT 1")(0)(0))
                        Else
                            'Letter - upgrade required
                            Throw New FeeExceptions.UpgradeToPacketException
                        End If
                    Else
                        'Letter
                        Return Convert.ToSingle(MySql.SelectData("SELECT Cost FROM whldata.postagecosts WHERE Weight > " + Weight.ToString + " AND Type='Letter' ORDER BY Weight ASC LIMIT 1")(0)(0))
                    End If
                Else
                    Throw New FeeExceptions.TooHeavyForPostException
                End If
            End If
        End Function

        '18/01/2016     This pulls the data from the database for the labour code chosen. I feel like this is a much tidier method than creating some bullshit databindings which 
        '               never work, or trying To Do it inline which Is ugly. 
        ''' <summary>
        ''' Gets the price for the given labour code. 
        ''' </summary>
        ''' <param name="LabourCode">The labour code. Should have been a selection in a dropdown where applicable. </param>
        ''' <returns></returns>
        Public Function GetLabourPrice(LabourCode As String) As Single
            Try
                Return Convert.ToSingle(MySql.SelectData("SELECT cost FROM whldata.labourcosts WHERE code='" + LabourCode + "';")(0)(0))
            Catch ex As Exception
                Throw New FeeExceptions.LabourDoesntExistException
            End Try
        End Function

        '18/01/2016     This is fairly trivial, it just gets the price of the envelope which is in the database, and in the passed parameter, and adds the label cost in the
        '               database 
        ''' <summary>
        ''' Gets the price of the envelope, adjusted to include the price of 
        ''' </summary>
        ''' <param name="Envelope"></param>
        ''' <returns></returns>
        Public Function GetEnvelopePrice(Envelope As Envelope) As Single
            Dim postlabelprice As Single = Convert.ToSingle(MySql.SelectData("SELECT value FROM whldata.feesandsurcharges WHERE `desc`='PostageLabelCost';")(0)(0))
            Return Envelope.IndividualPrice + postlabelprice
        End Function

        '18/01/2016     This one works out VAT, based on the VAT rate documented in the database/.
        ''' <summary>
        ''' Get the VAT charged on the item. 
        ''' </summary>
        ''' <param name="RetailPrice"></param>
        ''' <returns></returns>
        Public Function GetVATCost(RetailPrice As Single) As Single
            Dim vatrate As Single = 1 + Convert.ToSingle(MySql.SelectData("SELECT value FROM whldata.feesandsurcharges WHERE `desc`='VATRate';")(0)(0))
            Return vatrate / RetailPrice
        End Function

        '18/01/2016     This function takes the retail price and uses to work out the ebay+paypal and whatever fees. Call it listing fees, I dunno. 
        ''' <summary>
        ''' Get the ebay lsiting fees (including discounts, paypal surcharges etc) associated with the product
        ''' </summary>
        ''' <param name="RetailPrice"></param>
        ''' <returns></returns>
        Public Function GetListingFees(RetailPrice As Single) As Single
            Dim feerate As Single = Convert.ToSingle(MySql.SelectData("SELECT value FROM whldata.feesandsurcharges WHERE `desc`='TotalFeePerc';")(0)(0))
            Dim surcharge As Single = Convert.ToSingle(MySql.SelectData("SELECT value FROM whldata.feesandsurcharges WHERE `desc`='TotalSurcharge';")(0)(0))
            Return feerate * RetailPrice + surcharge
        End Function
    End Class
End Namespace

