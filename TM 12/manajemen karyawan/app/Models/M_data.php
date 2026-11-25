<?php

namespace App\Models;

use CodeIgniter\Model;

class M_data extends Model
{
    protected $table = 'user';
    protected $primaryKey = 'id';

    protected $allowedFields = ['nama', 'alamat', 'pekerjaan'];
}