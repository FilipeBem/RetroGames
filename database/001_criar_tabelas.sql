-- =====================================================================
-- RetroGames - 001_criar_tabelas.sql
-- Cria o banco e as tabelas do dicionario de dados (Espec. 4.1 a 4.3).
-- O banco guarda so a estrutura: PK, FK (comportamento padrao), tipos,
-- NOT NULL e UNIQUE. Sem trigger, function, CHECK, ON DELETE CASCADE
-- ou ON DELETE SET NULL. Regras de negocio ficam no Service.
-- Um script ja executado nao deve ser alterado: mudancas vao em um novo script.
-- =====================================================================

CREATE DATABASE IF NOT EXISTS retrogames
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_0900_ai_ci;

USE retrogames;

-- ---------------------------------------------------------------------
-- Tabelas de dominio (chave fixa, sem AUTO_INCREMENT)
-- ---------------------------------------------------------------------
CREATE TABLE tb_condicao_jogo (
    co_condicao_jogo  INT         NOT NULL,
    ds_condicao_jogo  VARCHAR(50) NOT NULL,
    CONSTRAINT pk_condicao_jogo PRIMARY KEY (co_condicao_jogo),
    CONSTRAINT uq_condicao_jogo_ds UNIQUE (ds_condicao_jogo)
);

CREATE TABLE tb_perfil (
    co_perfil  INT         NOT NULL,
    ds_perfil  VARCHAR(50) NOT NULL,
    CONSTRAINT pk_perfil PRIMARY KEY (co_perfil),
    CONSTRAINT uq_perfil_ds UNIQUE (ds_perfil)
);

-- ---------------------------------------------------------------------
-- Cadastros
-- ---------------------------------------------------------------------
CREATE TABLE tb_plataforma (
    co_seq_plataforma  INT         NOT NULL AUTO_INCREMENT,
    no_plataforma      VARCHAR(80) NOT NULL,
    no_fabricante      VARCHAR(80) NOT NULL,
    nu_ano_lancamento  INT         NOT NULL,
    CONSTRAINT pk_plataforma PRIMARY KEY (co_seq_plataforma),
    CONSTRAINT uq_plataforma_no UNIQUE (no_plataforma)
);

CREATE TABLE tb_genero (
    co_seq_genero  INT         NOT NULL AUTO_INCREMENT,
    no_genero      VARCHAR(50) NOT NULL,
    CONSTRAINT pk_genero PRIMARY KEY (co_seq_genero),
    CONSTRAINT uq_genero_no UNIQUE (no_genero)
);

CREATE TABLE tb_jogo (
    co_seq_jogo        INT            NOT NULL AUTO_INCREMENT,
    no_jogo            VARCHAR(150)   NOT NULL,
    ds_jogo            VARCHAR(1000)  NULL,
    nu_ano_lancamento  INT            NOT NULL,
    vl_preco           DECIMAL(10,2)  NOT NULL,
    qt_estoque         INT            NOT NULL,
    st_ativo           BOOLEAN        NOT NULL DEFAULT TRUE,
    dt_cadastro        DATETIME       NOT NULL,
    co_plataforma      INT            NOT NULL,
    co_condicao_jogo   INT            NOT NULL,
    CONSTRAINT pk_jogo PRIMARY KEY (co_seq_jogo),
    CONSTRAINT fk_jogo_plataforma FOREIGN KEY (co_plataforma)
        REFERENCES tb_plataforma (co_seq_plataforma),
    CONSTRAINT fk_jogo_condicao FOREIGN KEY (co_condicao_jogo)
        REFERENCES tb_condicao_jogo (co_condicao_jogo)
);

CREATE INDEX ix_jogo_no_jogo ON tb_jogo (no_jogo);

-- ---------------------------------------------------------------------
-- Associativa N:N entre jogo e genero (chave primaria composta)
-- ---------------------------------------------------------------------
CREATE TABLE tb_jogo_genero (
    co_jogo    INT NOT NULL,
    co_genero  INT NOT NULL,
    CONSTRAINT pk_jogo_genero PRIMARY KEY (co_jogo, co_genero),
    CONSTRAINT fk_jogo_genero_jogo FOREIGN KEY (co_jogo)
        REFERENCES tb_jogo (co_seq_jogo),
    CONSTRAINT fk_jogo_genero_genero FOREIGN KEY (co_genero)
        REFERENCES tb_genero (co_seq_genero)
);

-- ---------------------------------------------------------------------
-- Usuarios
-- ---------------------------------------------------------------------
CREATE TABLE tb_usuario (
    co_seq_usuario  INT           NOT NULL AUTO_INCREMENT,
    no_usuario      VARCHAR(100)  NOT NULL,
    ds_email        VARCHAR(256)  NOT NULL,
    ds_senha_hash   VARCHAR(500)  NOT NULL,
    st_ativo        BOOLEAN       NOT NULL DEFAULT TRUE,
    dt_cadastro     DATETIME      NOT NULL,
    co_perfil       INT           NOT NULL,
    CONSTRAINT pk_usuario PRIMARY KEY (co_seq_usuario),
    CONSTRAINT uq_usuario_email UNIQUE (ds_email),
    CONSTRAINT fk_usuario_perfil FOREIGN KEY (co_perfil)
        REFERENCES tb_perfil (co_perfil)
);
