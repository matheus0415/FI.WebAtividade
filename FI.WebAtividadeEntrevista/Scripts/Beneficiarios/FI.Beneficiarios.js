var contadorBeneficiarios = 0;
var beneficiarios = [];

function mascaraCPFBeneficiario(cpf) {
  cpf = cpf.replace(/\D/g, "");
  cpf = cpf.replace(/(\d{3})(\d)/, "$1.$2");
  cpf = cpf.replace(/(\d{3})(\d)/, "$1.$2");
  cpf = cpf.replace(/(\d{3})(\d{1,2})$/, "$1-$2");
  return cpf;
}

$(document).ready(function () {
  $("#modalCPFBeneficiario").on("input", function (e) {
    e.target.value = mascaraCPFBeneficiario(e.target.value);
  });

  $("#btnIncluirBeneficiario").on("click", function () {
    incluirBeneficiario();
  });

  $("#modalCPFBeneficiario, #modalNomeBeneficiario").on(
    "keypress",
    function (e) {
      if (e.which === 13) {
        incluirBeneficiario();
      }
    }
  );

  $("#modalBeneficiario").on("shown.bs.modal", function () {
    atualizarTabelaBeneficiarios();
    $("#modalCPFBeneficiario").focus();
  });

  $("#modalBeneficiario").on("hidden.bs.modal", function () {
    if (
      !$("#modalCPFBeneficiario").val() &&
      !$("#modalNomeBeneficiario").val()
    ) {
      $("#modalCPFBeneficiario").val("");
      $("#modalNomeBeneficiario").val("");
    }
  });

  $("#modalCPFBeneficiario").on("blur", function () {
    var cpf = $(this).val().trim();
    if (cpf) {
      validarCPFEmTempoReal(cpf);
    }
  });
});

function incluirBeneficiario() {
  var cpf = $("#modalCPFBeneficiario").val().trim();
  var nome = $("#modalNomeBeneficiario").val().trim();

  if (!cpf) {
    alert("Por favor, informe o CPF do beneficiário.");
    $("#modalCPFBeneficiario").focus();
    return;
  }

  if (!nome) {
    alert("Por favor, informe o nome do beneficiário.");
    $("#modalNomeBeneficiario").focus();
    return;
  }

  var cpfLimpo = cpf.replace(/\D/g, "");
  if (cpfLimpo.length !== 11) {
    alert("Por favor, informe um CPF válido com 11 dígitos.");
    $("#modalCPFBeneficiario").focus();
    return;
  }

  if (verificarCPFDuplicado(cpfLimpo)) {
    alert("Este CPF já foi adicionado como beneficiário para este cliente.");
    $("#modalCPFBeneficiario").focus();
    return;
  }

  var cpfCliente = $("#CPF").val();
  if (cpfCliente) {
    var cpfClienteLimpo = cpfCliente.replace(/\D/g, "");
    if (cpfLimpo === cpfClienteLimpo) {
      alert(
        "O CPF do beneficiário não pode ser igual ao CPF do cliente titular."
      );
      $("#modalCPFBeneficiario").focus();
      return;
    }
  }

  contadorBeneficiarios++;
  var beneficiario = {
    id: contadorBeneficiarios,
    cpf: cpf,
    cpfLimpo: cpfLimpo,
    nome: nome,
  };

  beneficiarios.push(beneficiario);
  atualizarTabelaBeneficiarios();

  $("#modalCPFBeneficiario").val("");
  $("#modalNomeBeneficiario").val("");
  $("#modalCPFBeneficiario").focus();

  atualizarCamposHidden();
}

function atualizarTabelaBeneficiarios() {
  var tbody = $("#tabelaBeneficiarios");
  var msgSemBeneficiarios = $("#msgSemBeneficiarios");

  tbody.empty();

  if (beneficiarios.length === 0) {
    msgSemBeneficiarios.show();
  } else {
    msgSemBeneficiarios.hide();

    $.each(beneficiarios, function (index, beneficiario) {
      var cpfFormatado = formatarCPF(beneficiario.cpfLimpo);
      var linha = `
                <tr id="linhaBeneficiario-${beneficiario.id}">
                    <td>${cpfFormatado}</td>
                    <td>${beneficiario.nome}</td>
                    <td>
                        <button type="button" class="btn btn-primary btn-xs" onclick="alterarBeneficiario(${beneficiario.id})" title="Alterar beneficiário">
                            Alterar
                        </button>
                        <button type="button" class="btn btn-danger btn-xs" onclick="excluirBeneficiario(${beneficiario.id})" title="Excluir beneficiário" style="margin-left: 5px;">
                            Excluir
                        </button>
                    </td>
                </tr>
            `;
      tbody.append(linha);
    });
  }
}

