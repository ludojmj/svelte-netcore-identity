// const.js
export const configuration = {
  client_id: "interactive.public.short",
  redirect_uri: window.location.origin + "/authentication/callback",
  silent_redirect_uri: window.location.origin + "/authentication/silent-callback",
  scope: "openid profile email api offline_access",
  authority: "https://demo.duendesoftware.com",
  // service_worker_relative_url: "/OidcServiceWorker.js",
  service_worker_only: false,
  monitor_session: false
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
