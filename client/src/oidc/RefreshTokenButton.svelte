<script>
  import { getContext } from "svelte";
  import {
    OIDC_CONTEXT_CLIENT_PROMISE,
    refreshToken,
  } from "./OidcContext.svelte";
  const oidcPromise = getContext(OIDC_CONTEXT_CLIENT_PROMISE);
</script>

<button
  type="button"
  class="btn btn-warning"
  on:click|preventDefault={() => refreshToken(oidcPromise)}
>
  <slot />
</button>
