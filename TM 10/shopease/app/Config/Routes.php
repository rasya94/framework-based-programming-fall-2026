<?php

namespace Config;

use CodeIgniter\Config\BaseConfig;
use CodeIgniter\Router\RouteCollection;

/** @var RouteCollection $routes */
$routes = Services::routes();

if (file_exists(SYSTEMPATH . 'Config/Routes.php')) {
    require SYSTEMPATH . 'Config/Routes.php';
}

$routes->setDefaultNamespace('App\Controllers');
$routes->setDefaultController('Home');
$routes->setDefaultMethod('index');
$routes->setTranslateURIDashes(false);
$routes->set404Override();
$routes->setAutoRoute(false);

$routes->get('/', 'Home::index');
$routes->get('switch-role', 'Home::switchRole');
$routes->get('enter/(:segment)', 'Home::enter/$1');
$routes->get('shop', 'ProductController::shop');
$routes->get('shop/(:num)', 'ProductController::show/$1');

$routes->get('categories', 'CategoryController::index');
$routes->get('categories/new', 'CategoryController::create');
$routes->post('categories', 'CategoryController::store');
$routes->get('categories/(:num)/edit', 'CategoryController::edit/$1');
$routes->post('categories/(:num)/update', 'CategoryController::update/$1');
$routes->post('categories/(:num)/delete', 'CategoryController::delete/$1');

$routes->get('products', 'ProductController::index');
$routes->get('products/new', 'ProductController::create');
$routes->post('products', 'ProductController::store');
$routes->get('products/(:num)/edit', 'ProductController::edit/$1');
$routes->post('products/(:num)/update', 'ProductController::update/$1');
$routes->post('products/(:num)/delete', 'ProductController::delete/$1');
