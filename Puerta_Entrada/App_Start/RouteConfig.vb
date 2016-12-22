Imports System.Web.Routing
Imports Microsoft.AspNet.FriendlyUrls

Public Module RouteConfig
    Sub RegisterRoutes(ByVal routes As RouteCollection)
        Dim settings As FriendlyUrlSettings = New FriendlyUrlSettings()
        settings.AutoRedirectMode = RedirectMode.Permanent
        'routes.EnableFriendlyUrls(settings)
        ' Adding custom url handler
        routes.EnableFriendlyUrls(settings, New Microsoft.AspNet.FriendlyUrls.Resolvers.IFriendlyUrlResolver() {New MyWebFormsFriendlyUrlResolver()})
    End Sub
End Module
