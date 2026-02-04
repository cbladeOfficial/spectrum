Imports System.ComponentModel
Imports System.Configuration
Imports System.Drawing
Imports System.IO
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Security.Cryptography.X509Certificates
Imports System.Text.Json
Imports System.Threading
Imports System.Timers
Imports gTrackBar
Imports LibVLCSharp.Shared
Imports LibVLCSharp.WinForms
Imports Microsoft.Web.WebView2.Core
Imports Microsoft.Web.WebView2.WinForms
Imports Microsoft.WindowsAPICodePack.Shell
Imports Microsoft.WindowsAPICodePack.Shell.PropertySystem
Imports NAudio.CoreAudioApi
Imports NAudio.Dsp
Imports NAudio.FileFormats
Imports NAudio.Wave
Imports Spectrum.Form1
Imports TagLib
Imports TagLib.Ape
Imports TagLib.Riff
Imports YamlDotNet.Serialization
Imports YamlDotNet.Serialization.NamingConventions

'if you see name pattern changes such as beginning with uppercase or lowercase, it depends on my mood at that time of creating the variables
Public Class Form1

    Public ItemsBackup As New List(Of ListViewItem)



    Dim _libVLC As New LibVLC("--aout=wasapi")



    Public _mediaPlayer As New MediaPlayer(_libVLC)

    Public loopback As WasapiLoopbackCapture

    Public IsPlaylistLoaded As Boolean = False
    Public IsDefaultArt As Boolean = True
    Public HowLooping As String = My.Settings.repeatMethod 'none, one, all

    Dim ShuffledIndexes As New List(Of Integer)
    Dim CurrentShufflePos As Integer = 0

    Public MusicCache As New List(Of SongInfo)
    Public VideoCache As New List(Of VideoInfo)
    'public ImageCache As New List(Of ImageInfo)
    Public RadioCache As New List(Of RadioStation)
    Public PLCache As New List(Of SpectrumPlaylist)

    Public ReadOnly AudioExts As HashSet(Of String) =
    New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
        ".mp3", ".flac", ".wav", ".m4a", ".ogg", ".aac", ".opus"
    }

    Public ReadOnly VideoExts As HashSet(Of String) =
    New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
        ".mp4", ".avi", ".mov", ".wmv", ".mpg", ".mpeg", ".mkv", ".webm"
    }

    Public Enum PlaylistKind
        AudioOnly
        VideoOnly
        Mixed
        Unknown
    End Enum

    Public Const BatchSize As Integer = 1
    Public lastUiIndex As Integer = 0

    Public isUserScrubbing As Boolean = False

    Public canSaveSettings = False

    Dim WasMutedDuringStart As Boolean = True


    Dim capture As WasapiLoopbackCapture
    Dim fftSize As Integer = 1024
    Dim fftBuffer(fftSize - 1) As NAudio.Dsp.Complex
    Dim fftPos As Integer = 0
    Dim fftResults(fftSize \ 2 - 1) As Single





    Enum TrackType
        Music
        Video
        Radio
        Disc
    End Enum

    Public CurrentTrackType As TrackType


    'rivate AlbumArtManager As AlbumArtManager


    'AddHandler() _mediaPlayer.EndReached, AddressOf OnMediaEnded

    Sub StartAudioCapture()
        capture = New WasapiLoopbackCapture()

        AddHandler capture.DataAvailable, AddressOf OnDataAvailable
        capture.StartRecording()
    End Sub


    Public Function DetectPlaylistKind(files As List(Of String)) As PlaylistKind
        Dim hasAudio = False
        Dim hasVideo = False

        For Each f In files
            Dim ext = IO.Path.GetExtension(f)

            If AudioExts.Contains(ext) Then hasAudio = True
            If VideoExts.Contains(ext) Then hasVideo = True

            If hasAudio AndAlso hasVideo Then
                Return PlaylistKind.Mixed
            End If
        Next

        If hasAudio Then Return PlaylistKind.AudioOnly
        If hasVideo Then Return PlaylistKind.VideoOnly
        Return PlaylistKind.Unknown
    End Function

    Public Sub OnDataAvailable(sender As Object, e As WaveInEventArgs)
        For i = 0 To e.BytesRecorded - 1 Step 4
            Dim sample As Single = BitConverter.ToSingle(e.Buffer, i)

            fftBuffer(fftPos).X = sample
            fftBuffer(fftPos).Y = 0
            fftPos += 1

            If fftPos >= fftSize Then
                fftPos = 0
                PerformFFT()
            End If
        Next
    End Sub


    Sub PerformFFT()
        Dim buffer = CType(fftBuffer.Clone(), NAudio.Dsp.Complex())

        FastFourierTransform.FFT(
        True,
        CInt(Math.Log(fftSize, 2)),
        buffer
    )

        For i = 0 To fftResults.Length - 1
            Dim mag = Math.Sqrt(buffer(i).X * buffer(i).X + buffer(i).Y * buffer(i).Y)
            fftResults(i) = CSng(Math.Min(mag * 8, 1.0F))
        Next

        SendFFTToWeb2()
    End Sub

    Sub SendFFTToWeb()
        If visualizer.CoreWebView2 Is Nothing Then Exit Sub

        Dim json = System.Text.Json.JsonSerializer.Serialize(fftResults)
        visualizer.CoreWebView2.PostWebMessageAsJson(json)
    End Sub

    Sub SendFFTToWeb2()
        If InvokeRequired Then
            BeginInvoke(Sub() SendFFTToWeb())
        Else
            SendFFTToWeb()
        End If
    End Sub
    Sub InitControls()
        'If _mediaPlayer.IsPlaying Then
        playBtn.Enabled = True
        stopBtn.Enabled = True
        repeatBtn.Enabled = True
        playSlider.Enabled = True
        speedBtn.Enabled = True
        'playSlider.Value = 0
        nextBtn.Enabled = True
        previousBtn.Enabled = True
        shuffleBtn.Enabled = True
        playbackTimer.Start()
        If _mediaPlayer.IsPlaying Then
            playBtn.Text = "❚❚"
        Else
            playBtn.Text = "▶"
        End If
        'End If
    End Sub

    Public playOrder As New List(Of Integer)
    Public currentIndex As Integer = -1
    Public isShuffle As Boolean = My.Settings.wantsToShuffle
    Public rng As New Random()

    Public Sub ShuffleList(list As List(Of Integer))
        'MsgBox(ListView1.Items.Count)
        For i = ListView1.Items.Count - 1 To 1 Step -1
            Dim j = rng.Next(i + 1)
            Dim temp = list(i)
            list(i) = list(j)
            list(j) = temp
        Next


    End Sub

    Sub PlayFromNextBtn()
        Dim currentFileIndex = GetIndexByTag(currentFile)
        ListView1.SelectedItems.Clear()
        Dim ni As Integer = currentFileIndex + 1
        ListView1.Items(ni).Selected = True
        ListView1.Items(ni).Focused = True
        PlayFile(ListView1.Items(ni).Tag.ToString())
        currentFile = ListView1.Items(ni).Tag.ToString()
    End Sub

    Sub PlayFromPrevBtn()
        Dim currentFileIndex = GetIndexByTag(currentFile)
        ListView1.SelectedItems.Clear()
        Dim ni As Integer = currentFileIndex - 1
        ListView1.Items(ni).Selected = True
        ListView1.Items(ni).Focused = True
        PlayFile(ListView1.Items(ni).Tag.ToString())
        currentFile = ListView1.Items(ni).Tag.ToString()
    End Sub

    Public Function GetIndexByTag(ByVal tagValue As Object) As Integer
        ' Loop through each item in the ListView
        For Each item As ListViewItem In ListView1.Items
            ' Check if the current item's Tag matches the search tagValue
            If item.Tag IsNot Nothing AndAlso item.Tag.Equals(tagValue) Then
                ' If found, return the zero-based index
                Return item.Index
            End If
        Next

        ' If the item is not found, return -1 (or handle as appropriate)
        Return 0
    End Function

    Public Sub BuildPlayOrder()
        playOrder.Clear()

        For i = 0 To ListView1.Items.Count - 1
            playOrder.Add(i)
        Next

        If isShuffle Then
            ShuffleList(playOrder)
        End If

        currentIndex = -1
    End Sub

    Public Sub PlayNext()
        If playOrder.Count = 0 Then Exit Sub
        If Not isShuffle Then Exit Sub
        playingNextSeq = True
        currentIndex += 1
        If currentIndex >= playOrder.Count Then
            If isShuffle Then
                ShuffleList(playOrder)
                currentIndex = 0
            Else
                currentIndex = playOrder.Count - 1
                Exit Sub
            End If
        End If

        Dim lvIndex = playOrder(currentIndex)
        Try
            Dim item = ListView1.Items(lvIndex)
            item.Selected = True
            item.EnsureVisible()
            Dim filePath = item.Tag.ToString()
            PlayFile(filePath)
            currentFile = ListView1.Items(lvIndex).Tag.ToString()
        Catch ex As Exception

        End Try



    End Sub

    'Dim bk As New List(Of ListViewItem)()
    Sub ShowPlayer()
        videoViewPane.Visible = True
        videoViewPane.Dock = DockStyle.Fill
        ListView1.Visible = False
        ListView1.Dock = DockStyle.Top
    End Sub

    Dim playingNextSeq As Boolean = False
    Dim lastIcon As String = ""

    Dim curSongInfos() As String = {}
    Dim cursonginfos2() As String

    Dim hasBeenSwitched As Short = 0

    Sub PFCodeMain(path As String, Optional pos As Long = 0)
        'MsgBox(hasBeenSwitched)
        lastTab = curTabIndex
        If Not hasBeenSwitched >= 1 Then ChooseSBTab(8)
        InitControls()

        If lastTab = 4 Or curTabIndex = 4 Then
            labelSong.Text = ""
            labelArtist.Text = ""
        End If


        fieldI = 0

        If pos = 0 Then

            Using media As New Media(_libVLC, New Uri(path))
                _mediaPlayer.Time = pos
                'MsgBox(pos & ", " & _mediaPlayer.Time)
                hasBeenSwitched += 1
                'MsgBox(hasBeenSwitched)
                _mediaPlayer.Play(media)
            End Using

        Else

            _mediaPlayer.Time = pos
            _mediaPlayer.Play()

        End If



        playbackTimer.Start()

        playBtn.Text = "❚❚"

        Dim title = ""

        Dim curSong As String = ""
        Dim curArtists As String = ""

        If ((lastTab = 0 OrElse lastTab = 1) And (curTabIndex <> 6)) Or (HasExtension(currentFile, {".mp3", ".flac", ".wav", ".m4a", ".ogg", ".aac", ".opus"})) Or curTabIndex = 9 Or lastTab = 9 Then
            If EnableVisualizationsToolStripMenuItem.Checked Then
                ShowVisualizerAsync()
            Else
                HideVisualizer()
            End If

            Try

                Using t = TagLib.File.Create(path)
                    title = If(String.IsNullOrEmpty(t.Tag.Title),
                               IO.Path.GetFileNameWithoutExtension(path),
                               t.Tag.Title)

                    Dim artist = String.Join(", ", t.Tag.Performers)

                    If labelSong.Text <> curSong Then
                        labelSong.Text = title
                        labelSong.ForeColor = Color.White
                        labelArtist.Text = artist
                        songTextQA.Text = title
                    Else
                        labelSong.Text = curSong
                        labelSong.ForeColor = Color.White
                        labelArtist.Text = curArtists
                        songTextQA.Text = curSong
                    End If


                    ''debug.writeline("artists: " & String.Join(", ", t.Tag.Performers))

                    curSongInfos = {
                        "Now Playing: " & title,
                        If(Not String.IsNullOrEmpty(t.Tag.Album), "From " & t.Tag.Album, ""),
                        If(t.Tag.Performers IsNot Nothing, "By " & String.Join(", ", t.Tag.Performers), ""),
                        If(t.Tag.Year = Nothing, "", "Released in " & t.Tag.Year.ToString()),
                        If(t.Tag.Genres Is Nothing OrElse t.Tag.Genres.Length = 0, "", "Based on " & String.Join(", ", t.Tag.Genres))
                    }

                    cursonginfos2 = curSongInfos.Where(Function(s) Not String.IsNullOrWhiteSpace(s)).ToArray()

                    curSong = title
                    curArtists = artist

                    If t.Tag.Pictures.Length > 0 Then
                        Using ms As New IO.MemoryStream(t.Tag.Pictures(0).Data.Data)
                            Dim img = Image.FromStream(ms)
                            IsDefaultArt = False
                            quickActionsPB.Image = img
                            playbackAlbumArt.Image = quickActionsPB.Image
                            'img.Dispose()
                        End Using

                    Else
                        IsDefaultArt = True
                        If HasExtension(path, {".mp4", ".avi", ".mov", ".wmv", ".mpg", ".mpeg", ".mkv", ".webm"}) Then
                            quickActionsPB.Image = GetFileThumbnail(path, 128)
                        Else
                            GetIcons()
                            ChooseCorrectIcon()
                        End If

                        playbackAlbumArt.Image = quickActionsPB.Image
                    End If
                End Using
            Catch ex As Exception
                If curTabIndex = 6 Then
                    MsgBox(currentFile)
                End If
            End Try

            Try
                currentFile = ListView1.SelectedItems(0).Tag.ToString()
                nowPlayingText.Text = "Now Playing: " & title.ToString()
                Dim ext = IO.Path.GetExtension(currentFile).ToLower()
                If {".mp3", ".flac", ".wav", ".m4a", ".ogg", ".aac", ".opus"}.Contains(ext) Then

                    nowPlayingInfo.Visible = True
                    nowPlayingCycle.Start()
                Else
                    Try
                        nowPlayingCycle.Stop()
                        nowPlayingInfo.Visible = False
                    Catch ex As Exception

                    End Try
                End If
            Catch ex As Exception

            End Try

        ElseIf lastTab = 2 Or (HasExtension(path, {".mp4", ".avi", ".mov", ".wmv", ".mpg", ".mpeg", ".mkv", ".webm"})) Then
            nowPlayingCycle.Stop()
            nowPlayingInfo.Visible = False
            songTextQA.Text = IO.Path.GetFileNameWithoutExtension(currentFile)
            labelSong.Text = songTextQA.Text
            quickActionsPB.Image = GetFileThumbnail(path, 128)
            playbackAlbumArt.Image = quickActionsPB.Image
            labelArtist.Text = ""
            labelSong.ForeColor = Color.White
            nowPlayingInfo.Visible = False
        ElseIf lastTab = 4 Then

            If ListView1.SelectedItems(0).SubItems(3).Text <> "Offline" And ListView1.SelectedItems(0).SubItems(3).Text <> "Checking" Then
                labelSong.Text = songTextQA.Text
                labelSong.ForeColor = Color.White
                playbackAlbumArt.Image = quickActionsPB.Image
                labelArtist.Text = "Live"
                playSlider.Enabled = False
                If My.Settings.showVizOnStart Then
                    EnableVisualizationsToolStripMenuItem.Checked = True
                    nowPlayingText.Visible = False
                    visualizer.Visible = True
                Else
                    EnableVisualizationsToolStripMenuItem.Checked = False
                    visualizer.Visible = False
                    nowPlayingText.Visible = True
                End If
                If EnableVisualizationsToolStripMenuItem.Checked Then
                    ShowVisualizerAsync()
                Else
                    HideVisualizer()
                End If
            Else

                MsgBox("Radio is either offline or in an undetermined status. Please check your stream URL or try again later.", MsgBoxStyle.Critical, "Error")
                _mediaPlayer.Stop()
                capture.StopRecording()
                ChooseSBTab(lastTab)
            End If
        End If
        If lastTab = 2 Then
                VideoView1.Visible = True
            End If
    End Sub
    Sub PlayFile(path As String, Optional pos As Long = 0)
        If lastTab <> 6 Then PFCodeMain(path, pos)
    End Sub

    Dim songsList As New List(Of ListViewItem)



    'public Sub AddSong(path As String)

    '    Try
    '        'ListView1.BeginUpdate()
    '        Using t = TagLib.File.Create(path)
    '            Dim title = If(String.IsNullOrEmpty(t.Tag.Title),
    '                       IO.Path.GetFileNameWithoutExtension(path),
    '                       t.Tag.Title)

    '            Dim artist = String.Join(", ", t.Tag.Performers)
    '            Dim duration = t.Properties.Duration

    '            Dim item As New ListViewItem(title)

    '            item.SubItems.Add(duration.ToString("hh\:mm\:ss"))
    '            item.SubItems.Add(t.Tag.Album)
    '            item.SubItems.Add(artist)

    '            If Not t.Tag.Year = 0 Then
    '                item.SubItems.Add(CInt(t.Tag.Year).ToString())
    '            Else
    '                item.SubItems.Add("")
    '            End If
    '            item.Tag = path
    '            item.SubItems.Add(t.Tag.JoinedGenres)
    '            ListView1.Items.Add(item)



    '            If t.Tag.Pictures.Length > 0 Then
    '                Using ms As New IO.MemoryStream(t.Tag.Pictures(0).Data.Data)
    '                    Dim img = Image.FromStream(ms)
    '                    ImageList1.Images.Add(img)
    '                    item.ImageIndex = ImageList1.Images.Count - 1
    '                End Using
    '            End If


    '            For Each songitem As ListViewItem In ListView1.Items
    '                songsList.Add(CType(item.Clone(), ListViewItem))
    '            Next

    '        End Using
    '        'ListView1.EndUpdate()
    '    Catch ex As TagLib.CorruptFileException


    '    Catch ex As Exception

    '    End Try
    'End Sub

    Public Sub AddSong(filePath As String)
        Dim song = ReadSongMetadata(filePath)
        If song Is Nothing Then Exit Sub

        SyncLock MusicCache

            MusicCache.Add(song)


        End SyncLock
    End Sub


    Dim currentFile As String = ""
    Public Sub ScaninBG()


    End Sub
    Public Sub StartMusicScan()

        Task.Run(Sub()

                     ScanMusic(_scanCts.Token)

                     ' scan is DONE here
                     ScanCompleted()
                 End Sub)
    End Sub

    Public Sub StartPLScan()

        Task.Run(Sub()

                     ScanPL(_scanCts.Token)

                     ' scan is DONE here
                     ScanCompleted()
                 End Sub)
    End Sub

    Public Sub ScanPL(token As CancellationToken)
        If InvokeRequired Then
            BeginInvoke(Sub()
                            ScanPL(_scanCts.Token)
                        End Sub)
        End If

        Dim addedCount As Integer = 0

        lastUiIndex = 0
        PLCache.Clear()

        If InvokeRequired Then
            BeginInvoke(Sub() ListView1.Items.Clear())
        Else
            ListView1.Items.Clear()
        End If

        For Each file In IO.Directory.EnumerateFiles(
My.Settings.playlistLocs.ToString(), "*.*", SearchOption.AllDirectories)

            If token.IsCancellationRequested Then Exit Sub

            Dim ext = IO.Path.GetExtension(file).ToLower()
            If Not {".specpl", ".specplx", ".spplx"}.Contains(ext) Then
                Continue For
            End If

            Me.BeginInvoke(Sub() AddToToolStripMenuItem.DropDownItems.Clear())

            SyncLock PLCache
                Dim newPL = SpectrumPlaylist.Load(file)
                PLCache.Add(newPL)
                Me.BeginInvoke(Sub()
                                   Dim newPX As New ToolStripMenuItem(newPL.Name)
                                   newPX.Tag = newPL.PLPath
                                   cpl2 = newPX.Tag.ToString()
                                   AddHandler newPX.Click, AddressOf newPX_Click

                                   AddToToolStripMenuItem.DropDownItems.Add(newPX)
                               End Sub)

            End SyncLock
        Next





        ''debug.writeline("Songs scanned: " & MusicCache.Count)
        'BatchUpdateUI()


    End Sub

    Public cpl2 As String = ""
    Sub newPX_Click()
        If ListView1.SelectedItems.Count > 0 Then
            If Not String.IsNullOrEmpty(cpl2) Then
                Dim newPL = SpectrumPlaylist.Load(cpl2)
                For Each item As ListViewItem In ListView1.SelectedItems
                    If Not newPL.Files.Contains(item.Tag.ToString()) Then
                        newPL.Files.Add(item.Tag.ToString())
                        MsgBox("Selected files successfully added to playlist!", MsgBoxStyle.Information, "Success")
                    Else
                        MsgBox("Selected files are already in the playlist!", MsgBoxStyle.Critical, "Error")
                    End If

                Next
                newPL.Save()
            End If
        End If


    End Sub

    Sub GetPlaylistFiles()
        ListView1.AllowDrop = True
        ListView1.Items.Clear()
        curTabIndex = 9
        currentPlaylist = SpectrumPlaylist.Load(currentFile)
        For Each filePath In currentPlaylist.Files
            If Not IO.File.Exists(filePath) Then Continue For
            Using t = TagLib.File.Create(filePath)
                Dim item As New ListViewItem(If(String.IsNullOrEmpty(t.Tag.Title),
                               IO.Path.GetFileNameWithoutExtension(filePath),
                               t.Tag.Title))
                item.SubItems.Add(IO.Path.GetExtension(filePath))
                item.SubItems.Add(GetVideoDuration(filePath).ToString("hh\:mm\:ss")) 'works for audio + video
                item.Tag = filePath
                ListView1.Items.Add(item)
            End Using
        Next
        CacheOriginalOrder()
        curTabIndex = 9
        lastTab = 6
        BuildPlayOrder()
    End Sub


    Public Sub StartVideoScan()

        Task.Run(Sub()

                     ScanVideo(_scanCts.Token)

                     ' scan is DONE here
                     ScanCompleted()
                 End Sub)
    End Sub

    'public Sub StartImageScan()

    '    Task.Run(Sub()

    '                 ScanImage(_scanCts.Token)

    '                 ' scan is DONE here
    '                 ScanCompleted()
    '             End Sub)
    'End Sub

    Public Sub ScanCompleted()
        If Me.InvokeRequired Then
            Me.BeginInvoke(Sub() PopulateMusicUI())
            Me.BeginInvoke(Sub() PopulateVideoUI())
            Me.BeginInvoke(Sub() PopulatePLUI())
            'Me.BeginInvoke(Sub() PopulateImageUI())
        Else
            PopulateMusicUI()
            PopulateVideoUI()
            PopulatePLUI()
        End If
    End Sub



    Dim resDir As String = Application.StartupPath & "\res"

    Sub GetIcons()
        Try
            If labelSong.Text = "Not Playing" And (curTabIndex <> 0 Or curTabIndex <> 2) Then

                Select Case curTabIndex
                    Case 1
                        playbackAlbumArt.Image = Image.FromFile(resDir & "\music.png")
                    Case 2
                        playbackAlbumArt.Image = Image.FromFile(resDir & "\videos.png")
                    Case 3
                        playbackAlbumArt.Image = Image.FromFile(resDir + "\pictures.png")
                    Case 4
                        playbackAlbumArt.Image = Image.FromFile(resDir + "\radio.png")
                    Case 5
                        playbackAlbumArt.Image = Image.FromFile(resDir + "\cd.png")
                    Case 6
                        playbackAlbumArt.Image = Image.FromFile(resDir + "\playlists.png")
                    Case 7
                        playbackAlbumArt.Image = Image.FromFile(resDir + "\help.png")
                    Case 8
                        'gets last icon
                    Case Else
                        playbackAlbumArt.Image = Image.FromFile(resDir & "\music.png")
                End Select
                quickActionsPB.Image = playbackAlbumArt.Image
            End If

            'playbackAlbumArt.Image = Image.FromFile(resDir & "\cd.png")

        Catch ex As Exception
            MsgBox("Unable to load resource icons. Exiting." & vbCrLf & "Details: " & ex.Message, MsgBoxStyle.Critical, "Error")
            End
        End Try

    End Sub

    Public Function LoadRadios(path As String) As List(Of RadioStation)

        If Not IO.File.Exists(path) Then
            'debug.writeline("YAML load failed: file does not exist")
            Return New List(Of RadioStation)
        End If

        Dim deserializer = New DeserializerBuilder().
    IgnoreUnmatchedProperties().
    Build()

        Using reader As New StreamReader(path)
            Dim root As RadioRoot = Nothing

            Try
                root = deserializer.Deserialize(Of RadioRoot)(reader)
            Catch

            End Try

            Try
                Return root.radios
            Catch ex As Exception

            End Try

        End Using

    End Function
    Public Sub RefreshRadios()
        scanCts?.Cancel()
        scanCts = New CancellationTokenSource()

        ListView1.Items.Clear()

        Task.Run(Sub() ScanRadios(scanCts.Token))
    End Sub

    Public Sub ScanRadios(token As CancellationToken)

        Dim radios As List(Of RadioStation)

        Try
            radios = LoadRadios(Application.StartupPath & "\radios\radios.yaml") ' YAML parsing here
            'debug.writeline("able to parse during scan")
        Catch
            Try
                radios = LoadRadios(Application.StartupPath & "\radios\radios.yml")
            Catch
                radios = New List(Of RadioStation)
            End Try
        End Try

        If token.IsCancellationRequested Then Exit Sub

        SyncLock RadioCache
            RadioCache.Clear()
            RadioCache.AddRange(radios)
            'debug.writeline("able to add all radios")
        End SyncLock

        NotifyRadioUI()

        For Each station In radios
            If token.IsCancellationRequested Then Exit Sub

            station.status = RadioStatus.Checking
            UpdateSingleRadioUI(station)

            Task.Run(Async Function()
                         Await CheckRadioStatusAsync(station, token)
                     End Function)
        Next
    End Sub

    Public Async Function CheckRadioStatusAsync(
    station As RadioStation,
    token As CancellationToken) As Task
        Dim sv As String = "Unknown"
        Try
            Using client As New Net.Http.HttpClient()
                client.Timeout = TimeSpan.FromSeconds(4)

                Using response = Await client.SendAsync(
                New Net.Http.HttpRequestMessage(
                    Net.Http.HttpMethod.Head,
                    station.stream_url),
                token)

                    station.status =
                    If(response.IsSuccessStatusCode Or response.StatusCode = 405 Or response.StatusCode <> 404,
                       RadioStatus.Online,
                       RadioStatus.Offline)
                End Using
            End Using

        Catch
            If Not token.IsCancellationRequested Then
                station.status = RadioStatus.Offline
                sv = station.status
            Else
                station.status = RadioStatus.Unknown
            End If
        End Try

        UpdateSingleRadioUI(station)
    End Function

    Public Sub UpdateSingleRadioUI(station As RadioStation, Optional status As String = "Unknown")

        If IsDisposed Then Exit Sub

        If InvokeRequired Then
            BeginInvoke(Sub() UpdateSingleRadioUI(station))
            Return
        End If

        For Each item As ListViewItem In ListView1.Items
            If item.Tag Is station Then
                item.SubItems(3).Text = station.status.ToString()
                Exit For
            End If
        Next
    End Sub


    Public Sub NotifyRadioUI()
        If IsDisposed Then Exit Sub

        If InvokeRequired Then
            BeginInvoke(Sub() NotifyRadioUI())
            Return
        End If

        PopulateRadioUI()
        'debug.writeline("able to notify")
    End Sub

    Enum RadioStatus
        Unknown
        Checking
        Online
        Offline
    End Enum

    Public Sub PopulateRadioUI()
        If Me.IsDisposed Then Exit Sub

        If Me.InvokeRequired Then
            Me.BeginInvoke(Sub() PopulateRadioUI())
            Return
        End If

        If curTabIndex = 4 Then
            ListView1.Items.Clear()
            ListView1.Columns.Clear()
            ListView1.BeginUpdate()
            ListView1.Columns.Add("Title", 290, HorizontalAlignment.Left)
            ListView1.Columns.Add("Country", 290, HorizontalAlignment.Left)
            ListView1.Columns.Add("Genre", 290, HorizontalAlignment.Left)
            ListView1.Columns.Add("Status", 150, HorizontalAlignment.Left)

            SyncLock RadioCache
                For Each station In RadioCache
                    'debug.writeline("test: " & station.name)
                    Dim item As New ListViewItem(station.name)
                    item.SubItems.Add(station.country)
                    item.SubItems.Add(String.Join(", ", station.genre))
                    item.SubItems.Add(station.status.ToString())
                    item.Tag = station

                    ListView1.Items.Add(item)
                Next
            End SyncLock
            ListView1.EndUpdate()

        End If


    End Sub


    Public Sub SendToWebView(buffer() As Single)


        If visualizer Is Nothing OrElse visualizer.CoreWebView2 Is Nothing Then Return

        Dim bars(63) As Single

        For i = 0 To bars.Length - 1
            bars(i) = buffer(i * 16)
        Next

        Dim js =
