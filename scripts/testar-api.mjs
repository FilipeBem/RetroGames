// Roda os casos de teste das aulas 5 e 6 contra a SUA API e escreve o resultado REAL nos arquivos
// docs/TESTES_AULA_5.md e docs/TESTES_AULA_6.md. Nada aqui é estimado: cada linha vem da resposta da API.
//
// Uso (na raiz do repositório, com a API rodando em dev e o MySQL ligado):
//   node scripts/testar-api.mjs 5            # aula 5
//   node scripts/testar-api.mjs 6            # aula 6
//   node scripts/testar-api.mjs 6 http://localhost:5080
// Exige Node 20 ou mais novo (fetch embutido). Para a aula 6, o .env precisa ter AdminInicial__Email e AdminInicial__Senha.
import { existsSync, readFileSync, writeFileSync } from 'node:fs'

const AULA = process.argv[2]
const BASE = (process.argv[3] ?? 'http://localhost:5080').replace(/\/$/, '')
if (AULA !== '5' && AULA !== '6') {
  console.error('Uso: node scripts/testar-api.mjs <5|6> [url-da-api]')
  process.exit(1)
}

// ---------- .env ----------
function lerEnv() {
  const valores = {}
  if (!existsSync('.env')) return valores
  for (const linha of readFileSync('.env', 'utf8').split(/\r?\n/)) {
    const m = linha.match(/^\s*([A-Za-z0-9_]+)\s*=\s*"?(.*?)"?\s*$/)
    if (m) valores[m[1]] = m[2]
  }
  return valores
}
const env = lerEnv()
const ADMIN_EMAIL = env.AdminInicial__Email
const ADMIN_SENHA = env.AdminInicial__Senha

// ---------- cookies (uma "janela" por jar; jar vazio = janela anônima) ----------
class Jar {
  constructor() { this.cookies = new Map() }
  guardar(headers) {
    for (const linha of headers.getSetCookie?.() ?? []) {
      const [par, ...atributos] = linha.split(';').map((p) => p.trim())
      const i = par.indexOf('=')
      const nome = par.slice(0, i)
      const valor = par.slice(i + 1)
      const expirado = atributos.some((a) => /^max-age=0$/i.test(a)) ||
        atributos.some((a) => /^expires=/i.test(a) && new Date(a.slice(8)) < new Date())
      if (!valor || expirado) this.cookies.delete(nome)
      else this.cookies.set(nome, valor)
    }
  }
  cabecalho() { return [...this.cookies].map(([n, v]) => `${n}=${v}`).join('; ') }
}

async function req(metodo, caminho, { corpo, bruto, jar } = {}) {
  const cabecalhos = {}
  let body
  if (bruto !== undefined) { body = bruto; cabecalhos['Content-Type'] = 'application/json' }
  else if (corpo !== undefined) { body = JSON.stringify(corpo); cabecalhos['Content-Type'] = 'application/json' }
  if (jar && jar.cookies.size) cabecalhos.Cookie = jar.cabecalho()
  try {
    const r = await fetch(BASE + caminho, { method: metodo, headers: cabecalhos, body })
    jar?.guardar(r.headers)
    const texto = await r.text()
    let json = null
    try { json = JSON.parse(texto) } catch { /* resposta sem JSON */ }
    return { status: r.status, json, texto, headers: r.headers }
  } catch (e) {
    return { status: 0, json: null, texto: String(e.cause?.code ?? e.message), headers: new Headers() }
  }
}

// ---------- registro dos casos ----------
const linhas = []
function registrar(n, descricao, esperado, r, ok, extra = '') {
  const codigo = r.json?.responseCode
  const obtido = `${r.status === 0 ? 'sem resposta (' + r.texto + ')' : r.status}` +
    (codigo !== undefined ? ` / responseCode ${codigo}` : '') + (extra ? ` / ${extra}` : '')
  linhas.push({ n, descricao, esperado, obtido, ok })
  console.log(`${ok ? 'OK    ' : 'FALHA '} #${n} ${descricao} -> ${obtido}`)
}
const chaves = (r) => Object.keys(r.json?.data ?? {})
const idDoLocation = (r) => Number(new URL(r.headers.get('location') ?? 'http://x/0').pathname.split('/').pop())

