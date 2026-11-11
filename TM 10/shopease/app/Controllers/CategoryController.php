<?php

namespace App\Controllers;

use App\Models\CategoryModel;
use App\Models\ProductModel;

class CategoryController extends BaseController
{
    public function index()
    {
        $model = new CategoryModel();

        return view('categories/index', [
            'categories' => $model->orderBy('name', 'ASC')->findAll(),
        ]);
    }

    public function create()
    {
        return view('categories/form', [
            'category' => null,
            'formAction' => site_url('categories'),
            'pageTitle' => 'Add Category',
        ]);
    }

    public function store()
    {
        if (!$this->validate([
            'name' => 'required|min_length[2]|max_length[100]',
            'description' => 'permit_empty|max_length[2000]',
        ])) {
            return redirect()->back()->withInput()->with('errors', $this->validator->getErrors());
        }

        (new CategoryModel())->insert([
            'name' => trim($this->request->getPost('name')),
            'description' => trim((string) $this->request->getPost('description')),
        ]);

        return redirect()->to('/categories')->with('success', 'Category added.');
    }

    public function edit($id)
    {
        $category = (new CategoryModel())->find($id);

        if (!$category) {
            return redirect()->to('/categories')->with('error', 'Category not found.');
        }

        return view('categories/form', [
            'category' => $category,
            'formAction' => site_url('categories/' . $id . '/update'),
            'pageTitle' => 'Edit Category',
        ]);
    }

    public function update($id)
    {
        $model = new CategoryModel();

        if (!$model->find($id)) {
            return redirect()->to('/categories')->with('error', 'Category not found.');
        }

        if (!$this->validate([
            'name' => 'required|min_length[2]|max_length[100]',
            'description' => 'permit_empty|max_length[2000]',
        ])) {
            return redirect()->back()->withInput()->with('errors', $this->validator->getErrors());
        }

        $model->update($id, [
            'name' => trim($this->request->getPost('name')),
            'description' => trim((string) $this->request->getPost('description')),
        ]);

        return redirect()->to('/categories')->with('success', 'Category updated.');
    }

    public function delete($id)
    {
        $products = new ProductModel();

        if ($products->where('category_id', $id)->countAllResults() > 0) {
            return redirect()->to('/categories')->with('error', 'Cannot delete a category that still has products.');
        }

        (new CategoryModel())->delete($id);

        return redirect()->to('/categories')->with('success', 'Category deleted.');
    }
}
