<?= $this->extend('layouts/main') ?>
<?= $this->section('content') ?>

<section class="role-choice">
    <div class="card shadow-sm role-choice-card text-center">
        <h1 class="h3 fw-bold mb-4">Pilih Login Sebagai</h1>
        <div class="row g-3 justify-content-center">
            <div class="col-md-5">
                <a class="btn btn-success btn-lg w-100 py-3" href="<?= site_url('enter/seller') ?>">Seller</a>
            </div>
            <div class="col-md-5">
                <a class="btn btn-outline-success btn-lg w-100 py-3" href="<?= site_url('enter/buyer') ?>">Buyer</a>
            </div>
        </div>
    </div>
</section>

<?= $this->endSection() ?>
