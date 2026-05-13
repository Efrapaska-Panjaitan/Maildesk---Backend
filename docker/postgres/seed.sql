-- ─────────────────────────────────────────────────
-- SEED ROLES
-- ─────────────────────────────────────────────────
INSERT INTO roles (nama_role) VALUES
    ('Admin'),
    ('Sekretaris'),
    ('Pimpinan'),
    ('Karyawan')
ON CONFLICT DO NOTHING;

-- ─────────────────────────────────────────────────
-- SEED USERS
-- ─────────────────────────────────────────────────
INSERT INTO users (nama, email, password, role_id) VALUES
    ('Admin Sistem',    'admin@maildesk.id',      'hashed_password', 1),
    ('Sekretaris',      'sekretaris@maildesk.id', 'hashed_password', 2),
    ('Direktur Utama',  'direktur@maildesk.id',   'hashed_password', 3),
    ('Dir. Operasional','operasional@maildesk.id', 'hashed_password', 3),
    ('Staff',           'staff@maildesk.id',       'hashed_password', 4)
ON CONFLICT DO NOTHING;

-- ─────────────────────────────────────────────────
-- SEED SURAT
-- ─────────────────────────────────────────────────
INSERT INTO surat (no_surat, nomor_agenda, jenis_surat, kategori_surat,
                   tanggal_surat, pengirim, penerima, perihal, status,
                   user_id, ditujukan_ke_id)
VALUES
    -- user_id=2 (Sekretaris mencatat), ditujukan_ke_id=3 (Direktur Utama)
    -- penerima = nama yang tertulis di surat fisik
    ('421.3/B.1/DISDIK/2026', 'SM/2026/05/001', 'Masuk', 'Undangan',
     '2026-05-01', 'Dinas Pendidikan Kab. Bogor', 'Kepala Dinas',
     'Undangan Workshop Digitalisasi', 'Baru', 2, 3),

    ('B-500/PUPR/2026', 'SM/2026/05/002', 'Masuk', NULL,
     '2026-05-02', 'Kementerian PUPR', 'Direktur Utama',
     'Rapat Koordinasi', 'Diproses', 2, 3)
ON CONFLICT DO NOTHING;


-- ─────────────────────────────────────────────────
-- SEED DISPOSISI
-- ─────────────────────────────────────────────────
INSERT INTO disposisi (surat_id, pemberi_id, penerima_id, tanggal_disposisi,
                        sifat_disposisi, instruksi, status, created_at)
VALUES
    -- Disposisi pertama: Direktur Utama → Dir. Operasional (untuk surat ID 2)
    (2, 3, 4, '2026-05-02', 'Penting',
     'Mohon segera ditindaklanjuti dan laporkan hasilnya.',
     'Accepted', '2026-05-02 07:00:00'),

    -- Disposisi kedua: Dir. Operasional → Staff (chain dari disposisi ID 1)
    (2, 4, 5, '2026-05-02', 'Biasa',
     'Siapkan bahan rapat dan koordinasi tim.',
     'Pending', '2026-05-02 10:00:00')
ON CONFLICT DO NOTHING;

-- ─────────────────────────────────────────────────
-- SEED DISPOSISI RELATION (chain: disposisi 1 → disposisi 2)
-- ─────────────────────────────────────────────────
INSERT INTO disposisi_relation (parent_id, child_id, created_at)
VALUES
    (1, 2, '2026-05-02 10:00:00')
ON CONFLICT DO NOTHING;