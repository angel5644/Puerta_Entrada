Public Class InfoPuertaEntrada
    Private _P_InfoTrsp As InfoTrsp
    Public Property P_InfoTrsp() As InfoTrsp
        Get
            Return _P_InfoTrsp
        End Get
        Set(value As InfoTrsp)
            _P_InfoTrsp = Value
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

    Private _P_AnotherPass As String
    Public Property P_AnotherPass() As String
        Get
            Return _P_AnotherPass
        End Get
        Set(ByVal value As String)
            _P_AnotherPass = value
        End Set
    End Property

    Private _P_Messaje As String
    Public Property P_Messaje() As String
        Get
            Return _P_Messaje
        End Get
        Set(ByVal value As String)
            _P_Messaje = value
        End Set
    End Property

    Private _P_EnableVGM As String
    Public Property P_EnableVGM() As String
        Get
            Return _P_EnableVGM
        End Get
        Set(ByVal value As String)
            _P_EnableVGM = value
        End Set
    End Property

    Private _P_SolTarjeton As String
    Public Property P_SolTarjeton() As String
        Get
            Return _P_SolTarjeton
        End Get
        Set(ByVal value As String)
            _P_SolTarjeton = value
        End Set
    End Property

    ''' <summary>
    ''' Crea la estructura del datatable para el cursor p_InfoTrsp
    ''' </summary>
    ''' <returns>Regresa el datatable que representa el cursor p_InfoTrsp</returns>
    Public Function CrearP_InfoTrsp() As DataTable
        ' Datatable que mapea cursor p_InfoTrsp
        Dim p_InfoTrsp As DataTable = New DataTable("p_InfoTrsp")
        p_InfoTrsp.Columns.Add("TRSP_PLATE_NUMBER", Type.GetType("System.String"))
        p_InfoTrsp.Columns.Add("TRSP_TRTY_TYPE", Type.GetType("System.String"))
        p_InfoTrsp.Columns.Add("TRSP_NAME", Type.GetType("System.String"))
        p_InfoTrsp.Columns.Add("LICENCIA", Type.GetType("System.String"))
        p_InfoTrsp.Columns.Add("TIPO_LIC", Type.GetType("System.String"))
        p_InfoTrsp.Columns.Add("TRSP_DRIVER_NAME", Type.GetType("System.String"))
        p_InfoTrsp.Columns.Add("TRSP_CALL_DATE", Type.GetType("System.String"))
        p_InfoTrsp.Columns.Add("TIPO_PASS", Type.GetType("System.String"))

        Return p_InfoTrsp
    End Function

    ''' <summary>
    ''' Crea la estructura del datatable para el cursor p_InfoTrsp
    ''' </summary>
    ''' <returns></returns>
    Public Function CrearP_Cursor() As DataTable
        ' Datatable que mapea cursor p_InfoTrsp
        Dim p_InfoTrsp As DataTable = New DataTable("p_Cursor")

        Return p_InfoTrsp
    End Function
End Class
