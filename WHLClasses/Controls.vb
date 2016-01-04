Imports System.Drawing
Imports System.Windows.Forms

Namespace Controls
    Public Class BPanel
        Inherits System.Windows.Forms.Panel

        Public Sub New()
            Me.BorderStyle = BorderStyle.None
        End Sub

        Private bWidth As Integer = 1
        ''' <summary>
        ''' The thickness of the stroke of the border. 
        ''' </summary>
        ''' <returns></returns>
        Public Property BorderWidth() As Integer
            Get
                Return Me.bWidth
            End Get
            Set(ByVal value As Integer)
                Me.bWidth = Math.Abs(value)
                Me.Refresh()
            End Set
        End Property

        Private btext As String = "Better Group"
        ''' <summary>
        ''' The text which appears at the edge of the control.
        ''' </summary>
        ''' <returns></returns>
        Public Property Title() As String
            Get

                Return btext
            End Get
            Set(ByVal value As String)
                btext = value
                Me.Refresh()
            End Set
        End Property

        Private bhighlight As Boolean = True
        ''' <summary>
        ''' A flag which states whether the control will highlight when a child control is focussed.
        ''' </summary>
        ''' <returns></returns>
        Public Property HighlightOnFocus() As Boolean
            Get

                Return bhighlight
            End Get
            Set(ByVal value As Boolean)
                bhighlight = value
            End Set
        End Property

        Private bColor As Color = Color.FromArgb(223, 223, 223)
        ''' <summary>
        ''' The color of the border of the panel.
        ''' </summary>
        ''' <returns></returns>
        Public Property HighlightNotBorderColor() As Color
            Get
                Return Me.bColor
            End Get
            Set(ByVal value As Color)
                Me.bColor = value
                Me.Refresh()
            End Set
        End Property

        Private bColorHighlight As Color = Color.FromArgb(98, 162, 228)
        ''' <summary>
        ''' The color of the border of the panel when the panel is highlighted. 
        ''' </summary>
        ''' <returns></returns>
        Public Property HighlightBorderColor() As Color
            Get
                Return Me.bColorHighlight
            End Get
            Set(ByVal value As Color)
                Me.bColorHighlight = value
                Me.Refresh()
            End Set
        End Property

        Private bColorBackHighlight As Color = Color.FromArgb(218, 233, 250)

        ''' <summary>
        ''' The background color of the panel when the panel is highlighted. 
        ''' </summary>
        ''' <returns></returns>
        Public Property HighlightBackgroundColor() As Color
            Get
                Return Me.bColorBackHighlight
            End Get
            Set(ByVal value As Color)
                Me.bColorBackHighlight = value
                Me.Refresh()
            End Set
        End Property


        Private bColorBack As Color
        ''' <summary>
        ''' The background color of the panel. 
        ''' </summary>
        ''' <returns></returns>
        Public Property HighlightNotBackgroundColor() As Color
            Get
                Return Me.bColorBack
            End Get
            Set(ByVal value As Color)
                Me.bColorBack = value
                Me.Refresh()
            End Set
        End Property

        Private Sub SetChildEvents(sender As Object, e As ControlEventArgs) Handles Me.ControlAdded
            Dim kid As Control = e.Control
            AddHandler kid.Enter, AddressOf ChildHover
            AddHandler kid.Leave, AddressOf ChildLeave

        End Sub

        Dim NowBorder As Color = bColor

        Public Overridable Sub MyPanel_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
            Dim newrect As New Rectangle(New Point(0, 0), New Size(Me.Width - 1, Me.Height - 1))
            e.Graphics.DrawRectangle(New Pen(NowBorder, Me.bWidth), newrect)
            Dim fBrush As New SolidBrush(NowBorder)
            Dim fFont As Font = New Font("Segoe UI", 9.0!, FontStyle.Regular, GraphicsUnit.Point)
            Dim fPoint As PointF = New PointF(-Me.Height / 2, 4)
            Dim fFormat As New StringFormat()
            fFormat.Alignment = StringAlignment.Center
            e.Graphics.RotateTransform(-90)
            e.Graphics.DrawString(btext, fFont, fBrush, fPoint, fFormat)
            Margin = New Padding(10)
            Padding = New Padding(21, 8, 8, 8)
        End Sub

        Public Sub ChildHover(Sender As Control, e As EventArgs)
            If bhighlight Then
                NowBorder = bColorHighlight
                BackColor = bColorBackHighlight
                Me.Refresh()
            End If

        End Sub
        Public Sub ChildLeave(Sender As Control, e As EventArgs)
            If bhighlight Then
                NowBorder = bColor
                BackColor = bColorBack
                Me.Refresh()
            End If

        End Sub

        Private Sub InitializeComponent()
            Me.SuspendLayout()
            Me.ResumeLayout(False)

        End Sub
    End Class
    Public Class ShittyTextBoxWithSpellCheckFromWPF
        Inherits System.Windows.Forms.Integration.ElementHost

        Public Event Enter(ByVal sender As Object, ByVal e As EventArgs)
        Public Event Leave(ByVal sender As Object, ByVal e As EventArgs)
        Public Event TextChanged(ByVal sender As Object, ByVal e As EventArgs)


        Public Sub New()
            Child = New SpellcheckTextbox
            control = Child
            Width = 130
            Height = 27


            Dim MParent As BPanel = Parent
            AddHandler control.Enter, AddressOf MParent.ChildHover
            AddHandler control.Leave, AddressOf MParent.ChildLeave
            AddHandler control.TextChanged, AddressOf FireTextChanged

        End Sub
        Dim control As SpellcheckTextbox

        Private Sub FireTextChanged(sender As Object, e As EventArgs)
            RaiseEvent TextChanged(sender, e)
        End Sub

        Public Overrides Property Text() As String
            Get
                Return control.Text
            End Get
            Set(value As String)
                control.Text = value
            End Set
        End Property
    End Class
End Namespace


