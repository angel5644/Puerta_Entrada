Public Class InfoTrspCancelar
    Private _Trsp_Plate_Number As String
    Public Property Trsp_Plate_Number() As String
        Get
            Return _Trsp_Plate_Number
        End Get
        Set
            _Trsp_Plate_Number = Value
        End Set
    End Property

    Private _Trsp_Call_Date As String
    Public Property Trsp_Call_Date() As String
        Get
            Return _Trsp_Call_Date
        End Get
        Set(ByVal value As String)
            _Trsp_Call_Date = value
        End Set
    End Property


    Private _Trsp_Driver_Name As String
    Public Property Trsp_Driver_Name() As String
        Get
            Return _Trsp_Driver_Name
        End Get
        Set(ByVal value As String)
            _Trsp_Driver_Name = value
        End Set
    End Property
End Class
