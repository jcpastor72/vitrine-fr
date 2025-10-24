const express = require('express');
const router = express.Router();
const { Pool } = require('pg');

// Configuration PostgreSQL
const pool = new Pool({
  host: process.env.DB_HOST || 'laborcontrol-db.postgres.database.azure.com',
  port: process.env.DB_PORT || 5432,
  database: process.env.DB_NAME || 'laborcontrol',
  user: process.env.DB_USER || 'laboradmin',
  password: process.env.DB_PASSWORD || 'Loulou@2025!',
  ssl: {
    rejectUnauthorized: false
  }
});

console.log(`🔌 Connexion PostgreSQL: ${pool.options.host}:${pool.options.port}/${pool.options.database}`);

// Test de connexion
pool.on('error', (err) => {
  console.error('❌ Erreur pool PostgreSQL:', err);
});

// GET /api/rfidchips - Récupérer toutes les puces
router.get('/', async (req, res) => {
  try {
    console.log('📡 Requête GET /api/rfidchips');

    // Récupérer toutes les puces depuis la table RfidChips
    const result = await pool.query('SELECT * FROM "RfidChips"');
    console.log(`✅ ${result.rows.length} puces récupérées`);
    res.json(result.rows);
  } catch (error) {
    console.error('❌ Erreur GET /api/rfidchips:', error.message);
    res.status(500).json({ error: error.message });
  }
});

// GET /api/rfidchips/:id - Récupérer une puce
router.get('/:id', async (req, res) => {
  try {
    const result = await pool.query(
      'SELECT * FROM rfid_chips WHERE id = $1',
      [req.params.id]
    );

    if (result.rows.length === 0) {
      return res.status(404).json({ error: 'Puce non trouvée' });
    }

    res.json(result.rows[0]);
  } catch (error) {
    console.error('❌ Erreur GET /api/rfidchips/:id:', error.message);
    res.status(500).json({ error: error.message });
  }
});

// POST /api/rfidchips/manual - Ajouter une puce manuellement
router.post('/manual', async (req, res) => {
  try {
    const { uid } = req.body;

    if (!uid) {
      return res.status(400).json({ error: 'UID requis' });
    }

    // Vérifier si la puce existe
    const existing = await pool.query(
      'SELECT id FROM rfid_chips WHERE uid = $1',
      [uid]
    );

    if (existing.rows.length > 0) {
      return res.status(409).json({ error: 'Cette puce existe déjà' });
    }

    // Insérer la nouvelle puce
    const result = await pool.query(
      `INSERT INTO rfid_chips (uid, status, created_at)
       VALUES ($1, $2, NOW())
       RETURNING *`,
      [uid, 'IN_WORKSHOP']
    );

    res.status(201).json(result.rows[0]);
  } catch (error) {
    console.error('❌ Erreur POST /api/rfidchips/manual:', error.message);
    res.status(500).json({ error: error.message });
  }
});

// PUT /api/rfidchips/:id/status - Mettre à jour le statut
router.put('/:id/status', async (req, res) => {
  try {
    const { status } = req.body;

    const result = await pool.query(
      'UPDATE rfid_chips SET status = $1 WHERE id = $2 RETURNING *',
      [status, req.params.id]
    );

    if (result.rows.length === 0) {
      return res.status(404).json({ error: 'Puce non trouvée' });
    }

    res.json(result.rows[0]);
  } catch (error) {
    console.error('❌ Erreur PUT /api/rfidchips/:id/status:', error.message);
    res.status(500).json({ error: error.message });
  }
});

module.exports = router;
