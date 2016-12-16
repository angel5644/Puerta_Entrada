Public Class RequestDocument

    Private _P_DocumentName As String
    Public Property P_DocumentName() As String
        Get
            Return _P_DocumentName
        End Get
        Set(ByVal value As String)
            _P_DocumentName = value
        End Set
    End Property

    Private _File As Byte()
    Public Property File() As Byte()
        Get
            Return _File
        End Get
        Set(ByVal value As Byte())
            _File = value
        End Set
    End Property

    Private _P_DocumentExtension As String
    Public Property P_DocumentExtension() As String
        Get
            Return _P_DocumentExtension
        End Get
        Set(ByVal value As String)
            _P_DocumentExtension = value
        End Set
    End Property


    Private _P_DocumentPath As String
    Public Property P_DocumentPath() As String
        Get
            Return _P_DocumentPath
        End Get
        Set(ByVal value As String)
            _P_DocumentPath = value
        End Set
    End Property



End Class
