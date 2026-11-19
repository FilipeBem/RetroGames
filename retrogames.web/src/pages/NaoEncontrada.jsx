import { Link } from 'react-router-dom'
import { EstadoVazio } from '../components/MensagensEstado'

function NaoEncontrada() {
  return (
    <EstadoVazio titulo="Página não encontrada">
      <p>O endereço que você acessou não existe.</p>
      <Link to="/" className="btn btn-primary">Voltar ao catálogo</Link>
    </EstadoVazio>
  )
}

export default NaoEncontrada
