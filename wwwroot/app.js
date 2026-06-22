const API_URL = '/api';

document.addEventListener('DOMContentLoaded', () => {
    carregarVeiculos();
    carregarClientes();
    carregarFabricantes();

    document.getElementById('veiculoForm').addEventListener('submit', salvarVeiculo);
    document.getElementById('clienteForm').addEventListener('submit', salvarCliente);
    document.getElementById('aluguelForm').addEventListener('submit', registrarAluguel);
});

async function carregarVeiculos() {
    try {
        const response = await fetch(`${API_URL}/veiculos`);
        if (!response.ok) throw new Error('Erro ao buscar veículos');
        
        const veiculos = await response.json();
        renderizarTabela(veiculos);
    } catch (error) {
        alert(error.message);
    }
}

async function carregarClientes() {
    try {
        const response = await fetch(`${API_URL}/clientes`);
        if (!response.ok) throw new Error('Erro ao buscar clientes');
        
        const clientes = await response.json();
        renderizarTabelaClientes(clientes);
    } catch (error) {
        alert(error.message);
    }
}

function renderizarTabela(veiculos) {
    const tbody = document.getElementById('tabelaVeiculos');
    tbody.innerHTML = '';

    if(veiculos.length === 0) {
        tbody.innerHTML = `<tr><td colspan="6" class="text-center text-muted">Nenhum veículo encontrado.</td></tr>`;
        return;
    }

    veiculos.forEach(v => {
        const isAlugado = v.aluguelId !== null && v.aluguelId !== undefined && v.aluguelId > 0;

        const badgeStatus = isAlugado 
            ? '<span class="badge bg-danger">Alugado</span>' 
            : '<span class="badge bg-success">Disponível</span>';

        const botaoAcao = isAlugado
            ? `<button class="btn btn-sm btn-info" onclick="devolverVeiculo(${v.aluguelId})">Devolver</button>`
            : `<button class="btn btn-sm btn-dark" onclick="abrirModalAluguel(${v.id}, '${v.modelo}')">Alugar</button>`;

        tbody.innerHTML += `
            <tr class="table-light text-muted">
                <td>${v.id}</td>
                <td><strong>${v.modelo}</strong> ${badgeStatus}</td>
                <td>${v.anoFabricacao}</td>
                <td><span class="badge bg-info text-dark">${v.nomeFabricante}</span></td>
                <td>${v.descricaoCategoria}</td>
                <td>
                    <button class="btn btn-sm btn-warning me-1" onclick="prepararEdicao(${JSON.stringify(v).replace(/"/g, '&quot;')})">Editar</button>
                    <button class="btn btn-sm btn-danger me-1" onclick="deletarVeiculo(${v.id})">Excluir</button>
                    ${botaoAcao}
                </td>
            </tr>
        `;
    });
}

function renderizarTabelaClientes(clientes) {
    const tbody = document.getElementById('tabelaClientes');
    tbody.innerHTML = '';

    if(clientes.length === 0) {
        tbody.innerHTML = `<tr><td colspan="5" class="text-center text-muted">Nenhum cliente encontrado.</td></tr>`;
        return;
    }

    clientes.forEach(c => {
        tbody.innerHTML += `
            <tr>
                <td>${c.id}</td>
                <td><strong>${c.nome}</strong></td>
                <td>${c.cpf}</td>
                <td>${c.email}</td>
                <td>
                    <button class="btn btn-sm btn-warning me-1" onclick="prepararEdicaoClientes(${JSON.stringify(c).replace(/"/g, '&quot;')})">Editar</button>
                    <button class="btn btn-sm btn-danger me-1" onclick="deletarCliente(${c.id})">Excluir</button>
                    <button class="btn btn-sm btn-info" onclick="abrirModalConsultaAluguel('${c.cpf}', '${c.nome}')">Consultar Aluguel</button>
                </td>
            </tr>
        `;
    });
}

async function carregarFabricantes() {
    try {
        const response = await fetch(`${API_URL}/fabricantes`);
        if (!response.ok) throw new Error('Erro ao buscar fabricantes');
        
        const fabricantes = await response.json();
        renderizarTabelaFabricantes(fabricantes);
    } catch (error) {
        alert(error.message);
    }
}

