<?php

namespace App\Controllers;

class Home extends BaseController
{
    public function index()
    {
        if (session()->get('role') === 'seller') {
            return redirect()->to('/products');
        }

        if (session()->get('role') === 'buyer') {
            return redirect()->to('/shop');
        }

        return view('home');
    }

    public function enter($role)
    {
        if ($role !== 'seller' && $role !== 'buyer') {
            return redirect()->to('/');
        }

        session()->set('role', $role);

        if ($role === 'seller') {
            return redirect()->to('/products');
        }

        return redirect()->to('/shop');
    }

    public function switchRole()
    {
        session()->remove('role');
        return redirect()->to('/');
    }
}
