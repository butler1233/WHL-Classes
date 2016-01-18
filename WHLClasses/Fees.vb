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
                If Not Weight > Convert.ToInt32(MySql.SelectData("SELECT value FROM whldata.feesandsurcharges WHERE desc='MaxCourierWeight';")(0)(0)) Then
                    Return Convert.ToSingle(MySql.SelectData("SELECT value FROM whldata.feesandsurcharges WHERE desc='Courier';")(0)(0))
                Else
                    Throw New FeeExceptions.TooHeavyForCourierException
                End If
            Else
                If Not Weight > Convert.ToInt32(MySql.SelectData("SELECT value FROM whldata.feesandsurcharges WHERE desc='MaxPacketWeight';")(0)(0)) Then
                    'Do the actual calculation. 
                    If Weight > Convert.ToInt32(MySql.SelectData("SELECT value FROM whldata.feesandsurcharges WHERE desc='MaxLetterWeight';")(0)(0)) Then
                        'Packet
                        Return Convert.ToSingle(MySql.SelectData("SELECT Cost FROM whldata.postagecosts WHERE Weight > " + Weight + " AND Type='Letter' ORDER BY Weight ASC LIMIT 1")(0)(0))
                    Else
                        'Letter
                        Return Convert.ToSingle(MySql.SelectData("SELECT Cost FROM whldata.postagecosts WHERE Weight > " + Weight + " AND Type='Packet' ORDER BY Weight ASC LIMIT 1")(0)(0))
                    End If
                Else
                    Throw New FeeExceptions.TooHeavyForPostException
                End If
            End If
        End Function

        Public Function GetLabourPrice(LabourCode As String) As Single
            Try
                Return Convert.ToSingle(MySql.SelectData("SELECT cost FROM whldata.labourcosts WHERE code='" + LabourCode + "';"))
            Catch ex As Exception
                Throw New FeeExceptions.LabourDoesntExistException
            End Try
        End Function
    End Class
End Namespace