function renderizarTabelaFabricantes(fabricantes) {
    const tbody = document.getElementById('tabelaFabricantes');
    tbody.innerHTML = '';

    if (fabricantes.length === 0) {
        tbody.innerHTML = `<tr><td colspan="3" class="text-center text-muted">Nenhum fabricante encontrado.</td></tr>`;
        return;
    }

    fabricantes.forEach(f => {
        tbody.innerHTML += `
            <tr>
                <td>${f.id}</td>
                <td><span class="badge bg-info text-dark fs-6">${f.nome}</span></td>
                <td>
                    <button class="btn btn-sm btn-dark" onclick="abrirModalHistoricoFabricante(${f.id}, '${f.nome}')">
                        Ver Histórico de Aluguéis
                    </button>
                </td>
            </tr>
        `;
    });
}

async function abrirModalHistoricoFabricante(fabricanteId, nomeFabricante) {
    document.getElementById('historicoFabricanteModalLabel').innerText = `Histórico de Aluguéis - ${nomeFabricante}`;
    
    const tbody = document.getElementById('tabelaHistoricoFabricante');
    tbody.innerHTML = `<tr><td colspan="5" class="text-center text-muted">Carregando histórico...</td></tr>`;

    const modal = new bootstrap.Modal(document.getElementById('historicoFabricanteModal'));
    modal.show();

    try {
        const response = await fetch(`${API_URL}/consultas/historico-fabricante/${fabricanteId}`);
        if (!response.ok) throw new Error('Erro ao buscar histórico do fabricante.');

        const historico = await response.json();
        tbody.innerHTML = '';

        if (historico.length === 0) {
            tbody.innerHTML = `<tr><td colspan="5" class="text-center text-muted">Nenhum veículo deste fabricante foi alugado ainda.</td></tr>`;
            return;
        }

        historico.forEach(h => {
            const dataSaidaFormatada = new Date(h.dataSaida).toLocaleString('pt-BR', {
                day: '2-digit',
                month: '2-digit',
                year: 'numeric',
                hour: '2-digit',
                minute: '2-digit'
            });

            const valorTotalFormatado = h.valorTotal 
                ? `R$ ${parseFloat(h.valorTotal).toFixed(2).replace('.', ',')}` 
                : 'Pendente';

            tbody.innerHTML += `
                <tr>
                    <td>${h.id}</td>
                    <td>${h.nomeCliente}</td>
                    <td><strong>${h.modeloVeiculo}</strong></td>
                    <td>${dataSaidaFormatada}</td>
                    <td><span class="text-success fw-bold">${valorTotalFormatado}</span></td>
                </tr>
            `;
        });

    } catch (error) {
        tbody.innerHTML = `<tr><td colspan="5" class="text-center text-danger">Erro: ${error.message}</td></tr>`;
    }
}

async function abrirModalConsultaAluguel(cpf, nomeCliente) {
    document.getElementById('consultaAluguelModalLabel').innerText = `Aluguéis Ativos - ${nomeCliente}`;
    
    const tbody = document.getElementById('tabelaConsultaAlugueis');
    tbody.innerHTML = `<tr><td colspan="5" class="text-center text-muted">Carregando aluguéis...</td></tr>`;

    const modal = new bootstrap.Modal(document.getElementById('consultaAluguelModal'));
    modal.show();

    try {
        const response = await fetch(`${API_URL}/consultas/alugueis-cliente/${encodeURIComponent(cpf)}`);
        
        if (!response.ok) {
            throw new Error('Erro ao buscar histórico de aluguéis.');
        }

        const alugueis = await response.json();
        tbody.innerHTML = '';

        if (alugueis.length === 0) {
            tbody.innerHTML = `<tr><td colspan="5" class="text-center text-muted">Este cliente não possui nenhum aluguel ativo no momento.</td></tr>`;
            return;
        }

        alugueis.forEach(a => {
            const dataSaidaFormatada = new Date(a.dataSaida).toLocaleString('pt-BR', {
                day: '2-digit',
                month: '2-digit',
                year: 'numeric',
                hour: '2-digit',
                minute: '2-digit'
            });

            const valorTotalFormatado = a.valorTotal 
                ? `R$ ${parseFloat(a.valorTotal).toFixed(2).replace('.', ',')}` 
                : 'Não informado';

            tbody.innerHTML += `
                <tr>
                    <td>${a.id}</td>
                    <td><strong>${a.modeloVeiculo}</strong></td>
                    <td><span class="badge bg-info text-dark">${a.fabricanteVeiculo}</span></td>
                    <td>${dataSaidaFormatada}</td>
                    <td><span class="text-success fw-bold">${valorTotalFormatado}</span></td>
                </tr>
            `;
        });

    } catch (error) {
        tbody.innerHTML = `<tr><td colspan="5" class="text-center text-danger">Erro: ${error.message}</td></tr>`;
    }
}

