<script>
  // Nav.svelte
  import { VanillaOidc } from "@axa-fr/vanilla-oidc";
  import { tokens } from "../../store.js";
  import { configuration } from "../../oidcConf.js";

  const vanillaOidc = VanillaOidc.getOrCreate(configuration);
</script>

{#if $tokens}
  <button class="btn btn-dark" on:click={() => vanillaOidc.logoutAsync()}>
    <span class="badge bg-primary">{$tokens.accessTokenPayload.name}</span> Logout
  </button>
{/if}
