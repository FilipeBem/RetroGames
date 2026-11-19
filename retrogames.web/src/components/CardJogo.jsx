import { Link } from 'react-router-dom'
import { formatarPreco } from '../lib/formato'

// Componente pequeno: só recebe dados por props.
function CardJogo({ jogo }) {
  const esgotado = jogo.estoque === 0

  return (
    <div className="card card-jogo h-100">
      <div className="card-body d-flex flex-column">
        <div className="text-muted small">{jogo.nomePlataforma}</div>
        <h2 className="h5 card-title">{jogo.nome}</h2>
        <div className="small mb-2">
          {jogo.anoLancamento} · {jogo.descricaoCondicao}
        </div>
        <div className="mb-2">
          {jogo.nomesGeneros.map((genero) => (
            <span key={genero} className="badge text-bg-secondary me-1">{genero}</span>
          ))}
        </div>
        <div className="mt-auto">
          <div className="fs-5 fw-bold">{formatarPreco(jogo.preco)}</div>
          {esgotado && <span className="badge text-bg-danger">Esgotado</span>}
        </div>
      </div>
      <div className="card-footer bg-transparent">
        <Link to={`/jogos/${jogo.id}`} className="btn btn-outline-primary btn-sm stretched-link">
          Ver detalhes
        </Link>
      </div>
    </div>
  )
}

export default CardJogo
