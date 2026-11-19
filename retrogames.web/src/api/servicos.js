import api from './api'

// Uma função por endpoint. Cada uma devolve o campo "data" do envelope (resposta.data.data).
// Gravação e login entram na aula 8.

export const plataformaApi = {
  listar: async () => (await api.get('/Plataforma')).data.data,
}

export const generoApi = {
  listar: async () => (await api.get('/Genero')).data.data,
}

export const condicaoJogoApi = {
  listar: async () => (await api.get('/CondicaoJogo')).data.data,
}

export const jogoApi = {
  // parametros: { busca, idPlataforma, idGenero, pagina, tamanhoPagina }
  listar: async (parametros) => (await api.get('/Jogo', { params: parametros })).data.data,
  buscarPorId: async (id) => (await api.get(`/Jogo/${id}`)).data.data,
}
