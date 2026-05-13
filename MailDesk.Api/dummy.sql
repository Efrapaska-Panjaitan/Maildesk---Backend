-- 1. Bikin 1 Surat Baru (Otomatis jadi ID 1)
INSERT INTO surat (no_surat, tanggal_surat, pengirim, penerima, perihal, status, user_id) 
VALUES ('001/MILDESK/V/2026', '2026-05-12', 'PT Coba Coba', 'Pimpinan', 'Kerja Sama Aplikasi', 'Diproses', 1);

-- 2. Bikin Riwayat Masuk Inbox (Dikirim ke Sekretaris)
INSERT INTO inbox (surat_id, pengirim_id, penerima_id, status, catatan_pengantar) 
VALUES (1, 1, 2, 'Belum Dibaca', 'Tolong dicek kelengkapan proposalnya ya.');

-- 3. Bikin Riwayat Disposisi (Dari Sekretaris diteruskan ke Pimpinan)
INSERT INTO disposisi (surat_id, pemberi_id, penerima_id, tanggal_disposisi, sifat_disposisi, instruksi) 
VALUES (1, 2, 3, '2026-05-12', 'Penting', 'Proposal sudah lengkap, mohon direview.');