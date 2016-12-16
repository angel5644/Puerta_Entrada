Public Class PuertaEntrada
    Inherits System.Web.UI.Page

    Private _puertaEntradaService As PuertaEntradaService = New PuertaEntradaService()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If (Not IsPostBack) Then

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

            gridIngresoUnidades.DataSource = tabla
            gridIngresoUnidades.DataBind()
        End If

        ' Volver a crear al hacer postback
        If lblP_EnableVGM.Text = "Y" Then
            AgregarBotonDinamico()
        End If
    End Sub

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
                Dim convertida As Boolean = Date.TryParseExact(fechaCita, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, fechaConvertida)

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

                        ' Agregar columna para archivo si p_EnableVGM = Y
                        If info.P_EnableVGM = "Y" Then
                            info.P_Cursor.Columns.Add("Archivo")
                        End If

                        ' Popular info del cursor p_InfoCita
                        gridIngresoUnidades.DataSource = info.P_Cursor
                        gridIngresoUnidades.DataBind()

                        lblP_EnableVGM.Text = info.P_EnableVGM
                        ' Agregar boton archivo
                        If info.P_EnableVGM = "Y" Then
                            AgregarBotonDinamico()
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
                    msgError.Text = "La fecha de la cita debe tener el formato dd/MM/yyyy"
                    divErrorPuertaEntrada.Visible = True
                End If
            Else
                ' Folio no es válido
                msgError.Text = "El campo folio debe ser un valor numérico"
                divErrorPuertaEntrada.Visible = True
            End If
        Else
            ' Mostrar mensaje, campos requeridos
            msgError.Text = "Por favor complete los campos requeridos"
            divErrorPuertaEntrada.Visible = True
        End If

        upPanelMensajes.Update()
    End Sub

    Protected Sub PermitirIngreso(sender As Object, e As EventArgs)
        ' nothing
    End Sub

    Protected Sub AbrirArchivo(sender As Object, e As EventArgs)
        LimpiarPanelMensajes()

        Try
            Dim requestId As String = gridIngresoUnidades.Rows(0).Cells(1).Text ' Request cell value 
            Dim docId As Integer = Integer.Parse(requestId) ' get id 'width=""500px"" height=""600px""

            Dim embed As String = ""
            Dim fileType As String = "image"
            Dim contentType As String = ""

            ' id image jpg: 5748
            ' id pdf file: 3009
            Dim doc As RequestDocument = _puertaEntradaService.ReadFile(docId)

            lblNombreArchivo.Text = doc.P_DocumentName

            ' Comprobar si es archivo pdf
            If doc.P_DocumentExtension = "pdf" Then
                fileType = "pdf"
                embed = "<object data=""{0}"" width=""100%"" height=""600px"" type=""application/pdf"" >"
                embed += "Si no puedes ver el archivo, lo puedes descargar desde <a href = ""{0}&download=1"">aquí</a>"
                embed += " o descargar <a target = ""_blank"" href = ""http://get.adobe.com/reader/"">Adobe PDF Reader</a> para ver el archivo."
                embed += "</object>"
            Else
                fileType = "image"
                ' Comprobar si es imagen
                If doc.P_DocumentExtension = "jpg" Then
                    embed = "<object data=""{0}"" class=""img-responsive"" type=""image/jpg"" >"
                    embed += "</object>"
                End If
            End If
            Dim url As String = String.Format(ResolveUrl("~/Default.aspx?Id={0}&fileType={1}"), docId, fileType)
            ltEmbed.Text = String.Format(embed, url)

            ' Mostrar modal
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "modalFile", "$('#modalFile').modal();", True)
            upPanelFile.Update()
        Catch ex As Exception
            Dim mensaje As String = String.Format("Error al obtener abrir archivo. Mensaje: {0}", ex.Message)

            msgError.Text = mensaje
            divErrorPuertaEntrada.Visible = True
        End Try

        upPanelMensajes.Update()
    End Sub

    Protected Sub AbrirModalRegistrar(sender As Object, e As EventArgs)
        LimpiarPanelMensajes()
        'lblModalTitle.Text = "Envio de correo por discrepancias en transporte" ' Titulo de modal

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
        upPanelMensajes.Update()
        upModal.Update()
    End Sub

    Protected Sub RegistrarDiscrepancia(sender As Object, e As EventArgs)
        LimpiarPanelMensajes()
        Dim formulario As String = Request.Form("formModalRegistrar")
        Dim comentarios As String = txtComentarios.Value
        Dim tipoDiscrepancia As Integer = 0
        Dim convertido As Boolean = Integer.TryParse(comboTipoDiscrepancia.SelectedValue, tipoDiscrepancia)

        If (convertido) Then
            ' Proceso para registrar discrepancia

            ' Cerrar modal
            ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "closeModal", "$('#modalRegistrarDiscrepancia').modal('hide');", True)

            msgSuccess.Text = "Discrepancia registrada con éxito"
            divSuccessPuertaEntrada.Visible = True
        Else
            msgError.Text = "Tipo de discrepancia seleccionada no válida"
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