async function salvarVeiculo(e) {
    e.preventDefault();

    const id = document.getElementById('veiculoId').value;
    const dto = {
        modelo: document.getElementById('modelo').value,
        anoFabricacao: parseInt(document.getElementById('anoFabricacao').value),
        quilometragemAtual: parseInt(document.getElementById('quilometragem').value),
        fabricanteId: parseInt(document.getElementById('fabricanteId').value),
        categoriaId: parseInt(document.getElementById('categoriaId').value)
    };

    const edicao = id !== "";
    const url = edicao ? `${API_URL}/veiculos/${id}` : `${API_URL}/veiculos`;
    const mtd = edicao ? 'PUT' : 'POST';

    try {
        const response = await fetch(url, {
            method: mtd,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(dto)
        });

        if (!response.ok) {
            const erroTxt = await response.text();
            throw new Error(erroTxt || 'Erro ao processar requisição');
        }

        alert(edicao ? 'Veículo atualizado!' : 'Veículo cadastrado com sucesso!');
        limparFormulario();
        carregarVeiculos();
    } catch (error) {
        alert('Erro: ' + error.message);
    }
}

async function salvarCliente(e) {
    e.preventDefault();

    const id = document.getElementById('clienteId').value;
    const dto = {
        nome: document.getElementById('nomeCliente').value,
        cpf: document.getElementById('cpfCliente').value,
        email: document.getElementById('emailCliente').value
    };

    const edicao = id !== "";
    const url = edicao ? `${API_URL}/clientes/${id}` : `${API_URL}/clientes`;
    const mtd = edicao ? 'PUT' : 'POST';

    try {
        const response = await fetch(url, {
            method: mtd,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(dto)
        });

        if (!response.ok) {
            const erroTxt = await response.text();
            throw new Error(erroTxt || 'Erro ao processar requisição');
        }

        alert(edicao ? 'Cliente atualizado!' : 'Cliente cadastrado com sucesso!');
        limparFormulario();
        carregarClientes();
    } catch (error) {
        alert('Erro: ' + error.message);
    }
}

async function deletarVeiculo(id) {
    if (!confirm('Deseja realmente excluir este veículo?')) return;

    try {
        const response = await fetch(`${API_URL}/veiculos/${id}`, { method: 'DELETE' });
        
        if (!response.ok) {
            const erroTxt = await response.text();
            throw new Error(erroTxt || 'Erro ao deletar veículo.');
        }

        alert('Veículo removido com sucesso!');
        carregarVeiculos();
    } catch (error) {
        alert('Erro: ' + error.message);
    }
}

async function deletarCliente(id) {
    if (!confirm('Deseja realmente excluir este cliente?')) return;

    try {
        const response = await fetch(`${API_URL}/clientes/${id}`, { method: 'DELETE' });
        
        if (!response.ok) {
            const erroTxt = await response.text();
            throw new Error(erroTxt || 'Erro ao deletar cliente.');
        }

        alert('Cliente removido com sucesso!');
        carregarClientes();
    } catch (error) {
        alert('Erro: ' + error.message);
    }
}

