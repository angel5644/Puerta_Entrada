﻿Imports Oracle.ManagedDataAccess.Client
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
        Dim msj As String = String.Empty
        Dim valido As Boolean = ValidarCampos(folio, fechaCita, placa, msj)

        ' Buscar pase si los datos son válidos
        If valido Then
            Try
                ' Ejecutar procedimiento
                ' **************

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

        Dim folio As String = txtFolio.Value ' Obtener de la caja de texto placa
        Dim fechaCita As String = txtFechaCita.Value
        Dim placa As String = txtPlaca.Value

        If String.IsNullOrEmpty(folio) Then
            ' Obtener de otra fuente
            '*******
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

        Dim folio As String = txtFolio.Value
        Dim fechaCita As String = txtFechaCita.Value
        Dim placa As String = txtPlaca.Value

        ' Obtener de la caja de texto placa
        If String.IsNullOrEmpty(folio) Then
            ' Obtener de otra fuente

        End If

        ' Validar campos
        Dim msj As String = String.Empty
        Dim valido As Boolean = ValidarCampos(folio, fechaCita, placa, msj)

        ' Cancelar pase si los datos son válidos
        If valido Then
            Try
                ' Ejecutar procedimiento
                ' **************

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
        If txtFolio.Value = lblFolioValido.Text And Not String.IsNullOrEmpty(txtOperador.Value) Then
            If txtFechaCita.Value = lblFechaValida.Text Then
                ' Existe
                Return True
            Else
                msj = msjCancelacionNoValida
            End If
        Else
            ' Validar placa y fecha
            If txtPlaca.Value = lblPlacaValida.Text And Not String.IsNullOrEmpty(txtOperador.Value) Then
                If txtFechaCita.Value = lblFechaValida.Text Then
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