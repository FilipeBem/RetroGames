import axios from 'axios'

// Único arquivo que importa o Axios. baseURL '/api': o proxy do Vite repassa para a API.
// withCredentials: o cookie de sessão vai junto em toda requisição.
const api = axios.create({
  baseURL: '/api',
  withCredentials: true,
})

export default api