async function filtrarVeiculos() {
    const fab = document.getElementById('buscaFabricante').value;
    const cat = document.getElementById('buscaCategoria').value;

    try {
        const response = await fetch(`${API_URL}/consultas/veiculos-por-tipo?fab=${encodeURIComponent(fab)}&cat=${encodeURIComponent(cat)}`);
        if (!response.ok) throw new Error('Erro ao filtrar dados');
        
        const dados = await response.json();
        renderizarTabela(dados);
    } catch (error) {
        alert(error.message);
    }
}

async function filtrarVeiculosKm() {
    const kmmax = document.getElementById('buscaKmMax').value;
    const catId = document.getElementById('buscaIdCategoria').value;

    try {
        const response = await fetch(`${API_URL}/consultas/veiculos-km?kmMax=${encodeURIComponent(kmmax)}&catId=${encodeURIComponent(catId)}`);
        if (!response.ok) throw new Error('Erro ao filtrar dados');
        
        const dados = await response.json();
        renderizarTabela(dados);
    } catch (error) {
        alert(error.message);
    }
}

function abrirModalAluguel(veiculoId, modelo) {
    document.getElementById('aluguelVeiculoId').value = veiculoId;
    document.getElementById('aluguelVeiculoModelo').value = modelo;
    document.getElementById('aluguelClienteId').value = "";
    document.getElementById('aluguelValorDiaria').value = "";


    const modal = new bootstrap.Modal(document.getElementById('aluguelModal'));
    modal.show();
} 

async function abrirModalConsultaAluguel(cpf, nomeCliente) {
    document.getElementById('consultaAluguelModalLabel').innerText = `Aluguéis Ativos - ${nomeCliente}`;

    document.getElementById('consultaAluguelCpf').value = cpf; 
    document.getElementById('filtroValorMinimo').value = '';

    const modal = new bootstrap.Modal(document.getElementById('consultaAluguelModal'));
    modal.show();

    carregarAlugueisDoCliente(cpf);
}

async function carregarAlugueisDoCliente(cpf) {
    const tbody = document.getElementById('tabelaConsultaAlugueis');
    tbody.innerHTML = `<tr><td colspan="5" class="text-center text-muted">Carregando aluguéis...</td></tr>`;

    try {
        const response = await fetch(`${API_URL}/consultas/alugueis-cliente/${encodeURIComponent(cpf)}`);
        if (!response.ok) throw new Error('Erro ao buscar histórico de aluguéis.');

        const alugueis = await response.json();
        exibirAlugueisNoModal(alugueis);
    } catch (error) {
        tbody.innerHTML = `<tr><td colspan="5" class="text-center text-danger">Erro: ${error.message}</td></tr>`;
    }
}

async function filtrarAlugueisClientePorValor() {
    const cpf = document.getElementById('consultaAluguelCpf').value;
    const valorMin = document.getElementById('filtroValorMinimo').value;

    if (!valorMin || parseFloat(valorMin) <= 0) {
        carregarAlugueisDoCliente(cpf);
        return;
    }

    const tbody = document.getElementById('tabelaConsultaAlugueis');
    tbody.innerHTML = `<tr><td colspan="5" class="text-center text-muted">Filtrando valores premium...</td></tr>`;

    try {
        const response = await fetch(`${API_URL}/consultas/alugueis-premium?valorMin=${encodeURIComponent(valorMin)}`);
        if (!response.ok) throw new Error('Erro ao aplicar filtro de valor mínimo.');

        const todosAlugueisPremium = await response.json();

        const alugueisFiltrados = todosAlugueisPremium.filter(a => 
            a.cpfCliente === cpf && 
            (a.dataDevolucao === null || a.dataDevolucao === undefined)
        );

        exibirAlugueisNoModal(alugueisFiltrados);

    } catch (error) {
        tbody.innerHTML = `<tr><td colspan="5" class="text-center text-danger">Erro: ${error.message}</td></tr>`;
    }
}

