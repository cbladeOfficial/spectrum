Public Class SongInfo
    Public Property FilePath As String
    Public Property Title As String
    Public Property Artist As String
    Public Property Album As String
    Public Property Year As String
    Public Property Genre As String
    Public Property Duration As TimeSpan
    'Public Property AlbumArt As Image 'SCRAPPED
    'Public Property AlbumArtBytes As Byte() 'SCRAPPED


    'Private AlbumArtIndexCache As New Dictionary(Of String, Integer) 'SCRAPPED

    Public Function ToListViewItem() As ListViewItem
        Dim item As New ListViewItem(Title)

        item.SubItems.Add(Duration.ToString("hh\:mm\:ss"))
        item.SubItems.Add(Album)
        item.SubItems.Add(Artist)
        item.SubItems.Add(Year)
        item.SubItems.Add(Genre)

        item.ImageIndex = 0
        item.Tag = FilePath

        Return item
    End Function


End Class
