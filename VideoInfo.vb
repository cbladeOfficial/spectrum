Imports System.Drawing

Public Class VideoInfo
    Public Property FilePath As String
    Public Property Title As String
    Public Property Duration As String = "00:00:00"
    Public Property Width As Integer
    Public Property Height As Integer

    Public Property Resolution As String = Width & "x" & Height

    Public Property DateTaken As String
    Public Property Thumbnail As Image ' small preview frame

    Public Function ToListViewItem() As ListViewItem
        Dim item As New ListViewItem(Title)

        item.SubItems.Add(Duration)
        item.SubItems.Add(DateTaken)
        item.SubItems.Add(Resolution)

        item.ImageIndex = 1
        item.Tag = FilePath

        Return item
    End Function
End Class