<?php

use CodeIgniter\Router\RouteCollection;

/** @var RouteCollection $routes */
$routes->get('/', 'Home::index');
$routes->get('/crud', 'Crud::index');
$routes->get('/crud/tambah', 'Crud::tambah');
$routes->post('/crud/tambah_aksi', 'Crud::tambah_aksi');
$routes->get('/crud/edit/(:num)', 'Crud::edit/$1');
$routes->post('/crud/update', 'Crud::update');
$routes->get('/crud/hapus/(:num)', 'Crud::hapus/$1');