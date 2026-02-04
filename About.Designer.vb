<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class About
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
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.okBtn = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.verLabel = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.LinkLabel21 = New wyDay.Controls.LinkLabel2()
        Me.LinkLabel22 = New wyDay.Controls.LinkLabel2()
        Me.LinkLabel23 = New wyDay.Controls.LinkLabel2()
        Me.LinkLabel24 = New wyDay.Controls.LinkLabel2()
        Me.Panel2.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.FromArgb(CType(CType(24, Byte), Integer), CType(CType(24, Byte), Integer), CType(CType(24, Byte), Integer))
        Me.Panel2.Controls.Add(Me.okBtn)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel2.ForeColor = System.Drawing.Color.White
        Me.Panel2.Location = New System.Drawing.Point(0, 261)
        Me.Panel2.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(861, 44)
        Me.Panel2.TabIndex = 1
        '
        'okBtn
        '
        Me.okBtn.Dock = System.Windows.Forms.DockStyle.Right
        Me.okBtn.FlatAppearance.BorderSize = 0
        Me.okBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.okBtn.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.okBtn.Location = New System.Drawing.Point(758, 0)
        Me.okBtn.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.okBtn.Name = "okBtn"
        Me.okBtn.Size = New System.Drawing.Size(103, 44)
        Me.okBtn.TabIndex = 0
        Me.okBtn.Text = "&OK"
        Me.okBtn.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.AutoScroll = True
        Me.Panel1.Controls.Add(Me.LinkLabel24)
        Me.Panel1.Controls.Add(Me.LinkLabel23)
        Me.Panel1.Controls.Add(Me.LinkLabel22)
        Me.Panel1.Controls.Add(Me.LinkLabel21)
        Me.Panel1.Controls.Add(Me.Label2)
        Me.Panel1.Controls.Add(Me.verLabel)
        Me.Panel1.Controls.Add(Me.PictureBox1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(861, 261)
        Me.Panel1.TabIndex = 2
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.Color.Black
        Me.PictureBox1.Dock = System.Windows.Forms.DockStyle.Top
        Me.PictureBox1.Location = New System.Drawing.Point(0, 0)
        Me.PictureBox1.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(861, 180)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBox1.TabIndex = 0
        Me.PictureBox1.TabStop = False
        '
        'verLabel
        '
        Me.verLabel.AutoSize = True
        Me.verLabel.Location = New System.Drawing.Point(92, 196)
        Me.verLabel.Name = "verLabel"
        Me.verLabel.Size = New System.Drawing.Size(54, 15)
        Me.verLabel.TabIndex = 1
        Me.verLabel.Text = "VERSION"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(92, 225)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(40, 15)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Links: "
        '
        'LinkLabel21
        '
        Me.LinkLabel21.ForeColor = System.Drawing.Color.White
        Me.LinkLabel21.HoverColor = System.Drawing.Color.Empty
        Me.LinkLabel21.Location = New System.Drawing.Point(138, 225)
        Me.LinkLabel21.Name = "LinkLabel21"
        Me.LinkLabel21.RegularColor = System.Drawing.Color.Empty
        Me.LinkLabel21.Size = New System.Drawing.Size(46, 16)
        Me.LinkLabel21.TabIndex = 5
        Me.LinkLabel21.Text = "GitHub"
        '
        'LinkLabel22
        '
        Me.LinkLabel22.ForeColor = System.Drawing.Color.White
        Me.LinkLabel22.HoverColor = System.Drawing.Color.Empty
        Me.LinkLabel22.Location = New System.Drawing.Point(216, 225)
        Me.LinkLabel22.Name = "LinkLabel22"
        Me.LinkLabel22.RegularColor = System.Drawing.Color.Empty
        Me.LinkLabel22.Size = New System.Drawing.Size(48, 16)
        Me.LinkLabel22.TabIndex = 5
        Me.LinkLabel22.Text = "Discord"
        '
        'LinkLabel23
        '
        Me.LinkLabel23.ForeColor = System.Drawing.Color.White
        Me.LinkLabel23.HoverColor = System.Drawing.Color.Empty
        Me.LinkLabel23.Location = New System.Drawing.Point(294, 225)
        Me.LinkLabel23.Name = "LinkLabel23"
        Me.LinkLabel23.RegularColor = System.Drawing.Color.Empty
        Me.LinkLabel23.Size = New System.Drawing.Size(50, 16)
        Me.LinkLabel23.TabIndex = 5
        Me.LinkLabel23.Text = "Website"
        '
        'LinkLabel24
        '
        Me.LinkLabel24.ForeColor = System.Drawing.Color.White
        Me.LinkLabel24.HoverColor = System.Drawing.Color.Empty
        Me.LinkLabel24.Location = New System.Drawing.Point(372, 225)
        Me.LinkLabel24.Name = "LinkLabel24"
        Me.LinkLabel24.RegularColor = System.Drawing.Color.Empty
        Me.LinkLabel24.Size = New System.Drawing.Size(70, 16)
        Me.LinkLabel24.TabIndex = 5
        Me.LinkLabel24.Text = "Help (Docs)"
        '
        'About
        '
        Me.AcceptButton = Me.okBtn
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(861, 305)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.Panel2)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ForeColor = System.Drawing.Color.White
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "About"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "About"
        Me.Panel2.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Panel2 As Panel
    Friend WithEvents okBtn As Button
    Friend WithEvents Panel1 As Panel
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents Label2 As Label
    Friend WithEvents verLabel As Label
    Friend WithEvents LinkLabel22 As wyDay.Controls.LinkLabel2
    Friend WithEvents LinkLabel21 As wyDay.Controls.LinkLabel2
    Friend WithEvents LinkLabel24 As wyDay.Controls.LinkLabel2
    Friend WithEvents LinkLabel23 As wyDay.Controls.LinkLabel2
End Class
