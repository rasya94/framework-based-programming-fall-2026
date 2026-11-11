<?= $this->extend('layouts/main') ?>
<?= $this->section('content') ?>

<a href="<?= site_url('shop') ?>" class="text-decoration-none text-success">&larr; Back to shop</a>
<div class="card shadow-sm mt-3 overflow-hidden">
    <div class="row g-0">
        <div class="col-md-6">
            <?php if ($product['image']): ?>
                <img src="<?= base_url('uploads/products/' . $product['image']) ?>" alt="<?= esc($product['name']) ?>" class="w-100 h-100 object-fit-cover product-image">
            <?php else: ?>
                <div class="w-100 h-100 d-flex align-items-center justify-content-center text-secondary product-image">ShopEase</div>
            <?php endif; ?>
        </div>
        <div class="col-md-6">
            <div class="card-body p-4 p-lg-5">
                <div class="text-success fw-semibold mb-2"><?= esc($product['category_name']) ?></div>
                <h1 class="h2 fw-bold"><?= esc($product['name']) ?></h1>
                <div class="fs-3 fw-bold my-3">Rp <?= number_format((float) $product['price'], 0, ',', '.') ?></div>
                <p class="text-secondary"><?= nl2br(esc($product['description'] ?: 'A quality find from ShopEase.')) ?></p>
                <p class="small text-secondary">In stock: <?= esc($product['stock']) ?></p>
                <button class="btn btn-success btn-lg mt-2" type="button" disabled title="Checkout is not set up yet">Add to cart · Coming soon</button>
            </div>
        </div>
    </div>
</div>

<?= $this->endSection() ?>
