Imports Oracle.ManagedDataAccess.Client

Public Class PEDBContext

    ''' <summary>
    ''' Obtiene la conexion de la base de datos de TIMSA
    ''' </summary>
    ''' <returns>Conexion de oracle (OracleConnection)</returns>
    ''' <remarks></remarks>
    Public Function ObtenerConexion() As OracleConnection
        ' Obtener cadena de conexion de TIMSA
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("ConnectionTIMSA").ConnectionString

        Return New OracleConnection(connectionString)
    End Function

    Public Function ObtenerCadenaConexion() As String
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("ConnectionTIMSA").ConnectionString

        Return connectionString
    End Function
End Class
