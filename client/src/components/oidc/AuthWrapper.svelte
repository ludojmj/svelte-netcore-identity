<script>
  // AuthWrapper.svelte
  import { isAuthLoading, isLoading, tokens } from "../../lib/store.js";
  import { getToken } from "../../lib/oidc.js";
  import Loading from "../common/Loading.svelte";
  import Login from "./Login.svelte";

  getToken();
</script>

<main class="container">
  <Login />
  {#if $isAuthLoading}
    <Loading />
  {:else if $tokens}
    <slot />
    {#if !$isLoading}
      <pre>{JSON.stringify($tokens, null, "\t")}</pre>
    {/if}
  {:else}
    <div class="alert alert-warning">
      You need to login to access this site.
    </div>
  {/if}
</main>
