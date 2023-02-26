<script>
  // Logout.svelte
  import { userInfo } from "../../lib/store.js";
  import { logoutAsync } from "../../lib/oidc.js";
</script>

{#if $userInfo}
  <button class="btn btn-danger" on:click={async () => await logoutAsync()}>
    <span>
      Logout &nbsp;

      <span class="badge bg-primary">
        {$userInfo.name}
      </span>
    </span>
  </button>
{/if}
