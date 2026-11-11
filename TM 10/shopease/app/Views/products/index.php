<?= $this->extend('layouts/main') ?>
<?= $this->section('content') ?>

<div class="d-flex justify-content-between align-items-center mb-3">
    <div>
        <h1 class="h3 mb-1">Products</h1>
        <p class="text-secondary mb-0">Manage the ShopEase product list.</p>
    </div>
    <a href="<?= site_url('products/new') ?>" class="btn btn-success">Add Product</a>
</div>

<div class="card shadow-sm">
    <div class="table-responsive">
        <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
                <tr><th>Product</th><th>Category</th><th>Price</th><th>Stock</th><th>Status</th><th class="text-end">Actions</th></tr>
            </thead>
            <tbody>
            <?php if (empty($products)): ?>
                <tr><td colspan="6" class="text-center text-secondary py-4">No products yet.</td></tr>
            <?php else: ?>
                <?php foreach ($products as $product): ?>
                    <tr>
                        <td>
                            <div class="d-flex align-items-center gap-2">
                                <?php if ($product['image']): ?>
                                    <img src="<?= base_url('uploads/products/' . $product['image']) ?>" alt="" width="44" height="44" class="rounded object-fit-cover">
                                <?php endif; ?>
                                <span class="fw-medium"><?= esc($product['name']) ?></span>
                            </div>
                        </td>
                        <td><?= esc($product['category_name']) ?></td>
                        <td>Rp <?= number_format((float) $product['price'], 0, ',', '.') ?></td>
                        <td><?= esc($product['stock']) ?></td>
                        <td><span class="badge <?= $product['status'] === 'active' ? 'text-bg-success' : 'text-bg-secondary' ?>"><?= esc(ucfirst($product['status'])) ?></span></td>
                        <td class="text-end text-nowrap">
                            <a class="btn btn-sm btn-outline-secondary" href="<?= site_url('products/' . $product['id'] . '/edit') ?>">Edit</a>
                            <form class="d-inline" method="post" action="<?= site_url('products/' . $product['id'] . '/delete') ?>" onsubmit="return confirm('Delete this product?')">
                                <?= csrf_field() ?>
                                <button class="btn btn-sm btn-outline-danger" type="submit">Delete</button>
                            </form>
                        </td>
                    </tr>
                <?php endforeach; ?>
            <?php endif; ?>
            </tbody>
        </table>
    </div>
</div>

<?= $this->endSection() ?>
