<!doctype html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title><?= esc($pageTitle ?? 'ShopEase') ?> | ShopEase</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet">
    <style>
        :root {
            --shop-green: #07b564;
            --bs-success: #07b564;
            --bs-success-rgb: 7, 181, 100;
        }

        body { background: #f6f8f6; }
        .navbar { background: var(--shop-green); padding: .85rem 0; }
        .btn-success {
            --bs-btn-bg: #07b564;
            --bs-btn-border-color: #07b564;
            --bs-btn-color: #fff;
            --bs-btn-hover-bg: #018a5c;
            --bs-btn-hover-border-color: #018a5c;
            --bs-btn-active-bg: #018a5c;
            --bs-btn-active-border-color: #018a5c;
        }
        .btn-outline-success {
            --bs-btn-color: #018a5c;
            --bs-btn-border-color: #018a5c;
            --bs-btn-hover-bg: #018a5c;
            --bs-btn-hover-border-color: #018a5c;
            --bs-btn-active-bg: #018a5c;
            --bs-btn-active-border-color: #018a5c;
        }
        .brand-dot { color: #eafff4; }
        .card { border: 1px solid #e5ebe6; }
        main.container { padding-top: 2rem; padding-bottom: 2rem; }
        .card-body { padding: 1.5rem; }
        .table th, .table td { padding: 1rem .9rem; }
        .form-control, .form-select { padding: .7rem .85rem; }
        .form-label { margin-bottom: .55rem; }
        .role-choice { min-height: 56vh; display: flex; align-items: center; justify-content: center; }
        .role-choice-card { width: 100%; max-width: 760px; padding: 2.5rem; }
        .shop-card { height: 100%; }
        .shop-image { height: 220px; object-fit: cover; background: #edf3ef; }
        .product-image { min-height: 360px; background: #edf3ef; }
    </style>
</head>
<body>
<nav class="navbar navbar-expand-lg navbar-dark">
    <div class="container">
        <a class="navbar-brand fw-semibold" href="<?= site_url('/') ?>">ShopEase<span class="brand-dot">.</span></a>
        <div class="navbar-nav ms-auto flex-row gap-3">
            <?php if (session()->get('role') === 'seller'): ?>
                <a class="nav-link text-white" href="<?= site_url('categories') ?>">Categories</a>
                <a class="nav-link text-white" href="<?= site_url('products') ?>">Products</a>
            <?php elseif (session()->get('role') === 'buyer'): ?>
                <a class="nav-link text-white" href="<?= site_url('shop') ?>">Shop</a>
            <?php endif; ?>
            <?php if (session()->get('role')): ?><a class="nav-link text-white" href="<?= site_url('switch-role') ?>">Switch role</a><?php endif; ?>
        </div>
    </div>
</nav>

<main class="container py-4">
    <?php if (session()->getFlashdata('success')): ?>
        <div class="alert alert-success"><?= esc(session()->getFlashdata('success')) ?></div>
    <?php endif; ?>
    <?php if (session()->getFlashdata('error')): ?>
        <div class="alert alert-danger"><?= esc(session()->getFlashdata('error')) ?></div>
    <?php endif; ?>

    <?= $this->renderSection('content') ?>
</main>
</body>
</html>
