Public Class InfoTrsp
    Private _Trsp_Plate_Number As String
    Public Property Trsp_Plate_Number() As String
        Get
            Return _Trsp_Plate_Number
        End Get
        Set
            _Trsp_Plate_Number = Value
        End Set
    End Property

    Private _Tipo_Pass As String
    Public Property Tipo_Pass() As String
        Get
            Return _Tipo_Pass
        End Get
        Set(ByVal value As String)
            _Tipo_Pass = value
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


    Private _Tipo_Lic As String
    Public Property Tipo_Lic() As String
        Get
            Return _Tipo_Lic
        End Get
        Set(ByVal value As String)
            _Tipo_Lic = value
        End Set
    End Property


    Private _Licencia As String
    Public Property Licencia() As String
        Get
            Return _Licencia
        End Get
        Set(ByVal value As String)
            _Licencia = value
        End Set
    End Property


    Private _Trsp_Name As String
    Public Property Trsp_Name() As String
        Get
            Return _Trsp_Name
        End Get
        Set(ByVal value As String)
            _Trsp_Name = value
        End Set
    End Property


    Private _Trsp_Trty_Type As String
    Public Property Trsp_Trty_Type() As String
        Get
            Return _Trsp_Trty_Type
        End Get
        Set(ByVal value As String)
            _Trsp_Trty_Type = value
        End Set
    End Property


End Class
