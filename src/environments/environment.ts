    // Localização: src/environments/environment.ts
    export const environment = {
      production: false,
      // <<-- ATENÇÃO: SEMPRE SEM '/api' E SEM A BARRA FINAL AQUI -->>
      apiUrl: 'http://localhost:5288',
      // Certifique-se de que esta URL base corresponde ao seu backend
      // Por exemplo: 'https://seubackend.com' ou 'http://localhost:5000'
      imagesUrl: 'http://localhost:5288' // URL base para imagens (sem /api)
    }; 
     