<!DOCTYPE html>
<html>

<head>
    <title>Data User</title>
</head>

<body>

    <h1>Data User</h1>

    <table border="1" cellpadding="10">
        <tr>
            <th>No</th>
            <th>Nama</th>
            <th>Alamat</th>
            <th>Pekerjaan</th>
            <th>Edit</th>
            <th>Delete</th>
        </tr>

        <?php
        $no = 1;
        foreach ($user as $u):
            ?>

            <tr>
                <td><?= $no++; ?></td>
                <td><?= esc($u['nama']); ?></td>
                <td><?= esc($u['alamat']); ?></td>
                <td><?= esc($u['pekerjaan']); ?></td>

                <td>
                    <a href="<?= base_url('crud/edit/' . $u['id']) ?>">Edit</a>
                </td>

                <td>
                    <a href="<?= base_url('crud/hapus/' . $u['id']) ?>"
                        onclick="return confirm('Yakin ingin menghapus data ini?')">
                        Delete
                    </a>
                </td>
            </tr>

        <?php endforeach; ?>

    </table>

    <br><br>
    <a href="<?= base_url('crud/tambah') ?>">Tambah Karyawan</a>

</body>

</html>