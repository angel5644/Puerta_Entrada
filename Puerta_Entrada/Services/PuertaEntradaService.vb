Imports Oracle.ManagedDataAccess.Client

Public Class PuertaEntradaService

    Private dbContext As PEDBContext

    Public Sub New()
        dbContext = New PEDBContext()
    End Sub

    ''' <summary>
    ''' Obtiene la información de la puerta de entrada
    ''' </summary>
    ''' <param name="p_FolioTrsp">Folio</param>
    ''' <param name="p_FechaCita">Fecha de la cita</param>
    ''' <returns></returns>
    Public Function GetInfoPuertaEntrada(ByVal p_FolioTrsp As Integer, ByVal p_FechaCita As Date) As InfoPuertaEntrada
        ' Variable para regresar ambos datatables
        Dim result As New InfoPuertaEntrada

        ' Datatable que mapea cursor p_InfoTrsp
        Dim p_InfoCita As DataTable = New DataTable("p_Cursor")

        '11/04/00 | 38

        ' --[ Added by Vleon
        Using conn As New OracleConnection(dbContext.ObtenerCadenaConexion())
            Try
                conn.Open()

                Using cmdComando As New OracleCommand("CTS.PK_CTS.p_GetInfoPuertaEntrada", conn)

                    cmdComando.CommandType = CommandType.StoredProcedure

                    'p_GetInfoPuertaEntrada(p_FolioTrsp IN NUMBER, p_FechaCita IN DATE,p_InfoTrsp OUT C_CURC,p_Cursor OUT C_CURC,p_EnableVGM OUT VARCHAR2,p_AnotherPass OUT VARCHAR2, p_Messaje OUT VARCHAR2, p_SolTarjeton OUT VARCHAR2) IS

                    cmdComando.Parameters.Add("p_FolioTrsp", OracleDbType.Int32, p_FolioTrsp, ParameterDirection.Input)
                    cmdComando.Parameters.Add("p_FechaCita", OracleDbType.Date, p_FechaCita, ParameterDirection.Input)

                    'Parametros de Salida
                    cmdComando.Parameters.Add("p_InfoTrsp", OracleDbType.RefCursor).Direction = ParameterDirection.Output
                    cmdComando.Parameters.Add("p_Cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output
                    Dim p As OracleParameter
                    p = New OracleParameter("p_EnableVGM", OracleDbType.Varchar2)
                    p.Direction = ParameterDirection.Output
                    p.Size = 100
                    cmdComando.Parameters.Add(p)

                    p = New OracleParameter("p_AnotherPass", OracleDbType.Varchar2)
                    p.Direction = ParameterDirection.Output
                    p.Size = 100
                    cmdComando.Parameters.Add(p)

                    p = New OracleParameter("p_Messaje", OracleDbType.Varchar2)
                    p.Direction = ParameterDirection.Output
                    p.Size = 1000
                    cmdComando.Parameters.Add(p)

                    p = New OracleParameter("p_SolTarjeton", OracleDbType.Varchar2)
                    p.Direction = ParameterDirection.Output
                    p.Size = 100
                    cmdComando.Parameters.Add(p)

                    Dim infoTrsp As New InfoTrsp
                    Dim p_EnableVGM As String = String.Empty
                    Dim p_AnotherPass As String = String.Empty
                    Dim p_Messaje As String = String.Empty
                    Dim p_SolTarjeton As String = String.Empty

                    'Leer resultado de procedimiento
                    Using dr As OracleDataReader = cmdComando.ExecuteReader()
                        ' Obtener datos out
                        p_EnableVGM = cmdComando.Parameters("p_EnableVGM").Value.ToString()
                        p_AnotherPass = cmdComando.Parameters("p_AnotherPass").Value.ToString()
                        p_Messaje = cmdComando.Parameters("p_Messaje").Value.ToString()
                        p_SolTarjeton = cmdComando.Parameters("p_SolTarjeton").Value.ToString()

                        If dr.HasRows Then
                            While dr.Read()
                                ' Leer primer cursor p_InfoTrsp
                                infoTrsp = New InfoTrsp

                                If dr.IsDBNull(0) Then
                                    infoTrsp.Trsp_Plate_Number = ""
                                Else
                                    infoTrsp.Trsp_Plate_Number = dr.GetString(0)
                                End If

                                If dr.IsDBNull(1) Then
                                    infoTrsp.Trsp_Trty_Type = ""
                                Else
                                    infoTrsp.Trsp_Trty_Type = dr.GetString(1)
                                End If

                                If dr.IsDBNull(2) Then
                                    infoTrsp.Trsp_Name = ""
                                Else
                                    infoTrsp.Trsp_Name = dr.GetString(2)
                                End If

                                If dr.IsDBNull(3) Then
                                    infoTrsp.Licencia = ""
                                Else
                                    infoTrsp.Licencia = dr.GetString(3)
                                End If

                                If dr.IsDBNull(4) Then
                                    infoTrsp.Tipo_Lic = ""
                                Else
                                    infoTrsp.Tipo_Lic = dr.GetString(4)
                                End If

                                If dr.IsDBNull(5) Then
                                    infoTrsp.Trsp_Driver_Name = ""
                                Else
                                    infoTrsp.Trsp_Driver_Name = dr.GetString(5)
                                End If

                                If dr.IsDBNull(6) Then
                                    infoTrsp.Trsp_Call_Date = ""
                                Else
                                    infoTrsp.Trsp_Call_Date = dr.GetDateTime(6)
                                End If

                                If dr.IsDBNull(7) Then
                                    infoTrsp.Tipo_Pass = ""
                                Else
                                    infoTrsp.Tipo_Pass = dr.GetString(7)
                                End If
                            End While

                            ' Checar si hay un segundo cursor
                            dr.NextResult()
                            ' Obtener nombres de columnas
                            For i = 0 To dr.FieldCount - 1
                                p_InfoCita.Columns.Add(dr.GetName(i))
                            Next i

                            If dr.HasRows Then
                                Dim j As Integer = 0
                                While (dr.Read())
                                    ' Leer segundo cursor p_Cursor
                                    Dim row As DataRow = p_InfoCita.NewRow()

                                    For i As Integer = 0 To dr.FieldCount - 1
                                        If dr.IsDBNull(i) Then
                                            row(i) = ""
                                        ElseIf IsDBNull(dr.GetValue(i)) Then
                                            row(i) = ""
                                        Else
                                            Try
                                                Dim value As String = dr.GetValue(i).ToString()
                                                row(i) = value
                                            Catch ex As Exception
                                                row(i) = "Error"
                                            End Try
                                        End If
                                    Next i

                                    p_InfoCita.Rows.Add(row)
                                    j = j + 1
                                End While
                            Else
                                ' Si no hay datos, llenar con cadenas vacias
                                Dim row As DataRow = p_InfoCita.NewRow()
                                For i As Integer = 0 To dr.FieldCount - 1
                                    row(i) = ""
                                Next i

                                p_InfoCita.Rows.Add(row)
                            End If
                        End If


                    End Using ' data reader

                    result.P_InfoTrsp = infoTrsp
                    result.P_Cursor = p_InfoCita
                    result.P_EnableVGM = p_EnableVGM
                    result.P_AnotherPass = p_AnotherPass
                    result.P_Messaje = p_Messaje
                    result.P_SolTarjeton = p_SolTarjeton
                End Using ' command

                Return result
            Catch oex As OracleException
                ' Agregamos detalles de porque falló la conexion a la bd
                Dim mensaje = String.Format("Error en la conexion a la base de datos. Error code: {0}. Mensaje: {1}. Detalles: {2}", oex.ErrorCode, oex.Message, oex.ToString())
                Debug.WriteLine(mensaje)

                ' Regresamos datos vacios 
                Throw
            Catch ex As Exception
                Dim mensaje = String.Format("Error al obtener información de apoyos. Mensaje: {0}. Detalles: {1}", ex.Message, ex.ToString())
                Debug.WriteLine(mensaje)

                ' Regresamos datos vacios 
                Throw
            End Try
        End Using
        ' --]
    End Function

    Public Function WriteFile(id As Integer) As String
        Dim path As String = String.Empty
        'Dim colBlob As Integer = 0 ' the column # of the BLOB field
        Dim name As String = String.Empty
        Using conn As New OracleConnection(dbContext.ObtenerCadenaConexion())
            Try
                conn.Open()

                Using cmd As New OracleCommand("SELECT PRSD_DOCUMENT_BINARY,PRSD_DOCUMENT_NAME ,PRSD_DOCUMENT_EXTENSION FROM CTS.PROGRAMED_SERVICES_DOCUMENTS where PRSD_DOCUMENT_ID = 3009", conn)

                    'Dim dr As OracleDataReader = cmd.ExecuteReader()
                    'dr.Read()
                    'Dim b(dr.GetBytes(colBlob, 0, Nothing, 0, Integer.MaxValue) - 1) As Byte
                    'dr.GetBytes(colBlob, 0, b, 0, b.Length)
                    'dr.Close()
                    'Dim fs As New System.IO.FileStream(path, IO.FileMode.Create, IO.FileAccess.Write)
                    'fs.Write(b, 0, b.Length)
                    'fs.Close()

                    Using dr As OracleDataReader = cmd.ExecuteReader()
                        dr.Read()
                        Dim bytes = DirectCast(dr("PRSD_DOCUMENT_BINARY"), Byte())
                        Dim extension As String = DirectCast(dr("PRSD_DOCUMENT_EXTENSION"), String)
                        name = DirectCast(dr("PRSD_DOCUMENT_NAME"), String)

                        path = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/" + name)

                        Dim fs As New System.IO.FileStream(path, IO.FileMode.Create, IO.FileAccess.Write)
                        fs.Write(bytes, 0, bytes.Length)
                        fs.Close()
                    End Using

                    conn.Close()

                    path = "~/App_Data/" + name

                    Return path
                End Using
            Catch ex As Exception
                Dim menesaje As String = ex.Message

                Debug.WriteLine(String.Format("Error al leer archivo. Mensaje: {0}. Detalles: {1}", ex.Message, ex.ToString()))

                Throw
            End Try
        End Using
    End Function

    Public Function ReadFile(prsd_Document_Id As Integer) As RequestDocument
        Dim document As New RequestDocument

        Using conn As New OracleConnection(dbContext.ObtenerCadenaConexion())
            Try
                conn.Open()

                ' id image jpg: 5748
                ' id pdf file: 3009

                Dim query As String = String.Format("SELECT PRSD_DOCUMENT_BINARY,PRSD_DOCUMENT_NAME ,PRSD_DOCUMENT_EXTENSION FROM CTS.PROGRAMED_SERVICES_DOCUMENTS where PRSD_DOCUMENT_ID = {0}", prsd_Document_Id)

                Using cmd As New OracleCommand(query, conn)

                    Using dr As OracleDataReader = cmd.ExecuteReader()
                        dr.Read()
                        document.File = DirectCast(dr("PRSD_DOCUMENT_BINARY"), Byte())
                        document.P_DocumentName = DirectCast(dr("PRSD_DOCUMENT_NAME"), String)
                        document.P_DocumentExtension = DirectCast(dr("PRSD_DOCUMENT_EXTENSION"), String)
                    End Using

                    conn.Close()

                    Return document
                End Using
            Catch ex As Exception
                Dim menesaje As String = ex.Message

                Debug.WriteLine(String.Format("Error al leer archivo. Mensaje: {0}. Detalles: {1}", ex.Message, ex.ToString()))

                Throw
            End Try
        End Using

    End Function

    'Private Sub SqlBlob2File(ByVal DestFilePath As String)
    '    Dim PictureCol As Integer = 0 ' the column # of the BLOB field
    '    Dim cn As New SqlConnection("server=localhost;integrated security=yes;database=NorthWind")
    '    Dim cmd As New SqlCommand("SELECT Picture FROM Categories WHERE CategoryName='Test'", cn)
    '    cn.Open()
    '    Dim dr As SqlDataReader = cmd.ExecuteReader()
    '    dr.Read()
    '    Dim b(dr.GetBytes(PictureCol, 0, Nothing, 0, Integer.MaxValue) - 1) As Byte
    '    dr.GetBytes(PictureCol, 0, b, 0, b.Length)
    '    dr.Close()
    '    cn.Close()
    '    Dim fs As New System.IO.FileStream(DestFilePath, IO.FileMode.Create, IO.FileAccess.Write)
    '    fs.Write(b, 0, b.length)
    '    fs.Close()
    'End Sub

    ''' <summary>
    ''' Obtiene el catálogo de incidentes
    ''' </summary>
    ''' <returns>Catalogo de incidentes</returns>
    Public Function GetIncidentCatalog() As List(Of Incident)
        ' Variable para regresar ambos datatables
        Dim catalogoIncidentes As New List(Of Incident)

        Using conn As New OracleConnection(dbContext.ObtenerCadenaConexion())
            Try
                conn.Open()

                Using cmdComando As New OracleCommand("CTS.PK_CTS.p_GetIncidentCatalog", conn)

                    cmdComando.CommandType = CommandType.StoredProcedure

                    'Parametros de Salida
                    cmdComando.Parameters.Add("p_Cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output

                    Using dr As OracleDataReader = cmdComando.ExecuteReader()
                        If dr.HasRows Then
                            While dr.Read()
                                ' Leer cursor p_Cursor
                                Dim incidente As New Incident

                                If dr.IsDBNull(0) Then
                                    incidente.Cict_Id = ""
                                Else
                                    incidente.Cict_Id = dr.GetInt32(0)
                                End If

                                If dr.IsDBNull(1) Then
                                    incidente.Cict_Name = ""
                                Else
                                    incidente.Cict_Name = dr.GetString(1)
                                End If

                                catalogoIncidentes.Add(incidente)
                            End While
                        End If
                    End Using ' data reader
                End Using ' command

                Return catalogoIncidentes
            Catch oex As OracleException
                ' Agregamos detalles de porque falló la conexion a la bd
                Dim mensaje = String.Format("Error en la conexion a la base de datos al obtener el catalogo de incidentes. Error code: {0}. Mensaje: {1}. Detalles: {2}", oex.ErrorCode, oex.Message, oex.ToString())
                Debug.WriteLine(mensaje)

                ' Cachamos excepcion 
                Throw
            Catch ex As Exception
                Dim mensaje = String.Format("Error al obtener el catalogo de incidentes. Mensaje: {0}. Detalles: {1}", ex.Message, ex.ToString())
                Debug.WriteLine(mensaje)

                ' Cachamos excepcion 
                Throw
            End Try
        End Using
    End Function
End Class
