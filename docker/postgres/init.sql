-- ============================================================
-- MailDesk Database — Sprint 1
-- PostgreSQL Schema
-- ============================================================

-- Tabel Roles
CREATE TABLE IF NOT EXISTS roles (
    id SERIAL PRIMARY KEY,
    nama_role VARCHAR(50) NOT NULL
);

-- Tabel Users
CREATE TABLE IF NOT EXISTS users (
    id SERIAL PRIMARY KEY,
    nama VARCHAR(100) NOT NULL,
    role_id INT REFERENCES roles(id),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabel Surat Masuk
CREATE TABLE IF NOT EXISTS surat_masuk (
    id SERIAL PRIMARY KEY,
    no_surat VARCHAR(100) NOT NULL,
    nomor_agenda VARCHAR(100) UNIQUE,
    tanggal_surat DATE NOT NULL,
    asal_pengirim VARCHAR(150) NOT NULL,
    perihal TEXT NOT NULL,
    file_lampiran BYTEA,
    nama_file VARCHAR(255),
    file_path VARCHAR(500),
    is_archived BOOLEAN DEFAULT FALSE,
    user_id INT REFERENCES users(id),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabel Surat Keluar
CREATE TABLE IF NOT EXISTS surat_keluar (
    id SERIAL PRIMARY KEY,
    no_surat VARCHAR(100),
    nomor_agenda VARCHAR(100) UNIQUE,
    tanggal_surat DATE,
    kepada VARCHAR(150) NOT NULL,
    perihal TEXT NOT NULL,
    file_lampiran BYTEA,
    nama_file VARCHAR(255),
    status VARCHAR(50) DEFAULT 'Draft',
    is_archived BOOLEAN DEFAULT FALSE,
    pembuat_id INT REFERENCES users(id),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabel Disposisi
CREATE TABLE IF NOT EXISTS disposisi (
    id SERIAL PRIMARY KEY,
    surat_masuk_id INT REFERENCES surat_masuk(id),
    pemberi_id INT REFERENCES users(id),
    penerima_id INT REFERENCES users(id),
    tanggal_disposisi DATE NOT NULL,
    sifat_disposisi VARCHAR(50),
    instruksi TEXT NOT NULL,
    nomor_agenda VARCHAR(100) UNIQUE,
    is_archived BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabel Tracking Disposisi
CREATE TABLE IF NOT EXISTS tracking_disposisi (
    id SERIAL PRIMARY KEY,
    disposisi_id INT REFERENCES disposisi(id),
    user_id INT REFERENCES users(id),
    status VARCHAR(50),
    catatan TEXT,
    waktu_update TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ============================================================
-- Indexing untuk optimasi pencarian (Sprint 1 — task Akmal)
-- ============================================================
CREATE INDEX IF NOT EXISTS idx_surat_masuk_tanggal
    ON surat_masuk(tanggal_surat);

CREATE INDEX IF NOT EXISTS idx_surat_masuk_nomor_agenda
    ON surat_masuk(nomor_agenda);

CREATE INDEX IF NOT EXISTS idx_surat_masuk_asal_pengirim
    ON surat_masuk(asal_pengirim);