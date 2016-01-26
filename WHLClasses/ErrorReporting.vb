
Imports System.Net.Mail

Namespace Reporting
    Public Module ErrorReporting
        Public Sub ReportException(Ex As Exception)
            Dim OS As String = My.Computer.Info.OSFullName
            Dim Computer As String = My.Computer.Name
            Dim Username As String = My.User.Name
            Dim Application As String = My.Application.Info.AssemblyName
            Dim StackTrace As String = Ex.StackTrace
            Dim Message As String = Ex.Message
            Dim Seperator As String = vbNewLine + "============================================================================" + vbNewLine
            Dim MsgBody As String = "An error report is contained within this email from " + Computer + " | " + Application + ". " + Seperator + "Stack Trace:" + vbNewLine + StackTrace + vbNewLine + "Error Message: " + Message + Seperator + OS + vbNewLine + Username + vbNewLine + Application + vbNewLine + Computer + Seperator + Ex.ToString + Seperator

            Try
                Dim Smtp_Server As New SmtpClient
                Dim e_mail As New MailMessage()
                Smtp_Server.UseDefaultCredentials = False
                Smtp_Server.Credentials = New Net.NetworkCredential("mantis@whitehinge.com", "mantispass")
                Smtp_Server.Port = 25
                Smtp_Server.EnableSsl = False
                Smtp_Server.Host = "mail.whitehinge.com"

                e_mail = New MailMessage()
                e_mail.From = New MailAddress("mantis@whitehinge.com", Application + " Bug report.")
                e_mail.To.Add("lee@whitehinge.com")
                e_mail.Subject = "Automatic bug report from '" + Computer + "'"
                e_mail.IsBodyHtml = False
                e_mail.Body = MsgBody
                Smtp_Server.Send(e_mail)
                MsgBox("An unexpected Error occured. It has been reported via email To the developer.")
            Catch exc As Exception
                MsgBox("An unexpected Error occured. We attempted to report it to the developer, but that failed miserably and the error didn't report. Here is the report.  " + vbNewLine + vbNewLine + MsgBody)
            End Try

        End Sub
    End Module
End Namespace