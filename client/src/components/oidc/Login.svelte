<script>
  // Login.svelte
  import { userInfo } from "../../lib/store.js";
  import { loginAsync } from "../../lib/oidc.js";
  import Logo from "../common/Logo.svelte";
</script>

{#if !$userInfo}
  <main class="container text-center">
    <div class="row">
      <div class="col">
        <Logo />
      </div>
      <div class="col">
        <h1 class="title">svelte-netcore-identity</h1>
      </div>
      <div class="col">
        <button
          class="btn btn-primary"
          on:click={async () => await loginAsync()}
        >
          Login
        </button>
      </div>
    </div>
  </main>
{/if}
