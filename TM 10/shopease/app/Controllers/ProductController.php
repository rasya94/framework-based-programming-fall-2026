<?php

namespace App\Controllers;

use App\Models\CategoryModel;
use App\Models\ProductModel;

class ProductController extends BaseController
{
    public function shop()
    {
        $model = new ProductModel();
        $products = $model
            ->select('products.*, categories.name AS category_name')
            ->join('categories', 'categories.id = products.category_id')
            ->where('products.status', 'active')
            ->where('products.stock >', 0)
            ->orderBy('products.id', 'DESC')
            ->findAll();

        return view('products/shop', ['products' => $products]);
    }

    public function show($id)
    {
        $model = new ProductModel();
        $product = $model
            ->select('products.*, categories.name AS category_name')
            ->join('categories', 'categories.id = products.category_id')
            ->where('products.status', 'active')
            ->where('products.id', $id)
            ->first();

        if (empty($product)) {
            return redirect()->to('/shop')->with('error', 'Product not found.');
        }

        return view('products/show', ['product' => $product]);
    }

    public function index()
    {
        $products = (new ProductModel())
            ->select('products.*, categories.name AS category_name')
            ->join('categories', 'categories.id = products.category_id')
            ->orderBy('products.id', 'DESC')
            ->findAll();

        return view('products/index', ['products' => $products]);
    }

    public function create()
    {
        $categories = (new CategoryModel())->orderBy('name', 'ASC')->findAll();

        if (empty($categories)) {
            return redirect()->to('/categories')->with('error', 'Add a category before adding a product.');
        }

        return view('products/form', [
            'product' => null,
            'categories' => $categories,
            'formAction' => site_url('products'),
            'pageTitle' => 'Add Product',
        ]);
    }

    public function store()
    {
        if (!$this->validate($this->productRules())) {
            return redirect()->back()->withInput()->with('errors', $this->validator->getErrors());
        }

        $imageName = $this->saveImage();

        (new ProductModel())->insert([
            'category_id' => $this->request->getPost('category_id'),
            'name' => trim($this->request->getPost('name')),
            'slug' => url_title(trim($this->request->getPost('name')), '-', true) . '-' . bin2hex(random_bytes(3)),
            'description' => trim((string) $this->request->getPost('description')),
            'price' => $this->request->getPost('price'),
            'stock' => $this->request->getPost('stock'),
            'image' => $imageName,
            'status' => $this->request->getPost('status'),
        ]);

        return redirect()->to('/products')->with('success', 'Product added.');
    }

    public function edit($id)
    {
        $product = (new ProductModel())->find($id);

        if (!$product) {
            return redirect()->to('/products')->with('error', 'Product not found.');
        }

        return view('products/form', [
            'product' => $product,
            'categories' => (new CategoryModel())->orderBy('name', 'ASC')->findAll(),
            'formAction' => site_url('products/' . $id . '/update'),
            'pageTitle' => 'Edit Product',
        ]);
    }

    public function update($id)
    {
        $model = new ProductModel();
        $product = $model->find($id);

        if (!$product) {
            return redirect()->to('/products')->with('error', 'Product not found.');
        }

        if (!$this->validate($this->productRules())) {
            return redirect()->back()->withInput()->with('errors', $this->validator->getErrors());
        }

        $imageName = $product['image'];
        $newImage = $this->saveImage();

        if ($newImage !== null) {
            $this->removeImage($imageName);
            $imageName = $newImage;
        }

        $model->update($id, [
            'category_id' => $this->request->getPost('category_id'),
            'name' => trim($this->request->getPost('name')),
            'slug' => url_title(trim($this->request->getPost('name')), '-', true) . '-' . bin2hex(random_bytes(3)),
            'description' => trim((string) $this->request->getPost('description')),
            'price' => $this->request->getPost('price'),
            'stock' => $this->request->getPost('stock'),
            'image' => $imageName,
            'status' => $this->request->getPost('status'),
        ]);

        return redirect()->to('/products')->with('success', 'Product updated.');
    }

    public function delete($id)
    {
        $model = new ProductModel();
        $product = $model->find($id);

        if ($product) {
            $this->removeImage($product['image']);
            $model->delete($id);
        }

        return redirect()->to('/products')->with('success', 'Product deleted.');
    }

    private function productRules(): array
    {
        return [
            'category_id' => 'required|is_natural_no_zero',
            'name' => 'required|min_length[2]|max_length[150]',
            'description' => 'permit_empty|max_length[5000]',
            'price' => 'required|decimal|greater_than_equal_to[0]',
            'stock' => 'required|integer|greater_than_equal_to[0]',
            'status' => 'required|in_list[active,inactive]',
            'image' => 'if_exist|is_image[image]|mime_in[image,image/jpg,image/jpeg,image/png,image/webp]|max_size[image,2048]',
        ];
    }

    private function saveImage(): ?string
    {
        $image = $this->request->getFile('image');

        if (!$image || !$image->isValid() || $image->hasMoved()) {
            return null;
        }

        $uploadPath = FCPATH . 'uploads/products';

        if (!is_dir($uploadPath)) {
            mkdir($uploadPath, 0775, true);
        }

        $imageName = $image->getRandomName();
        $image->move($uploadPath, $imageName);

        return $imageName;
    }

    private function removeImage(?string $imageName): void
    {
        if (!$imageName) {
            return;
        }

        $path = FCPATH . 'uploads/products/' . $imageName;

        if (is_file($path)) {
            unlink($path);
        }
    }
}
