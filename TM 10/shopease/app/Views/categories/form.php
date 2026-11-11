<?= $this->extend('layouts/main') ?>
<?= $this->section('content') ?>

<div class="row justify-content-center">
    <div class="col-lg-7">
        <div class="mb-3">
            <h1 class="h3 mb-1"><?= esc($pageTitle) ?></h1>
            <p class="text-secondary mb-0">Enter the category information.</p>
        </div>

        <?php $errors = session()->getFlashdata('errors') ?? []; ?>
        <?php if ($errors): ?>
            <div class="alert alert-danger">
                <?php foreach ($errors as $error): ?><div><?= esc($error) ?></div><?php endforeach; ?>
            </div>
        <?php endif; ?>

        <div class="card shadow-sm">
            <div class="card-body p-4">
                <form method="post" action="<?= esc($formAction) ?>">
                    <?= csrf_field() ?>
                    <div class="mb-4">
                        <label class="form-label">Category Name</label>
                        <input class="form-control" type="text" name="name" maxlength="100" required value="<?= esc(old('name', $category['name'] ?? '')) ?>">
                    </div>
                    <div class="mb-4">
                        <label class="form-label">Description</label>
                        <textarea class="form-control" name="description" rows="3"><?= esc(old('description', $category['description'] ?? '')) ?></textarea>
                    </div>
                    <div class="d-flex gap-2">
                        <button class="btn btn-success" type="submit">Save</button>
                        <a class="btn btn-outline-secondary" href="<?= site_url('categories') ?>">Cancel</a>
                    </div>
                </form>
            </div>
        </div>
    </div>
</div>

<?= $this->endSection() ?>
