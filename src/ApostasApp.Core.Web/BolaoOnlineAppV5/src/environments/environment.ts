    // Localização: src/environments/environment.ts
    /*
    export const environment = {
      production: false,
      // <<-- ATENÇÃO: SEMPRE SEM '/api' E SEM A BARRA FINAL AQUI -->>
      apiUrl: 'http://localhost:5288',
      // Certifique-se de que esta URL base corresponde ao seu backend
      // Por exemplo: 'https://seubackend.com' ou 'http://localhost:5000'
      imagesUrl: '/assets/images/'
    }; 
     */
    
    export const environment = {
  production: false,
  // Altere a URL da API para o seu App Service no Azure
  apiUrl: 'http://bolaoonline-testes4.azurewebsites.net', 
  imagesUrl: 'http://bolaoonline-testes4.azurewebsites.net' 
};