<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EditPlaylistInfo
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
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.plnText = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.okBtn = New System.Windows.Forms.Button()
        Me.cancelBtn = New System.Windows.Forms.Button()
        Me.piDel = New System.Windows.Forms.Button()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.pltRevert = New System.Windows.Forms.Button()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'PictureBox1
        '
        Me.PictureBox1.Cursor = System.Windows.Forms.Cursors.Hand
        Me.PictureBox1.Location = New System.Drawing.Point(205, 42)
        Me.PictureBox1.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(175, 175)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBox1.TabIndex = 0
        Me.PictureBox1.TabStop = False
        '
        'plnText
        '
        Me.plnText.BackColor = System.Drawing.Color.FromArgb(CType(CType(48, Byte), Integer), CType(CType(48, Byte), Integer), CType(CType(48, Byte), Integer))
        Me.plnText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.plnText.ForeColor = System.Drawing.Color.White
        Me.plnText.Location = New System.Drawing.Point(205, 265)
        Me.plnText.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.plnText.Name = "plnText"
        Me.plnText.Size = New System.Drawing.Size(344, 23)
        Me.plnText.TabIndex = 4
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(113, 267)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(85, 15)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "Playlist Name: "
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.FromArgb(CType(CType(24, Byte), Integer), CType(CType(24, Byte), Integer), CType(CType(24, Byte), Integer))
        Me.Panel2.Controls.Add(Me.okBtn)
        Me.Panel2.Controls.Add(Me.cancelBtn)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel2.ForeColor = System.Drawing.Color.White
        Me.Panel2.Location = New System.Drawing.Point(0, 304)
        Me.Panel2.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(567, 38)
        Me.Panel2.TabIndex = 6
        '
        'okBtn
        '
        Me.okBtn.Dock = System.Windows.Forms.DockStyle.Right
        Me.okBtn.FlatAppearance.BorderSize = 0
        Me.okBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.okBtn.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.okBtn.Location = New System.Drawing.Point(391, 0)
        Me.okBtn.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.okBtn.Name = "okBtn"
        Me.okBtn.Size = New System.Drawing.Size(88, 38)
        Me.okBtn.TabIndex = 0
        Me.okBtn.Text = "&OK"
        Me.okBtn.UseVisualStyleBackColor = True
        '
        'cancelBtn
        '
        Me.cancelBtn.Dock = System.Windows.Forms.DockStyle.Right
        Me.cancelBtn.FlatAppearance.BorderSize = 0
        Me.cancelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cancelBtn.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cancelBtn.Location = New System.Drawing.Point(479, 0)
        Me.cancelBtn.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.cancelBtn.Name = "cancelBtn"
        Me.cancelBtn.Size = New System.Drawing.Size(88, 38)
        Me.cancelBtn.TabIndex = 0
        Me.cancelBtn.Text = "&Cancel"
        Me.cancelBtn.UseVisualStyleBackColor = True
        '
        'piDel
        '
        Me.piDel.FlatAppearance.BorderSize = 0
        Me.piDel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.piDel.Location = New System.Drawing.Point(387, 42)
        Me.piDel.Name = "piDel"
        Me.piDel.Size = New System.Drawing.Size(33, 23)
        Me.piDel.TabIndex = 7
        Me.piDel.Text = "✖"
        Me.piDel.UseVisualStyleBackColor = True
        '
        'OpenFileDialog1
        '
        '
        'pltRevert
        '
        Me.pltRevert.FlatAppearance.BorderSize = 0
        Me.pltRevert.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.pltRevert.Font = New System.Drawing.Font("Segoe UI Semibold", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.pltRevert.Location = New System.Drawing.Point(516, 227)
        Me.pltRevert.Name = "pltRevert"
        Me.pltRevert.Size = New System.Drawing.Size(33, 32)
        Me.pltRevert.TabIndex = 7
        Me.pltRevert.Text = "⟲"
        Me.pltRevert.UseVisualStyleBackColor = True
        '
        'EditPlaylistInfo
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(567, 342)
        Me.Controls.Add(Me.pltRevert)
        Me.Controls.Add(Me.piDel)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.plnText)
        Me.Controls.Add(Me.PictureBox1)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ForeColor = System.Drawing.Color.White
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "EditPlaylistInfo"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Edit Playlist"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel2.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents plnText As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Panel2 As Panel
    Friend WithEvents okBtn As Button
    Friend WithEvents cancelBtn As Button
    Friend WithEvents piDel As Button
    Friend WithEvents OpenFileDialog1 As OpenFileDialog
    Friend WithEvents pltRevert As Button
End Class
