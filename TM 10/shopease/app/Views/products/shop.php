<?= $this->extend('layouts/main') ?>
<?= $this->section('content') ?>

<div class="mb-4">
    <h1 class="h2 fw-bold mb-1">Available Products</h1>
</div>

<div class="row g-4">
    <?php if (empty($products)): ?>
        <div class="col-12"><div class="card"><div class="card-body p-5 text-center text-secondary">No products are available right now.</div></div></div>
    <?php else: ?>
        <?php foreach ($products as $product): ?>
        <div class="col-sm-6 col-lg-4">
            <article class="card shadow-sm shop-card overflow-hidden">
                <?php if ($product['image']): ?>
                    <img class="card-img-top shop-image" src="<?= base_url('uploads/products/' . $product['image']) ?>" alt="<?= esc($product['name']) ?>">
                <?php else: ?>
                    <div class="shop-image d-grid place-items-center d-flex align-items-center justify-content-center text-secondary">ShopEase</div>
                <?php endif; ?>
                <div class="card-body d-flex flex-column p-4">
                    <div class="small text-success fw-semibold mb-2"><?= esc($product['category_name']) ?></div>
                    <h2 class="h5 fw-semibold"><?= esc($product['name']) ?></h2>
                    <p class="text-secondary flex-grow-1"><?= esc(mb_strimwidth((string) $product['description'], 0, 110, '…')) ?></p>
                    <div class="d-flex justify-content-between align-items-center mt-2">
                        <span class="fw-bold fs-5">Rp <?= number_format((float) $product['price'], 0, ',', '.') ?></span>
                        <a href="<?= site_url('shop/' . $product['id']) ?>" class="btn btn-success">View details</a>
                    </div>
                </div>
            </article>
        </div>
        <?php endforeach; ?>
    <?php endif; ?>
</div>

<?= $this->endSection() ?>
