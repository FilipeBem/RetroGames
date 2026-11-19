// Toda tela que busca dados trata três estados: carregando, erro e vazio.

export function Carregando({ texto = 'Carregando...' }) {
  return (
    <div className="text-center py-5" role="status">
      <div className="spinner-border text-secondary" aria-hidden="true"></div>
      <div className="mt-2 text-muted">{texto}</div>
    </div>
  )
}

export function MensagemErro({ mensagem, aoTentarNovamente }) {
  return (
    <div className="alert alert-danger d-flex justify-content-between align-items-center" role="alert">
      <span>{mensagem}</span>
      {aoTentarNovamente && (
        <button type="button" className="btn btn-outline-danger btn-sm" onClick={aoTentarNovamente}>
          Tentar novamente
        </button>
      )}
    </div>
  )
}

export function EstadoVazio({ titulo, children }) {
  return (
    <div className="text-center text-muted py-5">
      <h2 className="h5">{titulo}</h2>
      {children}
    </div>
  )
}
