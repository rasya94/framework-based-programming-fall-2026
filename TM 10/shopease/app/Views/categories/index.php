<?= $this->extend('layouts/main') ?>
<?= $this->section('content') ?>

<div class="d-flex justify-content-between align-items-center mb-3">
    <div>
        <h1 class="h3 mb-1">Categories</h1>
        <p class="text-secondary mb-0">Manage product categories.</p>
    </div>
    <a href="<?= site_url('categories/new') ?>" class="btn btn-success">Add Category</a>
</div>

<div class="card shadow-sm">
    <div class="table-responsive">
        <table class="table table-hover align-middle mb-0">
            <thead class="table-light">
                <tr><th>Name</th><th>Description</th><th class="text-end">Actions</th></tr>
            </thead>
            <tbody>
            <?php if (empty($categories)): ?>
                <tr><td colspan="3" class="text-center text-secondary py-4">No categories yet.</td></tr>
            <?php else: ?>
                <?php foreach ($categories as $category): ?>
                    <tr>
                        <td class="fw-medium"><?= esc($category['name']) ?></td>
                        <td><?= esc($category['description'] ?? '') ?></td>
                        <td class="text-end text-nowrap">
                            <a class="btn btn-sm btn-outline-secondary" href="<?= site_url('categories/' . $category['id'] . '/edit') ?>">Edit</a>
                            <form class="d-inline" method="post" action="<?= site_url('categories/' . $category['id'] . '/delete') ?>" onsubmit="return confirm('Delete this category?')">
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