$"window.chrome.webview.postMessage({{
    type: 'audio',
    values: [{String.Join(",", bars.Select(Function(v) v.ToString("0.000", Globalization.CultureInfo.InvariantCulture)))}]
}});"

        visualizer.BeginInvoke(Sub()
                                   visualizer.CoreWebView2.ExecuteScriptAsync(js)
                               End Sub)
    End Sub

    Public Const FALLBACK_VIZ As String =
        "<!doctype html>
        <html>
        <head>
        <meta charset='utf-8'>
        <style>
        html,body{
          margin:0;
          width:100%;
          height:100%;
          background:black;
          color:black;
          display:flex;
          align-items:center;
          justify-content:center;
          font-family:system-ui;
        }
        * {
          user-select: none;
        }
        </style>
        </head>
        <body>
        &nbsp;
        </body>
        <script>
        document.addEventListener('contextmenu', e => { e.preventDefault(); window.chrome.webview.postMessage( 'contextmenu|' + e.clientX + '|' + e.clientY ); });
        document.addEventListener('mousedown', e => {
          if (e.button === 0) { // left click
            chrome.webview.postMessage({ type: 'hideContextMenu' });
          }
        });
        </script>
        </html>"

    Public Sub GetAllViz()
        Dim vizPath = Path.Combine(Application.StartupPath, "visualizations")

        Dim files = Directory.GetFiles(vizPath, "*.html").
            Concat(Directory.GetFiles(vizPath, "*.htm")).
            Distinct()

        Dim noneViz As New ToolStripMenuItem("(none)")
        noneViz.Tag = "(none)"
        AddHandler noneViz.Click, AddressOf VisualizerChoice_Click
        VisualizationsToolStripMenuItem.DropDownItems.Add(noneViz)

        For Each foundFile In files
            Dim newViz As New ToolStripMenuItem(
            Path.GetFileNameWithoutExtension(foundFile)
        )
            If Path.GetFileNameWithoutExtension(foundFile) <> "default" Then
                newViz.Tag = Path.GetFileName(foundFile)
                newViz.CheckOnClick = True
                AddHandler newViz.Click, AddressOf VisualizerChoice_Click
                VisualizationsToolStripMenuItem.DropDownItems.Add(newViz)
            End If
        Next
    End Sub


    Public Sub SelectSavedViz()
        Dim saved = My.Settings.defaultViz ' e.g. "Rainbow Bars.html"

        For Each item As ToolStripMenuItem In VisualizationsToolStripMenuItem.DropDownItems
            item.Checked = False

            If String.Equals(
            CStr(item.Tag),
            saved,
            StringComparison.OrdinalIgnoreCase
        ) Then
                item.Checked = True

            End If
        Next
    End Sub


    Public Sub VisualizerChoice_Click(sender As Object, e As EventArgs)
        Dim clicked = DirectCast(sender, ToolStripMenuItem)

        For Each itm As ToolStripMenuItem In VisualizationsToolStripMenuItem.DropDownItems
            itm.Checked = False
        Next

        clicked.Checked = True

        If clicked.Tag.ToString() <> "(none)" Then
            visualizer.CoreWebView2.Navigate(New Uri(Application.StartupPath & "\visualizations\" & clicked.Tag.ToString()).AbsoluteUri)
            My.Settings.defaultViz = clicked.Tag.ToString()
        Else
            visualizer.CoreWebView2.Navigate(New Uri(Application.StartupPath & "\visualizations\default.html").AbsoluteUri)
            My.Settings.defaultViz = "(none)"
        End If

        My.Settings.defaultViz = clicked.Tag.ToString()
    End Sub

    Public Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim musicplDir As String = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) & "\Spectrum Playlists"
        If Not IO.Directory.Exists(musicplDir) And (My.Settings.playlistLocs = musicplDir Or Settings.plText.Text = musicplDir) Then
            IO.Directory.CreateDirectory(musicplDir)
        End If
        StartAudioCapture()
        Try
            GetAllViz()
            SelectSavedViz()
        Catch ex As Exception

        End Try

        AddHandler _mediaPlayer.Playing, AddressOf OnPlaying
        AddHandler _mediaPlayer.Paused, AddressOf OnPaused
        AddHandler _mediaPlayer.Stopped, AddressOf OnStopped
        'Dim server As New PcAudioServer(visualizer)
        'server.StartServer(8173)
        'MainMenuStrip = MenuStrip1
        'Me.Controls.SetChildIndex(MenuStrip1, 0)
        If My.Settings.showVizOnStart And EnableVisualizationsToolStripMenuItem.Checked = False Then
            EnableVisualizationsToolStripMenuItem.Checked = True
            nowPlayingText.Visible = False
            visualizer.Visible = True
        Else
            EnableVisualizationsToolStripMenuItem.Checked = False
            visualizer.Visible = False
            nowPlayingText.Visible = True
        End If
        curTabIndex = If(My.Settings.defaultTab <> 0, My.Settings.defaultTab, 1)
        GetIcons()
        ImageList1.ImageSize = New Size(16, 16)
        ImageList1.ColorDepth = ColorDepth.Depth32Bit
        ImageList1.Images.Add(Image.FromFile(Application.StartupPath + "\res\music.png"))
        ImageList1.Images.Add(Image.FromFile(Application.StartupPath + "\res\videos.png"))

        If Not Directory.Exists(Application.StartupPath + "\settings") Then
            Directory.CreateDirectory(Application.StartupPath + "\settings")
        End If
        If Not Directory.Exists(Application.StartupPath + "\res") Then
            Directory.CreateDirectory(Application.StartupPath + "\res")
        End If
        If Not Directory.Exists(Application.StartupPath + "\visualizations") Then
            Directory.CreateDirectory(Application.StartupPath + "\visualizations")
        End If
        If Not Directory.Exists(Application.StartupPath + "\radios") Then
            Directory.CreateDirectory(Application.StartupPath + "\radios")
        End If
        Dim radioPath As String = Application.StartupPath + "\radios"
        If Not IO.File.Exists(Application.StartupPath & "\visualizations\default.html") Then
            Try
                IO.File.WriteAllText(Application.StartupPath & "\visualizations\default.html", FALLBACK_VIZ)
            Catch
                'file already exists!
            End Try
        End If

        SetCueText(searchBox, "Search the list here...")
        Core.Initialize()

        VideoView1.MediaPlayer = _mediaPlayer


        _mediaPlayer.Volume = volSlider.Value
        labelSong.ForeColor = Color.Gray
        labelArtist.Text = ""
        ChooseSBTab(1)

        ListView1.View = View.Details
        ListView1.FullRowSelect = True
        ListView1.HideSelection = False

        If Not Directory.Exists(Application.StartupPath & "\playlists") Then
            Directory.CreateDirectory(Application.StartupPath & "\playlists")
        End If

        If My.Settings.defaultVol = 0 Then
            volSlider.Value = 0
            volAmt.Text = "0"
            WasMutedDuringStart = True
        Else
            WasMutedDuringStart = False
        End If

        'ScaninBG()

        AddHandler _mediaPlayer.EndReached, AddressOf OnMediaEnded
        'AddHandler _mediaPlayer.EndReached, AddressOf OnStartingShuffle
        AddHandler _mediaPlayer.Buffering, AddressOf OnBuffering
        'AddHandler _mediaPlayer.Playing, AddressOf OnPlaying


        If isShuffle Then
            shuffleBtn.BackColor = Color.FromArgb(56, 56, 56)
        Else
            shuffleBtn.BackColor = Color.FromArgb(64, 64, 64)
        End If

        If HowLooping = "none" Then
            repeatBtn.Text = "🔁"
            repeatBtn.BackColor = Color.FromArgb(64, 64, 64)
        ElseIf HowLooping = "all" Then
            repeatBtn.Text = "🔁"
            repeatBtn.BackColor = Color.FromArgb(56, 56, 56)
        Else
            repeatBtn.Text = "🔂"
            repeatBtn.BackColor = Color.FromArgb(52, 52, 52)
        End If

        If My.Settings.defaultViz = "(none)" Then
            visualizer.Source = New Uri(Application.StartupPath & "\visualizations\default.html")
        Else
            visualizer.Source = New Uri(Application.StartupPath & "\visualizations\" & My.Settings.defaultViz.ToString())
        End If

        cpl2 = ""
    End Sub

    'public Sub UpdateBars()
    '    ' Simulate bars with random values modulated by volume
    '    Dim barCount As Integer = 64
    '    Dim bars(barCount - 1) As Double

    '    For i As Integer = 0 To barCount - 1
    '        ' Random amplitude modulated by volume (0..1)
    '        bars(i) = rndViz.NextDouble() * _mediaPlayer.Volume / 100
    '    Next

    '    ' Serialize and send to WebView2
    '    Dim json As String = JsonSerializer.Serialize(New With {Key .type = "bars", Key .values = bars})
    '    visualizer.CoreWebView2.PostWebMessageAsJson(json)
    'End Sub

    Sub OnBuffering(sender As Object, e As MediaPlayerBufferingEventArgs)
        If Me.InvokeRequired Then
            Me.BeginInvoke(Sub() OnBuffering(sender, e))
        Else
            If lastTab = 4 OrElse curTabIndex = 4 Then
                If e.Cache >= 100.0F And ListView1.SelectedItems(0).SubItems(3).Text <> "Offline" Then
                    labelArtist.Text = "Live"
                    If EnableVisualizationsToolStripMenuItem.Checked Then
                        ShowVisualizerAsync()
                    Else
                        HideVisualizer()
                    End If
                Else
                    labelArtist.Text = "Buffering..."
                End If
            End If

        End If

    End Sub


    Function OnPlaying(sender As Object, e As EventArgs) As Task
        'If InvokeRequired Then
        '    BeginInvoke(Sub() visualizer.CoreWebView2.PostWebMessageAsString("viz:play"))
        'Else
        '    visualizer.CoreWebView2.PostWebMessageAsString("viz:play")
        'End If
    End Function

    Function OnPaused(sender As Object, e As EventArgs) As Task
        'If InvokeRequired Then
        '    BeginInvoke(Sub() visualizer.CoreWebView2.PostWebMessageAsString("viz:pause"))
        'Else
        '    visualizer.CoreWebView2.PostWebMessageAsString("viz:pause")
        'End If

    End Function

    Function OnStopped(sender As Object, e As EventArgs) As Task
        'If InvokeRequired Then
        '    BeginInvoke(Sub() visualizer.CoreWebView2.PostWebMessageAsString("viz:stop"))
        'Else
        '    visualizer.CoreWebView2.PostWebMessageAsString("viz:stop")
        'End If

    End Function

    Public _scanCts As New Threading.CancellationTokenSource

    Public Sub volSlider_ValueChanged(sender As Object, e As EventArgs) Handles volSlider.ValueChanged

    End Sub

    Public Sub UpdateVolSlider(sender As Object, mouseX As Integer)

        Dim tb As gTrackBar.gTrackBar = DirectCast(sender, gTrackBar.gTrackBar)

        Dim percent As Double = mouseX / tb.Width
        Dim newValue As Integer =
        tb.MinValue + CInt((tb.MaxValue - tb.MinValue) * percent)
        If newValue < tb.MinValue Then newValue = tb.MinValue
        If newValue > tb.MaxValue Then newValue = tb.MaxValue

        If tb.Value <> newValue Then
            tb.Value = newValue
        End If

        volAmt.Text = tb.Value.ToString()
        _mediaPlayer.Volume = volSlider.Value
        My.Settings.defaultVol = Int(volSlider.Value)


    End Sub

    Dim playSliderPos2 As Integer = 0
    Public Sub UpdatePlaySlider(sender As Object, mouseX As Integer)

        Dim tb As gTrackBar.gTrackBar = DirectCast(sender, gTrackBar.gTrackBar)

        Dim percent As Double = mouseX / tb.Width
        Dim newValue As Integer =
        tb.MinValue + CInt((tb.MaxValue - tb.MinValue) * percent)
        If newValue < tb.MinValue Then newValue = tb.MinValue
        If newValue > tb.MaxValue Then newValue = tb.MaxValue

        If tb.Value <> newValue Then
            tb.Value = newValue

        End If

        If tb.Value = tb.MaxValue Then
            playBtn.Text = "▶"
            stopBtn.Enabled = False
            _mediaPlayer.Time = tb.Value
            _mediaPlayer.Stop()
            capture.StopRecording()
        End If

        playSlider.Value = tb.Value
        _mediaPlayer.Time = playSlider.Value
        curTimePos = playSlider.Value

        If _mediaPlayer.Length < 3600000 Then
            startPos.Text = TimeSpan.FromMilliseconds(_mediaPlayer.Time).ToString("mm\:ss")
        Else
            startPos.Text = TimeSpan.FromMilliseconds(_mediaPlayer.Time).ToString("hh\:mm\:ss")
        End If

    End Sub

    Public Sub volSlider_MouseDown(sender As Object, e As MouseEventArgs) Handles volSlider.MouseDown

        UpdateVolSlider(sender, e.X)
    End Sub

    Public Sub volSlider_MouseMove(sender As Object, e As MouseEventArgs) Handles volSlider.MouseMove


        If e.Button <> MouseButtons.Left Then Return
        UpdateVolSlider(sender, e.X)
    End Sub

    Public Sub LsVisible_Click(sender As Object, e As EventArgs) Handles LsVisible.Click
        If leftSidebar.Width = 250 Then
            leftSidebar.Width = 18
            LsVisible.Text = "▶"
        Else
            leftSidebar.Width = 250
            LsVisible.Text = "◀"
        End If
    End Sub

    Public Sub rsVisible_Click(sender As Object, e As EventArgs) Handles rsVisible.Click

        If rightSidebar.Width = 340 Then
            rightSidebar.Width = 18
            rsVisible.Text = "◀"
        Else
            rightSidebar.Width = 340
            rsVisible.Text = "▶"
        End If
    End Sub

    Public Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        capture.StopRecording()
        My.Settings.Save()
    End Sub

    Public Sub playbackTimer_Tick(sender As Object, e As EventArgs) Handles playbackTimer.Tick
        'If isUserScrubbing Then Exit Sub

        Try
            playSlider.MaxValue = _mediaPlayer.Length
            playSlider.Value = _mediaPlayer.Time
            If _mediaPlayer.Length < 3600000 Then
                startPos.Text = TimeSpan.FromMilliseconds(_mediaPlayer.Time).ToString("mm\:ss")
            Else
                startPos.Text = TimeSpan.FromMilliseconds(_mediaPlayer.Time).ToString("hh\:mm\:ss")
            End If

            If _mediaPlayer.Length < 3600000 Then 'bad implementation but it works
                totalPos.Text = TimeSpan.FromMilliseconds(_mediaPlayer.Length).ToString("mm\:ss")
            Else
                totalPos.Text = TimeSpan.FromMilliseconds(_mediaPlayer.Length).ToString("hh\:mm\:ss")
            End If

            'if the time is something like 25:00:01 then the user is a psychopath

        Catch ex As Exception

        End Try
    End Sub

    Public Sub playSlider_MouseDown(sender As Object, e As MouseEventArgs) Handles playSlider.MouseDown
        isUserScrubbing = True
        UpdatePlaySlider(sender, e.X)
    End Sub

    Public Sub playSlider_MouseMove(sender As Object, e As MouseEventArgs) Handles playSlider.MouseMove
        If e.Button <> MouseButtons.Left Then Return
        UpdatePlaySlider(sender, e.X)
    End Sub

    Public Sub speedBtn_Click(sender As Object, e As EventArgs) Handles speedBtn.Click
        If _mediaPlayer.Rate = 1.0 Then
            _mediaPlayer.SetRate(1.25)
            speedBtn.Text = "1.25x"
        ElseIf _mediaPlayer.Rate = 1.25 Then
            _mediaPlayer.SetRate(1.5)
            speedBtn.Text = "1.5x"
        ElseIf _mediaPlayer.Rate = 1.5 Then
            _mediaPlayer.SetRate(1.75)
            speedBtn.Text = "1.75x"
        ElseIf _mediaPlayer.Rate = 1.75 Then
            _mediaPlayer.SetRate(2.0)
            speedBtn.Text = "2x"
        ElseIf _mediaPlayer.Rate = 2.0 Then
            _mediaPlayer.SetRate(0.5)
            speedBtn.Text = "0.5x"
        ElseIf _mediaPlayer.Rate = 0.5 Then
            _mediaPlayer.SetRate(0.75)
            speedBtn.Text = "0.75x"
        Else
            _mediaPlayer.SetRate(1.0)
            speedBtn.Text = "1x"
        End If
    End Sub

    Public Sub volAmt_DoubleClick(sender As Object, e As EventArgs) Handles volAmt.DoubleClick
        Dim lastVol As Integer = _mediaPlayer.Volume
        _mediaPlayer.ToggleMute()
        If Not WasMutedDuringStart Then
            If Not _mediaPlayer.Mute Then
                volAmt.Text = "0"
                volSlider.Value = 0
                My.Settings.defaultVol = 0
            Else
                volAmt.Text = _mediaPlayer.Volume.ToString()
                volSlider.Value = _mediaPlayer.Volume
                My.Settings.defaultVol = _mediaPlayer.Volume
            End If
        Else
            volAmt.Text = "100"
            volSlider.Value = 100
            My.Settings.defaultVol = 100
        End If

    End Sub

    Public Sub VideoView1_DoubleClick(sender As Object, e As EventArgs) Handles VideoView1.DoubleClick
        _mediaPlayer.ToggleFullscreen()
    End Sub

    Dim headerColor As Color = Color.FromArgb(12, 12, 12)
    Public Sub ListView1_DrawColumnHeader(sender As Object, e As DrawListViewColumnHeaderEventArgs)
        Using hBr As Brush = New SolidBrush(headerColor)
            e.Graphics.FillRectangle(hBr, e.Bounds)
            e.DrawText()
        End Using

    End Sub

    Public lastFoundIndex As Integer = -1

    Public originalOrder As New List(Of ListViewItem)

    Public Sub CacheOriginalOrder()
        originalOrder.Clear()
        For Each item As ListViewItem In ListView1.Items
            originalOrder.Add(item)
        Next
    End Sub
    'public Sub SearchListView(query As String)
    '    If String.IsNullOrWhiteSpace(query) Then Exit Sub

    '    query = query.ToLower()

    '    For i = lastFoundIndex + 1 To ListView1.Items.Count - 1
    '        Dim item = ListView1.Items(i)

    '        For Each subItem As ListViewItem.ListViewSubItem In item.SubItems
    '            If subItem.Text.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 Then
    '                ListView1.SelectedItems.Clear()
    '                ListView1.TopItem = item
    '                item.Selected = True
    '                item.Focused = True
    '                item.EnsureVisible()

    '                lastFoundIndex = i
    '                Exit Sub
    '            End If
    '        Next
    '    Next

    'End Sub

    Public Sub searchBox_TextChanged(sender As Object, e As EventArgs) Handles searchBox.TextChanged

        If String.IsNullOrWhiteSpace(searchBox.Text) Then
            RestoreOriginalOrder()
        Else
            MoveSearchMatchesToTop(searchBox.Text)
        End If

    End Sub

    Public Function GetAllFilesMedia(ByVal strPath As String) As List(Of String)

        Dim lst As New List(Of String)

        GetFilesMedia(strPath, lst)

        Return lst
    End Function

    'list view code
    Public Sub MoveSearchMatchesToTop(query As String)
        If String.IsNullOrWhiteSpace(query) Then Exit Sub

        query = query.ToLower()

        Dim matches As New List(Of ListViewItem)
        Dim nonMatches As New List(Of ListViewItem)

        For Each item As ListViewItem In ListView1.Items
            Dim isMatch As Boolean = False

            For Each subItem As ListViewItem.ListViewSubItem In item.SubItems
                If subItem.Text.ToLower().Contains(query) Then
                    isMatch = True
                    Exit For
                End If
            Next

            If isMatch Then
                matches.Add(item)
            Else
                nonMatches.Add(item)
            End If
        Next

        ListView1.BeginUpdate()
        ListView1.Items.Clear()
        ListView1.Items.AddRange(matches.ToArray())
        ListView1.Items.AddRange(nonMatches.ToArray())
        ListView1.EndUpdate()

        If ListView1.Items.Count > 0 Then
            ListView1.TopItem = ListView1.Items(0)
        End If
    End Sub

    Public Sub RestoreOriginalOrder()
        ListView1.BeginUpdate()
        ListView1.Items.Clear()
        ListView1.Items.AddRange(originalOrder.ToArray())
        ListView1.EndUpdate()

        If ListView1.Items.Count > 0 Then
            ListView1.TopItem = ListView1.Items(0)
        End If
    End Sub




    Public Sub GetFilesMedia(ByVal strpath As String, ByRef lstfiles As List(Of String))



        Try

            Dim str As String() = IO.Directory.GetFiles(strpath, "*.*", IO.SearchOption.TopDirectoryOnly)
            'Get Current Directory files
            lstfiles.AddRange(str)

            'Loop  over sub-directories
            For Each strDirectory As String In IO.Directory.GetDirectories(strpath, "*.mp3", IO.SearchOption.TopDirectoryOnly)


                Me.GetFilesMedia(strDirectory, lstfiles)


            Next

        Catch ex As UnauthorizedAccessException
            'Access Denied exception

        Catch ex1 As Exception
            'Other exceptions

        End Try

    End Sub
    Public Sub sb7_Click(sender As Object, e As EventArgs) Handles sb7.Click
        ChooseSBTab(7)
    End Sub

    Sub ChooseCorrectIcon()

        Select Case curTabIndex
            Case 1
                quickActionsPB.Image = Image.FromFile(resDir + "\music.png")
            Case 2
                quickActionsPB.Image = Image.FromFile(resDir + "\videos.png")
            Case 3
                quickActionsPB.Image = Image.FromFile(resDir + "\pictures.png")
            Case 4
                quickActionsPB.Image = Image.FromFile(resDir + "\radio.png")
            Case 6
                quickActionsPB.Image = Image.FromFile(resDir + "\playlists.png")
            Case Else
                quickActionsPB.Image = Image.FromFile(resDir & "\music.png")
        End Select
        'playbackAlbumArt.Image = quickActionsPB.Image
    End Sub
    Function HasExtension(path As String, exts As String()) As Boolean
        Return exts.Any(Function(e) path.EndsWith(e, StringComparison.OrdinalIgnoreCase))
    End Function

    Public Sub ListView1_MouseClick(sender As Object, e As MouseEventArgs) Handles ListView1.MouseClick
        Try
            If ListView1.SelectedItems.Count > 0 Then
                currentFile = ListView1.SelectedItems(0).Tag.ToString()
                If curTabIndex = 0 Or curTabIndex = 1 Or curTabIndex = 9 Then
                    Using t = TagLib.File.Create(ListView1.SelectedItems(0).Tag.ToString())
                        songTextQA.Text = If(String.IsNullOrEmpty(t.Tag.Title),
                            IO.Path.GetFileNameWithoutExtension(ListView1.SelectedItems(0).Tag.ToString()),
                            t.Tag.Title)
                        If t.Tag.Pictures.Length > 0 Then
                            Using ms As New IO.MemoryStream(t.Tag.Pictures(0).Data.Data)
                                quickActionsPB.Image = Image.FromStream(ms)
                            End Using
                            IsDefaultArt = False
                        Else
                            GetIcons()
                            ChooseCorrectIcon()
                            If curTabIndex = 0 Or curTabIndex = 1 Then IsDefaultArt = True
                        End If
                    End Using
                    If HasExtension(currentFile, {".mp4", ".avi", ".mov", ".wmv", ".mpg", ".mpeg", ".mkv", ".webm"}) Then
                        quickActionsPB.Image = GetFileThumbnail(currentFile, 128)
                        playBtn.Enabled = True
                        currentFile = ListView1.SelectedItems(0).Tag.ToString()
                    End If
                    playBtn.Enabled = True
                    currentFile = ListView1.SelectedItems(0).Tag.ToString()
                Else
                    currentFile = ListView1.SelectedItems(0).Tag.ToString()
                    If curTabIndex = 2 Or curTabIndex = 3 Or curTabIndex = 5 Then
                        songTextQA.Text = IO.Path.GetFileNameWithoutExtension(ListView1.SelectedItems(0).Tag.ToString())
                        GetIcons()
                        ChooseCorrectIcon()
                        playBtn.Enabled = True
                        currentFile = ListView1.SelectedItems(0).Tag.ToString()
                    Else
                        songTextQA.Text = ListView1.SelectedItems(0).Text.ToString()
                        GetIcons()
                        ChooseCorrectIcon()
                        playBtn.Enabled = True
                        currentFile = ListView1.SelectedItems(0).Tag.ToString()
                    End If

                    GetIcons()
                    If curTabIndex = 2 Or curTabIndex = 3 Or (curTabIndex = 9 AndAlso HasExtension(currentFile, {".mp4", ".avi", ".mov", ".wmv", ".mpg", ".mpeg", ".mkv", ".webm"})) Then
                        quickActionsPB.Image = GetFileThumbnail(currentFile, 128)
                        playBtn.Enabled = True
                        currentFile = ListView1.SelectedItems(0).Tag.ToString()
                    End If
                    currentFile = ListView1.SelectedItems(0).Tag.ToString()
                    If curTabIndex = 6 Then
                        Dim chosenPl = SpectrumPlaylist.Load(currentFile)
                        cpl2 = chosenPl.PLPath
                        quickActionsPB.Image = Image.FromFile(chosenPl.ImagePath)
                    End If

                    If curTabIndex = 9 Then
                        If HasExtension(currentFile, {".mp3", ".flac", ".wav", ".m4a", ".ogg", ".aac"}) Then
                            Using t = TagLib.File.Create(ListView1.SelectedItems(0).Tag.ToString())
                                songTextQA.Text = If(String.IsNullOrEmpty(t.Tag.Title),
                                    IO.Path.GetFileNameWithoutExtension(ListView1.SelectedItems(0).Tag.ToString()),
                                    t.Tag.Title)
                                If t.Tag.Pictures.Length > 0 Then
                                    Using ms As New IO.MemoryStream(t.Tag.Pictures(0).Data.Data)
                                        quickActionsPB.Image = Image.FromStream(ms)
                                    End Using
                                    IsDefaultArt = False
                                Else
                                    GetIcons()
                                    ChooseCorrectIcon()
                                    If curTabIndex = 9 Then IsDefaultArt = True
                                End If
                            End Using
                        Else

                        End If
                    End If
                End If
                currentFile = ListView1.SelectedItems(0).Tag.ToString()
            End If
        Catch ex As Exception

        End Try

        If e.Button = MouseButtons.Right Then

            If curTabIndex = 4 Or ListView1.SelectedItems.Count <= 0 Then
                AddToToolStripMenuItem.Enabled = False
            Else
                AddToToolStripMenuItem.Enabled = True
            End If

            If curTabIndex = 6 Then
                CreatePlaylistToolStripMenuItem.Enabled = True
                EditPlaylistToolStripMenuItem.Enabled = True
            Else
                CreatePlaylistToolStripMenuItem.Enabled = False
                EditPlaylistToolStripMenuItem.Enabled = False
            End If
        End If
    End Sub

    Public Sub ShowAlbumArt()
        If lastTab = 0 OrElse lastTab = 1 Or (labelSong.Text = "Not Playing" And (songTextQA.Text <> "-" Or songTextQA.Text = "")) Then
            If IsDefaultArt Then
                MsgBox("No album art available.", MsgBoxStyle.Critical, "Error")
            Else
                AlbumArt.Text = "Album Art"
                AlbumArt.PictureBox1.Image = quickActionsPB.Image
                AlbumArt.ShowDialog()
            End If
        End If

    End Sub

    Dim showPLC As Boolean = False
    Dim plcPath As String = ""
    Public Sub ListView1_DoubleClick(sender As Object, e As EventArgs) Handles ListView1.DoubleClick
        If ListView1.SelectedItems.Count = 1 Then
            Try
                currentFile = ListView1.SelectedItems(0).Tag.ToString()
                If curTabIndex <> 6 And curTabIndex <> 4 Then
                    currentFile = ListView1.SelectedItems(0).Tag.ToString()
                    curSongIndex2 = CInt(ListView1.SelectedIndices(0))
                    _mediaPlayer.Stop()
                    PFCodeMain(currentFile)
                    playbackAlbumArt.Image = quickActionsPB.Image
                ElseIf curTabIndex = 4 Then
                    Dim radio As RadioStation = CType(ListView1.SelectedItems(0).Tag, RadioStation)
                    currentFile = radio.stream_url
                    _mediaPlayer.Stop()
                    PFCodeMain(currentFile)
                    playbackAlbumArt.Image = quickActionsPB.Image
                Else
                    GetIcons()
                    ChooseCorrectIcon()
                    playbackAlbumArt.Image = quickActionsPB.Image
                    plcPath = currentFile
                    GetPlaylistFiles()
                End If

                'If curTabIndex = 9 Then GetPlaylistFiles()
            Catch ex As Exception
                'MsgBox("Error: " & ex.Message, MsgBoxStyle.Critical, "Error")
            End Try

        End If
    End Sub

    Dim lastTab As Short = 0
    Public Sub playBtn_Click(sender As Object, e As EventArgs) Handles playBtn.Click
        If playBtn.Text = "▶" Then
            If _mediaPlayer.Time = _mediaPlayer.Length Or CInt(_mediaPlayer.Time / 1000) = CInt(_mediaPlayer.Length / 1000) - 1 Or startPos.Text = totalPos.Text Then

                _mediaPlayer.Time = 0
                _mediaPlayer.Stop()
                'currentFile = ListView1.SelectedItems(0).Tag.ToString()
                PlayFile(currentFile)
            Else
                Try
                    PlayFile(currentFile, playSlider.Value)
                Catch ex As Exception
                    PlayFile(currentFile, curTimePos)
                End Try

            End If
            playBtn.Text = "❚❚"
        Else
            _mediaPlayer.Pause()
            playBtn.Text = "▶"
        End If
    End Sub

    Public Sub stopBtn_Click(sender As Object, e As EventArgs) Handles stopBtn.Click
        Try
            _mediaPlayer.Stop()
            curTabIndex = lastTab
            If curTabIndex <> 8 OrElse lastTab <> 8 Then ChooseSBTab(curTabIndex) Else ChooseSBTab(8)
            stopBtn.Enabled = False
            playBtn.Text = "▶"
            playBtn.Enabled = False
            repeatBtn.Enabled = True
            labelSong.Text = "Not Playing"
            labelSong.ForeColor = Color.Gray
            labelArtist.Text = ""
            currentFile = ""
            startPos.Text = "00:00"
            totalPos.Text = "00:00"
            playSlider.Value = 0
            playSlider.MaxValue = 0
            playSlider.ChangeSmall = 0
            playSlider.ChangeLarge = 0
            playSlider.Enabled = False
            playbackTimer.Stop()
            songTextQA.Text = "-"
            hasBeenSwitched = 0
            ChooseSBTab(lastTab)
            Try
                nowPlayingCycle.Stop()

            Catch ex As Exception
                'empty for now
            End Try

            'MsgBox(lastTab & ", " & curTabIndex)

            ChooseCorrectIcon()
        Catch ex As Exception
            MsgBox("Unable to stop." & vbCrLf & "Reason: " & ex.Message)
        End Try
    End Sub

    Sub loopcurrentFile()
        If currentFile = "" Then Exit Sub
        _mediaPlayer.Stop()
        _mediaPlayer.Time = 0
        PlayFile(currentFile)
        If Not songTextQA.Text = "-" OrElse Not songTextQA.Text = "" Then labelSong.Text = songTextQA.Text
    End Sub
    Public Sub OnMediaEnded(sender As Object, e As EventArgs)
        If InvokeRequired Then BeginInvoke(Sub()
                                               If Not isShuffle And HowLooping = "none" Then
                                                   playBtn.Text = "▶"
                                                   curTimePos = _mediaPlayer.Length
                                                   playbackTimer.Stop()
                                                   startPos.Text = totalPos.Text
                                                   nowPlayingCycle.Stop()
                                               Else
                                                   If HowLooping = "none" Then
                                                       playingNextSeq = False


                                                   ElseIf HowLooping = "all" Then
                                                       playingNextSeq = False
                                                       If Me.InvokeRequired Then
                                                           Me.BeginInvoke(New Action(AddressOf PlayNextSequential))
                                                       Else
                                                           PlayNextSequential()
                                                       End If

                                                   Else
                                                       HowLooping = "one"
                                                       playingNextSeq = False
                                                       If Me.InvokeRequired Then
                                                           Me.BeginInvoke(New Action(AddressOf loopcurrentFile))
                                                       Else
                                                           loopcurrentFile()
                                                       End If
                                                       'Exit Sub
                                                   End If

                                                   If isShuffle = True Then
                                                       'Debug.WriteLine("shiffle is supposed to work if this is printed")
                                                       If Me.InvokeRequired Then
                                                           Me.BeginInvoke(New Action(AddressOf PlayNext))
                                                       Else
                                                           PlayNext()
                                                       End If
                                                   End If
                                               End If




                                           End Sub)




    End Sub

    Public Sub shuffleBtn_Click(sender As Object, e As EventArgs) Handles shuffleBtn.Click
        isShuffle = Not isShuffle
        My.Settings.wantsToShuffle = isShuffle

        If isShuffle Then
            shuffleBtn.BackColor = Color.FromArgb(56, 56, 56)
        Else
            shuffleBtn.BackColor = Color.FromArgb(64, 64, 64)
        End If

        'BuildPlayOrder()
        My.Settings.Save()
    End Sub

    Dim curSongIndex2 As Long = 0
    Sub PlayNextSequential()
        If ListView1.Items.Count = 0 Then Exit Sub

        playingNextSeq = True

        If curTabIndex <> 8 Then ChooseSBTab(curTabIndex)

        Dim currentIndex As Integer = -1

        Dim itemsList As List(Of ListViewItem) = ListView1.Items.Cast(Of ListViewItem)().ToList()

        Dim index As Integer = itemsList.FindIndex(Function(item) item.Tag IsNot Nothing AndAlso item.Tag.Equals(currentFile))

        If index <> -1 Then
            currentIndex = index
        Else
            currentIndex = ListView1.FindItemWithText(currentFile).Index
            If ListView1.SelectedItems.Count > 0 And curSongIndex2 = ListView1.SelectedItems(0).Index Then
                currentIndex = ListView1.SelectedItems(0).Index
            Else
                currentIndex = 0
            End If
        End If



        Dim nextIndex As Integer = currentIndex + 1

        ' If end reached → loop back to start (repeat all)
        If nextIndex >= ListView1.Items.Count Then
            nextIndex = 0
        End If

        ListView1.SelectedItems.Clear()
        ListView1.Items(nextIndex).Selected = True
        ListView1.Items(nextIndex).Focused = True
        ListView1.EnsureVisible(nextIndex)


        'If ListView1.Items.IndexOf(ListView1.SelectedItems(0)) = curSongIndex Then
        '    _mediaPlayer.Stop()
        '    _mediaPlayer.Time = 0
        'End If
        currentFile = ListView1.Items(nextIndex).Tag.ToString()



        PlayFile(currentFile)

    End Sub



    Sub OnStartingShuffle(sender As Object, e As EventArgs)
        If isShuffle = True Then
            If Me.InvokeRequired Then
                Me.BeginInvoke(New Action(AddressOf PlayNext))
            Else
                PlayNext()
            End If
        End If
    End Sub

    Public Sub repeatBtn_Click(sender As Object, e As EventArgs) Handles repeatBtn.Click
        If HowLooping = "none" Then
            HowLooping = "all"
            repeatBtn.BackColor = Color.FromArgb(56, 56, 56)
        ElseIf HowLooping = "all" Then
            HowLooping = "one"
            repeatBtn.Text = "🔂"
            repeatBtn.BackColor = Color.FromArgb(52, 52, 52)
        Else
            HowLooping = "none"
            repeatBtn.Text = "🔁"
            repeatBtn.BackColor = Color.FromArgb(64, 64, 64)
        End If

        My.Settings.repeatMethod = HowLooping
    End Sub

    Dim curTimePos As Long = 0
    Public Sub playSlider_ValueChanged(sender As Object, e As EventArgs) Handles playSlider.ValueChanged
        curTimePos = playSlider.Value
    End Sub

    Public curTabIndex As Short = 0
    Sub ChooseSBTab(index As Integer)
        sb1.BackColor = Color.FromArgb(16, 16, 16)
        sb2.BackColor = Color.FromArgb(16, 16, 16)
        'sb3.BackColor = Color.FromArgb(16, 16, 16)
        sb4.BackColor = Color.FromArgb(16, 16, 16)
        sb5.BackColor = Color.FromArgb(16, 16, 16)
        sb6.BackColor = Color.FromArgb(16, 16, 16)
        sb7.BackColor = Color.FromArgb(16, 16, 16)
        sbb1.BackColor = Color.FromArgb(16, 16, 16)

        'ListView1.AllowDrop = False


        Select Case index
            Case 1
                curTabIndex = index
                sb1.BackColor = Color.FromArgb(6, 6, 6)
                ListView1.Items.Clear()
                ListView1.Columns.Clear()
                PopulateMusicUI()
                ShowListView()
                CacheOriginalOrder()


            Case 2
                curTabIndex = index
                sb2.BackColor = Color.FromArgb(6, 6, 6)
                ListView1.Items.Clear()
                ListView1.Columns.Clear()
                PopulateVideoUI()
                ShowListView()
                CacheOriginalOrder()

            Case 3
                curTabIndex = index
                'sb3.BackColor = Color.FromArgb(6, 6, 6)
                ListView1.Items.Clear()
                ListView1.Columns.Clear()
                'PopulateImageUI()
                ShowListView()
                CacheOriginalOrder()

            Case 4
                curTabIndex = index
                sb4.BackColor = Color.FromArgb(6, 6, 6)
                ListView1.Items.Clear()
                ListView1.Columns.Clear()
                If InvokeRequired Then
                    BeginInvoke(Sub()
                                    RefreshRadios()
                                    PopulateRadioUI()
                                End Sub)
                Else
                    RefreshRadios()
                    PopulateRadioUI()
                End If

                ShowListView()
                CacheOriginalOrder()


            Case 5
                curTabIndex = index
                sb5.BackColor = Color.FromArgb(6, 6, 6)
                ListView1.Items.Clear()
                ListView1.Columns.Clear()
                ShowListView()
                MsgBox("Coming soon!", MsgBoxStyle.Information, "Info")

            Case 6
                curTabIndex = index
                sb6.BackColor = Color.FromArgb(6, 6, 6)
                ListView1.Items.Clear()
                ListView1.Columns.Clear()
                If InvokeRequired Then
                    BeginInvoke(Sub()
                                    RefreshPL()
                                    'PopulatePLUI()
                                End Sub)
                Else
                    RefreshPL()
                    'PopulatePLUI()
                End If

                ShowListView()
                CacheOriginalOrder()

            Case 7
                curTabIndex = index
                sb7.BackColor = Color.FromArgb(6, 6, 6)
                ListView1.Items.Clear()
                ListView1.Columns.Clear()
                ShowListView()
                CacheOriginalOrder()
                MsgBox("Coming soon! Meanwhile check out the guide from the GitHub repository!", MsgBoxStyle.Information, "Info")

            Case 8
                curTabIndex = index
                sbb1.BackColor = Color.FromArgb(6, 6, 6)
                ShowPlayer()

        End Select
        If songTextQA.Text = "-" Or songTextQA.Text = "" Then GetIcons()
    End Sub

    Sub PlayDisc()
        Throw New NotImplementedException("Disc playback coming soon!!")
    End Sub
    Public Sub sbb1_Click(sender As Object, e As EventArgs) Handles sbb1.Click
        ChooseSBTab(8)
    End Sub

    Public Sub sb2_Click(sender As Object, e As EventArgs) Handles sb2.Click
        ChooseSBTab(2)
    End Sub

    Public Sub sb3_Click(sender As Object, e As EventArgs) 'Handles sb3.click
        ChooseSBTab(3)
    End Sub

    Public Sub sb4_Click(sender As Object, e As EventArgs) Handles sb4.Click
        ChooseSBTab(4)
    End Sub

    Public Sub sb5_Click(sender As Object, e As EventArgs) Handles sb5.Click
        ChooseSBTab(5)
    End Sub

    Dim curPLIndex = 0

    Public Sub sb6_Click(sender As Object, e As EventArgs) Handles sb6.Click
        ChooseSBTab(6)
    End Sub



    Sub ShowListView()
        ListView1.Visible = True
        ListView1.Dock = DockStyle.Fill
        videoViewPane.Visible = False
        videoViewPane.Dock = DockStyle.Bottom
    End Sub
    Public Sub sb1_Click(sender As Object, e As EventArgs) Handles sb1.Click
        ChooseSBTab(1)
    End Sub

    Public Sub ScanAllMedia(token As CancellationToken)
        ScanMusic(token)
        'ScanVideos(token)
        'ScanPictures(token)
    End Sub

    Enum PLParseState
        None
        Header
        Meta
        Entries
    End Enum
    Function ReadSongMetadata(path As String) As SongInfo
        Dim info As New SongInfo()
        info.FilePath = path

        Try
            Using t = TagLib.File.Create(path)
                info.Title = If(String.IsNullOrEmpty(t.Tag.Title),
                            IO.Path.GetFileNameWithoutExtension(path),
                            t.Tag.Title)

                info.Artist = String.Join(", ", t.Tag.Performers)
                info.Album = t.Tag.Album
                info.Year = If(t.Tag.Year > 0, t.Tag.Year.ToString(), "")
                info.Genre = t.Tag.JoinedGenres
                info.Duration = t.Properties.Duration


            End Using
            'info.AlbumArtBytes = Nothing

        Catch
            ' ignore broken files
        End Try

        Return info
    End Function

    Public Shared Function GetVideoDuration(filePath As String) As TimeSpan
        Using shell = ShellObject.FromParsingName(filePath)
            Dim prop As IShellProperty = shell.Properties.System.Media.Duration
            Dim t = CULng(prop.ValueAsObject)
            Return TimeSpan.FromTicks(t)
        End Using
    End Function

    Public Function GetVideoResolution(path As String) As String
        Using file As ShellFile = ShellFile.FromFilePath(path)
            Dim width = file.Properties.System.Video.FrameWidth.Value
            Dim height = file.Properties.System.Video.FrameHeight.Value


            If width.HasValue AndAlso height.HasValue Then
                Return width.ToString() & "x" & height.ToString()
            End If
        End Using

        Return "Unknown"
    End Function

    Public Function GetImageResolution(path As String) As String
        Using file As ShellFile = ShellFile.FromFilePath(path)
            Dim width As Integer? = file.Properties.System.Image.HorizontalSize.Value
            Dim height As Integer? = file.Properties.System.Image.VerticalSize.Value


            If width.HasValue AndAlso height.HasValue Then
                Return width.ToString & "x" & height.ToString
            End If
            Return "Unknown"
        End Using


    End Function

    Public Function GetFileThumbnail(path As String, maxsize As Integer) As Image
        Using shellFile As ShellFile = ShellFile.FromFilePath(path)
            Dim bmp As Bitmap = CType(shellFile.Thumbnail.LargeBitmap, Bitmap)
            If bmp IsNot Nothing Then
                If maxsize > 0 AndAlso (bmp.Width > maxsize OrElse bmp.Height > maxsize) Then
                    Dim scale As Double = Math.Min(maxsize / bmp.Width, maxsize / bmp.Height)
                    Dim newWidth As Integer = CInt(bmp.Width * scale)
                    Dim newHeight As Integer = CInt(bmp.Height * scale)
                    Return New Bitmap(bmp, New Size(newWidth, newHeight))
                Else
                    Return New Bitmap(bmp)
                End If
            End If
        End Using
        Return Nothing
    End Function

    Function ReadVidMetadata(path As String) As VideoInfo
        Dim info As New VideoInfo()
        info.FilePath = path

        Try
            info.Title = IO.Path.GetFileNameWithoutExtension(path)

            info.Duration = GetVideoDuration(path).ToString("hh\:mm\:ss")
            info.Resolution = GetVideoResolution(path).ToString()
            info.DateTaken = IO.File.GetCreationTime(path).ToString("yyyy-MM-dd")

            'End Using

        Catch
            ' ignore broken files
        End Try

        Return info
    End Function

    Public Sub ScanMusic(token As CancellationToken)

        Dim addedCount As Integer = 0

        lastUiIndex = 0
        MusicCache.Clear()

        ListView1.Items.Clear()

        Using reader As StreamReader =
        My.Computer.FileSystem.OpenTextFileReader(
            Application.StartupPath & "\settings\musicDir.spectrum")

            While Not reader.EndOfStream
                Dim line As String = reader.ReadLine()

                If token.IsCancellationRequested Then Exit Sub
                If String.IsNullOrWhiteSpace(line) Then Continue While
                If Not IO.Directory.Exists(line) Then Continue While

                For Each file In IO.Directory.EnumerateFiles(
                line, "*.*", SearchOption.AllDirectories)

                    If token.IsCancellationRequested Then Exit Sub

                    Dim ext = IO.Path.GetExtension(file).ToLower()
                    If Not {".mp3", ".flac", ".wav", ".m4a", ".ogg", ".aac"}.Contains(ext) Then
                        Continue For
                    End If

                    ' Read metadata + cache
                    Dim song = ReadSongMetadata(file)
                    If song Is Nothing Then Continue For

                    SyncLock MusicCache
                        MusicCache.Add(song)
                    End SyncLock

                    addedCount += 1

                    ' Batch UI update


                Next
            End While
        End Using


        ''debug.writeline("Songs scanned: " & MusicCache.Count)
        'BatchUpdateUI()


    End Sub

    Public Sub ScanVideo(token As CancellationToken)

        Dim addedCount As Integer = 0

        lastUiIndex = 0
        MusicCache.Clear()

        ListView1.Items.Clear()

        Using reader As StreamReader =
        My.Computer.FileSystem.OpenTextFileReader(
            Application.StartupPath & "\settings\videoDir.spectrum")

            While Not reader.EndOfStream
                Dim line As String = reader.ReadLine()

                If token.IsCancellationRequested Then Exit Sub
                If String.IsNullOrWhiteSpace(line) Then Continue While
                If Not IO.Directory.Exists(line) Then Continue While

                For Each file In IO.Directory.EnumerateFiles(
                line, "*.*", SearchOption.AllDirectories)

                    If token.IsCancellationRequested Then Exit Sub

                    Dim ext = IO.Path.GetExtension(file).ToLower()
                    If Not {".mp4", ".avi", ".mov", ".wmv", ".mpg", ".mpeg", ".mkv", ".webm"}.Contains(ext) Then
                        Continue For
                    End If

                    ' Read metadata + cache
                    Dim vid = ReadVidMetadata(file)
                    If vid Is Nothing Then Continue For

                    SyncLock VideoCache
                        VideoCache.Add(vid)
                    End SyncLock

                    addedCount += 1

                    ' Batch UI update


                Next
            End While
        End Using


        ''debug.writeline("Songs scanned: " & MusicCache.Count)
        'BatchUpdateUI()


    End Sub

    'THIS SUB IS SCRAPPED SO DO NOT USE
    'public Sub ScanImage(token As CancellationToken)

    '    Dim addedCount As Integer = 0

    '    lastUiIndex = 0
    '    ImageCache.Clear()

    '    ListView1.Items.Clear()

    '    Using reader As StreamReader =
    '    My.Computer.FileSystem.OpenTextFileReader(
    '        Application.StartupPath & "\settings\imageDir.spectrum")

    '        While Not reader.EndOfStream
    '            Dim line As String = reader.ReadLine()

    '            If token.IsCancellationRequested Then Exit Sub
    '            If String.IsNullOrWhiteSpace(line) Then Continue While
    '            If Not IO.Directory.Exists(line) Then Continue While

    '            For Each file In IO.Directory.EnumerateFiles(
    '            line, "*.*", SearchOption.AllDirectories)

    '                If token.IsCancellationRequested Then Exit Sub

    '                Dim ext = IO.Path.GetExtension(file).ToLower()
    '                If Not {".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff", ".avif"}.Contains(ext) Then
    '                    Continue For
    '                End If

    '                ' Read metadata + cache
    '                Dim img = ReadImgMetadata(file)
    '                If img Is Nothing Then Continue For

    '                SyncLock ImageCache
    '                    ImageCache.Add(img)
    '                End SyncLock

    '                addedCount += 1

    '                ' Batch UI update


    '            Next
    '        End While
    '    End Using


    '    ''debug.writeline("Songs scanned: " & MusicCache.Count)
    '    'BatchUpdateUI()


    'End Sub

    Sub ChooseTabAndPopulateUI(i As Integer)
        Select Case i
            Case 1
                ChooseSBTab(1)
            Case 2
                ChooseSBTab(2)
            Case 3
                ChooseSBTab(3)
            Case 4
                ChooseSBTab(4)
            Case 5
                ChooseSBTab(5)
            Case 6
                ChooseSBTab(6)
            Case 7
                ChooseSBTab(7)
            Case 8
                ChooseSBTab(8)
        End Select
    End Sub

    'public Sub BatchUpdateUI()
    '    If Me.IsDisposed Then Exit Sub

    '    If Me.InvokeRequired Then
    '        Me.BeginInvoke(Sub() BatchUpdateUI())
    '        Return
    '    End If

    '    ListView1.BeginUpdate()

    '    SyncLock MusicCache
    '        For i = lastUiIndex To MusicCache.Count - 1
    '            ListView1.Items.Add(MusicCache(i).ToListViewItem())
    '        Next
    '        lastUiIndex = MusicCache.Count
    '    End SyncLock

    '    ListView1.EndUpdate()
    '    'debug.writeline("UI adding from index " & lastUiIndex & " to " & (MusicCache.Count - 1))

    'End Sub

    'public Sub BatchUpdateUI()
    '    If Me.IsDisposed Then Exit Sub

    '    If Me.InvokeRequired Then
    '        Me.BeginInvoke(Sub() BatchUpdateUI())
    '        Return
    '    End If

    '    ListView1.BeginUpdate()

    '    SyncLock MusicCache
    '        For i = lastUiIndex To MusicCache.Count - 1
    '            Dim song = MusicCache(i)
    '            Dim imgIndex = AlbumArtManager.GetAlbumArtIndex(song)
    '            ListView1.Items.Add(song.ToListViewItem(imgIndex))
    '        Next

    '        lastUiIndex = MusicCache.Count
    '    End SyncLock

    '    ListView1.EndUpdate()
    'End Sub



    Public Sub PopulateMusicUI()
        If Me.IsDisposed Then Exit Sub

        If Me.InvokeRequired Then
            Me.BeginInvoke(Sub() PopulateMusicUI())
            Return
        End If

        If curTabIndex = 0 Or curTabIndex = 1 Then
            ListView1.Columns.Clear()
            ListView1.Columns.Add("Title", 290, HorizontalAlignment.Left)
            ListView1.Columns.Add("Length", 140, HorizontalAlignment.Left)
            ListView1.Columns.Add("Album", 220, HorizontalAlignment.Left)
            ListView1.Columns.Add("Artist(s)", 195, HorizontalAlignment.Left)
            ListView1.Columns.Add("Year", 80, HorizontalAlignment.Left)
            ListView1.Columns.Add("Genre(s)", 165, HorizontalAlignment.Left)

            ListView1.BeginUpdate()
            ListView1.Items.Clear()

            SyncLock MusicCache
                For Each song In MusicCache
                    ListView1.Items.Add(song.ToListViewItem())

                Next
            End SyncLock

            ListView1.EndUpdate()
            'RemoveDuplicatesAndSort(ListView1)


            ' reset batch pointer
            lastUiIndex = ListView1.Items.Count

            BuildPlayOrder()
        End If


    End Sub

    Public Sub PopulatePLUI()
        If Me.IsDisposed Then Exit Sub

        If Me.InvokeRequired Then
            Me.BeginInvoke(Sub() PopulatePLUI())
            Return
        End If
        ListView1.AllowDrop = False
        If curTabIndex = 6 Then
            ListView1.Columns.Clear()
            ListView1.Columns.Add("Title", 700, HorizontalAlignment.Left)
            ListView1.Columns.Add("Extension", 150, HorizontalAlignment.Left)
            ListView1.Columns.Add("Length", 150, HorizontalAlignment.Left)

            ListView1.BeginUpdate()
            ListView1.Items.Clear()

            SyncLock PLCache
                For Each pl In PLCache
                    ListView1.Items.Add(pl.ToListViewItem())
                Next
            End SyncLock

            ListView1.EndUpdate()
            'RemoveDuplicatesAndSort(ListView1)
            CacheOriginalOrder()

            ' reset batch pointer
            lastUiIndex = ListView1.Items.Count
        End If
    End Sub
    Public Sub PopulateVideoUI()
        If Me.IsDisposed Then Exit Sub

        If Me.InvokeRequired Then
            Me.BeginInvoke(Sub() PopulateVideoUI())
            Return
        End If
        If curTabIndex = 2 Then
            ListView1.Columns.Clear()
            ListView1.Columns.Add("Title", 290, HorizontalAlignment.Left)
            ListView1.Columns.Add("Length", 140, HorizontalAlignment.Left)
            ListView1.Columns.Add("Date (last created/modified)", 240, HorizontalAlignment.Left)
            ListView1.Columns.Add("Resolution", 210, HorizontalAlignment.Left)

            ListView1.BeginUpdate()
            ListView1.Items.Clear()

            SyncLock VideoCache
                For Each vid In VideoCache
                    ListView1.Items.Add(vid.ToListViewItem())

                Next
            End SyncLock

            ListView1.EndUpdate()
            'RemoveDuplicatesAndSort(ListView1)


            ' reset batch pointer
            lastUiIndex = ListView1.Items.Count

            BuildPlayOrder()
        End If
    End Sub

    'public Sub PopulateImageUI()
    '    If Me.IsDisposed Then Exit Sub

    '    If Me.InvokeRequired Then
    '        Me.BeginInvoke(Sub() PopulateImageUI())
    '        Return
    '    End If

    '    If curTabIndex = 3 Then
    '        ListView1.Columns.Clear()
    '        ListView1.Columns.Add("Title", 290, HorizontalAlignment.Left)
    '        ListView1.Columns.Add("Date (last created/modified)", 240, HorizontalAlignment.Left)
    '        ListView1.Columns.Add("Resolution", 210, HorizontalAlignment.Left)

    '        ListView1.BeginUpdate()
    '        ListView1.Items.Clear()

    '        SyncLock ImageCache
    '            For Each img In ImageCache
    '                ListView1.Items.Add(img.ToListViewItem())

    '            Next
    '        End SyncLock

    '        ListView1.EndUpdate()
    '        'RemoveDuplicatesAndSort(ListView1)


    '        ' reset batch pointer
    '        lastUiIndex = ListView1.Items.Count
    '    End If
    'End Sub

    Public Shared Sub RemoveDuplicatesAndSort(lv As ListView)
        Dim distictItems As ListViewItem() = lv.Items.Cast(Of ListViewItem)().Distinct(New LVItemComparer()).ToArray
        lv.BeginUpdate() ' suppress screen updates
        lv.Items.Clear()
        lv.Items.AddRange(distictItems)
        lv.EndUpdate()
    End Sub

    Public Class LVItemComparer : Implements IEqualityComparer(Of ListViewItem)
        Public Function Equals1(x As ListViewItem, y As ListViewItem) As Boolean Implements IEqualityComparer(Of ListViewItem).Equals
            Return x.Text.Equals(y.Text)
        End Function

        Public Function GetHashCode1(obj As ListViewItem) As Integer Implements IEqualityComparer(Of ListViewItem).GetHashCode
            Return obj.Text.GetHashCode
        End Function
    End Class


    Public Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown

        Task.Run(Sub() StartMusicScan())
        Task.Run(Sub() StartVideoScan())
        Task.Run(Sub() RefreshRadios())
        Task.Run(Sub() StartPLScan())
    End Sub

    Public Sub playSlider_MouseUp(sender As Object, e As MouseEventArgs) Handles playSlider.MouseUp
        isUserScrubbing = False
        UpdatePlaySlider(sender, e.X)
    End Sub

    Public Sub quickActionsPB_MouseClick(sender As Object, e As MouseEventArgs) Handles quickActionsPB.MouseClick
        If e.Button = MouseButtons.Right Then
            ContextMenu1.Show(quickActionsPB, New Point(e.X, e.Y))
        Else
            ShowAlbumArt()
        End If
    End Sub

    Public Sub playbackAlbumArt_MouseClick(sender As Object, e As MouseEventArgs) Handles playbackAlbumArt.MouseClick
        If e.Button = MouseButtons.Right Then
            ContextMenu1.Show(playbackAlbumArt, New Point(e.X, e.Y))
        Else
            ShowAlbumArt()
        End If
    End Sub

    Public Sub LinkLabel26_Click(sender As Object, e As EventArgs) Handles LinkLabel26.Click
        searchBox.Text = Nothing
    End Sub

    Public Sub Form1_Resize(sender As Object, e As EventArgs) Handles Me.Resize

    End Sub

    Dim fieldI As Short = 0
    Public Sub nowPlayingCycle_Tick(sender As Object, e As EventArgs) Handles nowPlayingCycle.Tick
        fieldI += 1
        If fieldI > 4 Then fieldI = 0
        Try
            If Not cursonginfos2(fieldI) = "By " Then
                nowPlayingText.Text = cursonginfos2(fieldI)
            Else
                Exit Try
            End If
        Catch ex As IndexOutOfRangeException
            nowPlayingText.Text = cursonginfos2(0)
        Catch ex As Exception

        End Try




    End Sub

    Public scanCts As CancellationTokenSource
    Public Sub RefreshMusic()
        scanCts?.Cancel()
        scanCts = New CancellationTokenSource()

        ListView1.Items.Clear()
        lastUiIndex = 0

        Task.Run(Sub() StartMusicScan())
    End Sub

    Public Sub RefreshPL()
        scanCts?.Cancel()
        scanCts = New CancellationTokenSource()

        ListView1.Items.Clear()
        lastUiIndex = 0

        Task.Run(Sub() StartPLScan())
    End Sub

    Public Sub RefreshVideo()
        scanCts?.Cancel()
        scanCts = New CancellationTokenSource()

        ListView1.Items.Clear()
        lastUiIndex = 0

        Task.Run(Sub() StartVideoScan())
    End Sub

    Public Sub LinkLabel27_Click(sender As Object, e As EventArgs) Handles LinkLabel27.Click
        If curTabIndex = 0 Or curTabIndex = 1 Then
            If Me.InvokeRequired Then
                Task.Run(Sub() RefreshMusic())
            Else
                RefreshMusic()
            End If

        ElseIf curTabIndex = 2 Then
            If Me.InvokeRequired Then
                Task.Run(Sub() RefreshVideo())
            Else
                RefreshVideo()
            End If

        ElseIf curTabIndex = 4 Then
            If Me.InvokeRequired Then
                Task.Run(Sub() RefreshRadios())
            Else
                RefreshRadios()
            End If
        ElseIf curTabIndex = 6 Then
            If Me.InvokeRequired Then
                Task.Run(Sub() RefreshPL())
            Else
                RefreshPL()
            End If
        Else
            MsgBox("There is nothing to refresh!", MsgBoxStyle.Critical, "Error")
        End If
    End Sub

    Public Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click
        About.ShowDialog()
    End Sub

    Public Sub SettingsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SettingsToolStripMenuItem.Click
        Settings.ShowDialog()
    End Sub

    Public webViewInitTask As Task = Nothing

    Public Async Function EnsureVisualizerReady() As Task
        If visualizer.CoreWebView2 IsNot Nothing Then Return

        If webViewInitTask Is Nothing Then
            webViewInitTask = visualizer.EnsureCoreWebView2Async()
        End If

        Await webViewInitTask
    End Function

    Public Async Function ShowVisualizerAsync() As Task
        Await EnsureVisualizerReady()
        VideoView1.Visible = False
        visualizer.Visible = True
        visualizer.BringToFront()
        nowPlayingText.Visible = False
    End Function

    Public Sub HideVisualizer()
        visualizer.Visible = False
        nowPlayingText.Visible = True
    End Sub
    'public Async Sub ShowVisualizer()

    '    If visualizer.Parent Is Nothing Then Exit Sub

    '    Await EnsureVisualizerReady()

    '    nowPlayingInfo.Visible = False

    '    visualizer.Dock = DockStyle.Fill
    '    visualizer.Visible = True
    '    visualizer.BringToFront()
    '    visualizer.Focus()

    '    My.Settings.showVizOnStart = True
    'End Sub

    'public Sub HideVisualizer()
    '    visualizer.Visible = False
    '    nowPlayingInfo.Visible = True
    '    My.Settings.showVizOnStart = False
    'End Sub


    'public Async Sub EnableVisualizationsToolStripMenuItem_CheckedChanged(sender As Object, e As EventArgs) Handles EnableVisualizationsToolStripMenuItem.CheckedChanged
    '    If EnableVisualizationsToolStripMenuItem.Checked Then
    '        ShowVisualizer()
    '        Debug.WriteLine("Viz visible: " & visualizer.Visible)

    '    Else
    '        HideVisualizer()
    '        Debug.WriteLine("Viz visible: " & visualizer.Visible)

    '    End If
    'End Sub

    Public isChangingVizState As Boolean = False

    Public Async Sub EnableVisualizationsToolStripMenuItem_CheckedChanged(
    sender As Object, e As EventArgs
) Handles EnableVisualizationsToolStripMenuItem.CheckedChanged

        If isChangingVizState Then Exit Sub
        isChangingVizState = True

        Try
            If EnableVisualizationsToolStripMenuItem.Checked Then
                Await ShowVisualizerAsync()
                My.Settings.showVizOnStart = True
            Else
                HideVisualizer()
                My.Settings.showVizOnStart = False
            End If
        Finally
            isChangingVizState = False
        End Try
    End Sub

    Public Sub EnableVisualizationsToolStripMenuItem_CheckStateChanged(sender As Object, e As EventArgs) Handles EnableVisualizationsToolStripMenuItem.CheckStateChanged

    End Sub

    Public Sub visualizer_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles visualizer.CoreWebView2InitializationCompleted
        'visualizer.CoreWebView2.Settings.AreDevToolsEnabled = False
        ''visualizer.CoreWebView2.Settings.AreDefaultContextMenusEnabled = False
        'visualizer.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = False
        'visualizer.CoreWebView2.Settings.IsBuiltInErrorPageEnabled = False
        'visualizer.CoreWebView2.Settings.IsZoomControlEnabled = False
        'visualizer.CoreWebView2.Settings.IsPinchZoomEnabled = False
        'visualizer.CoreWebView2.Settings.IsGeneralAutofillEnabled = False
        visualizer.CoreWebView2.Profile.ClearBrowsingDataAsync()

        If Not String.IsNullOrEmpty(My.Settings.defaultViz.ToString()) Then
            If My.Settings.defaultViz = "(none)" Then
                visualizer.CoreWebView2.Navigate(Application.StartupPath & "\visualizations\default.html")
            Else

                visualizer.CoreWebView2.Navigate(Application.StartupPath & "\visualizations\" & My.Settings.defaultViz.ToString())

            End If
        End If
    End Sub

    'public Async Sub InitWebView()
    '    Await visualizer.EnsureCoreWebView2Async()
    '    AddHandler visualizer.CoreWebView2.WebMessageReceived,
    '    AddressOf WebView2_WebMessageReceived
    'End Sub


    Public Sub visualizer_WebMessageReceived(
    sender As Object,
    e As Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs) Handles visualizer.WebMessageReceived



        Try
            Dim msg As String = e.TryGetWebMessageAsString()
            If String.IsNullOrEmpty(msg) Then Exit Sub
            If msg.StartsWith("contextmenu|") Then
                ContextMenuStrip1.Show(
                    visualizer,
                    visualizer.PointToClient(Cursor.Position)
                )
            End If

            Dim json = e.WebMessageAsJson
            If json.Contains("""hideContextMenu""") Then
                ContextMenuStrip1.Hide()
            End If
        Catch ex As Exception

        End Try

    End Sub

    Enum ViewMode
        Playlists
        PlaylistTracks
    End Enum

    Dim currentMode As ViewMode = ViewMode.Playlists
    Dim currentPlaylistPath As String = Nothing
    Dim currentPlaylist As SpectrumPlaylist


    Public Sub leftArrow_Click(sender As Object, e As EventArgs) Handles leftArrow.Click
        If curTabIndex = 9 Then
            curTabIndex = 6
            currentPlaylist = Nothing
            PopulatePLUI()
        Else
            'curTabIndex = 9
        End If
    End Sub

    Public playlistWindowFlag As Short = 0
    Public Sub LinkLabel29_Click(sender As Object, e As EventArgs) Handles LinkLabel29.Click
        playlistWindowFlag = 1
        If curTabIndex = 6 And ListView1.SelectedItems.Count = 1 Then EditPlaylistInfo.ShowDialog()
    End Sub

    Public Sub LinkLabel28_Click(sender As Object, e As EventArgs) Handles LinkLabel28.Click
        playlistWindowFlag = 0
        EditPlaylistInfo.ShowDialog()
    End Sub

    Public draggedItems As List(Of ListViewItem)
    Public Sub ListView1_ItemDrag(sender As Object, e As ItemDragEventArgs) Handles ListView1.ItemDrag
        If curTabIndex = 9 Then
            draggedItems = ListView1.SelectedItems.Cast(Of ListViewItem)().
                   Select(Function(i) CType(i.Clone(), ListViewItem)).
                   ToList()

            ListView1.DoDragDrop(draggedItems, DragDropEffects.Move)
        End If
    End Sub

    Private dropIndex As Integer = -1
    Public Sub ListView1_DragOver(sender As Object, e As DragEventArgs) Handles ListView1.DragOver
        If curTabIndex = 9 Then

            Dim pt = ListView1.PointToClient(New Point(e.X, e.Y))
            Dim item = ListView1.GetItemAt(pt.X, pt.Y)

            If item Is Nothing Then
                dropIndex = ListView1.Items.Count
            Else
                dropIndex = item.Index
            End If
        End If
    End Sub

    Public Sub ListView1_DragDrop(sender As Object, e As DragEventArgs) Handles ListView1.DragDrop
        If curTabIndex = 9 Then
            If Not e.Data.GetDataPresent(GetType(List(Of ListViewItem))) Then Return

            Dim dropPoint = ListView1.PointToClient(New Point(e.X, e.Y))
            Dim targetItem = ListView1.GetItemAt(dropPoint.X, dropPoint.Y)

            Dim insertIndex As Integer =
        If(targetItem Is Nothing, ListView1.Items.Count, targetItem.Index)

            For Each item In ListView1.SelectedItems.Cast(Of ListViewItem).ToList()
                ListView1.Items.Remove(item)
            Next

            For i = 0 To draggedItems.Count - 1
                ListView1.Items.Insert(insertIndex + i, draggedItems(i))
            Next

            SavePlaylistFromListView()
        End If
    End Sub

    Private Sub UpdateRadioOrderFromListView()
        Dim path = Application.StartupPath & "\radios\radios.yaml"

        Dim radios = New List(Of RadioStation)

        radios.Clear()

        radios = radios _
    .GroupBy(Function(r) r.id) _
    .Select(Function(g) g.First()) _
    .ToList()

        For Each item As ListViewItem In ListView1.Items
            radios.Add(CType(item.Tag, RadioStation))
        Next

        SaveRadios(path, radios)
    End Sub


    Public Sub SavePlaylistFromListView()
        currentPlaylist.Files.Clear()

        For Each item As ListViewItem In ListView1.Items
            currentPlaylist.Files.Add(CStr(item.Tag))
        Next

        currentPlaylist.Save()
    End Sub

    Public Sub RemoveRadio(path As String, radioToRemove As String)

        Dim radios = LoadRadios(path)

        ' Remove by ID / URL / Name (URL is safest)
        radios.RemoveAll(Function(r) r.id = radioToRemove)

        SaveRadios(path, radios)

    End Sub

    Public Sub SaveRadios(path As String, radios As List(Of RadioStation))

        Dim root As New RadioRoot With {
        .radios = radios
    }

        Dim serializer = New SerializerBuilder().
        ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull).
        Build()

        Using writer As New StreamWriter(path)
            serializer.Serialize(writer, root)
        End Using

    End Sub

    'Private curRadioId As

    Public Sub DeleteSelectedToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteSelectedToolStripMenuItem.Click
        Dim confirmMsg As DialogResult = MessageBox.Show("Are you sure you want to permanently delete these files from your system? This action cannot be undone!", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
        If confirmMsg = DialogResult.Yes Then
            If curTabIndex <> 9 And curTabIndex <> 4 Then
                For Each item As ListViewItem In ListView1.SelectedItems
                    If IO.File.Exists(item.Tag.ToString()) Then
                        IO.File.Delete(item.Tag.ToString())
                    End If
                    ListView1.Items.Remove(item)
                Next
                GetIcons()
                ChooseCorrectIcon()
                songTextQA.Text = "-"
            Else
                If curTabIndex <> 4 Then
                    For Each item As ListViewItem In ListView1.SelectedItems
                        ListView1.Items.Remove(item)
                        currentPlaylist.Files.Remove(item.Tag.ToString())
                    Next
                    GetIcons()
                    ChooseCorrectIcon()
                    songTextQA.Text = currentPlaylist.Name.ToString()
                    currentPlaylist.Save()
                ElseIf curTabIndex = 4 Then
                    Dim selRadio As RadioStation = CType(ListView1.SelectedItems(0).Tag, RadioStation)
                    RemoveRadio(Application.StartupPath & "\radios\radios.yaml", selRadio.id)
                    GetIcons()
                    ChooseCorrectIcon()
                    songTextQA.Text = "-"
                    RefreshRadios()
                End If
            End If
        End If
    End Sub

    Public Sub CreatePlaylistToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CreatePlaylistToolStripMenuItem.Click
        LinkLabel28_Click(sender, e)
    End Sub

    Public Sub EditPlaylistToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditPlaylistToolStripMenuItem.Click
        LinkLabel29_Click(sender, e)
    End Sub

    Public Sub LinkLabel23_Click(sender As Object, e As EventArgs) Handles LinkLabel23.Click
        DeleteSelectedToolStripMenuItem_Click(sender, e)
    End Sub

    Public radioWindowFlag As Short = 0
    Public Sub LinkLabel210_Click(sender As Object, e As EventArgs) Handles LinkLabel210.Click
        radioWindowFlag = 0
        EditRadioInfo.ShowDialog()
    End Sub

    Public Sub LinkLabel211_Click(sender As Object, e As EventArgs) Handles LinkLabel211.Click
        radioWindowFlag = 1
        EditRadioInfo.ShowDialog()
    End Sub

    Private Sub ListView1_DragEnter(sender As Object, e As DragEventArgs) Handles ListView1.DragEnter
        If e.Data.GetDataPresent(GetType(List(Of ListViewItem))) Then
            e.Effect = DragDropEffects.Move
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub nextBtn_Click(sender As Object, e As EventArgs) Handles nextBtn.Click
        If _mediaPlayer.IsPlaying Then
            Try
                PlayFromNextBtn()
            Catch ex As Exception

            End Try
        End If
    End Sub

    Private Sub previousBtn_Click(sender As Object, e As EventArgs) Handles previousBtn.Click
        If _mediaPlayer.IsPlaying Then
            Try
                PlayFromPrevBtn()
            Catch ex As Exception

            End Try
        End If
    End Sub
End Class