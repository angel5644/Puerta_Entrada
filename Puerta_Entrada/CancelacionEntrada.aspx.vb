Imports Oracle.ManagedDataAccess.Client
Imports System.Linq

Public Class CancelacionEntrada
    Inherits System.Web.UI.Page

    Private _puertaEntradaService As PuertaEntradaService = New PuertaEntradaService()

    Private ReadOnly msjRequeridos As String = "Los campos folio, placa y fecha de cita son requeridos"
    Private ReadOnly msjFechaFormato As String = "La fecha de la cita debe tener el formato dd/MM/yyyy"
    Private ReadOnly msjFolioNumerico As String = "El campo folio debe ser un valor numérico"

    Private ReadOnly msjCancelacionNoValida As String = "No es posible cancelar el pase. El folio, la placa o la fecha no son válidos"
    Private ReadOnly formatoFecha As String = "dd/MM/yyyy"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

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

        Dim folio As String = txtFolio.Value
        Dim fechaCita As String = txtFechaCita.Value
        Dim placa As String = txtPlaca.Value

        ' Validar campos
        If Not String.IsNullOrEmpty(folio) And Not String.IsNullOrEmpty(fechaCita) And Not String.IsNullOrEmpty(placa) Then

            ' Validar que el folio sea un valor numérico
            If IsNumeric(folio) Then

                Dim fechaConvertida As Date = Date.Now
                Dim convertida As Boolean = Date.TryParseExact(fechaCita, formatoFecha, System.Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, fechaConvertida)

                If (convertida) Then
                    Try

                        ' Ejecutar procedimiento buscar
                        Dim info As InfoPuertaEntrada = _puertaEntradaService.GetInfoPuertaEntrada(folio, fechaConvertida)



                    Catch ex As Exception
                        Dim mensaje As String = String.Format("Error al buscar. Mensaje: {0}", ex.Message)
                        msgError.Text = mensaje
                        divErrorPuertaEntrada.Visible = True

                        Debug.WriteLine(String.Format("{0}. Detalles: {1}", mensaje, ex.ToString()))
                    End Try
                Else
                    ' Fecha no es válida
                    msgError.Text = msjFechaFormato
                    divErrorPuertaEntrada.Visible = True
                End If
            Else
                ' Folio no es válido
                msgError.Text = msjFolioNumerico
                divErrorPuertaEntrada.Visible = True
            End If
        Else
            ' Mostrar mensaje, campos requeridos
            msgError.Text = msjRequeridos
            divErrorPuertaEntrada.Visible = True
        End If

        upPanelMensajes.Update()
    End Sub

    Protected Sub AbrirModalCancelar(sender As Object, e As EventArgs)
        LimpiarPanelMensajes()

        ' Validar campos folio, placa y fecha
        If txtFolio.Value = lblFolioValido.Text And Not String.IsNullOrEmpty(txtOperador.Value) Then
            If txtFechaCita.Value = lblFechaValida.Text Then
                If txtPlaca.Value = lblPlacaValida.Text Then

                    ' Mostrar modal
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "modalCancelar", "$('#modalCancelar').modal();", True)
                    upModalCancelar.Update()
                Else
                    msgError.Text = msjCancelacionNoValida
                    divErrorPuertaEntrada.Visible = True
                End If
            Else
                msgError.Text = msjCancelacionNoValida
                divErrorPuertaEntrada.Visible = True
            End If
        Else
            msgError.Text = msjCancelacionNoValida
            divErrorPuertaEntrada.Visible = True
        End If

        upPanelMensajes.Update()
    End Sub

    Protected Sub CancelarPase(sender As Object, e As EventArgs)
        LimpiarPanelMensajes()

        Dim textoFolio As String = txtFolio.Value
        Dim textoFechaCita As String = txtFechaCita.Value
        Dim textoPlaca As String = txtPlaca.Value

        ' Validar folio, placa y fecha
        If Not String.IsNullOrEmpty(textoFolio) And Not String.IsNullOrEmpty(textoFechaCita) And Not String.IsNullOrEmpty(textoPlaca) Then
            ' Convertir fecha en formato especificado
            Dim fechaCita As Date = Date.Now
            Dim convertida As Boolean = Date.TryParseExact(textoFechaCita, formatoFecha, System.Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, fechaCita)

            ' Si la fecha es válida
            If convertida Then
                If IsNumeric(textoFolio) Then
                    ' Convertir folio
                    Dim folio As Integer = Integer.Parse(textoFolio)

                    Try
                        ' Cancelar el pase ** Agregar procedimiento
                        Dim exito As Integer = 0

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
                        Dim mensaje As String = String.Format("Error al calcelar el pase. Mensaje: {0}", ex.Message)
                        msgError.Text = mensaje
                        divErrorPuertaEntrada.Visible = True

                        Debug.WriteLine(String.Format("{0}. Detalles: {1}", mensaje, ex.ToString()))
                    End Try
                Else
                    ' El folio debe ser numérico
                    msgError.Text = msjFolioNumerico
                    divErrorPuertaEntrada.Visible = True
                End If
            Else
                ' La fecha no esta en formato correcto
                msgError.Text = msjFechaFormato
                divErrorPuertaEntrada.Visible = True
            End If
        Else
            ' El folio, la placa o la fecha estan vacios
            msgError.Text = msjRequeridos
            divErrorPuertaEntrada.Visible = True
        End If

        upPanelMensajes.Update()
    End Sub

    Private Sub LimpiarPanelMensajes()
        msgSuccess.Text = ""
        divSuccessPuertaEntrada.Visible = False
        msgError.Text = ""
        divErrorPuertaEntrada.Visible = False
    End Sub

End Class