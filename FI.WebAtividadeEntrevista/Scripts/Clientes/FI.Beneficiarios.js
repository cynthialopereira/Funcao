
$(document).ready(function () {
    $('#formBeneficiario #CpfBeneficiario').mask('000.000.000-00', { placeholder: "999.999.999-99" });
    PopularTabela();
})

    function InserirBeneficiarioJS() {
        
        var cpf = document.getElementById('CpfBeneficiario');
        var nome = document.getElementById('NomeBeneficiario');
        var mensagem = document.getElementById('Mensagem');

        mensagem.innerText = "";

        if (cpf.value != '' && nome.value != '') {
            var request = {
                cpf: cpf.value,
                nome: nome.value
            };

            var jsonRequest = JSON.stringify(request);

            $.ajax({
                type: "Post",
                url: "/Cliente/InserirBeneficiario",
                data: jsonRequest,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data == "OK") {
                        cpf.value = '';
                        nome.value = '';
                        PopularTabela();

                    }
                    else {
                        mensagem.innerText = data;
                    }


                },
                error: function (xhr, status, error) {
                    alert(status);
                }
            });
        }
        else {
            mensagem.innerText = "Nome e CPF são obrigatórios.";
        }
    }

function ExcluirBeneficiarioJS(posicao) {

    var mensagem = document.getElementById('Mensagem');

    mensagem.innerText = "";

    var request = {
        posicao: posicao
    };

    var jsonRequest = JSON.stringify(request);

    $.ajax({
        type: "Post",
        url: "/Cliente/ExcluirBeneficiario",
        data: jsonRequest,
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data == "OK") {
                PopularTabela();
            }
            else {
                mensagem.innerText = data;
            }
        },
        error: function (xhr, status, error) {
            alert(status);
        }
    });
}

function LimparBeneficiarios()
{
    var mensagem = document.getElementById('Mensagem');
    var cpf = document.getElementById('CpfBeneficiario');
    var nome = document.getElementById('NomeBeneficiario');

    mensagem.innerText = "";
    cpf.value = "";
    nome.value = "";
    PopularTabela();
}

function AlterarBeneficiarioJS(posicao, cpfAntigo, nomeAntigo) {

    var mensagem = document.getElementById('Mensagem');
    var cpf = document.getElementById('CpfBeneficiario');
    var nome = document.getElementById('NomeBeneficiario');

    mensagem.innerText = "";

    cpf.value = cpfAntigo;
    nome.value = nomeAntigo;

    ExcluirBeneficiarioJS(posicao);

    mensagem.innerText = "";


}

    function PopularTabela() {
        if (document.getElementById("gridBeneficiarios"))
            $('#gridBeneficiarios').jtable({
                title: 'Beneficiários',
                paging: false, //Enable paging
                pageSize: 20, //Set page size (default: 10)
                sorting: false, //Enable sorting
                defaultSorting: 'Nome', //Set default sorting
                actions: {
                    listAction: "/Cliente/PopularBeneficiarios",
                },
                fields: {
                    Cpf: {
                        title: 'CPF',
                        width: '35%'
                    },
                    Nome: {
                        title: 'Nome',
                        width: '50%'
                    },
                    Alterar: {
                        title: '',
                        display: function (data) {
                            return '<button type="button" onclick="AlterarBeneficiarioJS(' + data.record.Posicao + ',\'' + data.record.Cpf + '\',\'' + data.record.Nome + '\');" class="btn btn-primary btn-sm">Alterar</button>';
                        }
                    },
                    Excluir: {
                        title: '',
                        display: function (data) {
                            return '<button type="button" onclick="ExcluirBeneficiarioJS(' + data.record.Posicao + ');" class="btn btn-primary btn-sm">Excluir</button>';
                        }
                    }



                }
            });

        if (document.getElementById("gridBeneficiarios"))
            $('#gridBeneficiarios').jtable('load');
    }