function exibirAlugueisNoModal(alugueis) {
    const tbody = document.getElementById('tabelaConsultaAlugueis');
    tbody.innerHTML = '';

    if (alugueis.length === 0) {
        tbody.innerHTML = `<tr><td colspan="5" class="text-center text-muted">Nenhum aluguel ativo atende a este critério.</td></tr>`;
        return;
    }

    alugueis.forEach(a => {
        const dataSaidaFormatada = new Date(a.dataSaida).toLocaleString('pt-BR', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });

        const valorTotalFormatado = a.valorTotal 
            ? `R$ ${parseFloat(a.valorTotal).toFixed(2).replace('.', ',')}` 
            : 'Não calculado';

        tbody.innerHTML += `
            <tr>
                <td>${a.id}</td>
                <td><strong>${a.modeloVeiculo}</strong></td>
                <td><span class="badge bg-info text-dark">${a.fabricanteVeiculo}</span></td>
                <td>${dataSaidaFormatada}</td>
                <td><span class="text-success fw-bold">${valorTotalFormatado}</span></td>
            </tr>
        `;
    });
}

async function registrarAluguel(e) {
    e.preventDefault();

    const dto = {
        clienteId: parseInt(document.getElementById('aluguelClienteId').value),
        veiculoId: parseInt(document.getElementById('aluguelVeiculoId').value),
        valorDiaria: parseFloat(document.getElementById('aluguelValorDiaria').value)
    };

    try {
        const response = await fetch(`${API_URL}/alugueis`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(dto)
        });

        if (!response.ok) {
            const erroTxt = await response.text();
            throw new Error(erroTxt || 'Erro ao registrar aluguel.');
        }

        const resultado = await response.json();
        alert(resultado.mensagem || 'Aluguel registrado com sucesso!');

        const modalElement = document.getElementById('aluguelModal');
        const modal = bootstrap.Modal.getInstance(modalElement);
        modal.hide();
        document.getElementById('aluguelForm').reset();

        carregarVeiculos();

    } catch (error) {
        alert('Erro: ' + error.message);
    }
}

async function devolverVeiculo(aluguelId) {
    if (!confirm('Deseja realmente processar a devolução deste veículo? (Isso removerá o registro do aluguel)')) return;

    try {
        const response = await fetch(`${API_URL}/alugueis/${aluguelId}`, {
            method: 'DELETE'
        });

        if (!response.ok) {
            const erroTxt = await response.text();
            throw new Error(erroTxt || 'Erro ao processar devolução do veículo.');
        }

        alert('Veículo devolvido com sucesso!');
        carregarVeiculos();
    } catch (error) {
        alert('Erro: ' + error.message);
    }
}

function prepararEdicao(veiculo) {
    document.getElementById('formTitulo').innerText = "Editar Veículo #" + veiculo.id;
    document.getElementById('veiculoId').value = veiculo.id;
    document.getElementById('modelo').value = veiculo.modelo;
    document.getElementById('anoFabricacao').value = veiculo.anoFabricacao;

    document.getElementById('quilometragem').value = 0; 
    document.getElementById('fabricanteId').value = 1; 
    document.getElementById('categoriaId').value = 1;

    document.getElementById('btnCancelar').classList.remove('d-none');
    document.getElementById('btnSalvar').className = "btn btn-warning w-100";
}

function prepararEdicaoClientes(cliente) {
    document.getElementById('formTituloClientes').innerText = "Editar Cliente #" + cliente.id;
    document.getElementById('clienteId').value = cliente.id;
    document.getElementById('nomeCliente').value = cliente.nome;
    document.getElementById('cpfCliente').value = cliente.cpf;
    document.getElementById('emailCliente').value = cliente.email;

    document.getElementById('btnCancelarCliente').classList.remove('d-none');
    document.getElementById('btnSalvarCliente').className = "btn btn-warning w-100";
}

function limparFormulario() {
    document.getElementById('formTitulo').innerText = "Cadastrar Veículo";
    document.getElementById('veiculoForm').reset();
    document.getElementById('veiculoId').value = "";
    document.getElementById('btnCancelar').classList.add('d-none');
    document.getElementById('btnSalvar').className = "btn btn-success w-100";
}

function limparFormularioClientes() {
    document.getElementById('formTituloClientes').innerText = "Cadastrar Cliente";
    document.getElementById('clienteForm').reset();
    document.getElementById('clienteId').value = "";
    document.getElementById('btnCancelarCliente').classList.add('d-none');
    document.getElementById('btnSalvarCliente').className = "btn btn-success w-100";
}
