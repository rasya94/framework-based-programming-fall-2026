<!DOCTYPE html>
<html>
<head>
    <title>Edit Data User</title>
</head>
<body>

    <h1>Edit Data User</h1>

    <form action="<?= base_url('crud/update') ?>" method="post">

        <input type="hidden" name="id" value="<?= esc($user['id']) ?>">

        <label>Nama</label><br>
        <input type="text" name="nama" value="<?= esc($user['nama']) ?>" required><br><br>

        <label>Alamat</label><br>
        <textarea name="alamat" required><?= esc($user['alamat']) ?></textarea><br><br>

        <label>Pekerjaan</label><br>
        <input type="text" name="pekerjaan" value="<?= esc($user['pekerjaan']) ?>" required><br><br>

        <button type="submit">Update</button>

    </form>

    <br>
    <a href="<?= base_url('crud') ?>">Kembali</a>

</body>
</html>