async function loginAdmin(jar) {
  if (!ADMIN_EMAIL || !ADMIN_SENHA) return 'sem-config'
  const r = await req('POST', '/api/Autenticacao/login', { corpo: { email: ADMIN_EMAIL, senha: ADMIN_SENHA }, jar })
  if (r.status === 200) return 'ok'
  if (r.status === 404 || r.status === 405) return 'sem-login' // API da aula 5 (sem autenticação)
  return 'falhou:' + r.status
}

const PLAT_OK = { nome: 'TESTE API Plataforma', fabricante: 'Teste', anoLancamento: 1999 }
const JOGO_OK = {
  nome: 'TESTE API Jogo', descricao: 'Jogo criado pelo script de testes', anoLancamento: 1995,
  preco: 50, estoque: 2, idPlataforma: 1, idCondicaoJogo: 2, idsGeneros: [1, 2],
}

// =====================================================================
// AULA 5
// =====================================================================
async function aula5() {
  const admin = new Jar()
  const estado = await loginAdmin(admin)
  if (estado === 'ok') console.log('(API com autenticação: gravações feitas como administrador)')
  else if (estado === 'sem-login') console.log('(API sem autenticação: aula 5)')
  else console.log(`ATENÇÃO: login do administrador não funcionou (${estado}); as gravações podem dar 401.`)
  const jar = admin

  // Plataformas
  let r = await req('POST', '/api/Plataforma', { corpo: PLAT_OK, jar })
  const idP = idDoLocation(r)
  registrar(1, 'POST /api/Plataforma (dados válidos)', '201, responseCode 0, cabeçalho Location', r,
    r.status === 201 && r.json?.responseCode === 0 && !!r.headers.get('location'), `Location ${r.headers.get('location') ?? 'ausente'}`)

  r = await req('GET', `/api/Plataforma/${idP}`, { jar })
  registrar(2, 'GET na URL do Location', '200, a plataforma criada', r, r.status === 200 && r.json?.data?.nome === PLAT_OK.nome)

  r = await req('PUT', `/api/Plataforma/${idP}`, { corpo: { ...PLAT_OK, nome: 'TESTE API Plataforma B' }, jar })
  registrar(3, 'PUT com outro nome', '200, registro alterado', r, r.status === 200 && r.json?.data?.nome === 'TESTE API Plataforma B')

  r = await req('PUT', `/api/Plataforma/${idP}`, { corpo: { nome: 'TESTE API Plataforma B', fabricante: 'Outro', anoLancamento: 1999 }, jar })
  registrar(4, 'PUT mantendo o mesmo nome (só muda o fabricante)', '200 (não é duplicidade)', r, r.status === 200)

  r = await req('DELETE', `/api/Plataforma/${idP}`, { jar })
  registrar(5, 'DELETE da plataforma criada', '200, responseCode 0', r, r.status === 200 && r.json?.responseCode === 0)

  r = await req('POST', '/api/Plataforma', { corpo: { nome: '', fabricante: 'X', anoLancamento: 1800 }, jar })
  registrar(6, 'POST com nome vazio e ano 1800', '400, responseCode 3, erros em data (nome, anoLancamento)', r,
    r.status === 400 && r.json?.responseCode === 3 && chaves(r).includes('nome') && chaves(r).includes('anoLancamento'),
    `campos: ${chaves(r).join(', ')}`)

  r = await req('POST', '/api/Plataforma', { bruto: '{"nome": "sem aspa final, "fabricante": "x"', jar })
  registrar(7, 'POST com JSON malformado', '400, responseCode 3', r, r.status === 400 && r.json?.responseCode === 3)

  r = await req('POST', '/api/Plataforma', { corpo: { nome: 'Super Nintendo', fabricante: 'Nintendo', anoLancamento: 1990 }, jar })
  registrar(8, 'POST com nome já existente (Super Nintendo)', '409, responseCode 101', r, r.status === 409 && r.json?.responseCode === 101)

  r = await req('DELETE', '/api/Plataforma/1', { jar })
  registrar(9, 'DELETE /api/Plataforma/1 (tem jogos)', '409, responseCode 102', r, r.status === 409 && r.json?.responseCode === 102)

  const put99 = await req('PUT', '/api/Plataforma/99', { corpo: PLAT_OK, jar })
  const del99 = await req('DELETE', '/api/Plataforma/99', { jar })
  registrar(10, 'PUT e DELETE /api/Plataforma/99', '404, responseCode 100 (os dois)',
    { ...put99, json: put99.json }, put99.status === 404 && put99.json?.responseCode === 100 && del99.status === 404 && del99.json?.responseCode === 100,
    `DELETE: ${del99.status} / ${del99.json?.responseCode}`)

  // Jogos
  r = await req('POST', '/api/Jogo', { corpo: JOGO_OK, jar })
  const idJ = idDoLocation(r)
  registrar(11, 'POST /api/Jogo com gêneros [1,2]', '201, Location, jogo com os gêneros', r,
    r.status === 201 && !!r.headers.get('location') && r.json?.data?.generos?.length === 2, `gêneros: ${r.json?.data?.generos?.length ?? '?'}`)

  r = await req('PUT', `/api/Jogo/${idJ}`, { corpo: { ...JOGO_OK, idsGeneros: [3], ativo: true }, jar })
  registrar(12, 'PUT trocando os gêneros para [3]', '200, gêneros novos (os antigos saem)', r,
    r.status === 200 && r.json?.data?.generos?.length === 1 && r.json?.data?.generos?.[0]?.id === 3)

  r = await req('DELETE', `/api/Jogo/${idJ}`, { jar })
  const aposDelete = await req('GET', `/api/Jogo/${idJ}`, { jar })
  registrar(13, 'DELETE do jogo (exclusão lógica)', '200; o jogo continua, com ativo = false', r,
    r.status === 200 && r.json?.responseCode === 0 && (aposDelete.json?.data?.ativo === false),
    `GET depois: ${aposDelete.status}, ativo = ${aposDelete.json?.data?.ativo}`)

  r = await req('PUT', `/api/Jogo/${idJ}`, { corpo: { ...JOGO_OK, idsGeneros: [1, 2], ativo: true }, jar })
  registrar(14, 'PUT com "ativo": true (reativar)', '200, jogo reativado', r, r.status === 200 && r.json?.data?.ativo === true)

  r = await req('POST', '/api/Jogo', { corpo: { ...JOGO_OK, preco: 0 }, jar })
  registrar(15, 'POST com preço 0', '400, responseCode 3, campo preco', r,
    r.status === 400 && r.json?.responseCode === 3 && chaves(r).includes('preco'), `campos: ${chaves(r).join(', ')}`)

  r = await req('POST', '/api/Jogo', { corpo: { ...JOGO_OK, idsGeneros: [] }, jar })
  registrar(16, 'POST com idsGeneros vazio', '400, responseCode 3, campo idsGeneros', r,
    r.status === 400 && r.json?.responseCode === 3 && chaves(r).includes('idsGeneros'), `campos: ${chaves(r).join(', ')}`)

  r = await req('POST', '/api/Jogo', { corpo: { ...JOGO_OK, idPlataforma: 99 }, jar })
  registrar(17, 'POST com idPlataforma 99', '404, responseCode 100', r, r.status === 404 && r.json?.responseCode === 100)

  r = await req('POST', '/api/Jogo', { corpo: { ...JOGO_OK, idCondicaoJogo: 99 }, jar })
  registrar(18, 'POST com idCondicaoJogo 99', '404, responseCode 104', r, r.status === 404 && r.json?.responseCode === 104)

  r = await req('POST', '/api/Jogo', { corpo: { ...JOGO_OK, idsGeneros: [99] }, jar })
  registrar(19, 'POST com idsGeneros [99]', '404, responseCode 105', r, r.status === 404 && r.json?.responseCode === 105)

  const put9999 = await req('PUT', '/api/Jogo/9999', { corpo: { ...JOGO_OK, ativo: true }, jar })
  const del9999 = await req('DELETE', '/api/Jogo/9999', { jar })
  registrar(20, 'PUT e DELETE /api/Jogo/9999', '404, responseCode 103 (os dois)', put9999,
    put9999.status === 404 && put9999.json?.responseCode === 103 && del9999.status === 404 && del9999.json?.responseCode === 103,
    `DELETE: ${del9999.status} / ${del9999.json?.responseCode}`)

  // Limpeza: jogo nunca é apagado (RN03), então o de teste é desativado.
  await req('DELETE', `/api/Jogo/${idJ}`, { jar })
  console.log(`\nO jogo de teste (id ${idJ}) ficou INATIVO no banco. Para removê-lo, rode database/limpeza_testes.sql no Workbench.`)
}

