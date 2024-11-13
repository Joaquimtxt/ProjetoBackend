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


// Call the update function when the page loads
window.onload = updateSelect;