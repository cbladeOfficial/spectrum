Public Class RadioRoot
    Public Property radios As List(Of RadioStation)
End Class

Public Class RadioStation
    Public Property id As String
    Public Property name As String
    Public Property stream_url As String
    Public Property website As String
    Public Property country As String
    Public Property language As String
    Public Property genre As List(Of String)

    Public Property bitrate As Integer?
    Public Property codec As String
    Public Property favicon As String

    Public Property last_status As String
    Public Property last_checked As DateTime?

    <YamlDotNet.Serialization.YamlIgnore>
    Public Property status As RadioStatus = RadioStatus.Unknown
End Class

Public Enum RadioStatus
    Unknown
    Checking
    Online
    Offline
End Enum
