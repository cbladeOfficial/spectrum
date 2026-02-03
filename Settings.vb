Imports System.IO

Public Class Settings
    Public IsSaved As Boolean = True

    Private Sub RemoveDuplicatesFromListBox(ByVal listBox As ListBox)
        Dim distinctItems As Object() = listBox.Items.Cast(Of Object)().Distinct().ToArray()
        listBox.Items.Clear()
        listBox.Items.AddRange(distinctItems)
    End Sub
    Sub ChooseTab(index As Integer)
        Select Case index
            Case 0
                tab1.BackColor = Color.FromArgb(64, 64, 64)
                tab2.BackColor = Color.FromArgb(32, 32, 32)
                tab3.BackColor = Color.FromArgb(32, 32, 32)
                tabPG1.Dock = DockStyle.Fill
                tabPG2.Visible = False
                tabPG3.Visible = False
                tabPG1.Visible = True
            Case 1
                tab2.BackColor = Color.FromArgb(64, 64, 64)
                tab1.BackColor = Color.FromArgb(32, 32, 32)
                tab3.BackColor = Color.FromArgb(32, 32, 32)
                tabPG2.Dock = DockStyle.Fill
                tabPG1.Visible = False
                tabPG3.Visible = False
                tabPG2.Visible = True
            Case 2
                tab3.BackColor = Color.FromArgb(64, 64, 64)
                tab1.BackColor = Color.FromArgb(32, 32, 32)
                tab2.BackColor = Color.FromArgb(32, 32, 32)
                tabPG3.Dock = DockStyle.Fill
                tabPG1.Visible = False
                tabPG2.Visible = False
                tabPG3.Visible = True
        End Select
    End Sub

    Sub ClearAllSettings()
        musicDirList.Items.Clear()
        videoDirList.Items.Clear()
        'imageDirList.Items.Clear()
    End Sub
    Private Sub cancelBtn_Click(sender As Object, e As EventArgs) Handles cancelBtn.Click
        If IsSaved Then
            ClearAllSettings()
            Hide()
            Exit Sub
        Else
            Dim msg = MessageBox.Show("Are you sure you want to cancel? All unsaved changes will be lost.", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
            If msg = DialogResult.Yes Then
                ClearAllSettings()
                Hide()
            Else
                ClearAllSettings()
                Exit Sub
            End If
        End If
    End Sub

    Private Sub okBtn_Click(sender As Object, e As EventArgs) Handles okBtn.Click
        Try
            Form1._scanCts.Cancel()
            Using objWriter As New StreamWriter(Application.StartupPath + "\settings\musicDir.spectrum")
                For Each item In musicDirList.Items
                    objWriter.WriteLine(item)
                Next
            End Using
            Using objWriter As New StreamWriter(Application.StartupPath + "\settings\videoDir.spectrum")
                For Each item In videoDirList.Items
                    objWriter.WriteLine(item)
                Next
            End Using
            'Using objWriter As New StreamWriter(Application.StartupPath + "\settings\imageDir.spectrum")
            '    For Each item In imageDirList.Items
            '        objWriter.WriteLine(item)
            '    Next
            'End Using
            My.Settings.playlistLocs = plText.Text
        Catch ex As Exception
            MsgBox("Unable to save settings. Reason: " & ex.Message, MsgBoxStyle.Critical, "Error")
        End Try


        IsSaved = True
        My.Settings.Save()
        Hide()
    End Sub

    Sub getsettings()
        Try
            musicDirList.Items.AddRange(IO.File.ReadAllLines(Application.StartupPath + "\settings\musicDir.spectrum"))
            videoDirList.Items.AddRange(IO.File.ReadAllLines(Application.StartupPath + "\settings\videoDir.spectrum"))
            'imageDirList.Items.AddRange(IO.File.ReadAllLines(Application.StartupPath + "\settings\imageDir.spectrum"))
            'If My.Settings.keepSwitchingTP Then kstpBox.Checked = True Else kstpBox.Checked = False
            If My.Settings.playlistLocs <> "" Then
                plText.Text = My.Settings.playlistLocs
            Else
                plText.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) & "\Spectrum Playlists"
                My.Settings.playlistLocs = plText.Text
            End If
        Catch ex As Exception
            Debugger.Log(ex.HResult, "Error", ex.Message)
        End Try
    End Sub
    Private Sub Settings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        BetterFolderBrowser1.Multiselect = True
        ChooseTab(0)
        getsettings()
    End Sub

    Private Sub Settings_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If IsSaved Then
            ClearAllSettings()
            Exit Sub
        Else
            Dim msg = MessageBox.Show("Are you sure you want to cancel? All unsaved changes will be lost.", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
            If msg = DialogResult.Yes Then
                ClearAllSettings()
                Hide()
            Else
                e.Cancel = True
            End If
        End If
    End Sub

    Private Sub tab2_Click(sender As Object, e As EventArgs) Handles tab2.Click
        ChooseTab(1)
    End Sub

    Private Sub tab1_Click(sender As Object, e As EventArgs) Handles tab1.Click
        ChooseTab(0)
    End Sub

    Private Sub tab3_Click(sender As Object, e As EventArgs) Handles tab3.Click
        ChooseTab(2)
    End Sub

    Private Sub musicBrowse_Click(sender As Object, e As EventArgs) Handles musicBrowse.Click
        If Not musicDirList.Items.Contains(BetterFolderBrowser1.SelectedPaths) Then
            If BetterFolderBrowser1.ShowDialog() = DialogResult.OK Then
                For Each path In BetterFolderBrowser1.SelectedPaths
                    musicDirList.Items.Add(path)
                Next
                IsSaved = False
            End If
        End If
    End Sub

    Private Sub mdUP_Click(sender As Object, e As EventArgs) Handles mdUP.Click
        Dim i As Integer = musicDirList.SelectedIndex
        If i > 0 Then
            Dim item As Object = musicDirList.SelectedItem
            musicDirList.Items.RemoveAt(i)
            musicDirList.Items.Insert(i - 1, item)
            musicDirList.SelectedIndex = i - 1
            IsSaved = False
        End If
    End Sub

    Private Sub mdDown_Click(sender As Object, e As EventArgs) Handles mdDown.Click
        Dim i As Integer = musicDirList.SelectedIndex
        If i < musicDirList.Items.Count - 1 Then
            Dim item As Object = musicDirList.SelectedItem
            musicDirList.Items.RemoveAt(i)
            musicDirList.Items.Insert(i + 1, item)
            musicDirList.SelectedIndex = i + 1
            IsSaved = False
        End If
    End Sub

    Private Sub mdDel_Click(sender As Object, e As EventArgs) Handles mdDel.Click
        Try
            musicDirList.Items.RemoveAt(musicDirList.SelectedIndex)
            IsSaved = False
        Catch ex As Exception
            Exit Sub
        End Try
    End Sub

    Private Sub videoBrowse_Click(sender As Object, e As EventArgs) Handles videoBrowse.Click
        If Not videoDirList.Items.Contains(BetterFolderBrowser1.SelectedPaths) Then
            If BetterFolderBrowser1.ShowDialog() = DialogResult.OK Then
                For Each path In BetterFolderBrowser1.SelectedPaths
                    videoDirList.Items.Add(path)
                Next
                IsSaved = False
            End If
        End If
    End Sub

    Private Sub vlUP_Click(sender As Object, e As EventArgs) Handles vlUP.Click
        Dim i As Integer = videoDirList.SelectedIndex
        If i > 0 Then
            Dim item As Object = videoDirList.SelectedItem
            videoDirList.Items.RemoveAt(i)
            videoDirList.Items.Insert(i - 1, item)
            videoDirList.SelectedIndex = i - 1
            IsSaved = False
        End If
    End Sub

    Private Sub vlDown_Click(sender As Object, e As EventArgs) Handles vlDown.Click
        Dim i As Integer = videoDirList.SelectedIndex
        If i < videoDirList.Items.Count - 1 Then
            Dim item As Object = videoDirList.SelectedItem
            videoDirList.Items.RemoveAt(i)
            videoDirList.Items.Insert(i + 1, item)
            videoDirList.SelectedIndex = i + 1
            IsSaved = False
        End If
    End Sub

    Private Sub VlDel_Click(sender As Object, e As EventArgs) Handles VlDel.Click
        videoDirList.Items.RemoveAt(videoDirList.SelectedIndex)
        IsSaved = False
    End Sub

    Private Sub mlDelAll_Click(sender As Object, e As EventArgs) Handles mlDelAll.Click
        Dim confirmation As DialogResult = MessageBox.Show("Are you sure you want to delete all music directories?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        If confirmation = DialogResult.Yes Then
            musicDirList.Items.Clear()
            IsSaved = False
        End If
    End Sub

    Private Sub vlDelAll_Click(sender As Object, e As EventArgs) Handles vlDelAll.Click
        Dim confirmation As DialogResult = MessageBox.Show("Are you sure you want to delete all video directories?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        If confirmation = DialogResult.Yes Then
            videoDirList.Items.Clear()
            IsSaved = False
        End If
    End Sub

    Private Sub plBrowse_Click(sender As Object, e As EventArgs) Handles plBrowse.Click
        BetterFolderBrowser1.Multiselect = False
        If BetterFolderBrowser1.ShowDialog() = DialogResult.OK Then
            plText.Text = BetterFolderBrowser1.SelectedPath
        End If
        BetterFolderBrowser1.Multiselect = True
    End Sub


    'Private Sub ilDelAll_Click(sender As Object, e As EventArgs)
    '    Dim confirmation As DialogResult = MessageBox.Show("Are you sure you want to delete all image directories?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
    '    If confirmation = DialogResult.Yes Then
    '        imageDirList.Items.Clear()
    '        IsSaved = False
    '    End If
    'End Sub
End Class