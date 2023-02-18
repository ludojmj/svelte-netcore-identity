// oidc.js
import { VanillaOidc } from "@axa-fr/vanilla-oidc";
import { isAuthLoading, tokens, userInfo } from "./store.js";
import { configuration } from "./const.js";

const href = window.location.href;
const vanillaOidc = VanillaOidc.getOrCreate(configuration);

export let getToken = () => {
  let result;
  isAuthLoading.set(true);
  vanillaOidc.tryKeepExistingSessionAsync().then(() => {
    if (href.includes(configuration.redirect_uri)) {
      vanillaOidc.loginCallbackAsync().then(() => {
        window.location.href = "/";
      });

      isAuthLoading.set(true);
      return;
    }

    result = vanillaOidc.tokens;
    tokens.set(result);
    isAuthLoading.set(false);
  });

  return result;
};

export let getUser = () => {
  let result;
  vanillaOidc.tryKeepExistingSessionAsync().then(() => {
    vanillaOidc.userInfoAsync().then((resp) => {
      result = resp;
      userInfo.set(result);
    });
  });

  return result;
};

export const loginAsync = async () => {
  await vanillaOidc.loginAsync("/");
};

export const logoutAsync = async () => {
  await vanillaOidc.logoutAsync();
};

