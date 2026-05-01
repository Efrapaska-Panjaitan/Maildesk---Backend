-- ============================================================
-- Seed Data — MailDesk Sprint 1
-- Data demo untuk keperluan testing frontend/mobile
-- ============================================================

-- Roles
INSERT INTO roles (nama_role) VALUES
    ('Admin'),
    ('TU'),
    ('Sekretaris'),
    ('Pimpinan'),
    ('User'),
    ('Yang Bersangkutan')
ON CONFLICT DO NOTHING;

-- Users demo
INSERT INTO users (nama, role_id) VALUES
    ('Admin Sistem',   1),  -- Admin
    ('Budi Santoso',   2),  -- TU
    ('Lizzy Sekre',    3),  -- Sekretaris
    ('Direktur Utama', 4),  -- Pimpinan
    ('Akmal Staff',    5),  -- User
    ('Efra Backend',   5)   -- User
ON CONFLICT DO NOTHING;

-- Sample surat masuk
INSERT INTO surat_masuk (no_surat, nomor_agenda, tanggal_surat, asal_pengirim, perihal, user_id)
VALUES
    ('421.3/B.1/DISDIK/2026', 'SM/2026/05/001', '2026-05-01',
     'Dinas Pendidikan Kab. Bogor', 'Undangan Workshop Digitalisasi', 2),
    ('B-500/PUPR/2026',       'SM/2026/05/002', '2026-05-02',
     'Kementerian PUPR',           'Rapat Koordinasi Infrastruktur', 2)
ON CONFLICT DO NOTHING;