// Configuration de la base de données
// À adapter selon votre infrastructure Azure

const config = {
  // Azure SQL Database
  sql: {
    server: process.env.DB_SERVER || 'localhost',
    database: process.env.DB_NAME || 'laborcontrol',
    authentication: {
      type: 'default',
      options: {
        userName: process.env.DB_USER || 'sa',
        password: process.env.DB_PASSWORD || 'password'
      }
    },
    options: {
      encrypt: true,
      trustServerCertificate: false,
      connectTimeout: 15000,
      requestTimeout: 30000,
      connectionTimeout: 15000
    }
  },

  // Cosmos DB
  cosmos: {
    endpoint: process.env.COSMOS_ENDPOINT || '',
    key: process.env.COSMOS_KEY || '',
    databaseId: process.env.COSMOS_DB || 'laborcontrol',
    containerId: 'rfidchips'
  },

  // MongoDB
  mongodb: {
    uri: process.env.MONGODB_URI || 'mongodb://localhost:27017/laborcontrol'
  }
};

module.exports = config;
