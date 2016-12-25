<%@ Page Title="Cancelar Pase" Language="VB" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="CancelacionEntrada.aspx.vb" Inherits="Puerta_Entrada.CancelacionEntrada" %>

<asp:content contentplaceholderid="head" runat="server">
    <%-- Estilos --%>
    <link rel="stylesheet" href="Content/puerta-entrada.css" type="text/css" />
    <link rel="stylesheet" href="Content/bootstrap_datepicker/bootstrap-datepicker.min.css" type="text/css" />

    <%-- Javascript --%>
    <script type="text/javascript" src='Scripts/jquery-1.10.2.min.js'></script>
    <script type="text/javascript" src='Scripts/bootstrap_datepicker/bootstrap-datepicker.min.js'></script>
    <script type="text/javascript" src='Scripts/bootstrap_datepicker/bootstrap-datepicker.es.min.js'></script>
    
    <script src="https://use.fontawesome.com/14d84931fc.js"></script>
    <script type="text/javascript" src='Scripts/jquery.form-validator.min.js'></script>
    <script type="text/javascript" src='Scripts/cancelacion-entrada.js'></script>
</asp:content>

<asp:Content ID="CancelarPaseContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1 class="text-center">Puerta de Entrada - Cancelar Pase</h1>
    <hr />
    <div class="container-fluid">
        <asp:UpdatePanel ID="upPanelMensajes" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="divErrorPuertaEntrada" class="alert alert-danger" runat="server" visible="false">
                    <button type="button" class="close" data-dismiss="alert">&times;</button>
                    <asp:Label ID="msgError" runat="server" Text=""></asp:Label>
                    <br />
                </div>

                <div id="divSuccessPuertaEntrada" class="alert alert-success" runat="server" visible="false">
                    <button type="button" class="close" data-dismiss="alert">&times;</button>
                    <asp:Label ID="msgSuccess" runat="server" Text=""></asp:Label>
                    <br />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        
        <%-- Formulario búsqueda --%>
        <div class="well">
            <div class="form-horizontal">
                <div class="form-group">
                    <h4 class="col-md-1">Transporte</h4>
                    <label for="folio" class="col-md-1 control-label">Folio</label>
                    <div class="col-md-2">
                        <input type="text" data-validation-optional="true" data-validation-error-msg="El campo folio debe ser un valor numérico" data-validation="number" onchange="borrarPlaca()" class="form-control txtFolio" id="txtFolio" runat="server" placeholder="Folio">
                    </div>
                    <label for="placa" class="col-md-1 control-label">Placa</label>
                    <div class="col-md-2">
                        <input type="text" class="form-control txtPlaca" id="txtPlaca" onchange="borrarFolio()" runat="server" placeholder="Placa">
                    </div>
                    <div class="col-md-4">
                        <div class="row">
                            <label for="fechaCita" class="col-md-4 control-label">Fecha cita</label>
                            <div class="col-md-6" id="controlFechaCita">
                                <div class="input-group">
                                    <input type="text" class="form-control datepicker" required id="txtFechaCita" runat="server" placeholder="dd/mm/yyyy">
                                    <span class="input-group-btn">
                                        <button class="btn btn-default open-datepicker" type="button"><i class="glyphicon glyphicon-calendar"></i></button>
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-1">
                        <asp:Button Text="Buscar" ID="btnBuscar" CssClass="btn btn-default pull-left" runat="server" OnClick="Buscar" />
                    </div>
                </div>
                <asp:Label ID="lblFolioValido" runat="server" Text="" Visible="false"></asp:Label>
                <asp:Label ID="lblFechaValida" runat="server" Text="" Visible="false"></asp:Label>
                <asp:Label ID="lblPlacaValida" runat="server" Text="" Visible="false"></asp:Label>
                <div class="form-group">
                    <label for="txtPlacas" class="col-md-1 control-label">Placas</label>
                    <div class="col-md-2">
                        <input type="text" class="form-control" runat="server" readonly id="txtPlacas">
                    </div>
                    <label for="txtOperador" class="col-md-1 control-label">Operador</label>
                    <div class="col-md-4">
                        <input type="text" class="form-control" runat="server" readonly id="txtOperador">
                    </div>
                    <label for="txtTelefono" class="col-md-2 control-label">Teléfono Celular</label>
                    <div class="col-md-2">
                        <input type="text" class="form-control" runat="server" readonly id="txtTelefono">
                    </div>
                </div>
            </div>
        </div>

        <%-- Tabla dinámica ingreso de unidades --%>
        <div class="row">
            <div class="col-md-12 table-responsive">
                <asp:GridView ID="gridCancelarUnidades" AutoGenerateColumns="True" runat="server" CssClass="table table-bordered grid-table">
                    <%--<HeaderStyle CssClass="color-well-gray" />--%>
                </asp:GridView>
            </div>
        </div>
        <br />
        <div class="row">
            <div class="col-md-2 col-md-offset-5">
                <input type="button" value="Cancelar Pase" OnServerClick="AbrirModalCancelar" runat="server" class="btn btn-default" id="btnCancelarPase" />
            </div>
        </div>
    </div>
    
     <!-- Modal Tarjeton -->
    <div class="modal fade" id="modalCancelar" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <asp:UpdatePanel ID="upModalCancelar" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                            <h4 class="modal-title">
                                <asp:Label ID="lblTitModalCancelar" runat="server" Text="Cancelar Pase"></asp:Label>
                            </h4>
                        </div>
                        <div class="modal-body">
                            <p>Realmente desea cancelar el pase de entrada <asp:Label ID="lblNombrePase" runat="server"></asp:Label> de la fecha <asp:Label ID="lblFechaPase" runat="server"></asp:Label></p>
                        </div>
                        <div class="modal-footer">
                            <asp:LinkButton CssClass="btn btn-default" Text="Continuar" runat="server" OnClick="CancelarPase" ID="btnContinuarCancelar" ></asp:LinkButton>
                            <button type="button" class="btn btn-default" data-dismiss="modal">Regresar</button>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
