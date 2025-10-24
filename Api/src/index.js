const express = require('express');
const cors = require('cors');
const rfidChipsRouter = require('./routes/rfidchips-pg');

const app = express();
const PORT = process.env.PORT || 7071;

// Middleware
app.use(cors());
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

// Routes
app.use('/api/rfidchips', rfidChipsRouter);

// Health check
app.get('/api/health', (req, res) => {
  res.json({ status: 'ok' });
});

// 404 handler
app.use((req, res) => {
  res.status(404).json({ error: 'Route not found' });
});

// Error handler
app.use((err, req, res, next) => {
  console.error('Erreur serveur:', err);
  res.status(500).json({ error: err.message });
});

app.listen(PORT, () => {
  console.log(`API serveur démarré sur le port ${PORT}`);
});

module.exports = app;
