# ShopEase — Level 1 (Basic)

Minimal CodeIgniter 4 + MySQL implementation for the class assignment.

## Included
- Database connection template
- Categories CRUD
- Products CRUD
- Product image upload
- Basic server-side validation
- Simple green Bootstrap 5 layout
- MVC structure (Controllers, Models, Views)

This is an application overlay, not a copy of the CodeIgniter framework. Create a fresh CI4 project first, then copy the included `app` and `public` folders into it.

## 1. Requirements
- PHP 8.1+ (or a version supported by your installed CodeIgniter 4 release)
- Composer
- MySQL
- XAMPP or Laragon

## 2. Create the CodeIgniter project

Open a terminal in the folder where you keep projects:

```bash
composer create-project codeigniter4/appstarter shopease
```

Copy the `app` and `public` folders from this package into the new `shopease` folder. Allow the files to merge/overwrite when prompted.

Copy `.env.example` to `.env` in the project root.

## 3. Create the database

In phpMyAdmin or MySQL, import:

`database/db_shopease.sql`

The script creates the `db_shopease` database and the `categories` and `products` tables.

## 4. Configure `.env`

Update the database values to match your local MySQL setup:

```ini
CI_ENVIRONMENT = development
database.default.hostname = localhost
database.default.database = db_shopease
database.default.username = root
database.default.password =
database.default.DBDriver = MySQLi
database.default.port = 3306
```

If your MySQL username, password, or port is different, change those values.

## 5. Run

From the `shopease` project folder:

```bash
php spark serve
```

Open the local address shown in the terminal (usually `http://localhost:8080`).

## Pages
- `/categories` — add, view, edit, and delete categories
- `/products` — add, view, edit, and delete products

## Notes
- Product images are stored in `public/uploads/products`.
- Allowed image formats: JPG, JPEG, PNG, WEBP; maximum size: 2 MB.
- A category cannot be deleted while products still use it.
- Product status is either `active` or `inactive`.
- This Level 1 package does not yet include login, cart, checkout, payments, or shipping. Those belong to later project levels.
