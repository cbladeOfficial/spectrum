Imports System.Text

Public Class SpectrumPlaylist

    Public Property Name As String
    Public Property ImagePath As String
    Public Property Created As DateTime
    Public Property Files As New List(Of String)
    Public Property PLPath As String

    Public ReadOnly MAX_VER As Double = 1.1F

    Public ReadOnly MIN_VER As Double = 1.0F

    '==================== LOAD ====================
    Public Shared Function Load(path As String) As SpectrumPlaylist
        Dim pl As New SpectrumPlaylist With {.PLPath = path}

        Dim hasHeader As Boolean = False
        Dim hasVersion As Boolean = False
        Dim inMeta As Boolean = False

        For Each rawLine In IO.File.ReadAllLines(path)
            Dim line = rawLine.Trim()
            If line = "" Then Continue For

            ' DIRECTIVES
            If line.StartsWith("!!!") Then
                Dim content = line.Substring(3).Trim()

                ' Strip inline comments ONLY for directives
                Dim hash = content.IndexOf("#"c)
                If hash >= 0 Then content = content.Substring(0, hash).Trim()

                Select Case True
                    Case content = "SPECTRUM_PLAYLIST"
                        hasHeader = True

                    Case content.StartsWith("SYNTAX_VERSION=")
                        Dim PlaylistVersions = New SpectrumPlaylist
                        hasVersion = True
                        Dim sv As Double = Val(content.Replace("SYNTAX_VERSION=", ""))
                        If sv > PlaylistVersions.MAX_VER And sv < PlaylistVersions.MIN_VER Then
                            MsgBox("The syntax version of the found playlist is unsupported. Currently supported versions are 1.0 and 1.1, please fix it and restart Spectrum to load the playlist!", MsgBoxStyle.Critical, "Error")
                            Exit Select
                        End If

                    Case content = "[META]"
                        If Not hasHeader OrElse Not hasVersion Then
                            Throw New Exception("META block before header/version")
                        End If
                        inMeta = True

                    Case inMeta AndAlso content.StartsWith("PLAYLIST_NAME=")
                        pl.Name = content.Substring(14)

                    Case inMeta AndAlso content.StartsWith("PLAYLIST_IMAGE=")
                        pl.ImagePath = content.Substring(15)

                    Case inMeta AndAlso content.StartsWith("CREATED=")
                        DateTime.TryParse(content.Substring(8), pl.Created)
                End Select

            Else
                If Not line.StartsWith("#") Then
                    ' FILE ENTRIES (allow #)
                    pl.Files.Add(line)
                Else Continue For
                End If
            End If
        Next

        ' VALIDATION
        If Not hasHeader Then Throw New Exception("Missing SPECTRUM_PLAYLIST header")
        If Not hasVersion Then Throw New Exception("Missing SYNTAX_VERSION")
        If String.IsNullOrWhiteSpace(pl.Name) Then
            pl.Name = IO.Path.GetFileNameWithoutExtension(path)
        End If

        Return pl
    End Function
    Public Sub Save()
        If String.IsNullOrEmpty(PLPath) Then
            Throw New InvalidOperationException("Playlist path not set")
        End If

        Dim sb As New StringBuilder()

        ' ===== HEADER =====
        sb.AppendLine("!!! SPECTRUM_PLAYLIST")
        sb.AppendLine("!!! SYNTAX_VERSION=" & MAX_VER.ToString())
        sb.AppendLine()

        ' ===== META =====
        sb.AppendLine("!!! [META]")
        sb.AppendLine("!!! PLAYLIST_NAME=" & Name)

        If Not String.IsNullOrEmpty(ImagePath) Then
            sb.AppendLine("!!! PLAYLIST_IMAGE=" & ImagePath)
        End If

        sb.AppendLine("!!! CREATED=" & Created.ToString("o"))
        sb.AppendLine()

        ' ===== INFO BLOCK =====
        sb.AppendLine("#=======================================================================#")
        sb.AppendLine("# THIS IS AN AUTO GENERATED PLAYLIST | PLEASE DO NOT REMOVE THE HEADER! #")
        sb.AppendLine("# ALL PLAYLIST FILES ARE BELOW       |          AS WELL AS THE VERSION! #")
        sb.AppendLine("#=======================================================================#")
        sb.AppendLine()

        ' ===== FILE ENTRIES =====
        For Each file In Files
            sb.AppendLine(file)
        Next

        IO.File.WriteAllText(PLPath, sb.ToString(), Encoding.UTF8)
    End Sub

    Public Sub SaveCreated()
        If String.IsNullOrEmpty(PLPath) Then
            Throw New InvalidOperationException("Playlist path not set")
        End If

        Dim sb As New StringBuilder()

        ' ===== HEADER =====
        sb.AppendLine("!!! SPECTRUM_PLAYLIST")
        sb.AppendLine("!!! SYNTAX_VERSION=" & MAX_VER.ToString())
        sb.AppendLine()

        ' ===== META =====
        sb.AppendLine("!!! [META]")
        sb.AppendLine("!!! PLAYLIST_NAME=" & Name)

        If Not String.IsNullOrEmpty(ImagePath) Then
            sb.AppendLine("!!! PLAYLIST_IMAGE=" & ImagePath)
        End If

        sb.AppendLine("!!! CREATED=" & Created.ToString("o"))
        sb.AppendLine()

        ' ===== INFO BLOCK =====
        sb.AppendLine("#=======================================================================#")
        sb.AppendLine("# THIS IS AN AUTO GENERATED PLAYLIST | PLEASE DO NOT REMOVE THE HEADER! #")
        sb.AppendLine("# ALL PLAYLIST FILES ARE BELOW       |          AS WELL AS THE VERSION! #")
        sb.AppendLine("#=======================================================================#")
        sb.AppendLine()


        IO.File.WriteAllText(PLPath, sb.ToString(), Encoding.UTF8)
    End Sub


    Public Function ToListViewItem() As ListViewItem
        Dim item As New ListViewItem(Name)
        item.ImageIndex = 4
        item.Tag = PLPath
        Return item
    End Function

End Class
