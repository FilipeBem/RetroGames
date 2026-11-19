import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { jogoApi } from '../../api/servicos'
import { obterMensagemErro } from '../../api/erros'
import { Carregando, MensagemErro } from '../../components/MensagensEstado'
import { formatarPreco } from '../../lib/formato'

function DetalheJogo() {
  const { id } = useParams() // o número vem da rota /jogos/:id
  const [jogo, setJogo] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState(null)
  const [tentativa, setTentativa] = useState(0)

  useEffect(() => {
    let ativo = true
    setCarregando(true)
    setErro(null)
    setJogo(null)

    jogoApi
      .buscarPorId(id)
      .then((dados) => {
        if (ativo) setJogo(dados)
      })
      .catch((e) => {
        // 404: mostra o errorMessage do envelope ("Jogo não encontrado.") sem quebrar a tela.
        if (ativo) setErro(obterMensagemErro(e))
      })
      .finally(() => {
        if (ativo) setCarregando(false)
      })

    return () => {
      ativo = false
    }
  }, [id, tentativa])

  return (
    <>
      <Link to="/" className="btn btn-link px-0 mb-3">← Voltar ao catálogo</Link>

      {carregando && <Carregando texto="Carregando jogo..." />}

      {!carregando && erro && (
        <MensagemErro mensagem={erro} aoTentarNovamente={() => setTentativa((t) => t + 1)} />
      )}

      {!carregando && !erro && jogo && (
        <div className="card">
          <div className="card-body">
            <div className="text-muted">{jogo.nomePlataforma}</div>
            <h1 className="h3">{jogo.nome}</h1>

            <dl className="row mt-3 mb-0">
              <dt className="col-sm-3">Ano de lançamento</dt>
              <dd className="col-sm-9">{jogo.anoLancamento}</dd>

              <dt className="col-sm-3">Condição</dt>
              <dd className="col-sm-9">{jogo.descricaoCondicao}</dd>

              <dt className="col-sm-3">Gêneros</dt>
              <dd className="col-sm-9">
                {jogo.generos.map((genero) => (
                  <span key={genero.id} className="badge text-bg-secondary me-1">{genero.nome}</span>
                ))}
              </dd>

              <dt className="col-sm-3">Descrição</dt>
              <dd className="col-sm-9">{jogo.descricao || 'Sem descrição.'}</dd>

              <dt className="col-sm-3">Preço</dt>
              <dd className="col-sm-9 fw-bold">{formatarPreco(jogo.preco)}</dd>

              <dt className="col-sm-3">Estoque</dt>
              <dd className="col-sm-9">
                {jogo.estoque === 0 ? (
                  <span className="badge text-bg-danger">Esgotado</span>
                ) : (
                  `${jogo.estoque} unidade(s)`
                )}
              </dd>
            </dl>
          </div>
        </div>
      )}
    </>
  )
}

export default DetalheJogo
