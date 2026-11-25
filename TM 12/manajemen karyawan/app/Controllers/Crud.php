<?php

namespace App\Controllers;

use App\Models\M_data;

class Crud extends BaseController
{
    public function index()
    {
        $model = new M_data();
        $data['user'] = $model->findAll();

        return view('v_tampil', $data);
    }

    public function tambah()
    {
        return view('v_input');
    }

    public function tambah_aksi()
    {
        $model = new M_data();

        $data = [
            'nama' => $this->request->getPost('nama'),
            'alamat' => $this->request->getPost('alamat'),
            'pekerjaan' => $this->request->getPost('pekerjaan')
        ];

        $model->insert($data);

        return redirect()->to(base_url('crud'));
    }

    public function edit($id)
    {
        $model = new M_data();
        $data['user'] = $model->find($id);

        if (!$data['user']) {
            throw \CodeIgniter\Exceptions\PageNotFoundException::forPageNotFound();
        }

        return view('v_edit', $data);
    }

    public function update()
    {
        $model = new M_data();

        $id = $this->request->getPost('id');

        $data = [
            'nama' => $this->request->getPost('nama'),
            'alamat' => $this->request->getPost('alamat'),
            'pekerjaan' => $this->request->getPost('pekerjaan')
        ];

        $model->update($id, $data);

        return redirect()->to(base_url('crud'));
    }

    public function hapus($id)
    {
        $model = new M_data();
        $model->delete($id);

        return redirect()->to(base_url('crud'));
    }
}