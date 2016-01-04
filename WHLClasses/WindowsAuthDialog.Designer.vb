<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class WindowsAuthDialog
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.LargeIconButton = New System.Windows.Forms.RadioButton()
        Me.Username = New System.Windows.Forms.TextBox()
        Me.Password = New System.Windows.Forms.TextBox()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.ErrorText = New System.Windows.Forms.Label()
        Me.Panel1.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.OK_Button.BackColor = System.Drawing.Color.Transparent
        Me.OK_Button.Location = New System.Drawing.Point(248, 15)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(75, 23)
        Me.OK_Button.TabIndex = 2
        Me.OK_Button.Text = "OK"
        Me.OK_Button.UseVisualStyleBackColor = False
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Cancel_Button.BackColor = System.Drawing.Color.Transparent
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Location = New System.Drawing.Point(333, 15)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(75, 23)
        Me.Cancel_Button.TabIndex = 3
        Me.Cancel_Button.Text = "Cancel"
        Me.Cancel_Button.UseVisualStyleBackColor = False
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(240, Byte), Integer), CType(CType(240, Byte), Integer))
        Me.Panel1.Controls.Add(Me.Panel2)
        Me.Panel1.Controls.Add(Me.OK_Button)
        Me.Panel1.Controls.Add(Me.Cancel_Button)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(0, 197)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(423, 54)
        Me.Panel1.TabIndex = 2
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.FromArgb(CType(CType(223, Byte), Integer), CType(CType(223, Byte), Integer), CType(CType(223, Byte), Integer))
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel2.Location = New System.Drawing.Point(0, 0)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(423, 1)
        Me.Panel2.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(13, 13)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(398, 66)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "The application requires a user ID and pin." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Please enter your user ID and PIN," &
    " not the username and password you use to login to the computer."
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.Color.FromArgb(CType(CType(223, Byte), Integer), CType(CType(223, Byte), Integer), CType(CType(223, Byte), Integer))
        Me.Panel3.Location = New System.Drawing.Point(16, 83)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(392, 1)
        Me.Panel3.TabIndex = 4
        '
        'LargeIconButton
        '
        Me.LargeIconButton.Appearance = System.Windows.Forms.Appearance.Button
        Me.LargeIconButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.LargeIconButton.Checked = True
        Me.LargeIconButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(98, Byte), Integer), CType(CType(162, Byte), Integer), CType(CType(228, Byte), Integer))
        Me.LargeIconButton.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(CType(CType(201, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(247, Byte), Integer))
        Me.LargeIconButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(218, Byte), Integer), CType(CType(233, Byte), Integer), CType(CType(250, Byte), Integer))
        Me.LargeIconButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(205, Byte), Integer), CType(CType(228, Byte), Integer), CType(CType(252, Byte), Integer))
        Me.LargeIconButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.LargeIconButton.Font = New System.Drawing.Font("Segoe UI", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LargeIconButton.Location = New System.Drawing.Point(21, 90)
        Me.LargeIconButton.Name = "LargeIconButton"
        Me.LargeIconButton.Size = New System.Drawing.Size(383, 88)
        Me.LargeIconButton.TabIndex = 12
        Me.LargeIconButton.TabStop = True
        Me.LargeIconButton.TextAlign = System.Drawing.ContentAlignment.TopLeft
        Me.LargeIconButton.UseVisualStyleBackColor = True
        '
        'Username
        '
        Me.Username.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Username.ForeColor = System.Drawing.Color.DimGray
        Me.Username.Location = New System.Drawing.Point(107, 103)
        Me.Username.Name = "Username"
        Me.Username.Size = New System.Drawing.Size(195, 23)
        Me.Username.TabIndex = 0
        Me.Username.Text = " Username"
        '
        'Password
        '
        Me.Password.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Password.ForeColor = System.Drawing.Color.DimGray
        Me.Password.Location = New System.Drawing.Point(107, 133)
        Me.Password.Name = "Password"
        Me.Password.Size = New System.Drawing.Size(195, 23)
        Me.Password.TabIndex = 1
        Me.Password.Text = " Password"
        Me.Password.UseSystemPasswordChar = True
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = Global.WHLClasses.My.Resources.Resources.Nouser
        Me.PictureBox1.Location = New System.Drawing.Point(33, 103)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(64, 64)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBox1.TabIndex = 15
        Me.PictureBox1.TabStop = False
        '
        'ErrorText
        '
        Me.ErrorText.AutoSize = True
        Me.ErrorText.BackColor = System.Drawing.Color.FromArgb(CType(CType(201, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(247, Byte), Integer))
        Me.ErrorText.ForeColor = System.Drawing.Color.FromArgb(CType(CType(54, Byte), Integer), CType(CType(54, Byte), Integer), CType(CType(54, Byte), Integer))
        Me.ErrorText.Location = New System.Drawing.Point(106, 168)
        Me.ErrorText.Name = "ErrorText"
        Me.ErrorText.Size = New System.Drawing.Size(105, 15)
        Me.ErrorText.TabIndex = 16
        Me.ErrorText.Text = "Error Message Text"
        Me.ErrorText.Visible = False
        '
        'WindowsAuthDialog
        '
        Me.AcceptButton = Me.OK_Button
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.BackColor = System.Drawing.Color.White
        Me.CancelButton = Me.Cancel_Button
        Me.ClientSize = New System.Drawing.Size(423, 251)
        Me.Controls.Add(Me.ErrorText)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.Password)
        Me.Controls.Add(Me.Username)
        Me.Controls.Add(Me.LargeIconButton)
        Me.Controls.Add(Me.Panel3)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Panel1)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "WindowsAuthDialog"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Authentication Required"
        Me.Panel1.ResumeLayout(False)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents LargeIconButton As System.Windows.Forms.RadioButton
    Friend WithEvents Username As System.Windows.Forms.TextBox
    Friend WithEvents Password As System.Windows.Forms.TextBox
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents ErrorText As System.Windows.Forms.Label
End Class
