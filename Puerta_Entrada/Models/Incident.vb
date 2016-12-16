Public Class Incident
    Private _Cict_Id As Integer
    Public Property Cict_Id() As Integer
        Get
            Return _Cict_Id
        End Get
        Set
            _Cict_Id = Value
        End Set
    End Property

    Private _Cict_Name As String
    Public Property Cict_Name() As String
        Get
            Return _Cict_Name
        End Get
        Set
            _Cict_Name = Value
        End Set
    End Property
End Class
