// Add bellow trusted domains, access tokens will automatically injected to be send to
// trusted domain can also be a path like https://www.myapi.com/users,
// then all subroute like https://www.myapi.com/useers/1 will be authorized to send access_token to.

// Domains used by OIDC server must be also declared here
// eslint-disable-next-line @typescript-eslint/no-unused-vars
const trustedDomains = {
  default: [
    'http://127.0.0.1:5173',
    'http://localhost:5173',
    'https://localhost:5001',
    'https://rita.azurewebsites.net',
    'https://ludal.azurewebsites.net',
    'https://demo.duendesoftware.com',
    'https://kdhttps.auth0.com'
  ],
  config_classic: [
    'http://127.0.0.1:5173',
    'http://localhost:5173',
    'https://localhost:5001',
    'https://rita.azurewebsites.net',
    'https://ludal.azurewebsites.net',
    'https://demo.duendesoftware.com'
  ],
  config_without_silent_login: [
    'http://127.0.0.1:5173',
    'http://localhost:5173',
    'https://localhost:5001',
    'https://rita.azurewebsites.net',
    'https://ludal.azurewebsites.net',
    'https://demo.duendesoftware.com'
  ],
  config_without_refresh_token: [
    'http://127.0.0.1:5173',
    'http://localhost:5173',
    'https://localhost:5001',
    'https://rita.azurewebsites.net',
    'https://ludal.azurewebsites.net',
    'https://demo.duendesoftware.com'
  ],
  config_without_refresh_token_silent_login: [
    'http://127.0.0.1:5173',
    'http://localhost:5173',
    'https://localhost:5001',
    'https://rita.azurewebsites.net',
    'https://ludal.azurewebsites.net',
    'https://demo.duendesoftware.com'
  ],
  config_google: [
    'http://127.0.0.1:5173',
    'http://localhost:5173',
    'https://localhost:5001',
    'https://rita.azurewebsites.net',
    'https://ludal.azurewebsites.net',
    'https://oauth2.googleapis.com',
    'https://openidconnect.googleapis.com'
  ],
  config_with_hash: [
    'http://127.0.0.1:5173',
    'http://localhost:5173',
    'https://localhost:5001',
    'https://rita.azurewebsites.net',
    'https://ludal.azurewebsites.net',
    'https://demo.duendesoftware.com'],
};
