export const configuration = {
  client_id: "interactive.public",
  redirect_uri: window.location.origin + "/authentication/callback",
  silent_redirect_uri:
    window.location.origin + "/authentication/silent-callback",
  scope: "openid profile email api offline_access",
  authority: "https://demo.duendesoftware.com",
  service_worker_relative_url: "/OidcServiceWorker.js",
  service_worker_only: true,
  monitor_session: true
};
