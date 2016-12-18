<%@ Page Title="Ingreso de Unidades" Language="VB" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="PuertaEntrada.aspx.vb" Inherits="Puerta_Entrada.PuertaEntrada" %>

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
    <script type="text/javascript" src='Scripts/puerta-entrada.js'></script>
</asp:content>

<asp:Content ID="PuertaEntradaContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1 class="text-center">Puerta de Entrada - Ingreso de Unidades</h1>
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
                    <h4 class="col-md-2">Transporte</h4>
                    <label for="folio" class="col-md-1 control-label">Folio</label>
                    <div class="col-md-2">
                        <input type="text" data-validation-error-msg="El campo folio debe ser un valor numérico" data-validation="number" class="form-control" required id="txtFolio" runat="server" placeholder="Folio">
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
                    <div class="col-md-3">
                        <asp:Button Text="Buscar" ID="btnBuscar" CssClass="btn btn-default" runat="server" OnClick="Buscar" />
                    </div>
                </div>
                 <asp:Label ID="lblFolioValido" runat="server" Text="" Visible="false"></asp:Label>
                 <asp:Label ID="lblFechaValida" runat="server" Text="" Visible="false"></asp:Label>
                <div class="form-group">
                    <label for="placas" class="col-md-1 control-label">Placas</label>
                    <div class="col-md-2">
                        <input type="text" class="form-control" runat="server" readonly id="txtPlacas">
                    </div>
                    <label for="tipoTransporte" class="col-md-2 control-label">Tipo de Transporte</label>
                    <div class="col-md-2">
                        <input type="text" class="form-control" runat="server" readonly id="txtTipoTrasporte" >
                    </div>
                    <label for="nombreTransportista" class="col-md-2 control-label">Nombre Transportista</label>
                    <div class="col-md-3">
                        <input type="text" class="form-control" runat="server" readonly id="txtNombreTransportista" >
                    </div>
                </div>

                <div class="form-group">
                    <label for="licencia" class="col-md-1 control-label">Licencia</label>
                    <div class="col-md-2">
                        <input type="text" class="form-control" runat="server" readonly id="txtLicencia" >
                    </div>
                    <label for="tipoLicencia" class="col-md-2 control-label" >Tipo de Licencia</label>
                    <div class="col-md-2">
                        <input type="text" class="form-control" runat="server" readonly id="txtTipoLicencia">
                    </div>
                    <label for="operador" class="col-md-2 control-label">Operador</label>
                    <div class="col-md-3">
                        <input type="text" class="form-control" runat="server" readonly id="txtOperador">
                    </div>
                </div>

                <div class="form-group">
                    <label for="horaLlamado" class="col-md-1 control-label">Hora de Llamado</label>
                    <div class="col-md-2">
                        <input type="text" class="form-control" runat="server" readonly id="txtHoraLlamado">
                    </div>
                    <label for="tipoPaseEntrada" class="col-md-2 control-label">Tipo de Pase de Entrada</label>
                    <div class="col-md-3">
                        <input type="text" class="form-control" runat="server" readonly id="txtTipoPaseEntrada">
                    </div>
                    <div class="col-md-offset-1 col-md-3">
                        <input type="button" value="Registrar Discrepancia" OnServerClick="AbrirModalRegistrar" runat="server" class="btn btn-default" id="btnRegistrarDiscrepancia" />
                    </div>
                </div>

            </div>
        </div>
        <asp:Label ID="lblP_EnableVGM" runat="server" Text="" Visible="false"></asp:Label>
        <asp:Label ID="lblP_SolTarjeton" runat="server" Text="" Visible="false"></asp:Label>

        <%-- Tabla dinámica ingreso de unidades --%>
        <div class="row">
            <div class="col-md-12 table-responsive">
                <asp:GridView ID="gridIngresoUnidades" AutoGenerateColumns="True" runat="server" CssClass="table table-bordered grid-table">
                    <%--<HeaderStyle CssClass="color-well-gray" />--%>
                </asp:GridView>
            </div>
        </div>
        <br />
        <div class="row">
            <div class="col-md-2 col-md-offset-5">
                <input type="button" value="Permitir Ingreso" OnServerClick="PermitirIngreso" runat="server" class="btn btn-default" id="btnPermitirIngreso" />
            </div>
        </div>
    </div>
     <!-- Open File Modal -->
    <div class="modal fade" id="modalFile" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <asp:UpdatePanel ID="upPanelFile" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-content modal-limit-file">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                            <h4 class="modal-title">
                                <asp:Label ID="lblNombreArchivo" runat="server" Text="Archivo"></asp:Label>
                            </h4>
                        </div>
                        <div class="modal-body">
                            <div class="row">
                                <div class="col-md-12">
                                    <asp:Literal ID="ltEmbed" runat="server" Visible="false" />
                                    <asp:Image ID="imageArchivo" runat="server" Visible="false"/>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Cerrar</button>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <!-- Modal Registrar Discrepancia -->
    <div class="modal fade" id="modalRegistrarDiscrepancia" role="dialog" aria-hidden="true">
        <div class="modal-dialog">
            <asp:UpdatePanel ID="upModal" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                            <h4 class="modal-title">
                                <asp:Label ID="lblModalTitle" runat="server" Text="Envio de correo por discrepancias en transporte"></asp:Label>
                            </h4>
                        </div>
                        <div class="modal-body">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <label for="comboTipoDiscrepancia" class="col-md-4 control-label">Tipo de Discrepancia</label>
                                    <div class="col-md-8">
                                        <%--<input type="text" class="form-control" id="comboTipoDiscrepancia" runat="server">--%>
                                        <asp:DropDownList id="comboTipoDiscrepancia" CssClass="form-control" runat="server"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <label for="txtComentarios" class="col-md-4 control-label">Comentarios adicionales</label>
                                    <div class="col-md-8">
                                        <textarea class="form-control text-limit-width" id="txtComentarios" runat="server"></textarea>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <input type="button" value="Enviar" OnServerClick="RegistrarDiscrepancia" runat="server" class="btn btn-default" id="btnEnvDiscrepancia" />
                            <button type="button" class="btn btn-default" data-dismiss="modal">Cancelar</button>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
     <!-- Modal Tarjeton -->
    <div class="modal fade" id="modalTarjeton" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <asp:UpdatePanel ID="upModalTarjeton" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                            <h4 class="modal-title">
                                <asp:Label ID="lblModalTitTarjeton" runat="server" Text="Tarjeton"></asp:Label>
                            </h4>
                        </div>
                        <div class="modal-body">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <label for="txtNoTarjeton" class="col-md-4 control-label">Número de Tarjetón</label>
                                    <div class="col-md-4">
                                        <input type="text" ClientIDMode="Static" data-validation-error-msg="El campo número de tarjetón debe ser numérico" data-validation="number" class="form-control" id="txtNoTarjeton" runat="server">
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:LinkButton CssClass="btn btn-default" Text="Continuar" runat="server" OnClick="IngresarUnidad" OnClientClick="return validarModalTarjeton();" ID="btnContinuarIngresoT" ></asp:LinkButton>
                            <button type="button" class="btn btn-default" data-dismiss="modal">Cancelar</button>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
