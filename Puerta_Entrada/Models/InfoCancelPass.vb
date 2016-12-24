Public Class InfoCancelPass
    Private _P_InfoTrsp As InfoTrspCancelar
    Public Property P_InfoTrsp() As InfoTrspCancelar
        Get
            Return _P_InfoTrsp
        End Get
        Set(value As InfoTrspCancelar)
            _P_InfoTrsp = value
        End Set
    End Property

    Private _P_Cursor As DataTable
    Public Property P_Cursor() As DataTable
        Get
            Return _P_Cursor
        End Get
        Set
            _P_Cursor = Value
        End Set
    End Property

End Class
