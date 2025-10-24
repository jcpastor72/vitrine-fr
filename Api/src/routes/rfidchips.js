const express = require('express');
const router = express.Router();
const axios = require('axios');

// Configuration de l'API backend
const BACKEND_API_URL = 'https://laborcontrol-api.azurewebsites.net';

// Cache des puces
let rfidChips = [];
let lastFetchTime = 0;
const CACHE_DURATION = 5 * 60 * 1000; // 5 minutes

// Fonction pour récupérer les puces depuis l'API backend
async function fetchChipsFromBackend(token) {
  try {
    const response = await axios.get(`${BACKEND_API_URL}/api/RfidChips`, {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      timeout: 10000
    });

    rfidChips = response.data || [];
    lastFetchTime = Date.now();
    console.log(`✅ Récupéré ${rfidChips.length} puces depuis l'API backend`);
    return rfidChips;
  } catch (error) {
    console.error('❌ Erreur lors de la récupération des puces:', error.message);
    return rfidChips; // Retourner le cache en cas d'erreur
  }
}

// Middleware pour extraire le token du header
function extractToken(req) {
  const authHeader = req.headers.authorization;
  if (authHeader && authHeader.startsWith('Bearer ')) {
    return authHeader.substring(7);
  }
  return null;
}

// GET /api/rfidchips - Récupérer toutes les puces
router.get('/', async (req, res) => {
  try {
    const token = extractToken(req);

    if (!token) {
      // Si pas de token, retourner le cache
      return res.json(rfidChips);
    }

    // Vérifier si le cache est encore valide
    if (rfidChips.length > 0 && (Date.now() - lastFetchTime) < CACHE_DURATION) {
      console.log(`📦 Retour du cache (${rfidChips.length} puces)`);
      return res.json(rfidChips);
    }

    // Récupérer les puces depuis l'API backend
    const chips = await fetchChipsFromBackend(token);
    res.json(chips);
  } catch (error) {
    console.error('Erreur GET /api/rfidchips:', error);
    res.status(500).json({ error: error.message });
  }
});

// GET /api/rfidchips/:id - Récupérer une puce
router.get('/:id', (req, res) => {
  try {
    const chip = rfidChips.find(c => c.id === req.params.id);
    if (!chip) {
      return res.status(404).json({ error: 'Puce non trouvée' });
    }
    res.json(chip);
  } catch (error) {
    console.error('Erreur GET /api/rfidchips/:id:', error);
    res.status(500).json({ error: error.message });
  }
});

// POST /api/rfidchips/manual - Ajouter une puce manuellement
router.post('/manual', (req, res) => {
  try {
    const { uid } = req.body;

    if (!uid) {
      return res.status(400).json({ error: 'UID requis' });
    }

    // Vérifier si la puce existe déjà
    const existing = rfidChips.find(c => c.uid === uid);
    if (existing) {
      return res.status(409).json({ error: 'Cette puce existe déjà' });
    }

    const newChip = {
      id: require('crypto').randomUUID(),
      uid: uid,
      chipId: `LC-${Math.random().toString(36).substring(2, 8).toUpperCase()}`,
      status: 'IN_WORKSHOP',
      description: 'Puce ajoutée manuellement',
      activationDate: new Date(),
      createdAt: new Date(),
      packagingCode: null,
      customerId: null,
      customerName: '',
      encryptionKey: null,
      keyHash: null,
      keyCreatedAt: null,
      keyCreatedBy: null,
      isKeyProgrammed: false
    };

    rfidChips.push(newChip);
    res.status(201).json(newChip);
  } catch (error) {
    console.error('Erreur POST /api/rfidchips/manual:', error);
    res.status(500).json({ error: error.message });
  }
});

// PUT /api/rfidchips/:id/status - Mettre à jour le statut
router.put('/:id/status', (req, res) => {
  try {
    const { status } = req.body;
    const chip = rfidChips.find(c => c.id === req.params.id);

    if (!chip) {
      return res.status(404).json({ error: 'Puce non trouvée' });
    }

    chip.status = status;
    res.json(chip);
  } catch (error) {
    console.error('Erreur PUT /api/rfidchips/:id/status:', error);
    res.status(500).json({ error: error.message });
  }
});

// POST /api/rfidchips/:id/key - Créer la clef
router.post('/:id/key', (req, res) => {
  try {
    const { chipId, encryptionKey, keyHash } = req.body;
    const chip = rfidChips.find(c => c.id === req.params.id);

    if (!chip) {
      return res.status(404).json({ error: 'Puce non trouvée' });
    }

    chip.chipId = chipId;
    chip.encryptionKey = encryptionKey;
    chip.keyHash = keyHash;
    chip.keyCreatedAt = new Date();
    chip.keyCreatedBy = 'admin'; // À remplacer par l'utilisateur authentifié
    chip.isKeyProgrammed = true;
    chip.status = 'IN_STOCK';

    res.json(chip);
  } catch (error) {
    console.error('Erreur POST /api/rfidchips/:id/key:', error);
    res.status(500).json({ error: error.message });
  }
});

// POST /api/rfidchips/import-excel - Importer depuis Excel
router.post('/import-excel', (req, res) => {
  try {
    const { uids, orderId } = req.body;

    if (!uids || !Array.isArray(uids)) {
      return res.status(400).json({ error: 'UIDs requis' });
    }

    const results = {
      successCount: 0,
      duplicateCount: 0,
      errorCount: 0,
      errors: []
    };

    uids.forEach(uid => {
      try {
        // Vérifier si la puce existe
        const existing = rfidChips.find(c => c.uid === uid);
        if (existing) {
          results.duplicateCount++;
          return;
        }

        const newChip = {
          id: require('crypto').randomUUID(),
          uid: uid,
          chipId: `LC-${Math.random().toString(36).substring(2, 8).toUpperCase()}`,
          status: 'IN_TRANSIT_SUPPLIER',
          description: 'Importée depuis Excel',
          activationDate: new Date(),
          createdAt: new Date(),
          packagingCode: null,
          customerId: null,
          customerName: '',
          supplierOrderId: orderId,
          encryptionKey: null,
          keyHash: null,
          keyCreatedAt: null,
          keyCreatedBy: null,
          isKeyProgrammed: false
        };

        rfidChips.push(newChip);
        results.successCount++;
      } catch (error) {
        results.errorCount++;
        results.errors.push(`Erreur pour UID ${uid}: ${error.message}`);
      }
    });

    res.json(results);
  } catch (error) {
    console.error('Erreur POST /api/rfidchips/import-excel:', error);
    res.status(500).json({ error: error.message });
  }
});

module.exports = router;
