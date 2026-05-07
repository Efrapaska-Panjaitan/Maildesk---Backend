-- ============================================================
-- MailDesk Database — Sprint 1 (Updated Schema)
-- ============================================================

CREATE TABLE IF NOT EXISTS roles (
    id        SERIAL PRIMARY KEY,
    nama_role VARCHAR(50) NOT NULL
);

CREATE TABLE IF NOT EXISTS users (
    id         SERIAL PRIMARY KEY,
    nama       VARCHAR(100) NOT NULL,
    email      VARCHAR(100) UNIQUE NOT NULL,  -- ← BARU
    password   VARCHAR(255) NOT NULL,          -- ← BARU
    role_id    INT REFERENCES roles(id),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS template_surat (  -- ← TABEL BARU
    id            SERIAL PRIMARY KEY,
    nama_template VARCHAR(100) NOT NULL,
    isi_template  TEXT NOT NULL,
    dibuat_oleh   INT REFERENCES users(id),
    created_at    TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS surat (  -- ← GANTI surat_masuk + surat_keluar
    id             SERIAL PRIMARY KEY,
    no_surat       VARCHAR(100),
    nomor_agenda   VARCHAR(100) UNIQUE,
    jenis_surat    VARCHAR(20) CHECK (jenis_surat IN ('Masuk', 'Keluar')),
    kategori_surat VARCHAR(50),
    tanggal_surat  DATE NOT NULL,
    pengirim       VARCHAR(150) NOT NULL,
    penerima       VARCHAR(150) NOT NULL,
    perihal        TEXT NOT NULL,
    isi_teks_ocr   TEXT,
    file_lampiran  BYTEA,
    nama_file      VARCHAR(255),
    status         VARCHAR(50) DEFAULT 'Baru',
    is_archived    BOOLEAN DEFAULT FALSE,
    user_id        INT REFERENCES users(id),
    created_at     TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS inbox (  -- ← TABEL BARU
    id                SERIAL PRIMARY KEY,
    surat_id          INT REFERENCES surat(id),
    pengirim_id       INT REFERENCES users(id),
    penerima_id       INT REFERENCES users(id),
    status            VARCHAR(50) DEFAULT 'Menunggu Tindakan',
    catatan_pengantar TEXT,
    created_at        TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS disposisi (
    id                SERIAL PRIMARY KEY,
    surat_id          INT REFERENCES surat(id),  -- ← BERUBAH dari surat_masuk_id
    pemberi_id        INT REFERENCES users(id),
    penerima_id       INT REFERENCES users(id),
    tanggal_disposisi DATE NOT NULL,
    sifat_disposisi   VARCHAR(50),
    instruksi         TEXT NOT NULL,
    status            VARCHAR(50) DEFAULT 'Pending',  -- ← BARU
    created_at        TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    waktu_diterima    TIMESTAMP,   -- ← BARU
    completed_at      TIMESTAMP    -- ← BARU
);

CREATE TABLE IF NOT EXISTS disposisi_relation (  -- ← GANTI tracking_disposisi
    id         SERIAL PRIMARY KEY,
    parent_id  INT REFERENCES disposisi(id),
    child_id   INT REFERENCES disposisi(id),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Indexing
CREATE INDEX IF NOT EXISTS idx_surat_tanggal
    ON surat(tanggal_surat);
CREATE INDEX IF NOT EXISTS idx_surat_nomor_agenda
    ON surat(nomor_agenda);
CREATE INDEX IF NOT EXISTS idx_surat_jenis
    ON surat(jenis_surat);
CREATE INDEX IF NOT EXISTS idx_surat_pengirim
    ON surat(pengirim);