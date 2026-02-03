<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EditRadioInfo
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
        Me.cancelBtn = New System.Windows.Forms.Button()
        Me.nameBox = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.urlBox = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.countryBox = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.genreDel = New System.Windows.Forms.Button()
        Me.gDelAll = New System.Windows.Forms.Button()
        Me.genreDown = New System.Windows.Forms.Button()
        Me.genreUP = New System.Windows.Forms.Button()
        Me.genreAdd = New System.Windows.Forms.Button()
        Me.genreList = New System.Windows.Forms.ListBox()
        Me.revert3 = New System.Windows.Forms.Button()
        Me.revert2 = New System.Windows.Forms.Button()
        Me.revert1 = New System.Windows.Forms.Button()
        Me.Panel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.FromArgb(CType(CType(24, Byte), Integer), CType(CType(24, Byte), Integer), CType(CType(24, Byte), Integer))
        Me.Panel2.Controls.Add(Me.okBtn)
        Me.Panel2.Controls.Add(Me.cancelBtn)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel2.ForeColor = System.Drawing.Color.White
        Me.Panel2.Location = New System.Drawing.Point(0, 306)
        Me.Panel2.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(567, 38)
        Me.Panel2.TabIndex = 7
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
        'nameBox
        '
        Me.nameBox.BackColor = System.Drawing.Color.FromArgb(CType(CType(48, Byte), Integer), CType(CType(48, Byte), Integer), CType(CType(48, Byte), Integer))
        Me.nameBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.nameBox.ForeColor = System.Drawing.Color.White
        Me.nameBox.Location = New System.Drawing.Point(104, 33)
        Me.nameBox.Name = "nameBox"
        Me.nameBox.Size = New System.Drawing.Size(394, 23)
        Me.nameBox.TabIndex = 14
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(53, 35)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(45, 15)
        Me.Label1.TabIndex = 15
        Me.Label1.Text = "Name: "
        '
        'urlBox
        '
        Me.urlBox.BackColor = System.Drawing.Color.FromArgb(CType(CType(48, Byte), Integer), CType(CType(48, Byte), Integer), CType(CType(48, Byte), Integer))
        Me.urlBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.urlBox.ForeColor = System.Drawing.Color.White
        Me.urlBox.Location = New System.Drawing.Point(104, 62)
        Me.urlBox.Name = "urlBox"
        Me.urlBox.Size = New System.Drawing.Size(394, 23)
        Me.urlBox.TabIndex = 14
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(24, 64)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(74, 15)
        Me.Label2.TabIndex = 15
        Me.Label2.Text = "Stream URL: "
        '
        'countryBox
        '
        Me.countryBox.BackColor = System.Drawing.Color.FromArgb(CType(CType(48, Byte), Integer), CType(CType(48, Byte), Integer), CType(CType(48, Byte), Integer))
        Me.countryBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.countryBox.ForeColor = System.Drawing.Color.White
        Me.countryBox.Location = New System.Drawing.Point(104, 91)
        Me.countryBox.Name = "countryBox"
        Me.countryBox.Size = New System.Drawing.Size(394, 23)
        Me.countryBox.TabIndex = 14
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(42, 93)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(56, 15)
        Me.Label3.TabIndex = 15
        Me.Label3.Text = "Country: "
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(54, 120)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(44, 15)
        Me.Label4.TabIndex = 15
        Me.Label4.Text = "Genre: "
        '
        'genreDel
        '
        Me.genreDel.FlatAppearance.BorderSize = 0
        Me.genreDel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.genreDel.Location = New System.Drawing.Point(465, 267)
        Me.genreDel.Name = "genreDel"
        Me.genreDel.Size = New System.Drawing.Size(33, 23)
        Me.genreDel.TabIndex = 16
        Me.genreDel.Text = "—"
        Me.genreDel.UseVisualStyleBackColor = True
        '
        'gDelAll
        '
        Me.gDelAll.FlatAppearance.BorderSize = 0
        Me.gDelAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.gDelAll.Location = New System.Drawing.Point(496, 267)
        Me.gDelAll.Name = "gDelAll"
        Me.gDelAll.Size = New System.Drawing.Size(33, 23)
        Me.gDelAll.TabIndex = 17
        Me.gDelAll.Text = "✖"
        Me.gDelAll.UseVisualStyleBackColor = True
        '
        'genreDown
        '
        Me.genreDown.FlatAppearance.BorderSize = 0
        Me.genreDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.genreDown.Location = New System.Drawing.Point(429, 267)
        Me.genreDown.Name = "genreDown"
        Me.genreDown.Size = New System.Drawing.Size(33, 23)
        Me.genreDown.TabIndex = 18
        Me.genreDown.Text = "˅"
        Me.genreDown.UseVisualStyleBackColor = True
        '
        'genreUP
        '
        Me.genreUP.FlatAppearance.BorderSize = 0
        Me.genreUP.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.genreUP.Location = New System.Drawing.Point(390, 267)
        Me.genreUP.Name = "genreUP"
        Me.genreUP.Size = New System.Drawing.Size(33, 23)
        Me.genreUP.TabIndex = 19
        Me.genreUP.Text = "˄"
        Me.genreUP.UseVisualStyleBackColor = True
        '
        'genreAdd
        '
        Me.genreAdd.FlatAppearance.BorderSize = 0
        Me.genreAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.genreAdd.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.genreAdd.Location = New System.Drawing.Point(353, 267)
        Me.genreAdd.Name = "genreAdd"
        Me.genreAdd.Size = New System.Drawing.Size(31, 23)
        Me.genreAdd.TabIndex = 20
        Me.genreAdd.Text = "➕"
        Me.genreAdd.UseVisualStyleBackColor = True
        '
        'genreList
        '
        Me.genreList.BackColor = System.Drawing.Color.FromArgb(CType(CType(48, Byte), Integer), CType(CType(48, Byte), Integer), CType(CType(48, Byte), Integer))
        Me.genreList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.genreList.ForeColor = System.Drawing.Color.White
        Me.genreList.FormattingEnabled = True
        Me.genreList.ItemHeight = 15
        Me.genreList.Location = New System.Drawing.Point(104, 120)
        Me.genreList.Name = "genreList"
        Me.genreList.ScrollAlwaysVisible = True
        Me.genreList.Size = New System.Drawing.Size(427, 137)
        Me.genreList.TabIndex = 21
        '
        'revert3
        '
        Me.revert3.FlatAppearance.BorderSize = 0
        Me.revert3.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.revert3.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.revert3.Location = New System.Drawing.Point(504, 91)
        Me.revert3.Name = "revert3"
        Me.revert3.Size = New System.Drawing.Size(33, 23)
        Me.revert3.TabIndex = 17
        Me.revert3.Text = "⟲"
        Me.revert3.UseVisualStyleBackColor = True
        '
        'revert2
        '
        Me.revert2.FlatAppearance.BorderSize = 0
        Me.revert2.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.revert2.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.revert2.Location = New System.Drawing.Point(504, 62)
        Me.revert2.Name = "revert2"
        Me.revert2.Size = New System.Drawing.Size(33, 23)
        Me.revert2.TabIndex = 17
        Me.revert2.Text = "⟲"
        Me.revert2.UseVisualStyleBackColor = True
        '
        'revert1
        '
        Me.revert1.FlatAppearance.BorderSize = 0
        Me.revert1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.revert1.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.revert1.Location = New System.Drawing.Point(504, 33)
        Me.revert1.Name = "revert1"
        Me.revert1.Size = New System.Drawing.Size(33, 23)
        Me.revert1.TabIndex = 17
        Me.revert1.Text = "⟲"
        Me.revert1.UseVisualStyleBackColor = True
        '
        'EditRadioInfo
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(567, 344)
        Me.Controls.Add(Me.genreList)
        Me.Controls.Add(Me.genreDel)
        Me.Controls.Add(Me.revert1)
        Me.Controls.Add(Me.revert2)
        Me.Controls.Add(Me.revert3)
        Me.Controls.Add(Me.gDelAll)
        Me.Controls.Add(Me.genreDown)
        Me.Controls.Add(Me.genreUP)
        Me.Controls.Add(Me.genreAdd)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.countryBox)
        Me.Controls.Add(Me.urlBox)
        Me.Controls.Add(Me.nameBox)
        Me.Controls.Add(Me.Panel2)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ForeColor = System.Drawing.Color.White
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "EditRadioInfo"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Edit Radio"
        Me.Panel2.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Panel2 As Panel
    Friend WithEvents okBtn As Button
    Friend WithEvents cancelBtn As Button
    Friend WithEvents nameBox As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents urlBox As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents countryBox As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents genreDel As Button
    Friend WithEvents gDelAll As Button
    Friend WithEvents genreDown As Button
    Friend WithEvents genreUP As Button
    Friend WithEvents genreAdd As Button
    Friend WithEvents genreList As ListBox
    Friend WithEvents revert3 As Button
    Friend WithEvents revert2 As Button
    Friend WithEvents revert1 As Button
End Class
