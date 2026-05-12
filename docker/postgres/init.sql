-- ─────────────────────────────────────────────────
-- ROLES
-- ─────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS roles (
    id          SERIAL PRIMARY KEY,
    nama_role   VARCHAR(50) NOT NULL UNIQUE
);

-- ─────────────────────────────────────────────────
-- USERS
-- ─────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS users (
    id          SERIAL PRIMARY KEY,
    nama        VARCHAR(150) NOT NULL,
    email       VARCHAR(100) NOT NULL UNIQUE,
    password    VARCHAR(255) NOT NULL,
    role_id     INT REFERENCES roles(id) ON DELETE SET NULL,
    created_at  TIMESTAMP DEFAULT NOW()
);

-- ─────────────────────────────────────────────────
-- SURAT
-- ─────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS surat (
    id              SERIAL PRIMARY KEY,
    no_surat        VARCHAR(100),
    nomor_agenda    VARCHAR(100) UNIQUE,
    jenis_surat     VARCHAR(20) NOT NULL CHECK (jenis_surat IN ('Masuk', 'Keluar')),
    kategori_surat  VARCHAR(50),
    tanggal_surat   DATE NOT NULL,
    pengirim        VARCHAR(150) NOT NULL,
    penerima        VARCHAR(150) NOT NULL,  -- nama penerima yang tertulis di surat fisik
    perihal         TEXT NOT NULL,
    isi_teks_ocr    TEXT,
    file_lampiran   BYTEA,
    nama_file       VARCHAR(255),
    status          VARCHAR(20) DEFAULT 'Baru'
                    CHECK (status IN ('Baru', 'Diproses', 'Selesai')),
    is_archived     BOOLEAN DEFAULT FALSE,
    user_id         INT REFERENCES users(id) ON DELETE SET NULL,  -- TU/Sekretaris yang mencatat
    ditujukan_ke_id INT REFERENCES users(id) ON DELETE SET NULL,  -- Pimpinan tujuan disposisi
    created_at      TIMESTAMP DEFAULT NOW()
);

-- ─────────────────────────────────────────────────
-- TEMPLATE SURAT
-- ─────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS template_surat (
    id              SERIAL PRIMARY KEY,
    nama_template   VARCHAR(150) NOT NULL,
    isi_template    TEXT,
    dibuat_oleh     INT REFERENCES users(id) ON DELETE SET NULL,
    created_at      TIMESTAMP DEFAULT NOW()
);

-- ─────────────────────────────────────────────────
-- INBOX
-- ─────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS inbox (
    id                  SERIAL PRIMARY KEY,
    surat_id            INT NOT NULL REFERENCES surat(id) ON DELETE CASCADE,
    pengirim_id         INT REFERENCES users(id) ON DELETE SET NULL,
    penerima_id         INT REFERENCES users(id) ON DELETE SET NULL,
    status              VARCHAR(20) DEFAULT 'Belum Dibaca',
    catatan_pengantar   TEXT,
    created_at          TIMESTAMP DEFAULT NOW()
);

-- ─────────────────────────────────────────────────
-- DISPOSISI
-- ─────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS disposisi (
    id                  SERIAL PRIMARY KEY,
    surat_id            INT NOT NULL REFERENCES surat(id) ON DELETE RESTRICT,
    pemberi_id          INT NOT NULL REFERENCES users(id) ON DELETE RESTRICT,
    penerima_id         INT NOT NULL REFERENCES users(id) ON DELETE RESTRICT,
    tanggal_disposisi   DATE NOT NULL,
    sifat_disposisi     VARCHAR(20) NOT NULL DEFAULT 'Biasa'
                        CHECK (sifat_disposisi IN ('Biasa', 'Penting', 'Mendesak', 'Rahasia')),
    instruksi           TEXT,
    status              VARCHAR(20) NOT NULL DEFAULT 'Pending'
                        CHECK (status IN ('Pending', 'Accepted', 'Completed')),
    waktu_diterima      TIMESTAMP,
    completed_at        TIMESTAMP,
    created_at          TIMESTAMP DEFAULT NOW()
);

-- ─────────────────────────────────────────────────
-- DISPOSISI RELATION (chain tracking)
-- ─────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS disposisi_relation (
    id          SERIAL PRIMARY KEY,
    parent_id   INT NOT NULL REFERENCES disposisi(id) ON DELETE RESTRICT,
    child_id    INT NOT NULL REFERENCES disposisi(id) ON DELETE RESTRICT,
    created_at  TIMESTAMP DEFAULT NOW(),
    UNIQUE (child_id)   -- satu child hanya boleh punya satu parent
);