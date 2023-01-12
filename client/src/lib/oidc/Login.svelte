<script>
  // Nav.svelte
  import { VanillaOidc } from "@axa-fr/vanilla-oidc";
  import { tokens } from "../../store.js";
  import { configuration } from "../../oidcConf.js";

  const vanillaOidc = VanillaOidc.getOrCreate(configuration);
</script>

{#if !$tokens}
  <button
    class="btn btn-primary"
    on:click={async () => vanillaOidc.loginAsync("/")}
  >
    Login
  </button>
{/if}
