Imports System.Drawing
Imports System.Drawing.Printing
Imports System.Windows.Forms

Public Class Labels
    Public Function GenerateC128String(Data As String) As String
        Return "Ì" + Data + GenerateC128Checksum(Data) + "Î"
    End Function

    Private Function GenerateC128Checksum(Data As String) As String

        Dim Checksum As Integer = 104
        Dim Loopo As Integer = 0
        While Loopo < Data.Length
            Dim CurrentCharacter As Double = Convert.ToInt32(Char.Parse(Data.Substring(Loopo, 1)))
            If CurrentCharacter < 127 Then CurrentCharacter -= 32 Else CurrentCharacter -= 100

            Checksum = (Checksum + ((Loopo + 1) * CurrentCharacter))
            Loopo += 1

        End While
        Checksum = Checksum Mod 103
        If Checksum < 95 Then
            Checksum += 32
        Else
            Checksum += 100
        End If
        Return Char.ConvertFromUtf32(Checksum)

    End Function
    Dim Printer As New PrintDocument

    Public Function CreateBarcode(data As String, Optional height As Integer = 20) As Bitmap
        Dim maker As New ZXing.BarcodeWriter
        maker.Format = ZXing.BarcodeFormat.CODE_128
        maker.Options.PureBarcode = True
        maker.Options.Height = height
        maker.Options.Margin = 0
        Return maker.Write(data)
    End Function
    Private Sub PrintBarcodeLabel(PrinterName As String, text As String)
        Dim pc As New StandardPrintController
        Printer.PrintController = pc
        Printer.PrinterSettings.PrinterName = PrinterName
        AddHandler Printer.PrintPage, AddressOf Print
        Printer.Print()

    End Sub

    Private Sub Print(sender As Object, e As PrintPageEventArgs)
        e.Graphics.DrawImage(CreateBarcode("qzu23"), New Point(0, 0))
    End Sub

    'checksum = 0;
    '                For (int loop = 0; loop < returnValue.Length; loop++)
    '                {
    '                    currentChar = (int)char.Parse(returnValue.Substring(loop, 1));
    '                    currentChar = currentChar < 127 ? currentChar - 32 : currentChar - 100;
    '                    If (loop == 0)
    '                        checksum = currentChar;
    '                    Else
    '                        checksum = (checksum + (loop * currentChar)) % 103;
    '                }

    '                // Calculation of the checksum ASCII code
    '                checksum = checksum < 95 ? checksum + 32 : checksum + 100;
    '                // Add the checksum And the STOP
    '                returnValue = returnValue +
    '                    ((char)checksum).ToString() +
    '                    ((char)206).ToString();
End Class
