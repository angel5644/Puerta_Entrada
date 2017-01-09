Imports Oracle.ManagedDataAccess.Client
Imports System.Linq

Public Class CancelacionEntrada
    Inherits System.Web.UI.Page

    Private _puertaEntradaService As PuertaEntradaService = New PuertaEntradaService()

    Private ReadOnly msjRequeridos As String = "Los campos folio y fecha de cita o placa y fecha de cita son requeridos"
    Private ReadOnly msjFechaFormato As String = "La fecha de la cita debe tener el formato dd/MM/yyyy"
    Private ReadOnly msjFolioNumerico As String = "El campo folio debe ser un valor numérico"
    Private ReadOnly msjPlacaRequerida As String = "La placa es requerida si no se ingresa folio"

    Private ReadOnly msjCancelacionNoValida As String = "No es posible cancelar el pase. El folio, la placa o la fecha no son válidos"
    Private ReadOnly formatoFecha As String = "dd/MM/yyyy"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Dim folio As Integer = 2256
        'Dim fecha As New DateTime(2016, 12, 28)
        'Dim r As InfoCancelPass = _puertaEntradaService.GetInfoCancelPass(2256, fecha, "")

        If (Not IsPostBack) Then
            gridCancelarUnidades.DataSource = InicializarDatatable()
            gridCancelarUnidades.DataBind()
        End If

    End Sub

    Public Function InicializarDatatable() As DataTable
        Dim tabla As DataTable = New DataTable("TablaIngreso")
        tabla.Columns.Add("Contenedor", Type.GetType("System.String"))
        tabla.Columns.Add("Trafico", Type.GetType("System.String"))
        tabla.Columns.Add("ISO", Type.GetType("System.String"))
        tabla.Columns.Add("Tamaño", Type.GetType("System.String"))
        tabla.Columns.Add("Tipo de Contenedor", Type.GetType("System.String"))
        tabla.Columns.Add("Tipo de Carga", Type.GetType("System.String"))
        tabla.Columns.Add("Sello 1", Type.GetType("System.String"))

        Dim row As DataRow = tabla.NewRow()
        For i As Integer = 0 To tabla.Columns.Count - 1
            row(i) = ""
        Next
        tabla.Rows.Add(row)

        Return tabla
    End Function

    Protected Sub Buscar(ByVal sender As Object, ByVal e As EventArgs)
        LimpiarPanelMensajes()
        lblFolioValido.Text = String.Empty
        lblFechaValida.Text = String.Empty
        lblPlacaValida.Text = String.Empty

        Dim textoFolio As String = txtFolio.Value.Trim()
        Dim textoFechaCita As String = txtFechaCita.Value.Trim()
        Dim placa As String = txtPlaca.Value.Trim()

        ' Validar campos
        Dim msj As String = String.Empty
        Dim valido As Boolean = ValidarCampos(textoFolio, textoFechaCita, placa, msj)

        ' Buscar pase si los datos son válidos
        If valido Then
            Try
                Dim folio As Integer? = Nothing
                If (Not String.IsNullOrEmpty(textoFolio)) Then
                    ' Ya no se valida que sea entero porque ya se validó anteriormente
                    folio = Integer.Parse(textoFolio)

                    lblNombrePase.Text = textoFolio
                Else
                    lblNombrePase.Text = placa
                End If
                Dim fechaCita As Date = Date.ParseExact(textoFechaCita, formatoFecha, System.Globalization.CultureInfo.InvariantCulture)

                ' Ejecutar procedimiento para obtener info cancel
                Dim infoCancel As InfoCancelPass = _puertaEntradaService.GetInfoCancelPass(folio, fechaCita, placa)

                ' Setear valores ocultos para posteriores validaciones
                lblFolioValido.Text = infoCancel.P_InfoTrsp.FolioPass
                lblFechaValida.Text = fechaCita
                lblPlacaValida.Text = placa

                lblFechaPase.Text = fechaCita.ToString(formatoFecha)

                ' Popular info del cursor p_InfoTrsp
                txtPlacas.Value = infoCancel.P_InfoTrsp.Trsp_Plate_Number
                txtOperador.Value = infoCancel.P_InfoTrsp.Trsp_Driver_Name
                txtTelefono.Value = infoCancel.P_InfoTrsp.Trsp_Call_Date

                ' Llenar con vacios si no hay datos
                If infoCancel.P_Cursor.Rows.Count <= 0 Then
                    Dim row As DataRow = infoCancel.P_Cursor.NewRow()
                    For i As Integer = 0 To infoCancel.P_Cursor.Columns.Count - 1
                        row(i) = ""
                    Next i

                    infoCancel.P_Cursor.Rows.Add(row)
                End If

                ' Popular info del cursor P_Cursor
                gridCancelarUnidades.DataSource = infoCancel.P_Cursor
                gridCancelarUnidades.DataBind()

            Catch ex As Exception
                Dim mensaje As String = String.Format("Error al buscar pase. Mensaje: {0}", ex.Message)
                msgError.Text = mensaje
                divErrorPuertaEntrada.Visible = True

                Debug.WriteLine(String.Format("{0}. Detalles: {1}", mensaje, ex.ToString()))
            End Try
        Else
            ' Mostrar mensaje
            msgError.Text = msj
            divErrorPuertaEntrada.Visible = True
        End If

        upPanelMensajes.Update()
    End Sub

    Protected Sub AbrirModalCancelar(sender As Object, e As EventArgs)
        LimpiarPanelMensajes()

        Dim folio As String = txtFolio.Value.Trim() ' Obtener de la caja de texto folio
        Dim fechaCita As String = txtFechaCita.Value.Trim()
        Dim placa As String = txtPlaca.Value.Trim()

        If String.IsNullOrEmpty(folio) Then
            ' Obtener de otra fuente
            folio = lblFolioValido.Text
        End If

        ' Comprobar si pase existe
        Dim msj As String = String.Empty
        Dim existe As Boolean = PaseExiste(folio, fechaCita, placa, msj)

        If existe Then
            ' Mostrar modal
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "modalCancelar", "$('#modalCancelar').modal();", True)
            upModalCancelar.Update()
        Else
            msgError.Text = msj
            divErrorPuertaEntrada.Visible = True
        End If

        upPanelMensajes.Update()
    End Sub

    Protected Sub CancelarPase(sender As Object, e As EventArgs)
        LimpiarPanelMensajes()

        Dim folioTexto As String = txtFolio.Value.Trim()
        Dim fechaCitaTexto As String = txtFechaCita.Value.Trim()
        Dim placaTexto As String = txtPlaca.Value.Trim()

        ' Obtener de la caja de texto placa
        If String.IsNullOrEmpty(folioTexto) Then
            ' Obtener de otra fuente
            folioTexto = lblFolioValido.Text
        End If

        ' Validar campos
        Dim msj As String = String.Empty
        Dim existe As Boolean = PaseExiste(folioTexto, fechaCitaTexto, placaTexto, msj)

        ' Cancelar pase si los datos son válidos
        If existe Then
            Try
                ' Ejecutar procedimiento
                Dim folio As Integer = Integer.Parse(folioTexto)
                Dim fechaCita As Date = Date.ParseExact(fechaCitaTexto, formatoFecha, System.Globalization.CultureInfo.InvariantCulture)
                Dim exito As Integer = _puertaEntradaService.CancelPassEntrance(folio, fechaCita)

                If exito > 0 Then
                    ' Mostrar mensaje si el procedimiento se ejecutó con exito
                    ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "msgCancelado", "alert('El pase fue cancelado con éxito. El módulo será limpiado.');", True)

                    ' Cargar página de nuevo
                    ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "loadCurrentPage", "loadCurrentPage();", True)
                Else
                    ' No se ejecutó exitosamente
                    msgError.Text = "El pase no fue cancelado correctamente. Por favor intente de nuevo más tarde."
                    divErrorPuertaEntrada.Visible = True
                End If

            Catch ex As Exception
                Dim mensaje As String = String.Format("Error al cancelar pase. Mensaje: {0}", ex.Message)
                msgError.Text = mensaje
                divErrorPuertaEntrada.Visible = True

                Debug.WriteLine(String.Format("{0}. Detalles: {1}", mensaje, ex.ToString()))
            End Try
        Else
            ' Mostrar mensaje
            msgError.Text = msj
            divErrorPuertaEntrada.Visible = True
        End If

        upPanelMensajes.Update()
    End Sub

    Private Function ValidarCampos(ByVal folio As String, ByVal fechaCita As String, ByVal placa As String, ByRef msj As String) As Boolean

        ' Validar campos
        If (Not String.IsNullOrEmpty(folio) And Not String.IsNullOrEmpty(fechaCita)) Or (Not String.IsNullOrEmpty(placa) And Not String.IsNullOrEmpty(fechaCita)) Then
            Dim fechaConvertida As Date = Date.Now
            Dim convertida As Boolean = Date.TryParseExact(fechaCita, formatoFecha, System.Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, fechaConvertida)

            ' Validar fecha
            If (convertida) Then
                If Not String.IsNullOrEmpty(folio) Then
                    ' Validar que el folio sea un valor numérico
                    If IsNumeric(folio) Then
                        ' Valido
                        Return True
                    Else
                        ' Folio no es válido
                        msj = msjFolioNumerico
                    End If
                Else
                    If Not String.IsNullOrEmpty(placa) Then
                        ' Valido 
                        Return True
                    Else
                        ' Placa es requerida
                        msj = msjPlacaRequerida
                    End If
                End If
            Else
                ' Fecha no es válida
                msj = msjFechaFormato
            End If
        Else
            ' Mostrar mensaje, campos requeridos
            msj = msjRequeridos
        End If

        Return False
    End Function

    Private Function PaseExiste(ByVal folio As String, ByVal fechaCita As String, ByVal placa As String, ByRef msj As String) As Boolean

        ' Validar folio y fecha
        If (Not String.IsNullOrEmpty(txtFolio.Value) And txtFolio.Value = lblFolioValido.Text) And Not String.IsNullOrEmpty(txtOperador.Value) Then
            If Not String.IsNullOrEmpty(txtFechaCita.Value) And txtFechaCita.Value = lblFechaValida.Text Then
                ' Existe
                Return True
            Else
                msj = msjCancelacionNoValida
            End If
        Else
            ' Validar placa y fecha
            If (Not String.IsNullOrEmpty(txtPlaca.Value) And txtPlaca.Value = lblPlacaValida.Text) And Not String.IsNullOrEmpty(txtOperador.Value) Then
                If Not String.IsNullOrEmpty(txtFechaCita.Value) And txtFechaCita.Value = lblFechaValida.Text Then
                    ' Existe
                    Return True
                Else
                    msj = msjCancelacionNoValida
                End If
            Else
                msj = msjCancelacionNoValida
            End If
        End If

        Return False
    End Function

    Private Sub LimpiarPanelMensajes()
        msgSuccess.Text = ""
        divSuccessPuertaEntrada.Visible = False
        msgError.Text = ""
        divErrorPuertaEntrada.Visible = False
    End Sub

End Class