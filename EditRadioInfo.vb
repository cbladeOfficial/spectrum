Imports System.IO
Imports System.Runtime.Remoting.Contexts
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports YamlDotNet.Core

Public Class EditRadioInfo
    Public Property Radio As RadioStation

    Dim IsSaved As Boolean = True

    Dim eradio As RadioStation

    Private Sub EditRadioInfo_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Form1.radioWindowFlag = 0 Then
            Text = "Add Radio"
            eradio = New RadioStation
            nameBox.Text = ""
            urlBox.Text = ""
            countryBox.Text = ""
            genreList.Items.Clear()
        Else
            Text = "Edit Radio"
            
            If Form1.ListView1.SelectedItems.Count > 0 Then
                Dim item = Form1.ListView1.SelectedItems(0)
                eradio = CType(Form1.ListView1.SelectedItems(0).Tag, RadioStation)
                nameBox.Text = eradio.name
                urlBox.Text = eradio.stream_url
                countryBox.Text = eradio.country
                genreList.Items.Clear()
                For Each genre In eradio.genre
                    genreList.Items.Add(genre)
                Next
                lastname = eradio.name
                lasturl = eradio.stream_url
                lastcountry = eradio.country
            Else
                MsgBox("Please select a radio first and then try again!", MsgBoxStyle.Critical, "Error")
                IsSaved = True
                Close()
            End If

        End If
    End Sub

    Private Sub genreAdd_Click(sender As Object, e As EventArgs) Handles genreAdd.Click
        Dim newGenre = InputBox("Enter the genre: ", "Input")
        If Not String.IsNullOrEmpty(newGenre) Then
            genreList.Items.Add(newGenre)
            IsSaved = False
        End If
    End Sub

    Private Sub genreUP_Click(sender As Object, e As EventArgs) Handles genreUP.Click
        Dim i As Integer = genreList.SelectedIndex
        If i > 0 Then
            Dim item As Object = genreList.SelectedItem
            genreList.Items.RemoveAt(i)
            genreList.Items.Insert(i - 1, item)
            genreList.SelectedIndex = i - 1
            IsSaved = False
        End If
    End Sub

    Private Sub genreDown_Click(sender As Object, e As EventArgs) Handles genreDown.Click
        Dim i As Integer = genreList.SelectedIndex
        If i < genreList.Items.Count - 1 Then
            Dim item As Object = genreList.SelectedItem
            genreList.Items.RemoveAt(i)
            genreList.Items.Insert(i + 1, item)
            genreList.SelectedIndex = i + 1
            IsSaved = False
        End If
    End Sub

    Private Sub genreDel_Click(sender As Object, e As EventArgs) Handles genreDel.Click
        Try
            genreList.Items.RemoveAt(genreList.SelectedIndex)
            IsSaved = False
        Catch ex As Exception
            Exit Sub
        End Try
    End Sub

    Private Sub gDelAll_Click(sender As Object, e As EventArgs) Handles gDelAll.Click
        Dim confirmation As DialogResult = MessageBox.Show("Are you sure you want to delete all genres?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        If confirmation = DialogResult.Yes Then
            genreList.Items.Clear()
            IsSaved = False
        End If
    End Sub

    Dim lastname As String = ""
    Dim lasturl As String = ""
    Dim lastcountry As String = ""
    Private Sub TextBox6_TextChanged(sender As Object, e As EventArgs) Handles nameBox.TextChanged

    End Sub

    Private Sub revert1_Click(sender As Object, e As EventArgs) Handles revert1.Click
        If nameBox.Text <> lastname Or Not String.IsNullOrEmpty(lastname) Then nameBox.Text = lastname
    End Sub

    Private Sub revert2_Click(sender As Object, e As EventArgs) Handles revert2.Click
        If urlBox.Text <> lasturl Or Not String.IsNullOrEmpty(lasturl) Then urlBox.Text = lasturl
    End Sub

    Private Sub revert3_Click(sender As Object, e As EventArgs) Handles revert3.Click
        If countryBox.Text <> lastcountry Or Not String.IsNullOrEmpty(lastcountry) Then countryBox.Text = lastcountry
    End Sub

    Dim path As String = Application.StartupPath & "\radios\radios.yaml"
    Private Sub okBtn_Click(sender As Object, e As EventArgs) Handles okBtn.Click
        If Form1.radioWindowFlag = 1 Then
            Dim eradio = CType(Form1.ListView1.SelectedItems(0).Tag, RadioStation)

            Dim radios = Form1.LoadRadios(path)

            Dim realRadio =
    radios.FirstOrDefault(Function(r) r.id = eradio.id)

            If realRadio Is Nothing Then Exit Sub

            If Not String.IsNullOrEmpty(nameBox.Text) Then
                realRadio.name = nameBox.Text
            Else
                MsgBox("Radio name required!", MsgBoxStyle.Critical, "Error")
                Exit Sub
            End If
            If Not String.IsNullOrEmpty(urlBox.Text) Then
                realRadio.stream_url = urlBox.Text
            Else
                MsgBox("Stream URL required!", MsgBoxStyle.Critical, "Error")
                Exit Sub
            End If
            realRadio.country = countryBox.Text
            realRadio.genre = genreList.Items.Cast(Of String).ToList()

            Form1.SaveRadios(path, radios)
            IsSaved = True
            Hide()
        Else
            Dim radios = Form1.LoadRadios(path)

            ' Generate a safe unique ID
            Dim baseId = nameBox.Text.Trim().
                ToLower().
                Replace(" ", "_")

            Dim newId = baseId
            Dim i As Integer = 1

            While radios.Any(Function(r) r.id = newId)
                newId = baseId & "_" & i
                i += 1
            End While

            If Not String.IsNullOrEmpty(nameBox.Text) Or Not String.IsNullOrEmpty(urlBox.Text) Then


                Dim newRadio As New RadioStation With {
                    .id = newId,
                    .name = nameBox.Text,
                    .stream_url = urlBox.Text,
                    .country = countryBox.Text,
                    .genre = genreList.Items.Cast(Of String).ToList()
                }
                IsSaved = True
                radios.Add(newRadio)
                Form1.SaveRadios(path, radios)
            Else
                MsgBox("Please check the name or the stream URL as " & If(String.IsNullOrEmpty(nameBox.Text), "the radio name", "the stream URL") & " seems to be empty!", MsgBoxStyle.Critical, "Error")
                Exit Sub
            End If


            Hide()
        End If
        Form1.ChooseSBTab(4)
    End Sub
End Class