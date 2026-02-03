Imports System.Reflection

Public Class EditPlaylistInfo

    Dim IsSaved As Boolean = True
    Private Sub EditPlaylistInfo_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Form1.playlistWindowFlag = 0 Then
            Text = "Create Playlist"
            PictureBox1.ImageLocation = Application.StartupPath & "\res\playlists.png"
            plnText.Text = ""
            lastPLN = ""
        Else
            Text = "Edit Playlist"
            Dim editP = SpectrumPlaylist.Load(Form1.cpl2)
            plnText.Text = editP.Name
            lastPLN = editP.Name
            If editP.ImagePath = Nothing Then
                PictureBox1.ImageLocation = Application.StartupPath & "\res\playlists.png"
            Else
                PictureBox1.ImageLocation = editP.ImagePath
            End If
            IsSaved = True
        End If

    End Sub

    Sub ConfigForEdit()
        PictureBox1.ImageLocation = Application.StartupPath & "\res\playlists.png"
    End Sub

    Private Sub piDel_Click(sender As Object, e As EventArgs) Handles piDel.Click
        PictureBox1.ImageLocation = Application.StartupPath & "\res\playlists.png"
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            Try
                PictureBox1.ImageLocation = OpenFileDialog1.FileName
                IsSaved = False
            Catch ex As Exception
                MsgBox("Invalid image.", MsgBoxStyle.Critical, "Error")
            End Try
        End If
    End Sub

    Private Sub EditPlaylistInfo_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If Not IsSaved Then
            Dim confirmMsg As DialogResult = MessageBox.Show("Are you sure you want to cancel and exit this dialog? The changes or the creation of the playlist will not be fulfilled!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
            If confirmMsg = DialogResult.Yes Then
                Hide()
            Else
                e.Cancel = True
            End If
        End If

    End Sub

    Public lastPLN As String = ""
    Private Sub plnText_TextChanged(sender As Object, e As EventArgs) Handles plnText.TextChanged
        If plnText.Text <> lastPLN Then
            IsSaved = False
        End If
    End Sub

    Private Sub pltRevert_Click(sender As Object, e As EventArgs) Handles pltRevert.Click
        If plnText.Text <> lastPLN Or Not String.IsNullOrEmpty(lastPLN) Then plnText.Text = lastPLN
    End Sub

    Private Sub okBtn_Click(sender As Object, e As EventArgs) Handles okBtn.Click
        If String.IsNullOrEmpty(plnText.Text) Then
            MsgBox("Please enter a playlist name and try again!", MsgBoxStyle.Critical, "Error")
            Exit Sub
        End If

        IsSaved = True
        Form1.ChooseSBTab(6)
        If Form1.playlistWindowFlag = 1 Then
            Dim editP = SpectrumPlaylist.Load(Form1.cpl2)
            editP.Name = plnText.Text
            editP.ImagePath = PictureBox1.ImageLocation
            editP.Save()
        Else
            Dim createdP As New SpectrumPlaylist
            createdP.Name = plnText.Text
            createdP.ImagePath = PictureBox1.ImageLocation
            createdP.PLPath = My.Settings.playlistLocs & "\" & plnText.Text & ".specpl"
            createdP.SaveCreated()
        End If

        If Form1.curTabIndex = 6 Then
            Form1.RefreshPL()
        End If
        Hide()
    End Sub

    Private Sub cancelBtn_Click(sender As Object, e As EventArgs) Handles cancelBtn.Click
        If Not IsSaved Then
            Dim confirmMsg As DialogResult = MessageBox.Show("Are you sure you want to cancel and exit this dialog? The changes or the creation of the playlist will not be fulfilled!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
            If confirmMsg = DialogResult.Yes Then
                Hide()
            End If
        End If
    End Sub
End Class