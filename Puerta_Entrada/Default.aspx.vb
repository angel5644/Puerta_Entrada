Public Class _Default
    Inherits Page
    Private _puertaEntradaService As New PuertaEntradaService
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

    End Sub

    ''' <summary>
    ''' Handle files requests
    ''' </summary>
    ''' <param name="context"></param>
    ''' <remarks></remarks>
    Public Overrides Sub ProcessRequest(context As HttpContext)
        Dim idParam As String = context.Request.QueryString("Id")

        If Not String.IsNullOrEmpty(idParam) Then
            Dim id As Integer = Integer.Parse(context.Request.QueryString("Id"))
            Dim fileType As String = context.Request.QueryString("fileType")
            Dim download As String = context.Request.QueryString("download")

            Dim doc As RequestDocument = _puertaEntradaService.ReadFile(id)

            context.Response.Buffer = True
            context.Response.Charset = ""
            If download = "1" Then
                context.Response.AppendHeader("Content-Disposition", Convert.ToString("attachment; filename=") & doc.P_DocumentName)
            End If
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache)
            If fileType.Contains("image") Then
                context.Response.ContentType = fileType
            ElseIf fileType = "pdf" Then
                context.Response.ContentType = "application/pdf"
            End If

            context.Response.BinaryWrite(doc.File)
            context.Response.Flush()
            context.Response.[End]()
        End If
    End Sub
End Class