-- Roles (berubah dari sebelumnya)
INSERT INTO roles (nama_role) VALUES
    ('Admin'),
    ('Sekretaris'),
    ('Pimpinan'),
    ('Karyawan')
ON CONFLICT DO NOTHING;

-- Users (tambah email & password)
INSERT INTO users (nama, email, password, role_id) VALUES
    ('Admin TU',          'admin@demo.com',      'hashed', 1),
    ('Sekretaris',        'sekretaris@demo.com', 'hashed', 2),
    ('Pimpinan',          'pimpinan@demo.com',   'hashed', 3),
    ('Manager Umum',      'manager@demo.com',    'hashed', 4),
    ('Kepala Departemen', 'kadep@demo.com',       'hashed', 4),
    ('Staff',             'staff@demo.com',       'hashed', 4)
ON CONFLICT DO NOTHING;

-- Sample surat masuk
INSERT INTO surat (no_surat, nomor_agenda, jenis_surat, tanggal_surat,
    pengirim, penerima, perihal, status, user_id)
VALUES
    ('421.3/B.1/DISDIK/2026', 'SM/2026/05/001', 'Masuk', '2026-05-01',
     'Dinas Pendidikan Kab. Bogor', 'Sekretaris', 'Undangan Workshop', 'Baru', 2),
    ('B-500/PUPR/2026', 'SM/2026/05/002', 'Masuk', '2026-05-02',
     'Kementerian PUPR', 'Sekretaris', 'Rapat Koordinasi', 'Baru', 2)
ON CONFLICT DO NOTHING;