// =====================================================================
// AULA 6 (cada caso em "janela anônima" = jar novo, salvo quando indicado)
// =====================================================================
async function aula6() {
  if (!ADMIN_EMAIL || !ADMIN_SENHA) {
    console.error('O .env precisa ter AdminInicial__Email e AdminInicial__Senha (rode: node scripts/configurar-env.mjs).')
    process.exit(1)
  }
  const anonimo = new Jar()
  const admin = new Jar()

  let r = await req('POST', '/api/Plataforma', { corpo: PLAT_OK, jar: anonimo })
  registrar(1, 'POST /api/Plataforma sem login', '401, responseCode 4', r, r.status === 401 && r.json?.responseCode === 4)

  r = await req('POST', '/api/Autenticacao/login', { corpo: { email: ADMIN_EMAIL, senha: ADMIN_SENHA + 'x-errada' }, jar: new Jar() })
  registrar(2, 'Login com senha errada', '401, responseCode 6', r, r.status === 401 && r.json?.responseCode === 6)

  r = await req('POST', '/api/Autenticacao/login', { corpo: { email: 'abc', senha: '123' }, jar: new Jar() })
  registrar(3, 'Login com e-mail mal formado', '400, responseCode 3, campo email em data', r,
    r.status === 400 && r.json?.responseCode === 3 && chaves(r).includes('email'), `campos: ${chaves(r).join(', ')}`)

  r = await req('POST', '/api/Autenticacao/login', { corpo: { email: ADMIN_EMAIL, senha: ADMIN_SENHA }, jar: admin })
  const setCookie = (r.headers.getSetCookie?.() ?? []).find((c) => c.startsWith('retrogames.sessao=')) ?? ''
  const semHash = !/hash/i.test(r.texto)
  const flags = `cookie HttpOnly=${/httponly/i.test(setCookie)}, SameSite=Strict=${/samesite=strict/i.test(setCookie)}`
  registrar(4, 'Login do administrador (e-mail e senha do .env)', '200, responseCode 0, sem hash na resposta, cookie retrogames.sessao', r,
    r.status === 200 && r.json?.responseCode === 0 && semHash && !!setCookie && /httponly/i.test(setCookie) && /samesite=strict/i.test(setCookie),
    `${flags}, sem hash: ${semHash}`)

  r = await req('POST', '/api/Plataforma', { corpo: PLAT_OK, jar: admin })
  const idP = idDoLocation(r)
  registrar(5, 'POST /api/Plataforma logado como administrador', '201, responseCode 0', r, r.status === 201 && r.json?.responseCode === 0)
  if (idP) await req('DELETE', `/api/Plataforma/${idP}`, { jar: admin })

  r = await req('GET', '/api/Jogo?incluirInativos=true&tamanhoPagina=100', { jar: new Jar() })
  const inativosAnon = (r.json?.data?.itens ?? []).filter((j) => j.ativo === false).length
  registrar(6, 'GET /api/Jogo?incluirInativos=true sem login', '200, só jogos ativos', r, r.status === 200 && inativosAnon === 0, `inativos na lista: ${inativosAnon}`)

  r = await req('GET', '/api/Jogo?incluirInativos=true&tamanhoPagina=100', { jar: admin })
  const inativosAdmin = (r.json?.data?.itens ?? []).filter((j) => j.ativo === false).length
  registrar(7, 'GET /api/Jogo?incluirInativos=true como administrador', '200, inclui jogo(s) inativo(s) (ex.: Tekken 3)', r,
    r.status === 200 && inativosAdmin > 0, `inativos na lista: ${inativosAdmin}`)

  r = await req('GET', '/api/Jogo/16', { jar: new Jar() })
  registrar(8, 'GET /api/Jogo/16 (inativo) sem login', '404, responseCode 103', r, r.status === 404 && r.json?.responseCode === 103)

  r = await req('GET', '/api/Autenticacao/eu', { jar: admin })
  registrar(9, 'GET /api/Autenticacao/eu logado', '200, dados do usuário', r,
    r.status === 200 && r.json?.data?.email?.toLowerCase() === ADMIN_EMAIL.toLowerCase(), `perfil: ${r.json?.data?.perfil}`)

  const sair = await req('POST', '/api/Autenticacao/logout', { jar: admin })
  r = await req('GET', '/api/Autenticacao/eu', { jar: admin })
  registrar(10, 'POST logout e depois GET eu', '200 e 401 (responseCode 0 e 4)', r,
    sair.status === 200 && sair.json?.responseCode === 0 && r.status === 401 && r.json?.responseCode === 4,
    `logout: ${sair.status} / ${sair.json?.responseCode}; eu depois: ${r.status}`)

  const rotas = ['/api/Plataforma', '/api/Genero', '/api/CondicaoJogo', '/api/Jogo']
  const respostas = []
  for (const rota of rotas) respostas.push(await req('GET', rota, { jar: new Jar() }))
  registrar(11, 'GET de plataformas, gêneros, condições e jogos sem login', '200 em todos', respostas[0],
    respostas.every((x) => x.status === 200), rotas.map((rota, i) => `${rota.split('/').pop()}: ${respostas[i].status}`).join(', '))

  r = await req('GET', '/swagger/index.html', { jar: new Jar() })
  registrar(12, '/swagger sem login', 'abre normalmente (200)', r, r.status === 200)
}

