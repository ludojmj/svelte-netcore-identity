// oidc.js
import { VanillaOidc } from "@axa-fr/vanilla-oidc";
import { isAuthLoading, tokens } from "./store.js";
import { configuration } from "./const.js";

const href = window.location.href;
const vanillaOidc = VanillaOidc.getOrCreate(configuration);

export let getTokenAync = async () => {
  isAuthLoading.set(true);
  vanillaOidc.tryKeepExistingSessionAsync().then(() => {
    if (href.includes(configuration.redirect_uri)) {
      vanillaOidc.loginCallbackAsync().then(() => {
        window.location.href = "/";
      });

      isAuthLoading.set(true);
      return;
    }

    tokens.set(vanillaOidc.tokens);
    isAuthLoading.set(false);
  });
};

export const loginAsync = async () => {
  await vanillaOidc.loginAsync("/");
};

export const logoutAsync = async () => {
  await vanillaOidc.logoutAsync();
};
