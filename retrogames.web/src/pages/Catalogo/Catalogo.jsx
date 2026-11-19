import { useCallback, useEffect, useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { condicaoJogoApi, generoApi, jogoApi, plataformaApi } from '../../api/servicos'
import { obterMensagemErro } from '../../api/erros'
import CardJogo from '../../components/CardJogo'
import Paginacao from '../../components/Paginacao'
import { Carregando, EstadoVazio, MensagemErro } from '../../components/MensagensEstado'

const TAMANHO_PAGINA = 12

function Catalogo() {
  // Os filtros ficam na URL: ?busca=mario&idPlataforma=2&idGenero=4&pagina=2
  const [parametros, setParametros] = useSearchParams()
  const busca = parametros.get('busca') ?? ''
  const idPlataforma = parametros.get('idPlataforma') ?? ''
  const idGenero = parametros.get('idGenero') ?? ''
  const pagina = Number(parametros.get('pagina')) || 1

  const [textoBusca, setTextoBusca] = useState(busca)
  const [plataformas, setPlataformas] = useState([])
  const [generos, setGeneros] = useState([])
  const [resultado, setResultado] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState(null)
  const [tentativa, setTentativa] = useState(0)

  // Mantém o campo de busca igual à URL (por exemplo, ao usar o botão Voltar).
  useEffect(() => {
    setTextoBusca(busca)
  }, [busca])

  // As listas de plataformas e gêneros carregam uma vez.
  useEffect(() => {
    let ativo = true
    Promise.all([plataformaApi.listar(), generoApi.listar()])
      .then(([listaPlataformas, listaGeneros]) => {
        if (ativo) {
          setPlataformas(listaPlataformas)
          setGeneros(listaGeneros)
        }
      })
      .catch(() => {
        // Sem as listas, os filtros ficam vazios; o erro principal aparece na busca dos jogos.
      })
    return () => {
      ativo = false
    }
  }, [])

  // Os jogos recarregam sempre que um filtro da URL muda.
  useEffect(() => {
    let ativo = true
    setCarregando(true)
    setErro(null)

    const filtros = { pagina, tamanhoPagina: TAMANHO_PAGINA }
    if (busca) filtros.busca = busca
    if (idPlataforma) filtros.idPlataforma = idPlataforma
    if (idGenero) filtros.idGenero = idGenero

    jogoApi
      .listar(filtros)
      .then((dados) => {
        if (ativo) setResultado(dados)
      })
      .catch((e) => {
        if (ativo) setErro(obterMensagemErro(e))
      })
      .finally(() => {
        if (ativo) setCarregando(false)
      })

    return () => {
      ativo = false
    }
  }, [busca, idPlataforma, idGenero, pagina, tentativa])

  // Trocar um filtro volta à página 1.
  const alterarFiltro = useCallback(
    (nome, valor) => {
      const novos = new URLSearchParams(parametros)
      if (valor) {
        novos.set(nome, valor)
      } else {
        novos.delete(nome)
      }
      novos.delete('pagina')
      setParametros(novos)
    },
    [parametros, setParametros],
  )

  const mudarPagina = (novaPagina) => {
    const novos = new URLSearchParams(parametros)
    if (novaPagina > 1) {
      novos.set('pagina', String(novaPagina))
    } else {
      novos.delete('pagina')
    }
    setParametros(novos)
    window.scrollTo({ top: 0 })
  }

  const enviarBusca = (evento) => {
    evento.preventDefault()
    alterarFiltro('busca', textoBusca.trim())
  }

  const limparFiltros = () => setParametros(new URLSearchParams())

  return (
    <>
      <h1 className="h3 mb-3">Catálogo</h1>

      <form className="row g-2 mb-4" onSubmit={enviarBusca}>
        <div className="col-12 col-md-5">
          <label htmlFor="busca" className="visually-hidden">Buscar por nome</label>
          <input
            id="busca"
            type="search"
            className="form-control"
            placeholder="Buscar por nome"
            value={textoBusca}
            onChange={(e) => setTextoBusca(e.target.value)}
          />
        </div>
        <div className="col-6 col-md-3">
          <label htmlFor="plataforma" className="visually-hidden">Plataforma</label>
          <select
            id="plataforma"
            className="form-select"
            value={idPlataforma}
            onChange={(e) => alterarFiltro('idPlataforma', e.target.value)}
          >
            <option value="">Todas as plataformas</option>
            {plataformas.map((p) => (
              <option key={p.id} value={p.id}>{p.nome}</option>
            ))}
          </select>
        </div>
        <div className="col-6 col-md-2">
          <label htmlFor="genero" className="visually-hidden">Gênero</label>
          <select
            id="genero"
            className="form-select"
            value={idGenero}
            onChange={(e) => alterarFiltro('idGenero', e.target.value)}
          >
            <option value="">Todos os gêneros</option>
            {generos.map((g) => (
              <option key={g.id} value={g.id}>{g.nome}</option>
            ))}
          </select>
        </div>
        <div className="col-12 col-md-2 d-flex gap-2">
          <button type="submit" className="btn btn-primary flex-fill">Buscar</button>
          <button type="button" className="btn btn-outline-secondary" onClick={limparFiltros}>Limpar</button>
        </div>
      </form>

      {carregando && <Carregando texto="Carregando jogos..." />}

      {!carregando && erro && (
        <MensagemErro mensagem={erro} aoTentarNovamente={() => setTentativa((t) => t + 1)} />
      )}

      {!carregando && !erro && resultado && resultado.itens.length === 0 && (
        <EstadoVazio titulo="Nenhum jogo encontrado">
          <p>Tente outra busca ou limpe os filtros.</p>
        </EstadoVazio>
      )}

      {!carregando && !erro && resultado && resultado.itens.length > 0 && (
        <>
          <p className="text-muted small">{resultado.totalItens} jogo(s) encontrado(s)</p>
          <div className="row row-cols-1 row-cols-sm-2 row-cols-lg-3 row-cols-xl-4 g-3">
            {resultado.itens.map((jogo) => (
              <div className="col" key={jogo.id}>
                <CardJogo jogo={jogo} />
              </div>
            ))}
          </div>
          <Paginacao
            pagina={resultado.pagina}
            totalPaginas={resultado.totalPaginas}
            aoMudar={mudarPagina}
          />
        </>
      )}
    </>
  )
}

export default Catalogo