// =====================================================================
await (AULA === '5' ? aula5() : aula6())

const total = linhas.length
const falhas = linhas.filter((l) => !l.ok)
console.log(`\n${total - falhas.length}/${total} casos como esperado.`)

const agora = new Date().toLocaleString('pt-BR')
const titulo = AULA === '5' ? '# Testes da Aula 5 (gravação)' : '# Testes da Aula 6 (acesso)'
let md = `${titulo}\n\n> Resultado gerado automaticamente por \`scripts/testar-api.mjs ${AULA}\` em ${agora}, contra ${BASE}.\n` +
  `> Cada linha vem da resposta real da API. ${AULA === '6' ? 'Cada caso usa uma sessão nova (equivale a janela anônima), exceto os do administrador.' : ''}\n\n` +
  '| # | Caso | Esperado | Obtido | Resultado |\n|---|---|---|---|---|\n'
for (const l of linhas) md += `| ${l.n} | ${l.descricao} | ${l.esperado} | ${l.obtido.replaceAll('|', '/')} | ${l.ok ? 'OK' : '**FALHOU**'} |\n`
md += `\n**${total - falhas.length} de ${total} casos como esperado.**\n`
if (falhas.length) md += `\nCasos que não deram o esperado: ${falhas.map((l) => '#' + l.n).join(', ')}. Investigue a causa (validator, Service, controller ou configuração) antes de entregar.\n`
writeFileSync(`docs/TESTES_AULA_${AULA}.md`, md, 'utf8')
console.log(`Escrito em docs/TESTES_AULA_${AULA}.md`)
process.exit(falhas.length ? 2 : 0)