function formatarCPF(cpf) {
  if (cpf && cpf.length === 11) {
    return cpf.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, "$1.$2.$3-$4");
  }
  return cpf;
}

function alterarBeneficiario(id) {
  var beneficiario = beneficiarios.find((b) => b.id === id);
  if (beneficiario) {
    $("#modalCPFBeneficiario").val(beneficiario.cpf);
    $("#modalNomeBeneficiario").val(beneficiario.nome);

    beneficiarios = beneficiarios.filter((b) => b.id !== id);

    atualizarTabelaBeneficiarios();
    atualizarCamposHidden();
    $("#modalCPFBeneficiario").focus();
  }
}

function excluirBeneficiario(id) {
  var beneficiario = beneficiarios.find((b) => b.id === id);
  if (
    beneficiario &&
    confirm(
      `Tem certeza que deseja excluir o beneficiário ${beneficiario.nome}?`
    )
  ) {
    beneficiarios = beneficiarios.filter((b) => b.id !== id);
    atualizarTabelaBeneficiarios();
    atualizarCamposHidden();
  }
}

function atualizarCamposHidden() {
  $('input[name*="Beneficiarios"]').remove();

  $.each(beneficiarios, function (index, beneficiario) {
    var inputNome = `<input type="hidden" name="Beneficiarios[${index}].Nome" value="${beneficiario.nome}">`;
    var inputCPF = `<input type="hidden" name="Beneficiarios[${index}].CPF" value="${beneficiario.cpfLimpo}">`;

    $("#formCadastro").append(inputNome).append(inputCPF);
  });

  atualizarListaPrincipal();
}

function atualizarListaPrincipal() {
  var container = $("#listaBeneficiarios");
  container.empty();

  if (beneficiarios.length > 0) {
    var plural = beneficiarios.length === 1 ? "" : "s";
    var resumo = `
            <div class="alert alert-info">
                <i class="glyphicon glyphicon-info-sign"></i> 
                <strong>${beneficiarios.length}</strong> beneficiário${plural} adicionado${plural}.
                <button type="button" class="btn btn-xs btn-default pull-right" data-toggle="modal" data-target="#modalBeneficiario">
                    <i class="glyphicon glyphicon-edit"></i> Gerenciar Beneficiários
                </button>
                <div class="clearfix"></div>
            </div>
        `;
    container.html(resumo);
  }
}

function verificarCPFDuplicado(cpfLimpo) {
  return beneficiarios.some((b) => b.cpfLimpo === cpfLimpo);
}

function validarCPFEmTempoReal(cpf) {
  var $campo = $("#modalCPFBeneficiario");
  var cpfLimpo = cpf.replace(/\D/g, "");

  $campo.removeClass("is-valid is-invalid");

  if (cpfLimpo.length === 11) {
    if (verificarCPFDuplicado(cpfLimpo)) {
      $campo.addClass("is-invalid");
      mostrarTooltip($campo, "CPF já cadastrado para este cliente");
      return false;
    }

    var cpfCliente = $("#CPF").val();
    if (cpfCliente) {
      var cpfClienteLimpo = cpfCliente.replace(/\D/g, "");
      if (cpfLimpo === cpfClienteLimpo) {
        $campo.addClass("is-invalid");
        mostrarTooltip($campo, "CPF não pode ser igual ao do titular");
        return false;
      }
    }

    $campo.addClass("is-valid");
    ocultarTooltip($campo);
    return true;
  } else if (cpfLimpo.length > 0) {
    $campo.addClass("is-invalid");
    mostrarTooltip($campo, "CPF deve ter 11 dígitos");
    return false;
  }

  return true;
}

function mostrarTooltip($elemento, mensagem) {
  $elemento.attr("title", mensagem).tooltip("show");
}

function ocultarTooltip($elemento) {
  $elemento.removeAttr("title").tooltip("hide");
}
