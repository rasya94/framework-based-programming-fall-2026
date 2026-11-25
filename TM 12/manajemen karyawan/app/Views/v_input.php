<!DOCTYPE html>
<html>
<head>
    <title>Tambah Data User</title>
</head>
<body>

    <h1>Tambah Data User</h1>

    <form action="<?= base_url('crud/tambah_aksi') ?>" method="post">

        <label>Nama</label><br>
        <input type="text" name="nama" required><br><br>

        <label>Alamat</label><br>
        <textarea name="alamat" required></textarea><br><br>

        <label>Pekerjaan</label><br>
        <input type="text" name="pekerjaan" required><br><br>

        <button type="submit">Simpan</button>

    </form>

    <br>
    <a href="<?= base_url('crud') ?>">Kembali</a>

</body>
</html>