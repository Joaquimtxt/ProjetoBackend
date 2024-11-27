// Seleciona todos os itens de navegação
const navLinks = document.querySelectorAll('.navbar-nav .custom-link');
// Obtém o caminho do URL atual
const currentPath = window.location.pathname;

// Adiciona a classe 'active' ao link que corresponde ao caminho atual e remove dos demais
navLinks.forEach(link => {
    if (link.getAttribute('href') === currentPath) {
        link.classList.add('active');
    } else {
        link.classList.remove('active');
    }
});


// Get a reference to the hidden input and select elements
const hiddenInput = document.getElementById('cadAtivo');
const selectElement = document.getElementById('CadastroAtivo');

// Function to update the select element based on the hidden input value
function updateSelect() {
    const hiddenValue = hiddenInput.value;

    // Select the appropriate option based on the hidden value
    if (hiddenValue === 'True') {
        selectElement.value = 'True';
    } else {
        selectElement.value = 'False';
    }
}


function onChangeSelect() {
    hiddenInput.value = selectElement.value;
}


// Lista para armazenar os itens
let listaItens = [];

// Função para adicionar produto
document.getElementById("AdicionarProduto").addEventListener("click", function () {
    let codigoProduto = document.getElementById("CodigoProduto").value;
    let quantidadeProduto = document.getElementById("QuantidadeProduto").value;

    if (codigoProduto && quantidadeProduto) {
        // Buscar o produto no banco usando o código (aqui você pode fazer uma chamada AJAX)
        fetch(`/Produtos/BuscarPorCodigo?codigo=${codigoProduto}`)
            .then(response => response.json())
            .then(produto => {
                // Calcular total do item
                let totalProduto = produto.Preco * quantidadeProduto;

                // Adicionar o item à lista
                listaItens.push({
                    ProdutoNome: produto.Nome,
                    PrecoUnitario: produto.Preco,
                    Quantidade: quantidadeProduto,
                    Total: totalProduto
                });

                // Atualizar a tabela com o novo item
                atualizarTabela();

                // Limpar campos
                document.getElementById("CodigoProduto").value = '';
                document.getElementById("QuantidadeProduto").value = '';
            });
    }
});

// Função para atualizar a tabela
function atualizarTabela() {
    let tabela = document.getElementById("TabelaItens").getElementsByTagName('tbody')[0];
    tabela.innerHTML = ''; // Limpa a tabela antes de preencher novamente

    let valorTotalVenda = 0;

    listaItens.forEach(item => {
        let row = tabela.insertRow();
        row.innerHTML = `<td>${item.ProdutoNome}</td>
                                 <td>${item.PrecoUnitario.toFixed(2)}</td>
                                 <td>${item.Quantidade}</td>
                                 <td>${item.Total.toFixed(2)}</td>`;
        valorTotalVenda += item.Total;
    });

    // Atualizar o valor total
    document.getElementById("ValorTotal").value = valorTotalVenda.toFixed(2);
}
// Função para buscar o serviço pelo nome
function buscarServico() {
    var nomeServico = document.getElementById("NomeServico").value;

    if (nomeServico.length > 2) { // Apenas inicia a busca depois de digitar pelo menos 3 caracteres
        $.ajax({
            url: '@Url.Action("GetServicoPorNome", "Servicos")',
            data: { nome: nomeServico },
            type: 'GET',
            success: function (data) {
                if (data) {
                    // Se o serviço for encontrado, preenche o preço
                    $('#PrecoServico').val(data.ValorServico);
                } else {
                    // Se não encontrar, limpar o campo de preço
                    $('#PrecoServico').val('');
                }
            },
            error: function () {
                $('#PrecoServico').val('');
                alert("Erro ao buscar o serviço.");
            }
        });
    }
}

// Função para adicionar o serviço à tabela
function adicionarServico() {
    var nomeServico = document.getElementById("NomeServico").value;
    var precoServico = document.getElementById("PrecoServico").value;

    if (!nomeServico || !precoServico) {
        alert("Preencha o nome do serviço e o preço.");
        return;
    }

    var tabela = document.getElementById("TabelaServicos").getElementsByTagName('tbody')[0];
    var novaLinha = tabela.insertRow();

    // Adiciona o nome do serviço e o preço
    var celulaNome = novaLinha.insertCell(0);
    var celulaPreco = novaLinha.insertCell(1);

    celulaNome.textContent = nomeServico;
    celulaPreco.textContent = precoServico;

    // Limpa os campos após adicionar
    document.getElementById("NomeServico").value = '';
    document.getElementById("PrecoServico").value = '';
}

// Atualizar o valor total
document.getElementById("ValorTotal").value = (valorTotalVenda + valorTotalServicos).toFixed(2);
}
