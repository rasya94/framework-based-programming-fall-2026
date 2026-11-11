<?= $this->extend('layouts/main') ?>
<?= $this->section('content') ?>

<div class="row justify-content-center">
    <div class="col-lg-8">
        <div class="mb-3">
            <h1 class="h3 mb-1"><?= esc($pageTitle) ?></h1>
            <p class="text-secondary mb-0">Enter the product information.</p>
        </div>

        <?php $errors = session()->getFlashdata('errors') ?? []; ?>
        <?php if ($errors): ?>
            <div class="alert alert-danger">
                <?php foreach ($errors as $error): ?><div><?= esc($error) ?></div><?php endforeach; ?>
            </div>
        <?php endif; ?>

        <div class="card shadow-sm">
            <div class="card-body p-4">
                <form method="post" action="<?= esc($formAction) ?>" enctype="multipart/form-data">
                    <?= csrf_field() ?>
                    <div class="mb-4">
                        <label class="form-label">Product Name</label>
                        <input class="form-control" type="text" name="name" maxlength="150" required value="<?= esc(old('name', $product['name'] ?? '')) ?>">
                    </div>
                    <div class="mb-4">
                        <label class="form-label">Category</label>
                        <select class="form-select" name="category_id" required>
                            <option value="">Choose category</option>
                            <?php foreach ($categories as $category): ?>
                                <option value="<?= $category['id'] ?>" <?= (string) old('category_id', $product['category_id'] ?? '') === (string) $category['id'] ? 'selected' : '' ?>>
                                    <?= esc($category['name']) ?>
                                </option>
                            <?php endforeach; ?>
                        </select>
                    </div>

                    <div class="mb-4">
                        <label class="form-label">Description</label>
                        <textarea class="form-control" name="description" rows="3"><?= esc(old('description', $product['description'] ?? '')) ?></textarea>
                    </div>

                    <div class="mb-4">
                        <label class="form-label">Price (Rp)</label>
                        <input class="form-control" type="number" name="price" min="0" step="0.01" required value="<?= esc(old('price', $product['price'] ?? '')) ?>">
                    </div>
                    <div class="mb-4">
                        <label class="form-label">Stock</label>
                        <input class="form-control" type="number" name="stock" min="0" step="1" required value="<?= esc(old('stock', $product['stock'] ?? '0')) ?>">
                    </div>
                    <div class="mb-4">
                        <label class="form-label">Status</label>
                        <?php $selectedStatus = old('status', $product['status'] ?? 'active'); ?>
                        <select class="form-select" name="status" required>
                            <option value="active" <?= $selectedStatus === 'active' ? 'selected' : '' ?>>Active</option>
                            <option value="inactive" <?= $selectedStatus === 'inactive' ? 'selected' : '' ?>>Inactive</option>
                        </select>
                    </div>

                    <div class="mb-4">
                        <label class="form-label">Product Image (JPG, PNG, WEBP; max 2 MB)</label>
                        <input class="form-control" type="file" name="image" accept=".jpg,.jpeg,.png,.webp">
                        <?php if (!empty($product['image'])): ?>
                            <div class="mt-2"><img src="<?= base_url('uploads/products/' . $product['image']) ?>" alt="" width="72" class="rounded"></div>
                        <?php endif; ?>
                    </div>

                    <div class="d-flex gap-2">
                        <button class="btn btn-success" type="submit">Save</button>
                        <a class="btn btn-outline-secondary" href="<?= site_url('products') ?>">Cancel</a>
                    </div>
                </form>
            </div>
        </div>
    </div>
</div>

<?= $this->endSection() ?>
