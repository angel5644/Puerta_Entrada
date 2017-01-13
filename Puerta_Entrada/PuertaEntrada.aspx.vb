﻿Imports Oracle.ManagedDataAccess.Client
Imports System.Linq

Public Class PuertaEntrada
    Inherits System.Web.UI.Page

    Private _puertaEntradaService As PuertaEntradaService = New PuertaEntradaService()
    Private ReadOnly msjFolioFechaRequerido As String = "Los campos folio y fecha de cita son requeridos"
    Private ReadOnly msjFechaFormato As String = "La fecha de la cita debe tener el formato dd/MM/yyyy HH:mm"
    Private ReadOnly msjFolioNumerico As String = "El campo folio debe ser un valor numérico"
    Private ReadOnly msjIngresoNoValido As String = "No es posible perimitir el ingreso. El folio o la fecha no son válidos"
    Private ReadOnly msjDiscrepanciaNoValida As String = "No es posible registrar la discrepancia. El folio o la fecha no son válidos"
    Private ReadOnly formatoFecha As String = "dd/MM/yyyy HH:mm"
    Private ReadOnly msjNoTarjNumerico As String = "El campo número de tarjetón debe ser numérico"
    Private ReadOnly extImagValidas As String() = New String() {"jpg", "bmp", "gif", "png", "tif"} ' Agregar más extensiones si es necesario

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If (Not IsPostBack) Then
            gridIngresoUnidades.DataSource = InicializarDatatableIngreso()
            gridIngresoUnidades.DataBind()
        End If

        ' Volver a crear al hacer postback
        If lblP_EnableVGM.Text = "Y" And lblMostrarArchivo.Text = "Y" Then
            AgregarBotonDinamico()
        End If

    End Sub

    Public Function InicializarDatatableIngreso() As DataTable
        Dim tabla As DataTable = New DataTable("TablaIngreso")
        tabla.Columns.Add("Contenedor", Type.GetType("System.String"))
        tabla.Columns.Add("Trafico", Type.GetType("System.String"))
        tabla.Columns.Add("ISO", Type.GetType("System.String"))
        tabla.Columns.Add("Tamaño", Type.GetType("System.String"))
        tabla.Columns.Add("Tipo de Contenedor", Type.GetType("System.String"))
        tabla.Columns.Add("Tipo de Carga", Type.GetType("System.String"))
        tabla.Columns.Add("Sello 1", Type.GetType("System.String"))
        tabla.Columns.Add("Peso Bruto", Type.GetType("System.String"))
        tabla.Columns.Add("Fecha descarga del contenedor", Type.GetType("System.String"))
        tabla.Columns.Add("Posicion", Type.GetType("System.String"))

        Dim row As DataRow = tabla.NewRow()
        For i As Integer = 0 To tabla.Columns.Count - 1
            row(i) = ""
        Next
        tabla.Rows.Add(row)

        Return tabla
    End Function

    Private Sub AgregarBotonDinamico()
        For i As Integer = 0 To gridIngresoUnidades.Rows.Count - 1
            Dim lastCell As Integer = gridIngresoUnidades.Rows(i).Cells.Count - 1
            Dim btnArchivo As New System.Web.UI.HtmlControls.HtmlInputButton
            btnArchivo.Value = "Archivo"
            btnArchivo.Visible = True
            btnArchivo.Attributes.Add("class", "btn btn-default")
            AddHandler btnArchivo.ServerClick, AddressOf AbrirArchivo

            ' OnServerClick="AbrirModalRegistrar" runat="server"
            btnArchivo.ID = ("btnArchivo" + i.ToString())
            gridIngresoUnidades.Rows(i).Cells(lastCell).Controls.Add(btnArchivo)
        Next
    End Sub

    Protected Sub Buscar(ByVal sender As Object, ByVal e As EventArgs)
        LimpiarPanelMensajes()
        lblFolioValido.Text = String.Empty
        lblFechaValida.Text = String.Empty

        Dim formulario As String = Request.Form("formPuertaEntrada")
        Dim folio As String = txtFolio.Value
        Dim fechaCita As String = txtFechaCita.Value

        ' Validar campos
        If Not String.IsNullOrEmpty(folio) And Not String.IsNullOrEmpty(fechaCita) Then
            divErrorPuertaEntrada.Visible = False

            ' Validar que el folio sea un valor numérico
            If IsNumeric(folio) Then
                divErrorPuertaEntrada.Visible = False

                Dim fechaConvertida As Date = Date.Now
                Dim convertida As Boolean = Date.TryParseExact(fechaCita, formatoFecha, System.Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, fechaConvertida)

                If (convertida) Then
                    divErrorPuertaEntrada.Visible = False
                    Try
                        divErrorPuertaEntrada.Visible = False

                        ' Ejecutar procedimiento buscar
                        Dim info As InfoPuertaEntrada = _puertaEntradaService.GetInfoPuertaEntrada(folio, fechaConvertida)

                        ' Popular info del cursor p_InfoTrsp
                        txtPlacas.Value = info.P_InfoTrsp.Trsp_Plate_Number
                        txtTipoTrasporte.Value = info.P_InfoTrsp.Trsp_Trty_Type
                        txtNombreTransportista.Value = info.P_InfoTrsp.Trsp_Name
                        txtLicencia.Value = info.P_InfoTrsp.Licencia
                        txtTipoLicencia.Value = info.P_InfoTrsp.Tipo_Lic
                        txtOperador.Value = info.P_InfoTrsp.Trsp_Driver_Name
                        txtHoraLlamado.Value = info.P_InfoTrsp.Trsp_Call_Date
                        txtTipoPaseEntrada.Value = info.P_InfoTrsp.Tipo_Pass

                        ' Setear valores ocultos para posteriores validaciones
                        lblFolioValido.Text = folio
                        lblFechaValida.Text = fechaCita
                        lblP_EnableVGM.Text = info.P_EnableVGM
                        lblP_SolTarjeton.Text = info.P_SolTarjeton

                        ' Agregar columna para archivo si p_EnableVGM = Y
                        If info.P_EnableVGM = "Y" Then
                            ' Obtener document id
                            Dim docIdText As String = info.P_Cursor.Rows(0)("DOC_ID").ToString()
                            ' Validar que el document id sea válido
                            Dim docId As Integer = 0
                            Dim idConvertido = Integer.TryParse(docIdText, docId)
                            If (idConvertido) Then
                                ' Guardar doc id
                                lblDocId.Text = docId.ToString()
                            End If

                            ' Esconder columna doc_id
                            info.P_Cursor.Columns.Remove("DOC_ID")
                            ' Agregar columna archivo
                            info.P_Cursor.Columns.Add("Archivo")
                        End If

                        ' Llenar con vacios si no hay datos
                        If info.P_Cursor.Rows.Count <= 0 Then
                            Dim row As DataRow = info.P_Cursor.NewRow()
                            For i As Integer = 0 To info.P_Cursor.Columns.Count - 1
                                row(i) = ""
                            Next i

                            info.P_Cursor.Rows.Add(row)
                        End If

                        ' Popular info del cursor p_InfoCita
                        gridIngresoUnidades.DataSource = info.P_Cursor
                        gridIngresoUnidades.DataBind()

                        ' Agregar botón archivo si p_EnableVGM = Y
                        If info.P_Cursor.Rows.Count > 0 Then
                            If info.P_EnableVGM = "Y" Then
                                lblMostrarArchivo.Text = "Y"
                                AgregarBotonDinamico()
                            End If
                        End If

                        ' Validar variables
                        If info.P_AnotherPass = "Y" Then
                            ' Mostrar mensaje
                            msgSuccess.Text = info.P_Messaje
                            divSuccessPuertaEntrada.Visible = True

                            Dim alert As String = String.Format("alert('{0}');", info.P_Messaje)
                            ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "mensajeAnotherPass", alert, True)
                        End If

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
            msgError.Text = msjFolioFechaRequerido
            divErrorPuertaEntrada.Visible = True
        End If

        upPanelMensajes.Update()
    End Sub

    Protected Sub AbrirArchivo(sender As Object, e As EventArgs)
        LimpiarPanelMensajes()

        Try
            Dim requestId As String = lblDocId.Text ' Folio cell value 
            Dim docId As Integer = 0
            Dim idConvertido As Boolean = Integer.TryParse(requestId, docId) ' get id 'width=""500px"" height=""600px""

            If idConvertido Then
                Dim embed As String = ""
                Dim fileType As String = ""
                Dim esValido As Boolean = False ' Variable para saber si el archivo a mostrar le válido

                ' id image jpg: 5748
                ' id image/png: 15697
                ' id pdf file: 3009
                ' Obtener el archivo
                'docId = 3009 ' test
                Dim doc As RequestDocument = _puertaEntradaService.F_GetRequestDocument(docId)
                lblNombreArchivo.Text = doc.P_DocumentName

                ' Comprobar si es archivo pdf
                If doc.P_DocumentExtension = "pdf" Then
                    esValido = True
                    fileType = "pdf"
                    embed = "<object data=""{0}"" width=""100%"" height=""600px"" type=""application/pdf"" >"
                    embed += "Si no puedes ver el archivo, lo puedes descargar desde <a href = ""{0}&download=1"">aquí</a>"
                    embed += " o descargar <a target = ""_blank"" href = ""http://get.adobe.com/reader/"">Adobe PDF Reader</a> para ver el archivo."
                    embed += "</object>"

                    Dim url As String = String.Format(ResolveUrl("~/Default.aspx?Id={0}&fileType={1}"), docId, fileType)
                    ltEmbed.Text = String.Format(embed, url)
                    ltEmbed.Visible = True
                Else
                    If ExtImagenEsValida(doc.P_DocumentExtension) Then
                        esValido = True
                        fileType = String.Format("image/{0}", doc.P_DocumentExtension)

                        ' Obtener datos de imagen
                        Dim base64String As String = Convert.ToBase64String(doc.File, 0, doc.File.Length)

                        ' Setear datos de imagen
                        imageArchivo.ImageUrl = Convert.ToString(String.Format("data:{0};base64,", fileType)) & base64String
                        imageArchivo.Visible = True
                        imageArchivo.CssClass = "img-responsive"
                    Else
                        msgError.Text = "No es un extensión de imagen válida. Extensiones soportadas: jpg, bmp, gif, png, tif"
                        divErrorPuertaEntrada.Visible = True
                    End If
                End If

                If esValido Then
                    ' Mostrar modal
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "modalFile", "$('#modalFile').modal();", True)
                    upPanelFile.Update()
                End If
            Else

            End If
        Catch ex As Exception
            Dim mensaje As String = String.Format("Error al abrir archivo. Mensaje: {0}", ex.Message)

            msgError.Text = mensaje
            divErrorPuertaEntrada.Visible = True
        End Try

        upPanelMensajes.Update()
    End Sub

    Protected Sub AbrirModalRegistrar(sender As Object, e As EventArgs)
        LimpiarPanelMensajes()
        'lblModalTitle.Text = "Envio de correo por discrepancias en transporte" ' Titulo de modal

        If txtFolio.Value = lblFolioValido.Text And Not String.IsNullOrEmpty(txtNombreTransportista.Value) Then
            If txtFechaCita.Value = lblFechaValida.Text Then
                ' Limpiar campos
                txtComentarios.Value = ""
                comboTipoDiscrepancia.Items.Clear()

                Dim catalogoIncidentes As New List(Of Incident)
                Try
                    divErrorPuertaEntrada.Visible = False

                    ' Obtener catalogo de incidentes
                    catalogoIncidentes = _puertaEntradaService.GetIncidentCatalog()
                Catch ex As Exception
                    Dim mensaje As String = String.Format("Error al obtener catálogo de incidentes. Mensaje: {0}", ex.Message)
                    msgError.Text = mensaje
                    divErrorPuertaEntrada.Visible = True

                    Debug.WriteLine(String.Format("{0}. Detalles: {1}", mensaje, ex.ToString()))
                End Try

                ' Llenar el dropdown con el catalogo
                For Each incidente As Incident In catalogoIncidentes
                    Dim item As ListItem = New ListItem(incidente.Cict_Name, incidente.Cict_Id)

                    comboTipoDiscrepancia.Items.Add(item)
                Next
                comboTipoDiscrepancia.DataBind()

                ' Mostrar modal
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "modalRegistrarDiscrepancia", "$('#modalRegistrarDiscrepancia').modal();", True)
                upModal.Update()
            Else
                msgError.Text = msjDiscrepanciaNoValida
                divErrorPuertaEntrada.Visible = True
            End If
        Else
            msgError.Text = msjDiscrepanciaNoValida
            divErrorPuertaEntrada.Visible = True
        End If

        upPanelMensajes.Update()
    End Sub

    Protected Sub RegistrarDiscrepancia(sender As Object, e As EventArgs)
        LimpiarPanelMensajes()

        Dim textoFolio As String = txtFolio.Value
        Dim textoFechaCita As String = txtFechaCita.Value

        ' Validar folio y fecha
        If Not String.IsNullOrEmpty(textoFolio) And Not String.IsNullOrEmpty(textoFechaCita) Then
            ' Convertir fecha en formato especificado
            Dim fechaCita As Date = Date.Now
            Dim convertida As Boolean = Date.TryParseExact(textoFechaCita, formatoFecha, System.Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, fechaCita)

            ' Si la fecha es válida
            If convertida Then
                If IsNumeric(textoFolio) Then
                    Dim folio As Integer = Integer.Parse(textoFolio)
                    Dim comentarios As String = txtComentarios.Value
                    ' Obtener el tipo de discrepancia
                    Dim tipoDiscrepancia As Integer = 0
                    Dim disConvertida As Boolean = Integer.TryParse(comboTipoDiscrepancia.SelectedValue, tipoDiscrepancia)

                    If (disConvertida) Then

                        ' Proceso para registrar discrepancia
                        Try
                            Dim enviado As Integer = _puertaEntradaService.EnviarIncidente(folio, fechaCita, tipoDiscrepancia, comentarios)

                            If enviado > 0 Then
                                ' Mostrar mensaje si se envió exitosamente
                                ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "msgEnviado", "alert('Discrepancia enviada con éxito. El módulo será limpiado.');", True)

                                ' Cargar página de nuevo
                                ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "loadCurrentPage", "loadCurrentPage();", True)
                            Else
                                ' No se envió la incidencia
                                msgError.Text = "La discrepancia no se envió exitosamente. Por favor intente de nuevo más tarde."
                                divErrorPuertaEntrada.Visible = True
                            End If
                        Catch ex As Exception
                            Dim mensaje As String = String.Format("Error al enviar discrepancia. Mensaje: {0}", ex.Message)
                            msgError.Text = mensaje
                            divErrorPuertaEntrada.Visible = True

                            Debug.WriteLine(String.Format("{0}. Detalles: {1}", mensaje, ex.ToString()))
                        End Try

                        ' Cerrar modal
                        ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "closeModal", "$('#modalRegistrarDiscrepancia').modal('hide');", True)
                    Else
                        msgError.Text = "Tipo de discrepancia seleccionada no válida"
                        divErrorPuertaEntrada.Visible = True
                    End If
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
            ' El folio o la fecha estan vacios
            msgError.Text = msjFolioFechaRequerido
            divErrorPuertaEntrada.Visible = True
        End If

        upPanelMensajes.Update()
    End Sub

    Protected Sub PermitirIngreso(sender As Object, e As EventArgs)
        LimpiarPanelMensajes()

        ' Validar campos folio y fecha
        If txtFolio.Value = lblFolioValido.Text And Not String.IsNullOrEmpty(txtNombreTransportista.Value) Then
            If txtFechaCita.Value = lblFechaValida.Text Then
                ' Limpiar campo tarjeton
                txtNoTarjeton.Value = ""

                If lblP_SolTarjeton.Text = "Y" Then
                    ' Mostrar modal
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "modalTarjeton", "$('#modalTarjeton').modal();", True)
                    upModalTarjeton.Update()
                Else
                    ' Continuar el proceso, ingresar unidad
                    IngresarUnidad(btnContinuarIngresoT, EventArgs.Empty)
                End If
            Else
                msgError.Text = msjIngresoNoValido
                divErrorPuertaEntrada.Visible = True
            End If
        Else
            msgError.Text = msjIngresoNoValido
            divErrorPuertaEntrada.Visible = True
        End If

        upPanelMensajes.Update()
    End Sub

    Protected Sub IngresarUnidad(sender As Object, e As EventArgs)
        LimpiarPanelMensajes()

        Dim textoFolio As String = txtFolio.Value
        Dim textoFechaCita As String = txtFechaCita.Value

        ' Validar folio y fecha
        If Not String.IsNullOrEmpty(textoFolio) And Not String.IsNullOrEmpty(textoFechaCita) Then
            ' Convertir fecha en formato especificado
            Dim fechaCita As Date = Date.Now
            Dim convertida As Boolean = Date.TryParseExact(textoFechaCita, formatoFecha, System.Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, fechaCita)

            ' Si la fecha es válida
            If convertida Then
                If IsNumeric(textoFolio) Then
                    ' Convertir folio
                    Dim folio As Integer = Integer.Parse(textoFolio)

                    Try
                        ' Ingresar unidad
                        Dim exito As Integer = 0
                        Dim p_SolTarjeton As String = lblP_SolTarjeton.Text
                        'If lblP_SolTarjeton.Text = "Y" Then
                        '    ' Necesitamos ejecutar otro procedimiento cuando se solicita un no de tarjetón?
                        '    exito = _puertaEntradaService.IngresoPuertaEntrada(folio, fechaCita)
                        'Else
                        '    exito = _puertaEntradaService.IngresoPuertaEntrada(folio, fechaCita)
                        'End If
                        exito = _puertaEntradaService.IngresoPuertaEntrada(folio, fechaCita, p_SolTarjeton)

                        If exito > 0 Then
                            ' Mostrar mensaje si el procedimiento se ejecutó con exito
                            ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "msgIngresado", "alert('Ingreso de unidad realizado con éxito. El módulo será limpiado.');", True)

                            ' Cargar página de nuevo
                            ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "loadCurrentPage", "loadCurrentPage();", True)
                        Else
                            ' No se ejecutó exitosamente
                            msgError.Text = "El ingreso de la unidad no se realizó correctamente. Por favor intente de nuevo más tarde."
                            divErrorPuertaEntrada.Visible = True
                        End If
                    Catch ex As Exception
                        Dim mensaje As String = String.Format("Error al realizar el ingreso de la unidad. Mensaje: {0}", ex.Message)
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
            ' El folio o la fecha estan vacios
            msgError.Text = msjFolioFechaRequerido
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

    Private Function ExtImagenEsValida(extension As String) As Boolean
        extension = extension.ToLower().Trim()
        For Each validExtension As String In extImagValidas
            If extension = validExtension Then
                Return True
            End If
        Next

        Return False
    End Function

    Private Function ObtenerValorCelda(grid As GridView, numFila As Integer, nombreColumna As String) As String
        Dim index As Integer
        Dim valor As String = ""

        For Each tc As TableCell In grid.HeaderRow.Cells
            If tc.Text.Equals(nombreColumna) Then
                index = grid.HeaderRow.Cells.GetCellIndex(tc)
                valor = grid.Rows(numFila).Cells(index).Text
                Exit For
            End If
        Next

        Return valor
    End Function

    Public Sub LimpiarTodo()

        ' Lipiar campos de busqueda 
        txtFolio.Value = ""
        txtFechaCita.Value = ""

        ' Limpiar cambios de info prs
        txtPlacas.Value = ""
        txtTipoTrasporte.Value = ""
        txtNombreTransportista.Value = ""
        txtLicencia.Value = ""
        txtTipoLicencia.Value = ""
        txtOperador.Value = ""
        txtHoraLlamado.Value = ""
        txtTipoPaseEntrada.Value = ""

        ' Limpiar datatable
        gridIngresoUnidades.DataSource = InicializarDatatableIngreso()
        gridIngresoUnidades.DataBind()

        ' Limpiar campos hidden
        lblFechaValida.Text = ""
        lblFolioValido.Text = ""
        lblP_EnableVGM.Text = ""
    End Sub

End Class