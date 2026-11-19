// Cria (ou completa) o arquivo .env na raiz do repositório, perguntando as senhas.
// Nenhuma senha fica no código: os valores digitados vão só para o .env, que está no .gitignore.
// Uso (na raiz do repositório):  node scripts/configurar-env.mjs
import { existsSync, readFileSync, writeFileSync } from 'node:fs'
import { createInterface } from 'node:readline'
import { Writable } from 'node:stream'

const ARQUIVO = '.env'

function lerChaves() {
  if (!existsSync(ARQUIVO)) return new Set()
  return new Set(
    readFileSync(ARQUIVO, 'utf8')
      .split(/\r?\n/)
      .map((linha) => linha.match(/^\s*([A-Za-z0-9_]+)\s*=/)?.[1])
      .filter(Boolean),
  )
}

function perguntar(texto, { oculto = false, padrao = '' } = {}) {
  return new Promise((resolve) => {
    let mudo = false
    const saida = new Writable({
      write(chunk, _enc, cb) {
        if (!mudo) process.stdout.write(chunk)
        cb()
      },
    })
    const rl = createInterface({ input: process.stdin, output: saida, terminal: true })
    process.stdout.write(padrao ? `${texto} [${padrao}]: ` : `${texto}: `)
    mudo = oculto
    rl.question('', (resposta) => {
      rl.close()
      if (oculto) process.stdout.write('\n')
      resolve(resposta.trim() || padrao)
    })
  })
}

function valido(valor, nome) {
  if (!valor) {
    console.error(`${nome} não pode ficar vazio.`)
    return false
  }
  if (/["\r\n]/.test(valor)) {
    console.error(`${nome} não pode conter aspas nem quebra de linha.`)
    return false
  }
  return true
}

const chaves = lerChaves()
const novas = []

if (!chaves.has('ConnectionStrings__Default')) {
  console.log('Connection string do MySQL (deixe o padrão se estiver igual ao do seu Workbench).')
  const servidor = await perguntar('Servidor', { padrao: 'localhost' })
  const porta = await perguntar('Porta', { padrao: '3306' })
  const banco = await perguntar('Banco', { padrao: 'retrogames' })
  const usuario = await perguntar('Usuário', { padrao: 'root' })
  const senha = await perguntar('Senha do MySQL', { oculto: true })
  if (!valido(senha, 'A senha do MySQL') || senha.includes(';')) {
    console.error('Senha inválida (vazia, com aspas ou com ponto e vírgula). Nada foi gravado.')
    process.exit(1)
  }
  novas.push(`ConnectionStrings__Default="server=${servidor};port=${porta};database=${banco};user=${usuario};password=${senha}"`)
} else {
  console.log('ConnectionStrings__Default já existe no .env: mantida.')
}

if (!chaves.has('AdminInicial__Email')) {
  const email = await perguntar('E-mail do administrador inicial', { padrao: 'admin@retrogames.com' })
  if (!valido(email, 'O e-mail')) process.exit(1)
  novas.push(`AdminInicial__Email="${email}"`)
} else {
  console.log('AdminInicial__Email já existe no .env: mantido.')
}

if (!chaves.has('AdminInicial__Senha')) {
  const senhaAdmin = await perguntar('Senha do administrador inicial (você usará no login)', { oculto: true })
  if (!valido(senhaAdmin, 'A senha do administrador')) process.exit(1)
  novas.push(`AdminInicial__Senha="${senhaAdmin}"`)
} else {
  console.log('AdminInicial__Senha já existe no .env: mantida.')
}

if (novas.length === 0) {
  console.log('Nada a acrescentar: o .env já está completo.')
} else {
  const atual = existsSync(ARQUIVO) ? readFileSync(ARQUIVO, 'utf8') : ''
  const separador = atual && !atual.endsWith('\n') ? '\n' : ''
  writeFileSync(ARQUIVO, atual + separador + novas.join('\n') + '\n', 'utf8')
  console.log(`Gravado em ${ARQUIVO}: ${novas.length} linha(s) nova(s).`)
}
console.log('Confira: "git status" NÃO pode listar o .env.')
