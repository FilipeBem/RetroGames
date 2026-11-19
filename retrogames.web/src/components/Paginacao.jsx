// Anterior, números e Próxima. Não aparece quando há uma página só.
function Paginacao({ pagina, totalPaginas, aoMudar }) {
  if (totalPaginas <= 1) {
    return null
  }

  const numeros = Array.from({ length: totalPaginas }, (_, indice) => indice + 1)

  return (
    <nav aria-label="Paginação dos jogos" className="mt-4">
      <ul className="pagination justify-content-center flex-wrap">
        <li className={`page-item ${pagina === 1 ? 'disabled' : ''}`}>
          <button type="button" className="page-link" onClick={() => aoMudar(pagina - 1)} disabled={pagina === 1}>
            Anterior
          </button>
        </li>
        {numeros.map((numero) => (
          <li key={numero} className={`page-item ${numero === pagina ? 'active' : ''}`}>
            <button
              type="button"
              className="page-link"
              onClick={() => aoMudar(numero)}
              aria-current={numero === pagina ? 'page' : undefined}
            >
              {numero}
            </button>
          </li>
        ))}
        <li className={`page-item ${pagina === totalPaginas ? 'disabled' : ''}`}>
          <button
            type="button"
            className="page-link"
            onClick={() => aoMudar(pagina + 1)}
            disabled={pagina === totalPaginas}
          >
            Próxima
          </button>
        </li>
      </ul>
    </nav>
  )
}

export default Paginacao
