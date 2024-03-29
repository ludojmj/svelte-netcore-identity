// const.js
export const configuration = {
  client_id: "interactive.public.short",
  redirect_uri: window.location.origin + "/authentication/callback",
  silent_redirect_uri: window.location.origin + "/authentication/silent-callback",
  scope: "openid profile email api offline_access",
  authority: "https://demo.duendesoftware.com",
  refresh_time_before_tokens_expiration_in_second: 40,
  service_worker_relative_url: '/OidcServiceWorker.js',
  service_worker_only: false,
  // silent_login_timeout: 1000,
  // monitor_session: true,
  token_renew_mode: 'access_token_invalid'
};

export const apiErrMsg = {
  generic: "An error occured. Try again later.",
  unauthorized: "Please login again."
}

export const crud = {
  CREATE: "Create",
  READ: "Read",
  UPDATE: "Update",
  DELETE: "Delete",
};
