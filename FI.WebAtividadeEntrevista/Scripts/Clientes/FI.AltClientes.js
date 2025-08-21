$(document).ready(function () {
  if (obj) {
    $("#formCadastro #Nome").val(obj.Nome);
    $("#formCadastro #CEP").val(obj.CEP);
    $("#formCadastro #Email").val(obj.Email);
    $("#formCadastro #Sobrenome").val(obj.Sobrenome);
    $("#formCadastro #Nacionalidade").val(obj.Nacionalidade);
    $("#formCadastro #Estado").val(obj.Estado);
    $("#formCadastro #Cidade").val(obj.Cidade);
    $("#formCadastro #Logradouro").val(obj.Logradouro);
    $("#formCadastro #Telefone").val(obj.Telefone);

    if (obj.CPF) {
      var cpfFormatted = obj.CPF.replace(
        /(\d{3})(\d{3})(\d{3})(\d{2})/,
        "$1.$2.$3-$4"
      );
      $("#formCadastro #CPF").val(cpfFormatted);
    }

    if (obj.Beneficiarios && obj.Beneficiarios.length > 0) {
      beneficiarios = obj.Beneficiarios.map(function (b) {
        return {
          CPF: b.CPF,
          Nome: b.Nome,
        };
      });
      atualizarTabelaBeneficiarios();
    }
  }

  $("#formCadastro").submit(function (e) {
    e.preventDefault();

    var formData = {
      NOME: $(this).find("#Nome").val(),
      CEP: $(this).find("#CEP").val(),
      Email: $(this).find("#Email").val(),
      Sobrenome: $(this).find("#Sobrenome").val(),
      Nacionalidade: $(this).find("#Nacionalidade").val(),
      Estado: $(this).find("#Estado").val(),
      Cidade: $(this).find("#Cidade").val(),
      Logradouro: $(this).find("#Logradouro").val(),
      Telefone: $(this).find("#Telefone").val(),
      CPF: $(this).find("#CPF").val().replace(/\D/g, ""),
      Id: obj ? obj.Id : 0,
    };

    if (typeof beneficiarios !== "undefined" && beneficiarios.length > 0) {
      for (var i = 0; i < beneficiarios.length; i++) {
        formData["Beneficiarios[" + i + "].CPF"] = beneficiarios[i].CPF;
        formData["Beneficiarios[" + i + "].Nome"] = beneficiarios[i].Nome;
      }
    }

    $.ajax({
      url: urlPost,
      method: "POST",
      data: formData,
      error: function (r) {
        if (r.status == 400) ModalDialog("Ocorreu um erro", r.responseJSON);
        else if (r.status == 500)
          ModalDialog(
            "Ocorreu um erro",
            "Ocorreu um erro interno no servidor."
          );
      },
      success: function (r) {
        ModalDialog("Sucesso!", r);
        $("#formCadastro")[0].reset();
        window.location.href = urlRetorno;
      },
    });
  });
});

function ModalDialog(titulo, texto) {
  var random = Math.random().toString().replace(".", "");
  var texto =
    '<div id="' +
    random +
    '" class="modal fade">                                                               ' +
    '        <div class="modal-dialog">                                                                                 ' +
    '            <div class="modal-content">                                                                            ' +
    '                <div class="modal-header">                                                                         ' +
    '                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>         ' +
    '                    <h4 class="modal-title">' +
    titulo +
    "</h4>                                                    " +
    "                </div>                                                                                             " +
    '                <div class="modal-body">                                                                           ' +
    "                    <p>" +
    texto +
    "</p>                                                                           " +
    "                </div>                                                                                             " +
    '                <div class="modal-footer">                                                                         ' +
    '                    <button type="button" class="btn btn-default" data-dismiss="modal">Fechar</button>             ' +
    "                                                                                                                   " +
    "                </div>                                                                                             " +
    "            </div><!-- /.modal-content -->                                                                         " +
    "  </div><!-- /.modal-dialog -->                                                                                    " +
    "</div> <!-- /.modal -->                                                                                        ";

  $("body").append(texto);
  $("#" + random).modal("show");
}
