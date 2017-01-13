$(document).ready(function () {
    // Mover reloj al cargar la pagina
    mueveReloj();

    // Init jquery validator
    $.validate({
        //modules : 'toggleDisabled'
    });

    $('#fechaCita').datetimepicker({
        format: 'DD/MM/YYYY HH:mm',
        locale: "es"

    });

    // Inicializar datepicker
    //$('.datepicker').datepicker({
    //    format: 'dd/mm/yyyy HH/mm',
    //    todayHighlight: true,
    //    language: 'es',
    //    //startDate: '-3d' // Restringir fecha en el pasado
    //}).off('focus')
    //          .click(function () {
    //              $(this).datepicker('show');
    //          });

    //$('.open-datepicker').click(function (event) {

    //    $(".datepicker").datepicker('show');

    //    //return false;
    //});
});

$(".txtFolio").on("change", function () {
    // Borrar placa
    $(".txtPlaca").val("")
});

$(".txtPlaca").on("change", function () {
    // Borrar folio
    $(".txtFolio").val("")
});

function borrarPlaca() {
    // Borrar placa
    $(".txtPlaca").val("")
    //$(".txtPlaca").focus()
}

function borrarFolio() {
    // Borrar folio
    $(".txtFolio").val("")
    $(".txtFolio").focus()
    $(".txtPlaca").focus()
    //$(".txtFolio").focusout()
}

function mueveReloj() {
    momentoActual = new Date()

    var f = new Date();
    var ano = f.getFullYear();
    var mes = f.getMonth();
    var dia = f.getDate();
    var diasem = f.getDay();
    var estiloDia;
    var meses = new Array("Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre");
    var diasSemana = new Array("Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado");
    var diasMes = new Array(31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31);
    var diaMaximo = diasMes[mes];
    if (mes == 1 && (((ano % 4 == 0) && (ano % 100 != 0)) || (ano % 400 == 0)))
        diaMaximo = 29;

    hora = momentoActual.getHours()
    minuto = momentoActual.getMinutes()
    segundo = momentoActual.getSeconds()

    horaImprimible = diasSemana[diasem] + ' ' + dia + ' de ' + meses[mes] + ' de ' + ano + '  ' + hora + " : " + minuto + " : " + segundo

    document.getElementById('lblHora').innerHTML = horaImprimible

    setTimeout("mueveReloj()", 1000)
}

function reloadPage() {
    window.location.reload()
}

function loadCurrentPage() {
    window.location.assign(window.location.href)
}

